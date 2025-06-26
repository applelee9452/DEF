#if !DEF_CLIENT

using MemoryPack;
using ProtoBuf;

namespace DEF.IM;

[ProtoContract]
[MemoryPackable]
#if !DEF_CLIENT
[GenerateSerializer]
#endif
public partial class DataRegion
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
    public int RegionId { get; set; }// 分区Id

    [ProtoMember(3)]
#if !DEF_CLIENT
    [Id(2)]
#endif
    public string RegionName { get; set; }// 分区名称

    [ProtoMember(4)]
#if !DEF_CLIENT
    [Id(3)]
#endif
    public int PlayerNum { get; set; }// 分区人数

    [ProtoMember(5)]
#if !DEF_CLIENT
    [Id(4)]
#endif
    public DateTime Dt { get; set; }// 分区创建的时间点

    [ProtoMember(6)]
#if !DEF_CLIENT
    [Id(5)]
#endif
    public bool IsActive { get; set; }// 是否激活

    [ProtoMember(7)]
#if !DEF_CLIENT
    [Id(6)]
#endif
    public int Merge2RegionId { get; set; }// 失活后合并入的新分区Id
}

[ProtoContract]
[MemoryPackable]
#if !DEF_CLIENT
[GenerateSerializer]
#endif
public partial class DataRegionDefault
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
    public int RegionId { get; set; }// 默认分区Id，用作自动分配给新玩家的默认分区
}

#endif