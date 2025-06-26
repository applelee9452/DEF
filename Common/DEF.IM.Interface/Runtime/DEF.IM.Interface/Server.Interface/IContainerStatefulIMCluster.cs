#if !DEF_CLIENT

namespace DEF.IM;

[ContainerRpc("DEF.IM", "Cluster", ContainerStateType.Stateful)]
public interface IContainerStatefulIMCluster : IContainerRpc
{
    Task Setup();

    Task Touch();
}

#endif