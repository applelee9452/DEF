using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpc("DEF.IM", ContainerStateType.Stateful)]
    public interface IComponentRpcIMPlayerMailBox : IComponentRpc
    {
        // 请求发送邮件
        Task RequestSendMail(Mail mail);

        // 已读邮件
        Task<int> RequestReadMails(string mail_guid);

        // 请求领取附件奖励，-1是全部领取
        Task<List<DEF.IM.Mail>> RequestGetReward(string mail_guid);

        // 删除已读邮件
        Task<int> RequestDeleteMails();
    }
}