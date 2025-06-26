using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEF.CustomerSvc.Data;

public enum MessageType
{
    Text,
    Image
}

public class Message
{
    public string UserName { get; set; }
    public DateTime Time { get; set; }
    public MessageType Type { get; set; }
    public string Body { get; set; }
}