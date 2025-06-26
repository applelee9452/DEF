using MongoDB.Driver;

namespace DEF.UCenter;

public class ContainerStatefullVCoinPlayer : ContainerStateful, IContainerStatefulVCoinPlayer
{
    DbClientMongo Db;
    string PlayerGuid { get; set; }
    DataVCoinPlayer DataAccountVCoin { get; set; }

    public override async Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        //HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        //RpcMethod = new RpcMethod(Logger, _notifyClient);
        PlayerGuid = ContainerId;//this.GetPrimaryKey().ToString();

        //RpcMethod.Def<Casinos.Co.VCoinWithdrawAddressValidRequest>(
        //    (ushort)LobbyMethodType.VCoinWithdrawAddressValidRequest, ClientVCoinWithdrawAddressValidRequest);// c->s, 钱包，请求验证提现地址有效性
        //RpcMethod.Def<Casinos.Co.VCoinWithdrawRequest>(
        //    (ushort)LobbyMethodType.VCoinWithdrawRequest, ClientVCoinWithdrawRequest);// c->s, 钱包，请求提现
        //RpcMethod.Def<string, string>(
        //    (ushort)LobbyMethodType.VCoinGetOrGenerateAddressRequest, ClientVCoinGetOrGenerateAddressRequest);// c->s, 钱包，请求获取虚拟币地址
        //RpcMethod.Def(
        //    (ushort)LobbyMethodType.VCoinGetTransListRequest, ClientVCoinGetTransListRequest);// c->s, 钱包，请求获取交易记录
        //RpcMethod.Def(
        //    (ushort)LobbyMethodType.VCoinGetMyVCoinInfoRequest, ClientVCoinGetMyVCoinInfoRequest);// c->s, 钱包，请求获取本人VCoin信息

        DataAccountVCoin = await Db.ReadAsync<DataVCoinPlayer>(
            a => a.Id == PlayerGuid,
            StringDef.DbCollectionDataPlayerVCoin);
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    // PayAzex Webhook，生成指定虚拟币的信息
    async Task IContainerStatefulVCoinPlayer.PayAzexWebhookGenerateAddress(string currency, string address, string memo)
    {
        // 记录Db
        if (DataAccountVCoin == null)
        {
            DataAccountVCoin = new DataVCoinPlayer()
            {
                Id = PlayerGuid,
                ListVCoinItem = []
            };

            await Db.InsertAsync(StringDef.DbCollectionDataPlayerVCoin, DataAccountVCoin);
        }

        if (DataAccountVCoin.ListVCoinItem == null)
        {
            DataAccountVCoin.ListVCoinItem = [];
        }

        bool exist = false;
        foreach (var i in DataAccountVCoin.ListVCoinItem)
        {
            if (i.Currency == currency)
            {
                exist = true;
                if (string.IsNullOrEmpty(i.Address))
                {
                    i.Address = address;
                    i.Memo = memo;
                }
            }
        }

        if (!exist)
        {
            var vcoin_item = new VCoinItem()
            {
                Currency = currency,
                Address = address,
                Memo = memo,
            };
            DataAccountVCoin.ListVCoinItem.Add(vcoin_item);
        }

        await Db.ReplaceOneData(StringDef.DbCollectionDataPlayerVCoin, PlayerGuid, DataAccountVCoin);

        //VCoinMyInfo vcoin_myinfo = _getMyInfo4Client();

        // 推送给Client
        //var notify = new VCoinGetOrGenerateAddressNotify()
        //{
        //    Result = WalletResult.Success,
        //    Currency = currency,
        //    Address = address,
        //    Memo = memo
        //};

        //await RpcMethod.Rpc((ushort)LobbyMethodType.VCoinGetOrGenerateAddressNotify, notify, vcoin_myinfo);
    }

    // PayAzex Webhook，提现地址有效性验证
    Task IContainerStatefulVCoinPlayer.PayAzexWebhookWithdrawAddressValidation(string address, string memo, string isvalid)
    {
        // 推送给Client
        //var notify = new VCoinWithdrawAddressValidNotify()
        //{
        //    Result = WalletResult.Success,
        //    Channel = "Azex",
        //    Address = address,
        //    Memo = memo,
        //    IsValid = bool.Parse(isvalid)
        //};

        //RpcMethod.Rpc((ushort)LobbyMethodType.VCoinWithdrawAddressValidNotify, notify);

        return Task.CompletedTask;
    }

