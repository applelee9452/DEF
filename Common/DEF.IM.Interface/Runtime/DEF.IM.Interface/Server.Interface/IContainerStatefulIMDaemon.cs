#if !DEF_CLIENT

namespace DEF.IM;

[ContainerRpc("DEF.IM", "Daemon", ContainerStateType.Stateful)]
public interface IContainerStatefulIMDaemon : IContainerRpc
{
    Task Touch();
}

#endif