using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DEF.UCenter;

public class ContainerStatelessMerchant : ContainerStateless, IContainerStatelessMerchant
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    IOptions<UCenterOptions> UCenterOptions { get; set; }

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;
        UCenterOptions = UCenterContext.Instance.UCenterOptions;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    // 获取商户指定日期推广的用户总数
    async Task<int> IContainerStatelessMerchant.MerchantObtainUserCount(string auth, DateTime begin_time, DateTime end_time)
    {
        await Task.Delay(1);

        return 253;
    }

    // 获取商户指定日期推广的用户列表，分页的，每页最多100条，需要指定页码
    async Task<List<MerchantAccount>> IContainerStatelessMerchant.MerchantObtainUserList(string auth, int page_index, DateTime begin_time, DateTime end_time)
    {
        List<MerchantAccount> list = [];

        await Task.Delay(1);

        int count = 100;
        if (page_index == 0 || page_index == 1)
        {
            count = 100;
        }
        else if (page_index == 2)
        {
            count = 53;
        }
        else
        {
            count = 0;
        }

        for (int i = 0; i < count; i++)
        {
            MerchantAccount acc = new()
            {
                AccountId = Guid.NewGuid().ToString(),
                AccountType = AccountType.NormalAccount,
                AccountStatus = AccountStatus.Active,
                PhoneId = "13800001234",
                NickName = "昵称" + i,
                ParentAccountId = string.Empty,
                JoinDt = DateTime.UtcNow,
                LastLoginDateTime = DateTime.UtcNow,
                LastLoginClientIp = string.Empty,
            };
            list.Add(acc);
        }

        return list;
    }

    // 获取商户的一级代理商的总数
    async Task<int> IContainerStatelessMerchant.MerchantGetTopUserCount(string auth, DateTime begin_time, DateTime end_time)
    {
        await Task.Delay(1);

        return 31;
    }

    // 获取商户指定日期推广的一级代理商列表，分页的，每页最多100条，需要指定页码
    async Task<List<MerchantAccount>> IContainerStatelessMerchant.MerchantGetTopUserList(string auth, int page_index, DateTime begin_time, DateTime end_time)
    {
        List<MerchantAccount> list = [];

        await Task.Delay(1);

        int count = 31;

        for (int i = 0; i < count; i++)
        {
            MerchantAccount acc = new()
            {
                AccountId = Guid.NewGuid().ToString(),
                AccountType = AccountType.NormalAccount,
                AccountStatus = AccountStatus.Active,
                PhoneId = "13800001234",
                NickName = "昵称" + i,
                ParentAccountId = string.Empty,
                JoinDt = DateTime.UtcNow,
                LastLoginDateTime = DateTime.UtcNow,
                LastLoginClientIp = string.Empty,
            };
            list.Add(acc);
        }

        return list;
    }

    // 获取商户的指定日期的指定代理商的直接用户的总数
    async Task<int> IContainerStatelessMerchant.MerchantGetChildrenCount(string auth, string agent_id, DateTime begin_time, DateTime end_time)
    {
        await Task.Delay(1);

        return 5;
    }

    // 获取商户指定日期的指定代理商的直接用户总数，分页的，每页最多100条，需要指定页码
    async Task<List<MerchantAccount>> IContainerStatelessMerchant.MerchantGetChildrenList(string auth, string agent_id, int page_index, DateTime begin_time, DateTime end_time)
    {
        List<MerchantAccount> list = [];

        await Task.Delay(1);

        int count = 5;

        for (int i = 0; i < count; i++)
        {
            MerchantAccount acc = new()
            {
                AccountId = Guid.NewGuid().ToString(),
                AccountType = AccountType.NormalAccount,
                AccountStatus = AccountStatus.Active,
                PhoneId = "13800001234",
                NickName = "昵称" + i,
                ParentAccountId = string.Empty,
                JoinDt = DateTime.UtcNow,
                LastLoginDateTime = DateTime.UtcNow,
                LastLoginClientIp = string.Empty,
            };
            list.Add(acc);
        }

        return list;
    }

    // 获取商户指定日期的所有用户的充值笔数
    async Task<int> IContainerStatelessMerchant.MerchantGetUserRechargeCount(string auth, DateTime begin_time, DateTime end_time)
    {
        await Task.Delay(1);

        return 175;
    }

    // 获取商户指定日期的所有用户的充值列表，分页的，每页最多100条，需要指定页码
    async Task<List<MerchantAccountRecharge>> IContainerStatelessMerchant.MerchantGetUserRechargeList(string auth, int page_index, DateTime begin_time, DateTime end_time)
    {
        List<MerchantAccountRecharge> list = [];

        await Task.Delay(1);

        int count = 100;
        if (page_index == 0)
        {
            count = 100;
        }
        else if (page_index == 1)
        {
            count = 75;
        }
        else
        {
            count = 0;
        }

        for (int i = 0; i < count; i++)
        {
            MerchantAccountRecharge recharge = new()
            {
                AccountId = Guid.NewGuid().ToString(),
                NickName = "昵称" + i,
                Recharge = 500,
                RechargeDt = DateTime.UtcNow,
            };
            list.Add(recharge);
        }

        return list;
    }

    // 加密函数
    //static string EncodeAuthSign(string merchant_id, string merchant_secret)
    //{
    //    DateTime utc_now = DateTime.UtcNow;
    //    string secret_str = $"{merchant_secret}_{utc_now.ToString("yyyyMMddHHmm")}";

    //    using MD5 md5 = MD5.Create();
    //    byte[] input_bytes = Encoding.UTF8.GetBytes(secret_str);
    //    byte[] hash_bytes = md5.ComputeHash(input_bytes);

    //    string sign_str = $"{merchant_id}_{Encoding.UTF8.GetString(hash_bytes)}";

    //    string sign = Convert.ToBase64String(Encoding.UTF8.GetBytes(sign_str));

    //    return sign;
    //}
}