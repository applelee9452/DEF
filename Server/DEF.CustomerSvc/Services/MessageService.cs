using DEF.CustomerSvc.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEF.CustomerSvc.Services;

public class MessageService
{
    public IEnumerable<Message> FetchMessages()
    {
        return Enumerable.Range(0, 2).Select(x => new Message()
        {
            UserName = x % 2 == 0 ? "客服" : "James",
            Time = DateTime.Now.AddMinutes(x),
            Body = x % 2 == 0 ? "您好！请问有什么可以帮到你？" : "我想问一下...",
        });
    }
}