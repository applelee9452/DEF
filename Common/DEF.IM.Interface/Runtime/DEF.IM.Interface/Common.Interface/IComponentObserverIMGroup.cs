using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpcObserver("IMGroup")]
    public interface IComponentObserverIMGroup : IComponentRpcObserver
    {
        // 测试
        Task Test(string s);
    }
}