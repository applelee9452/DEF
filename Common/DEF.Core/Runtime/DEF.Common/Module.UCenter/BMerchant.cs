#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;

namespace DEF.UCenter
{
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class MerchantAccount
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccountId { get; set; }// 账户唯一Id

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public AccountType AccountType { get; set; }// 账户类型：普通，游客

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public AccountStatus AccountStatus { get; set; }// 账户状态：正常，禁用

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string PhoneId { get; set; }// 手机号

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string NickName { get; set; }// 用户昵称

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string ParentAccountId { get; set; }// 父级账户唯一Id

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public DateTime JoinDt { get; set; }// 注册时间

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public DateTime LastLoginDateTime { get; set; }// 最新一次登录的时间

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string LastLoginClientIp { get; set; }// 最新一次登录的客户端Ip
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class MerchantAccountRecharge
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccountId { get; set; }// 账户唯一Id

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string NickName { get; set; }// 昵称

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public int Recharge { get; set; }// 充值总额，单位人民币分

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public DateTime RechargeDt { get; set; }// 充值日期
    }
}