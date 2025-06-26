#if !DEF_CLIENT

using DEF;
using ProtoBuf;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "Marquee", ContainerStateType.Stateful)]
public interface IContainerStatefulIMMarquee : IContainerRpc
{
    // 保活
    Task Touch();

    // 跑马灯广播，直接广播发送
    Task BroadcastMarquee(BIMMarquee im_marquee);

    // 跑马灯广播，可定制发送次数，延迟发送时间，发送间隔（单位：秒）
    Task BroadcastMarqueeEx(BIMMarquee im_marquee, int broadcast_count, float broadcast_due, float broadcast_period);

    // 添加跑马灯预设，会保存到DB中
    Task AddMarqueePrefab(DataMarquee data_im_marque);

    // 删除跑马灯预设
    Task RemoveMarqueePrefab(string data_im_marque_guid);

    // 获取所有跑马灯预设
    Task<List<DataMarquee>> GetAllMarqueePrefab();
}

#endif