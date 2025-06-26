#if !DEF_CLIENT

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// 单实例，启动时一次性运行后不再使用
public class ContainerStatefulIMCluster : ContainerStateful, IContainerStatefulIMCluster
{
    IDisposable TimerHandleTouch { get; set; }
    bool Inited { get; set; } = false;

    public override Task OnCreate()
    {
        Logger.LogDebug($"ContainerStatefulCluster.OnCreate()");

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        if (TimerHandleTouch != null)
        {
            TimerHandleTouch.Dispose();
            TimerHandleTouch = null;
        }

        Logger.LogDebug($"ContainerStatefulCluster.OnDestroy()");

        return Task.CompletedTask;
    }

    async Task IContainerStatefulIMCluster.Setup()
    {
        if (Inited) return;
        Inited = true;

        // 初始化Db
        {
            var c = GetContainerRpc<IContainerStatefulInitDb>();
            await c.Setup();
        }

        TimerHandleTouch = RegisterTimer((_) => TimerTouch(),
            null, TimeSpan.FromMilliseconds(5000), TimeSpan.FromMilliseconds(5000));
    }

    Task IContainerStatefulIMCluster.Touch()
    {
        return Task.CompletedTask;
    }

    Task TimerTouch()
    {
        var container_daemon = GetContainerRpc<IContainerStatefulIMDaemon>();
        return container_daemon.Touch();
    }
}

#endif