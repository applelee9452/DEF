namespace DEF.UCenter;

public class ContainerStatelessChannelAzexHelper : ContainerStateless, IContainerStatelessChannelAzexHelper
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    Dictionary<string, List<string>> MapPlayerGuid4GenerateAddress { get; set; } = [];// key=PlayerGuid
    Dictionary<string, List<string>> MapPlayerGuid4WithdrawAddressValid { get; set; } = [];// key=address_memo

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    Task IContainerStatelessChannelAzexHelper.AddPlayerGuid4GenerateAddress(string player_guid)
    {
        MapPlayerGuid4GenerateAddress.TryGetValue(player_guid, out var list);
        if (list == null)
        {
            list = new List<string>();
            MapPlayerGuid4GenerateAddress[player_guid] = list;
        }
        list.Add(player_guid);

        return Task.CompletedTask;
    }

    Task<string> IContainerStatelessChannelAzexHelper.GetThenRemovePlayerGuid4GenerateAddress()
    {
        string data = null;
        foreach (var i in MapPlayerGuid4GenerateAddress)
        {
            if (i.Value == null) continue;
            if (i.Value.Count > 0)
            {
                data = i.Value[0];
                i.Value.RemoveAt(0);
                if (i.Value.Count == 0) MapPlayerGuid4GenerateAddress.Remove(i.Key);
                break;
            }
        }

        return Task.FromResult(data);
    }

    Task<bool> IContainerStatelessChannelAzexHelper.IsExistPlayerGuid4GenerateAddress(string player_guid)
    {
        bool b = false;
        MapPlayerGuid4GenerateAddress.TryGetValue(player_guid, out var list);
        if (list != null && list.Count >= 0)
        {
            b = true;
        }
        return Task.FromResult(b);
    }

    Task IContainerStatelessChannelAzexHelper.AddPlayerGuid4WithdrawAddressValid(string address, string memo, string player_guid)
    {
        string key = string.Empty;
        if (string.IsNullOrEmpty(memo))
        {
            key = address;
        }
        else
        {
            key = string.Format("{0}_{1}", address, memo);
        }

        if (string.IsNullOrEmpty(key))
        {
            return Task.CompletedTask;
        }

        MapPlayerGuid4WithdrawAddressValid.TryGetValue(key, out var list);
        if (list == null)
        {
            list = new List<string>();
            MapPlayerGuid4WithdrawAddressValid[key] = list;
        }
        list.Add(player_guid);

        return Task.CompletedTask;
    }

    Task<string> IContainerStatelessChannelAzexHelper.GetThenRemovePlayerGuid4WithdrawAddressValid(string address, string memo)
    {
        string data = null;

        if (string.IsNullOrEmpty(address))
        {
            return Task.FromResult(data);
        }

        string key = string.Empty;
        if (string.IsNullOrEmpty(memo))
        {
            key = address;
        }
        else
        {
            key = string.Format("{0}_{1}", address, memo);
        }

        MapPlayerGuid4WithdrawAddressValid.TryGetValue(key, out var list);
        if (list != null && list.Count >= 0)
        {
            data = list[0];
            list.RemoveAt(0);
            if (list.Count == 0) MapPlayerGuid4WithdrawAddressValid.Remove(key);
        }

        return Task.FromResult(data);
    }
}