using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF
{
    [Serializable]
    [ProtoContract]
    public class SyncVarFloat : ISyncVar
    {
        [ProtoIgnore]
        public Action<float> OnValueChanged { get; set; }

        [ProtoIgnore]
        Scene Scene { get; set; }

        [ProtoIgnore]
        Component Component { get; set; }

        [ProtoIgnore]
        string StateName { get; set; }

        [ProtoMember(1)]
        public List<SyncVarModify> Modifys { get; set; }

        [ProtoMember(2)]
        float BaseValue { get; set; }

        [ProtoMember(3)]
        float CurrentValue { get; set; }

        public void ApplyDirtyCustomState(byte cmd, byte[] value)
        {
            if (cmd == 1)
            {
                // AddModify

                var modify = MemoryPackSerializer.Deserialize<SyncVarModify>(value);

                AddModify(modify.Key, modify.Op, modify.Modify);
            }
            else if (cmd == 2)
            {
                // RemoveModiy

                string key = MemoryPackSerializer.Deserialize<string>(value);

                RemoveModify(key);
            }
            else if (cmd == 3)
            {
                // SetDefaultValue

                float base_value = MemoryPackSerializer.Deserialize<float>(value);

                BaseValue = base_value;
                CurrentValue = BaseValue;
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

        public void SetDefaultValue(float base_value)
        {
            BaseValue = base_value;
            CurrentValue = BaseValue;
            OnValueChanged?.Invoke(CurrentValue);

#if !DEF_CLIENT
            if (Component.Entity.NetworkSyncFlag)
            {
                byte[] param = MemoryPackSerializer.Serialize(BaseValue);

                Scene.WriteNetworkSyncBinlogCumstomState(Component.Entity.ClientSubFilter, Component.Entity.Id, Component.Name, StateName, 3, param);
            }
#endif
        }

        public float GetCurrentValue()
        {
            return CurrentValue;
        }

        public void AddModify(string key, SyncVarModifyOp op, float v)
        {
            bool exist = false;

            if (Modifys != null)
            {
                foreach (var i in Modifys)
                {
                    if (i.Key == key)
                    {
                        i.Op = op;
                        i.Modify = v;

                        exist = true;
                        break;
                    }
                }
            }

            if (!exist)
            {
                var modify = new SyncVarModify()
                {
                    Key = key,
                    Op = op,
                    Modify = v
                };

                Modifys ??= new();
                Modifys.Add(modify);
            }

            float current_value = BaseValue;
            foreach (var i in Modifys)
            {
                switch (i.Op)
                {
                    case SyncVarModifyOp.Add:
                        current_value += i.Modify;
                        break;
                    case SyncVarModifyOp.Sub:
                        current_value -= i.Modify;
                        break;
                    case SyncVarModifyOp.Mul:
                        current_value *= i.Modify;
                        break;
                    case SyncVarModifyOp.Div:
                        current_value /= i.Modify;
                        break;
                }
            }
            CurrentValue = current_value;

            OnValueChanged?.Invoke(CurrentValue);

#if !DEF_CLIENT
            if (Component.Entity.NetworkSyncFlag)
            {
                var modify2 = new SyncVarModify()
                {
                    Key = key,
                    Op = op,
                    Modify = v
                };

                byte[] param = MemoryPackSerializer.Serialize(modify2);

                Scene.WriteNetworkSyncBinlogCumstomState(Component.Entity.ClientSubFilter, Component.Entity.Id, Component.Name, StateName, 1, param);
            }
#endif
        }

        public void RemoveModify(string key)
        {
            if (Modifys == null) return;

            Modifys.RemoveAll(x => x.Key == key);

            float current_value = BaseValue;
            foreach (var i in Modifys)
            {
                switch (i.Op)
                {
                    case SyncVarModifyOp.Add:
                        current_value += i.Modify;
                        break;
                    case SyncVarModifyOp.Sub:
                        current_value -= i.Modify;
                        break;
                    case SyncVarModifyOp.Mul:
                        current_value *= i.Modify;
                        break;
                    case SyncVarModifyOp.Div:
                        current_value /= i.Modify;
                        break;
                }
            }
            CurrentValue = current_value;

            OnValueChanged?.Invoke(CurrentValue);

#if !DEF_CLIENT
            if (Component.Entity.NetworkSyncFlag)
            {
                byte[] param = MemoryPackSerializer.Serialize(key);

                Scene.WriteNetworkSyncBinlogCumstomState(Component.Entity.ClientSubFilter, Component.Entity.Id, Component.Name, StateName, 2, param);
            }
#endif
        }
    }
}