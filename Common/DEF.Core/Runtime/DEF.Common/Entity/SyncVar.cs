using MemoryPack;
using ProtoBuf;
using System;

namespace DEF
{
    public enum SyncVarModifyOp
    {
        Add = 0,// 加
        Sub,// 减
        Mul,// 乘
        Div,// 除
    }

    [Serializable]
    [ProtoContract]
    [MemoryPackable]
    public partial class SyncVarModify
    {
        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public SyncVarModifyOp Op { get; set; }

        [ProtoMember(3)]
        public float Modify { get; set; }
    }

    [Serializable]
    [ProtoContract]
    [MemoryPackable]
    public partial class SyncVarModify2
    {
        [ProtoMember(1)]
        public string Key { get; set; }

        [ProtoMember(2)]
        public SyncVarModifyOp Op { get; set; }

        [ProtoMember(3)]
        public string Modify { get; set; }
    }

    public interface ISyncVar
    {
        void ApplyDirtyCustomState(byte cmd, byte[] value);
    }
}