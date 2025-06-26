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
    public partial class IPAdderssData
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string ip = null;

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string long_ip = null;

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string isp = null;

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string area = null;

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string region_id = null;

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string region = null;

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string city_id = null;

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public string city = null;

        [ProtoMember(9)]
#if !DEF_CLIENT
        [Id(8)]
#endif
        public string country_id = null;

        [ProtoMember(10)]
#if !DEF_CLIENT
        [Id(9)]
#endif
        public string country = null;
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class IPCheckResult
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public int ret = 0;

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string msg;

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public IPAdderssData data = null;

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string log_id = null;
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class IPCheckRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string ip;
    }
}