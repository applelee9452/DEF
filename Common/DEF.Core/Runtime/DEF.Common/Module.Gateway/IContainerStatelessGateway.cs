using System.Threading.Tasks;

namespace DEF.Gateway
{
    [ContainerRpc("DEF.Gateway", "Gateway", ContainerStateType.Stateless)]
    public interface IContainerStatelessGateway : IContainerRpc
    {
        // 心跳保持
        Task Heartbeat();

        // Ping
        Task<int> Ping();

        // 认证，通过UCenter
        Task<ClientAuthResponse> Auth(ClientAuthRequest request);

        // 免认证
        Task<ClientAuthResponse> AuthNo(ClientAuthNoRequest request);
    }
}