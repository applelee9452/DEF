using Microsoft.Extensions.Logging;

namespace DEF;

public abstract class ContainerStatelessNoReentrant : ContainerBase
{
    public string ContainerType { get; private set; }
    protected GrainContainerStatelessNoReentrant Grain { get; set; }

    public void Setup(ILogger logger, IService service, GrainContainerStatelessNoReentrant grain, IGrainFactory grain_factory, string container_type)
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

public abstract class ContainerStatelessNoReentrantFactory
{
    public abstract string GetName();

    public abstract ContainerStatelessNoReentrant Create(ILogger logger, IService service, GrainContainerStatelessNoReentrant grain, IGrainFactory grain_factory, string container_type);
}

public class ContainerStatelessNoReentrantFactory<T> : ContainerStatelessNoReentrantFactory where T : ContainerStatelessNoReentrant, new()
{
    public override string GetName()
    {
        return typeof(T).Name;
    }

    public override ContainerStatelessNoReentrant Create(ILogger logger, IService service, GrainContainerStatelessNoReentrant grain, IGrainFactory grain_factory, string container_type)
    {
        var container = new T();
        container.Setup(logger, service, grain, grain_factory, container_type);

        return container;
    }
}