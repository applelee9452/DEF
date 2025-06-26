using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DEF.UCenter;

public class ContainerStatelessChannelMeida : ContainerStateless, IContainerStatelessChannelMeida
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    string NotifyUrl { get; set; }

    // 商家 ID 商家 Key 应该读取配置文件
    const string PID = "6145";
    const string Key = "Mj3ekxb6BXmfJZjEkaW2D1WW261Df023";
    const string PayApiUrl = "https://v63kdwa.bjpinjing.com//mapi.php";

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;
        NotifyUrl = UCenterContext.Instance.UCenterOptions.Value.MeidaPayNotifyUrl;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    async Task<PayChargeDetail> IContainerStatelessChannelMeida.MeidaRequest(PayChargeDetail pay_charge_detail, string client_ip)
    {
        float money = pay_charge_detail.Amount * 0.01f;
        // float money = 1;
        var data = new Dictionary<string, string>()
        {
            { "pid", PID },
            { "type", pay_charge_detail.PayType },// alipay wxpay
            { "out_trade_no", pay_charge_detail.ChargeId },
            { "notify_url",  NotifyUrl},
            { "return_url", "meida_pay_success" },
            { "name", pay_charge_detail.ItemName },
            { "money", money.ToString() },
            { "clientip", client_ip},
            { "device", pay_charge_detail.Device },
            { "rawurl", "1" },
            { "param", "" },
            { "sign", "" },
            { "sign_type", "MD5" }
        };

        data["sign"] = HashData(data);

        using var client = HttpClientFactory.CreateClient();

        var content = new FormUrlEncodedContent(data);

        try
        {
            var response = await client.PostAsync(PayApiUrl, content);

            var result = await response.Content.ReadFromJsonAsync<MeidaRequestResult>();

            if (result?.Code == 1 && !string.IsNullOrEmpty(result?.PayUrl))
            {
                pay_charge_detail.ReqWebPayUrlString = result.PayUrl;
                pay_charge_detail.ThirdPartyPayOrderId = result.TradeNo;
            }
            else
            {
                pay_charge_detail.Status = PayChargeStatus.Error;
                DbEvPayChargeError charge_error = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = typeof(DbEvPayChargeError).Name,
                    EventTm = DateTime.UtcNow,
                    ChargeId = pay_charge_detail.ChargeId,
                    PayPlatform = pay_charge_detail.Platform,
                    PayErrorCode = PayErrorCode.PayChargeCreateFail,
                    Message = $"meida request error code {result.Code} msg {result.Message}"
                };
                await Db.InsertAsync(StringDef.DbCollectionEvPayChargeError, charge_error);
            }
        }
        catch (Exception e)
        {
            Logger.LogError($"请求 Meida 创建订单异常 \n{e}");

            pay_charge_detail.Status = PayChargeStatus.Error;

            DbEvPayChargeError charge_error = new()
            {
                Id = Guid.NewGuid().ToString(),
                EventType = typeof(DbEvPayChargeError).Name,
                EventTm = DateTime.UtcNow,
                ChargeId = pay_charge_detail.ChargeId,
                PayPlatform = pay_charge_detail.Platform,
                PayErrorCode = PayErrorCode.PayChargeCreateFail,
                Message = $"meida request exception : {e}"
            };
            await Db.InsertAsync(StringDef.DbCollectionEvPayChargeError, charge_error);
        }

        return pay_charge_detail;
    }

    async Task<string> IContainerStatelessChannelMeida.MeidaWebhook(IEnumerable<KeyValuePair<string, string>> map_data)
    {
        Logger.LogInformation("ContainerStatelessChannelMeida.MeidaWebhook()");

        Dictionary<string, string> dic_data = map_data.ToDictionary();
        var sign_value = HashData(map_data);

        dic_data.TryGetValue("sign", out string sign);

        // 签名对不上的话可能中间出现了篡改
        if (sign != sign_value)
        {
            Logger.LogError("sign check fail");

            return "param error";
        }

        dic_data.TryGetValue("out_trade_no", out string charge_id);
        //dic_data.TryGetValue("trade_no", out string third_party_pay_order_id);
        dic_data.TryGetValue("trade_status", out string trade_status);

        if (trade_status != "TRADE_SUCCESS")
        {
            // 这里根据这个状态处理不成功的情况，但一般来说应该都是要结束订单
            Logger.LogError($"trade_status fail {trade_status}");

            return "success";
        }

        var charge_db = await Db.ReadAsync<DataPayCharge>(a => a.Id == charge_id, StringDef.DbCollectionDataPayCharge);
        if (charge_db == null)
        {
            Logger.LogError($"订单不存在 ChargeId = {charge_id}");

            return $"no charge id:{charge_id}";
        }

        if (charge_db.Status >= PayChargeStatus.Cancel)
        {
            return $"PayErrorCode : {charge_db.Status}";
        }

        var rpc = GetContainerRpc<IContainerStatelessPayService>();
        PayChargeInfo charge_info = await rpc.PayChargeSuccessComplete(charge_db.AccountId, "token", charge_id, "client_ip");

        if (charge_info.ErrorCode != PayErrorCode.NoError)
        {
            return $"PayErrorCode : {charge_info.ErrorCode}";
        }

        // 这里需要返回，不然对方不知道结果
        return "success";
    }

    static string HashData(IEnumerable<KeyValuePair<string, string>> paras)
    {
        var sorted_param = paras.OrderBy(i => i.Key);
        var sign_str = string.Join("&", sorted_param.Where(x => x.Key != "sign" && x.Key != "sign_type" && !string.IsNullOrEmpty(x.Value)).Select(x => $"{x.Key}={x.Value}"));
        sign_str += Key;
        var bytes = MD5.HashData(Encoding.UTF8.GetBytes(sign_str));

        return Convert.ToHexStringLower(bytes).Replace("-", "");
    }
}