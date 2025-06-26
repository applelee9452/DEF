using Microsoft.Extensions.Logging;

namespace DEF.UCenter;

public class ContainerStatelessApp : ContainerStateless, IContainerStatelessApp
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    async Task IContainerStatelessApp.AppCreateRequest(AppInfo request)
    {
        var app = await Db.ReadAsync<ConfigApp>(d => d.Id == request.AppId, StringDef.DbCollectionConfigApp);

        if (app == null)
        {
            app = new ConfigApp
            {
                Id = request.AppId,
                Name = request.AppId,
                WechatAppId = "",
                WechatAppSecret = ""
            };

            await Db.InsertAsync(StringDef.DbCollectionConfigApp, app);
        }

        var data = new AppResponse
        {
            AppId = request.AppId,
            AppSecret = request.AppSecret
        };

        Logger.LogInformation($"ContainerStatelessApp.AppCreateRequest() AppId={request.AppId}");
    }

    async Task IContainerStatelessApp.AppCreateConfigRequest(AppConfigInfo request)
    {
        var app_configuration = await Db.ReadAsync<ConfigAppConfig>(
            d => d.Id == request.AppId,
            StringDef.DbCollectionConfigAppConfig);

        if (app_configuration == null)
        {
            app_configuration = new ConfigAppConfig
            {
                Id = request.AppId,
                Configuration = request.Config,
            };

            await Db.InsertAsync(StringDef.DbCollectionConfigAppConfig, app_configuration);
        }
        else
        {
            await Db.ReplaceOneData(StringDef.DbCollectionConfigAppConfig, app_configuration.Id, app_configuration);
        }
    }

    Task IContainerStatelessApp.AppGetConfigRequest(string request)
    {
        //AppConfigResponse data = new();// await appConfigurationCacheProvider.Get(appId, token);

        //Logger.LogInformation("ContainerStatelessApp.AppGetConfigRequest() AppId={0}", request);

        return Task.CompletedTask;
    }

    async Task<Tuple<UCenterErrorCode, Gateway.GatewayAuthResponse>> IContainerStatelessApp.AppAuth(string app_id, string acc_id, string token)
    {
        UCenterErrorCode result = UCenterErrorCode.Error;

        var auth_response = new Gateway.GatewayAuthResponse()
        {
            PlayerGuid = string.Empty,
            Gender = 2,
        };

        var account = await Db.ReadAsync<DataAccount>(
            d => d.Id == acc_id,
            StringDef.DbCollectionDataAccount);

        if (account == null)
        {
            result = UCenterErrorCode.AccountNotExist;
            Logger.LogError("AppAuth，账号不存在");
            goto End;
        }

        if (account.Token != token)
        {
            result = UCenterErrorCode.AccountTokenUnauthorized;
            Logger.LogError("AppAuth，Token错误");
            goto End;
        }

        var data_id = GetAppAccountDataId(app_id, acc_id);

        var account_data = await Db.ReadAsync<DataAccountAppData>(
            d => d.Id == data_id,
            StringDef.DbCollectionDataAccountAppData);

        if (account_data != null)
        {
            var data_read = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(account_data.Data);

            if (data_read.ContainsKey("player_guid"))
            {
                auth_response.PlayerGuid = data_read["player_guid"];
            }
            else
            {
                auth_response.PlayerGuid = Guid.NewGuid().ToString();
                data_read["player_guid"] = auth_response.PlayerGuid;
                account_data.Data = Newtonsoft.Json.JsonConvert.SerializeObject(data_read);

                await Db.ReplaceOneData(StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);
            }
        }
        else
        {
            auth_response.PlayerGuid = Guid.NewGuid().ToString();

            var data_write = new Dictionary<string, string>()
            {
                { "player_guid", auth_response.PlayerGuid },
                { "nick_name", "" }
            };

            account_data = new DataAccountAppData()
            {
                Id = data_id,
                AccountId = acc_id,
                AppId = "",
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow,
                Data = Newtonsoft.Json.JsonConvert.SerializeObject(data_write)
            };

            await Db.ReplaceOneData(StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);
        }

        result = UCenterErrorCode.NoError;
        auth_response.Gender = (int)account.Gender;

    End:

        Logger.LogInformation("ContainerStatelessApp.AppAuth() AccId={0} Result={1}", acc_id, result);

        return new Tuple<UCenterErrorCode, Gateway.GatewayAuthResponse>(result, auth_response);
    }

    async Task<Tuple<UCenterErrorCode, AppAccountLoginResponse>> IContainerStatelessApp.AppVerifyAccountLoginRequest(AppAccountLoginInfo request)
    {
        AppAccountLoginResponse data = null;
        UCenterErrorCode result = UCenterErrorCode.Error;

        var account = await Db.ReadAsync<DataAccount>(
            d => d.Id == request.AccountId,
            StringDef.DbCollectionDataAccount);

        if (account == null)
        {
            result = UCenterErrorCode.AccountNotExist;
            Logger.LogError("账号不存在");

            goto End;
        }

        if (account.Token != request.AccountToken)
        {
            result = UCenterErrorCode.AccountTokenUnauthorized;
            Logger.LogError("Token错误");

            goto End;
        }

        ConfigApp app_entity = UCenterContext.Instance.GetAppEntityByAppId(request.AppId);
        if (app_entity == null)
        {
            result = UCenterErrorCode.AppNotExists;
            Logger.LogError("App不存在");

            goto End;
        }

        data = new AppAccountLoginResponse()
        {
            AccountId = account.Id,
            AccountName = account.AccountName,
            Gender = account.Gender,
            Icon = account.ProfileImage,
            AccountToken = account.Token,
            ClientInfo = new ClientInfo()
            {
                LastLoginDateTime = account.LastLoginDateTime,
                LastLoginClientIp = account.LastLoginClientIp,
                LastLoginDeviceId = account.LastLoginDeviceId,
                Identity = account.Identity
            }
        };

        var data_id = GetAppAccountDataId(request.AppId, request.AccountId);

        var account_data = await Db.ReadAsync<DataAccountAppData>(
            d => d.Id == data_id,
            StringDef.DbCollectionDataAccountAppData);

        if (account_data != null)
        {
            data.AccountData = account_data.Data;
        }

        result = UCenterErrorCode.NoError;

    End:

        Logger.LogInformation("ContainerStatelessApp.AppVerifyAccountLoginRequest() Result={0}", result);

        return new Tuple<UCenterErrorCode, AppAccountLoginResponse>(result, data);
    }

    async Task IContainerStatelessApp.AppReadAccountDataRequest(AppAccountDataInfo request)
    {
        AppAccountDataResponse data = null;
        UCenterErrorCode result = UCenterErrorCode.Error;

        ConfigApp app_entity = UCenterContext.Instance.GetAppEntityByAppId(request.AppId);
        if (app_entity == null)
        {
            result = UCenterErrorCode.AppNotExists;

            goto End;
        }

        var data_id = GetAppAccountDataId(request.AppId, request.AccountId);

        var account_data = await Db.ReadAsync<DataAccountAppData>(
            d => d.Id == data_id,
            StringDef.DbCollectionDataAccountAppData);

        data = new AppAccountDataResponse
        {
            AppId = request.AppId,
            AccountId = request.AccountId,
            Data = account_data?.Data
        };

        result = UCenterErrorCode.NoError;

    End:
        Logger.LogInformation("ContainerStatelessApp.AppReadAccountDataRequest() Result={0}", result);
    }

    async Task IContainerStatelessApp.AppWriteAccountDataRequest(AppAccountDataInfo request)
    {
        AppAccountDataResponse data = null;
        UCenterErrorCode result = UCenterErrorCode.Error;

        ConfigApp app_entity = UCenterContext.Instance.GetAppEntityByAppId(request.AppId);
        if (app_entity == null)
        {
            result = UCenterErrorCode.AppNotExists;

            goto End;
        }

        var account = await Db.ReadAsync<DataAccount>(
            d => d.Id == request.AccountId,
            StringDef.DbCollectionDataAccount);

        if (account == null)
        {
            result = UCenterErrorCode.AccountNotExist;
            goto End;
        }

        var data_id = GetAppAccountDataId(request.AppId, request.AccountId);

        var account_data = await Db.ReadAsync<DataAccountAppData>(
            d => d.Id == data_id,
            StringDef.DbCollectionDataAccountAppData);

        if (account_data != null)
        {
            account_data.Data = request.Data;
        }
        else
        {
            account_data = new DataAccountAppData
            {
                Id = data_id,
                AppId = request.AppId,
                AccountId = request.AccountId,
                Data = request.Data
            };
        }

        await Db.ReplaceOneData(StringDef.DbCollectionDataAccountAppData, account_data.Id, account_data);

        data = new AppAccountDataResponse
        {
            AppId = request.AppId,
            AccountId = request.AccountId,
            Data = account_data.Data
        };

        result = UCenterErrorCode.NoError;

    End:
        Logger.LogInformation("ContainerStatelessApp.AppWriteAccountDataRequest() Result={0}", result);
    }

    // App读取AccountData4Auth
    Task IContainerStatelessApp.AppReadAccountData4Auth()
    {
        return Task.CompletedTask;
    }

    // App写入AccountData4Auth
    Task IContainerStatelessApp.AppWriteAccountData4Auth()
    {
        return Task.CompletedTask;
    }

    async Task<IPCheckResult> IContainerStatelessApp.GetIPAddress(IPCheckRequest request)
    {
        IPCheckResult response = null;

        string appcode = "6e0ee92481054ea88a4e7dbddcd469f5";

        if (request == null || string.IsNullOrEmpty(request.ip))
        {
            goto End;
        }

        Logger.LogInformation("ContainerStatelessApp.GetIPAddress() Ip=" + request.ip);

        string uri = @"http://ip.api.apistore.cn/ip?ip=" + request.ip;

        var http_request = new HttpRequestMessage(HttpMethod.Get, uri);
        http_request.Headers.Add("Authorization", "APPCODE " + appcode);

        var http_client = HttpClientFactory.CreateClient();
        var http_response = await http_client.SendAsync(http_request);
        if (http_response.IsSuccessStatusCode)
        {
            string s = await http_response.Content.ReadAsStringAsync();

            Logger.LogInformation("获取IP所在地，IP：{0} Result：\n{1}", request.ip, s);

            response = Newtonsoft.Json.JsonConvert.DeserializeObject<IPCheckResult>(s);
        }
        else
        {
            // todo, log error
        }

    End:

        return response;
    }

    string GetAppAccountDataId(string app_id, string account_id)
    {
        return $"{app_id}_{account_id}";
    }
}