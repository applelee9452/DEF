using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpc("DEF.IM", ContainerStateType.Stateful)]
    public interface IComponentRpcIMPlayerFriends : IComponentRpc
    {
        // 请求添加指定玩家Id为好友，这是第1步，C2S
        Task RequestAddFriend(string player_guid);

        // 响应对方的添加好友请求，deal_mode，忽略=0，同意=1，拒绝=2
        Task ResponseAddFriend(string player_guid, int deal_mode);

        // 批量响应对方的添加好友请求，deal_mode，忽略=0，同意=1，拒绝=2
        Task ResponseAddFriends(List<string> list_player_guid, int deal_mode);

        // 请求删除好友。把player_guid从我的好友列表中删除，并让对方我把从对方的好友列表中删除
        Task RequestDeleteFriend(string player_guid);

        // 请求发送私聊消息
        Task RequestSendSingleChatMsg(SingleChatMsgSend msg);

        // 请求获取最新的好友私聊消息
        Task<List<SingleChatMsgRecv>> RequestGetLastestChatMsg(string player_guid);
    }
}