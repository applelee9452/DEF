using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DEF.UCenter;

public class ContainerStatelessAccountTaptap : ContainerStateless, IContainerStatelessAccountTaptap
{
	DbClientMongo Db { get; set; }
	IHttpClientFactory HttpClientFactory { get; set; }
	IOptions<UCenterOptions> UCenterOptions { get; set; }

	public override Task OnCreate()
	{
		Db = UCenterContext.Instance.Db;
		HttpClientFactory = UCenterContext.Instance.HttpClientFactory;
		UCenterOptions = UCenterContext.Instance.UCenterOptions;

		return Task.CompletedTask;
	}

	public override Task OnDestroy()
	{
		return Task.CompletedTask;
	}

	async Task<AccountLoginResponse> IContainerStatelessAccountTaptap.TaptapLoginRequest(AccountTaptapLoginRequest request, string client_ip)
	{
		AccountLoginResponse response = new()
		{
			ErrorCode = UCenterErrorCode.Error,
		};

		if (request == null)
		{
			Logger.LogInformation("ContainerAccount.TaptapLoginRequest() Error response==null");

			response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
			goto End;
		}

		if (string.IsNullOrEmpty(request.AppId))
		{
			Logger.LogInformation("ContainerAccount.TaptapLoginRequest() Error request.AppId==string.Empty");

			response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
			goto End;
		}

		bool b = EnsureDeviceInfo(request.Device);
		if (!b)
		{
			Logger.LogInformation("ContainerAccount.TaptapLoginRequest() Error EnsureDeviceInfo");

			response.ErrorCode = UCenterErrorCode.DeviceInfoNull;
			goto End;
		}

		ConfigApp app = UCenterContext.Instance.GetAppEntityByTaptapAppId(request.AppId);
		if (app == null)
		{
			Logger.LogInformation("ContainerAccount.TaptapLoginRequest() Error GetAppEntityByTaptapAppId");

			response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
			goto End;
		}

		string client_id = "hrz1lohnaddh5htxtr";
		string kid = request.Token;
		string mac_key = request.MacKey;
		string method = "GET";
		string request_url = "https://openapi.taptap.com/account/profile/v1?client_id=" + client_id;
		string authorization = GetAuthorization(request_url, method, kid, mac_key);

		TaptapProfileResponse taptap_profile_response = null;

		try
		{
			using var http_client = HttpClientFactory.CreateClient();

			using var http_request = new HttpRequestMessage(HttpMethod.Get, request_url);
			http_request.Headers.Add("Authorization", authorization);

			using HttpResponseMessage http_response = await http_client.SendAsync(http_request);

			if (http_response.StatusCode != HttpStatusCode.OK)
			{
				Logger.LogInformation("ContainerAccount.TaptapLoginRequest() Error openapi.taptap.com");

				response.ErrorCode = UCenterErrorCode.AccountTokenUnauthorized;
				goto End;
			}

			using (Stream st = http_response.Content.ReadAsStream())
			{
				using StreamReader reader = new(st, Encoding.GetEncoding("utf-8"));
				string str = reader.ReadToEnd();

				Logger.LogInformation(str);

				taptap_profile_response = Newtonsoft.Json.JsonConvert.DeserializeObject<TaptapProfileResponse>(str);
			}
		}
		catch (Exception ex)
		{
			Logger.LogError(ex.ToString());

			response.ErrorCode = UCenterErrorCode.AccountTokenUnauthorized;
			goto End;
		}

		if (taptap_profile_response == null || !taptap_profile_response.Success)
		{
			Logger.LogInformation("ContainerAccount.TaptapLoginRequest() Error taptap_profile_response");

			response.ErrorCode = UCenterErrorCode.AccountTokenUnauthorized;
			goto End;
		}

		bool need_update_nickname = false;
		bool need_update_icon = false;
		string unionid = taptap_profile_response.Data.Unionid;
		string openid = taptap_profile_response.Data.Openid;
		string nickname = taptap_profile_response.Data.Name;
		string headimgurl = taptap_profile_response.Data.Avatar;
		var device = request.Device;

		// 查找AccountTaptap
		var acc_taptap = await Db.ReadAsync<DataAccountTaptap>(
			a => a.Unionid == unionid
				 && a.OpenId == openid
				 && a.AppId == app.TaptapAppId,
			StringDef.DbCollectionDataAccountTaptap);

		// 创建AccountTaptap
		if (acc_taptap == null)
		{
			acc_taptap = new DataAccountTaptap()
			{
				Id = Guid.NewGuid().ToString(),
				AccountId = Guid.NewGuid().ToString(),
				Unionid = unionid,
				OpenId = openid,
				AppId = app.TaptapAppId,
				NickName = nickname,
				Gender = GenderType.Unknow,
				Province = string.Empty,
				City = string.Empty,
				Country = string.Empty,
				Headimgurl = headimgurl
			};

			await Db.InsertAsync(StringDef.DbCollectionDataAccountTaptap, acc_taptap);

			need_update_nickname = true;
			need_update_icon = true;
		}
		else
		{
			if (acc_taptap.Headimgurl != headimgurl)
			{
				acc_taptap.Headimgurl = headimgurl;
				need_update_icon = true;
			}

			if (acc_taptap.NickName != nickname)
			{
				acc_taptap.NickName = nickname;
				need_update_nickname = true;
			}

			if (need_update_icon || need_update_nickname)
			{
				await Db.ReplaceOneData(StringDef.DbCollectionDataAccountTaptap, acc_taptap.Id, acc_taptap);
			}
		}

		// 查找Account
		var acc = await Db.ReadAsync<DataAccount>(a => a.Id == acc_taptap.AccountId, StringDef.DbCollectionDataAccount);

		// 创建Account
		if (acc == null)
		{
			acc = new DataAccount()
			{
				Id = acc_taptap.AccountId,
				AccountName = openid,
				AccountType = AccountType.NormalAccount,
				AccountStatus = AccountStatus.Active,
				Password = Guid.NewGuid().ToString(),
				SuperPassword = Guid.NewGuid().ToString(),
				Token = Guid.NewGuid().ToString(),
				Gender = acc_taptap.Gender,
				Identity = string.Empty,
				PhoneCode = string.Empty,
				PhoneNumber = string.Empty,
				Email = string.Empty
			};

			await Db.InsertAsync(
				StringDef.DbCollectionDataAccount, acc);

			need_update_nickname = true;
			need_update_icon = true;
		}

		// Taptap头像覆盖Acc头像
		if (acc.ProfileImage != headimgurl)
		{
			acc.ProfileImage = headimgurl;

			await Db.ReplaceOneData(StringDef.DbCollectionDataAccount, acc.Id, acc);
		}

		string current_nickname = string.Empty;
		var data_id = GetAppAccountDataId(app.Id, acc.Id);
		var account_data = await Db.ReadAsync<DataAccountAppData>(
			a => a.Id == data_id,
			StringDef.DbCollectionDataAccountAppData);

		if (account_data != null)
		{
			var m = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(account_data.Data);

			if (m.ContainsKey("nick_name"))
			{
				current_nickname = m["nick_name"];
			}

			// Taptap昵称覆盖Acc昵称
			if (current_nickname != acc_taptap.NickName && !string.IsNullOrEmpty(acc_taptap.NickName))
			{
				m["nick_name"] = acc_taptap.NickName;
				account_data.Data = Newtonsoft.Json.JsonConvert.SerializeObject(m);

				await Db.ReplaceOneData(
					StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);
			}
		}
		else
		{
			Dictionary<string, string> m = new()
			{
				{ "nick_name", acc_taptap.NickName }
			};

			account_data = new DataAccountAppData
			{
				Id = data_id,
				AppId = app.Id,
				AccountId = acc.Id,
				Data = Newtonsoft.Json.JsonConvert.SerializeObject(m)
			};

			await Db.ReplaceOneData(
				StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);
		}

		string device_id = string.Empty;
		if (device != null && !string.IsNullOrEmpty(device.Id))
		{
			device_id = device.Id;
		}

		if (acc.AccountStatus == AccountStatus.Disabled)
		{
			await TraceAccountErrorAsync(acc, client_ip, UCenterErrorCode.AccountDisabled, "The account is disabled");

			response.ErrorCode = UCenterErrorCode.AccountDisabled;
			goto End;
		}

		acc.Token = Guid.NewGuid().ToString();
		acc.LastLoginDateTime = DateTime.UtcNow;
		acc.LastLoginClientIp = client_ip;
		acc.LastLoginDeviceId = device_id;

		var filter = Builders<DataAccount>.Filter.Where(e => e.Id == acc.Id);
		var update = Builders<DataAccount>.Update
			.Set(x => x.Token, acc.Token)
			.Set(x => x.LastLoginDateTime, acc.LastLoginDateTime)
			.Set(x => x.LastLoginClientIp, acc.LastLoginClientIp)
			.Set(x => x.LastLoginDeviceId, acc.LastLoginDeviceId);
		await Db.UpdateOneAsync(filter, StringDef.DbCollectionDataAccount, update);

		if (device != null && !string.IsNullOrEmpty(device.Id))
		{
			await LogDeviceInfo(device);
		}

		if (acc.AccountStatus == AccountStatus.Disabled)
		{
			await TraceAccountErrorAsync(acc.AccountName, client_ip, UCenterErrorCode.AccountDisabled);

			Logger.LogInformation("ContainerAccount.TaptapLoginRequest() Error AccountDisabled");
			response.ErrorCode = UCenterErrorCode.AccountDisabled;
			goto End;
		}

		await TraceAccountEvent(acc, "TaptapLogin", null);

		response.ErrorCode = UCenterErrorCode.NoError;
		response.AccountId = acc.Id;
		response.AccountName = acc.AccountName;
		response.AccountType = acc.AccountType;
		response.AccountStatus = acc.AccountStatus;
		response.Name = acc.Name;
		response.ProfileImage = acc.ProfileImage;
		response.ProfileThumbnail = acc.ProfileThumbnail;
		response.Gender = acc.Gender;
		response.Identity = acc.Identity;
		response.PhoneCode = acc.PhoneCode;
		response.PhoneNumber = acc.PhoneNumber;
		response.Email = acc.Email;
		response.Token = acc.Token;
		response.LastLoginDateTime = acc.LastLoginDateTime;

	End:
		Logger.LogInformation("ContainerAccount.TaptapLoginRequest() Result={0}", response.ErrorCode);

		return response;
	}

