#if DEF_CLIENT

using System;
using System.Collections.Generic;

// 定时事件节点
public class EbTimeEvent
{
    public ulong mExpires;// 事件时间
    public Action OnTime;
}

public class EbTimer
{
    TimerShaft TimerShaft { get; set; }
    ulong TmSpan { get; set; }
    ulong LastTimeJeffies { get; set; }
    bool Closed { get; set; }
    Action<float> ActionCb { get; set; }

    public EbTimer(TimerShaft timer_shaft, ulong tm_span, Action<float> cb)
    {
        TimerShaft = timer_shaft;
        TmSpan = tm_span;
        ActionCb = cb;
        LastTimeJeffies = TimerShaft.TimeJeffies;// + TmSpan;
        Closed = false;

        var node = TimerShaft.GenNode();
        node.Obj.mExpires = TimerShaft.TimeJeffies;// + TmSpan;
        node.Obj.OnTime = _onTimer;

        TimerShaft._addTimerNode(node);
    }

    public void Close()
    {
        Closed = true;
        ActionCb = null;
    }

    void _onTimer()
    {
        if (Closed) return;

        var delta_tm = TimerShaft.TimeJeffies - LastTimeJeffies;
        LastTimeJeffies = TimerShaft.TimeJeffies;

        var node = TimerShaft.GenNode();
        node.Obj.mExpires = TimerShaft.TimeJeffies + TmSpan;
        node.Obj.OnTime = _onTimer;
        TimerShaft._addTimerNode(node);

        float tm = delta_tm / 1000.0f;
        ActionCb(tm);
    }
}

public class EbTimeWheel
{
    public EbDoubleLinkList<EbTimeEvent>[] ListTimerSpoke { get; private set; } = null;// Key列表下标
    int SpokeCount { get; set; }
    int LastSpokeIndex { get; set; }

    public EbTimeWheel(int spoke_count)
    {
        LastSpokeIndex = -1;
        SpokeCount = spoke_count;
        ListTimerSpoke = new EbDoubleLinkList<EbTimeEvent>[spoke_count];

        for (int i = 0; i < spoke_count; ++i)
        {
            EbDoubleLinkList<EbTimeEvent> list_event = new EbDoubleLinkList<EbTimeEvent>();
            ListTimerSpoke[i] = list_event;
        }
    }

    public void Destroy()
    {
        for (int i = 0; i < ListTimerSpoke.Length; i++)
        {
            ListTimerSpoke[i].Destroy();
        }
    }
}

public class TimerShaft : IDisposable
{
    static TimerShaft Instance { get; set; }
    public ulong TimeJeffies { get; private set; }
    Queue<EbDoubleLinkNode<EbTimeEvent>> Pool { get; set; } = new Queue<EbDoubleLinkNode<EbTimeEvent>>();
    EbDoubleLinkList<EbTimeEvent> ExcuteList { get; set; }
    EbTimeWheel Wheel1 { get; set; } = null;
    EbTimeWheel Wheel2 { get; set; } = null;
    EbTimeWheel Wheel3 { get; set; } = null;
    EbTimeWheel Wheel4 { get; set; } = null;
    EbTimeWheel Wheel5 { get; set; } = null;

    readonly ulong MIN_WHEEL = 6;
    readonly ulong MAX_WHEEL = 8;
    readonly ulong MIN_WHEEL_SIZE = (1 << 6);// 64
    readonly ulong MAX_WHEEL_SIZE = (1 << 8);// 256
    readonly ulong MIN_WHEEL_MASK = (64 - 1);
    readonly ulong MAX_WHEEL_MASK = (256 - 1);

    public TimerShaft()
    {
        Instance = this;
        ExcuteList = new EbDoubleLinkList<EbTimeEvent>();

        Wheel1 = new EbTimeWheel((int)MAX_WHEEL_SIZE);
        Wheel2 = new EbTimeWheel((int)MIN_WHEEL_SIZE);
        Wheel3 = new EbTimeWheel((int)MIN_WHEEL_SIZE);
        Wheel4 = new EbTimeWheel((int)MIN_WHEEL_SIZE);
        Wheel5 = new EbTimeWheel((int)MIN_WHEEL_SIZE);
    }

    void IDisposable.Dispose()
    {
        Wheel5.Destroy();
        Wheel4.Destroy();
        Wheel3.Destroy();
        Wheel2.Destroy();
        Wheel1.Destroy();

        UnityEngine.Debug.Log("TimerShaft.Dispose()");
    }

    // tm_span单位是毫秒
    public static EbTimer RegisterTimer(ulong tm_span, Action<float> cb)
    {
        return new EbTimer(Instance, tm_span, cb);
    }

