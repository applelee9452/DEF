#nullable enable

using System.Text.Json.Serialization;
using DEF.UCenter;

namespace DEF.UCenter;

public class MeidaRequestResult
{
    public int Code { get; set; } // 1为成功，其它值为失败

    [JsonPropertyName("msg")]
    public string? Message { get; set; } // 失败时返回原因

    [JsonPropertyName("trade_no")]
    public string? TradeNo { get; set; }// 支付订单号

    [JsonPropertyName("payurl")]
    public string? PayUrl { get; set; }// 如果返回该字段，则直接跳转到该url支付

    [JsonPropertyName("qrcode")]
    public string? QRCode { get; set; } // 如果返回该字段，则根据该url生成二维码

    [JsonPropertyName("urlscheme")]
    public string? UrlScheme { get; set; } //小程序跳转url 如果返回该字段，则使用js跳转该url，可发起微信小程序支付
}

[ContainerRpc("DEF.UCenter", "ChannelMeida", ContainerStateType.Stateless)]
public interface IContainerStatelessChannelMeida : IContainerRpc
{
    Task<string> MeidaWebhook(IEnumerable<KeyValuePair<string, string>> map_data);

    Task<PayChargeDetail> MeidaRequest(PayChargeDetail pay_charge_detail, string client_ip);
}