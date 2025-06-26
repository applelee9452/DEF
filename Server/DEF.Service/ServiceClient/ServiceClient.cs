using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace DEF;

public class ServiceClient
{
    public string Id { get; private set; }
    public IOptions<DEFOptions> DEFOptions { get; private set; }
    public IOptions<ServiceClientOptions> ServiceClientOptions { get; private set; }
    public Service Service { get; private set; }
    public Rpcer4Service Rpcer { get; private set; }
    ConcurrentDictionary<string, OrleansClient> MapOrleansClient { get; set; } = new();
    ILogger Logger { get; set; }
    ServiceDiscover ServiceDiscover { get; set; }
    IServiceClientObserverListener ServiceClientObserverListener { get; set; }

    public ServiceClient(ILogger<ServiceClient> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceClientOptions> serviceclient_options,
        IServiceClientObserverListener listener,
        ServiceDiscover service_discover,
        Service service,
        Rpcer4Service rpcer)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClientOptions = serviceclient_options;
        ServiceClientObserverListener = listener;
        ServiceDiscover = service_discover;
        Service = service;
        Rpcer = rpcer;
        Id = Guid.NewGuid().ToString();

        // 去重
        var l = ServiceClientOptions.Value.ServiceDiscoverListenServices;
        HashSet<string> hs = [];

        for (int i = 0; i < l?.Count;)
        {
            var v = l[i];

            if (hs.Contains(v.ServiceName))
            {
                l.RemoveAt(i);
            }
            else
            {
                hs.Add(v.ServiceName);
                i++;
            }
        }

