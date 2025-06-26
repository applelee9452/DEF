using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpcObserver("IMRegion")]
    public interface IComponentObserverIMRegion : IComponentRpcObserver
    {
        // 测试
        Task Test(string s);
    }
}