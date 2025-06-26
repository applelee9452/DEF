using DEF.UCenter;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using ProtoBuf;

namespace DEF.Gateway;

public class PayCenterController : ControllerBase
{
    ILogger Logger { get; set; }
    ServiceClient ServiceClient { get; set; }

    public PayCenterController(ILogger<PayCenterController> logger, ServiceClient service_client)
        : base()
    {
        Logger = logger;
        ServiceClient = service_client;
    }

    // 测试
    [HttpGet]
    [Route("api/paycenter/test")]
    public async Task<string> PayCenterTest(string auth, DateTime begin_time, DateTime end_time)
    {
        Logger.LogDebug("PayCenterTest，auth={auth}, begin_time={begin_time} end_time={end_time}", auth, begin_time, end_time);
        return "PayCenterTest OK";
    }

    // 测试创建订单（实际会从 Client 直接到 PayCenter
    [HttpGet]
    [Route("api/paycenter/fake_meida_create_charge")]
    public async Task<string> FakeMeidaCreateCharge(string auth, DateTime begin_time, DateTime end_time)
    {
        Logger.LogDebug("PayCenterTest，auth={auth}, begin_time={begin_time} end_time={end_time}", auth, begin_time, end_time);

        var container_pay_center = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessPayCenter>();

        string player_guid = "377c681d-c4ea-4a2e-a3c9-165f852320d6";
        PayCreateChargeRequest charge_request = new PayCreateChargeRequest();
        charge_request.IAPProductId = "";
        charge_request.Platform = PayPlatform.Meida;
        charge_request.ItemTbId = 1094;
        charge_request.ItemName = "测试用的副本礼包";
        charge_request.Token = "??????";
        charge_request.AccountId = "C60";
        charge_request.AppId = "xxxxx";
        charge_request.PlayerGuid = player_guid;
        charge_request.Amount = 100;
        charge_request.Currency = "rmb";

        PayChargeDetail charge_detail = await container_pay_center.PayCreateCharge(charge_request, "192.168.1.100");
        string json = charge_detail.ToJson();
        return $"PayCenterCreateCharge OK \n {json}";
    }

    [HttpGet]
    [Route("api/paycenter/fake_cancel")]
    public async Task<string> PayChargeCancel([FromQuery] string out_trade_no)
    {
        if (string.IsNullOrEmpty(out_trade_no))
            return "no out_trade_no";
        var c = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessPayCenter>();
        PayChargeInfo pay_charge_info = await c.PayCancelCharge("C60", "token", out_trade_no, "192.168.0.168");
        return pay_charge_info.Status.ToString();
    }

    [HttpGet]
    [Route("api/paycenter/fake_meida_webhook")]
    public async Task<string> PayChargeWebhook([FromQuery] string out_trade_no)
    {
        if (string.IsNullOrEmpty(out_trade_no))
            return "no out_trade_no";

        Dictionary<string, string> dict_kv = new Dictionary<string, string>();
        dict_kv.Add("trade_no", out_trade_no);
        dict_kv.Add("out_trade_no", out_trade_no);
        dict_kv.Add("type", "alipay");
        dict_kv.Add("money", "1");
        dict_kv.Add("param", "");
        dict_kv.Add("sign", "--x--");
        dict_kv.Add("sign_type", "MD5");
        dict_kv.Add("trade_status", "TRADE_SUCCESS");

        var c = ServiceClient.GetContainerRpc<DEF.UCenter.IContainerStatelessPayCenter>();
        string result = await c.MeidaWebhook(dict_kv);

        return result;
    }
}