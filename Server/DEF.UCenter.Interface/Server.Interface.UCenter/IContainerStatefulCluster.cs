#if !DEF_CLIENT

namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "Cluster", ContainerStateType.Stateful)]
public interface IContainerStatefulCluster : IContainerRpc
{
    Task Setup();

    Task Touch();
}

#endif