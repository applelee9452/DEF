using DEF;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.CCenter;

[ContainerRpc("DEF.CCenter", "InitDb", ContainerStateType.Stateful)]
public interface IContainerInitDb : IContainerRpc
{
    Task Touch();
}