#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF.IM
{
    [ProtoContract]
    [MemoryPackable]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class Notice
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string Title { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Writer { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string Content { get; set; }// 公告内容

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public DateTime Dt { get; set; }
    }
}