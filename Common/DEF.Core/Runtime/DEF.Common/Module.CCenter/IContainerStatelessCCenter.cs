using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.CCenter
{
    [ContainerRpc("DEF.CCenter", "CCenter", ContainerStateType.Stateless)]
    public interface IContainerStatelessCCenter : IContainerRpc
    {
        Task<Dictionary<string, string>> GetCfg(string list_ns, string bundle_version);
    }
}