	// 时间戳转换为时间
	//public DateTime _stampToDateTime(string timeStamp)
	//{
	//    DateTime dateTimeStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
	//    long lTime = long.Parse(timeStamp + "0000000");
	//    TimeSpan toNow = new(lTime);
	//    return dateTimeStart.Add(toNow);
	//}

	async Task TraceAccountErrorAsync(DataAccount account, string client_ip, UCenterErrorCode code, string message = null)
	{
		var account_error_event = new EvAccountError()
		{
			Id = Guid.NewGuid().ToString(),
			Code = code,
			ClientIp = client_ip,
			LoginArea = string.Empty,
			Message = message
		};

		if (account != null)
		{
			account_error_event.AccountName = account.AccountName;
			account_error_event.AccountId = account.Id;
		}

		await Db.InsertAsync(StringDef.DBCollectionEvAccountError, account_error_event);
	}

	async Task TraceAccountErrorAsync(string account_name, string client_ip, UCenterErrorCode code)
	{
		var account_error_event = new EvAccountError()
		{
			Id = Guid.NewGuid().ToString(),
			AccountName = account_name,
			Code = code,
			ClientIp = client_ip,
			LoginArea = string.Empty,
			Message = string.Empty
		};

		await Db.InsertAsync(StringDef.DBCollectionEvAccountError, account_error_event);
	}

