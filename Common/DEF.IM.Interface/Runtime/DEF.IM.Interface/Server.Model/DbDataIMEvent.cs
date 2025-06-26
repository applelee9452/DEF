#if !DEF_CLIENT

using System;

namespace DEF.IM;

public class EvIMAddFriend : DataBase
{
    public string FromPlayerGuid;// 发起添加者，索引
    public string ToPlayerGuid;// 被添加者，索引
    public DateTime DtRequest;// 请求时间
    public DateTime DtResponse;// 响应时间
}

public class EvIMDeleteFriend : DataBase
{
    public string FromPlayerGuid;// 发起删除者，索引
    public string ToPlayerGuid;// 被删者，索引
    public DateTime DtDelete;// 删除时间
}

#endif