using MemoryPack;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using StackExchange.Redis;

namespace DEF.UCenter;

public class PayGiveItemInfo
{
    public DataPayCharge DataPayCharge { get; set; }
    public float LeftTm { get; set; }// 剩余多久发货时间通知发货
    public bool IsFinish { get; set; } = false;
}

public class ContainerStatefullCharge : ContainerStateful, IContainerStatefulCharge
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    ConfigPay ConfigPay { get; set; }
    DbClientMongo DbPay { get; set; }
    string AccountId { get; set; }
    List<DataPayCharge> ListDbCharge { get; set; } = [];// 未完成订单列表
    List<PayGiveItemInfo> ListGiveItem { get; set; } = [];// 待发货列表

    public override async Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        AccountId = ContainerId;
        DbPay = UCenterContext.Instance.Db;

        // 激活时查询与本账号关联的所有未完成订单
        ListDbCharge = await DbPay.ReadListAsync<DataPayCharge>(
            a => a.AccountId == AccountId && a.Status <= PayChargeStatus.FinishAndNotGiveItem,
            StringDef.DbCollectionDataPayCharge);

        ListDbCharge ??= [];

        ConfigPay = new ConfigPay();

        ListGiveItem = new List<PayGiveItemInfo>();
        foreach (var i in ListDbCharge)
        {
            if (i.Status == PayChargeStatus.FinishAndNotGiveItem)
            {
                var give_item = new PayGiveItemInfo
                {
                    DataPayCharge = i,
                    LeftTm = 0f
                };
                ListGiveItem.Add(give_item);
            }
        }

        if (ListGiveItem.Count > 0)
        {
            TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(), null,
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }
    }

    public override Task OnDestroy()
    {
        if (TimerHandleUpdate != null)
        {
            TimerHandleUpdate.Dispose();
            TimerHandleUpdate = null;
        }

        return Task.CompletedTask;
    }

    // 获取订单详情
    async Task<PayChargeDetail> IContainerStatefulCharge.PayChargeGetDetail(string charge_id, string client_ip)
    {
        var charge_detail = new PayChargeDetail
        {
            ErrorCode = PayErrorCode.Error
        };

        var charge_db = await DbPay.ReadAsync<DataPayCharge>(
            a => a.AccountId == AccountId && a.Id == charge_id,
            StringDef.DbCollectionDataPayCharge);
        if (charge_db != null)
        {
            charge_detail.ChargeId = charge_db.Id;
            charge_detail.Status = charge_db.Status;
            charge_detail.AppId = charge_db.AppId;
            charge_detail.AccountId = charge_db.AccountId;
            charge_detail.ItemTbId = charge_db.ItemTbId;
            charge_detail.ItemName = charge_db.ItemName;
            charge_detail.Platform = charge_db.Platform;
            charge_detail.Amount = charge_db.Amount;
            charge_detail.Currency = charge_db.Currency;
            charge_detail.IAPProductId = charge_db.IAPProductId;
            charge_detail.Receipt = charge_db.Receipt;
            charge_detail.Transaction = charge_db.Transaction;
            charge_detail.PurchaseToken = charge_db.PurchaseToken;
        }

        charge_detail.ErrorCode = PayErrorCode.NoError;

        return charge_detail;
    }

    // 创建订单
    async Task<PayChargeDetail> IContainerStatefulCharge.PayChargeCreate(PayCreateChargeRequest request, string client_ip)
    {
        Logger.LogInformation("ContainerStatefullCharge.PayChargeCreate() ClientIp={0} request.Platform={1}", client_ip, request.Platform);

        PayChargeDetail charge_detail = null;

        // 验证需要创建的订单的合法性
        foreach (var i in ListDbCharge)
        {
            if (!PayChannelTraits.ChargeProductIdConcurrency(request.Platform))
            {
                if (i.Platform == request.Platform && i.ItemTbId == request.ItemTbId)
                {
                    charge_detail = new PayChargeDetail()
                    {
                        ErrorCode = PayErrorCode.PayOrderRepeat,// 计费点重复
                        ChargeId = i.Id,
                        Status = i.Status,
                        AppId = i.AppId,
                        AccountId = i.AccountId,
                        ItemTbId = i.ItemTbId,
                        ItemName = i.ItemName,
                        Platform = i.Platform,
                        Amount = i.Amount,
                        Currency = i.Currency,
                        IAPProductId = i.IAPProductId,
                        Receipt = string.Empty,
                        Transaction = i.Transaction,
                        PurchaseToken = i.PurchaseToken
                    };

                    goto End;
                }
            }
        }

        var account_entity = await Db.ReadAsync<DataAccount>(a => a.Id == request.AccountId, StringDef.DbCollectionDataAccount);
        if (account_entity == null)
        {
            // 指定AccountId的账号不存在
            charge_detail = new PayChargeDetail
            {
                ErrorCode = PayErrorCode.Error
            };

            Logger.LogError($"指定AccountId的账号不存在 {request.AccountId} ");

            goto End;
        }

        string charge_id = Guid.NewGuid().ToString();
        DataPayCharge charge_db = new()
        {
            Id = charge_id,
            Status = PayChargeStatus.Created,
            CreatedTime = DateTime.UtcNow,
            IAPProductId = request.IAPProductId,
            Platform = request.Platform,
            ItemTbId = request.ItemTbId,
            ItemName = request.ItemName,
            AccountId = request.AccountId,
            AgentParents = account_entity.AgentParents,
            PlayerGuid = request.PlayerGuid,
            AppId = request.AppId,
            Amount = request.Amount,
            Currency = request.Currency,
            PayType = request.PayType,
            PurchaseToken = string.Empty,
            IsSandbox = false,
        };

        await DbPay.InsertAsync(StringDef.DbCollectionDataPayCharge, charge_db);

        charge_detail = new PayChargeDetail()
        {
            ErrorCode = PayErrorCode.NoError,
            ChargeId = charge_db.Id,
            Status = charge_db.Status,
            AppId = charge_db.AppId,
            AccountId = charge_db.AccountId,
            ItemTbId = charge_db.ItemTbId,
            ItemName = charge_db.ItemName,
            Platform = charge_db.Platform,
            Amount = charge_db.Amount,
            Currency = charge_db.Currency,
            IAPProductId = charge_db.IAPProductId,
            Receipt = string.Empty,
            Transaction = charge_db.Transaction,
            PurchaseToken = charge_db.PurchaseToken,
            PayType = charge_db.PayType,
        };

        ListDbCharge.Add(charge_db);

        // 请求第三方支付
        bool is_create_by_server = charge_detail.Platform == PayPlatform.Meida;
        if (is_create_by_server)
        {
            var container = GetContainerRpc<IContainerStatelessChannelMeida>();
            charge_detail = await container.MeidaRequest(charge_detail, client_ip);

            if (charge_detail.Status > PayChargeStatus.Error)
            {
                await ((IContainerStatefulCharge)this).PayChargeErrorClose(charge_detail.ChargeId, PayErrorCode.PayChargeCreateFail);
            }
            else
            {
                charge_db.ThirdPartyPayOrderId = charge_detail.ThirdPartyPayOrderId;

                var filter = Builders<DataPayCharge>.Filter.Where(x => x.Id == charge_db.Id);
                var update = Builders<DataPayCharge>
                    .Update
                    .Set(x => x.UpdatedTime, DateTime.UtcNow)
                    .Set(x => x.ThirdPartyPayOrderId, charge_db.ThirdPartyPayOrderId);
                await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataPayCharge, update);
            }
        }

    End:
        return charge_detail;
    }

    // 取消订单
    async Task<PayChargeInfo> IContainerStatefulCharge.PayChargeCancel(string charge_id, string client_ip)
    {
        Logger.LogInformation("ContainerStatefullCharge.PayChargeCancel() ClientIp={ClientIp}", client_ip);

        PayChargeInfo charge_info = null;
        DataPayCharge charge_db = null;

        foreach (var i in ListDbCharge)
        {
            if (i.Id == charge_id)
            {
                charge_db = i;
                break;
            }
        }

        if (charge_db == null)
        {
            charge_info = new()
            {
                ErrorCode = PayErrorCode.Error,
                ChargeId = charge_id,
                Platform = PayPlatform.Meida,
                Status = PayChargeStatus.Error,
                Amount = 0,
                IAPProductId = string.Empty,
            };

            return charge_info;
        }

        if (charge_db.Status != PayChargeStatus.Created)
        {
            charge_info = new()
            {
                ErrorCode = PayErrorCode.Error,
                ChargeId = charge_id,
                Platform = PayPlatform.Meida,
                Status = PayChargeStatus.Error,
                Amount = 0,
                IAPProductId = string.Empty,
            };

            return charge_info;
        }

        charge_db.Status = PayChargeStatus.Cancel;
        ListDbCharge.Remove(charge_db);

        var filter = Builders<DataPayCharge>.Filter
            .Where(x => x.Id == charge_db.Id);
        var update = Builders<DataPayCharge>
            .Update.Set(x => x.Status, charge_db.Status);
        await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataPayCharge, update);

        charge_info = new()
        {
            ErrorCode = PayErrorCode.NoError,
            ChargeId = charge_id,
            Platform = charge_db.Platform,
            Status = charge_db.Status,
            Amount = charge_db.Amount,
            IAPProductId = charge_db.IAPProductId,
        };

        return charge_info;
    }

    // 确认收货Ack
    async Task<PayChargeInfo> IContainerStatefulCharge.PayChargeAck(string charge_id, string client_ip)
    {
        Logger.LogInformation("ContainerStatefullCharge.PayChargeAck() ClientIp={ClientIp}", client_ip);

        PayChargeInfo charge_info = null;
        DataPayCharge charge_db = null;

        for (int i = 0; i < ListGiveItem.Count; i++)
        {
            var give_item = ListGiveItem[i];

            if (give_item.DataPayCharge.Id == charge_id)
            {
                give_item.IsFinish = true;
            }
        }

        foreach (var i in ListDbCharge)
        {
            if (i.Id == charge_id)
            {
                charge_db = i;
                break;
            }
        }

        if (charge_db == null)
        {
            charge_info = new()
            {
                ErrorCode = PayErrorCode.Error,
                ChargeId = charge_id,
                Platform = PayPlatform.Meida,
                Status = PayChargeStatus.Error,
                Amount = 0,
                IAPProductId = string.Empty,
            };

            return charge_info;
        }

        if (charge_db.Status != PayChargeStatus.FinishAndNotGiveItem)
        {
            charge_info = new()
            {
                ErrorCode = PayErrorCode.Error,
                ChargeId = charge_id,
                Platform = charge_db.Platform,
                Status = charge_db.Status,
                Amount = charge_db.Amount,
                IAPProductId = charge_db.IAPProductId,
            };

            return charge_info;
        }

        charge_db.Status = PayChargeStatus.AckConfirm;
        ListDbCharge.Remove(charge_db);

        var filter = Builders<DataPayCharge>.Filter
            .Where(x => x.Id == charge_db.Id);
        var update = Builders<DataPayCharge>
            .Update
            .Set(x => x.Status, charge_db.Status)
            .Set(x => x.UpdatedTime, DateTime.UtcNow);
        await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataPayCharge, update);

        charge_info = new()
        {
            ErrorCode = PayErrorCode.NoError,
            ChargeId = charge_id,
            Platform = charge_db.Platform,
            Status = charge_db.Status,
            Amount = charge_db.Amount,
            IAPProductId = charge_db.IAPProductId,
        };

        return charge_info;
    }

    // 验证订单
    //async Task<PayChargeInfo> IContainerStatefulCharge.PayChargeVerify(PayVerifyChargeRequest request, string client_ip)
    //{
    //    Logger.LogInformation("ContainerStatefullCharge.PayChargeVerify() ClientIp={ClientIp}", client_ip);

    //    var response = new PayChargeInfo()
    //    {
    //        ErrorCode = PayErrorCode.Error
    //    };

    //    if (request == null)
    //    {
    //        Logger.LogError("参数不合法 Request==null");
    //        //var response_str = Newtonsoft.Json.JsonConvert.SerializeObject(response);
    //    }

    //    var charge_db = await DbPay.ReadAsync<DataPayCharge>(
    //        a => a.Id == request.ChargeId,
    //        StringDef.DbCollectionDataPayCharge);
    //    if (charge_db == null)
    //    {
    //        Logger.LogError("订单不存在 ChargeId={0}", request.ChargeId);

    //        response.ErrorCode = PayErrorCode.PayInvalidOrder;
    //        //var response_str = Newtonsoft.Json.JsonConvert.SerializeObject(response);

    //        return response;
    //    }

    //    if (charge_db.Platform == PayPlatform.GooglePlay || charge_db.Platform == PayPlatform.AppStore || charge_db.Platform == PayPlatform.WindowsStore)
    //    {
    //        // 客户端校验，GoogleIAP，AppleIAP，WindowsStore
    //        return await VerifyChargeOfClientVerify(charge_db, request, client_ip);
    //    }
    //    else
    //    {
    //        // 服务端Webhook，Enjoy渠道
    //        return await VerifyChargeOfServerVerify(charge_db, request, client_ip);
    //    }
    //}

    // 结束订单

    // 第三方支付通知支付完成，WebHook
    async Task<PayChargeInfo> IContainerStatefulCharge.PayChargeSuccessComplete(string charge_id, string client_ip)
    {
        Logger.LogInformation("ContainerStatefullCharge.PayChargeSuccessComplete() ClientIp={ClientIp}", client_ip);

        PayChargeInfo charge_info = null;
        DataPayCharge charge_db = null;

        foreach (var i in ListDbCharge)
        {
            if (i.Id == charge_id)
            {
                charge_db = i;
                break;
            }
        }

        if (charge_db == null)
        {
            charge_info = new PayChargeInfo()
            {
                ErrorCode = PayErrorCode.Error
            };

            return charge_info;
        }

        if (charge_db.Status != PayChargeStatus.Created)
        {
            charge_info = new PayChargeInfo()
            {
                ErrorCode = PayErrorCode.Error
            };

            return charge_info;
        }

        charge_db.Status = PayChargeStatus.FinishAndNotGiveItem;

        var filter = Builders<DataPayCharge>.Filter
            .Where(x => x.Id == charge_db.Id);
        var update = Builders<DataPayCharge>
            .Update
            .Set(x => x.Status, charge_db.Status)
            .Set(x => x.UpdatedTime, DateTime.UtcNow);
        await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataPayCharge, update);

        var give_item = new PayGiveItemInfo
        {
            DataPayCharge = charge_db,
            LeftTm = 0f
        };
        ListGiveItem.Add(give_item);

        if (ListGiveItem.Count > 0 && TimerHandleUpdate == null)
        {
            // todo，待整理
            TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(), null,
                TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        charge_info ??= new PayChargeInfo()
        {
            ErrorCode = PayErrorCode.NoError
        };

        return charge_info;
    }

    async Task IContainerStatefulCharge.PayChargeErrorClose(string charge_id, PayErrorCode pay_error_code)
    {
        DataPayCharge charge_db = null;

        foreach (var i in ListDbCharge)
        {
            if (i.Id == charge_id)
            {
                charge_db = i;
                ListDbCharge.Remove(i);// 从Client列表中移除该笔订单
                break;
            }
        }

        if (charge_db != null)
        {
            charge_db.Status = PayChargeStatus.Error;

            var filter = Builders<DataPayCharge>.Filter
                .Where(x => x.Id == charge_db.Id);
            var update = Builders<DataPayCharge>
                .Update
                .Set(x => x.Status, charge_db.Status);
            await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataPayCharge, update);
        }
    }

    // 第三方服务器通知完成订单
    async Task<string> IContainerStatefulCharge.PayChargeFinishByServer(DataPayCharge charge, string client_ip)
    {
        await ((IContainerStatefulCharge)this).PayChargeSuccessComplete(charge.Id, client_ip);

        string result = string.Empty;
        return result;
    }

    //// 客户端校验类型的支付渠道
    //async Task<PayChargeInfo> VerifyChargeOfClientVerify(DataPayCharge charge_db, PayVerifyChargeRequest request, string client_ip)
    //{
    //    PayChargeInfo charge_info = null;
    //    DataPayCharge charge_db_cache = null;

    //    foreach (var i in ListDbCharge)
    //    {
    //        if (i.Id == request.ChargeId)
    //        {
    //            charge_db_cache = i;
    //            break;
    //        }
    //    }

    //    foreach (var i in ListChargeInfo)
    //    {
    //        if (i.ChargeId == request.ChargeId)
    //        {
    //            charge_info = i;
    //            break;
    //        }
    //    }

    //    // 检测订单是否存在
    //    if (charge_db_cache == null || charge_info == null)
    //    {
    //        Logger.LogError("订单不存在 ChargeId={0}", request.ChargeId);

    //        charge_info = new PayChargeInfo()
    //        {
    //            ErrorCode = PayErrorCode.PayInvalidOrder
    //        };
    //        goto End;
    //    }

    //    // 检测订单状态合法性
    //    if (charge_db_cache.Status > PayChargeStatus.PayAndNotVerify)
    //    {
    //        Logger.LogError("订单状态不合法 ChargeId={0}, Status={1}", charge_db_cache.Id, charge_db_cache.Status);

    //        charge_info.ErrorCode = PayErrorCode.PayUnauthorized;
    //        goto End;
    //    }

    //    charge_info.Status = PayChargeStatus.PayAndNotVerify;// 先更新订单状态为已支付未校验
    //    charge_db_cache.Status = PayChargeStatus.PayAndNotVerify;

    //    Logger.LogInformation("ChargeId={0} Platform={1} Amount={2}", charge_db_cache.Id, charge_db_cache.PlayerGuid, charge_db_cache.Amount);

    //    // 去GooglePlay&AppStore校验订单合法性
    //    switch (charge_db_cache.Platform)
    //    {
    //        case PayPlatform.AppStore:
    //            {
    //                var grain_channel_applestoreiap = GetContainerRpc<IContainerStatelessChannelAppleStoreIAP>();
    //                var verify_response = await grain_channel_applestoreiap.VerifyReceipt(request, charge_db_cache.Transaction, charge_db_cache.IAPProductId);
    //                charge_db_cache.IsSandbox = verify_response.IsSandbox;
    //                if (!string.IsNullOrEmpty(verify_response.Transaction))
    //                {
    //                    charge_db_cache.Transaction = verify_response.Transaction;
    //                }
    //                if (!string.IsNullOrEmpty(verify_response.IAPProductId))
    //                {
    //                    charge_db_cache.IAPProductId = verify_response.IAPProductId;
    //                }
    //                if (verify_response.PayErrorCode != PayErrorCode.NoError)
    //                {
    //                    charge_info.ErrorCode = verify_response.PayErrorCode;
    //                    goto End;
    //                }
    //            }
    //            break;
    //        case PayPlatform.GooglePlay:
    //            {
    //                var grain_channel_googleplayiap = GetContainerRpc<IContainerStatelessChannelGooglePlayIAP>();
    //                var verify_response = await grain_channel_googleplayiap.VerifyReceipt(request, charge_db_cache.Transaction, charge_db_cache.IAPProductId);
    //                charge_db_cache.IsSandbox = verify_response.IsSandbox;
    //                if (!string.IsNullOrEmpty(verify_response.Transaction))
    //                {
    //                    charge_db_cache.Transaction = verify_response.Transaction;
    //                }
    //                if (!string.IsNullOrEmpty(verify_response.IAPProductId))
    //                {
    //                    charge_db_cache.IAPProductId = verify_response.IAPProductId;
    //                }
    //                if (!string.IsNullOrEmpty(verify_response.PurchaseToken))
    //                {
    //                    charge_db_cache.PurchaseToken = verify_response.PurchaseToken;
    //                }
    //                if (verify_response.PayErrorCode != PayErrorCode.NoError)
    //                {
    //                    charge_info.ErrorCode = verify_response.PayErrorCode;
    //                    goto End;
    //                }
    //            }
    //            break;
    //        case PayPlatform.WindowsStore:
    //            {
    //                var grain_channel_windowsstoreiap = GetContainerRpc<IContainerStatelessChannelWindowsStoreIAP>();
    //                var verify_response = await grain_channel_windowsstoreiap.VerifyReceipt(request, charge_db_cache.Transaction, charge_db_cache.IAPProductId);
    //                charge_db_cache.IsSandbox = verify_response.IsSandbox;
    //                if (!string.IsNullOrEmpty(verify_response.Transaction))
    //                {
    //                    charge_db_cache.Transaction = verify_response.Transaction;
    //                }
    //                if (!string.IsNullOrEmpty(verify_response.IAPProductId))
    //                {
    //                    charge_db_cache.IAPProductId = verify_response.IAPProductId;
    //                }
    //                if (verify_response.PayErrorCode != PayErrorCode.NoError)
    //                {
    //                    charge_info.ErrorCode = verify_response.PayErrorCode;
    //                    goto End;
    //                }
    //            }
    //            break;
    //        default:
    //            break;
    //    }

    //    charge_info.ErrorCode = PayErrorCode.NoError;

    //End:

    //    if (charge_info.ErrorCode == PayErrorCode.NoError)
    //    {
    //        // 校验成功，更新Db

    //        charge_db_cache.Status = PayChargeStatus.VerifyAndNotFinish;
    //        charge_info.Status = PayChargeStatus.VerifyAndNotFinish;

    //        var filter = Builders<DataPayCharge>.Filter
    //            .Where(x => x.Id == charge_db_cache.Id);
    //        var update = Builders<DataPayCharge>.Update
    //            .Set(x => x.Status, charge_db_cache.Status)
    //            .Set(x => x.IAPProductId, charge_db_cache.IAPProductId)
    //            .Set(x => x.IsSandbox, charge_db_cache.IsSandbox)
    //            .Set(x => x.Transaction, charge_db_cache.Transaction)
    //            .Set(x => x.PurchaseToken, charge_db_cache.PurchaseToken);
    //        await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataPayCharge, update);

    //        Logger.LogInformation("订单校验成功 ChargeId={0}, Status={1}", request.ChargeId, charge_db_cache.Status);
    //    }
    //    else
    //    {
    //        // 校验失败，写入校验失败日志，纳入安全防范

    //        Logger.LogError("订单校验失败 ChargeId={0}, Status={1}", request.ChargeId, charge_db.Status);
    //    }

    //    return charge_info;
    //}

    //// 服务端校验类型的支付渠道
    //async Task<PayChargeInfo> VerifyChargeOfServerVerify(DataPayCharge charge_db, PayVerifyChargeRequest request, string client_ip)
    //{
    //    PayChargeInfo charge_info = null;
    //    DataPayCharge charge_db_cache = null;

    //    // 检测订单状态，Webhook是否已经提前处理
    //    if (charge_db.Status > PayChargeStatus.PayAndNotVerify)
    //    {
    //        charge_info = new PayChargeInfo()
    //        {
    //            ErrorCode = PayErrorCode.NoError
    //        };
    //        goto End;
    //    }

    //    foreach (var i in ListDbCharge)
    //    {
    //        if (i.Id == request.ChargeId)
    //        {
    //            charge_db_cache = i;
    //            break;
    //        }
    //    }

    //    foreach (var i in ListChargeInfo)
    //    {
    //        if (i.ChargeId == request.ChargeId)
    //        {
    //            charge_info = i;
    //            break;
    //        }
    //    }

    //    // 检测订单是否存在
    //    if (charge_db_cache == null || charge_info == null)
    //    {
    //        Logger.LogError("订单不存在 ChargeId={0}", request.ChargeId);

    //        charge_info.ErrorCode = PayErrorCode.NoError;
    //        goto End;
    //    }

    //    // 检测订单状态合法性
    //    if (charge_db_cache.Status > PayChargeStatus.PayAndNotVerify)
    //    {
    //        Logger.LogError("订单状态不合法 ChargeId={0}, Status={1}", charge_db_cache.Id, charge_db_cache.Status);

    //        charge_info.ErrorCode = PayErrorCode.NoError;
    //        goto End;
    //    }

    //    charge_info.ErrorCode = PayErrorCode.NoError;

    //End:

    //    if (charge_info.ErrorCode == PayErrorCode.NoError)
    //    {
    //        // 校验成功，更新Db

    //        if (charge_info != null && charge_info.Status < PayChargeStatus.VerifyAndNotFinish)
    //        {
    //            charge_info.Status = PayChargeStatus.VerifyAndNotFinish;
    //        }

    //        if (charge_db_cache != null && charge_db_cache.Status < PayChargeStatus.VerifyAndNotFinish)
    //        {
    //            charge_db_cache.Status = PayChargeStatus.VerifyAndNotFinish;

    //            var filter = Builders<DataPayCharge>.Filter
    //                .Where(x => x.Id == charge_db_cache.Id);
    //            var update = Builders<DataPayCharge>.Update
    //                .Set(x => x.Status, charge_db_cache.Status)
    //                .Set(x => x.IAPProductId, charge_db_cache.IAPProductId)
    //                .Set(x => x.IsSandbox, charge_db_cache.IsSandbox)
    //                .Set(x => x.Transaction, charge_db_cache.Transaction)
    //                .Set(x => x.PurchaseToken, charge_db_cache.PurchaseToken);
    //            await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataPayCharge, update);
    //        }

    //        if (charge_db_cache != null)
    //        {
    //            Logger.LogInformation("订单校验成功 ChargeId={0}, Status={1}", request.ChargeId, charge_db_cache.Status);
    //        }
    //        else
    //        {
    //            Logger.LogInformation("订单校验成功 ChargeId={0}, Status={1}", request.ChargeId, charge_db.Status);
    //        }
    //    }
    //    else
    //    {
    //        // 校验失败，写入校验失败日志，纳入安全防范

    //        Logger.LogError("订单校验失败 ChargeId={0}, Status={1}", request.ChargeId, charge_db.Status);
    //    }

    //    return charge_info;
    //}

    // 广告，客户端请求创建
    async Task<string> IContainerStatefulCharge.AdCreate()
    {
        var ad_list = await DbPay.ReadListAsync<DataAd>(
            a => a.AccountId == AccountId && a.AdStatus <= AdStatus.AdCreate,
            StringDef.DbCollectionDataAd);

        string ad_guid;
        if (ad_list.Count == 0)
        {
            ad_guid = Guid.NewGuid().ToString();

            DataAd ad_db = new()
            {
                Id = ad_guid,
                AdStatus = AdStatus.AdCreate,
                AdType = AdType.GroMore,
                Timestamp = DateTime.UtcNow,
                AdUnit = string.Empty,
                AccountId = AccountId,
                PlayerGuid = string.Empty,
                RewardAmount = 0,
                RewardItem = string.Empty,
                Timeout = 5 * 60 // 5分钟 没有使用 就作废
            };

            await DbPay.InsertAsync(StringDef.DbCollectionDataAd, ad_db);

            Logger.LogInformation("新创建广告订单 AccountId={0}, ad_guid={1}", AccountId, ad_guid);
        }
        else
        {

            ad_guid = ad_list[0].Id;

            Logger.LogInformation("使用已经存在的广告订单 AccountId={0}, ad_guid={1}", AccountId, ad_guid);
        }

        return ad_guid;
    }

    // 广告，客户端观看完成
    async Task<bool> IContainerStatefulCharge.AdFinishedShow(string ad_guid)
    {
        var ad_db = await DbPay.ReadAsync<DataAd>(
            a => a.AccountId == AccountId && a.Id == ad_guid,
            StringDef.DbCollectionDataAd);

        if (ad_db != null)
        {
            // 如果是其他状态
            if (ad_db.AdStatus == AdStatus.AdCreate || ad_db.AdStatus == AdStatus.AdVerifySuccess)
            {
                ad_db.AdStatus = AdStatus.AdClientShowFinished;
                ad_db.Timeout = 5.0f;

                var filter = Builders<DataAd>.Filter.Where(x => x.Id == ad_guid);
                var update = Builders<DataAd>
                    .Update.Set(x => x.AdStatus, AdStatus.AdClientShowFinished);
                await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataAd, update);
            }

            Logger.LogInformation("广告订单观看完成 AccountId={0}, ad_guid={1} AdStatus={2}", AccountId, ad_guid, ad_db.AdStatus);
        }
        else
        {
            Logger.LogInformation("广告订单没有找到 AccountId={0}, ad_guid={1}", AccountId, ad_guid);
        }

        return true;
    }

    // 广告，客户端取消观看广告
    async Task<bool> IContainerStatefulCharge.AdCancel(string ad_guid)
    {
        var ad_db = await DbPay.ReadAsync<DataAd>(
            a => a.AccountId == AccountId && a.Id == ad_guid,
            StringDef.DbCollectionDataAd);

        if (ad_db == null)
        {
            Logger.LogInformation("取消广告订单没有找到 AccountId={0}, ad_guid={1}", AccountId, ad_guid);
            return false;
        }

        var filter = Builders<DataAd>.Filter.Where(x => x.Id == ad_guid);
        var update = Builders<DataAd>
            .Update.Set(x => x.AdStatus, AdStatus.AdCancel);
        await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataAd, update);

        Logger.LogInformation("广告订单取消成功 AccountId={0}, ad_guid={1}", AccountId, ad_guid);

        return true;
    }

    // 广告，Admob回调
    async Task<bool> IContainerStatefulCharge.AdCallback(bool ret, string ad_guid, string account_id, string ad_network, string ad_unit, string transaction_id, string key_id, string reward_item, int reward_amount)
    {
        var ad_db = await DbPay.ReadAsync<DataAd>(
        a => a.AccountId == AccountId && a.Id == ad_guid,
        StringDef.DbCollectionDataAd);

        if (ad_db != null)
        {
            // 如果是其他状态
            if (ad_db.AdStatus == AdStatus.AdCreate)
            {
                ad_db.AdStatus = ret ? AdStatus.AdVerifySuccess : AdStatus.AdVerifyFailed;// 验证成功就可以发货了

                var filter = Builders<DataAd>.Filter.Where(x => x.Id == ad_guid);
                var update = Builders<DataAd>.Update
                    .Set(x => x.AdStatus, ad_db.AdStatus)
                    .Set(x => x.AdUnit, ad_unit)
                    .Set(x => x.TransactionId, transaction_id)
                    .Set(x => x.RewardItem, reward_item)
                    .Set(x => x.RewardAmount, reward_amount);
                await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataAd, update);
            }

            Logger.LogInformation("Google广告订单CallBack AccountId={0}, ad_guid={1} AdStatus={2}", AccountId, ad_guid, ad_db.AdStatus);

            return true;
        }
        else
        {
            Logger.LogInformation("Google广告订单没有找到 AccountId={0}, ad_guid={1}", AccountId, ad_guid);
        }

        return false;
    }

    // 广告，GroMore回调
    async Task<bool> IContainerStatefulCharge.GroMoreCallback(bool ret, string user_id, string trans_id, int reward_amount, string reward_name, string prime_rit, string ad_guid)
    {
        var ad_db = await DbPay.ReadAsync<DataAd>(
            a => a.AccountId == AccountId && a.Id == ad_guid,
            StringDef.DbCollectionDataAd);

        if (ad_db != null)
        {
            // 如果是其他状态
            if (ad_db.AdStatus == AdStatus.AdCreate)
            {
                ad_db.AdStatus = ret ? AdStatus.AdVerifySuccess : AdStatus.AdVerifyFailed;// 验证成功就可以发货了

                var filter = Builders<DataAd>.Filter.Where(x => x.Id == ad_guid);
                var update = Builders<DataAd>.Update
                    .Set(x => x.AdStatus, ad_db.AdStatus)
                    .Set(x => x.AccountId, user_id)
                    .Set(x => x.AdUnit, prime_rit)
                    .Set(x => x.TransactionId, trans_id)
                    .Set(x => x.RewardItem, reward_name)
                    .Set(x => x.RewardAmount, reward_amount);
                await DbPay.UpdateOneAsync(filter, StringDef.DbCollectionDataAd, update);

                Logger.LogInformation("GroMore广告订单CallBack AccountId={0}, ad_guid={1} AdStatus={2}", AccountId, ad_guid, ad_db.AdStatus);

                return true;
            }
            else
            {
                // 说明处理过了 不能再处理了

                Logger.LogInformation("GroMore广告订单Callback处理过了CallBack AccountId={0}, ad_guid={1} AdStatus={2}", AccountId, ad_guid, ad_db.AdStatus);

                return false;
            }
        }

        Logger.LogInformation("GroMore广告订单Callback没有找到 AccountId={0}, ad_guid={1}", AccountId, ad_guid);

        return false;
    }

    async Task TimerUpdate()
    {
        if (string.IsNullOrEmpty(ConfigPay.GiveItemWebhook))
        {
            return;
        }

        List<PayGiveItemInfo> wait_remove = null;
        foreach (var give_item_info in ListGiveItem)
        {
            give_item_info.LeftTm -= 1f;

            if (give_item_info.LeftTm <= 0f)
            {
                give_item_info.LeftTm = 5f;

                // 通知发货
                var charge_give_item = new PayChargeGiveItem()
                {
                    ChargeId = give_item_info.DataPayCharge.Id,
                    AccountId = give_item_info.DataPayCharge.AccountId,
                    PlayerGuid = give_item_info.DataPayCharge.PlayerGuid,
                    ItemTbId = give_item_info.DataPayCharge.ItemTbId,
                    Amount = give_item_info.DataPayCharge.Amount,
                    IsSandbox = give_item_info.DataPayCharge.IsSandbox
                };

                DataPayCharge charge_db = null;
                foreach (var k in ListDbCharge)
                {
                    if (k.Id == charge_give_item.ChargeId)
                    {
                        charge_db = k;
                        break;
                    }
                }

                if (charge_db == null) continue;

                if (give_item_info.IsFinish) continue;

                // 消息队列发送物品
                var redis_channel = RedisChannel.Literal(UCenterSubPubChannel.PayCharge.ToString());
                SubPubEvent sub_pub_event = new()
                {
                    EvType = (int)UCenterSubPubEventType.ChargeGiveItem,
                    Data = MemoryPackSerializer.Serialize(charge_give_item)
                };
                byte[] event_bytes = MemoryPackSerializer.Serialize(sub_pub_event);
                await UCenterContext.Instance.DbClientRedis.DB.PublishAsync(redis_channel, event_bytes);
            }
        }

        // 发货处理移除
        foreach (var give_item_info in ListGiveItem)
        {
            if (give_item_info.IsFinish)
            {
                if (wait_remove is null)
                {
                    wait_remove = new List<PayGiveItemInfo>();
                }
                wait_remove.Add(give_item_info);
            }
        }

        if (wait_remove is not null)
        {
            foreach (var re in wait_remove)
            {
                ListGiveItem.Remove(re);
            }
        }
    }
}