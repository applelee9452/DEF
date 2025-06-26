namespace DEF.UCenter;

// 账号登陆登出事件
public class EvAccountLoginLogout
{
    public string Id { get; set; }// 事件Id
    public DateTime Dt { get; set; } = DateTime.UtcNow;// 事件产生的时间点
    public string AccountId { get; set; }// 账号Id
    public bool LoginOrLogout { get; set; }// 登陆还是登出，Login=true，Logout=false
    public string AccountName { get; set; }
    public string DeviceId { get; set; }// 设备Id
    public string ClientIp { get; set; }// 用户IP
}