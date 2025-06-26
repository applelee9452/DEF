using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpcObserver("IMPlayerGroup")]
    public interface IComponentObserverIMPlayerGroup : IComponentRpcObserver
    {
        // 测试
        Task Test(string s);

        // 收到群聊消息
        Task OnRecvGroupChatMsg(GroupChatMsg msg);
    }
}