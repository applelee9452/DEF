namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "App", ContainerStateType.Stateless)]
public interface IContainerStatelessApp : IContainerRpc
{
    Task AppCreateRequest(AppInfo request);

    Task AppCreateConfigRequest(AppConfigInfo request);

    Task AppGetConfigRequest(string request);

    Task<Tuple<UCenterErrorCode, Gateway.GatewayAuthResponse>> AppAuth(string app_id, string acc_id, string token);

    Task<Tuple<UCenterErrorCode, AppAccountLoginResponse>> AppVerifyAccountLoginRequest(AppAccountLoginInfo request);

    Task AppReadAccountDataRequest(AppAccountDataInfo request);

    Task AppWriteAccountDataRequest(AppAccountDataInfo request);

    // App读取AccountData4Auth
    Task AppReadAccountData4Auth();

    // App写入AccountData4Auth
    Task AppWriteAccountData4Auth();

    Task<IPCheckResult> GetIPAddress(IPCheckRequest request);
}