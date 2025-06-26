#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;

namespace DEF.UCenter
{
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GetPhoneVerificationCodeRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string PhoneCode { get; set; }// 国际区号

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string PhoneNumber { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GetPhoneNumberRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Token { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GetPhoneNumberResponse
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string PhoneCode { get; set; }// 国际区号

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string PhoneNumber { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class ModifyPhoneNumberRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Token { get; set; }
        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string PhoneCode { get; set; }// 国际区号

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string PhoneNumber { get; set; }
        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string PhoneVerificationCode { get; set; }
    }
}