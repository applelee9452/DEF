#if !DEF_CLIENT

namespace DEF.IM;

// 玩家领取 SystemMail 事件
public class DbEvPlayerRecvSystemMail : EventBase
{
    public string SystenMailGuid { get; set; }
    public string RegionGuid { get; set; }
    public string PlayerGuid { get; set; }
    public string MailTitle { get; set; }
    public string RegionName { get; set; }
    public string PlayerNickName { get; set; }
}

#endif