#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System.Collections.Generic;

namespace DEF.UCenter
{
    // 广告状态
    public enum AdStatus
    {
        AdCreate = 0,// 已创建未完成
        AdClientShowFinished,// 客户端看完
        AdCancel,// 已取消（失败 超时取消，人为取消）
        AdVerifyFailed,// 校验失败（失败）
        AdVerifySuccess,// 校验成功 等待发货（成功）
        AdGiveItemSuccess,// 已发货（成功）
    }

    // 广告类型
    public enum AdType
    {
        Admob = 0,// Admob广告
        GroMore = 1,// 穿山甲GroMore
    }

    // 充值订单状态
    public enum PayChargeStatus
    {
        Created = 0,// 已创建未支付

        //PayAndNotVerify = 101,// 已支付未校验
        //VerifyAndNotFinish = 102,// 已校验未结单
        //VerifyFailed = 103,// 校验失败（订单完成，失败）

        Cancel = 201,// 已取消（订单完成，失败）
        //RecvPayed = 202,// 已结单
        FinishAndNotGiveItem = 203,// 发送添加物品消息之前

        AckConfirm = 300,// 游戏服确认执行了发送物品

        Error = 500,// 大于 500 是异常后，异常后会直接关闭订单
    }

    public enum UCenterSubPubChannel
    {
        PayCharge
    }

    public enum UCenterSubPubEventType
    {
        ChargeGiveItem = 0
    }

    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GetUnFinishChargeListResponse
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public PayErrorCode ErrorCode { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public List<PayChargeInfo> UnFinishChargeList { get; set; }
    }

    // 充值订单信息
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class PayChargeInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public PayErrorCode ErrorCode { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string ChargeId { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public PayPlatform Platform { get; set; }// 商城平台 GooglePlay AppStore

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public PayChargeStatus Status { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public long Amount { get; set; }// 数量，单位：分

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string IAPProductId { get; set; }// 内购计费点
    }

    // 充值订单详情
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class PayChargeDetail
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public PayErrorCode ErrorCode { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string ChargeId { get; set; }// 订单唯一Id，由UCenter生成

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public PayChargeStatus Status { get; set; }// 订单状态，唯一可变参数

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string AppId { get; set; }// 为UCenterAppId，如King

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public int ItemTbId { get; set; }// 常规，首冲，打折，如何处理？

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string ItemName { get; set; }// 可选参数，增加可读性

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public PayPlatform Platform { get; set; }// 商城平台 GooglePlay AppStore

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public long Amount { get; set; }// 数量，单位：分

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public string Currency { get; set; }// 货币类型

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public string IAPProductId { get; set; }// 内购计费点

        [ProtoMember(12)]
#if !DEF_CLIENT
        [Id(11)]
#endif
        public string Receipt { get; set; }// 凭据

        [ProtoMember(13)]
#if !DEF_CLIENT
        [Id(12)]
#endif
        public string Transaction { get; set; }// 商店中的交易号

        [ProtoMember(14)]
#if !DEF_CLIENT
        [Id(13)]
#endif
        public string PurchaseToken { get; set; }// google支付token拿这个结单

        [ProtoMember(15)]
#if !DEF_CLIENT
        [Id(14)]
#endif
        public string ReqWebPayUrlString { get; set; }//请求web支付的字符串参数

        [ProtoMember(16)]
#if !DEF_CLIENT
        [Id(15)]
#endif
        public string Device { get; set; }//设备类型，默认为 pc 电脑浏览器 mobile 手机浏览器 qq 手机QQ内浏览器 wechat 微信内浏览器 alipay 支付宝客户端

        [ProtoMember(17)]
#if !DEF_CLIENT
        [Id(16)]
#endif
        public string PayType { get; set; }// alipay 支付宝 wxpay 微信支付 usdt USDT alipaybig 支付宝大额

        [ProtoMember(18)]
#if !DEF_CLIENT
        [Id(17)]
#endif
        public string ThirdPartyPayOrderId { get; set; }// 第三方支订单ID
    }

    // 充值订单创建请求
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class PayCreateChargeRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string IAPProductId { get; set; }// 内购计费点

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public PayPlatform Platform { get; set; }// 商城平台 GooglePlay AppStore

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public int ItemTbId { get; set; }// 道具id

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string ItemName { get; set; }// 道具名字

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Token { get; set; }// 为账号token

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string AppId { get; set; }

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string PlayerGuid { get; set; }

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public long Amount { get; set; }// 数量，单位：分

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public string Currency { get; set; }// 货币类型

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public string ChargeId { get; set; }

        [ProtoMember(12)]
#if !DEF_CLIENT
        [Id(11)]
#endif
        public string PayType { get; set; }//alipay wxpay
    }

    // 充值订单校验请求
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class PayVerifyChargeRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string ChargeId { get; set; }// 订单唯一Id，由UCenter生成

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string IAPProductId { get; set; }// 内购计费点

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string OrderId { get; set; }// 凭据

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string PackageName { get; set; }// ?

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string PurchaseToken { get; set; }// google支付token拿这个结单

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string Token { get; set; }// 为账号token

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string Receipt { get; set; }

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string Transaction { get; set; }
    }

    // 充值订单发货
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class PayChargeGiveItem
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string ChargeId { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string PlayerGuid { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public int ItemTbId { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public long Amount { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public bool IsSandbox { get; set; }
    }
}