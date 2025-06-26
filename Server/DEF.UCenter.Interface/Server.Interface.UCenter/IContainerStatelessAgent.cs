namespace DEF.UCenter;

[ContainerRpc("DEF.UCenter", "Agent", ContainerStateType.Stateless)]
public interface IContainerStatelessAgent : IContainerRpc
{
    // 请求新建代理账号
    Task<DataAgent> CreateAgent(string user_name, ulong parent_agent_id);

    // 请求删除代理账号
    Task RequestDeleteAgent(ulong agent_id);
}