namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "InitDb", ContainerStateType.Stateful)]
public interface IContainerStatefulInitDb : IContainerRpc
{
    Task Setup();

    Task Touch();
}