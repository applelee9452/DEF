using MemoryPack;
using ProtoBuf;
using System.Collections.Generic;

namespace DEF
{
    public enum BinlogOpType : byte
    {
        UpdateState = 0,
        DestroyEntity,
        AddEntity,
        ChangeEntityParent,
        AddEntityTag,
        SetEntityTagValue,
        RemoveEntityTag,
        CustomStateOp,// 自定义State相关增量协议
        CustomOp,// 更通用的自定义协议，不限于State
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogAddEntity
    {
        [ProtoMember(1)]
        public long ParentId;

        [ProtoMember(2)]
        public EntityData EntityData;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogDestroyEntity
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public string Reason;

        [ProtoMember(3)]
        public byte[] UserData;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogChangeEntityParent
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public long NewParentId;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogUpdateState
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public string Name;// 组件名

        [ProtoMember(3)]
        public string Key;

        [ProtoMember(4)]
        public byte[] Value;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogCustomState
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public string ComponentName;

        [ProtoMember(3)]
        public string StateKey;

        [ProtoMember(4)]
        public byte Cmd;

        [ProtoMember(5)]
        public byte[] Param;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogCustom
    {
        [ProtoMember(1)]
        public string Cmd;

        [ProtoMember(2)]
        public string Param1;

        [ProtoMember(3)]
        public byte[] Param2;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogAddEntityTag
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public int Tag;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogRemoveEntityTag
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public int Tag;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogSetEntityTagValue
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public int Tag;

        [ProtoMember(3)]
        public byte[] Value;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogAttributeAddModify
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public string Name;// 组件名

        [ProtoMember(3)]
        public string Key;

        [ProtoMember(4)]
        public SyncVarModifyOp Op;

        [ProtoMember(5)]
        public float Value;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogAttributeRemoveModify
    {
        [ProtoMember(1)]
        public long EntityId;

        [ProtoMember(2)]
        public string Name;// 组件名

        [ProtoMember(3)]
        public string Key;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial struct BinlogOp
    {
        [ProtoMember(1)]
        public BinlogOpType Op;

        [ProtoMember(2)]
        public ushort Index;
    }

    [ProtoContract]
    [MemoryPackable]
    public partial class NetworkSyncBinlog
    {
        [ProtoMember(1)]
        public List<BinlogOp> ListOp = new();

        [ProtoMember(2)]
        public List<BinlogAddEntity> ListAddEntity = new();

        [ProtoMember(3)]
        public List<BinlogDestroyEntity> ListRemoveEntity = new();

        [ProtoMember(4)]
        public List<BinlogUpdateState> ListUpdateState = new();

        [ProtoMember(5)]
        public List<BinlogChangeEntityParent> ListChangeEntityParent = new();

        [ProtoMember(6)]
        public List<BinlogAddEntityTag> ListAddEntityTag = new();

        [ProtoMember(7)]
        public List<BinlogRemoveEntityTag> ListRemoveEntityTag = new();

        [ProtoMember(8)]
        public List<BinlogSetEntityTagValue> ListSetEntityTagValue = new();

        [ProtoMember(9)]
        public List<BinlogAttributeAddModify> ListAttributeAddModify = new();

        [ProtoMember(10)]
        public List<BinlogAttributeRemoveModify> ListAttributeRemoveModify = new();

        [ProtoMember(11)]
        public List<BinlogCustomState> ListCustomState = new();

        [ProtoMember(12)]
        public List<BinlogCustom> ListCustom = new();

        public void Clear()
        {
            ListOp?.Clear();
            ListAddEntity?.Clear();
            ListRemoveEntity?.Clear();
            ListUpdateState?.Clear();
            ListChangeEntityParent?.Clear();
            ListAddEntityTag?.Clear();
            ListRemoveEntityTag?.Clear();
            ListSetEntityTagValue?.Clear();
            ListAttributeAddModify?.Clear();
            ListAttributeRemoveModify?.Clear();
            ListCustomState?.Clear();
            ListCustom?.Clear();
        }
    }
}