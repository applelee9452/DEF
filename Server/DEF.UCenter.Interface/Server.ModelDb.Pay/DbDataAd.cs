namespace DEF.UCenter;

public class DataAd
{
    public string Id { get; set; }// Guid唯一性
    public string TransactionId { get; set; }// 第三方广告的 trans_id
    public AdStatus AdStatus { get; set; }// 广告状态
    public AdType AdType { get; set; }// 广告类型
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string AdUnit { get; set; }// 广告ID
    public string AccountId { get; set; }
    public string PlayerGuid { get; set; }
    public int RewardAmount { get; set; }
    public string RewardItem { get; set; }
    public float Timeout { get; set; }// 5秒倒计时
}