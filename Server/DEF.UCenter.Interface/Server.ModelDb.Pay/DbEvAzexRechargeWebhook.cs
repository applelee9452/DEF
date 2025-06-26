namespace DEF.UCenter;

public class EvAzexRechargeWebhook
{
    public string Id { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public string Message { get; set; }
    public string StackTrace { get; set; }
}