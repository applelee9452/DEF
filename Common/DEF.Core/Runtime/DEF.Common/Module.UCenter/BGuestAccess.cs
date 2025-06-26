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
    public partial class GuestAccessInfo
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AppId { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public DeviceInfo Device { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class GuestAccessResponse
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
        public string AccountName { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public AccountType AccountType { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string Token { get; set; }
    }
}