#if !DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "IMRegion", ContainerStateType.Stateful)]
public interface IContainerStatefulIMRegion : IContainerRpc
{
    // 保活
    Task Touch();

    // 发送区域消息
    Task SendRegionChatMsg(RegionChatMsg msg);

    //发送区域邮件
    Task SendRegionSystemMail(SystemMail msg);
}

#endif