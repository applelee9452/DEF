using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DEF
{
    [Serializable]
    [ProtoContract]
    public class SyncDictionary<TKey, TValue>
    {
        [ProtoIgnore]
        Scene Scene { get; set; }

        [ProtoIgnore]
        Component Component { get; set; }

        [ProtoIgnore]
        string StateName { get; set; }

        [ProtoMember(1)]
        Dictionary<TKey, TValue> MapItem { get; set; }

        public void ApplyDirtyCustomState(byte cmd, byte[] value)
        {
            if (cmd == 1)
            {
                // AddModify

                //var modify = MemoryPackSerializer.Deserialize<SyncVarModify>(value);

                //AddModify(modify.Key, modify.Op, modify.Modify);
            }
            else if (cmd == 2)
            {
                // RemoveModiy

                //string key = MemoryPackSerializer.Deserialize<string>(value);

                //RemoveModify(key);
            }
            else if (cmd == 3)
            {
                // SetDefaultValue

                //float base_value = MemoryPackSerializer.Deserialize<float>(value);

                //BaseValue = base_value;
                //CurrentValue = BaseValue;
            }
        }

        public void Init(Component component, string state_name)
        {
            Component = component;
            Scene = Component.Entity.Scene;
            StateName = state_name;
        }

        public void Release()
        {
        }
    }
}