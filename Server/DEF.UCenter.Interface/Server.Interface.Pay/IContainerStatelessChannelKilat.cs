namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "ChannelKilat", ContainerStateType.Stateless)]
public interface IContainerStatelessChannelKilat : IContainerRpc
{
    Task<string> KilatPayWebhookRequest(Dictionary<string, string> map_data, string client_ip);

    // 获得WebUrl地址
    Task<string> GetWebUrl(string charge_id, long amount);
}