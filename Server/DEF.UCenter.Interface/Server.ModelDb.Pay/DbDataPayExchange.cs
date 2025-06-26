namespace DEF.UCenter;

public class DataPayExchange
{
    public string Id { get; set; }// Guid，没有用在业务上
    public string ExchangeId { get; set; }// 订单唯一Id，由UCenter生成，Guid
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public string AppId { get; set; }// 为UCenterAppId，如King
    public string AccountId { get; set; }
    public string PlayerGuid { get; set; }
    public int ItemTbId { get; set; }// 常规，首冲，打折，如何处理？
    public string ItemName { get; set; }// 可选参数，增加可读性
    public PayExchangeStatus Status { get; set; }// 订单状态，唯一可变参数
    public PayPlatform Platform { get; set; } // 商城平台 GooglePlay AppStore
    public string Currency { get; set; }// 货币类型
    public int Amount { get; set; }// 数量，单位：分
    public string IAPProductId { get; set; }// 内购计费点
    public string Receipt { get; set; }// 凭据
    public string Transaction { get; set; }// 商店中的交易号
    public string PurchaseToken { get; set; }// Google支付Token拿这个结单
    public bool IsSandbox { get; set; }// 是否是沙盒模式
}