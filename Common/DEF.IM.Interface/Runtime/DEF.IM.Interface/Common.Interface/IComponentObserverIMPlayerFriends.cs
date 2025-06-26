using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpcObserver("IMPlayerFriends")]
    public interface IComponentObserverIMPlayerFriends : IComponentRpcObserver
    {
        // 收到被添加好友消息
        Task OnAddFriend(FriendItem friend_item);

        // 收到单聊消息
        Task RecvSingleChatMsg(SingleChatMsgRecv msg);
    }
}