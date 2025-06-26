using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF.IM
{
    [ComponentRpcObserver("IMPlayerMailBox")]
    public interface IComponentObserverIMPlayerMailBox : IComponentRpcObserver
    {
        // 收到邮件列表
        Task RecvMailList(List<Mail> list_mail);

        
    }
}