namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "ChannelEnjoy", ContainerStateType.Stateless)]
public interface IContainerStatelessChannelEnjoy : IContainerRpc
{
    Task<string> EnjoyWebhookRequest(Dictionary<string, string> map_data, string client_ip);
}