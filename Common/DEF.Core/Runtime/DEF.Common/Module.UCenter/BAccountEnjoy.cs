#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;

namespace DEF.UCenter
{
    // Enjoy登陆请求
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class EnjoyLoginRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string ClientId { get; set; }// Client该字段赋值为string.Empty

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string ClientSecret { get; set; }// Client该字段赋值为string.Empty

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Token { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public DeviceInfo Device { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class EnjoyUserDetails
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string Unid { get; set; }// 用户唯一Id

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string LoginType { get; set; }// 登陆类型

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string LoginId { get; set; }// 第三方登陆Id

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string NickName { get; set; }// 用户昵称

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Email { get; set; }// 用户邮箱

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string CountryCode { get; set; }// 用户地区（国家码）

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public DateTime CreateTime { get; set; }// 用户创建时间

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public bool Paid { get; set; }// 是否是付费用户

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string DeviceId { get; set; }// 设备Id
    }

    // Enjoy登陆响应
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class EnjoyLoginResponse
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string Market { get; set; }// 市场标识

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string AppId { get; set; }// 应用Id

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public EnjoyUserDetails Details { get; set; }// 用户数据
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class EnjoyResponse
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string Code { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public EnjoyLoginResponse Data { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Message { get; set; }
    }
}