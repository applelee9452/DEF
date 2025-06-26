using Microsoft.Extensions.Logging;
using System.Text;

namespace DEF.UCenter;

public class ContainerStatelessChannelAppleStoreIAP : ContainerStateless, IContainerStatelessChannelAppleStoreIAP
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

    async Task<AppleStoreVerifyResponse> IContainerStatelessChannelAppleStoreIAP.VerifyReceipt(PayVerifyChargeRequest request, string transaction, string iap_productid)
    {
        var response = new AppleStoreVerifyResponse()
        {
            PayErrorCode = PayErrorCode.Error,
            Transaction = string.Empty,
            IAPProductId = string.Empty,
            IsSandbox = false,
        };

        // todo，待整理
        bool is_sandbox = true;// Config.ConfigUCenter.AppStoreSandbox;
        string verify_url = is_sandbox ? "https://sandbox.itunes.apple.com/verifyReceipt" : "https://buy.itunes.apple.com/verifyReceipt";
        string request_data = "{\"receipt-data\":\"" + request.Receipt + "\"}";
        string response_data = string.Empty;

        using (var web_client = HttpClientFactory.CreateClient())
        {
            using var res = await web_client.PostAsync(verify_url, new StringContent(request_data, Encoding.UTF8, "application/json"));
            response_data = await res.Content.ReadAsStringAsync();
        }

        try
        {
            Newtonsoft.Json.Linq.JObject jobject = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(response_data);

            long status = (long)jobject["status"];
            if (status == 27017)
            {
                response.IsSandbox = true;
            }
            else if (status != 0)
            {
                Logger.LogError("订单校验错误，Status={0}", status);

                response.PayErrorCode = PayErrorCode.PayNetError;
                goto End;
            }

            Newtonsoft.Json.Linq.JArray jarr = (Newtonsoft.Json.Linq.JArray)jobject["receipt"]["in_app"];

            if (jarr.Count == 0)
            {
                Logger.LogError("订单校验错误，没有需要校验的订单");

                response.PayErrorCode = PayErrorCode.PayInvalidReceipt;
                goto End;
            }

            Newtonsoft.Json.Linq.JToken jtoken = null;
            foreach (var i in jarr)
            {
                if (i["transaction_id"].ToString() == request.Transaction)
                {
                    jtoken = i;
                    break;
                }
            }

            if (jtoken == null)
            {
                Logger.LogError("订单校验错误，没有找到transaction_id");

                response.PayErrorCode = PayErrorCode.PayInvalidReceipt;
                goto End;
            }

            string product_id = (string)jtoken["product_id"];
            uint quantity = Convert.ToUInt32(jtoken["quantity"]);
            if (quantity != 1)
            {
                Logger.LogError("订单校验错误，quantity={0}", quantity);

                response.PayErrorCode = PayErrorCode.PayInvalidOrder;
                goto End;
            }

            if (transaction == request.Transaction && iap_productid == product_id)
            {
                // 重复校验
                //Logger.LogError("重复校验 ChargeId={0} Platform={1} Amount={2}",
                //    charge_db_cache.ChargeId, charge_db_cache.PlayerGuid, charge_db_cache.Amount);

                response.PayErrorCode = PayErrorCode.PayOrderRepeat;
                goto End;
            }
            else
            {
                response.Transaction = request.Transaction;
                response.IAPProductId = product_id;
            }
        }
        catch (Exception e)
        {
            Logger.LogError("订单校验错误，" + e.ToString());

            response.PayErrorCode = PayErrorCode.PayInvalidOrder;
            goto End;
        }

        response.PayErrorCode = PayErrorCode.NoError;

    End:

        return response;
    }
}