    public EbDoubleLinkNode<EbTimeEvent> GenNode()
    {
        if (Pool.Count > 0)
        {
            return Pool.Dequeue();
        }
        else
        {
            var time_ev = new EbTimeEvent()
            {
                mExpires = 0,
                OnTime = null,
            };
            var node = new EbDoubleLinkNode<EbTimeEvent>()
            {
                Obj = time_ev
            };

            return node;
        }
    }

    // 根据时间，执行时间事件
    public void ProcessTimer(ulong jeffies)
    {
        while (jeffies >= TimeJeffies)
        {
            ulong index = TimeJeffies & MAX_WHEEL_MASK;

            if (index == 0 && _cascadeTimer(Wheel2, 0) == 0 && _cascadeTimer(Wheel3, 1) == 0 && _cascadeTimer(Wheel4, 2) == 0)
            {
                _cascadeTimer(Wheel5, 3);
            }

            TimeJeffies++;

            var list = Wheel1.ListTimerSpoke[(int)index];
            if (!list.Empty())
            {
                ExcuteList.AddTailList(list);
            }
        }

        EbDoubleLinkNode<EbTimeEvent> head, curr, next = null;
        head = ExcuteList.Head();
        curr = head.next;

        while (curr != head)
        {
            next = curr.next;

            EbDoubleLinkList<EbTimeEvent>.DelNode(curr);

            Pool.Enqueue(curr);

            var node = curr;
            curr = next;

            // 调用委托
            node.Obj.OnTime();
        }
    }

    // 添加事件
    public void _addTimerNode(EbDoubleLinkNode<EbTimeEvent> timer_node)
    {
        ulong expires = timer_node.Obj.mExpires;
        ulong idx = expires - TimeJeffies;

        EbDoubleLinkList<EbTimeEvent> list_timer_spoke = null;

        if (idx < MAX_WHEEL_SIZE)
        {
            // 第1个轮子
            ulong i = expires & MAX_WHEEL_MASK;
            list_timer_spoke = Wheel1.ListTimerSpoke[(int)i];
        }
        else if (idx < (ulong)1 << Convert.ToInt32(MAX_WHEEL + MIN_WHEEL))
        {
            // 第2个轮子                
            ulong i = expires >> Convert.ToInt32(MAX_WHEEL) & MIN_WHEEL_MASK;
            list_timer_spoke = Wheel2.ListTimerSpoke[(int)i];
        }
        else if (idx < (ulong)1 << Convert.ToInt32(MAX_WHEEL + 2 * MIN_WHEEL))
        {
            // 第3个轮子
            ulong i = expires >> Convert.ToInt32(MAX_WHEEL + MIN_WHEEL) & MIN_WHEEL_MASK;
            list_timer_spoke = Wheel3.ListTimerSpoke[(int)i];
        }
        else if (idx < (ulong)1 << Convert.ToInt32(MAX_WHEEL + 3 * MIN_WHEEL))
        {
            // 第4个轮子
            ulong i = expires >> Convert.ToInt32(MAX_WHEEL + 2 * MIN_WHEEL) & MIN_WHEEL_MASK;
            list_timer_spoke = Wheel4.ListTimerSpoke[(int)i];
        }
        else if ((long)idx < 0)
        {
            // Can happen if you add a timer with expires == jiffies,
            // or you set a timer to go off in the past
            list_timer_spoke = Wheel1.ListTimerSpoke[(int)(TimeJeffies & MAX_WHEEL_MASK)];
        }
        else
        {
            // If the timeout is larger than 0xffffffff on 64-bit
            // architectures then we use the maximum timeout:
            if (idx > 0xffffffffUL)
            {
                idx = 0xffffffffUL;
                expires = idx + TimeJeffies;
            }
            ulong i = expires >> Convert.ToInt32(MAX_WHEEL + 3 * MIN_WHEEL) & MIN_WHEEL_MASK;
            list_timer_spoke = Wheel5.ListTimerSpoke[(int)i];
        }

        // Timers are FIFO:
        list_timer_spoke.AddTailNode(timer_node);
    }

    // 调整时间轮上的事件
    int _cascadeTimer(EbTimeWheel timer_wheel, int wheel_index)
    {
        int index = (int)((TimeJeffies >> (Convert.ToInt32(MAX_WHEEL) + wheel_index * Convert.ToInt32(MIN_WHEEL))) & MIN_WHEEL_MASK);

        EbDoubleLinkNode<EbTimeEvent> head, curr, next;
        head = timer_wheel.ListTimerSpoke[index].Head();
        curr = head.next;

        while (curr != head)
        {
            next = curr.next;

            EbDoubleLinkList<EbTimeEvent>.DelNode(curr);

            _addTimerNode(curr);

            curr = next;
        }

        return index;
    }
}

#endif