#if !DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "IMGroup", ContainerStateType.Stateful)]
public interface IContainerStatefulIMGroup : IContainerRpc
{
    // 保活
    Task Touch();

    // 初始化，新建群时调用一次。群名，群主Guid
    Task<CreateGroupResult> Setup(string group_name, GroupMember admin);

    // 发送群组消息
    Task SendGroupChatMsg(GroupChatMsg msg);

    // 请求解散该群组
    Task<IMResult> RequestDisbandGroup(string player_guid);

    // 请求退出该群组
    Task<IMResult> RequestLeaveGroup(string player_guid, string new_admin_guid);
}

#endif