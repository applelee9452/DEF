using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DEF.UCenter;

public class ContainerStatelessChannelGooglePlayIAP : ContainerStateless, IContainerStatelessChannelGooglePlayIAP
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

    async Task<GooglePlayVerifyResponse> IContainerStatelessChannelGooglePlayIAP.VerifyReceipt(PayVerifyChargeRequest request, string transaction, string iap_productid)
    {
        var response = new GooglePlayVerifyResponse()
        {
            PayErrorCode = PayErrorCode.Error,
            Transaction = string.Empty,
            IAPProductId = string.Empty,
            IsSandbox = false,
        };

        // 有效订单返回格式示例
        //{
        //    "kind": "androidpublisher#productPurchase",
        //    "purchaseTimeMillis": "1539054135375",  // 支付时间, 毫秒
        //    "purchaseState": 0, // 是否付费: 0 已支付, 1 取消
        //    "consumptionState": 0, // 是否被消费: 0 未消费, 1 已消费
        //    "developerPayload": "xxxxx", // 开发者透传参数
        //    "orderId": "GPA.3337-xxxx-xxxx-xxxx", // 谷歌订单号
        //    "purchaseType": 0 // 支付类型:  0 测试, 1 真实
        //}

        // 无效订单返回格式示例
        //{
        //    "error": {
        //        "errors": [
        //         {
        //       "domain": "global",
        //                                  "reason": "invalid",
        //                                  "message": "Invalid Value"
        //      }
        //     ],
        //     "code": 400,
        //     "message": "Invalid Value"
        //    }
        //}

        try
        {
            //Dictionary<string, object> dict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(request.Receipt);
            //Logger.LogInformation("订单信息:", request.Receipt);

            //string transaction_id = string.Empty;
            //dict.TryGetValue("orderId", out var obj_str);
            //if (obj_str != null)
            //{
            //    transaction_id = dict["orderId"].ToString();
            //}
            //string packageName = dict["packageName"].ToString();
            //string product_id = dict["productId"].ToString();
            //string purchaseToken = dict["purchaseToken"].ToString();
            //string purchaseTime = dict["purchaseTime"].ToString();

            string transaction_id = request.OrderId;

            string packageName = request.PackageName;
            string product_id = request.IAPProductId;
            string purchaseToken = request.PurchaseToken;
            //string purchaseTime = dict["purchaseTime"].ToString();

            // 玩家自定义参数
            //string developerPayload = dict["developerPayload"].ToString();

            var grain_googleplay_accesstoken = GetContainerRpc<IContainerStatelessChannelGooglePlayAccessToken>();
            string googleplay_accesstoken = await grain_googleplay_accesstoken.GetAccessToken();

            // todo，待整理
            string url = string.Format(UCenterOptions.Value.GooglePayVerifyUrl, packageName, product_id, purchaseToken, googleplay_accesstoken);
            Logger.LogInformation(url);

            string response_data = string.Empty;
            using (var web_client = HttpClientFactory.CreateClient())
            {
                HttpRequestMessage request_msg = new HttpRequestMessage();
                request_msg.RequestUri = new Uri(url);
                request_msg.Headers.Add("Accept", "application/json");

                using var res = await web_client.SendAsync(request_msg);
                response_data = await res.Content.ReadAsStringAsync();

                Logger.LogInformation(response_data);
            }

            Newtonsoft.Json.Linq.JObject jobject = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(response_data);

            string purchaseTimeMillis = string.Empty;
            int purchaseState = 0;
            int consumptionState = 0;
            if (!jobject.ContainsKey("error"))
            {
                purchaseState = int.Parse(jobject["purchaseState"].ToString());
                consumptionState = int.Parse(jobject["consumptionState"].ToString());

                jobject.TryGetValue("purchaseType", out var purchase_type_t);
                if (purchase_type_t != null && purchase_type_t.ToString() == "0")
                {
                    response.IsSandbox = true;
                }

                response.IAPProductId = product_id;
                response.Transaction = transaction_id;
                response.PurchaseToken = purchaseToken;
            }
            else
            {
                string code = jobject["code"].ToString();
                string message = jobject["message"].ToString();
                Logger.LogError(message);

                response.PayErrorCode = PayErrorCode.PayInvalidOrder;
                goto End;
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