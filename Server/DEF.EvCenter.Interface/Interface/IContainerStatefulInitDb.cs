namespace DEF.EvCenter;

[ContainerRpc("DEF.EvCenter", "InitDb", ContainerStateType.Stateful)]
public interface IContainerStatefulInitDb : IContainerRpc
{
    Task Setup();
}