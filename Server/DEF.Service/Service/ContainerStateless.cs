using Microsoft.Extensions.Logging;

namespace DEF;

public abstract class ContainerStateless : ContainerBase
{
    public string ContainerType { get; private set; }
    protected GrainContainerStateless Grain { get; set; }

    public void Setup(ILogger logger, IService service, GrainContainerStateless grain, IGrainFactory grain_factory, string container_type)
    {
        ContainerType = container_type;
        Grain = grain;

        Setup2(logger, ContainerType, string.Empty, service, grain_factory);
    }

    public abstract Task OnCreate();

    public abstract Task OnDestroy();

    protected IDisposable RegisterTimer(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
    {
        return Grain.RegisterTimer2(asyncCallback, state, dueTime, period);
    }
}

public abstract class ContainerStatelessFactory
{
    public abstract string GetName();

    public abstract ContainerStateless Create(ILogger logger, IService service, GrainContainerStateless grain, IGrainFactory grain_factory, string container_type);
}

public class ContainerStatelessFactory<T> : ContainerStatelessFactory where T : ContainerStateless, new()
{
    public override string GetName()
    {
        return typeof(T).Name;
    }

    public override ContainerStateless Create(ILogger logger, IService service, GrainContainerStateless grain, IGrainFactory grain_factory, string container_type)
    {
        var container = new T();
        container.Setup(logger, service, grain, grain_factory, container_type);

        return container;
    }
}