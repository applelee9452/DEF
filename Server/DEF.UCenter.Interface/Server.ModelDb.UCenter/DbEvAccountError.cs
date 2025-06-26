namespace DEF.UCenter;

public class EvAccountError : DataBase
{
    public string AccountName { get; set; }
    public string AccountId { get; set; }
    public string ClientIp { get; set; }
    public UCenterErrorCode Code { get; set; }
    public string LoginArea { get; set; }
    public string Message { get; set; }
}