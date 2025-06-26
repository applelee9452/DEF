#if !DEF_CLIENT

namespace DEF.IM;

[ContainerRpc("DEF.IM", "SystemMailBox", ContainerStateType.Stateful)]
public interface IContainerStatefulIMSystemMailBox : IContainerRpc
{
    // 保活
    Task Touch();

    // 请求收取最新的系统邮件列表
    Task<List<SystemMail>> RequestGetLastestMailList(string region_id, string player_guid, DateTime last_date);

    // 添加一封新邮件
    Task AddMail(SystemMail mail);

    // 所有分区所有玩家广播发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task SendMailToAllRegionPlayers(Mail mail);

    // 向指定分区所有玩家广播发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task SendMailToOneRegionPlayers(Mail mail, int region_id);

    // 向指定分区列表的所有玩家广播发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task SendMailToRegionListPlayers(Mail mail, List<int> list_region_id);

    // 向指定玩家发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task SendMailToPlayer(Mail mail, string player_guid);

    // 向指定玩家列表发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task SendMailToPlayers(Mail mail, List<string> list_player_guid);
}

#endif