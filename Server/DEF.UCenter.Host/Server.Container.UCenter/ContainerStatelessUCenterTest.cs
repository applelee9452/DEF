using Microsoft.Extensions.Options;

namespace DEF.UCenter;

public class ContainerStatelessUCenterTest : ContainerStateless, IContainerStatelessUCenterTest
{
    DbClientMongo Db { get; set; }
    IOptions<UCenterOptions> UCenterOptions { get; set; }
    Random Rd { get; set; } = new();

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        UCenterOptions = UCenterContext.Instance.UCenterOptions;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    // 测试
    Task IContainerStatelessUCenterTest.Test()
    {
        return Task.CompletedTask;
    }

    // 批量注册一批用户，绑定已有代理Id
    async Task IContainerStatelessUCenterTest.TestCreateUsers(int count)
    {
        if (count <= 0) return;

        var agent_list = await Db.ReadListAsync<DataAgent>(StringDef.DbCollectionDataAgent);
        int agent_count = agent_list == null ? 0 : agent_list.Count;

        List<Task> tasks = [];
        for (int i = 0; i < count; i++)
        {
            ulong agent_id = 0;
            if (agent_count > 0)
            {
                int index = Rd.Next(0, agent_count);
                agent_id = agent_list[index].AgentId;
            }

            DataAccount data_account = new()
            {
                Id = Guid.NewGuid().ToString(),
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow,
                AccountType = AccountType.NormalAccount,
                AccountStatus = AccountStatus.Active,
                PhoneCode = string.Empty,
                PhoneNumber = string.Empty,
                Email = string.Empty,
                Password = string.Empty,
            };

            data_account.AccountName = $"Test{data_account.Id}";

            // 是通过代理推广注册的用户

            if (agent_id != 0)
            {
                var data_agent = await Db.ReadAsync<DataAgent>(e => e.AgentId == agent_id, StringDef.DbCollectionDataAgent);
                if (data_agent != null && !data_agent.IsDelete)
                {
                    data_account.AgentId = agent_id;

                    ulong[] arr = null;
                    if (data_agent.AgentParents != null && data_agent.AgentParents.Length > 0)
                    {
                        arr = new ulong[1 + data_agent.AgentParents.Length];
                        arr[0] = data_agent.AgentId;
                        data_agent.AgentParents.CopyTo(arr, 1);
                    }
                    else
                    {
                        arr = new ulong[1];
                        arr[0] = data_agent.AgentId;
                    }
                    data_account.AgentParents = arr;
                }
            }

            var t = Db.InsertAsync(StringDef.DbCollectionDataAccount, data_account);
            tasks.Add(t);
        }

        await Task.WhenAll(tasks);
    }

    // 批量生成一批支付订单
    async Task IContainerStatelessUCenterTest.TestCreatePayCharges(int ordercharge_count, int account_count, DateTime dt_begin, DateTime dt_end)
    {
        if (ordercharge_count <= 0 || account_count <= 0)
        {
            return;
        }

        if (dt_begin > dt_end)
        {
            return;
        }

        var list_account = await Db.ReadListAsync<DataAccount>(StringDef.DbCollectionDataAccount);
        if (list_account == null || list_account.Count == 0) return;

        var list_account2 = list_account.Shuffle(Rd);
        if (account_count > list_account2.Count())
        {
            account_count = list_account2.Count();
        }

        var list_account3 = list_account2.Take(account_count);

        for (int i = 0; i < ordercharge_count; i++)
        {
            var account = list_account3.GetRandom(Rd);

            // 生成支付订单
            var data_pay_charge = new DataPayCharge()
            {
                Id = Guid.NewGuid().ToString(),
                Status = PayChargeStatus.AckConfirm,
                AppId = "A",
                AccountId = account.Id,
                AgentParents = account.AgentParents,
                PlayerGuid = string.Empty,
                IsSandbox = false,
                Currency = "RMB",
                Amount = Random.Shared.Next(6, 100) * 100,
                ItemTbId = 1,
                ItemName = "测试",
                Platform = PayPlatform.GooglePlay,
                IAPProductId = "test",
                Receipt = "12345678",
                Transaction = "1234567",
                PurchaseToken = "token"
            };

            // 生成直系代理树

            //var data_account = await Db.ReadAsync<DataAccount>(e => e.Id == data_order_charge.AccountId, StringDef.DbCollectionDataAccount);
            //if (data_account != null)
            //{
            //    data_order_charge.AgentParents = data_account.AgentParents;
            //}

            await Db.InsertAsync(StringDef.DbCollectionDataPayCharge, data_pay_charge);
        }
    }
}