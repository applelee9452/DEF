#if !DEF_CLIENT

using System.Threading.Tasks;

namespace DEF.IM;

// IM客户端，无状态
public class ContainerStatelessIMClient : ContainerStateless, IContainerStatelessIMClient
{
    public override Task OnCreate()
    {
        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    // 请求兑换CDKey
    async Task<IMErrorCode> IContainerStatelessIMClient.ExchangeCdkey(string player_guid, string code)
    {
        var container_cdkeymgr = GetContainerRpc<IContainerStatefulIMCDKeyMgr>();
        IMErrorCode exchange_cdkey_result = await container_cdkeymgr.ExchangeCdkey(player_guid, code);

        return exchange_cdkey_result;
    }
}

#endif