namespace DEF.UCenter;

public class DataAccountTaptap : DataBase
{
    public string AccountId { get; set; }
    public string Unionid { get; set; }
    public string OpenId { get; set; }
    public string AppId { get; set; }
    public string NickName { get; set; }
    public GenderType Gender { get; set; }
    public string Province { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Headimgurl { get; set; }
}