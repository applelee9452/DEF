using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace DEF;

public abstract class ContainerStatefulNoReentrant : ContainerBase
{
    public string ContainerType { get; private set; }
    public string ContainerId { get; private set; }
    protected GrainContainerStatefulNoReentrant Grain { get; set; }

    public void Setup(ILogger logger, IService service, GrainContainerStatefulNoReentrant grain, IGrainFactory grain_factory, string container_type, string container_id)
    {
        ContainerType = container_type;
        ContainerId = container_id;
        Grain = grain;

        Setup2(logger, ContainerType, ContainerId, service, grain_factory);
    }

    public abstract Task OnCreate();

    public abstract Task OnDestroy();

    protected IGrainTimer RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
    {
        GrainTimerCreationOptions options = new()
        {
            DueTime = dueTime,
            Period = period,
            Interleave = false,
            KeepAlive = true
        };

        return Grain.RegisterTimer2(asyncCallback, state, options);
    }

    protected IGrainTimer RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period, bool interleave, bool keep_alive)
    {
        GrainTimerCreationOptions options = new()
        {
            DueTime = dueTime,
            Period = period,
            Interleave = interleave,
            KeepAlive = keep_alive
        };

        return Grain.RegisterTimer2(asyncCallback, state, options);
    }

    public IStreamProvider GetStreamProvider2()
    {
        return Grain.GetStreamProvider2();
    }

    public ContainerStatefulStream<T> CreateStream<T>(string name_space, string steam_guid)
    {
        var stream = new ContainerStatefulStream<T>(this, name_space, steam_guid);

        return stream;
    }

    public async Task<ContainerStatefulStreamSub<T>> CreateStreamSubAsync<T>(string name_space, string steam_guid, Func<T, StreamSequenceToken, Task> on_next_async)
    {
        var stream_sub = new ContainerStatefulStreamSub<T>();

        await stream_sub.SubAsync(this, name_space, steam_guid, on_next_async);

        return stream_sub;
    }

    public void DeactivateOnIdle1()
    {
        Grain.DeactivateOnIdle1();
    }

    public void DeactivateOnIdle2(TimeSpan ts)
    {
        Grain.DeactivateOnIdle2(ts);
    }
}

public abstract class ContainerStatefulNoReentrantFactory
{
    public abstract ContainerStatefulNoReentrant Create(ILogger logger, IService service, GrainContainerStatefulNoReentrant grain,
        IGrainFactory grain_factory, string container_type, string container_id);
}

public class ContainerStatefulNoReentrantFactory<T> : ContainerStatefulNoReentrantFactory where T : ContainerStatefulNoReentrant, new()
{
    public override ContainerStatefulNoReentrant Create(ILogger logger, IService service, GrainContainerStatefulNoReentrant grain,
        IGrainFactory grain_factory, string container_type, string container_id)
    {
        var container = new T();
        container.Setup(logger, service, grain, grain_factory, container_type, container_id);

        return container;
    }
}