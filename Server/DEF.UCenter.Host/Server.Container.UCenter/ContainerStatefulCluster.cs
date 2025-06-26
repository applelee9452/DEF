#if !DEF_CLIENT

using Microsoft.Extensions.Logging;

namespace DEF.UCenter;

// 单实例，启动时一次性运行后不再使用
public class ContainerStatefulCluster : ContainerStateful, IContainerStatefulCluster
{
    IGrainTimer TimerHandleTouch { get; set; }
    bool Inited { get; set; } = false;

    public override Task OnCreate()
    {
        Logger.LogInformation($"ContainerStatefulCluster.OnCreate()");

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        if (TimerHandleTouch != null)
        {
            TimerHandleTouch.Dispose();
            TimerHandleTouch = null;
        }

        Logger.LogInformation($"ContainerStatefulCluster.OnDestroy()");

        return Task.CompletedTask;
    }

    async Task IContainerStatefulCluster.Setup()
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

    Task IContainerStatefulCluster.Touch()
    {
        return Task.CompletedTask;
    }

    Task TimerTouch()
    {
        var container_daemon = GetContainerRpc<IContainerStatefulDaemon>();
        return container_daemon.Touch();
    }
}

#endif