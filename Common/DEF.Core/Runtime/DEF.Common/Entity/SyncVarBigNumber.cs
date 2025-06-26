using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF
{
    [Serializable]
    [ProtoContract]
    public class SyncVarBigNumber : ISyncVar
    {
        [ProtoIgnore]
        public Action<string> OnValueChanged { get; set; }

        [ProtoIgnore]
        Scene Scene { get; set; }

        [ProtoIgnore]
        Component Component { get; set; }

        [ProtoIgnore]
        string StateName { get; set; }

        [ProtoMember(1)]
        public List<SyncVarModify2> Modifys { get; set; }

        [ProtoMember(2)]
        string BaseValue { get; set; }

        [ProtoMember(3)]
        string CurrentValue { get; set; }

        BigNumber CurrentValue2 { get; set; }

        public void ApplyDirtyCustomState(byte cmd, byte[] value)
        {
            if (cmd == 1)
            {
                // AddModify

                var modify = MemoryPackSerializer.Deserialize<SyncVarModify2>(value);

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

                string base_value = MemoryPackSerializer.Deserialize<string>(value);

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

        public void SetDefaultValue(string base_value)
        {
            BaseValue = string.IsNullOrEmpty(base_value) ? "0" : base_value;
            CurrentValue = BaseValue;
            CurrentValue2 = new BigNumber(CurrentValue);

            OnValueChanged?.Invoke(CurrentValue);

#if !DEF_CLIENT
            if (Component.Entity.NetworkSyncFlag)
            {
                byte[] param = MemoryPackSerializer.Serialize(BaseValue);

                Scene.WriteNetworkSyncBinlogCumstomState(Component.Entity.ClientSubFilter, Component.Entity.Id, Component.Name, StateName, 3, param);
            }
#endif
        }

        public string GetDefaultValue()
        {
            return BaseValue;
        }

        public string GetCurrentValue()
        {
            if (string.IsNullOrEmpty(CurrentValue))
            {
                CurrentValue = CurrentValue2.ToString();
            }

            return CurrentValue;
        }

        public void AddModify(string key, SyncVarModifyOp op, string v)
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
                var modify = new SyncVarModify2()
                {
                    Key = key,
                    Op = op,
                    Modify = v
                };

                Modifys ??= new();
                Modifys.Add(modify);
            }

            BigNumber current_value = string.IsNullOrEmpty(BaseValue) ? new BigNumber("0") : new BigNumber(BaseValue);

            foreach (var i in Modifys)
            {
                switch (i.Op)
                {
                    case SyncVarModifyOp.Add:
                        current_value += new BigNumber(i.Modify);
                        break;
                    case SyncVarModifyOp.Sub:
                        current_value -= new BigNumber(i.Modify);
                        break;
                    case SyncVarModifyOp.Mul:
                        current_value *= float.Parse(i.Modify);
                        break;
                    case SyncVarModifyOp.Div:
                        current_value /= float.Parse(i.Modify);
                        break;
                }
            }

            CurrentValue2 = current_value;
            CurrentValue = CurrentValue2.ToString();

            OnValueChanged?.Invoke(CurrentValue);

#if !DEF_CLIENT
            if (Component.Entity.NetworkSyncFlag)
            {
                var modify2 = new SyncVarModify2()
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

            BigNumber current_value = string.IsNullOrEmpty(BaseValue) ? new BigNumber("0") : new BigNumber(BaseValue);

            foreach (var i in Modifys)
            {
                switch (i.Op)
                {
                    case SyncVarModifyOp.Add:
                        current_value += new BigNumber(i.Modify);
                        break;
                    case SyncVarModifyOp.Sub:
                        current_value -= new BigNumber(i.Modify);
                        break;
                    case SyncVarModifyOp.Mul:
                        current_value *= float.Parse(i.Modify);
                        break;
                    case SyncVarModifyOp.Div:
                        current_value /= float.Parse(i.Modify);
                        break;
                }
            }

            CurrentValue2 = current_value;
            CurrentValue = CurrentValue2.ToString();

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