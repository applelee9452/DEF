using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DEF.UCenter;

public class ContainerStatefulInitDb : ContainerStateful, IContainerStatefulInitDb
{
    public override async Task OnCreate()
    {
        Logger.LogInformation($"ContainerStatefulInitDb.OnCreate()");

        await CreateDatabaseIndexesAsync();
    }

    public override Task OnDestroy()
    {
        Logger.LogInformation($"ContainerStatefulInitDb.OnDestroy()");

        return Task.CompletedTask;
    }

    Task IContainerStatefulInitDb.Setup()
    {
        return Task.CompletedTask;
    }

    Task IContainerStatefulInitDb.Touch()
    {
        return Task.CompletedTask;
    }

    async Task CreateDatabaseIndexesAsync()
    {
        List<Task> list_task = [];
        Task t = null;
        DbClientMongo db = UCenterContext.Instance.Db;

        // CachePhoneVerificationCode
        {
            string c = StringDef.DbCollectionCachePhoneVerificationCode;
            db.CreateCollection<CachePhoneVerificationCode>(c);
            var b = new IndexKeysDefinitionBuilder<CachePhoneVerificationCode>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.PhoneCode),
                b.Ascending(x => x.PhoneNumber),
                b.Ascending(x => x.VerificationCode));
            if (t != null)
            {
                list_task.Add(t);
            }

