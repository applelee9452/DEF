#if !DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "TestMgr", ContainerStateType.Stateful)]
public interface IContainerStatefulIMTestMgr : IContainerRpc
{
    // 保活
    Task Touch();
}

#endif