    // PayAzex Webhook，提现状态改变
    async Task IContainerStatefulVCoinPlayer.PayAzexWebhookWithdrawStatusChange(DataVCoinTransRecord record, string withdraw_id, string status)
    {
        await Task.Delay(1);

        //var result = WalletResult.False;

        record.WithdrawId = withdraw_id;
        if (record.Status == "None" || record.Status == "Pendding")
        {
            return;
        }

        // 查询提现结果
        //var grain_payazex_service = GrainFactory.GetGrain<IGrainChannelAzexService>(0);
        //MerchantWithdrawInfoDto dto = await grain_payazex_service.QueryWithdrawRequest(record.WithdrawId);
        //if (dto == null)
        //{
        //    return;
        //}

        //if (record.Status == dto.Status)
        //{
        //    return;
        //}

        //record.Fee = dto.Fee;
        //record.FeeCurrency = dto.FeeCurrency;
        //record.Tag = dto.Tag;
        //record.TxNo = dto.TxNo;
        ////record.ValidResult = dto.ValidResult;
        //record.Status = dto.Status;
        //record.CreatedAt = dto.CreatedAt;
        //record.DoneAt = dto.DoneAt;

        //Logger.LogInformation("Dto WithdrawId={0} Fee={1} FeeCurrency={2} Tag={3} TxNo={4} ValidResult={5} Status={6}",
        //    withdraw_id, dto.Fee, dto.FeeCurrency, dto.Tag, dto.TxNo, dto.ValidResult, dto.Status);

        //await Db.ReplaceOneData(StringDef.DbCollectionDataVCoinTransRecord, record.Id, record);

        //if (dto.Status == "Success")
        //{
        //    // 成功完成

        //    // 减少系统虚拟币总额
        //    var grain_vcoinsys = GrainFactory.GetGrain<IGrainVCoinSys>(StringDef.GrainVCoinSys);
        //    await grain_vcoinsys.SubtractVCoinVolme(dto.Currency, dto.Volume);
        //}
        //else
        //{
        //    // 失败完成，返还预扣除的Gold

        //    // 根据汇率计算Gold
        //    var grain_vcoinsys = GrainFactory.GetGrain<IGrainVCoinSys>(StringDef.GrainVCoinSys);
        //    double f = await grain_vcoinsys.QueryGoldVCoinExchangeRate(dto.Currency);
        //    long gold_change = (long)(dto.Volume * (decimal)f);

        //    // todo，返还GoldAcc
        //    //SGoldAccChangeInfo change_info;
        //    //change_info.Reason = GoldAccChangeReason.VCoinWithdrawRequestFaild;
        //    //change_info.ChangeGold = gold_change;
        //    //change_info.FactoryName = string.Empty;
        //    //change_info.ParamS = dto.Volume.ToString();
        //    //change_info.Param = 0;
        //    //var grain_player = GrainFactory.GetGrain<IGrainPlayer>(this.GetPrimaryKey());
        //    //await grain_player.UpdateGoldAcc(change_info);
        //}

        //result = WalletResult.Success;

        // 推送给Client，提现结果
        //var notify = new VCoinTransRecrod()
        //{
        //    Id = record.Id,
        //    ChargeOrWithdraw = true,
        //    Currency = record.Currency,
        //    Dt = DateTime.UtcNow.ToString(),
        //    Volume = (double)record.Volume,
        //    WithdrawStatus = record.Status,
        //};

        //await RpcMethod.Rpc((ushort)LobbyMethodType.VCoinTransNotify, result, notify);
    }

    // PayAzex Webhook，充值
    async Task IContainerStatefulVCoinPlayer.PayAzexWebhookCharge(string id, string currency, string address, string memo, string volume, string fee)
    {
        // 插入一条DataVCoinTransRecord
        var record = new DataVCoinTransRecord()
        {
            Id = id,
            PlayerGuid = PlayerGuid,
            ChargeOrWithdraw = true,
            Currency = currency,
            Address = address,
            Memo = memo,
            Volume = decimal.Parse(volume),
            Fee = 0,
            FeeCurrency = currency
        };
        await Db.InsertAsync(StringDef.DbCollectionDataVCoinTransRecord, record);

        // 增加系统虚拟币总额
        //var grain_vcoinsys = GrainFactory.GetGrain<IGrainVCoinSys>(StringDef.GrainVCoinSys);
        //await grain_vcoinsys.AddVCoinVolme(record.Currency, record.Volume);

        // 根据汇率计算Gold
        //double f = await grain_vcoinsys.QueryGoldVCoinExchangeRate(record.Currency);
        //long gold_add = (long)(record.Volume * (decimal)f);

        // todo，增加GoldAcc
        //SGoldAccChangeInfo change_info;
        //change_info.Reason = GoldAccChangeReason.VCoinCharge;
        //change_info.ChangeGold = gold_add;
        //change_info.FactoryName = string.Empty;
        //change_info.ParamS = record.Volume.ToString();
        //change_info.Param = 0;
        //var grain_player = GrainFactory.GetGrain<IGrainPlayer>(this.GetPrimaryKey());
        //await grain_player.UpdateGoldAcc(change_info);

        // 推送给Client
        //var notify = new VCoinTransRecrod()
        //{
        //    Id = id,
        //    ChargeOrWithdraw = true,
        //    Currency = record.Currency,
        //    Dt = DateTime.UtcNow.ToString(),
        //    Volume = (double)record.Volume,
        //    WithdrawStatus = "None"
        //};

        //await RpcMethod.Rpc((ushort)LobbyMethodType.VCoinTransNotify, Casinos.Co.WalletResult.Success, notify);
    }

