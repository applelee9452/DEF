using Microsoft.Extensions.Logging;

namespace DEF.UCenter;

public class ContainerStatefullVCoinSys : ContainerStateful, IContainerStatefulVCoinSys
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    DataVCoinSys DataVCoinSys { get; set; }
    string Key { get; set; }

    public override async Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        Key = "DataVCoinSys";

        DataVCoinSys = await Db.ReadAsync<DataVCoinSys>(
            a => a._id == Key, StringDef.DbCollectionDataVCoinSys);

        if (DataVCoinSys == null)
        {
            DataVCoinSys = new DataVCoinSys()
            {
                _id = Key,
                ListVCoinItem = []
            };
            await Db.InsertAsync(StringDef.DbCollectionDataVCoinSys, DataVCoinSys);
        }
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    // 增加系统VCoin总额
    Task IContainerStatefulVCoinSys.AddVCoinVolme(string currency, decimal volme)
    {
        if (DataVCoinSys.ListVCoinItem == null)
        {
            DataVCoinSys.ListVCoinItem = new List<VCoinSysItem>();
        }

        bool exist = false;
        foreach (var i in DataVCoinSys.ListVCoinItem)
        {
            if (i.Currency == currency)
            {
                i.Volme += volme;
                exist = true;
                break;
            }
        }

        if (!exist)
        {
            VCoinSysItem item = new VCoinSysItem()
            {
                Currency = currency,
                Volme = volme
            };
            DataVCoinSys.ListVCoinItem.Add(item);
        }

        return Db.ReplaceOneData(StringDef.DbCollectionDataVCoinSys, Key, DataVCoinSys);
    }

    // 减少系统VCoin总额
    Task IContainerStatefulVCoinSys.SubtractVCoinVolme(string currency, decimal volme)
    {
        if (DataVCoinSys.ListVCoinItem == null)
        {
            DataVCoinSys.ListVCoinItem = new List<VCoinSysItem>();
        }

        bool exist = false;
        foreach (var i in DataVCoinSys.ListVCoinItem)
        {
            if (i.Currency == currency)
            {
                if (volme > i.Volme)
                {
                    Logger.LogError("VCoinSys余额不足");
                    return Task.CompletedTask;
                }

                i.Volme -= volme;
                exist = true;
                break;
            }
        }

        if (!exist)
        {
            Logger.LogError("VCoinSys不存在Currency={0}", currency);
            return Task.CompletedTask;
        }

        return Db.ReplaceOneData(StringDef.DbCollectionDataVCoinSys, Key, DataVCoinSys);
    }

    // 根据VCoin类型，查询汇率f，Gold=VCoin*f
    Task<double> IContainerStatefulVCoinSys.QueryGoldVCoinExchangeRate(string currency)
    {
        double f = 0;
        if (currency == "btc")
        {
            f = 1;
        }
        else if (currency == "bitcny")
        {
            f = 1;
        }
        return Task.FromResult(f);
    }

    // 根据VCoin类型，查询汇率f，VCoin=Gold*f
    Task<double> IContainerStatefulVCoinSys.QueryVCoinGoldExchangeRate(string currency)
    {
        double f = 0;
        if (currency == "btc")
        {
            f = 0.00000000001;
        }
        else if (currency == "bitcny")
        {
            f = 0.000001;
        }
        return Task.FromResult(f);
    }
}