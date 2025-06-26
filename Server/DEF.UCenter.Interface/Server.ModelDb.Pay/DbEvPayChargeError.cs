namespace DEF.UCenter;

public abstract class EventBase : DataBase
{
    public string EventType { get; set; }// 事件类型
    public DateTime EventTm { get; set; }// 事件时间
}

// 落地更多支付相关的信息，就算冗余也是值得的
// 订单的状态变化过程与时间在 ChargeDetail 等结构中是无法储存的 
public class DbEvPayChargeError : EventBase
{
    public string ChargeId { get; set; }
    public PayPlatform PayPlatform { get; set; }
    public PayErrorCode PayErrorCode { get; set; }
    public string Message{ get; set; }

}