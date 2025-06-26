#if !DEF_CLIENT

namespace DEF.IM;

public class DataSingleChatMsg : DataBase
{
    public ulong MsgId { get; set; }
    public string SenderGuid { get; set; }
    public string RecverGuid { get; set; }
    public string Msg { get; set; }
    public DateTime Dt { get; set; }
}

#endif