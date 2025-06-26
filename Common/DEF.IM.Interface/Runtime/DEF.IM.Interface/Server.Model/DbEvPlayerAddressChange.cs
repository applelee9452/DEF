#if !DEF_CLIENT

namespace DEF.IM;

// 玩家收货地址变更事件
public class EvPlayerAddressChange : EventBase
{
    public string Name { get; set; }
    public string PhoneNum { get; set; }
    public string QQ { get; set; }
    public string Weixin { get; set; }
    public string Address { get; set; }
    public string EMail { get; set; }
}

#endif