            // TTL索引，100秒超时删除
            var keys = new IndexKeysDefinitionBuilder<CachePhoneVerificationCode>()
                .Ascending(x => x.UpdatedTime);
            CreateIndexOptions options1 = new CreateIndexOptions()
            {
                ExpireAfter = TimeSpan.FromSeconds(100)
            };
            var index_model = new CreateIndexModel<CachePhoneVerificationCode>(keys, options1);
            var c1 = db.Database.GetCollection<CachePhoneVerificationCode>(c);
            var t1 = c1.Indexes.CreateOneAsync(index_model);
            list_task.Add(t1);
        }

        // CacheWechatAccessToken
        {
            string c = StringDef.DbCollectionCacheWechatAccessToken;
            db.CreateCollection<CacheWechatAccessToken>(c);
            var b = new IndexKeysDefinitionBuilder<CacheWechatAccessToken>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.AppId),
                b.Ascending(x => x.OpenId),
                b.Ascending(x => x.AccessToken),
                b.Ascending(x => x.RefreshToken));
            if (t != null)
            {
                list_task.Add(t);
            }

            // TTL索引，29天超时删除
            var keys = new IndexKeysDefinitionBuilder<CacheWechatAccessToken>()
                .Ascending(x => x.UpdatedTime);
            CreateIndexOptions options1 = new CreateIndexOptions()
            {
                ExpireAfter = TimeSpan.FromDays(29)
            };
            var index_model = new CreateIndexModel<CacheWechatAccessToken>(keys, options1);
            var c1 = db.Database.GetCollection<CacheWechatAccessToken>(c);
            var t1 = c1.Indexes.CreateOneAsync(index_model);
            list_task.Add(t1);
        }

        // DataAccount
        {
            string c = StringDef.DbCollectionDataAccount;
            db.CreateCollection<DataAccount>(c);
            var b = new IndexKeysDefinitionBuilder<DataAccount>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.AccountName),
                b.Ascending(x => x.Password),
                b.Ascending(x => x.SuperPassword),
                b.Ascending(x => x.Gender),
                b.Ascending(x => x.Identity),
                b.Ascending(x => x.PhoneCode),
                b.Ascending(x => x.PhoneNumber),
                b.Ascending(x => x.Email),
                b.Ascending(x => x.AccountStatus),
                b.Ascending(x => x.AccountType));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataAccountAppData
        {
            string c = StringDef.DbCollectionDataAccountAppData;
            db.CreateCollection<DataAccountAppData>(c);
            var b = new IndexKeysDefinitionBuilder<DataAccountAppData>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.AppId),
                b.Ascending(x => x.AccountId));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataAccountWechat
        {
            string c = StringDef.DbCollectionDataAccountWechat;
            db.CreateCollection<DataAccountWechat>(c);
            var b = new IndexKeysDefinitionBuilder<DataAccountWechat>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.AccountId),
                b.Ascending(x => x.Unionid),
                b.Ascending(x => x.OpenId),
                b.Ascending(x => x.AppId));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataAccountFacebook
        {
            string c = StringDef.DbCollectionDataAccountFacebook;
            db.CreateCollection<DataAccountFacebook>(c);
            var b = new IndexKeysDefinitionBuilder<DataAccountFacebook>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.AccountId),
                b.Ascending(x => x.UserId),
                b.Ascending(x => x.AppId));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataAccountEnjoy
        {
            string c = StringDef.DbCollectionDataAccountEnjoy;
            db.CreateCollection<DataAccountEnjoy>(c);
            var b = new IndexKeysDefinitionBuilder<DataAccountEnjoy>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.AccountId),
                b.Ascending(x => x.Market),
                b.Ascending(x => x.Unid),
                b.Ascending(x => x.LoginType),
                b.Ascending(x => x.LoginId),
                b.Ascending(x => x.AppId2),
                b.Ascending(x => x.AppId));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataAgent
        {
            string c = StringDef.DbCollectionDataAgent;
            db.CreateCollection<DataAgent>(c);
            var b = new IndexKeysDefinitionBuilder<DataAgent>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.AgentId));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataDevice
        {
            string c = StringDef.DbCollectionDataDevice;
            db.CreateCollection<DataDevice>(c);
            var b = new IndexKeysDefinitionBuilder<DataDevice>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.Type));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataDeviceGuest
        {
            string c = StringDef.DbCollectionDataDeviceGuest;
            db.CreateCollection<DataDeviceGuest>(c);
            var b = new IndexKeysDefinitionBuilder<DataDeviceGuest>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.AccountId),
                b.Ascending(x => x.AppId));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataIdCard
        {
            string c = StringDef.DbCollectionDataIdCard;
            db.CreateCollection<DataIdCard>(c);
            var b = new IndexKeysDefinitionBuilder<DataIdCard>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.CardNo),
                b.Ascending(x => x.RealName),
                b.Ascending(x => x.Birth),
                b.Ascending(x => x.Sex));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvAccount
        {
            string c = StringDef.DBCollectionEvAccount;
            db.CreateCollection<EvAccount>(c);
            var b = new IndexKeysDefinitionBuilder<EvAccount>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime),
                b.Ascending(x => x.AccountName),
                b.Ascending(x => x.AccountId),
                b.Ascending(x => x.EventName),
                b.Ascending(x => x.DeviceId),
                b.Ascending(x => x.ClientIp));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvAccountError
        {
            string c = StringDef.DBCollectionEvAccountError;
            db.CreateCollection<EvAccountError>(c);
            var b = new IndexKeysDefinitionBuilder<EvAccountError>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // EvException
        {
            string c = StringDef.DBCollectionEvException;
            db.CreateCollection<EvException>(c);
            var b = new IndexKeysDefinitionBuilder<EvException>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.CreatedTime));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        await Task.WhenAll(list_task);

        // DataPayCharge
        {
            string c = StringDef.DbCollectionDataPayCharge;
            db.CreateCollection<DataPayCharge>(c);
            var b = new IndexKeysDefinitionBuilder<DataPayCharge>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.Id),
                b.Descending(x => x.CreatedTime),
                b.Descending(x => x.UpdatedTime),
                b.Ascending(x => x.Status),
                b.Ascending(x => x.AppId),
                b.Ascending(x => x.AccountId),
                b.Ascending(x => x.ItemTbId),
                b.Ascending(x => x.PlayerGuid),
                b.Ascending(x => x.Platform),
                b.Ascending(x => x.Amount),
                b.Ascending(x => x.Currency),
                b.Ascending(x => x.IsSandbox),
                b.Ascending(x => x.IAPProductId),
                b.Ascending(x => x.Transaction));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataPayExchange
        {
            string c = StringDef.DbCollectionDataPayExchange;
            db.CreateCollection<DataPayExchange>(c);
            var b = new IndexKeysDefinitionBuilder<DataPayExchange>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.ExchangeId),
                b.Ascending(x => x.Status),
                b.Ascending(x => x.AppId),
                b.Ascending(x => x.AccountId),
                b.Ascending(x => x.ItemTbId),
                b.Ascending(x => x.Platform),
                b.Ascending(x => x.Amount),
                b.Ascending(x => x.Currency),
                b.Ascending(x => x.IsSandbox),
                b.Ascending(x => x.IAPProductId),
                b.Ascending(x => x.Transaction));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        // DataAd
        {
            string c = StringDef.DbCollectionDataPayCharge;
            db.CreateCollection<DataAd>(c);
            var b = new IndexKeysDefinitionBuilder<DataAd>();
            t = db.CreateIndexEx(c,
                b.Ascending(x => x.Id),
                 b.Ascending(x => x.TransactionId),
                b.Ascending(x => x.AdStatus),
                b.Ascending(x => x.AdType),
                b.Ascending(x => x.AccountId),
                b.Ascending(x => x.Timestamp),
                b.Ascending(x => x.AdUnit),
                b.Ascending(x => x.RewardAmount),
                b.Ascending(x => x.RewardItem),
                b.Ascending(x => x.PlayerGuid));
            if (t != null)
            {
                list_task.Add(t);
            }
        }

        await Task.WhenAll(list_task);
    }
}