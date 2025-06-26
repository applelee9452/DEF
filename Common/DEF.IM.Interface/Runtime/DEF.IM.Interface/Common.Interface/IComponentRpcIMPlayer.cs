using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpc("DEF.IM", ContainerStateType.Stateful)]
    public interface IComponentRpcIMPlayer : IComponentRpc
    {
        // GM命令
        Task<string> GMCommand(string command, string param1, string param2, string param3);

        // 请求修改昵称
        Task RequestChangeNickname(string nickname);

        // 请求创建群组
        Task<CreateGroupResult> RequestCreateGroup(string group_name);

        // 请求加入群组
        Task RequestJoinGroup(string group_guid);
    }
}