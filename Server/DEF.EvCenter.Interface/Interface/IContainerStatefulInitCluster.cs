namespace DEF.EvCenter;

[ContainerRpc("DEF.EvCenter", "InitCluster", ContainerStateType.Stateful)]
public interface IContainerStatefulInitCluster : IContainerRpc
{
    Task Touch();
}