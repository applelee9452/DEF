namespace DEF.UCenter;

public class DataPayCharge
{
    public string Id { get; set; }
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public string AppId { get; set; }// 为UCenterAppId，如King
    public string AccountId { get; set; }
    public ulong[] AgentParents { get; set; }
    public string PlayerGuid { get; set; }
    public int ItemTbId { get; set; }// 常规，首冲，打折，如何处理？
    public string ItemName { get; set; }// 可选参数，增加可读性
    public PayChargeStatus Status { get; set; }// 订单状态，唯一可变参数
    public PayPlatform Platform { get; set; } // 商城平台 GooglePlay AppStore
    public string Currency { get; set; }// 货币类型
    public long Amount { get; set; }// 数量，单位：分
    public string IAPProductId { get; set; }// 内购计费点
    public string Receipt { get; set; }// 凭据
    public string Transaction { get; set; }// 商店中的交易号
    public string PurchaseToken { get; set; }// Google支付token拿这个结单
    public bool IsSandbox { get; set; }// 是否是沙盒模式
    public string ThirdPartyPayOrderId { get; set; }// 第三方支付订单ID
    public string PayType { get; set; }// 支付方式
}

public class UCenterPlayerInfo
{
    public string PlayerGuid { get; set; }
    public string PlayerUid { get; set; }
    public string PlayerNickName { get; set; }

    public UCenterPlayerInfo()
    {
        PlayerNickName = "未知";
        PlayerUid = "未知";
    }
}

public class DataPayChargeView : DataPayCharge
{
    public DataPayChargeView()
    {
    }

    public DataPayChargeView(DataPayCharge data)
    {
        this.Id = data.Id;
        this.CreatedTime = data.CreatedTime;
        this.UpdatedTime = data.UpdatedTime;
        this.AppId = data.AppId;
        this.AccountId = data.AccountId;
        this.AgentParents = data.AgentParents;
        this.PlayerGuid = data.PlayerGuid;
        this.ItemTbId = data.ItemTbId;
        this.ItemName = data.ItemName;
        this.Status = data.Status;
        this.Platform = data.Platform;
        this.Currency = data.Currency;
        this.Amount = data.Amount;
        this.IAPProductId = data.IAPProductId;
        this.Receipt = data.Receipt;
        this.Transaction = data.Transaction;
        this.PurchaseToken = data.PurchaseToken;
        this.IsSandbox = data.IsSandbox;
        this.ThirdPartyPayOrderId = data.ThirdPartyPayOrderId;
        this.PayType = data.PayType;
    }

    public string PlayerUid { get; set; }
    public string PlayerNickName { get; set; }
    public string AgentId { get; set; }
    public string AgentNickName { get; set; }
}