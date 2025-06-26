using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpc("DEF.IM", ContainerStateType.Stateful)]
    public interface IComponentRpcIMPlayerGroup : IComponentRpc
    {
        // 请求解散该群组
        Task RequestDisbandGroup();

        // 请求退出该群组
        Task RequestLeaveGroup(string new_admin_guid);

        // 请求发送群消息
        Task RequestSendGroupChatMsg(GroupChatMsg msg);
    }
}