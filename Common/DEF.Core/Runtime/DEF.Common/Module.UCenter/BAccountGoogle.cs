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
    public partial class AccountGoogleOAuthInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccessToken { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string UserId { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string NickName { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public DeviceInfo Device { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Headimgurl { get; set; }
    }

    // Google登录验证信息
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GoogleLoginAuthInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AppId { get; set; }// app_id

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string AppName { get; set; }// 来自的应用名称

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string UserId { get; set; }// 用户Id

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string NickName { get; set; }// 用户昵称

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public bool IsValid { get; set; } = false;// 验证是否有效

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public DateTime AccessTokenExpires { get; set; }// AccessToken过期时间

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public DateTime UserTokenExpires { get; set; }// 用户的token过期时间

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public int ErrorCode { get; set; }

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string ErrorInfo { get; set; }// 错误信息

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public string Headimgurl { get; set; }

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public string Email { get; set; }
    }
}