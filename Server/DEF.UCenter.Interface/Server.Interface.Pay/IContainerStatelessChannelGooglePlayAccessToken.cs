namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "ChannelGooglePlayAccessToken", ContainerStateType.Stateless)]
public interface IContainerStatelessChannelGooglePlayAccessToken : IContainerRpc
{
    Task Setup();

    Task<string> GetAccessToken();
}