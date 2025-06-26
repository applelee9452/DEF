namespace DEF.EvCenter;

[ContainerRpc("DEF.EvCenter", "EvClientException", ContainerStateType.Stateless)]
public interface IContainerStatelessEvClientException : IContainerRpc
{
    Task ClientCrashReport(CrashReportInfo info, string client_ip);
}