    // 请求验证提现地址有效性
    //public async Task ClientVCoinWithdrawAddressValidRequest(VCoinWithdrawAddressValidRequest request)
    //{
    //    var grain_payazex_helper = GrainFactory.GetGrain<IGrainChannelAzexHelper>(StringDef.GrainPayAzexHelper);
    //    await grain_payazex_helper.AddPlayerGuid4WithdrawAddressValid(request.Address, request.Memo, PlayerGuid);

    //    // 发出请求
    //    var grain_payazex_service = GrainFactory.GetGrain<IGrainChannelAzexService>(0);
    //    await grain_payazex_service.WithdrawAddressValidationRequest(request.Currency, request.Address, request.Memo);
    //}

    //// 请求提现
    //public async Task ClientVCoinWithdrawRequest(VCoinWithdrawRequest request)
    //{
    //    //var result = WalletResult.False;
    //    string request_id = string.Empty;

    //    // 判定提现条件，例如Gold是否足够。根据汇率计算Gold
    //    var grain_vcoinsys = GrainFactory.GetGrain<IGrainVCoinSys>(StringDef.GrainVCoinSys);
    //    double f = await grain_vcoinsys.QueryGoldVCoinExchangeRate(request.Currency);
    //    long gold_change = (long)(double.Parse(request.Volume) * f);

    //    // 预扣除Gold
    //    //SGoldAccChangeInfo change_info;
    //    //change_info.Reason = GoldAccChangeReason.VCoinWithdrawRequest;
    //    //change_info.ChangeGold = gold_change;
    //    //change_info.FactoryName = string.Empty;
    //    //change_info.ParamS = request.Volume.ToString();
    //    //change_info.Param = 0;
    //    //var grain_player = GrainFactory.GetGrain<IGrainPlayer>(this.GetPrimaryKey());
    //    //bool b = await grain_player.UpdateGoldAcc(change_info);
    //    //if (!b)
    //    //{
    //    //    goto End;
    //    //}

    //    // 记录该提现请求
    //    var record = new DataVCoinTransRecord()
    //    {
    //        Id = Guid.NewGuid().ToString(),
    //        PlayerGuid = PlayerGuid,
    //        ChargeOrWithdraw = false,
    //        Currency = request.Currency,
    //        Address = request.Address,
    //        Memo = request.Memo,
    //        Volume = decimal.Parse(request.Volume),
    //        Fee = 0,
    //        FeeCurrency = request.Currency,
    //        Status = "None",
    //        //ValidResult = Azex.WithdrawlValidResult.None,
    //        CreatedAt = DateTime.UtcNow,
    //    };
    //    await Db.InsertAsync(StringDef.DbCollectionDataVCoinTransRecord, record);

    //    // 发出请求
    //    var grain_payazex_service = GrainFactory.GetGrain<IGrainChannelAzexService>(0);
    //    await grain_payazex_service.WithdrawRequest(
    //        record.Id, request.Currency, request.Volume.ToString(), request.Address, request.Memo);

    //    //result = WalletResult.Success;
    //    request_id = record.Id;

    //    //End:

    //    // 推送给Client
    //    //var notify = new VCoinTransRecrod()
    //    //{
    //    //    Id = request_id,
    //    //    ChargeOrWithdraw = false,
    //    //    Currency = request.Currency,
    //    //    Dt = DateTime.UtcNow.ToString(),
    //    //    Volume = double.Parse(request.Volume),
    //    //    WithdrawStatus = "None",
    //    //};

    //    //await RpcMethod.Rpc((ushort)LobbyMethodType.VCoinTransNotify, result, notify);
    //}

