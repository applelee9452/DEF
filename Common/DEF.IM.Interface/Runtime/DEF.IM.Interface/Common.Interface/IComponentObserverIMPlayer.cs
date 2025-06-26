using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpcObserver("IMPlayer")]
    public interface IComponentObserverIMPlayer : IComponentRpcObserver
    {
        // 测试
        Task Test(string s);

        // 收到跑马灯消息
        Task RecvMarquee(BIMMarquee marquee);
    }
}