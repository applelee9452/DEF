#if !DEF_CLIENT

using MemoryPack;
using ProtoBuf;

namespace DEF.IM;

[ProtoContract]
[MemoryPackable]
#if !DEF_CLIENT
[GenerateSerializer]
#endif
public partial class DataNotice
{
    [ProtoMember(1)]
#if !DEF_CLIENT
    [Id(0)]
#endif
    public string _id { get; set; }

    [ProtoMember(2)]
#if !DEF_CLIENT
    [Id(1)]
#endif
    public string Title { get; set; }

    [ProtoMember(3)]
#if !DEF_CLIENT
    [Id(2)]
#endif
    public string Writer { get; set; }

    [ProtoMember(4)]
#if !DEF_CLIENT
    [Id(3)]
#endif
    public string Content { get; set; }

    [ProtoMember(5)]
#if !DEF_CLIENT
    [Id(4)]
#endif
    public DateTime Dt { get; set; }
}

#endif