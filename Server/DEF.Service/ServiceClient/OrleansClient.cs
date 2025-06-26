using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using StackExchange.Redis;

namespace DEF;

public class OrleansClient : IGrainServiceClientObserver
{
    public IClusterClient Client { get; set; }
    public bool ExistStateful { get; set; }
    ServiceClient ServiceClient { get; set; }
    ILogger Logger { get; set; }
    IOptions<DEFOptions> DEFOptions { get; set; }
    string ServiceName { get; set; }
    int OrleansGatewayPort { get; set; }
    IServiceClientObserverListener ObserverListener { get; set; }
    IHost ClientHost { get; set; }

    public OrleansClient(ILogger logger,
        IOptions<DEFOptions> def_options,
        ServiceClient service_client,
        string service_name,
        int orleans_gateway_port,
        bool exist_stateful,
        IServiceClientObserverListener listener)
    {
        Logger = logger;
        DEFOptions = def_options;
        ServiceClient = service_client;
        ServiceName = service_name;
        OrleansGatewayPort = orleans_gateway_port;
        ExistStateful = exist_stateful;
        ObserverListener = listener;
    }

    public async Task Start()
    {
        //var gateways = Enumerable.Range(OrleansGatewayPort, 1).Select(i => new IPEndPoint(IPAddress.Loopback, i)).ToArray();
        //var gateways = new int[] { 30000, 30001 };

        try
        {
            ClientHost = Host.CreateDefaultBuilder(null)
                .UseOrleansClient(client_builder =>
                {
                    client_builder
                        .UseConnectionRetryFilter(RetryFilter)
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = ServiceName;
                            options.ServiceId = DEFOptions.Value.Cluster + DEFOptions.Value.Env;
                        })
                        .Configure<ClientMessagingOptions>(options =>
                        {
                            options.ResponseTimeout = TimeSpan.FromSeconds(10);
                            options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(DEFOptions.Value.ResponseTimeoutWithDebugger);
                        })
                        .Configure<SiloMessagingOptions>(options =>
                        {
                            options.ResponseTimeout = TimeSpan.FromSeconds(10);
                            options.ResponseTimeoutWithDebugger = TimeSpan.FromSeconds(DEFOptions.Value.ResponseTimeoutWithDebugger);
                        })
                        .AddClusterConnectionLostHandler(ConnectionToClusterLostHandler)
                        .AddMemoryStreams("StreamProvider")
                        //.AddClusterConnectionStatusObserver
                        //.AddGatewayCountChangedHandler((sender, e) =>
                        //{
                        //})
                        //.AddClusterConnectionLostHandler((sender, e) =>
                        //{
                        //})
                        //.UseStaticClustering(gateways)
                        //.UseZooKeeperClustering(options =>
                        //{
                        //    options.ConnectionString = DEFOptions.Value.ZkConnectString;
                        //})
                        .UseRedisClustering(options =>
                        {
                            options.ConfigurationOptions = ConfigurationOptions.Parse(DEFOptions.Value.RedisConnectString);
                        });
                })
                .Build();

            Logger.LogInformation("OrleansClient.Start() 开始连接{ServiceName}", ServiceName);

            await ClientHost.StartAsync();

            Client = ClientHost.Services.GetRequiredService<IClusterClient>();

            if (ExistStateful)
            {
                var grain = Client.GetGrain<IGrainServiceClient>(ServiceClient.Id);

                var reference = Client.CreateObjectReference<IGrainServiceClientObserver>(this);
                await grain.Sub(reference);
            }

            Logger.LogInformation("OrleansClient.Start() 完成连接{ServiceName}", ServiceName);
        }
        catch (Exception e)
        {
            Logger.LogError("OrleansClient.Start() {Exception}", e.ToString());
        }
    }

    public async Task Stop()
    {
        if (Client == null) return;

        try
        {
            if (ExistStateful)
            {
                var grain = Client.GetGrain<IGrainServiceClient>(ServiceClient.Id);
                await grain.Unsub(this);
            }

            await ClientHost.StopAsync();

            ClientHost.Dispose();
            Client = null;
        }
        catch (Exception e)
        {
            Logger.LogError("OrleansClient.Stop() {Exception}", e.ToString());
        }
    }

    async Task<bool> RetryFilter(Exception exception, CancellationToken cancellation_token)
    {
        string ex = exception.ToString();
        Logger.LogInformation("OrleansClient.RetryFilter Exception: {Exception}", ex);

        await Task.Delay(TimeSpan.FromSeconds(3), cancellation_token);

        return true;
    }

    void ConnectionToClusterLostHandler(object sender, EventArgs e)
    {
        string ex = e.ToString();

        Logger.LogInformation("OrleansClient.ConnectionToClusterLostHandler Exception: {Exception}", ex);

        ClientHost?.Dispose();
        ClientHost = null;
        Client = null;
    }

    Task IGrainServiceClientObserver.NotifySession(ObserverInfo observer_info, string session_guid,
        string method_name)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name);
    }

    Task IGrainServiceClientObserver.NotifySession<T1>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1);
    }

    Task IGrainServiceClientObserver.NotifySession<T1, T2>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1, obj2);
    }

    Task IGrainServiceClientObserver.NotifySession<T1, T2, T3>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1, obj2, obj3);
    }

    Task IGrainServiceClientObserver.NotifySession<T1, T2, T3, T4>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1, obj2, obj3, obj4);
    }

    Task IGrainServiceClientObserver.NotifySession<T1, T2, T3, T4, T5>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1, obj2, obj3, obj4, obj5);
    }

    Task IGrainServiceClientObserver.NotifySession<T1, T2, T3, T4, T5, T6>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
    }

    Task IGrainServiceClientObserver.NotifySession<T1, T2, T3, T4, T5, T6, T7>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
    }

    Task IGrainServiceClientObserver.NotifySession<T1, T2, T3, T4, T5, T6, T7, T8>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
    }

    Task IGrainServiceClientObserver.NotifySession<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9)
    {
        return ObserverListener.NotifySession(observer_info, session_guid,
            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
    }

    Task IGrainServiceClientObserver.DisConnectSession(string session_guid, string reason)
    {
        Logger.LogInformation("接收到OrleansHost的主动断开指定连接消息，SessionGuid={SessionGuid}，Reason={Reason}",
            session_guid, reason);

        return ObserverListener.DisConnectSession(session_guid, reason);
    }
}