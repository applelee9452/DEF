using System.Threading.Tasks;

namespace DEF.IM
{
    [ContainerRpc("DEF.IM", "IMClient", ContainerStateType.Stateless)]
    public interface IContainerStatelessIMClient : IContainerRpc
    {
        // 请求兑换CDKey
        Task<IMErrorCode> ExchangeCdkey(string player_guid, string code);
    }
}