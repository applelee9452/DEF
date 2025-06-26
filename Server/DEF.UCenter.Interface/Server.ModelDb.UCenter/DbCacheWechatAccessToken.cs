namespace DEF.UCenter;

public class CacheWechatAccessToken : DataBase
{
    public string AppId { get; set; }
    public string OpenId { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}