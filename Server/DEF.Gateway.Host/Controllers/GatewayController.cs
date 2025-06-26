using Microsoft.AspNetCore.Mvc;

namespace DEF.Gateway;

public class GetIpAddressData
{
    public string ip;
    public string long_ip;
    public string isp;
    public string area;
    public string region_id;
    public string region;
    public string city_id;
    public string city;
    public string country_id;
    public string country;
}

public class GetIpAddressResult
{
    public int ret;
    public string msg;
    public GetIpAddressData data;
    public string log_id;
}

public class DataCrashReport
{
    public DateTime Dt { get; set; }
    public string Data { get; set; }
}

public class GatewayController : ControllerBase
{
    ILogger Logger { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    ServiceClient ServiceClient { get; set; }
    GatewayService GatewayService { get; set; }

    public GatewayController(ILogger<GatewayController> logger, IHttpClientFactory httpclient_factory, ServiceClient service_client, GatewayService gateway_service)
        : base()
    {
        Logger = logger;
        HttpClientFactory = httpclient_factory;
        ServiceClient = service_client;
        GatewayService = gateway_service;
    }

    // Container Rpc，有返回值
    [HttpPost]
    [Route("api/ccr")]
    public async Task<byte[]> ClientContainerRpc([FromQuery] string r)
    {
        int len = (int)Request.ContentLength;
        byte[] buf = new byte[len];
        await Request.Body.ReadExactlyAsync(buf.AsMemory(0, len));
        byte[] method_data = buf;

        if (string.IsNullOrEmpty(r))
        {
            return null;
        }

        var arr_r = r.Split('|');

        if (arr_r.Length != 4 && arr_r.Length != 5)
        {
            return null;
        }

        string service_name = string.Empty;
        int containerstate_type = 0;
        string container_type = string.Empty;
        string container_id = string.Empty;
        string method_name = string.Empty;

        if (arr_r.Length == 4)
        {
            service_name = arr_r[0];
            containerstate_type = int.Parse(arr_r[1]);
            container_type = arr_r[2];
            container_id = string.Empty;
            method_name = arr_r[3];
        }

        if (arr_r.Length == 5)
        {
            service_name = arr_r[0];
            containerstate_type = int.Parse(arr_r[1]);
            container_type = arr_r[2];
            container_id = arr_r[3];
            method_name = arr_r[4];
        }

        //Logger.LogInformation("Controller.ClientContainerRpc() ClientIp={client_ip} Len={len}", client_ip, len);

        if (service_name == "def.gateway")
        {
            var client_ip = ControllerContext.HttpContext.GetClientIpAddress();

            //string client_ipaddress = await GetIpAddress(client_ip);
            string client_ipaddress = string.Empty;

            RpcData rpc_data = new()
            {
                Ticket = 0,
                HasResult = true,
                ServiceName = service_name,
                ContainerStateType = (ContainerStateType)containerstate_type,
                ContainerType = container_type,
                ContainerId = container_id,
                EntityId = 0,
                ComponentName = string.Empty,
                MethodName = method_name,
                MethodData = method_data,
                MethodDataLen = method_data == null ? 0 : method_data.Length,
                TotalDataLen = 0,
            };

            byte[] result_bytes = await GatewayService.OnRecvPackage(rpc_data, null, client_ip, client_ipaddress, false);

            return result_bytes;
        }
        else
        {
            var result_bytes = await ServiceClient.ForwardContainerRpc(
                service_name, containerstate_type, container_type, container_id, method_name, method_data);

            return result_bytes;
        }
    }

    // Container Rpc，无返回值
    [HttpPost]
    [Route("api/ccr2")]
    public async Task ClientContainerRpcNoResult([FromQuery] string r)
    {
        int len = (int)Request.ContentLength;
        byte[] buf = new byte[len];
        await Request.Body.ReadExactlyAsync(buf.AsMemory(0, len));
        byte[] method_data = buf;

        if (string.IsNullOrEmpty(r))
        {
            return;
        }

        var arr_r = r.Split('|');

        if (arr_r.Length != 4 && arr_r.Length != 5)
        {
            return;
        }

        string service_name = string.Empty;
        int containerstate_type = 0;
        string container_type = string.Empty;
        string container_id = string.Empty;
        string method_name = string.Empty;

        if (arr_r.Length == 4)
        {
            service_name = arr_r[0];
            containerstate_type = int.Parse(arr_r[1]);
            container_type = arr_r[2];
            container_id = string.Empty;
            method_name = arr_r[3];
        }

        if (arr_r.Length == 5)
        {
            service_name = arr_r[0];
            containerstate_type = int.Parse(arr_r[1]);
            container_type = arr_r[2];
            container_id = arr_r[3];
            method_name = arr_r[4];
        }

        //Logger.LogInformation("Controller.ClientContainerRpc() ClientIp={client_ip} Len={len}", client_ip, len);

        if (service_name == "def.gateway")
        {
            var client_ip = ControllerContext.HttpContext.GetClientIpAddress();

            //var client_ipaddress = await GetIpAddress(client_ip);
            string client_ipaddress = string.Empty;

            RpcData rpc_data = new()
            {
                Ticket = 0,
                HasResult = false,
                ServiceName = service_name,
                ContainerStateType = (ContainerStateType)containerstate_type,
                ContainerType = container_type,
                ContainerId = container_id,
                EntityId = 0,
                ComponentName = string.Empty,
                MethodName = method_name,
                MethodData = method_data,
                MethodDataLen = method_data == null ? 0 : method_data.Length,
                TotalDataLen = 0,
            };

            await GatewayService.OnRecvPackage(rpc_data, null, client_ip, client_ipaddress, false);
        }
        else
        {
            await ServiceClient.ForwardContainerRpcNoResult(
                service_name, containerstate_type, container_type, container_id, method_name, method_data);
        }
    }

    // Entity Rpc，有返回值
    [HttpPost]
    [Route("api/cer")]
    public async Task<byte[]> ClientEntityRpc([FromQuery] string r)
    {
        //var client_ip = ControllerContext.HttpContext.GetClientIpAddress();

        int len = (int)Request.ContentLength;
        byte[] buf = new byte[len];
        await Request.Body.ReadExactlyAsync(buf.AsMemory(0, len));
        byte[] method_data = buf;

        //Logger.LogInformation("Controller.ClientEntityRpc() ClientIp={client_ip} Len={len}", client_ip, len);

        if (string.IsNullOrEmpty(r))
        {
            return null;
        }

        var arr_r = r.Split('|');

        if (arr_r.Length != 7)
        {
            return null;
        }

        string service_name = string.Empty;
        int containerstate_type = 0;
        string container_type = string.Empty;
        string container_id = string.Empty;
        long entity_id = 0;
        string component_name = string.Empty;
        string method_name = string.Empty;

        if (arr_r.Length == 7)
        {
            service_name = arr_r[0];
            containerstate_type = int.Parse(arr_r[1]);
            container_type = arr_r[2];
            container_id = arr_r[3];
            entity_id = long.Parse(arr_r[4]);
            component_name = arr_r[5];
            method_name = arr_r[6];
        }

        var result_bytes = await ServiceClient.ForwardEntityRpc(
            service_name, containerstate_type, container_type, container_id, entity_id, component_name, method_name, method_data);

        return result_bytes;
    }

    // Entity Rpc，无返回值
    [HttpPost]
    [Route("api/cer2")]
    public async Task ClientEntityRpcNoResult([FromQuery] string r)
    {
        //var client_ip = ControllerContext.HttpContext.GetClientIpAddress();

        int len = (int)Request.ContentLength;
        byte[] buf = new byte[len];
        await Request.Body.ReadExactlyAsync(buf.AsMemory(0, len));
        byte[] method_data = buf;

        //Logger.LogInformation("Controller.ClientEntityRpc() ClientIp={client_ip} Len={len}", client_ip, len);

        if (string.IsNullOrEmpty(r))
        {
            return;
        }

        var arr_r = r.Split('|');

        if (arr_r.Length != 7)
        {
            return;
        }

        string service_name = string.Empty;
        int containerstate_type = 0;
        string container_type = string.Empty;
        string container_id = string.Empty;
        long entity_id = 0;
        string component_name = string.Empty;
        string method_name = string.Empty;

        if (arr_r.Length == 7)
        {
            service_name = arr_r[0];
            containerstate_type = int.Parse(arr_r[1]);
            container_type = arr_r[2];
            container_id = arr_r[3];
            entity_id = long.Parse(arr_r[4]);
            component_name = arr_r[5];
            method_name = arr_r[6];
        }

        await ServiceClient.ForwardEntityRpcNoResult(
            service_name, containerstate_type, container_type, container_id, entity_id, component_name, method_name, method_data);
    }

    // 客户端上报CrashReport
    [HttpGet]
    [Route("api/clientcrashreport")]
    public async Task ClientCrashReport()
    {
        var client_ip = ControllerContext.HttpContext.GetClientIpAddress();

        Logger.LogDebug("GatewayController.ClientCrashReport() ClientIp={client_ip}", client_ip);

        var db = GatewayContext.Instance.Mongo;

        DataCrashReport data = new()
        {
            Dt = DateTime.UtcNow,
            Data = "aaaaaa",
        };

        string collection_name = "CrashReport";

        await db.InsertOneData(collection_name, data);
    }

    // https://market.aliyun.com/apimarket/detail/cmapi00064443?spm=5176.78296.J_6761784310.5.50f45d76vIslAB
    async Task<string> GetIpAddress(string ip)
    {
        if (string.IsNullOrEmpty(ip) || ip == "127.0.0.1")
        {
            return string.Empty;
        }

        string appcode = "APPCODE 6e0ee92481054ea88a4e7dbddcd469f5";
        string url = $"http://c2ba.api.huachen.cn/ip?ip={ip}";

        try
        {
            using HttpClient hc = HttpClientFactory.CreateClient();

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Add("Authorization", appcode);

            using var res = await hc.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                string response_str = await res.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(response_str))
                {
                    //Logger.LogInformation(response_str);

                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<GetIpAddressResult>(response_str);
                    if (result != null && result.data != null)
                    {
                        string result_s = $"{result.data.country}.{result.data.region}.{result.data.city}";

                        //Logger.LogInformation(result_s);

                        return result_s;
                    }
                }
            }
            else
            {
                // Log Error
                Logger.LogError("GetIpAddress HttpError，StatusCode={0}", res.StatusCode);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "GetIpAddress");
        }

