#if !DEF_CLIENT

namespace DEF.IM;

[ContainerRpc("DEF.IM", "SingleChatMsgCache", ContainerStateType.Stateful)]
public interface IContainerStatefulIMSingleChatMsgCache : IContainerRpc
{
    // 添加一条消息到聊天记录
    Task AddSingleChatMsg(SingleChatMsgSend msg);

    // 从聊天记录找查询与指定玩家的聊天记录，从msg_id向前
    Task<List<SingleChatMsgRecv>> GetSingleChatMsgRecord();
}

#endif