    // 请求获取虚拟币地址
    //public async Task ClientVCoinGetOrGenerateAddressRequest(string currency, string channel)
    //{
    //    var data_vcoinitem = _getVCoinItem(currency);
    //    if (data_vcoinitem != null)
    //    {
    //        // 推送给Client
    //        //var notify = new VCoinGetOrGenerateAddressNotify()
    //        //{
    //        //    Result = WalletResult.Success,
    //        //    Currency = data_vcoinitem.Currency,
    //        //    Address = data_vcoinitem.Address,
    //        //    Memo = data_vcoinitem.Memo
    //        //};

    //        //await RpcMethod.Rpc((ushort)LobbyMethodType.VCoinGetOrGenerateAddressNotify, notify);

    //        return;
    //    }

    //    // 生成Address的请求是否已经在队列中
    //    //var grain_payazex_helper = GrainFactory.GetGrain<IGrainChannelAzexHelper>(StringDef.GrainPayAzexHelper);
    //    //bool is_exist = await grain_payazex_helper.IsExistPlayerGuid4GenerateAddress(PlayerGuid);
    //    //if (is_exist)
    //    //{
    //    //    return;
    //    //}

    //    //await grain_payazex_helper.AddPlayerGuid4GenerateAddress(PlayerGuid);

    //    //// 发出请求
    //    //var grain_payazex_service = GrainFactory.GetGrain<IGrainChannelAzexService>(0);
    //    //await grain_payazex_service.GenerateAddressRequest(currency);
    //}

    // 请求获取交易记录
    public async Task ClientVCoinGetTransListRequest()
    {
        var collection = Db.Database.GetCollection<DataVCoinTransRecord>(typeof(DataVCoinTransRecord).Name);
        var filter = Builders<DataVCoinTransRecord>.Filter
            .Where(x => x.PlayerGuid == PlayerGuid);
        var list = await collection
            .Find(filter)
            .Limit(10)
            .ToListAsync();

        //List<VCoinTransRecrod> list_transrecord = new();
        //if (list != null && list.Count > 0)
        //{
        //    foreach (var i in list)
        //    {
        //        VCoinTransRecrod record = new()
        //        {
        //            Id = i.Id,
        //            ChargeOrWithdraw = i.ChargeOrWithdraw,
        //            Currency = i.Currency,
        //            Dt = i.CreatedAt.ToString(),
        //            Volume = (double)i.Volume,
        //            WithdrawStatus = i.Status,
        //        };
        //        list_transrecord.Add(record);
        //    }
        //}

        //await RpcMethod.Rpc((ushort)LobbyMethodType.VCoinGetTransListNotify, list_transrecord);
    }

    // 请求获取本人VCoin信息
    public Task ClientVCoinGetMyVCoinInfoRequest()
    {
        //VCoinMyInfo vcoin_myinfo = _getMyInfo4Client();

        //return RpcMethod.Rpc((ushort)LobbyMethodType.VCoinGetMyVCoinInfoResponse, vcoin_myinfo);

        return Task.CompletedTask;
    }

    // 获取指定虚拟币的地址信息
    VCoinItem _getVCoinItem(string currency)
    {
        VCoinItem item = null;
        if (DataAccountVCoin == null || DataAccountVCoin.ListVCoinItem == null)
        {
            return item;
        }
        foreach (var i in DataAccountVCoin.ListVCoinItem)
        {
            if (i.Currency == currency)
            {
                item = i;
                break;
            }
        }
        return item;
    }

    // 通过GrainPlayer转发
    Task _notifyClient(ushort method_id, byte[] method_data)
    {
        //var grain_player = GrainFactory.GetGrain<IGrainPlayer>(this.GetPrimaryKey());
        //return grain_player.ClientNotify(method_id, method_data);

        return Task.CompletedTask;
    }

    // 获取Client所需的本人信息
    //VCoinMyInfo _getMyInfo4Client()
    //{
    //    VCoinMyInfo vcoin_myinfo = new()
    //    {
    //        ListItem = new List<VCoinMyInfoItem>()
    //    };

    //    if (DataAccountVCoin != null && DataAccountVCoin.ListVCoinItem != null)
    //    {
    //        foreach (var i in DataAccountVCoin.ListVCoinItem)
    //        {
    //            VCoinMyInfoItem item = new()
    //            {
    //                Currency = i.Currency,
    //                Address = i.Address,
    //                Memo = i.Memo
    //            };
    //            vcoin_myinfo.ListItem.Add(item);
    //        }
    //    }

    //    return vcoin_myinfo;
    //}
}