        return string.Empty;
    }
}

// admob广告 callback
//[HttpGet]
//[Route("admob/webhook")]
//public async Task<string> AdmobWebhook([FromBody] object value)
//{
//    string str = value.ToString();

//    Logger.LogInformation("UCenter.AdmobWebhook = {0}", str);

//    var container_app = ServiceClient.GetContainerRpc<UCenter.IContainerPayService>();
//    return await container_app.AdmobWebhookRequest(str);
//}

// 穿山甲 聚合广告 GroMore callback
//[HttpGet]
//[Route("gromore/webhook")]
//public async Task<string> GroMoreWebhook()
//{
//    string user_id = HttpContext.Request.Query["user_id"].ToString();// 调用SDK透传，应用对用户的唯一标识
//    string trans_id = HttpContext.Request.Query["trans_id"].ToString();//完成观看的唯一交易ID，由Gromore生成
//    string sign = HttpContext.Request.Query["sign"].ToString();//由key和trans_id生成的报文签名
//    string extra = HttpContext.Request.Query["extra"].ToString();//即customData，调用SDK传入并透传，如无需要则为空
//    string reward_amount = HttpContext.Request.Query["reward_amount"].ToString();//媒体平台配置或调用SDK传入
//    string reward_name = HttpContext.Request.Query["reward_name"].ToString();//媒体平台配置或调用SDK传入
//    string prime_rit = HttpContext.Request.Query["prime_rit"].ToString();//广告位id

