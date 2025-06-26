using Microsoft.AspNetCore.Mvc;
using ProtoBuf;
using System.Text;

namespace DEF.Gateway;

// 穿山甲广告，依赖DEF.UCenter
[ApiController]
public class GroMoreController : ControllerBase
{
    ILogger<GroMoreController> Logger { get; set; }
    ServiceClient ServiceClient { get; set; }

    public GroMoreController(ILogger<GroMoreController> log, ServiceClient service_client)
    {
        Logger = log;
        ServiceClient = service_client;
    }

    [HttpGet]
    [Route("gromore/webhook")]
    public async Task<string> GroMoreWebhook()
    {
        string user_id = HttpContext.Request.Query["user_id"].ToString();// 调用SDK透传，应用对用户的唯一标识
        string trans_id = HttpContext.Request.Query["trans_id"].ToString();// 完成观看的唯一交易ID，由Gromore生成
        string sign = HttpContext.Request.Query["sign"].ToString();// 由key和trans_id生成的报文签名
        string extra = HttpContext.Request.Query["extra"].ToString();// 即customData，调用SDK传入并透传，如无需要则为空
        string reward_amount = HttpContext.Request.Query["reward_amount"].ToString();// 媒体平台配置或调用SDK传入
        string reward_name = HttpContext.Request.Query["reward_name"].ToString();// 媒体平台配置或调用SDK传入
        string prime_rit = HttpContext.Request.Query["prime_rit"].ToString();// 广告位id
        int reward_amount_i = Convert.ToInt32(reward_amount);

        Logger.LogDebug("穿山甲聚合广告回调 user_id ={user_id},trans_id={trans_id},sign={sign}, extra={extra}, reward_amount={reward_amount},reward_name={reward_name},prime_rit={prime_rit}",
            user_id, trans_id, sign, extra, reward_amount, reward_name, prime_rit);

        SerializeObj<string, string, int, string, string, string, string> so = new()
        {
            obj1 = user_id,
            obj2 = trans_id,
            obj3 = reward_amount_i,
            obj4 = reward_name,
            obj5 = sign,
            obj6 = prime_rit,
            obj7 = extra
        };

        var serializer_type = GatewayContext.Instance.ServiceNode.Service.Config.SerializerType;

        byte[] method_data = null;

        if (serializer_type == SerializerType.LitJson)
        {
            string s = LitJson.JsonMapper.ToJson(so);
            method_data = Encoding.UTF8.GetBytes(s);
        }
        else if (serializer_type == SerializerType.Protobuf)
        {
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, so);
            method_data = ms.ToArray();
        }

        string service_name = "def.paycenter";
        int containerstate_type = 0;
        string container_type = "payservice";
        string container_id = string.Empty;
        string method_name = "GroMoreWebhookRequest";

        byte[] response_data = await ServiceClient.ForwardContainerRpc(service_name, containerstate_type, container_type, container_id, method_name, method_data);

        string result = string.Empty;
        if (serializer_type == SerializerType.LitJson)
        {
            string s = Encoding.UTF8.GetString(response_data);
            result = LitJson.JsonMapper.ToObject<string>(s);
        }
        else if (serializer_type == SerializerType.Protobuf)
        {
            using var ms = new MemoryStream(method_data);
            result = Serializer.Deserialize<string>(ms);
        }

        return result;
    }
}