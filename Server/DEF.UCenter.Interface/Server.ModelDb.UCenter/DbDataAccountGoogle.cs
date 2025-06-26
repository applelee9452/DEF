namespace DEF.UCenter;

public class DataAccountGoogle : DataBase
{
    public string AccountId { get; set; }
    public string UserId { get; set; }
    public string AppId { get; set; }
    public string NickName { get; set; }
    public GenderType Gender { get; set; }
    public string Province { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Headimgurl { get; set; }
}