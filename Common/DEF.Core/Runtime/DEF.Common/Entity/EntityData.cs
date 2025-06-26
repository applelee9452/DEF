#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System.Collections.Generic;

namespace DEF
{
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial struct EntityData
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public long Id { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string ContainerType { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string ContainerId { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string Name { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public Dictionary<int, byte[]> Tags { get; set; }// Tags

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public Dictionary<int, int> TagsRefCount { get; set; }// Tags的引用计数

        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public KeyValuePair<string, byte[]>[] EntityStates { get; set; }

        [ProtoMember(8)]
#if !DEF_CLIENT
        [Id(7)]
#endif
        public List<EntityData> Children { get; set; }
    }
}