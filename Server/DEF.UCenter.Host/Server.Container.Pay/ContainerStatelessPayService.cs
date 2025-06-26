using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Text;

namespace DEF.UCenter;

public class ContainerStatelessPayService : ContainerStateless, IContainerStatelessPayService
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

    // 充值订单，请求获取详情
    Task<PayChargeDetail> IContainerStatelessPayService.PayGetChargeDetail(string acc_id, string token, string charge_id, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatefulCharge>(acc_id);
        return container.PayChargeGetDetail(charge_id, client_ip);
    }

    // 充值订单，请求创建
    Task<PayChargeDetail> IContainerStatelessPayService.PayCreateCharge(PayCreateChargeRequest request, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatefulCharge>(request.AccountId);
        return container.PayChargeCreate(request, client_ip);
    }

    // 充值订单，请求取消
    Task<PayChargeInfo> IContainerStatelessPayService.PayCancelCharge(string acc_id, string token, string charge_id, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatefulCharge>(acc_id);
        return container.PayChargeCancel(charge_id, client_ip);
    }

    // Ack
    Task<PayChargeInfo> IContainerStatelessPayService.PayAckCharge(string acc_id, string token, string charge_id, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatefulCharge>(acc_id);
        return container.PayChargeAck(charge_id, client_ip);
    }

    // 充值订单，请求结束
    Task<PayChargeInfo> IContainerStatelessPayService.PayChargeSuccessComplete(string acc_id, string token, string charge_id, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatefulCharge>(acc_id);
        return container.PayChargeSuccessComplete(charge_id, client_ip);
    }

    // 兑换订单，请求创建
    Task<string> IContainerStatelessPayService.PayExchangeCreate(PayExchangeCreateRequest request, string client_ip)
    {
        var container = GetContainerRpc<IContainerStatefulExchange>(request.AccountId);
        return container.PayExchangeCreate(request, client_ip);
    }

    // 第三方服务器通知完成充值订单
    async Task<string> IContainerStatelessPayService.PayChargeFinishByServer(string data, string client_ip)
    {
        var enjoy_data = Newtonsoft.Json.JsonConvert.DeserializeObject<PayEnjoyWebhookData>(data);

        if (enjoy_data == null || string.IsNullOrEmpty(enjoy_data.payload))
        {
            return string.Empty;
        }

        string charge_id = enjoy_data.payload;

        var db_charge = await Db.ReadAsync<DataPayCharge>(
            a => a.Id == charge_id,
            StringDef.DbCollectionDataPayCharge);

        if (db_charge == null)
        {
            Logger.LogError("订单不存在");

            return string.Empty;
        }
        else
        {
            //var grain = GrainFactory.GetGrain<IGrainCharge>(Guid.Parse(db_charge.AccountId));
            //await grain.PayChargeFinishByServer(db_charge, client_ip);
        }

        return "{\"code\":1000}";
    }

    // 第三方服务器通知完成充值订单
    async Task<string> IContainerStatelessPayService.PayChargeFinishByServer(string charge_id, int amount, string client_ip, string ret_value)
    {
        if (string.IsNullOrEmpty(charge_id))
        {
            return string.Empty;
        }

        var db_charge = await Db.ReadAsync<DataPayCharge>(
            a => a.Id == charge_id,
            StringDef.DbCollectionDataPayCharge);

        if (db_charge == null)
        {
            Logger.LogError("订单不存在");

            return string.Empty;
        }
        else
        {
            if (db_charge.Amount != amount)
            {
                Logger.LogError("金额不对 we amount={0} rec amount={1}", db_charge.Amount, amount);
                return string.Empty;
            }
            else
            {
                //var grain = GrainFactory.GetGrain<IGrainCharge>(Guid.Parse(db_charge.AccountId));
                //await grain.PayChargeFinishByServer(db_charge, client_ip);
            }
        }

        return ret_value;
    }

    // 获得公钥
    async Task<string> getPublicKey(string key_id)
    {
        var url = "https://www.gstatic.com/admob/reward/verifier-keys.json";
        string response_data = string.Empty;
        using (var web_client = HttpClientFactory.CreateClient())
        {
            HttpRequestMessage request_msg = new()
            {
                RequestUri = new Uri(url)
            };
            request_msg.Headers.Add("Accept", "application/json");

            using var res = await web_client.SendAsync(request_msg);
            response_data = await res.Content.ReadAsStringAsync();

            Logger.LogInformation(response_data);
        }

        Newtonsoft.Json.Linq.JObject jobject = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(response_data);

        string publick_key = "";
        if (jobject.ContainsKey("keys"))
        {
            var obj_keys = jobject["keys"];
            if (obj_keys != null)
            {
                foreach (var tmp in obj_keys)
                {
                    if (tmp.Count() >= 3)
                    {
                        var ob_key = tmp.ElementAt(0);
                        var key = ob_key.ElementAt(0).ToString();
                        if (key != key_id) continue;
                        var tmp2 = tmp.ElementAt(1);
                        publick_key = tmp2.ElementAt(0).ToString();
                        break;
                    }
                }
            }
        }
        return publick_key;
    }

    // 广告，客户端请求创建
    Task<string> IContainerStatelessPayService.AdCreate(string acc_id)
    {
        var container = GetContainerRpc<IContainerStatefulCharge>(acc_id);
        return container.AdCreate();
    }

    // 广告，客户端观看完成
    Task<bool> IContainerStatelessPayService.AdFinishedShow(string acc_id, string ad_guid)
    {
        var container = GetContainerRpc<IContainerStatefulCharge>(acc_id);
        return container.AdFinishedShow(ad_guid);
    }

    // 广告，客户端取消观看广告
    Task<bool> IContainerStatelessPayService.AdCancel(string acc_id, string ad_guid)
    {
        var container = GetContainerRpc<IContainerStatefulCharge>(acc_id);
        return container.AdCancel(ad_guid);
    }

    // 广告，Admob回调
    async Task<string> IContainerStatelessPayService.AdmobWebhookRequest(string ad_json)
    {
        //ad_network: '5450213213286189855',
        //ad_unit: '1234567890',
        //custom_data: 'test',
        //reward_amount: '1',
        //reward_item: 'Reward',
        //timestamp: '1642779307809',
        //transaction_id: '123456789',
        //user_id: '123456',
        //signature: 'MEUCIQDLRiDEWHa7mwkBE3So8sIvC-UQ3IqZ50JzfqEMSePzOwIgfkPlOBl2wCuR9lTDvPY_2hoajoCcz-4EyEuFk4vOhtw',
        //key_id: '3335741209'

        // DataAdmob
        string ret_value = "OK";
        Logger.LogInformation("ad_json ={0}", ad_json);
        var ad_info = LitJson.JsonMapper.ToObject(ad_json);
        if (ad_info.ContainsKey("ad_network"))
        {
            var ad_network = ad_info["ad_network"].ToString();
            var ad_unit = ad_info["ad_unit"].ToString();
            var ad_guid = ad_info["custom_data"].ToString();
            var reward_amount = ad_info["reward_amount"].ToString();
            var reward_item = ad_info["reward_item"].ToString();
            var timestamp = ad_info["timestamp"].ToString();
            var transaction_id = ad_info["transaction_id"].ToString();
            var account_id = ad_info["user_id"].ToString();
            var signature_str = ad_info["signature"].ToString();
            var key_id = ad_info["key_id"].ToString();

            var pubkeyString = await getPublicKey(key_id);

            var pemreader = new PemReader(new StringReader(pubkeyString));
            var pubkey = (AsymmetricKeyParameter)pemreader.ReadObject();

            var data = string.Format("ad_network={0}&ad_unit={1}&reward_amount={2}&reward_item={3}&timestamp={4}&transaction_id={5}", ad_network, ad_unit, reward_amount, reward_item, timestamp, transaction_id);
            //var data = "ad_network=5450213213286189855&ad_unit=1234567890&custom_data=106&reward_amount=1&reward_item=Reward&timestamp=1621586055036&transaction_id=123456789&user_id=11111";
            StringBuilder signature = new StringBuilder();
            signature.Append(signature_str);
            //对base64格式化，替换和补位
            signature = signature.Replace('-', '+').Replace('_', '/');
            int remainder = signature.Length % 4;
            if (remainder > 0)
            {
                signature.Append('=', 4 - remainder);
            }
            var signatureBytes = Convert.FromBase64String(signature.ToString());

            // Verify using the public key
            var signer = SignerUtilities.GetSigner("SHA-256withECDSA");
            signer.Init(false, pubkey);
            signer.BlockUpdate(Encoding.UTF8.GetBytes(data), 0, data.Length);
            var success = signer.VerifySignature(signatureBytes);

            if (success)
            {
                //Console.WriteLine("admob广告验证成功");
            }
            else
            {
                //Console.WriteLine("admob广告验证失败");
            }

            var container = GetContainerRpc<IContainerStatefulCharge>(account_id);
            await container.AdCallback(success, ad_guid, account_id, ad_network, ad_unit, transaction_id, key_id, reward_item, Convert.ToInt32(reward_amount));
        }
        return ret_value;
    }

    public static string SHA256(string input)
    {
        System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashedBytes = sha.ComputeHash(inputBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    // 广告，GroMore回调
    async Task<string> IContainerStatelessPayService.GroMoreWebhookRequest(string user_id, string trans_id, int reward_amount, string reward_name, string sign, string prime_rit, string ad_order_id)
    {
        // m - key:transId
        string ad_key = "e5823f031c32e03d8e040cff289d105c";// 穿山甲配置的回调地方 获取广告 key值
        bool verify = false;
        int reason_int = 1000;
        // 签名验证
        string com_sign = SHA256(ad_key + ":" + trans_id);
        if (com_sign.Equals(sign))
        {
            verify = true;
            reason_int = 0;
        }

        Logger.LogInformation("穿山甲聚合回调 验证结果 ={0} com_sign={1} 对方的sign={2}", verify, com_sign, sign);

        // DataAd
        var container = GetContainerRpc<IContainerStatefulCharge>(user_id);
        var ret_ = await container.GroMoreCallback(verify, user_id, trans_id, Convert.ToInt32(reward_amount), reward_name, prime_rit, ad_order_id);

        // 穿山甲要求这样返回
        var ret = new
        {
            is_verify = ret_,
            reason = reason_int
        };

        string ret_value = Newtonsoft.Json.JsonConvert.SerializeObject(ret);

        return ret_value;
    }
}