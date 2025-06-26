using StackExchange.Redis;
using System.Net;

namespace DEF;

// Test Redis
//{
//    await CasinosContext.Instance.Redis.DB.StringSetAsync("a", "aaaaaa");
//    string s = await CasinosContext.Instance.Redis.DB.StringGetAsync("a");
//    Console.WriteLine(s);

//    await CasinosContext.Instance.Redis.DB.KeyRenameAsync("a", "b");
//    s = await CasinosContext.Instance.Redis.DB.StringGetAsync("b");
//    Console.WriteLine(s);

//    await CasinosContext.Instance.Redis.DB.HashSetAsync("m1", "ha", "ssssssssssssssssssss");
//    await CasinosContext.Instance.Redis.DB.HashSetAsync("m1", "hb", "ggggggggggggggggg");
//    s = await CasinosContext.Instance.Redis.DB.HashGetAsync("m1", "ha");
//    Console.WriteLine(s);

//    await CasinosContext.Instance.Redis.DB.SetAddAsync("s1", "sa");
//    await CasinosContext.Instance.Redis.DB.SetAddAsync("s1", "sb");
//    await CasinosContext.Instance.Redis.DB.SetAddAsync("s1", "sc");
//    await CasinosContext.Instance.Redis.DB.SetAddAsync("s1", "sd");
//    await CasinosContext.Instance.Redis.DB.SetAddAsync("s1", "se");
//    s = await CasinosContext.Instance.Redis.DB.SetRandomMemberAsync("s1");
//    Console.WriteLine(s);
//}

public class DbClientRedis
{
    public IDatabase DB { get; private set; }
    public int DatabaseNumber { get; private set; }
    public ConnectionMultiplexer CM { get; private set; }

    public DbClientRedis(string database_name, string connection_string, string timezone)
    {
        DatabaseNumber = int.Parse(database_name);
        object asyncState = null;

        var c = ConfigurationOptions.Parse(connection_string);
        c.AllowAdmin = true;

        CM = ConnectionMultiplexer.Connect(c);
        DB = CM.GetDatabase(DatabaseNumber, asyncState);

        //var db = CM.GetDatabase();
        //if (string.IsNullOrEmpty(timezone))
        //{
        //    db.StringSet("timezone", "Asia/Shanghai");
        //}
        //else
        //{
        //    db.StringSet("timezone", timezone);
        //}
    }

    public void ClearDatabase()
    {
        EndPoint[] l = CM.GetEndPoints();
        foreach (var i in l)
        {
            IServer s = CM.GetServer(i);
            s.FlushDatabase(DatabaseNumber);
        }
    }
}