using MemoryPack;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF
{
    [Serializable]
    [ProtoContract]
#if DEF_CLIENT
    public class SyncLerpVector2 : ISyncLerp, ISyncVar
#else
    public class SyncLerpVector2
#endif
    {
        [ProtoIgnore]
        public Action<DEF.Math2.Vector2> OnPosChanged { get; set; }
        [ProtoIgnore]
        public Action<float, float> OnSpeedChanged { get; set; }
        [ProtoIgnore]
        Scene Scene { get; set; }
        [ProtoIgnore]
        Component Component { get; set; }
        [ProtoIgnore]
        string StateName { get; set; }
        [ProtoIgnore]
        float SpeedCurrent { get; set; }// 客户端当前速度
        [ProtoIgnore]
        List<DEF.Math2.Vector2> ListHistoryPos { get; set; } = new();// 历史位置
        [ProtoIgnore]
        public DEF.Math2.Vector2 Pos { get; private set; }// 插值平滑后的位置

        [ProtoMember(1)]
        public DEF.Math2.Vector2 PosLastest { get; private set; }
        [ProtoMember(2)]
        float SpeedNormal { get; set; } = 5f;
        [ProtoMember(3)]
        float SpeedFaster { get; set; } = 8f;

        public void Init(Component component, string state_name)
        {
            Component = component;
            Scene = Component.Entity.Scene;
            StateName = state_name;
            Pos = PosLastest;

#if DEF_CLIENT
            Component.Scene.AddLerp(this);
#endif
        }

        public void Release()
        {
#if DEF_CLIENT
            Component.Scene.RemoveLerp(this);
#endif
        }

        // 客户端同步自定义State
        public void ApplyDirtyCustomState(byte cmd, byte[] value)
        {
            if (cmd == 1)
            {
                // 位置同步，有插值

                var pos = MemoryPackSerializer.Deserialize<DEF.Math2.Vector2>(value);

                PosLastest = pos;
                ListHistoryPos.Add(pos);
            }
            else if (cmd == 2)
            {
                // 位置同步，无插值

                var pos = MemoryPackSerializer.Deserialize<DEF.Math2.Vector2>(value);

                ListHistoryPos.Clear();
                PosLastest = pos;
                Pos = PosLastest;

                OnPosChanged?.Invoke(Pos);
            }
            else if (cmd == 3)
            {
                // 速度同步

                var so = MemoryPackSerializer.Deserialize<SerializeObj<float, float>>(value);

                SpeedNormal = so.obj1;
                SpeedFaster = so.obj2;

                OnSpeedChanged?.Invoke(so.obj1, so.obj2);
            }
        }

#if !DEF_CLIENT
        public void SetPos(DEF.Math2.Vector2 pos, bool no_lerp = false)
        {
            PosLastest = pos;
            Pos = pos;

            if (Component.Entity.NetworkSyncFlag)
            {
                if (!no_lerp)
                {
                    // 有插值

                    byte[] param = MemoryPackSerializer.Serialize(pos);

                    Scene.WriteNetworkSyncBinlogCumstomState(Component.Entity.ClientSubFilter, Component.Entity.Id, Component.Name, StateName, 1, param);
                }
                else
                {
                    // 无插值，立即生效

                    byte[] param = MemoryPackSerializer.Serialize(pos);

                    Scene.WriteNetworkSyncBinlogCumstomState(Component.Entity.ClientSubFilter, Component.Entity.Id, Component.Name, StateName, 2, param);
                }
            }
        }

        public void SetSpeed(float speed_normal, float speed_faster)
        {
            SpeedNormal = speed_normal;
            SpeedFaster = speed_faster;

            if (Component.Entity.NetworkSyncFlag)
            {
                var so = new SerializeObj<float, float>()
                {
                    obj1 = SpeedNormal,
                    obj2 = SpeedFaster
                };

                // 速度同步

                byte[] param = MemoryPackSerializer.Serialize(so);

                Scene.WriteNetworkSyncBinlogCumstomState(Component.Entity.ClientSubFilter, Component.Entity.Id, Component.Name, StateName, 3, param);
            }
        }
#else
        public void Update(float tm)
        {
            // 平滑插值

            if (ListHistoryPos.Count > 0)
            {
                float close_enough = 0.11f;

                // 取出队列中的第一个设为插值的目标；位置足够接近，从队列中移除第一个，紧接着就是第二个

                Pos = Lerp(Pos, ListHistoryPos[0], tm, SpeedCurrent);

                if (Distance(Pos, ListHistoryPos[0]) < close_enough)
                {
                    ListHistoryPos.RemoveAt(0);
                }

                // 如果同步队列过大，加快插值速率，使其更快到达目标点
                if (ListHistoryPos.Count > 4)
                {
                    SpeedCurrent = SpeedFaster;
                }
                else
                {
                    SpeedCurrent = SpeedNormal;
                }
            }

            OnPosChanged?.Invoke(Pos);
        }

        float Distance(Math2.Vector2 a, Math2.Vector2 b)
        {
            float distance = Math2.Vector2.Distance(b, a);

            return distance;
        }

        Math2.Vector2 Lerp(Math2.Vector2 a, Math2.Vector2 b, float tm, float speed)
        {
            float distance = Distance(a, b);

            if (distance > 0.01f)
            {
                float need_tm = distance / speed;

                a = DEF.Math2.Vector2.Lerp(a, b, tm / need_tm);
            }

            return a;
        }
#endif
    }
}