#if !DEF_CLIENT

namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "Daemon", ContainerStateType.Stateful)]
public interface IContainerStatefulDaemon : IContainerRpc
{
    Task Touch();
}

#endif