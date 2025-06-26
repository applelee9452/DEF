using ProtoBuf;
using System.Collections.Generic;

namespace DEF
{
    public enum EntityDataType : byte
    {
        Entity = 0,
        Prefab,
        Scene,
    }

    [ProtoContract]
    public class EntityDataInfo
    {
        [ProtoMember(1)]
        public EntityDataType EntityDataType { get; set; }

        [ProtoMember(2)]
        public string SessionId { get; set; }

        [ProtoMember(3)]
        public long EntityId { get; set; }

        [ProtoMember(4)]
        public string Name { get; set; }
    }

    [ProtoContract]
    public class NetworkSyncEntityData
    {
        [ProtoMember(1)]
        public List<EntityDataInfo> ListEntityDataInfo = new();

        public void Clear()
        {
            ListEntityDataInfo?.Clear();
        }
    }
}