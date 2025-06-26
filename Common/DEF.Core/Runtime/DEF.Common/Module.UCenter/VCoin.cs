#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System.Collections.Generic;

namespace DEF.UCenter
{
    public enum MoneyType// 币种
    {
        CNY = 0,// 人民币
        USD,// 美元
        EUR,// 欧元
        NGN,// 尼日利亚，奈拉
        GHC,// 加纳，塞地
    }

    public enum WalletResult
    {
        Success = 0,
        False,
        Timeout,
        OverDailyLimit,// 超过日限额
        LackOfBalance,// 余额不足
        ManualVerifyLine// 需要人工处理
    }

    // VCoin的本人信息Item
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class VCoinMyInfoItem
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string Currency { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Address { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Memo { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public double FeeRate { get; set; }// 手续费比例

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public double FeeMin { get; set; }// 最低手续费

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public double ExchangeRateGold2VCoin { get; set; }// 汇率，Gold兑换为VCoin

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public double ExchangeRateVCoin2Gold { get; set; }// 汇率，VCoin兑换为Gold
    }

    // VCoin的本人信息
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class VCoinMyInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public List<VCoinMyInfoItem> ListItem { get; set; }
    }

    // VCoin的一条交易记录
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class VCoinTransRecrod
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string Id { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public bool ChargeOrWithdraw { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Currency { get; set; }// 币种

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public double Volume { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Dt { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string WithdrawStatus { get; set; }
    }

    // 提现请求
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class VCoinWithdrawRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string Channel { get; set; }// 提现渠道

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Currency { get; set; }// 币种

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Volume { get; set; }// 数量

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string Address { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Memo { get; set; }
    }

    // 获取虚拟币地址通知
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class VCoinGetOrGenerateAddressNotify
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public WalletResult Result { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Currency { get; set; }// 币种

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Address { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string Memo { get; set; }
    }

    // 请求验证提现地址有效性
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class VCoinWithdrawAddressValidRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string Channel { get; set; }// 渠道

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Currency { get; set; }// 币种

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Address { get; set; }// 虚拟币Address

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string Memo { get; set; }// 虚拟币Memo
    }

    // 验证提现地址有效性通知
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class VCoinWithdrawAddressValidNotify
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public WalletResult Result { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Channel { get; set; }// 渠道

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Address { get; set; }// 虚拟币Address

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string Memo { get; set; }// 虚拟币Memo

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public bool IsValid { get; set; }// 是否有效
    }
}