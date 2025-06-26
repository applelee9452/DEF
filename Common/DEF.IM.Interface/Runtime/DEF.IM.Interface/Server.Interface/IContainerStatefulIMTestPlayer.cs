#if !DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "TestPlayer", ContainerStateType.Stateful)]
public interface IContainerStatefulIMTestPlayer : IContainerRpc
{
    // 保活
    Task Touch();
}

#endif