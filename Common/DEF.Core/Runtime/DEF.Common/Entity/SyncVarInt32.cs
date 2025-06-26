using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF
{
    [Serializable]
    [ProtoContract]
    public class SyncVarInt32 : ISyncVar
    {
        [ProtoIgnore]
        public Action<int> OnValueChanged { get; set; }

        [ProtoIgnore]
        Scene Scene { get; set; }

        [ProtoIgnore]
        Component Component { get; set; }

        [ProtoIgnore]
        string StateName { get; set; }

        [ProtoMember(1)]
        public List<SyncVarModify> Modifys { get; set; }

        [ProtoMember(2)]
        int BaseValue { get; set; }

        [ProtoMember(3)]
        int CurrentValue { get; set; }

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

                int base_value = MemoryPackSerializer.Deserialize<int>(value);

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

        public void SetDefaultValue(int base_value)
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

        public int GetDefaultValue()
        {
            return BaseValue;
        }

        public int GetCurrentValue()
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

            CurrentValue = BaseValue;

            foreach (var i in Modifys)
            {
                switch (i.Op)
                {
                    case SyncVarModifyOp.Add:
                        CurrentValue += (int)i.Modify;
                        break;
                    case SyncVarModifyOp.Sub:
                        CurrentValue -= (int)i.Modify;
                        break;
                    case SyncVarModifyOp.Mul:
                        CurrentValue = (int)(CurrentValue / i.Modify);
                        break;
                    case SyncVarModifyOp.Div:
                        CurrentValue = (int)(CurrentValue * i.Modify);
                        break;
                }
            }

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

            CurrentValue = BaseValue;

            foreach (var i in Modifys)
            {
                switch (i.Op)
                {
                    case SyncVarModifyOp.Add:
                        CurrentValue += (int)i.Modify;
                        break;
                    case SyncVarModifyOp.Sub:
                        CurrentValue -= (int)i.Modify;
                        break;
                    case SyncVarModifyOp.Mul:
                        CurrentValue = (int)(CurrentValue / i.Modify);
                        break;
                    case SyncVarModifyOp.Div:
                        CurrentValue = (int)(CurrentValue * i.Modify);
                        break;
                }
            }

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