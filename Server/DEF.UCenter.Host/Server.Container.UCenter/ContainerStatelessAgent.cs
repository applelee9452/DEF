using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DEF.UCenter;

public class ContainerStatelessAgent : ContainerStateless, IContainerStatelessAgent
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    IOptions<UCenterOptions> UCenterOptions { get; set; }

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;
        UCenterOptions = UCenterContext.Instance.UCenterOptions;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    // 请求新建代理账号
    async Task<DataAgent> IContainerStatelessAgent.CreateAgent(string user_name, ulong parent_agent_id)
    {
        Logger.LogInformation("ContainerStatelessAgent.CreateAgent()");

        ulong agent_id = (ulong)UCenterContext.Instance.IdGen.NewLong();

        while (true)
        {
            var data_agent = await Db.ReadAsync<DataAgent>(a => a.AgentId == agent_id, StringDef.DbCollectionDataAgent);

            if (data_agent == null) break;

            agent_id = (ulong)UCenterContext.Instance.IdGen.NewLong();
        }

        DataAgent parent = null;
        if (parent_agent_id > 0)
        {
            parent = await Db.ReadAsync<DataAgent>(a => a.AgentId == parent_agent_id, StringDef.DbCollectionDataAgent);

            if (parent != null && parent.IsDelete)
            {
                parent = null;
            }
        }

        ulong[] arr = null;
        if (parent != null)
        {
            if (parent.AgentParents != null && parent.AgentParents.Length > 0)
            {
                arr = new ulong[1 + parent.AgentParents.Length];
                arr[0] = parent.AgentId;
                parent.AgentParents.CopyTo(arr, 1);
            }
            else
            {
                arr = new ulong[1];
                arr[0] = parent.AgentId;
            }
        }

        DataAgent data_agent1 = new()
        {
            Id = Guid.NewGuid().ToString(),
            CreatedTime = DateTime.UtcNow,
            UpdatedTime = DateTime.UtcNow,
            AccountStatus = AccountStatus.Active,
            UserName = user_name,
            AgentId = agent_id,
            AgentParents = arr
        };

        await Db.InsertAsync(StringDef.DbCollectionDataAgent, data_agent1);

        return data_agent1;
    }

    // 请求删除代理账号，只是标记为删除
    async Task IContainerStatelessAgent.RequestDeleteAgent(ulong agent_id)
    {
        Logger.LogInformation("ContainerStatelessAgent.RequestDeleteAgent()");

        if (agent_id == 0) return;

        var data_agent = await Db.ReadAsync<DataAgent>(a => a.AgentId == agent_id, StringDef.DbCollectionDataAgent);

        if (data_agent == null) return;

        data_agent.IsDelete = true;

        await Db.UpsertAsync(a => a.AgentId == agent_id, StringDef.DbCollectionDataAgent, data_agent);
    }
}