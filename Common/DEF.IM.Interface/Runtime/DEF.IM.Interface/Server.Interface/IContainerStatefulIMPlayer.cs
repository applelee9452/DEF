#if !DEF_CLIENT

namespace DEF.IM;

[ContainerRpc("DEF.IM", "IMPlayer", ContainerStateType.Stateful)]
public interface IContainerStatefulIMPlayer : IContainerRpc
{
    // 客户端连接并验证成功
    Task ClientAttached(Gateway.GatewayAuthedInfo info, string extra_data);

    // 客户端断开链接
    Task ClientDeattached(string session_guid);

    // 添加好友，先放到申请好友列表中。第2步，S2S
    Task Add2AddFriendList(AddFriendItem add_friend);

    // 添加好友，添加好友列表中。第3步，S2S
    Task Add2FriendList(FriendItem friend_item);

    // 删除好友，从好友列表，申请列表中移除
    Task DeleteFriend(string player_guid);

    // 收到单聊消息，推送给Client
    Task RecvSingleChatMsg(SingleChatMsgRecv msg);

    // 请求获取玩家信息
    Task<PlayerInitInfo> GetPlayerInitInfo();

    // 请求获取玩家信息
    Task<PlayerInfo> GetPlayerInfo();

    // 获取分区信息
    Task<Region> GetRegionInfo();

    // 玩家收取一封邮件
    Task AddMail(Mail mail);
}

#endif