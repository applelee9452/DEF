namespace DEF.UCenter;

public class ConfigApp : DataBase
{
    public string Name { get; set; }
    public string WechatAppId { get; set; }
    public string WechatAppSecret { get; set; }
    public string WechatMpAppId { get; set; }
    public string WechatMpAppSecret { get; set; }
    public string TaptapAppId { get; set; }
    public string AliOssBucketName { get; set; }
    public string FacebookAppId { get; set; }
    public string FacebookSecret { get; set; }
    public string FacebookAccessToken { get; set; }
    public string GoogleAppId { get; set; }
}