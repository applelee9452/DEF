using MemoryPack;

namespace DEF.UCenter
{
    // 兑换订单状态
    public enum PayExchangeStatus
    {
        Create = 0,// 创建状态
        Debit,// 扣款状态
        GiveItem,// 发货状态
        Cancel,// 取消状态
        Refund,// 退款状态
        End,// 结束状态
    }

    // 兑换订单简要信息
    [MemoryPackable]
    public partial class PayExchangeInfo
    {
        public string ExchangeId { get; set; }
        public PayPlatform Platform { get; set; }// 商城平台 GooglePlay AppStore
        public PayExchangeStatus Status { get; set; }
        public int Amount { get; set; }// 数量，单位：分
    }

    // 兑换订单详情信息
    [MemoryPackable]
    public partial class PayExchangeDetail
    {
        public string AppId { get; set; }// 为UCenterAppId，如King
        public string AccountId { get; set; }
        public string ExchangeId { get; set; }// 订单唯一Id，由UCenter生成
        public PayExchangeStatus Status { get; set; }// 订单状态，唯一可变参数
        public int ItemTbId { get; set; }// 常规，首冲，打折，如何处理？
        public string ItemName { get; set; }// 可选参数，增加可读性
        public PayPlatform Platform { get; set; }// 商城平台 GooglePlay AppStore
        public int Amount { get; set; }// 数量，单位：分
        public string Currency { get; set; }// 货币类型
        public string IAPProductId { get; set; }// 内购计费点
        public string Receipt { get; set; }// 凭据
        public string Transaction { get; set; }// 商店中的交易号
        public string PurchaseToken { get; set; }// google支付token拿这个结单
        public string WebPayUrl { get; set; }// 请求web支付的字符串参数
    }

    // 兑换订单创建请求
    [MemoryPackable]
    public partial class PayExchangeCreateRequest
    {
        public string AccountId { get; set; }
        public string Token { get; set; }// 为账号token
        public string AppId { get; set; }
        public string PlayerGuid { get; set; }
        public PayPlatform Platform { get; set; }// 商城平台 GooglePlay AppStore
        public int ItemTbId { get; set; }// 道具id
        public string ItemName { get; set; }// 道具名字
        public string Currency { get; set; }// 货币类型
        public int Amount { get; set; }// 数量，单位：分
    }

    // 兑换订单校验请求
    [MemoryPackable]
    public partial class PayExchangeVerifyRequest
    {
        public string ExchangeId { get; set; }// 订单唯一Id，由UCenter生成
        public string Receipt { get; set; }// 凭据
        public string Transaction { get; set; }// ?
        public string PurchaseToken { get; set; }// google支付token拿这个结单
        public string Token { get; set; }// 为账号token
        public string AccountId { get; set; }
    }

    // 兑换订单扣款
    [MemoryPackable]
    public partial class PayExchange4Debit
    {
        public string ExchangeId { get; set; }
        public string AccountId { get; set; }
        public string PlayerGuid { get; set; }
        public int ItemTbId { get; set; }
        public string ItemName { get; set; }// 可选参数，增加可读性
        public string Currency { get; set; }// 货币类型
        public long Amount { get; set; }// 货币数量
        public bool IsSandbox { get; set; }
    }

    // 兑换订单发货
    [MemoryPackable]
    public partial class PayExchange4GiveItem
    {
        public string ExchangeId { get; set; }
        public string AccountId { get; set; }
        public string PlayerGuid { get; set; }
        public int ItemTbId { get; set; }
        public long Amount { get; set; }
        public bool IsSandbox { get; set; }
    }
}