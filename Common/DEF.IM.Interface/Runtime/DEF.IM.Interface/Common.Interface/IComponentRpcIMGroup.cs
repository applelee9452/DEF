using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpc("DEF.IM", ContainerStateType.Stateful)]
    public interface IComponentRpcIMGroup : IComponentRpc
    {
        // 请求修改群名
        Task ModifyGroupName(string group_name);

        // 群组人员权限，管理员

        // 设为管理员

        // 申请加群

        // 同意加群

        // 踢出成员

        // 退群

        // 请求发送群消息

        // 群消息广播通知
    }
}