//    Logger.LogInformation("穿山甲聚合广告回调 user_id ={0},trans_id={1},sign={2}, extra={3}, reward_amount={4},reward_name={5},prime_rit={6}", user_id, trans_id, sign, extra, reward_amount, reward_name, prime_rit);

//    var container_app = ServiceClient.GetContainerRpc<UCenter.IContainerPayService>();
//    return await container_app.GroMoreWebhookRequest(user_id, trans_id, Convert.ToInt32(reward_amount), reward_name, sign, prime_rit, extra);
//}

//[HttpPost]
//[Route("api/paychargewebhook")]
//public async Task<string> PayChargeWebhook([FromBody] object value)
//{
//    string str = value.ToString();

//    Logger.LogInformation("LobbyController.PayChargeWebhook()");

//    var charge = LitJson.JsonMapper.ToObject<Pay.PayCharge4GiveItem>(str);

//    var grain_player = GrainFactory.GetGrain<IGrainPlayer>(Guid.Parse(charge.PlayerGuid));
//    await grain_player.PayGiveItem(charge.ChargeId, charge.ItemTbId, charge.IsSandbox);

//    return string.Empty;
//}

//[HttpPost]
//[Route("api/payexchangewebhook")]
//public async Task<string> PayExchangeWebhook([FromBody] object value)
//{
//    string str = value.ToString();

//    Logger.LogInformation("LobbyController.PayExchangeWebhook()");

//    var debit = LitJson.JsonMapper.ToObject<Pay.PayExchange4Debit>(str);

//    var grain_player = GrainFactory.GetGrain<IGrainPlayer>(Guid.Parse(debit.PlayerGuid));
//    await grain_player.PayDebit(debit);

//    return string.Empty;
//}