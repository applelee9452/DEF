using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DEF
{
    public interface ISyncLerp
    {
        void Update(float tm);
    }

    [Serializable]
    [ProtoContract]
    public class SyncLerp<T> : ISyncLerp
    {
        [ProtoIgnore]
        public Action<T> OnLerp { get; set; }
        [ProtoMember(1)]
        public T Pos { get; private set; }// 插值平滑后的位置
        [ProtoMember(2)]
        public T PosLastest { get; private set; }
        [ProtoMember(3)]
        float SpeedNormal { get; set; } = 5f;
        [ProtoMember(4)]
        float SpeedFaster { get; set; } = 8f;

        float SpeedCurrent { get; set; }// 当前速度
        List<T> ListHistoryPos { get; set; } = new();// 历史位置

        public SyncLerp(T pos, T pos_lastest)
        {
            Pos = pos;
            PosLastest = pos_lastest;
        }

        public void SetPosLastest(T pos_lastest)
        {
            PosLastest = pos_lastest;

#if !DEF_CLIENT
            Pos = PosLastest;
#else
            ListHistoryPos.Add(pos_lastest);
#endif
        }

        public void SetSpeed(float speed_normal, float speed_faster)
        {
            SpeedNormal = speed_normal;
            SpeedFaster = speed_faster;
            SpeedCurrent = SpeedNormal;
        }

        public void Update(float tm)
        {
#if !DEF_CLIENT
#else
            HistoryLerping(tm);
#endif
        }

        // 平滑插值
        void HistoryLerping(float tm)
        {
            if (ListHistoryPos.Count > 0)
            {
                float close_enough = 0.11f;

                // 取出队列中的第一个设为插值的目标；位置足够接近，从队列中移除第一个，紧接着就是第二个
                if (typeof(T) == typeof(DEF.Math2.Vector2))
                {
                    Pos = (T)(object)Lerp((Math2.Vector2)(object)Pos, (Math2.Vector2)(object)ListHistoryPos[0], tm, SpeedCurrent);

                    if (Distance((Math2.Vector2)(object)Pos, (Math2.Vector2)(object)ListHistoryPos[0]) < close_enough)
                    {
                        ListHistoryPos.RemoveAt(0);
                    }
                }
                else if (typeof(T) == typeof(DEF.Math2.Vector3))
                {
                    Pos = (T)(object)Lerp((Math2.Vector3)(object)Pos, (Math2.Vector3)(object)ListHistoryPos[0], tm, SpeedCurrent);

                    if (Distance((Math2.Vector3)(object)Pos, (Math2.Vector3)(object)ListHistoryPos[0]) < close_enough)
                    {
                        ListHistoryPos.RemoveAt(0);
                    }
                }
                else if (typeof(T) == typeof(int))
                {
                    Pos = (T)(object)Lerp((int)(object)Pos, (int)(object)ListHistoryPos[0], tm, SpeedCurrent);

                    if (Distance((int)(object)Pos, (int)(object)ListHistoryPos[0]) < close_enough)
                    {
                        ListHistoryPos.RemoveAt(0);
                    }
                }
                else if (typeof(T) == typeof(float))
                {
                    Pos = (T)(object)Lerp((float)(object)Pos, (float)(object)ListHistoryPos[0], tm, SpeedCurrent);

                    if (Distance((float)(object)Pos, (float)(object)ListHistoryPos[0]) < close_enough)
                    {
                        ListHistoryPos.RemoveAt(0);
                    }
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

            OnLerp?.Invoke(Pos);
        }

        float Distance(int a, int b)
        {
            float distance = b - a;

            return distance;
        }

        int Lerp(int a, int b, float tm, float speed)
        {
            float distance = Distance(a, b);

            if (distance > 0.001f)
            {
                float need_tm = distance / speed;

                a += (int)(distance * tm / need_tm);
            }

            return a;
        }

        float Distance(float a, float b)
        {
            float distance = b - a;

            return distance;
        }

        float Lerp(float a, float b, float tm, float speed)
        {
            float distance = Distance(a, b);

            if (distance > 0.001f)
            {
                float need_tm = distance / speed;

                a += (int)(distance * tm / need_tm);
            }

            return a;
        }

        float Distance(Math2.Vector2 a, Math2.Vector2 b)
        {
            float distance = Math2.Vector2.Distance(b, a);

            return distance;
        }

        Math2.Vector2 Lerp(Math2.Vector2 a, Math2.Vector2 b, float tm, float speed)
        {
            float distance = Distance(a, b);

            if (distance > 0.001f)
            {
                float need_tm = distance / speed;

                a = DEF.Math2.Vector2.Lerp(a, b, tm / need_tm);
            }

            return a;
        }

        float Distance(DEF.Math2.Vector3 a, DEF.Math2.Vector3 b)
        {
            float distance = Math2.Vector2.Distance(b, a);

            return distance;
        }

        Math2.Vector3 Lerp(Math2.Vector3 a, Math2.Vector3 b, float tm, float speed)
        {
            float distance = Distance(a, b);

            if (distance > 0.001f)
            {
                float need_tm = distance / speed;

                a = DEF.Math2.Vector3.Lerp(a, b, tm / need_tm);
            }

            return a;
        }
    }
}