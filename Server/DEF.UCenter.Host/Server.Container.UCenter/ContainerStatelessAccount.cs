using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DEF.Cloud;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace DEF.UCenter;

public class ContainerStatelessAccount : ContainerStateless, IContainerStatelessAccount
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    IOptions<UCenterOptions> UCenterOptions { get; set; }
    Random Rd { get; set; } = new(Guid.NewGuid().GetHashCode());

    const string EnjoyAppId = "";
    const string EnjoyClientSecret = "";

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

    // 请求账号登陆
    async Task<AccountLoginResponse> IContainerStatelessAccount.LoginRequest(AccountLoginInfo request, string client_ip)
    {
        var response = new AccountLoginResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null)
        {
            // 请求参数无效
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.AccountName) && request.AccountName.Length > 24)
        {
            response.ErrorCode = UCenterErrorCode.InvalidAccountName;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.PhoneCode) && request.PhoneCode.Length > 24)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber.Length > 24)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.Email) && request.Email.Length > 256)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.Password) && request.Password.Length > 64)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.PhoneVerificationCode) && request.PhoneVerificationCode.Length > 24)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (request.Device != null)
        {
            if (request.Device.Id.Length > 256)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }

            if (request.Device.Name.Length > 256)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }

            if (request.Device.Type.Length > 128)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }

            if (request.Device.Model.Length > 128)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }

            if (request.Device.OperationSystem.Length > 128)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }
        }

        DataAccount account_entity = null;

        if (!string.IsNullOrEmpty(request.AccountName))
        {
            account_entity = await Db.ReadAsync<DataAccount>(
                a => a.AccountName == request.AccountName,
                StringDef.DbCollectionDataAccount);
        }
        else if (!string.IsNullOrEmpty(request.PhoneCode) && !string.IsNullOrEmpty(request.PhoneNumber))
        {
            account_entity = await Db.ReadAsync<DataAccount>(
                a => a.PhoneCode == request.PhoneCode && a.PhoneNumber == request.PhoneNumber,
                StringDef.DbCollectionDataAccount);
        }
        else if (!string.IsNullOrEmpty(request.Email))
        {
            account_entity = await Db.ReadAsync<DataAccount>(
                a => a.Email == request.Email,
                StringDef.DbCollectionDataAccount);
        }

        if (account_entity == null)
        {
            await TraceAccountErrorAsync(request.AccountName, client_ip, UCenterErrorCode.AccountNotExist);

            // 账号不存在
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        if (account_entity.AccountStatus == AccountStatus.Disabled)
        {
            await TraceAccountErrorAsync(request.AccountName, client_ip, UCenterErrorCode.AccountDisabled);

            // 账号被禁用
            response.ErrorCode = UCenterErrorCode.AccountDisabled;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            if (!EncryptHelper.VerifyHash(request.Password, account_entity.Password))
            {
                await TraceAccountErrorAsync(account_entity, client_ip,
                    UCenterErrorCode.AccountPasswordUnauthorized, "The account name and password do not match");

                // 密码不正确
                response.ErrorCode = UCenterErrorCode.AccountPasswordUnauthorized;
                goto End;
            }
        }
        else if (!string.IsNullOrEmpty(request.PhoneVerificationCode))
        {
            CachePhoneVerificationCode entity = await Db.ReadAsync<CachePhoneVerificationCode>(
                a => a.Id == request.PhoneCode + request.PhoneNumber, StringDef.DbCollectionCachePhoneVerificationCode);
            if (entity == null || entity.VerificationCode != request.PhoneVerificationCode)
            {
                await TraceAccountErrorAsync(account_entity, client_ip,
                    UCenterErrorCode.AccountPasswordUnauthorized, "The account phone and phoneverificationcode do not match");

                // 手机验证码不正确
                response.ErrorCode = UCenterErrorCode.PhoneVerificationCodeError;
                goto End;
            }
        }
        else
        {
            await TraceAccountErrorAsync(account_entity, client_ip,
                UCenterErrorCode.AccountPasswordUnauthorized, "The account password and phoneverificationcode is null");

            // 其他验证错误
            response.ErrorCode = UCenterErrorCode.AccountPasswordUnauthorized;
            goto End;
        }

        string device_id = string.Empty;
        if (request.Device != null && !string.IsNullOrEmpty(request.Device.Id))
        {
            device_id = request.Device.Id;
        }
        await _accountLoginAsync(device_id, account_entity, client_ip);

        if (request.Device != null && !string.IsNullOrEmpty(request.Device.Id))
        {
            await LogDeviceInfo(request.Device);
        }

        await TraceAccountEvent(account_entity, client_ip, "Login", request.Device);

        // 登陆成功
        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation($"GrainAccountService.LoginRequest() PhoneCode={request.PhoneCode} PhoneNumber={request.PhoneNumber} Result={response.ErrorCode}");

        return response;
    }

    // 请求获取手机验证码
    async Task<UCenterErrorCode> IContainerStatelessAccount.GetPhoneVerificationCodeRequest(GetPhoneVerificationCodeRequest request, string client_ip)
    {
        UCenterErrorCode response;

        if (request == null)
        {
            response = UCenterErrorCode.ClientError;
            goto End;
        }

        // 验证手机号码有效性
        bool b = ValidatePhone(request.PhoneCode, request.PhoneNumber);
        if (!b)
        {
            response = UCenterErrorCode.InvalidAccountPhone;
            goto End;
        }

        // 生成验证码
        int verification_code = Rd.Next(1000, 10000);

        // 保存到Db
        CachePhoneVerificationCode entity = new()
        {
            Id = request.PhoneCode + request.PhoneNumber,
            PhoneCode = request.PhoneCode,
            PhoneNumber = request.PhoneNumber,
            VerificationCode = verification_code.ToString(),
            UpdatedTime = DateTime.UtcNow
        };

        await Db.ReplaceOneData(
                StringDef.DbCollectionCachePhoneVerificationCode, entity.Id, entity);

        var r = await Cloud.Service.Sms.SendVerificationCode(request.PhoneCode, request.PhoneNumber, entity.VerificationCode);

        if (r == Cloud.Result.Success) response = UCenterErrorCode.NoError;
        else response = UCenterErrorCode.Error;

        End:

        Logger.LogInformation("GrainAccountService.GetPhoneVerificationCodeRequest() PhoneCode={PhoneCode} PhoneNumber={PhoneNumber} Result={response}",
            request.PhoneCode, request.PhoneNumber, response);

        return response;
    }

    // 请求注册账号
    async Task<AccountRegisterResponse> IContainerStatelessAccount.RegisterRequest(AccountRegisterRequestInfo request, ulong agent_id, string client_ip)
    {
        var response = new AccountRegisterResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        // 检测注册信息合法性
        var r = ValidateAccount(request);
        if (r != UCenterErrorCode.NoError)
        {
            response.ErrorCode = r;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.AppId) && request.AppId.Length > 64)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.SuperPassword) && request.SuperPassword.Length > 64)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.Name) && request.Name.Length > 64)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.Identity) && request.Identity.Length > 64)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (!string.IsNullOrEmpty(request.PhoneVerificationCode) && request.PhoneVerificationCode.Length > 12)
        {
            response.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (request.Device != null)
        {
            if (request.Device.Id.Length > 256)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }

            if (request.Device.Name.Length > 256)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }

            if (request.Device.Type.Length > 128)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }

            if (request.Device.Model.Length > 128)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }

            if (request.Device.OperationSystem.Length > 128)
            {
                response.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }
        }

        // 检测帐号名是否已经被注册
        if (!string.IsNullOrEmpty(request.AccountName))
        {
            var entity = await Db.ReadAsync<DataAccount>(
                a => a.AccountName == request.AccountName,
                StringDef.DbCollectionDataAccount);
            if (entity != null)
            {
                response.ErrorCode = UCenterErrorCode.AccountNameAlreadyExist;
                goto End;
            }
        }
        else if (!string.IsNullOrEmpty(request.PhoneCode) && !string.IsNullOrEmpty(request.PhoneNumber))
        {
            var entity = await Db.ReadAsync<DataAccount>(
                a => a.PhoneCode == request.PhoneCode && a.PhoneNumber == request.PhoneNumber,
                StringDef.DbCollectionDataAccount);
            if (entity != null)
            {
                response.ErrorCode = UCenterErrorCode.AccountNameAlreadyExist;
                goto End;
            }
        }
        else if (!string.IsNullOrEmpty(request.Email))
        {
            var entity = await Db.ReadAsync<DataAccount>(
                a => a.Email == request.Email,
                StringDef.DbCollectionDataAccount);
            if (entity != null)
            {
                response.ErrorCode = UCenterErrorCode.AccountNameAlreadyExist;
                goto End;
            }
        }

        // 如果是手机号注册，则需要检测验证码是否匹配
        if (!string.IsNullOrEmpty(request.PhoneCode) && !string.IsNullOrEmpty(request.PhoneNumber))
        {
            CachePhoneVerificationCode entity = await Db.ReadAsync<CachePhoneVerificationCode>(
                a => a.Id == request.PhoneCode + request.PhoneNumber, StringDef.DbCollectionCachePhoneVerificationCode);
            if (entity == null || entity.VerificationCode != request.PhoneVerificationCode)
            {
                response.ErrorCode = UCenterErrorCode.PhoneVerificationCodeError;
                goto End;
            }
        }

        // 检查Device是否绑定了游客号
        DataAccount account_entity = null;
        if (request != null
            && !string.IsNullOrEmpty(request.AppId)
            && request.Device != null
            && !string.IsNullOrEmpty(request.Device.Id))
        {
            string guest_deviceid = $"{request.AppId}_{request.Device.Id}";
            var guest_device_entity = await Db.ReadAsync<DataDeviceGuest>(
                a => a.Id == guest_deviceid,
                StringDef.DbCollectionDataDeviceGuest);

            if (guest_device_entity != null)
            {
                account_entity = await Db.ReadAsync<DataAccount>(
                    a => a.Id == guest_device_entity.AccountId,
                    StringDef.DbCollectionDataAccount);
            }
        }

        bool guest_convert = true;
        if (account_entity == null)
        {
            guest_convert = false;

            // 没有绑定游客号，正常注册
            account_entity = new DataAccount
            {
                Id = Guid.NewGuid().ToString(),
            };
        }

        account_entity.AccountName = request.AccountName;
        account_entity.AccountType = AccountType.NormalAccount;
        account_entity.AccountStatus = AccountStatus.Active;
        account_entity.Name = request.Name;
        account_entity.Identity = request.Identity;
        account_entity.Password = EncryptHelper.ComputeHash(request.Password);
        //account_entity.SuperPassword = EncryptHelper.ComputeHash(request.SuperPassword);
        account_entity.PhoneCode = request.PhoneCode;
        account_entity.PhoneNumber = request.PhoneNumber;
        account_entity.Email = request.Email;
        account_entity.Gender = request.Gender;
        account_entity.LastLoginClientIp = client_ip;
        account_entity.LastLoginDateTime = DateTime.UtcNow;
        account_entity.LastLoginDeviceId = request.Device != null ? request.Device.Id : string.Empty;

        // 是通过代理推广注册的用户

        if (agent_id != 0)
        {
            var data_agent = await Db.ReadAsync<DataAgent>(e => e.AgentId == agent_id, StringDef.DbCollectionDataAgent);
            if (data_agent != null && !data_agent.IsDelete)
            {
                account_entity.AgentId = agent_id;

                ulong[] arr = null;
                if (data_agent.AgentParents != null && data_agent.AgentParents.Length > 0)
                {
                    arr = new ulong[1 + data_agent.AgentParents.Length];
                    arr[0] = data_agent.AgentId;
                    data_agent.AgentParents.CopyTo(arr, 1);
                }
                else
                {
                    arr = new ulong[1];
                    arr[0] = data_agent.AgentId;
                }
                account_entity.AgentParents = arr;
            }
        }

        if (guest_convert)
        {
            // 绑定了游客号，游客号转正
            await Db.ReplaceOneData(StringDef.DbCollectionDataAccount, account_entity.Id, account_entity);

            try
            {
                var builder = Builders<DataDeviceGuest>.Filter.Where(d => d.AppId == request.AppId && d.AccountId == account_entity.Id);
                await Db.DeleteOneAsync(StringDef.DbCollectionDataDeviceGuest, builder);
            }
            catch (Exception)
            {
            }

            await TraceAccountEvent(account_entity, client_ip, "GuestConvert");
        }
        else
        {
            // 没有绑定游客号，正常注册

            await Db.InsertAsync(StringDef.DbCollectionDataAccount, account_entity);

            await TraceAccountEvent(account_entity, client_ip, "Register", request.Device);
        }

        if (request.Device != null)
        {
            await LogDeviceInfo(request.Device);
        }

        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation($"GrainAccountService.RegisterRequest() Result={response.ErrorCode}");

        return response;
    }

    // 请求微信自动登陆
    async Task<AccountLoginResponse> IContainerStatelessAccount.WechatAutoLoginRequest(AccountWechatAutoLoginRequest request, string client_ip)
    {
        var response = new AccountLoginResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null || string.IsNullOrEmpty(request.AppId) || string.IsNullOrEmpty(request.OpenId))
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        ConfigApp app = UCenterContext.Instance.GetAppEntityByWechatAppId(request.AppId);
        if (app == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        CacheWechatAccessToken entity_accesstoken = await Db.ReadAsync<CacheWechatAccessToken>(
            a => a.AppId == request.AppId && a.OpenId == request.OpenId,
            StringDef.DbCollectionCacheWechatAccessToken);
        if (entity_accesstoken == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        OAuthAccessTokenResult access_token_result = await OAuthApi.RefreshTokenAsync(entity_accesstoken.AppId, entity_accesstoken.RefreshToken);
        if (access_token_result == null || access_token_result.errcode != 0)
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        DataAccount account_entity = await _wechatLoginAsync(app, request.Device, access_token_result, client_ip);
        if (account_entity == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        if (account_entity.AccountStatus == AccountStatus.Disabled)
        {
            await TraceAccountErrorAsync(account_entity.AccountName, client_ip, UCenterErrorCode.AccountDisabled);

            response.ErrorCode = UCenterErrorCode.AccountDisabled;
            goto End;
        }

        await TraceAccountEvent(account_entity, "WechatAutoLogin", null);

        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation("GrainAccountService.WechatAutoLoginRequest() Result={ErrorCode}", response.ErrorCode);

        return response;
    }

    // 请求微信登陆
    async Task<AccountLoginResponse> IContainerStatelessAccount.WechatLoginRequest(AccountWechatOAuthInfo request, string client_ip)
    {
        var response = new AccountLoginResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null || string.IsNullOrEmpty(request.AppId) || string.IsNullOrEmpty(request.Code))
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        ConfigApp app = UCenterContext.Instance.GetAppEntityByWechatAppId(request.AppId);
        if (app == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        OAuthAccessTokenResult access_token_result;
        try
        {
            access_token_result = await OAuthApi.GetAccessTokenAsync(app.WechatAppId, app.WechatAppSecret, request.Code);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());

            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        if (access_token_result == null || access_token_result.errcode != 0)
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        DataAccount account_entity = await _wechatLoginAsync(app, request.Device, access_token_result, client_ip);
        if (account_entity == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        if (account_entity.AccountStatus == AccountStatus.Disabled)
        {
            await TraceAccountErrorAsync(account_entity.AccountName, client_ip, UCenterErrorCode.AccountDisabled);

            response.ErrorCode = UCenterErrorCode.AccountDisabled;
            goto End;
        }

        await TraceAccountEvent(account_entity, "WechatLogin", null);

        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation("ContainerAccount.WechatLoginRequest() Result={0}", response.ErrorCode);

        return response;
    }

    // 请求微信公众号登陆
    async Task<AccountLoginResponse> IContainerStatelessAccount.WechatMPLoginRequest(AccountWechatOAuthInfo request, string client_ip)
    {
        var response = new AccountLoginResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null || string.IsNullOrEmpty(request.AppId) || string.IsNullOrEmpty(request.Code))
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        ConfigApp app = UCenterContext.Instance.GetAppEntityByWechatAppId(request.AppId);
        if (app == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        string app_id = request.AppId;
        var appid = app.WechatMpAppId;
        var screct = app.WechatMpAppSecret;
        var uri = $"https://api.weixin.qq.com/sns/jscode2session?appid={appid}&secret={screct}&js_code={request.Code}&grant_type=authorization_code";

        try
        {
            using HttpClient hc = HttpClientFactory.CreateClient();

            using var req = new HttpRequestMessage(HttpMethod.Post, uri);

            using var res = await hc.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                string wechat_response_str = await res.Content.ReadAsStringAsync();

                var wechat_response = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountWechatAutoLoginResponse>(wechat_response_str);

                if (wechat_response == null || wechat_response.errcode != 0)
                {
                    response.ErrorCode = UCenterErrorCode.AccountPasswordUnauthorized;
                    goto End;
                }

                // 下面开始保存Db

                // 查找AccountEnjoy
                var acc_wechat = await Db.ReadAsync<DataAccountWechat>(
                    a => a.AppId == app_id && a.OpenId == wechat_response.openid,
                    StringDef.DbCollectionDataAccountWechat);

                // 创建AccountEnjoy
                if (acc_wechat == null)
                {
                    acc_wechat = new DataAccountWechat()
                    {
                        Id = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                        AppId = app_id,
                        OpenId = wechat_response.openid,
                        Unionid = wechat_response.unionid,
                        AppType = DataAccountWechat.WXAppType.MP,
                    };

                    await Db.InsertAsync(StringDef.DbCollectionDataAccountWechat, acc_wechat);
                }

                // 查找Account
                var account_entity = await Db.ReadAsync<DataAccount>(
                    a => a.Id == acc_wechat.AccountId,
                    StringDef.DbCollectionDataAccount);

                // 创建Account
                if (account_entity == null)
                {
                    account_entity = new DataAccount()
                    {
                        Id = acc_wechat.AccountId,
                        AccountName = Guid.NewGuid().ToString(),
                        AccountType = AccountType.NormalAccount,
                        AccountStatus = AccountStatus.Active,
                        Password = Guid.NewGuid().ToString(),
                        SuperPassword = Guid.NewGuid().ToString(),
                        Token = Guid.NewGuid().ToString(),
                        Gender = GenderType.Unknow,
                        Identity = string.Empty,
                        PhoneCode = string.Empty,
                        PhoneNumber = string.Empty,
                    };

                    await Db.InsertAsync(StringDef.DbCollectionDataAccount, account_entity);
                }

                string device_id = string.Empty;
                if (request.Device != null && !string.IsNullOrEmpty(request.Device.Id))
                {
                    device_id = request.Device.Id;
                }
                await _accountLoginAsync(device_id, account_entity, client_ip);

                if (request.Device != null && !string.IsNullOrEmpty(request.Device.Id))
                {
                    await LogDeviceInfo(request.Device);
                }

                response.ErrorCode = UCenterErrorCode.NoError;
                response.AccountId = account_entity.Id;
                response.AccountName = account_entity.AccountName;
                response.AccountType = account_entity.AccountType;
                response.AccountStatus = account_entity.AccountStatus;
                response.Name = account_entity.Name;
                response.ProfileImage = account_entity.ProfileImage;
                response.ProfileThumbnail = account_entity.ProfileThumbnail;
                response.Gender = account_entity.Gender;
                response.Identity = account_entity.Identity;
                response.PhoneCode = account_entity.PhoneCode;
                response.PhoneNumber = account_entity.PhoneNumber;
                response.Email = account_entity.Email;
                response.Token = account_entity.Token;
                response.LastLoginDateTime = account_entity.LastLoginDateTime;
            }
            else
            {
                // Log Error
                Logger.LogError("GrainAccountService.WechatMPLoginRequest() HttpError，StatusCode={0}", res.StatusCode);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "GrainAccountService.WechatMPLoginRequest()");
        }

    End:
        Logger.LogInformation("ContainerAccount.WechatLoginRequest() Result={0}", response.ErrorCode);

        return response;
    }

    // 请求Facebook登陆
    async Task<AccountLoginResponse> IContainerStatelessAccount.FacebookLoginRequest(AccountFacebookOAuthInfo request, string client_ip)
    {
        var response = new AccountLoginResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null || string.IsNullOrEmpty(request.AccessToken) || string.IsNullOrEmpty(request.UserId))
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        var facebook_auth_info = await _getFacebookAuthInfo(request.AccessToken);

        // 有效用户
        if (!facebook_auth_info.IsValid)
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        DataAccount account_entity = await _facebookLoginAsync(facebook_auth_info, request.Device, client_ip);
        if (account_entity == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        if (account_entity.AccountStatus == AccountStatus.Disabled)
        {
            await TraceAccountErrorAsync(account_entity.AccountName, client_ip, UCenterErrorCode.AccountDisabled);

            response.ErrorCode = UCenterErrorCode.AccountDisabled;
            goto End;
        }

        await TraceAccountEvent(account_entity, client_ip, "FacebookLogin");

        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation("GrainAccountService.FacebookLoginRequest() Result={0}", response.ErrorCode);

        return response;
    }

    // 请求Google登陆
    async Task<AccountLoginResponse> IContainerStatelessAccount.GoogleLoginRequest(AccountGoogleOAuthInfo request, string client_ip)
    {
        var response = new AccountLoginResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null || string.IsNullOrEmpty(request.AccessToken))
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        //var google_auth_info = await _getGoogleAuthInfo(request.AccessToken);

        var verify = await _verifyGoogleLoginCode(request.AccessToken);
        if (!verify)
        {
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        var google_auth_info = new GoogleLoginAuthInfo
        {
            NickName = request.NickName,
            AppId = "com.XX.XXX",
            UserId = request.UserId,
            Email = ""
        };
        google_auth_info.NickName = request.NickName;
        google_auth_info.Headimgurl = request.Headimgurl;

        DataAccount account_entity = await _googleLoginAsync(google_auth_info, request.Device, client_ip);

        if (account_entity == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        if (account_entity.AccountStatus == AccountStatus.Disabled)
        {
            await TraceAccountErrorAsync(account_entity.AccountName, client_ip, UCenterErrorCode.AccountDisabled);

            response.ErrorCode = UCenterErrorCode.AccountDisabled;
            goto End;
        }

        await TraceAccountEvent(account_entity, client_ip, "GoogleLogin");

        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation("GrainAccountService.GoogleLoginRequest() Result={0}", response.ErrorCode);

        return response;
    }

    // 请求Enjoy登陆
    async Task<AccountLoginResponse> IContainerStatelessAccount.EnjoyLoginRequest(EnjoyLoginRequest request, string client_ip)
    {
        var response = new AccountLoginResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null || string.IsNullOrEmpty(request.Token))
        {
            goto End;
        }

        string app_id = "Vw.Lobby";
        string uri = "https://sdk.enjoy.link/enjoy-server/tokeninfo.gson?client_id={0}&&client_secret={1}&&token={2}";

        try
        {
            using HttpClient hc = HttpClientFactory.CreateClient();

            using var req = new HttpRequestMessage(HttpMethod.Post, string.Format(uri, EnjoyAppId, EnjoyClientSecret, request.Token));

            using var res = await hc.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                string enjoy_response_str = await res.Content.ReadAsStringAsync();

                var enjoy_response = Newtonsoft.Json.JsonConvert.DeserializeObject<EnjoyResponse>(enjoy_response_str);

                if (enjoy_response == null || enjoy_response.Code != "1000" || enjoy_response.Data == null)
                {
                    response.ErrorCode = UCenterErrorCode.AccountPasswordUnauthorized;
                    goto End;
                }

                // 下面开始保存Db

                // 查找AccountEnjoy
                var acc_enjoy = await Db.ReadAsync<DataAccountEnjoy>(
                    a => a.AppId2 == enjoy_response.Data.AppId && a.Unid == enjoy_response.Data.Details.Unid,
                    StringDef.DbCollectionDataAccountEnjoy);

                // 创建AccountEnjoy
                if (acc_enjoy == null)
                {
                    acc_enjoy = new DataAccountEnjoy()
                    {
                        Id = Guid.NewGuid().ToString(),
                        AccountId = Guid.NewGuid().ToString(),
                        AppId = app_id,
                        Market = enjoy_response.Data.Market,
                        AppId2 = enjoy_response.Data.AppId,
                        Unid = enjoy_response.Data.Details.Unid,
                        LoginType = enjoy_response.Data.Details.LoginType,
                        LoginId = enjoy_response.Data.Details.LoginId,
                        NickName = enjoy_response.Data.Details.NickName,
                        Email = enjoy_response.Data.Details.Email,
                        CountryCode = enjoy_response.Data.Details.CountryCode,
                        CreateTime = enjoy_response.Data.Details.CreateTime,
                        Paid = enjoy_response.Data.Details.Paid,
                        DeviceId = enjoy_response.Data.Details.DeviceId,
                    };

                    await Db.InsertAsync(StringDef.DbCollectionDataAccountEnjoy, acc_enjoy);
                }

                // 查找Account
                var account_entity = await Db.ReadAsync<DataAccount>(
                    a => a.Id == acc_enjoy.AccountId,
                    StringDef.DbCollectionDataAccount);

                // 创建Account
                if (account_entity == null)
                {
                    account_entity = new DataAccount()
                    {
                        Id = acc_enjoy.AccountId,
                        AccountName = Guid.NewGuid().ToString(),
                        AccountType = AccountType.NormalAccount,
                        AccountStatus = AccountStatus.Active,
                        Password = Guid.NewGuid().ToString(),
                        SuperPassword = Guid.NewGuid().ToString(),
                        Token = Guid.NewGuid().ToString(),
                        Gender = GenderType.Unknow,
                        Identity = string.Empty,
                        PhoneCode = string.Empty,
                        PhoneNumber = string.Empty,
                        Email = acc_enjoy.Email
                    };

                    await Db.InsertAsync(
                        StringDef.DbCollectionDataAccount, account_entity);
                }

                string device_id = string.Empty;
                if (request.Device != null && !string.IsNullOrEmpty(request.Device.Id))
                {
                    device_id = request.Device.Id;
                }

                await _accountLoginAsync(device_id, account_entity, client_ip);

                if (request.Device != null && !string.IsNullOrEmpty(request.Device.Id))
                {
                    await LogDeviceInfo(request.Device);
                }

                response.ErrorCode = UCenterErrorCode.NoError;
                response.AccountId = account_entity.Id;
                response.AccountName = account_entity.AccountName;
                response.AccountType = account_entity.AccountType;
                response.AccountStatus = account_entity.AccountStatus;
                response.Name = account_entity.Name;
                response.ProfileImage = account_entity.ProfileImage;
                response.ProfileThumbnail = account_entity.ProfileThumbnail;
                response.Gender = account_entity.Gender;
                response.Identity = account_entity.Identity;
                response.PhoneCode = account_entity.PhoneCode;
                response.PhoneNumber = account_entity.PhoneNumber;
                response.Email = account_entity.Email;
                response.Token = account_entity.Token;
                response.LastLoginDateTime = account_entity.LastLoginDateTime;
            }
            else
            {
                // Log Error
                Logger.LogError("GrainAccountService.EnjoyLoginRequest() HttpError，StatusCode={0}", res.StatusCode);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "GrainAccountService.EnjoyLoginRequest()");
        }

    End:

        Logger.LogInformation("GrainAccountService.EnjoyLoginRequest() Result={0}", response.ErrorCode);

        return response;
    }

    // 请求游客访问
    async Task<GuestAccessResponse> IContainerStatelessAccount.GuestAccessRequest(GuestAccessInfo request, string client_ip)
    {
        GuestAccessResponse result = new()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null)
        {
            result.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (string.IsNullOrEmpty(request.AppId))
        {
            result.ErrorCode = UCenterErrorCode.AppIdNull;
            goto End;
        }

        bool b = EnsureDeviceInfo(request.Device);
        if (!b)
        {
            result.ErrorCode = UCenterErrorCode.DeviceInfoNull;
            goto End;
        }

        if (!UCenterOptions.Value.EnableGuestAccess)
        {
            result.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        DataAccount account_entity = null;
        string guest_deviceid = $"{request.AppId}_{request.Device.Id}";

        var guest_device_entity = await Db.ReadAsync<DataDeviceGuest>(
            a => a.Id == guest_deviceid,
            StringDef.DbCollectionDataDeviceGuest);

        if (guest_device_entity == null)
        {
            account_entity = new DataAccount()
            {
                Id = Guid.NewGuid().ToString(),
                AccountName = Guid.NewGuid().ToString(),
                AccountType = AccountType.Guest,
                AccountStatus = AccountStatus.Active,
                Password = Guid.NewGuid().ToString(),
                SuperPassword = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString(),
                LastLoginClientIp = client_ip,
                LastLoginDateTime = DateTime.UtcNow,
                LastLoginDeviceId = request.Device != null ? request.Device.Id : string.Empty
            };

            await Db.InsertAsync(
                StringDef.DbCollectionDataAccount, account_entity);

            guest_device_entity = new DataDeviceGuest()
            {
                Id = $"{request.AppId}_{request.Device.Id}",
                AccountId = account_entity.Id,
                AppId = request.AppId,
                Device = request.Device
            };

            await Db.InsertAsync(
                StringDef.DbCollectionDataDeviceGuest, guest_device_entity);

            await TraceAccountEvent(account_entity, client_ip, "GuestRegister", request.Device);
        }
        else
        {
            account_entity = await Db.ReadAsync<DataAccount>(
                a => a.Id == guest_device_entity.AccountId,
                StringDef.DbCollectionDataAccount);

            if (account_entity.AccountStatus == AccountStatus.Disabled)
            {
                //await TraceAccountErrorAsync(request.AccountName, client_ip, UCenterErrorCode.AccountDisabled);

                // 账号被禁用
                result.ErrorCode = UCenterErrorCode.AccountDisabled;
                goto End;
            }

            account_entity.Token = Guid.NewGuid().ToString();
            account_entity.LastLoginDateTime = DateTime.UtcNow;
            account_entity.LastLoginClientIp = client_ip;
            account_entity.LastLoginDeviceId = request.Device != null ? request.Device.Id : string.Empty;

            var filter = Builders<DataAccount>.Filter.Where(e => e.Id == account_entity.Id);
            var update = Builders<DataAccount>.Update
                .Set(x => x.Token, account_entity.Token)
                .Set(x => x.LastLoginDateTime, account_entity.LastLoginDateTime)
                .Set(x => x.LastLoginClientIp, account_entity.LastLoginClientIp)
                .Set(x => x.LastLoginDeviceId, account_entity.LastLoginDeviceId);
            await Db.UpdateOneAsync(filter, StringDef.DbCollectionDataAccount, update);

            await TraceAccountEvent(account_entity, client_ip, "GuestLogin", request.Device);
        }

        await LogDeviceInfo(request.Device);

        result.ErrorCode = UCenterErrorCode.NoError;
        result.AccountId = account_entity.Id;
        result.AccountName = account_entity.AccountName;
        result.AccountType = account_entity.AccountType;
        result.Token = account_entity.Token;

    End:

        Logger.LogInformation("GrainAccountService.GuestAccessRequest() Result={0}", result.ErrorCode);

        return result;
    }

    // 请求通过手机验证码重置密码
    async Task<AccountResetPasswordResponse> IContainerStatelessAccount.ResetPasswordByPhoneRequest(AccountResetPasswordByPhoneRequest request, string client_ip)
    {
        var response = new AccountResetPasswordResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null || string.IsNullOrEmpty(request.PhoneCode) || string.IsNullOrEmpty(request.PhoneNumber))
        {
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        //Logger.LogInformation("PhoneCode={0}", request.PhoneCode);
        //Logger.LogInformation("PhoneNumber={0}", request.PhoneNumber);
        //Logger.LogInformation("NewPassword={0}", request.NewPassword);
        //Logger.LogInformation("PhoneVerificationCode={0}", request.PhoneVerificationCode);

        var account_entity = await Db.ReadAsync<DataAccount>(
            a => a.PhoneCode == request.PhoneCode && a.PhoneNumber == request.PhoneNumber,
            StringDef.DbCollectionDataAccount);
        if (account_entity == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        CachePhoneVerificationCode entity = await Db.ReadAsync<CachePhoneVerificationCode>(
            a => a.Id == request.PhoneCode + request.PhoneNumber, StringDef.DbCollectionCachePhoneVerificationCode);
        if (entity == null || entity.VerificationCode != request.PhoneVerificationCode)
        {
            await TraceAccountErrorAsync(
                account_entity, client_ip,
                UCenterErrorCode.AccountPasswordUnauthorized,
                "The account phone and phoneverificationcode do not match");

            response.ErrorCode = UCenterErrorCode.PhoneVerificationCodeError;
            goto End;
        }

        account_entity.Password = EncryptHelper.ComputeHash(request.NewPassword);

        await Db.ReplaceOneData(
            StringDef.DbCollectionDataAccount, account_entity.Id, account_entity);

        account_entity.Password = request.NewPassword;

        await TraceAccountEvent(account_entity, client_ip, "ResetPassword");

        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation("GrainAccountService.ResetPasswordByPhoneRequest() Result={0}", response.ErrorCode);

        return response;
    }

    // 请求修改性别
    async Task<AccountChangeGenderResponse> IContainerStatelessAccount.ChangeGenderRequest(AccountChangeGenderInfo request, string client_ip)
    {
        var response = new AccountChangeGenderResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null
            || string.IsNullOrEmpty(request.AccountId)
            || string.IsNullOrEmpty(request.Token))
        {
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        DataAccount account_entity = await Db.ReadAsync<DataAccount>(
            a => a.Id == request.AccountId && a.Token == request.Token,
            StringDef.DbCollectionDataAccount);

        if (account_entity == null)
        {
            await TraceAccountErrorAsync(
                request.AccountId,
                client_ip,
                UCenterErrorCode.AccountNotExist);

            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }
        else
        {
            account_entity.Gender = request.NewGender;

            //Logger.LogInformation("AccountName={0}", account_entity.AccountName);
            //Logger.LogInformation("PhoneCode={0}", account_entity.PhoneCode);
            //Logger.LogInformation("PhoneNumber={0}", account_entity.PhoneNumber);
            //Logger.LogInformation("Email={0}", account_entity.Email);
            //Logger.LogInformation("NewGender={0}", account_entity.Gender);

            await Db.ReplaceOneData(
                StringDef.DbCollectionDataAccount, account_entity.Id, account_entity);
        }

        await TraceAccountEvent(account_entity, client_ip, "ChangeGender");

        AccountChangeGenderResponse data = new()
        {
            AccountId = account_entity.Id,
            AccountName = account_entity.AccountName,
            AccountType = account_entity.AccountType,
            AccountStatus = account_entity.AccountStatus,
            Name = account_entity.Name,
            ProfileImage = account_entity.ProfileImage,
            ProfileThumbnail = account_entity.ProfileThumbnail,
            Gender = account_entity.Gender,
            Identity = account_entity.Identity,
            PhoneCode = account_entity.PhoneCode,
            PhoneNumber = account_entity.PhoneNumber,
            Email = account_entity.Email,
            Token = account_entity.Token,
            LastLoginDateTime = account_entity.LastLoginDateTime
        };

        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation("GrainAccountService.ChangeGenderRequest() Result={0}", response.ErrorCode);

        return response;
    }

    // 请求上传头像
    async Task<AccountUploadProfileImageResponse> IContainerStatelessAccount.UploadProfileImageRequest(AccountUploadProfileImageRequest request, string client_ip)
    {
        //var small_profile_name11 = Config.ConfigUCenter.ImageProfileThumbnailForBlobNameTemplate.FormatInvariant(request.AccountId);
        //string key = string.Format(Config.ConfigUCenter.ImageContainerName, Config.ConfigCommon.OssRootDir) + small_profile_name11;

        var response = new AccountUploadProfileImageResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        DataAccount account_entity;

        IStorage storage_context = UCenterContext.Instance.StorageContext;

        ConfigApp app = UCenterContext.Instance.GetAppEntityByAppId(request.AppId);
        if (app == null)
        {
            Logger.LogError("App == null, appId={0}", request.AppId);
            response.ErrorCode = UCenterErrorCode.AccountOAuthTokenUnauthorized;
            goto End;
        }

        account_entity = await GetAndVerifyAccount(request.AccountId, client_ip);
        if (account_entity == null)
        {
            response.ErrorCode = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        byte[] file_data = Convert.FromBase64String(request.file_data);
        using (var stream = new MemoryStream(file_data))
        {
            Image<Rgba32> image = null;

            try
            {
                image = (Image<Rgba32>)Image.Load(stream);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            using (var thumbnail_stream = GetThumbprintStream(image))
            {
                var small_profile_name = UCenterOptions.Value.ImageProfileThumbnailForBlobNameTemplate.FormatInvariant(request.AccountId);

                try
                {
                    account_entity.ProfileThumbnail =
                        await storage_context.UploadBlobAsync(app.AliOssBucketName, small_profile_name, thumbnail_stream);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.ToString());
                }
            }

            try
            {
                string profile_name = UCenterOptions.Value.ImageProfileImageForBlobNameTemplate.FormatInvariant(request.AccountId);
                stream.Seek(0, SeekOrigin.Begin);
                await storage_context.UploadBlobAsync(app.AliOssBucketName, profile_name, stream);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }

            await Db.ReplaceOneData(
                StringDef.DbCollectionDataAccount, account_entity.Id, account_entity);

            await TraceAccountEvent(account_entity, client_ip, "UploadProfileImage");
        }

        //AccountUploadProfileImageResponse data = new()
        //{
        //    AccountId = account_entity.Id,
        //    AccountName = account_entity.AccountName,
        //    AccountType = account_entity.AccountType,
        //    AccountStatus = account_entity.AccountStatus,
        //    Name = account_entity.Name,
        //    ProfileImage = account_entity.ProfileImage,
        //    ProfileThumbnail = account_entity.ProfileThumbnail,
        //    Gender = account_entity.Gender,
        //    Identity = account_entity.Identity,
        //    PhoneCode = account_entity.PhoneCode,
        //    PhoneNumber = account_entity.PhoneNumber,
        //    Email = account_entity.Email,
        //    Token = account_entity.Token,
        //    LastLoginDateTime = account_entity.LastLoginDateTime
        //};

        response.ErrorCode = UCenterErrorCode.NoError;
        response.AccountId = account_entity.Id;
        response.AccountName = account_entity.AccountName;
        response.AccountType = account_entity.AccountType;
        response.AccountStatus = account_entity.AccountStatus;
        response.Name = account_entity.Name;
        response.ProfileImage = account_entity.ProfileImage;
        response.ProfileThumbnail = account_entity.ProfileThumbnail;
        response.Gender = account_entity.Gender;
        response.Identity = account_entity.Identity;
        response.PhoneCode = account_entity.PhoneCode;
        response.PhoneNumber = account_entity.PhoneNumber;
        response.Email = account_entity.Email;
        response.Token = account_entity.Token;
        response.LastLoginDateTime = account_entity.LastLoginDateTime;

    End:
        Logger.LogInformation("GrainAccountService.UploadProfileImageRequest() FileStrLen={FileStrLen} ClientIp={ClientIp}, Result={Result}",
            request.file_data.Length, client_ip, response.ErrorCode);

        return response;
    }

    // 请求实名认证
    async Task<IdCardResponse> IContainerStatelessAccount.IdCardCheckCardAndNameRequest(CheckCardAndNameRequest request, string client_ip)
    {
        var response = new IdCardResponse()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        IdCardResponse idcard_response = null;

        if (request == null
            || string.IsNullOrEmpty(request.CardNo)
            || string.IsNullOrEmpty(request.RealName)
            || string.IsNullOrEmpty(request.AccountId)
            || string.IsNullOrEmpty(request.Token))
        {
            // 参数不合法
            response.ErrorCode = UCenterErrorCode.Error;
            goto End;
        }

        if (!UCenterOptions.Value.IdCardCheckEnable)
        {
            // 禁用实名认证
            response.ErrorCode = UCenterErrorCode.Error;
            goto End;
        }

        var check_code = await CheckPhoneVerificationCode(request.PhoneCode, request.PhoneNumber, request.PhoneVerificationCode);
        if (!check_code)
        {
            response.ErrorCode = UCenterErrorCode.PhoneVerificationCodeError;
            goto End;
        }

        string app_code = "6e0ee92481054ea88a4e7dbddcd469f5";
        string url = "http://1.api.apistore.cn/idcard3?";

        try
        {
            using var http_client = HttpClientFactory.CreateClient();

            using var http_request = new HttpRequestMessage(HttpMethod.Post, url);

            // 设置Header
            http_request.Headers.Add("Authorization", "APPCODE " + app_code);

            // 设置Body
            var content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("cardNo", request.CardNo),
                new KeyValuePair<string, string>("realName", request.RealName)
            ]);
            http_request.Content = content;

            using HttpResponseMessage http_response = await http_client.SendAsync(http_request);

            if (http_response.StatusCode != HttpStatusCode.OK)
            {
                response.ErrorCode = UCenterErrorCode.InternalHttpServerError;

                Logger.LogError("ContainerAccount.IdCardCheckCardAndNameRequest Error, StatusCode={StatusCode}", http_response.StatusCode);

                goto End;
            }

            string response_str = await http_response.Content.ReadAsStringAsync();
            Logger.LogInformation(response_str);

            idcard_response = Newtonsoft.Json.JsonConvert.DeserializeObject<IdCardResponse>(response_str);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }

    End:

        idcard_response ??= new IdCardResponse()
        {
            error_code = 90099,
            reason = "认证失败",
            result = null,
            ordersign = string.Empty
        };

        if (idcard_response.error_code == 0)
        {
            // 认证成功则将IdCardEntity写入数据库
            DataIdCard idcard_entity = new()
            {
                Id = idcard_response.result.cardNo,
                CardNo = idcard_response.result.cardNo,
                RealName = idcard_response.result.realName,
                AddrCode = idcard_response.result.details.addrCode,
                Birth = DateTime.Parse(idcard_response.result.details.birth),
                Sex = idcard_response.result.details.sex == 1 ? GenderType.Male : GenderType.Female,
                Length = idcard_response.result.details.length,
                CheckBit = idcard_response.result.details.checkBit,
                Addr = idcard_response.result.details.addr
            };

            await Db.ReplaceOneData(StringDef.DbCollectionDataIdCard, idcard_entity.Id, idcard_entity);

            // 更新AccountEntity中的身份证，真实姓名，性别信息
            DataAccount account_entity = await Db.ReadAsync<DataAccount>(
                a => a.Id == request.AccountId && a.Token == request.Token,
                StringDef.DbCollectionDataAccount);
            if (account_entity != null)
            {
                account_entity.Gender = idcard_entity.Sex;
                account_entity.Name = idcard_entity.RealName;
                account_entity.Identity = idcard_entity.CardNo;

                var filter = Builders<DataAccount>.Filter.Where(e => e.Id == account_entity.Id);
                var update = Builders<DataAccount>.Update
                    .Set(x => x.Identity, account_entity.Identity)
                    .Set(x => x.Name, account_entity.Name)
                    .Set(x => x.Gender, account_entity.Gender);
                await Db.UpdateOneAsync(filter, StringDef.DbCollectionDataAccount, update);
            }
        }

        var v = await _ModifyPhoneNumber(request.AccountId, request.Token, request.PhoneCode, request.PhoneNumber);

        response.ErrorCode = UCenterErrorCode.NoError;
        response.error_code = idcard_response.error_code;
        response.reason = idcard_response.reason;
        response.result = idcard_response.result;
        response.ordersign = idcard_response.ordersign;

        Logger.LogInformation("GrainAccountService.IdCardCheckCardAndNameRequest()");

        return response;
    }

    // 请求实名认证
    async Task<IdCardResult> IContainerStatelessAccount.IdCardResultRequest(DEF.UCenter.IDCradResultRequest request, string client_ip)
    {
        // 更新AccountEntity中的身份证，真实姓名，性别信息
        DataAccount account_entity = await Db.ReadAsync<DataAccount>(
            a => a.Id == request.AccountId && a.Token == request.Token,
            StringDef.DbCollectionDataAccount);
        if (account_entity != null && !string.IsNullOrEmpty(account_entity.Identity))
        {
            DataIdCard id_card = await Db.ReadAsync<DataIdCard>(
                a => a.Id == account_entity.Identity,
                StringDef.DbCollectionDataIdCard);

            if (id_card != null)
            {
                return new IdCardResult()
                {
                    cardNo = id_card.CardNo,
                    realName = id_card.RealName,
                    details = new IdCardDetails()
                    {
                        addr = id_card.Addr,
                        sex = (int)id_card.Sex,
                        birth = id_card.Birth.ToString(),
                        addrCode = id_card.AddrCode,
                        checkBit = id_card.CheckBit,
                    }
                };
            }
        }

        return null;
    }

    // 请求获取Ip所在地
    async Task<SerializeObj<UCenterErrorCode, string>> IContainerStatelessAccount.GetIPAddressRequest(IPCheckRequest request, string client_ip)
    {
        var response = new SerializeObj<UCenterErrorCode, string>()
        {
            obj1 = UCenterErrorCode.Error,
            obj2 = null
        };


        if (request == null || string.IsNullOrEmpty(request.ip))
        {
            goto End;
        }

        string app_code = "6e0ee92481054ea88a4e7dbddcd469f5";
        string url = $"http://c2ba.api.huachen.cn/ip?ip={request.ip}";

        //if (host.Contains("https://"))
        //{
        //    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
        //}

        try
        {
            using var http_client = HttpClientFactory.CreateClient();

            using var http_request = new HttpRequestMessage(HttpMethod.Get, url);

            // 设置Header
            http_request.Headers.Add("Authorization", "APPCODE " + app_code);

            using HttpResponseMessage http_response = await http_client.SendAsync(http_request);

            if (http_response.StatusCode != HttpStatusCode.OK)
            {
                response.obj1 = UCenterErrorCode.InternalHttpServerError;
                response.obj2 = "未知";

                Logger.LogError("ContainerAccount.IdCardCheckCardAndNameRequest Error, StatusCode={StatusCode}", http_response.StatusCode);

                goto End;
            }

            string response_str = await http_response.Content.ReadAsStringAsync();
            Logger.LogInformation(response_str);

            response.obj1 = UCenterErrorCode.InternalHttpServerError;
            var ip_check_result = Newtonsoft.Json.JsonConvert.DeserializeObject<IPCheckResult>(response_str);
            response.obj2 = $"{ip_check_result.data.country}{ip_check_result.data.region}{ip_check_result.data.city}";
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }

    End:

        Logger.LogInformation("GrainAccountService.OnGetIPAddressRequest() Ip={Ip}", request.ip);

        return response;
    }

    // 请求修改手机号？
    async Task<UCenterErrorCode> IContainerStatelessAccount.ModifyPhoneNumber(ModifyPhoneNumberRequest request, string client_ip)
    {
        if (!await CheckPhoneVerificationCode(request.PhoneCode, request.PhoneNumber, request.PhoneVerificationCode))
        {
            return UCenterErrorCode.PhoneVerificationCodeError;
        }

        await _ModifyPhoneNumber(request.AccountId, request.Token, request.PhoneCode, request.PhoneNumber);

        return UCenterErrorCode.NoError;
    }

    // 请求获取指定账号关联的手机号
    async Task<GetPhoneNumberResponse> IContainerStatelessAccount.GetPhoneNumber(GetPhoneNumberRequest request, string client_ip)
    {
        DataAccount account_entity = await Db.ReadAsync<DataAccount>(
                a => a.Id == request.AccountId && a.Token == request.Token,
                StringDef.DbCollectionDataAccount);
        if (account_entity == null)
        {
            return null;
        }
        return new GetPhoneNumberResponse()
        {
            PhoneCode = account_entity.PhoneCode,
            PhoneNumber = account_entity.PhoneNumber,
        };
    }

    // 手机验证码登录
    async Task<PhoneVerificaitonCodeAccessResponse> IContainerStatelessAccount.PhoneVertificationCodeAccessRequest(PhoneVerificaitonCodeAccessInfo request, string client_ip)
    {
        // zhb todo
        PhoneVerificaitonCodeAccessResponse result = new()
        {
            ErrorCode = UCenterErrorCode.Error,
        };

        if (request == null)
        {
            result.ErrorCode = UCenterErrorCode.InvalidParam;
            goto End;
        }

        if (string.IsNullOrEmpty(request.AppId))
        {
            result.ErrorCode = UCenterErrorCode.AppIdNull;
            goto End;
        }

        bool b = EnsureDeviceInfo(request.Device);
        if (!b)
        {
            result.ErrorCode = UCenterErrorCode.DeviceInfoNull;
            goto End;
        }

        //if (!UCenterOptions.Value.EnableGuestAccess)
        //{
        //    result.ErrorCode = UCenterErrorCode.AccountNotExist;
        //    goto End;
        //}

        DataAccount account_entity = null;
        string guest_deviceid = $"{request.AppId}_{request.Device.Id}";

        var data_account_entity = await Db.ReadAsync<DataAccount>(
            a => a.PhoneNumber == request.PhoneNum && a.PhoneCode == request.PhoneCode,
            StringDef.DbCollectionDataAccount);

        if (data_account_entity == null)
        {
            account_entity = new DataAccount()
            {
                Id = Guid.NewGuid().ToString(),
                AccountName = request.PhoneNum.ToString(),
                AccountType = AccountType.NormalAccount,
                AccountStatus = AccountStatus.Active,
                Password = string.Empty,
                SuperPassword = string.Empty,
                Token = Guid.NewGuid().ToString(),
                LastLoginClientIp = client_ip,
                LastLoginDateTime = DateTime.UtcNow,
                LastLoginDeviceId = request.Device != null ? request.Device.Id : string.Empty,
                PhoneCode = request.PhoneCode,
                PhoneNumber = request.PhoneNum,
            };

            await Db.InsertAsync(
                StringDef.DbCollectionDataAccount, account_entity);

            //var data_device_entity = new DataDevice()
            //{
            //    Id = $"{request.AppId}_{request.Device.Id}",
            //    Name = request.Device.Name,
            //    Type = request.Device.Name,
            //    Model = request.Device.Name,
            //    OperationSystem = request.Device.
            //    AccountId = account_entity.Id,
            //    AppId = request.AppId,
            //    Device = request.Device
            //};

            //await Db.InsertAsync(
            //    StringDef.DbCollectionDataDeviceGuest, guest_device_entity);

            await TraceAccountEvent(account_entity, client_ip, "GuestRegister", request.Device);
        }
        else
        {
            account_entity = await Db.ReadAsync<DataAccount>(
            a => a.PhoneNumber == request.PhoneNum && a.PhoneCode == request.PhoneCode,
            StringDef.DbCollectionDataAccount);

            if (account_entity.AccountStatus == AccountStatus.Disabled)
            {
                //await TraceAccountErrorAsync(request.AccountName, client_ip, UCenterErrorCode.AccountDisabled);

                // 账号被禁用
                result.ErrorCode = UCenterErrorCode.AccountDisabled;
                goto End;
            }
            if (string.IsNullOrEmpty(request.VertificationCode) && string.IsNullOrEmpty(request.Token))
            {
                // 参数不合法
                result.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }


            if (!string.IsNullOrEmpty(request.VertificationCode) && !string.IsNullOrEmpty(request.Token))
            {
                // 参数不合法
                result.ErrorCode = UCenterErrorCode.InvalidParam;
                goto End;
            }
            if (!string.IsNullOrEmpty(request.VertificationCode))
            {
                bool check_existed = await CheckPhoneVerificationCode(request.PhoneCode, request.PhoneNum, request.VertificationCode);
                if (!check_existed)
                {
                    // 验证码校验错误
                    result.ErrorCode = UCenterErrorCode.PhoneVerificationCodeError;
                    goto End;
                }
            }

            if (!string.IsNullOrEmpty(request.Token))
            {
                if (account_entity.Token != request.Token)
                {
                    // Token校验错误
                    result.ErrorCode = UCenterErrorCode.AccountTokenUnauthorized;
                    goto End;
                }
                if (account_entity.LastLoginDateTime.AddDays(15) < DateTime.UtcNow)
                {
                    result.ErrorCode = UCenterErrorCode.TokenOutdated;
                    goto End;
                }
            }



            account_entity.Token = Guid.NewGuid().ToString();
            account_entity.LastLoginDateTime = DateTime.UtcNow;
            account_entity.LastLoginClientIp = client_ip;
            account_entity.LastLoginDeviceId = request.Device != null ? request.Device.Id : string.Empty;

            var filter = Builders<DataAccount>.Filter.Where(e => e.Id == account_entity.Id);
            var update = Builders<DataAccount>.Update
                .Set(x => x.Token, account_entity.Token)
                .Set(x => x.LastLoginDateTime, account_entity.LastLoginDateTime)
                .Set(x => x.LastLoginClientIp, account_entity.LastLoginClientIp)
                .Set(x => x.LastLoginDeviceId, account_entity.LastLoginDeviceId);
            await Db.UpdateOneAsync(filter, StringDef.DbCollectionDataAccount, update);

            await TraceAccountEvent(account_entity, client_ip, "GuestLogin", request.Device);
        }

        await LogDeviceInfo(request.Device);

        result.ErrorCode = UCenterErrorCode.NoError;
        result.AccountId = account_entity.Id;
        result.PhoneNum = account_entity.PhoneNumber;
        result.PhoneCode = account_entity.PhoneCode;
        result.AccountType = account_entity.AccountType;
        result.Token = account_entity.Token;
    End:
        Logger.LogInformation("GrainAccountService.PhoneVertificationCodeAccessRequest() Result={0}", result.ErrorCode);

        return result;
    }

    // 请求获取AppData，待Review
    async Task<AppAccountDataResponse> IContainerStatelessAccount.GetAppData(Dictionary<string, string> request_dict, string client_ip)
    {
        // 构建过滤器
        var filters = new List<FilterDefinition<DataAccountAppData>>();
        foreach (var kvp in request_dict)
        {
            var filter = Builders<DataAccountAppData>.Filter.And(
                Builders<DataAccountAppData>.Filter.Where(a =>
                a.DataDictionary != null
                && a.DataDictionary.ContainsKey(kvp.Key)
                && a.DataDictionary[kvp.Key] == kvp.Value)
            );
            filters.Add(filter);
        }
        var finalFilter = Builders<DataAccountAppData>.Filter.And(filters);

        var collection = Db.GetCollection<DataAccountAppData>(typeof(DataAccountAppData).Name);
        var list = await collection.Find(finalFilter, null).ToListAsync();
        var data = list.FirstOrDefault();

        AppAccountDataResponse response = new AppAccountDataResponse();
        if (data is null)
            return response;

        response.AccountId = data.AccountId;
        response.AppId = data.AppId;
        response.Data = data.Data;
        response.DataDictionary = data.DataDictionary;
        return response;
    }

    //async Task<bool> _downloadWechatHeadIcon(string wechat_appid, string url, string account_id)
    //{
    //    ConfigApp app = UCenterContext.Instance.GetAppEntityByWechatAppId(wechat_appid);
    //    if (app == null)
    //    {
    //        Logger.LogError("App == null, WechatAppId={0}", wechat_appid);

    //        return false;
    //    }

    //    try
    //    {
    //        IStorage storage_context = UCenterContext.Instance.StorageContext;

    //        var http_client = HttpClientFactory.CreateClient();
    //        var data = await http_client.GetByteArrayAsync(url);

    //        using MemoryStream stream = new(data);
    //        var image = (Image<Rgba32>)Image.Load(stream);

    //        using (var thumbnail_stream = GetThumbprintStream(image))
    //        {
    //            var small_profile_name = UCenterOptions.Value.ImageProfileThumbnailForBlobNameTemplate.FormatInvariant(account_id);

    //            try
    //            {
    //                await storage_context.UploadBlobAsync(app.AliOssBucketName, small_profile_name, thumbnail_stream);
    //            }
    //            catch (Exception ex)
    //            {
    //                Logger.LogError(ex.ToString());
    //            }
    //        }

    //        try
    //        {
    //            string profile_name = UCenterOptions.Value.ImageProfileImageForBlobNameTemplate.FormatInvariant(account_id);
    //            stream.Seek(0, SeekOrigin.Begin);
    //            await storage_context.UploadBlobAsync(app.AliOssBucketName, profile_name, stream);
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.LogError(ex.ToString());
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.LogError(ex.ToString());
    //    }

    //    return true;
    //}

    //async Task<bool> _downloadFacebookHeadIcon(string facebook_appid, string head_img_url, string UserId, string account_id)
    //{
    //    ConfigApp app = UCenterContext.Instance.GetAppEntityByFacebookAppId(facebook_appid);
    //    if (app == null)
    //    {
    //        Logger.LogError("App == null, facebook_appid={0}", facebook_appid);
    //        return false;
    //    }

    //    try
    //    {
    //        // 上传小头像
    //        IStorage storage_context = UCenterContext.Instance.StorageContext;
    //        //string small_head_url = string.Format(UCenterOptions.Value.FacebookGetSmallHeadImageUrl, UserId);

    //        var http_client = HttpClientFactory.CreateClient();
    //        var data = await http_client.GetByteArrayAsync(head_img_url);

    //        using (MemoryStream stream = new(data))
    //        {
    //            var small_profile_name = UCenterOptions.Value.ImageProfileThumbnailForBlobNameTemplate.FormatInvariant(account_id);
    //            stream.Seek(0, SeekOrigin.Begin);
    //            await storage_context.UploadBlobAsync(app.AliOssBucketName, small_profile_name, stream);
    //        }

    //        // 上传大头像
    //        //string big_head_url = string.Format(UCenterOptions.Value.FacebookGetBigHeadImageUrl, UserId);

    //        //var big_data = await http_client.GetByteArrayAsync(big_head_url);

    //        //using (MemoryStream stream = new(big_data))
    //        //{
    //        //    string profile_name = UCenterOptions.Value.ImageProfileImageForBlobNameTemplate.FormatInvariant(account_id);
    //        //    stream.Seek(0, SeekOrigin.Begin);
    //        //    await storage_context.UploadBlobAsync(app.AliOssBucketName, profile_name, stream);
    //        //}
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.LogError(ex.ToString());
    //    }

    //    return true;
    //}

    async Task<bool> _downloadGoogleHeadIcon(string google_appid, string account_id, string head_url)
    {
        ConfigApp app = UCenterContext.Instance.GetAppEntityByGoogleAppId(google_appid);
        if (app == null)
        {
            Logger.LogError("App == null, google_appid={google_appid}", google_appid);
            return false;
        }

        try
        {
            // 上传小头像
            IStorage storage_context = UCenterContext.Instance.StorageContext;
            //string small_head_url = string.Format(UCenterOptions.Value.FacebookGetSmallHeadImageUrl, UserId);

            using var http_client = HttpClientFactory.CreateClient();

            var data = await http_client.GetByteArrayAsync(head_url);

            using (MemoryStream stream = new(data))
            {
                var small_profile_name = UCenterOptions.Value.ImageProfileThumbnailForBlobNameTemplate.FormatInvariant(account_id);
                stream.Seek(0, SeekOrigin.Begin);
                await storage_context.UploadBlobAsync(app.AliOssBucketName, small_profile_name, stream);
            }

            // 上传大头像
            //string big_head_url = string.Format(UCenterOptions.Value.FacebookGetBigHeadImageUrl, UserId);

            var big_data = await http_client.GetByteArrayAsync(head_url);

            using (MemoryStream stream = new(big_data))
            {
                string profile_name = UCenterOptions.Value.ImageProfileImageForBlobNameTemplate.FormatInvariant(account_id);
                stream.Seek(0, SeekOrigin.Begin);
                await storage_context.UploadBlobAsync(app.AliOssBucketName, profile_name, stream);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());
        }

        return true;
    }

    async Task<bool> _accountLoginAsync(string device_id, DataAccount account_entity, string client_ip)
    {
        if (account_entity.AccountStatus == AccountStatus.Disabled)
        {
            await TraceAccountErrorAsync(
                 account_entity, client_ip,
                 UCenterErrorCode.AccountDisabled,
                 "The account is disabled");

            return false;
        }

        account_entity.Token = Guid.NewGuid().ToString();
        account_entity.LastLoginDateTime = DateTime.UtcNow;
        account_entity.LastLoginClientIp = client_ip;
        account_entity.LastLoginDeviceId = device_id;

        var filter = Builders<DataAccount>.Filter.Where(e => e.Id == account_entity.Id);
        var update = Builders<DataAccount>.Update
            .Set(x => x.Token, account_entity.Token)
            .Set(x => x.LastLoginDateTime, account_entity.LastLoginDateTime)
            .Set(x => x.LastLoginClientIp, account_entity.LastLoginClientIp)
            .Set(x => x.LastLoginDeviceId, account_entity.LastLoginDeviceId);
        await Db.UpdateOneAsync(filter, StringDef.DbCollectionDataAccount, update);

        return true;
    }

    async Task<DataAccount> _wechatLoginAsync(ConfigApp app, DeviceInfo device, OAuthAccessTokenResult access_token_result, string client_ip)
    {
        OAuthUserInfo user_info = null;

        try
        {
            user_info = await OAuthApi.GetUserInfoAsync(
            access_token_result.access_token, access_token_result.openid);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.ToString());

            return null;
        }

        if (user_info == null)
        {
            return null;
        }

        Logger.LogInformation("OpenId={OpenId}，NickName={NickName}，Headimgurl={Headimgurl}，Unionid={Unionid}",
            user_info.openid, user_info.nickname, user_info.headimgurl, user_info.unionid);

        bool need_update_nickname = false;
        bool need_update_icon = false;

        // 查找AccountWechat
        var acc_wechat = await Db.ReadAsync<DataAccountWechat>(
            a => a.Unionid == user_info.unionid
            && a.OpenId == user_info.openid
            && a.AppId == app.WechatAppId,
            StringDef.DbCollectionDataAccountWechat);

        // 创建AccountWechat
        if (acc_wechat == null)
        {
            acc_wechat = new DataAccountWechat()
            {
                Id = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString(),
                Unionid = user_info.unionid,
                OpenId = user_info.openid,
                AppId = app.WechatAppId,
                NickName = user_info.nickname,
                Gender = GenderType.Unknow,
                Province = string.Empty,
                City = string.Empty,
                Country = string.Empty,
                Headimgurl = user_info.headimgurl,
                AppType = DataAccountWechat.WXAppType.App,

            };

            await Db.InsertAsync(StringDef.DbCollectionDataAccountWechat, acc_wechat);

            need_update_nickname = true;
            need_update_icon = true;
        }
        else
        {
            if (acc_wechat.Headimgurl != user_info.headimgurl)
            {
                acc_wechat.Headimgurl = user_info.headimgurl;
                need_update_icon = true;
            }

            if (acc_wechat.NickName != user_info.nickname)
            {
                acc_wechat.NickName = user_info.nickname;
                need_update_nickname = true;
            }

            if (need_update_icon || need_update_nickname)
            {
                await Db.ReplaceOneData(
                    StringDef.DbCollectionDataAccountWechat, acc_wechat.Id, acc_wechat);
            }
        }

        // 查找Account
        var acc = await Db.ReadAsync<DataAccount>(
            a => a.Id == acc_wechat.AccountId,
            StringDef.DbCollectionDataAccount);

        // 创建Account
        if (acc == null)
        {
            acc = new DataAccount()
            {
                Id = acc_wechat.AccountId,
                AccountName = Guid.NewGuid().ToString(),
                AccountType = AccountType.NormalAccount,
                AccountStatus = AccountStatus.Active,
                Password = Guid.NewGuid().ToString(),
                SuperPassword = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString(),
                Gender = acc_wechat.Gender,
                Identity = string.Empty,
                PhoneCode = string.Empty,
                PhoneNumber = string.Empty,
                Email = string.Empty
            };

            await Db.InsertAsync(StringDef.DbCollectionDataAccount, acc);

            need_update_nickname = true;
            need_update_icon = true;
        }

        acc.AccountName = user_info.openid;// AccountName中保存OpenId

        // 微信头像覆盖Acc头像
        if (acc.ProfileImage != user_info.headimgurl)
        {
            //Logger.LogInformation("微信头像覆盖Acc头像，Headimgurl={0}", acc_wechat.Headimgurl);

            //await _downloadWechatHeadIcon(app.WechatAppId, user_info.headimgurl, acc.Id);

            acc.ProfileImage = user_info.headimgurl;

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

            // 微信昵称覆盖Acc昵称
            if (current_nickname != acc_wechat.NickName && !string.IsNullOrEmpty(acc_wechat.NickName))
            {
                m["nick_name"] = acc_wechat.NickName;
                account_data.Data = Newtonsoft.Json.JsonConvert.SerializeObject(m);

                await Db.ReplaceOneData(
                    StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);
            }
        }
        else
        {
            Dictionary<string, string> m = new()
            {
                { "nick_name", acc_wechat.NickName }
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
        await _accountLoginAsync(device_id, acc, client_ip);

        if (device != null && !string.IsNullOrEmpty(device.Id))
        {
            await LogDeviceInfo(device);
        }

        // 更新微信登录Token缓存
        CacheWechatAccessToken cache = new()
        {
            Id = app.WechatAppId + user_info.openid,
            UpdatedTime = DateTime.UtcNow,
            AppId = app.WechatAppId,
            OpenId = user_info.openid,
            AccessToken = access_token_result.access_token,
            RefreshToken = access_token_result.refresh_token
        };
        await Db.ReplaceOneData(StringDef.DbCollectionCacheWechatAccessToken, cache.Id, cache);

        return acc;
    }

    // 时间戳转换为时间
    DateTime _stampToDateTime(string timeStamp)
    {
        DateTime dateTimeStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new(lTime);
        return dateTimeStart.Add(toNow);
    }

    // 获得Facebook用户验证信息
    async Task<FacebookAuthInfo> _getFacebookAuthInfo(string client_token)
    {
        var auth_info = new FacebookAuthInfo();

        //string url = string.Format(UCenterOptions.Value.FacebookAuthUrl,
        //    client_token, UCenterOptions.Value.FacebookAppId, UCenterOptions.Value.FacebookSecret);

        string url = string.Format(UCenterOptions.Value.FacebookAuthUrl, client_token, UCenterOptions.Value.FacebookAccessToken);
        //Logger.LogInformation("Facebook 验证用户信息=" + url);
        //HttpWebResponse facebook_response = _createGetHttpResponse(url, 10000);

        //string respString = string.Empty;//_getResponseString(facebook_response);
        //using (Stream s = facebook_response.GetResponseStream())
        //{
        //    StreamReader reader = new StreamReader(s, Encoding.UTF8);
        //    respString = reader.ReadToEnd();
        //}

        string respString = string.Empty;

        using (HttpClient hc = HttpClientFactory.CreateClient())
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);

            using var res = await hc.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                respString = await res.Content.ReadAsStringAsync();
            }
        }

        Logger.LogInformation("Facebook 返回结果respString={respString}", respString);
        LitJson.JsonData responseData = LitJson.JsonMapper.ToObject(respString);

        if (responseData.ContainsKey("data"))
        {
            var data = responseData["data"];
            if (data.ContainsKey("is_valid"))
            {
                auth_info.IsValid = (bool)data["is_valid"];
            }

            if (data.ContainsKey("app_id"))
            {
                auth_info.AppId = data["app_id"].ToString();
            }

            if (data.ContainsKey("application"))
            {
                auth_info.AppName = data["application"].ToString();
            }

            if (data.ContainsKey("user_id"))
            {
                auth_info.UserId = data["user_id"].ToString();
            }

            if (data.ContainsKey("data_access_expires_at"))
            {
                auth_info.AccessTokenExpires = _stampToDateTime(data["data_access_expires_at"].ToString());
            }

            if (data.ContainsKey("expires_at"))
            {
                auth_info.UserTokenExpires = _stampToDateTime(data["expires_at"].ToString());
            }

            if (data.ContainsKey("error"))
            {
                var error_info = data["error"];
                if (error_info.ContainsKey("code"))
                {
                    auth_info.ErrorCode = int.Parse(error_info["code"].ToString());
                }

                if (error_info.ContainsKey("message"))
                {
                    auth_info.ErrorInfo = error_info["message"].ToString();
                }
            }

            // 获取一次用户信息
            url = string.Format(UCenterOptions.Value.FacebookGetUserInfoUrl, auth_info.UserId, UCenterOptions.Value.FacebookAccessToken);

            respString = string.Empty;

            using (HttpClient hc = HttpClientFactory.CreateClient())
            {
                using var req = new HttpRequestMessage(HttpMethod.Get, url);

                using var res = await hc.SendAsync(req);

                if (res.IsSuccessStatusCode)
                {
                    respString = await res.Content.ReadAsStringAsync();
                }
            }

            LitJson.JsonData tmp_info = LitJson.JsonMapper.ToObject(respString);
            if (tmp_info.ContainsKey("name"))
            {
                auth_info.NickName = tmp_info["name"].ToString();
            }

            if (tmp_info.ContainsKey("picture"))
            {
                LitJson.JsonData tmp_pic_info = tmp_info["picture"];
                if (tmp_pic_info != null && tmp_pic_info.ContainsKey("data"))
                {
                    LitJson.JsonData tmp_pic_info2 = tmp_pic_info["data"];
                    if (tmp_pic_info2 != null && tmp_pic_info2.ContainsKey("url"))
                    {
                        auth_info.HeadImgUrl = tmp_pic_info2["url"].ToString();
                    }
                }
            }
        }

        return auth_info;
    }

    async Task<DataAccount> _facebookLoginAsync(FacebookAuthInfo facebook_user_info, DeviceInfo device, string client_ip)
    {
        bool need_update_nickname = false;
        bool need_update_icon = false;

        // 查找AccountFacebook
        var acc_facebook = await Db.ReadAsync<DataAccountFacebook>(
            a => a.UserId == facebook_user_info.UserId,
            StringDef.DbCollectionDataAccountFacebook);

        // 创建AccountFacebook
        if (acc_facebook == null)
        {
            acc_facebook = new DataAccountFacebook()
            {
                Id = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString(),
                UserId = facebook_user_info.UserId,
                AppId = facebook_user_info.AppId,
                NickName = facebook_user_info.NickName,
                Headimgurl = facebook_user_info.HeadImgUrl,
            };

            await Db.InsertAsync(
                StringDef.DbCollectionDataAccountFacebook, acc_facebook);

            need_update_nickname = true;
            need_update_icon = true;
        }
        else
        {
            if (string.IsNullOrEmpty(acc_facebook.Headimgurl))
            {
                acc_facebook.Headimgurl = facebook_user_info.HeadImgUrl;
                need_update_icon = true;
            }

            if (acc_facebook.NickName != facebook_user_info.NickName)
            {
                acc_facebook.NickName = facebook_user_info.NickName;
                need_update_nickname = true;
            }

            if (need_update_icon || need_update_nickname)
            {
                await Db.ReplaceOneData(
                    StringDef.DbCollectionDataAccountFacebook, acc_facebook.Id, acc_facebook);
            }
        }

        ConfigApp app = UCenterContext.Instance.GetAppEntityByFacebookAppId(facebook_user_info.AppId);

        // 查找Account
        var acc = await Db.ReadAsync<DataAccount>(
            a => a.Id == acc_facebook.AccountId,
            StringDef.DbCollectionDataAccount);

        // 创建Account
        if (acc == null)
        {
            acc = new DataAccount()
            {
                Id = acc_facebook.AccountId,
                AccountName = Guid.NewGuid().ToString(),
                AccountType = AccountType.NormalAccount,
                AccountStatus = AccountStatus.Active,
                Password = Guid.NewGuid().ToString(),
                SuperPassword = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString(),
                Gender = acc_facebook.Gender,
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

        acc.AccountName = facebook_user_info.UserId;// AccountName中保存OpenId
        acc.Name = facebook_user_info.NickName;

        // Facebook头像覆盖Acc头像
        if (acc.ProfileImage != acc_facebook.Headimgurl)
        {
            Logger.LogInformation("facebook头像覆盖Acc头像，Headimgurl={0}", acc_facebook.Headimgurl);

            //await _downloadFacebookHeadIcon(facebook_user_info.AppId, acc_facebook.Headimgurl, acc_facebook.UserId, acc.Id);

            acc.ProfileImage = acc_facebook.Headimgurl;
            acc.ProfileThumbnail = acc_facebook.Headimgurl;

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

            // Facebook昵称覆盖Acc昵称
            if (current_nickname != facebook_user_info.NickName && !string.IsNullOrEmpty(facebook_user_info.NickName))
            {
                m["nick_name"] = facebook_user_info.NickName;
                account_data.Data = Newtonsoft.Json.JsonConvert.SerializeObject(m);

                await Db.ReplaceOneData(StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);
            }
        }
        else
        {
            Dictionary<string, string> m = new()
            {
                { "nick_name", facebook_user_info.NickName }
            };

            account_data = new DataAccountAppData
            {
                Id = data_id,
                AppId = app.Id,
                AccountId = acc.Id,
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(m)
            };

            await Db.ReplaceOneData(StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);
        }

        string device_id = string.Empty;
        if (device != null && !string.IsNullOrEmpty(device.Id))
        {
            device_id = device.Id;
        }
        await _accountLoginAsync(device_id, acc, client_ip);

        if (device != null && !string.IsNullOrEmpty(device.Id))
        {
            await LogDeviceInfo(device);
        }

        // 更新Facebook登录Token缓存
        //Db.CacheWechatAccessToken cache = new Db.CacheWechatAccessToken()
        //{
        //    Id = app.WechatAppId + user_info.openid,
        //    UpdatedTime = DateTime.UtcNow,
        //    AppId = app.WechatAppId,
        //    OpenId = user_info.openid,
        //    AccessToken = access_token_result.access_token,
        //    RefreshToken = access_token_result.refresh_token
        //};
        //await DbUCenter.ReplaceOneData(
        //    StringDef.DbCollectionCacheWechatAccessToken, cache.Id, cache);

        return acc;
    }

    // client_code 客户端授权代码
    async Task<bool> _verifyGoogleLoginCode(string client_code)
    {

        var request_data1 = new LitJson.JsonData
        {
            ["grant_type"] = "authorization_code",// 判定客户端授权代码 是否为我们游戏的，通过我们的ClientId 和 client secret 验证
            ["client_id"] = UCenterOptions.Value.GooglePlayLoginClientId,
            ["client_secret"] = UCenterOptions.Value.GooglePlayLoginClientSecret,
            ["code"] = client_code
        };
        string request_data2 = request_data1.ToJson();

        string response_data = string.Empty;
        using (var web_client = HttpClientFactory.CreateClient())
        {
            // todo，待整理
            using var response = await web_client.PostAsync(UCenterOptions.Value.GooglePayGetAccessTokenUrl,
                new StringContent(request_data2, Encoding.UTF8, "application/json"));
            response_data = await response.Content.ReadAsStringAsync();
        }

        var responseData = LitJson.JsonMapper.ToObject(response_data);

        if (responseData.ContainsKey("access_token"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 获得Google用户验证信息
    async Task<GoogleLoginAuthInfo> _getGoogleAuthInfo(string client_token)
    {
        var auth_info = new GoogleLoginAuthInfo();

        string url = string.Format(UCenterOptions.Value.GooglePlayLoginVerifyUrl, client_token);
        //Logger.LogInformation("Facebook 验证用户信息=" + url);
        //HttpWebResponse facebook_response = _createGetHttpResponse(url, 10000);

        //string respString = string.Empty;//_getResponseString(facebook_response);
        //using (Stream s = facebook_response.GetResponseStream())
        //{
        //    StreamReader reader = new StreamReader(s, Encoding.UTF8);
        //    respString = reader.ReadToEnd();
        //}

        string respString = string.Empty;

        using (HttpClient hc = HttpClientFactory.CreateClient())
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);

            using var res = await hc.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                respString = await res.Content.ReadAsStringAsync();
            }
        }

        Logger.LogInformation("Google 返回结果respString=" + respString);
        LitJson.JsonData responseData = LitJson.JsonMapper.ToObject(respString);

        if (!responseData.ContainsKey("error"))
        {
            auth_info.IsValid = true;
            auth_info.AppId = "com.XX.XXX";
            if (responseData.ContainsKey("sub"))
            {
                auth_info.UserId = responseData["sub"].ToString();
            }

            if (responseData.ContainsKey("email"))
            {
                auth_info.Email = responseData["email"].ToString();
            }

            if (responseData.ContainsKey("picture"))
            {
                auth_info.Headimgurl = responseData["picture"].ToString();
            }

            if (responseData.ContainsKey("email_verified"))
            {
                var is_verified = responseData["email_verified"].ToString();
                if (is_verified == "true")
                {
                    auth_info.IsValid = true;
                }
                else
                {
                    auth_info.IsValid = false;
                }
            }

            //if(responseData.ContainsKey("exp"))
            //{
            //    var exp = (int)responseData["exp"];
            //}
        }
        else
        {
            var error = responseData["error"].ToString();
            auth_info.ErrorInfo = error;
            auth_info.IsValid = false;
        }

        return auth_info;
    }

    async Task<DataAccount> _googleLoginAsync(GoogleLoginAuthInfo _user_info, DeviceInfo device, string client_ip)
    {
        bool need_update_nickname = false;
        bool need_update_icon = false;

        // 查找AccountGoogle
        var acc_google = await Db.ReadAsync<DataAccountGoogle>(
            a => a.UserId == _user_info.UserId,
            StringDef.DbCollectionDataAccountGoogle);

        // 创建AccountFacebook
        if (acc_google == null)
        {
            acc_google = new DataAccountGoogle()
            {
                Id = Guid.NewGuid().ToString(),
                AccountId = Guid.NewGuid().ToString(),
                UserId = _user_info.UserId,
                AppId = _user_info.AppId,
                NickName = _user_info.NickName,
                Headimgurl = _user_info.Headimgurl,
            };

            await Db.InsertAsync(
                StringDef.DbCollectionDataAccountGoogle, acc_google);

            need_update_nickname = true;
            need_update_icon = true;
        }
        else
        {
            if (acc_google.Headimgurl != _user_info.Headimgurl)
            {
                acc_google.Headimgurl = _user_info.Headimgurl;
                need_update_icon = true;
            }

            if (acc_google.NickName != _user_info.NickName)
            {
                acc_google.NickName = _user_info.NickName;
                need_update_nickname = true;
            }

            if (need_update_icon || need_update_nickname)
            {
                await Db.ReplaceOneData(
                    StringDef.DbCollectionDataAccountGoogle, acc_google.Id, acc_google);
            }
        }

        ConfigApp app = UCenterContext.Instance.GetAppEntityByGoogleAppId(_user_info.AppId);

        // 查找Account
        var acc = await Db.ReadAsync<DataAccount>(
            a => a.Id == acc_google.AccountId,
            StringDef.DbCollectionDataAccount);

        // 创建Account
        if (acc == null)
        {
            acc = new DataAccount()
            {
                Id = acc_google.AccountId,
                AccountName = Guid.NewGuid().ToString(),
                AccountType = AccountType.NormalAccount,
                AccountStatus = AccountStatus.Active,
                Password = Guid.NewGuid().ToString(),
                SuperPassword = Guid.NewGuid().ToString(),
                Token = Guid.NewGuid().ToString(),
                Gender = acc_google.Gender,
                Identity = string.Empty,
                PhoneCode = string.Empty,
                PhoneNumber = string.Empty,
                Email = _user_info.Email,
                ProfileImage = acc_google.Headimgurl,
                ProfileThumbnail = acc_google.Headimgurl
            };

            await Db.InsertAsync(
                StringDef.DbCollectionDataAccount, acc);

            need_update_nickname = true;
            need_update_icon = true;
        }

        // AccountName中保存OpenId
        acc.AccountName = acc_google.UserId;
        //acc.ProfileImage = acc_google.Headimgurl;
        //acc.ProfileThumbnail = acc_google.Headimgurl;

        // Facebook头像覆盖Acc头像
        if (acc.ProfileImage != acc_google.Headimgurl)
        {
            Logger.LogInformation("google头像覆盖Acc头像，Headimgurl={0}", acc_google.Headimgurl);

            await _downloadGoogleHeadIcon(acc_google.AppId, acc_google.UserId, acc.Id);

            acc.ProfileImage = acc_google.Headimgurl;

            await Db.ReplaceOneData(
                StringDef.DbCollectionDataAccount, acc.Id, acc);
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

            // Facebook昵称覆盖Acc昵称
            if (current_nickname != _user_info.NickName && !string.IsNullOrEmpty(_user_info.NickName))
            {
                m["nick_name"] = _user_info.NickName;
                account_data.Data = Newtonsoft.Json.JsonConvert.SerializeObject(m);

                await Db.ReplaceOneData(
                    StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);
            }
        }
        else
        {
            Dictionary<string, string> m = new()
            {
                { "nick_name", _user_info.NickName }
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
        await _accountLoginAsync(device_id, acc, client_ip);

        if (device != null && !string.IsNullOrEmpty(device.Id))
        {
            await LogDeviceInfo(device);
        }

        // 更新Facebook登录Token缓存
        //Db.CacheWechatAccessToken cache = new Db.CacheWechatAccessToken()
        //{
        //    Id = app.WechatAppId + user_info.openid,
        //    UpdatedTime = DateTime.UtcNow,
        //    AppId = app.WechatAppId,
        //    OpenId = user_info.openid,
        //    AccessToken = access_token_result.access_token,
        //    RefreshToken = access_token_result.refresh_token
        //};
        //await DbUCenter.ReplaceOneData(
        //    StringDef.DbCollectionCacheWechatAccessToken, cache.Id, cache);

        return acc;
    }

    async Task TraceAccountErrorAsync(DataAccount account, string client_ip,
            UCenterErrorCode code, string message = null)
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
            UserAgent = string.Empty,// todo: Migrate to asp.net core Request.Headers.UserAgent.ToString(),
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

    async Task<DataAccount> GetAndVerifyAccount(string account_id, string client_ip)
    {
        var account = await Db.ReadAsync<DataAccount>(
            a => a.Id == account_id,
            StringDef.DbCollectionDataAccount);
        if (account == null)
        {
            await TraceAccountErrorAsync(account, client_ip,
                 UCenterErrorCode.AccountNotExist, "The account does not exist");
        }

        return account;
    }

    Stream GetThumbprintStream(Image<Rgba32> source_image)
    {
        var stream = new MemoryStream();
        if (source_image.Width > UCenterOptions.Value.ImageMaxThumbnailWidth || source_image.Height > UCenterOptions.Value.ImageMaxThumbnailHeight)
        {
            var radio = Math.Min(
                (double)UCenterOptions.Value.ImageMaxThumbnailWidth / source_image.Width,
                (double)UCenterOptions.Value.ImageMaxThumbnailHeight / source_image.Height);

            var twidth = (int)(source_image.Width * radio);
            var theigth = (int)(source_image.Height * radio);

            source_image.Mutate(x => x
                .Resize(twidth, theigth));
            source_image.SaveAsPng(stream);
        }
        else
        {
            source_image.SaveAsPng(stream);
        }

        stream.Seek(0, SeekOrigin.Begin);
        return stream;
    }

    string GetAppAccountDataId(string app_id, string account_id)
    {
        return $"{app_id}_{account_id}";
    }

    bool ValidatePhone(string phone_code, string phone_number)
    {
        if (string.IsNullOrEmpty(phone_code) || string.IsNullOrEmpty(phone_number))
        {
            return false;
        }

        if (phone_code.Length > 12 || phone_number.Length > 16)
        {
            return false;
        }

        Regex reg = new(@"^((\\+)|(00)|(\\*)|())[0-9]{3,14}((\\#)|())$");
        if (!reg.IsMatch(phone_code + phone_number))
        {
            return false;
        }

        return true;
    }

    UCenterErrorCode ValidateAccount(AccountRegisterRequestInfo account)
    {
        string account_name_pattern = @"^[a-zA-Z0-9.@]*$";
        var account_name_regex = new Regex(account_name_pattern, RegexOptions.IgnoreCase);

        if (account == null)
        {
            return UCenterErrorCode.Error;
        }

        if (string.IsNullOrEmpty(account.AccountName)
            && string.IsNullOrEmpty(account.PhoneCode + account.PhoneNumber)
            && string.IsNullOrEmpty(account.Email))
        {
            return UCenterErrorCode.InvalidAccountName;
        }

        if (!string.IsNullOrEmpty(account.AccountName))
        {
            if (!account_name_regex.IsMatch(account.AccountName)
            || account.AccountName.Length < 4
            || account.AccountName.Length > 64)
            {
                return UCenterErrorCode.InvalidAccountName;
            }
        }

        if (!string.IsNullOrEmpty(account.Password))
        {
            if (account.Password.Length < 6 || account.Password.Length > 64)
            {
                return UCenterErrorCode.InvalidAccountPassword;
            }
        }

        if (!string.IsNullOrEmpty(account.PhoneCode) && !string.IsNullOrEmpty(account.PhoneNumber))
        {
            ValidatePhone(account.PhoneCode, account.PhoneNumber);
        }

        if (!string.IsNullOrEmpty(account.Email))
        {
            string email_pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
            var email_regex = new Regex(email_pattern, RegexOptions.IgnoreCase);

            if (!email_regex.IsMatch(account.Email))
            {
                return UCenterErrorCode.InvalidAccountEmail;
            }
        }

        return UCenterErrorCode.NoError;
    }

    async Task<bool> CheckPhoneVerificationCode(string phone_code, string phone_number, string phone_verification_code)
    {
        CachePhoneVerificationCode entity = await Db.ReadAsync<CachePhoneVerificationCode>(
            a => a.Id == phone_code + phone_number, StringDef.DbCollectionCachePhoneVerificationCode);

        if (entity == null || entity.VerificationCode != phone_verification_code)
        {
            return false;
        }

        var timespan = System.DateTime.UtcNow - entity.UpdatedTime;
        if (timespan.TotalSeconds > 300)
        {
            return false;
        }

        return true;
    }

    async Task<UCenterErrorCode> _ModifyPhoneNumber(string account_id, string token, string phone_code, string phone_number)
    {
        var filter = Builders<DataAccount>.Filter
            .Where(a => a.Id == account_id && a.Token == token);
        var update = Builders<DataAccount>.Update
            .Set(x => x.PhoneCode, phone_code)
            .Set(x => x.PhoneNumber, phone_number);

        await Db.UpdateOneAsync(filter, StringDef.DbCollectionDataAccount, update);

        var entity = await Db.ReadAsync<DataAccount>(a => a.Id == account_id && a.Token == token, StringDef.DbCollectionDataAccount);

        Logger.LogInformation($"UpdatePhone={entity.PhoneCode},{entity.PhoneNumber}");

        return UCenterErrorCode.NoError;
    }
}