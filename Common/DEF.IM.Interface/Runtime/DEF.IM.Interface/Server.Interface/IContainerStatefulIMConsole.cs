#if !DEF_CLIENT

using DEF;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "Console", ContainerStateType.Stateful)]
public interface IContainerStatefulIMConsole : IContainerRpc
{
    Task Touch();

    Task<string> ExcuteCmd2(string s);

    Task<string> ExcuteCmd(string cmd, string[] args);
}

#endif