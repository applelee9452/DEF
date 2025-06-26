#if !DEF_CLIENT

namespace DEF.IM;

public class ConfigUniqId : DataBase
{
    public ulong UniqPlayerId { get; set; }// 自增的唯一Id
}

#endif