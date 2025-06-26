namespace DEF.IM
{
    public enum IMErrorCode
    {
        NoError = 0,// 没有错误
        Error = 1,// 错误
        WrongRegion = 3,// 分区错误
        WrongPlayerGuid = 4,// 玩家guid错误

        // CDKey

        NoHasCDKey = 2,// 该CDKey不存在
        Exchange_Before = 5// 已经领取过
    }
}