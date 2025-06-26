#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// 单实例，启动时一次性运行后不再使用
public class ContainerStatefulIMDaemon : ContainerStateful, IContainerStatefulIMDaemon
{
    IDisposable TimerHandleTouch { get; set; }
    IDisposable TimerHandleCluster { get; set; }
    DbClientRedis DbClientRedis { get; set; }
    bool Inited { get; set; } = false;

    public override Task OnCreate()
    {
        Logger.LogDebug($"ContainerStatefulDaemon.OnCreate()");

        TimerHandleTouch = RegisterTimer((_) => TimerTouch(),
            null, TimeSpan.FromMilliseconds(5000), TimeSpan.FromMilliseconds(5000));

        TimerHandleCluster = RegisterTimer((_) => TimerCluster(),
            null, TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(10000));

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        if (TimerHandleTouch != null)
        {
            TimerHandleTouch.Dispose();
            TimerHandleTouch = null;
        }

        if (TimerHandleCluster != null)
        {
            TimerHandleCluster.Dispose();
            TimerHandleCluster = null;
        }

        Logger.LogDebug($"ContainerStatefulDaemon.OnDestroy()");

        return Task.CompletedTask;
    }

    Task IContainerStatefulIMDaemon.Touch()
    {
        return Task.CompletedTask;
    }

    Task TimerTouch()
    {
        List<Task> list_task = [];

        // 系统邮箱
        {
            var container = GetContainerRpc<IContainerStatefulIMSystemMailBox>();
            var t = container.Touch();
            list_task.Add(t);
        }

        // 系统公告
        {
            var container = GetContainerRpc<IContainerStatefulIMSystemNotice>();
            var t = container.Touch();
            list_task.Add(t);
        }

        // 分区管理
        {
            var container = GetContainerRpc<IContainerStatefulIMRegionMgr>();
            var t = container.Touch();
            list_task.Add(t);
        }

        // CDKey管理
        {
            var container = GetContainerRpc<IContainerStatefulIMCDKeyMgr>();
            var t = container.Touch();
            list_task.Add(t);
        }

        return Task.WhenAll(list_task);
    }

    Task TimerCluster()
    {
        var container = GetContainerRpc<IContainerStatefulIMCluster>();
        return container.Touch();
    }
}

#endif