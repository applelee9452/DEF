using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEF.CCenter;

public class ContainerStatefulInitDb : ContainerStateful, IContainerInitDb
{
    public override async Task OnCreate()
    {
        Logger.LogDebug($"ContainerStatefulInitDb.OnCreate()");

        // 初始化集合和索引
        await CreateDatabaseIndexesAsync();
    }

    public override Task OnDestroy()
    {
        Logger.LogDebug($"ContainerStatefulInitDb.OnDestroy()");

        return Task.CompletedTask;
    }

    Task IContainerInitDb.Touch()
    {
        return Task.CompletedTask;
    }

    async Task CreateDatabaseIndexesAsync()
    {
        List<Task> list_task = new();
        //Task t = null;
        //DbClientMongo db = CCenterContext.Instance.Mongo;

        // DataPlayer
        //{
        //    var c = typeof(DataPlayer).Name;
        //    db.CreateCollection<DataPlayer>(c);
        //    var b = new IndexKeysDefinitionBuilder<DataPlayer>();
        //    t = db.CreateIndexEx(c,
        //        b.Ascending(x => x.AccountId),
        //        b.Ascending(x => x.ActorId),
        //        b.Ascending(x => x.NickName),
        //        b.Ascending(x => x.GoldAcc),
        //        b.Ascending(x => x.GoldBank),
        //        b.Ascending(x => x.Diamond),
        //        b.Ascending(x => x.Level),
        //        b.Ascending(x => x.VIPLevel),
        //        b.Ascending(x => x.VIPDataTime),
        //        b.Ascending(x => x.LoginDateTime),
        //        b.Ascending(x => x.LogoutDateTime),
        //        b.Ascending(x => x.JoinDateTime),
        //        b.Ascending(x => x.Region));
        //    list_task.Add(t);
        //}

        // EvManagerChangePlayerForbidden
        //{
        //    var ev = typeof(EvManagerChangePlayerForbidden).Name;
        //    var b = new IndexKeysDefinitionBuilder<EvManagerChangePlayerForbidden>();
        //    db.CreateCollection<EvManagerChangePlayerForbidden>(ev);
        //    t = db.CreateIndexEx(ev,
        //        b.Ascending(x => x.EventTm),
        //        b.Ascending(x => x.PlayerGuid),
        //        b.Ascending(x => x.ManagerUserId),
        //        b.Ascending(x => x.SetIsForbidden),
        //        b.Ascending(x => x.IsForbiddenAfterChange));
        //    if (t != null)
        //    {
        //        list_task.Add(t);
        //    }
        //}

        await Task.WhenAll(list_task);
    }
}