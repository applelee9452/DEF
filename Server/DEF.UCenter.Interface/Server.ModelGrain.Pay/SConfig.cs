namespace DEF.UCenter;

public class ConfigPay
{
    public bool EnableGooglePlayIAP { get; set; }
    public bool EnableAppleStoreIAP { get; set; }
    public bool EnableEnjoy { get; set; }
    public bool EnalbeBeeCloud { get; set; }
    public string GiveItemWebhook { get; set; }// 发货的Webhook

    public ConfigPay()
    {
        EnableGooglePlayIAP = true;
        EnableAppleStoreIAP = true;
        EnableEnjoy = true;
        EnalbeBeeCloud = true;
        GiveItemWebhook = "http://lobby-local:10065/lobby/paychargewebhook";
    }

    public Dictionary<string, string> Get()
    {
        Dictionary<string, string> m = new Dictionary<string, string>();

        m["EnableGooglePlayIAP(bool)"] = EnableGooglePlayIAP.ToString();
        m["EnableAppleStoreIAP(bool)"] = EnableAppleStoreIAP.ToString();
        m["EnableEnjoy(bool)"] = EnableEnjoy.ToString();
        m["EnalbeBeeCloud(bool)"] = EnalbeBeeCloud.ToString();
        m["GiveItemWebhook(string)"] = GiveItemWebhook.ToString();

        return m;
    }

    public void Set(Dictionary<string, string> m)
    {
        EnableGooglePlayIAP = bool.Parse(m["EnableGooglePlayIAP(bool)"]);
        EnableAppleStoreIAP = bool.Parse(m["EnableAppleStoreIAP(bool)"]);
        EnableEnjoy = bool.Parse(m["EnableEnjoy(bool)"]);
        EnalbeBeeCloud = bool.Parse(m["EnalbeBeeCloud(bool)"]);
        GiveItemWebhook = m["GiveItemWebhook(string)"];
    }
}