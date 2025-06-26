#if !DEF_CLIENT

namespace DEF.IM;

// 玩家地址，用于快递收货等
public class DbDataPlayerAddress : DataBase
{
    public string Name { get; set; }
    public string PhoneNum { get; set; }
    public string QQ { get; set; }
    public string Weixin { get; set; }
    public string Address { get; set; }
    public string EMail { get; set; }
}

#endif