	async Task TraceAccountEvent(DataAccount account, string client_ip, string event_name, DeviceInfo device = null)
	{
		var ev_account_entity = new EvAccount
		{
			Id = Guid.NewGuid().ToString(),
			EventName = event_name,
			ClientIp = client_ip,
			LoginArea = string.Empty,
			UserAgent = string.Empty, // todo: Migrate to asp.net core Request.Headers.UserAgent.ToString(),
			Message = string.Empty
		};

		if (account != null)
		{
			ev_account_entity.AccountName = account.AccountName;
			ev_account_entity.AccountId = account.Id;
		}

		if (device != null)
		{
			ev_account_entity.DeviceId = device.Id;
		}

		await Db.InsertAsync(StringDef.DBCollectionEvAccount, ev_account_entity);
	}

	bool EnsureDeviceInfo(DeviceInfo device)
	{
		if (device == null)
		{
			return false;
		}

		if (string.IsNullOrEmpty(device.Id))
		{
			return false;
		}

		return true;
	}

	async Task LogDeviceInfo(DeviceInfo device)
	{
		if (device == null) return;

		var device_entity = await Db.ReadAsync<DataDevice>(
			d => d.Id == device.Id,
			StringDef.DbCollectionDataDevice);
		if (device_entity == null)
		{
			device_entity = new DataDevice()
			{
				Id = device.Id,
				Name = device.Name,
				Type = device.Type,
				Model = device.Model,
				OperationSystem = device.OperationSystem
			};

			await Db.InsertAsync(
				StringDef.DbCollectionDataDevice, device_entity);
		}
		else
		{
			var filterDefinition = Builders<DataDevice>.Filter.Where(e => e.Id == device_entity.Id);
			var updateDefinition = Builders<DataDevice>.Update.Set(e => e.UpdatedTime, DateTime.UtcNow);
			await Db.UpdateOneAsync(filterDefinition, StringDef.DbCollectionDataDevice, updateDefinition);
		}
	}

