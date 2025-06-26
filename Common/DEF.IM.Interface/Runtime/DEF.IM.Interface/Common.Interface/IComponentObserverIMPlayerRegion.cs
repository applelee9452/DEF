using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpcObserver("IMPlayerRegion")]
    public interface IComponentObserverIMPlayerRegion : IComponentRpcObserver
    {
        // 测试
        Task Test(string s);

        // 收到分区消息
        Task OnRecvRegionChatMsg(RegionChatMsg msg);
    }
}