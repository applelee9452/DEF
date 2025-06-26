#if !DEF_CLIENT
using MemoryPack;
using Orleans;
using ProtoBuf;

[ProtoContract]
[MemoryPackable]
#if !DEF_CLIENT
[GenerateSerializer]
#endif
public partial class SubPubEvent
{
    [ProtoMember(1)]
#if !DEF_CLIENT
    [Id(0)]
#endif
    public int EvType;

    [ProtoMember(2)]
#if !DEF_CLIENT
    [Id(1)]
#endif
    public byte[] Data;
}
#endif