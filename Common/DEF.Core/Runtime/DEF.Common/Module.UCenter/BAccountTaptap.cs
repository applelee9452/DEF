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
    public partial class AccountTaptapLoginRequest
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

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Token { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string MacKey { get; set; }
    }

    [MemoryPackable]
    public partial class TaptapProfileData
    {
        public string Avatar { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Openid { get; set; }
        public string Unionid { get; set; }
    }

    [MemoryPackable]
    public partial class TaptapProfileResponse
    {
        public TaptapProfileData Data { get; set; }
        public int Now { get; set; }
        public bool Success { get; set; }
    }
}