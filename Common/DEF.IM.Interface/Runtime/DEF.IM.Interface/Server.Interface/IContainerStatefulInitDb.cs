#if !DEF_CLIENT

using DEF;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "InitDb", ContainerStateType.Stateful)]
public interface IContainerStatefulInitDb : IContainerRpc
{
    Task Setup();

    Task Touch();
}

#endif