#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// 测试管理器
public class ContainerStatefulIMTestMgr : ContainerStateful, IContainerStatefulIMTestMgr
{
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    int Index { get; set; } = 1;
    int Count { get; set; }

    public override Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefulTestMgr.OnCreate()");

        StopwatchUpdate = new Stopwatch();
        StopwatchUpdate.Start();
        TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(),
            null, TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(3000));

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        if (TimerHandleUpdate != null)
        {
            TimerHandleUpdate.Dispose();
            TimerHandleUpdate = null;
        }

        if (StopwatchUpdate != null)
        {
            StopwatchUpdate.Stop();
            StopwatchUpdate = null;
        }

        Logger.LogDebug("ContainerStatefulTestMgr.OnDestroy()");

        return Task.CompletedTask;
    }

    Task IContainerStatefulIMTestMgr.Touch()
    {
        Logger.LogDebug("ContainerStatefulTestMgr.Touch()");

        return Task.CompletedTask; ;
    }

    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        Count++;
        if (Count > 4) return Task.CompletedTask;

        List<Task> tasks = [];
        for (int i = 0; i < 1000; i++)
        {
            var c = GetContainerRpc<IContainerStatefulIMTestPlayer>((Index++).ToString());
            var t = c.Touch();
            tasks.Add(t);
        }
        return Task.WhenAll(tasks);
    }
}

#endif