#if !DEF_CLIENT

using System.Collections.Generic;

namespace DEF.IM;

// _id==本人Guid，索引
public class DataIMFriend : DataBase
{
    public List<string> FriendList;
    public List<string> BlackList;
    public List<DataMail> ListMail;
}

#endif