        //ServiceDiscover.ActionServiceNodeChanged += OnServiceNodeChanged;
    }

    public async Task StartAsync(CancellationToken cancellation_token)
    {
        Logger.LogInformation("ServiceClient.StartAsync()");

        // 读取配置文件，创建OrleansClient列表
        var l = ServiceClientOptions.Value.ServiceDiscoverListenServices;
        if (l != null)
        {
            foreach (var i in l)
            {
                OrleansClient oc = new(Logger, DEFOptions, this, i.ServiceName, i.OrleansGatewayPort, i.IsStateful, ServiceClientObserverListener);

                await oc.Start();

                MapOrleansClient[i.ServiceName.ToLower()] = oc;
            }
        }
    }

    public async Task StopAsync(CancellationToken cancellation_token)
    {
        foreach (var i in MapOrleansClient)
        {
            await i.Value.Stop();
        }

        MapOrleansClient.Clear();

        Logger.LogInformation("ServiceClient.StopAsync()");
    }

    public TInterface GetContainerRpc<TInterface>(string container_id) where TInterface : IContainerRpc
    {
        return ((IService)Service).GetContainerRpc<TInterface>(container_id);
    }

    public TInterface GetContainerRpc<TInterface>() where TInterface : IContainerRpc
    {
        return ((IService)Service).GetContainerRpc<TInterface>(string.Empty);
    }

    public IClusterClient GetOrleansClient(string target_servicename)
    {
        MapOrleansClient.TryGetValue(target_servicename.ToLower(), out var client);
        return client?.Client;
    }

    public bool AllConnected()
    {
        bool all_connect = true;
        foreach (var i in MapOrleansClient)
        {
            if (i.Value.Client == null)
            {
                all_connect = false;
                break;
            }
        }

        return all_connect;
    }

    public async Task SessionDisConnect(string player_guid, string session_guid)
    {
        List<Task> list_task = [];
        foreach (var i in MapOrleansClient)
        {
            if (i.Value == null || i.Value.Client == null) continue;

            if (!i.Value.ExistStateful) continue;

            var grain = i.Value.Client.GetGrain<IGrainServiceClient>(Id);
            var task = grain.OnSessionDisConnect(player_guid, session_guid);
            list_task.Add(task);
        }

        try
        {
            await Task.WhenAll(list_task);
        }
        catch (Exception e)
        {
            Logger.LogError("ServiceClient.SessionDisConnect() {Exception}", e.ToString());
        }
    }

    public async Task SessionAuthed(Gateway.GatewayAuthedInfo info, string extra_data)
    {
        foreach (var i in MapOrleansClient)
        {
            if (i.Value == null || i.Value.Client == null) continue;

            if (!i.Value.ExistStateful) continue;

            var grain = i.Value.Client.GetGrain<IGrainServiceClient>(Id);
            await grain.OnSessionConnectedAndAuthed(info, extra_data);
        }
    }

    public async Task SessionTouch()
    {
        List<Task> list_task = [];
        foreach (var i in MapOrleansClient)
        {
            if (i.Value == null || i.Value.Client == null) continue;

            if (!i.Value.ExistStateful) continue;

            var grain = i.Value.Client.GetGrain<IGrainServiceClient>(Id);
            var task = grain.Touch();
            list_task.Add(task);
        }

        try
        {
            await Task.WhenAll(list_task);
        }
        catch (Exception e)
        {
            Logger.LogError("ServiceClient.SessionTouch() {Exception}", e.ToString());
        }
    }

    public async Task<byte[]> ForwardContainerRpc(
        string service_name,
        int containerstate_type,
        string container_type,
        string container_id,
        string method_name,
        byte[] method_data)
    {
        MapOrleansClient.TryGetValue(service_name, out var client);
        if (client == null)
        {
            Logger.LogError("ServiceClient.ForwardContainerRpc() Error，服务名{ServiceName}不存在！", service_name);

            return null;
        }

        if (client.Client == null)
        {
            // todo，log error
            return null;
        }

        switch (containerstate_type)
        {
            case 0:// Stateless
                {
                    var grain = client.Client.GetGrain<IGrainContainerStateless>(container_type);
                    var response_data = await grain.OnContainerRpc2(method_name, method_data);

                    //int response_len = response_data == null ? 0 : response_data.Length;

                    //Logger.LogInformation("ServiceClient.ClientContainerRpc() ContainerId={container_id} MethodName={method_name} RequestLen={len} ResponseLen={response_len}",
                    //    container_id, method_name, len, response_len);

                    return response_data;
                }
            case 1:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStateful>(string.Format("{0}_{1}", container_type, container_id));
                    var response_data = await grain.OnContainerRpc2(method_name, method_data);

                    //int response_len = response_data == null ? 0 : response_data.Length;

                    //Logger.LogInformation("ServiceClient.ClientContainerRpc() ContainerId={container_id} MethodName={method_name} RequestLen={len} ResponseLen={response_len}",
                    //    container_id, method_name, len, response_len);

                    return response_data;
                }
            case 2:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStatelessNoReentrant>(container_type);
                    var response_data = await grain.OnContainerRpc2(method_name, method_data);

                    //int response_len = response_data == null ? 0 : response_data.Length;

                    //Logger.LogInformation("ServiceClient.ClientContainerRpc() ContainerId={container_id} MethodName={method_name} RequestLen={len} ResponseLen={response_len}",
                    //    container_id, method_name, len, response_len);

                    return response_data;
                }
            case 3:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStatefulNoReentrant>(string.Format("{0}_{1}", container_type, container_id));
                    var response_data = await grain.OnContainerRpc2(method_name, method_data);

                    //int response_len = response_data == null ? 0 : response_data.Length;

                    //Logger.LogInformation("ServiceClient.ClientContainerRpc() ContainerId={container_id} MethodName={method_name} RequestLen={len} ResponseLen={response_len}",
                    //    container_id, method_name, len, response_len);

                    return response_data;
                }
        }

        return null;
    }

    public Task ForwardContainerRpcNoResult(
        string service_name,
        int containerstate_type,
        string container_type,
        string container_id,
        string method_name,
        byte[] method_data)
    {
        MapOrleansClient.TryGetValue(service_name, out var client);
        if (client == null)
        {
            Logger.LogError("ServiceClient.ForwardContainerRpcNoResult() Error，服务名{ServiceName}不存在！", service_name);

            return Task.CompletedTask;
        }

        if (client.Client == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        switch (containerstate_type)
        {
            case 0:// Stateless
                {
                    var grain = client.Client.GetGrain<IGrainContainerStateless>(container_type);
                    return grain.OnContainerRpcNoResult2(method_name, method_data);
                }
            case 1:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStateful>(string.Format("{0}_{1}", container_type, container_id));
                    return grain.OnContainerRpcNoResult2(method_name, method_data);
                }
            case 2:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStatelessNoReentrant>(container_type);
                    return grain.OnContainerRpcNoResult2(method_name, method_data);
                }
            case 3:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStatefulNoReentrant>(string.Format("{0}_{1}", container_type, container_id));
                    return grain.OnContainerRpcNoResult2(method_name, method_data);
                }
        }

        return Task.CompletedTask;
    }

    public async Task<byte[]> ForwardEntityRpc(
        string service_name,
        int containerstate_type,
        string container_type,
        string container_id,
        long entity_id,
        string component_name,
        string method_name,
        byte[] method_data)
    {
        MapOrleansClient.TryGetValue(service_name, out var client);
        if (client == null)
        {
            Logger.LogError("ServiceClient.ForwardEntityRpc() Error，服务名{ServiceName}不存在！", service_name);

            return null;
        }

        if (client.Client == null)
        {
            // todo，log error
            return null;
        }

        byte[] response_data = null;

        switch (containerstate_type)
        {
            case 0:// Stateless
                {
                }
                break;
            case 1:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStateful>(string.Format("{0}_{1}", container_type, container_id));
                    response_data = await grain.OnEntityRpc2(entity_id, component_name, method_name, method_data);
                }
                break;
            case 2:
                {
                }
                break;
            case 3:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStatefulNoReentrant>(string.Format("{0}_{1}", container_type, container_id));
                    response_data = await grain.OnEntityRpc2(entity_id, component_name, method_name, method_data);
                }
                break;
        }

        return response_data;
    }

    public Task ForwardEntityRpcNoResult(
        string service_name,
        int containerstate_type,
        string container_type,
        string container_id,
        long entity_id,
        string component_name,
        string method_name,
        byte[] method_data)
    {
        MapOrleansClient.TryGetValue(service_name, out var client);
        if (client == null)
        {
            Logger.LogError("ServiceClient.ForwardEntityRpcNoResult() Error，服务名{ServiceName}不存在！", service_name);

            return Task.CompletedTask;
        }

        if (client.Client == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        switch (containerstate_type)
        {
            case 0:// Stateless
                {
                }
                break;
            case 1:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStateful>(string.Format("{0}_{1}", container_type, container_id));
                    return grain.OnEntityRpcNoResult2(entity_id, component_name, method_name, method_data);
                }
            case 2:
                {
                }
                break;
            case 3:
                {
                    var grain = client.Client.GetGrain<IGrainContainerStatefulNoReentrant>(string.Format("{0}_{1}", container_type, container_id));
                    return grain.OnEntityRpcNoResult2(entity_id, component_name, method_name, method_data);
                }
        }

        return Task.CompletedTask;
    }

    //async Task OnServiceNodeChanged(string service_name, bool add_or_remove)
    //{
    //    Logger.LogInformation("OnServiceNodeChanged ServiceName={service_name}, AddOrRemove={add_or_remove}", service_name, add_or_remove);

    //    string s = service_name.ToLower();
    //    MapOrleansClient.TryGetValue(s, out var oc);
    //    if (oc != null)
    //    {
    //        if (add_or_remove)
    //        {
    //            await oc.Start();
    //        }
    //        else
    //        {
    //            await oc.Stop();
    //        }
    //    }
    //}
}