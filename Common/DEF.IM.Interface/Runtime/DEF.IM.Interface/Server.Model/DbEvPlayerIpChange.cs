#if !DEF_CLIENT

namespace DEF.IM;

// 玩家Ip改变事件，Ip由没有转为有时不记录，由有转为没有时记录
public class EvPlayerIpChange : EventBase
{
    public string PlayerGuid { get; set; }
    public string ClientIp { get; set; }
}

#endif