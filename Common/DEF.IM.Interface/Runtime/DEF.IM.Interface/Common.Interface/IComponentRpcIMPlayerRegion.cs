using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpc("DEF.IM", ContainerStateType.Stateful)]
    public interface IComponentRpcIMPlayerRegion : IComponentRpc
    {
        // 请求发送区域消息
        Task RequestSendRegionChatMsg(RegionChatMsg msg);
    }
}