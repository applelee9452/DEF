#if !DEF_CLIENT

using Microsoft.Extensions.Logging;

namespace DEF.UCenter;

// 单实例，启动时一次性运行后不再使用
public class ContainerStatefulDaemon : ContainerStateful, IContainerStatefulDaemon
{
    IGrainTimer TimerHandleTouch { get; set; }
    IGrainTimer TimerHandleCluster { get; set; }
    DbClientRedis DbClientRedis { get; set; }
    bool Inited { get; set; } = false;

    public override Task OnCreate()
    {
        Logger.LogInformation($"ContainerStatefulDaemon.OnCreate()");

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

        Logger.LogInformation($"ContainerStatefulDaemon.OnDestroy()");

        return Task.CompletedTask;
    }

    Task IContainerStatefulDaemon.Touch()
    {
        return Task.CompletedTask;
    }

    Task TimerTouch()
    {
        List<Task> list_task = [];

        if (list_task.Count == 0)
        {
            return Task.CompletedTask;
        }

        return Task.WhenAll(list_task);
    }

    Task TimerCluster()
    {
        var container = GetContainerRpc<IContainerStatefulCluster>();
        return container.Touch();
    }
}

#endif