	static string GetAppAccountDataId(string app_id, string account_id)
	{
		return $"{app_id}_{account_id}";
	}

	static string GetAuthorization(string request_url, string method, string key_id, string mac_key)
	{
		try
		{
			Uri url = new(request_url);
			string time = String.Format("{0:D10}", DateTimeOffset.Now.ToUnixTimeSeconds());
			string randomStr = GetRandomString(16);
			string host = url.Host;
			string uri = request_url.Substring(request_url.IndexOf(host) + host.Length);
			string port = (url.Scheme == "https") ? "443" : "80";
			string other = "";
			string sign = Sign(MergeSign(time, randomStr, method, uri, host, port, other), mac_key);

			return "MAC " + GetAuthorizationParam("id", key_id) + "," + GetAuthorizationParam("ts", time) + "," + GetAuthorizationParam("nonce", randomStr) + "," + GetAuthorizationParam("mac", sign);
		}
		catch (UriFormatException)
		{
			//Console.WriteLine(ex.Message);
			return null;
		}
	}

	static string GetRandomString(int length)
	{
		byte[] bytes = new byte[length];
		new Random().NextBytes(bytes);
		string base64String = Convert.ToBase64String(bytes);
		return base64String;
	}

	static string MergeSign(string time, string randomCode, string httpType, string uri, string domain, string port, string other)
	{
		if (string.IsNullOrEmpty(time) || string.IsNullOrEmpty(randomCode) || string.IsNullOrEmpty(httpType) ||
			string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(port))
		{
			return null;
		}
		string prefix = time + "\n" + randomCode + "\n" + httpType + "\n" + uri + "\n" + domain + "\n" + port + "\n";
		if (string.IsNullOrEmpty(other))
		{
			prefix += "\n";
		}
		else
		{
			prefix += (other + "\n");
		}
		return prefix;
	}

	static string Sign(string signatureBaseString, string key)
	{
		try
		{
			byte[] keyBytes = Encoding.UTF8.GetBytes(key);
			HMACSHA1 hmacsha1 = new HMACSHA1(keyBytes);
			byte[] text = Encoding.UTF8.GetBytes(signatureBaseString);
			byte[] signatureBytes = hmacsha1.ComputeHash(text);
			string signature = Convert.ToBase64String(signatureBytes);
			return signature;
		}
		catch (Exception e)
		{
			throw new Exception(e.Message);
		}
	}

	static string GetAuthorizationParam(string key, string value)
	{
		if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
		{
			return null;
		}

		return key + "=" + "\"" + value + "\"";
	}
}