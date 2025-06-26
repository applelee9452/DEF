using DEF.EvCenter.Db;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEF.EvCenter;

public class ContainerStatefulInitDb : ContainerStateful, IContainerStatefulInitDb
{
    public override Task OnCreate()
    {
        Logger.LogDebug($"ContainerStatefulInitDb.OnCreate()");

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        Logger.LogDebug($"ContainerStatefulInitDb.OnDestroy()");

        return Task.CompletedTask;
    }

    async Task IContainerStatefulInitDb.Setup()
    {
        List<Task> list_task = [];
        Task t = null;
        DbClientMongo db = EvCenterContext.Instance.Mongo;

        // EvClientException
        {
            string c = StringDef.DBCollectionEvClientException;
            db.CreateCollection<Db.EvClientException>(c);
            var b = new IndexKeysDefinitionBuilder<Db.EvClientException>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.TimeStamp),
                b.Descending(x => x.TimeStamp),
                b.Ascending(x => x.AccId),
                b.Ascending(x => x.PlayerGuid));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvClientCrashReport
        {
            string c = typeof(EvClientCrashReport).Name;
            db.CreateCollection<Db.EvClientCrashReport>(c);
            var b = new IndexKeysDefinitionBuilder<Db.EvClientCrashReport>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.Dt),
                b.Descending(x => x.Dt),
                b.Ascending(x => x.PlayerGuid));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        await Task.WhenAll(list_task);
    }
}