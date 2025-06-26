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
    public partial class AccountResetPasswordByPhoneRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccountName { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string PhoneCode { get; set; }// 国际区号

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string PhoneNumber { get; set; }// 手机

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string Email { get; set; }// 邮箱

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string NewPassword { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string PhoneVerificationCode { get; set; }// 手机验证码
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class AccountResetPasswordResponse
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public UCenterErrorCode ErrorCode { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string AccountName { get; set; }// 如果是微信登录，则该字段保存的是OpenId

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public AccountType AccountType { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public AccountStatus AccountStatus { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string Name { get; set; }

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string ProfileImage { get; set; }

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string ProfileThumbnail { get; set; }

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public GenderType Gender { get; set; }

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public string Identity { get; set; }

        [ProtoMember(11)]
#if !DEF_CLIENT
        [Id(10)]
#endif
        public string PhoneCode { get; set; }// 国际区号

        [ProtoMember(12)]
#if !DEF_CLIENT
        [Id(11)]
#endif
        public string PhoneNumber { get; set; }

        [ProtoMember(13)]
#if !DEF_CLIENT
        [Id(12)]
#endif
        public string Email { get; set; }

        [ProtoMember(14)]
#if !DEF_CLIENT
        [Id(13)]
#endif
        public string Token { get; set; }

        [ProtoMember(15)]
#if !DEF_CLIENT
        [Id(14)]
#endif
        public DateTime LastLoginDateTime { get; set; }
    }
}