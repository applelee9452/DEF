using Microsoft.Extensions.Logging;
using System.Text;

namespace DEF.UCenter;

public class ContainerStatelessChannelKilat : ContainerStateless, IContainerStatelessChannelKilat
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }

    string KilatPayDevRsaPublicKey;// KilatPay开发公钥 
    string KilatPayProRsaPublicKey;//KilatPay发布公钥
    string PokerilyDevRsaPrivateKey;// Pokerily 开发密钥
    string PokerilyProRsaPrivateKey;// Pokerily 发布密钥

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        KilatPayDevRsaPublicKey = File.ReadAllText("./PayConfig/KilatPay/kilatpay_dev_rsa_public_key.pem");
        KilatPayProRsaPublicKey = File.ReadAllText("./PayConfig/KilatPay/kilatpay_pro_rsa_public_key.pem");
        PokerilyDevRsaPrivateKey = File.ReadAllText("./PayConfig/KilatPay/xxx_dev_rsa_1024_priv_pkcs8.pem");
        PokerilyProRsaPrivateKey = File.ReadAllText("./PayConfig/KilatPay/xxx_pro_rsa_1024_priv_pkcs8.pem");

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    async Task<string> IContainerStatelessChannelKilat.KilatPayWebhookRequest(Dictionary<string, string> map_data, string client_ip)
    {
        var kilat_data = map_data["data"];

        Logger.LogInformation("GrainChannelKilat.KilatPayWebhookRequest() 回调数据：" + kilat_data);

        var map_data_info = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(kilat_data);

        string need_verify_content = Utils.GenSignOrigin(map_data_info);

        map_data_info.TryGetValue("app_id", out string app_id);
        map_data_info.TryGetValue("out_trade_no", out string out_trade_no);
        map_data_info.TryGetValue("amount", out string amount);
        map_data_info.TryGetValue("pay_type", out string pay_type);
        map_data_info.TryGetValue("status", out string status);//订单状态： 01 未支付， 02 已支付， 06 支付失败
        map_data_info.TryGetValue("time", out string time);
        map_data_info.TryGetValue("sign", out string sign);

        bool verify_ret = false;

        try
        {
            // todo，待整理
            //verify_ret = RSAFromPkcs8.verify(need_verify_content, sign, _getKilatPayRsaPublicKey());
        }
        catch (Exception e)
        {
            Logger.LogInformation("Kilat 验证结果Exception:" + e);
        }

        Logger.LogInformation("Kilat 验证结果:" + verify_ret);

        if (!verify_ret) return string.Empty;
        if (status != "02") return string.Empty;

        int amount_int = 0;
        if (!string.IsNullOrEmpty(amount))
        {
            amount_int = Convert.ToInt32(amount);
        }

        // todo，待整理
        var container = GetContainerRpc<IContainerStatelessPayService>();
        return await container.PayChargeFinishByServer(out_trade_no, amount_int, client_ip, "SUCCESS");
    }

    // 获得WebUrl地址
    Task<string> IContainerStatelessChannelKilat.GetWebUrl(string charge_id, long amount)
    {
        // todo，待整理
        string web_string = string.Empty;

        var map_data_info = new Dictionary<string, string>();
        map_data_info["app_id"] = string.Empty;//Config.ConfigUCenter.KilatPayAppId;
        map_data_info["out_trade_no"] = charge_id;
        map_data_info["amount"] = amount.ToString();
        map_data_info["timestamp"] = Utils.ConvertDateTimeToInt(DateTime.UtcNow).ToString();

        string need_sign_content = Utils.GenSignOrigin(map_data_info);
        string sign = string.Empty;//RSAFromPkcs8.sign(need_sign_content, Config.ConfigUCenter.KilatPaySandbox ? Config.ConfigUCenter.PokerilyDevRSAPrivateKey : Config.ConfigUCenter.PokerilyProRSAPrivateKey);
        //string sign = RSAFromPkcs8.sign(need_sign_content, Config.ConfigUCenter.KilatPaySandbox ? PokerilyDevRsaPrivateKey : PokerilyProRsaPrivateKey);
        map_data_info["sign"] = System.Web.HttpUtility.UrlEncode(sign, Encoding.UTF8);

        string url = string.Empty;///Config.ConfigUCenter.KilatPaySandbox ? Config.ConfigUCenter.KilatPayDevUrl : Config.ConfigUCenter.KilatPayProUrl;
        web_string = string.Format("{0}payer/home?{1}", url, Utils.GenUrlStrings(map_data_info));

        return Task.FromResult(web_string);
    }

    private string _getKilatPayRsaPublicKey()
    {
        //if (Config.ConfigUCenter.KilatPaySandbox)
        //{
        //    return Config.ConfigUCenter.KilatPayDevRSAPublicKey;
        //}
        //else
        //{
        //    return Config.ConfigUCenter.KilatPayProRSAPublicKey;
        //}

        // todo，待整理
        return string.Empty;
    }
}