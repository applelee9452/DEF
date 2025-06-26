using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DEF.SyncDb;

public class TestEntity
{
    IServiceProvider ServiceProvider { get; set; }
    Scene Scene { get; set; }

    public TestEntity(IServiceProvider service_provider)
    {
        ServiceProvider = service_provider;
    }

    public async Task Setup()
    {
        var def_service = ServiceProvider.GetRequiredService<Service>();
        var assembly1 = typeof(IComponentStatePlayer).Assembly;
        def_service.Setup("Test", assembly1);

        Scene = Scene.New("Player", "Player", "aaa", def_service, null);

        EntityDef entity_def = new("Player", typeof(ComPlayer))
        {
            Children =
                [
                    new("PlayerInventory", typeof(ComPlayerInventory))
                    {
                    }
                ]
        };

        var et_player = Scene.CreateEntity(entity_def);

        var mc = TestContext.Instance.MongoClient;

        await mc.DropDatabaseAsync("Test");

        var mongo_db = mc.GetDatabase("Test");

        await et_player.SyncDelta2Db(mongo_db, "EntityPlayer");

        var collection_data_account = mongo_db.GetCollection<DataAccount>(typeof(DataAccount).Name);
        var collection_ev_loginlogout = mongo_db.GetCollection<EvAccountLoginLogout>(typeof(EvAccountLoginLogout).Name);

        //int account_count = 50;// 账号个数
        //int day_count = 30;// 从今天开始向前倒退天数

        //// 生成account_count个账号
        //List<DataAccount> list_account = [];
        //for (int i = 0; i < account_count; i++)
        //{
        //    DataAccount account = new()
        //    {
        //        Id = Guid.NewGuid().ToString(),
        //        AccountName = $"Test{i}"
        //    };

        //    list_account.Add(account);
        //}

        //// 向Db中插入account_count个账号
        //await collection_data_account.InsertManyAsync(list_account);

        //var dt_now = DateTime.Now;
        //dt_now = dt_now.AddDays(-day_count);
        //var dt_begin = new DateTime(dt_now.Year, dt_now.Month, dt_now.Day);

        //// day_count天，每个每个账号登陆0~3次
        //var rd = TestContext.Instance.Rd;
        //List<EvAccountLoginLogout> list_ev = [];
        //for (int i = 0; i < day_count; i++)
        //{
        //    var dt = dt_begin.AddDays(i);

        //    for (int j = 0; j < list_account.Count; j++)
        //    {
        //        var account = list_account[j];

        //        int login_count = rd.Next(0, 4);
        //        for (int k = 0; k < login_count; k++)
        //        {
        //            var seconds = rd.Next(0, 3600 * 24);

        //            EvAccountLoginLogout ev = new()
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                AccountId = account.Id,
        //                AccountName = account.AccountName,
        //                LoginOrLogout = true,
        //                Dt = dt.AddSeconds(seconds),
        //            };

        //            list_ev.Add(ev);
        //        }
        //    }
        //}

        //await collection_ev_loginlogout.InsertManyAsync(list_ev);
    }
}