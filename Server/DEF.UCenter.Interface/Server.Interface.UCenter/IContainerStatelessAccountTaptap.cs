namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "AccountTaptap", ContainerStateType.Stateless)]
public interface IContainerStatelessAccountTaptap : IContainerRpc
{
    // 请求Taptap登陆
    Task<AccountLoginResponse> TaptapLoginRequest(AccountTaptapLoginRequest request, string client_ip);
}