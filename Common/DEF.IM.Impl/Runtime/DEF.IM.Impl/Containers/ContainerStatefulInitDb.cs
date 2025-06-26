#if !DEF_CLIENT

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DEF.IM;

public class ContainerStatefulInitDb : ContainerStateful, IContainerStatefulInitDb
{
    DbClientMongo Db { get; set; }

    public override Task OnCreate()
    {
        Logger.LogInformation($"ContainerStatefulIMInitDb.OnCreate()");

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        Logger.LogInformation($"ContainerStatefulIMInitDb.OnDestroy()");

        return Task.CompletedTask;
    }

    async Task IContainerStatefulInitDb.Setup()
    {
        Db = IMContext.Instance.Mongo;

        // 检查并创建ConfigUniqId
        ConfigUniqId doc = await Db.ReadAsync<ConfigUniqId>(
            x => x._id == StringDef.DocConfigUniqId, StringDef.DbCollectionConfigUniqId);
        if (doc == null)
        {
            doc = new ConfigUniqId()
            {
                _id = StringDef.DocConfigUniqId,
                UniqPlayerId = 10000000,
            };

            await Db.InsertAsync(StringDef.DbCollectionConfigUniqId, doc);
        }

        List<Task> list_task = [];

        //DbEvPlayerRecvSystemMail
        {
            var c = typeof(DbEvPlayerRecvSystemMail).Name;
            Db.CreateCollection<DbEvPlayerRecvSystemMail>(c);
            var b = new IndexKeysDefinitionBuilder<DbEvPlayerRecvSystemMail>();
            var t = Db.CreateIndexEx(c,
                b.Descending(x => x.EventTm),
                b.Descending(x => x.PlayerGuid),
                b.Descending(x => x.RegionGuid),
                b.Descending(x => x.SystenMailGuid));
            list_task.Add(t);
        }

        // EvIMDeleteFriend
        {
            var c = typeof(EvIMDeleteFriend).Name;
            Db.CreateCollection<EvIMDeleteFriend>(c);
            var b = new IndexKeysDefinitionBuilder<EvIMDeleteFriend>();
            var t = Db.CreateIndexEx(c,
                b.Ascending(x => x.FromPlayerGuid),
                b.Ascending(x => x.ToPlayerGuid),
                b.Ascending(x => x.DtDelete));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvIMAddFriend
        {
            var c = typeof(EvIMAddFriend).Name;
            Db.CreateCollection<EvIMAddFriend>(c);
            var b = new IndexKeysDefinitionBuilder<EvIMAddFriend>();
            var t = Db.CreateIndexEx(c,
                b.Ascending(x => x.FromPlayerGuid),
                b.Ascending(x => x.ToPlayerGuid),
                //b.Ascending(x => x.Result),
                b.Ascending(x => x.DtRequest),
                b.Ascending(x => x.DtResponse));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvPlayerCreate
        {
            var ev = typeof(EvPlayerCreate).Name;
            var b = new IndexKeysDefinitionBuilder<EvPlayerCreate>();
            Db.CreateCollection<EvPlayerCreate>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Ascending(x => x.EventTm),
                b.Ascending(x => x.PlayerGuid),
                b.Ascending(x => x.ChannelId));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvPlayerDiamondChange
        {
            var ev = typeof(EvPlayerDiamondChange).Name;
            var b = new IndexKeysDefinitionBuilder<EvPlayerDiamondChange>();
            Db.CreateCollection<EvPlayerDiamondChange>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Ascending(x => x.PlayerGuid),
                b.Descending(x => x.BeforeChangeDiamond),
                b.Descending(x => x.EventTm),
                b.Descending(x => x.AfterChangeDiamond),
                b.Descending(x => x.ChangeDiamond),
                b.Ascending(x => x.ParamS),
                b.Ascending(x => x.Result),
                b.Descending(x => x.IsBot)
                );

            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvPlayerLoginLogout
        {
            var ev = typeof(EvPlayerLoginLogout).Name;
            var b = new IndexKeysDefinitionBuilder<EvPlayerLoginLogout>();
            Db.CreateCollection<EvPlayerLoginLogout>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Ascending(x => x.EventTm),
                b.Descending(x => x.EventTm),
                b.Ascending(x => x.PlayerGuid),
                b.Ascending(x => x.Action));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvPlayerPointChange
        {
            var ev = typeof(EvPlayerPointChange).Name;
            var b = new IndexKeysDefinitionBuilder<EvPlayerPointChange>();
            Db.CreateCollection<EvPlayerPointChange>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Ascending(x => x.PlayerGuid),
                b.Descending(x => x.BeforeChangePoint),
                b.Descending(x => x.EventTm),
                b.Descending(x => x.AfterChangePoint),
                b.Descending(x => x.ChangePoint),
                b.Ascending(x => x.ParamS),
                b.Ascending(x => x.Result),
                b.Descending(x => x.IsBot)
                );

            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvPlayerIpChange
        {
            var ev = typeof(EvPlayerIpChange).Name;
            var b = new IndexKeysDefinitionBuilder<EvPlayerIpChange>();
            Db.CreateCollection<EvPlayerIpChange>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Ascending(x => x.EventTm),
                b.Ascending(x => x.PlayerGuid));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvPlayerReportOther
        {
            var ev = typeof(EvPlayerReportOther).Name;
            var b = new IndexKeysDefinitionBuilder<EvPlayerReportOther>();
            Db.CreateCollection<EvPlayerReportOther>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Ascending(x => x.EventTm),
                b.Ascending(x => x.ReportPlayer));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvExchangeCDKey
        {
            var ev = typeof(EvExchangeCDKey).Name;
            var b = new IndexKeysDefinitionBuilder<EvExchangeCDKey>();
            Db.CreateCollection<EvExchangeCDKey>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Descending(x => x.EventTm),
                b.Descending(x => x.PlayerGuid));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvManagerChangePlayerGM
        {
            var ev = typeof(EvManagerChangePlayerGM).Name;
            var b = new IndexKeysDefinitionBuilder<EvManagerChangePlayerGM>();
            Db.CreateCollection<EvManagerChangePlayerGM>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Ascending(x => x.EventTm),
                b.Ascending(x => x.PlayerGuid),
                b.Ascending(x => x.ManagerUserId),
                b.Ascending(x => x.SetIsGM),
                b.Ascending(x => x.IsGMAfterChange));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvManagerChangePlayerForbidden
        {
            var ev = typeof(EvManagerChangePlayerForbidden).Name;
            var b = new IndexKeysDefinitionBuilder<EvManagerChangePlayerForbidden>();
            Db.CreateCollection<EvManagerChangePlayerForbidden>(ev);
            var t = Db.CreateIndexEx(ev,
                b.Ascending(x => x.EventTm),
                b.Ascending(x => x.PlayerGuid),
                b.Ascending(x => x.ManagerUserId),
                b.Ascending(x => x.SetIsForbidden),
                b.Ascending(x => x.IsForbiddenAfterChange));
            if (t != null)
            {
                list_task.Add(t);
            }
        }
    }

    Task IContainerStatefulInitDb.Touch()
    {
        return Task.CompletedTask;
    }
}

#endif