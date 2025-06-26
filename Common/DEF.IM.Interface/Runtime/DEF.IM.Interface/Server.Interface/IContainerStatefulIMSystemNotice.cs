#if !DEF_CLIENT

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM;

[ContainerRpc("DEF.IM", "SystemNotice", ContainerStateType.Stateful)]
public interface IContainerStatefulIMSystemNotice : IContainerRpc
{
    // 保活
    Task Touch();

    // 添加最新的系统公告
    Task AddNotice(Notice notice);

    // 请求拉取最新的系统公告列表
    Task<List<Notice>> RequestGetLastestNoticeList();
}

#endif