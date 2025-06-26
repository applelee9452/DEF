using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Text;

namespace DEF.UCenter;

public class ContainerStatefullExchange : ContainerStateful, IContainerStatefulExchange
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    Random Rd { get; set; } = new Random();
    StringBuilder Sb { get; set; } = new StringBuilder(256);
    DbClientMongo DbPay { get; set; }
    ConfigPay ConfigPay { get; set; }
    string AccountId { get; set; }
    List<DataPayExchange> ListDbExchange { get; set; }// 未完成订单列表

    public override async Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        AccountId = ContainerId;//this.GetPrimaryKey().ToString();
        DbPay = UCenterContext.Instance.Db;

        // 激活时查询与本账号关联的所有未完成订单
        ListDbExchange = await DbPay.ReadListAsync<DataPayExchange>(
            a => a.AccountId == AccountId && a.Status != PayExchangeStatus.End,
            StringDef.DbCollectionDataPayExchange);

        if (ListDbExchange == null)
        {
            ListDbExchange = [];
        }

        // todo，待整理
        //var grain_config_key = GrainConfigPartition.GetPartitionKey(Rd, Sb);
        //var grain_config = GrainFactory.GetGrain<IGrainConfigPartition>(grain_config_key);
        //ConfigPay = await grain_config.GetConfigPay();

        //TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(), null,
        //    TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
    }

    public override Task OnDestroy()
    {
        //if (TimerHandleUpdate != null)
        //{
        //    TimerHandleUpdate.Dispose();
        //    TimerHandleUpdate = null;
        //}

        return Task.CompletedTask;
    }

    // 获取未结单订单列表
    Task<string> IContainerStatefulExchange.PayExchangeGetUnFinishList(string client_ip)
    {
        return Task.FromResult(string.Empty);
    }

    // 获取订单详情
    Task<string> IContainerStatefulExchange.PayExchangeGetDetail(string exchange_id, string client_ip)
    {
        return Task.FromResult(string.Empty);
    }

    // 创建订单
    async Task<string> IContainerStatefulExchange.PayExchangeCreate(PayExchangeCreateRequest request, string client_ip)
    {
        Logger.LogInformation("GrainExchange.PayExchangeCreate() ClientIp={0} Platform={1}", client_ip, request.Platform);

        var response = new SerializeObj<PayErrorCode, PayExchangeDetail>()
        {
            obj1 = PayErrorCode.Error,
            obj2 = null
        };

        string exchange_id = Guid.NewGuid().ToString();
        var exchange_db = new DataPayExchange()
        {
            Id = exchange_id,
            CreatedTime = DateTime.UtcNow,
            ExchangeId = exchange_id,
            Platform = request.Platform,
            ItemTbId = request.ItemTbId,
            ItemName = request.ItemName,
            AccountId = request.AccountId,
            PlayerGuid = request.PlayerGuid,
            AppId = request.AppId,
            Amount = request.Amount,
            Currency = request.Currency,
            PurchaseToken = string.Empty,
            IsSandbox = false
        };

        await DbPay.InsertAsync(StringDef.DbCollectionDataPayExchange, exchange_db);

        //var exchange_info = new PayExchangeInfo()
        //{
        //    ExchangeId = exchange_db.ExchangeId,
        //    Platform = exchange_db.Platform,
        //    Status = exchange_db.Status,
        //    Amount = exchange_db.Amount,
        //};

        var exchange_client = new PayExchangeDetail()
        {
            ExchangeId = exchange_db.ExchangeId,
            Status = exchange_db.Status,
            AppId = exchange_db.AppId,
            AccountId = exchange_db.AccountId,
            ItemTbId = exchange_db.ItemTbId,
            ItemName = exchange_db.ItemName,
            Platform = exchange_db.Platform,
            Amount = exchange_db.Amount,
            Currency = exchange_db.Currency,
            IAPProductId = exchange_db.IAPProductId,
            Receipt = string.Empty,
            Transaction = exchange_db.Transaction,
            PurchaseToken = exchange_db.PurchaseToken
        };

        ListDbExchange.Add(exchange_db);

        response.obj1 = PayErrorCode.NoError;

        response.obj2 = exchange_client;
        var response_str = Newtonsoft.Json.JsonConvert.SerializeObject(response);

        Logger.LogInformation(response_str);

        return response_str;
    }

    //// 取消订单
    //Task<string> IContainerExchange.PayExchangeCancel(string exchange_id, string client_ip);

    //// 验证订单
    //Task<string> IContainerExchange.PayExchangeVerify(PayVerifyChargeRequest request, string client_ip);

    //// 结束订单
    //Task<string> IContainerExchange.PayExchangeFinish(string exchange_id, string client_ip);

    // 第三方服务器通知完成订单
    Task<string> IContainerStatefulExchange.PayExchangeFinishByServer(DataPayExchange exchange, string client_ip)
    {
        return Task.FromResult(string.Empty);
    }

    async Task TimerUpdate()
    {
        //if (string.IsNullOrEmpty(ConfigPay.GiveItemWebhook))
        //{
        //    return;
        //}

        DataPayExchange exchange_db = null;

        foreach (var i in ListDbExchange)
        {
            exchange_db = i;
            break;
        }

        if (exchange_db == null)
        {
            return;
        }

        if (exchange_db.Status == PayExchangeStatus.Create || exchange_db.Status == PayExchangeStatus.Debit)
        {
            // 通知扣款

            var debit = new PayExchange4Debit()
            {
                ExchangeId = exchange_db.ExchangeId,
                AccountId = exchange_db.AccountId,
                PlayerGuid = exchange_db.PlayerGuid,
                ItemTbId = exchange_db.ItemTbId,
                ItemName = exchange_db.ItemName,
                Currency = exchange_db.Currency,
                Amount = exchange_db.Amount,
                IsSandbox = exchange_db.IsSandbox
            };

            string request_data = LitJson.JsonMapper.ToJson(debit);

            using var web_client = HttpClientFactory.CreateClient();
            using var response = await web_client.PostAsync(ConfigPay.GiveItemWebhook, new StringContent(request_data, Encoding.UTF8, "application/json"));
            string response_data = await response.Content.ReadAsStringAsync();

            // 扣款成功
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                exchange_db.Status = PayExchangeStatus.GiveItem;

                var filter = Builders<DataPayExchange>.Filter
                    .Where(x => x.ExchangeId == exchange_db.ExchangeId);
                var update = Builders<DataPayExchange>
                    .Update.Set(x => x.Status, exchange_db.Status);
                await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataPayExchange, update);
            }
        }

        if (exchange_db.Status == PayExchangeStatus.GiveItem)
        {
            // 通知发货
        }

        if (exchange_db.Status == PayExchangeStatus.End)
        {
            // 完成兑换
            ListDbExchange.Remove(exchange_db);
        }
    }
}