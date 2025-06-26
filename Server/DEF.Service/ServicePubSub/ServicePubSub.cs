using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace DEF;

public class ServicePubSubEvent
{
    public string Id;
    public string Data;
}

public class ServicePubSub
{
    IServiceProvider ServiceProvider { get; set; }
    IOptions<DEFOptions> DEFOptions { get; set; }
    IOptions<ServiceOptions> ServiceOptions { get; set; }
    IOptions<ServiceClientOptions> ServiceClientOptions { get; set; }
    ILogger Logger { get; set; }
    string ChannelNameAllServiceNodes { get; set; } = string.Empty;
    ISubscriber SubscriberAllServiceNodes { get; set; } = null;
    DbClientRedis DbClientRedis { get; set; }
    IServiceListener ServiceListener { get; set; }// 可能为空

    public ServicePubSub(
        IServiceProvider service_provider,
        ILogger<ServicePubSub> logger,
        IOptions<DEFOptions> def_options,
        IOptions<ServiceOptions> service_options,
        IOptions<ServiceClientOptions> serviceclient_options)
    {
        ServiceProvider = service_provider;
        Logger = logger;
        DEFOptions = def_options;
        ServiceOptions = service_options;
        ServiceClientOptions = serviceclient_options;

        if (ServiceOptions.Value != null)
        {
            ChannelNameAllServiceNodes = DEFOptions.Value.Cluster + DEFOptions.Value.Env + "_" + ServiceOptions.Value.ServiceName;
        }

        DbClientRedis = new(DEFOptions.Value.RedisName, DEFOptions.Value.RedisConnectString, DEFOptions.Value.Timezone);
    }

    public async Task StartAsync()
    {
        ServiceListener = (IServiceListener)ServiceProvider.GetService(typeof(IServiceListener));

        if (!string.IsNullOrEmpty(ChannelNameAllServiceNodes))
        {
            SubscriberAllServiceNodes = DbClientRedis.CM.GetSubscriber();
            await SubscriberAllServiceNodes.SubscribeAsync(RedisChannel.Literal(ChannelNameAllServiceNodes), (RedisChannel c, RedisValue data) =>
            {
                var ev = Newtonsoft.Json.JsonConvert.DeserializeObject<ServicePubSubEvent>((string)data);

                ServiceListener?.OnStreamRecvedServiceNodeEvent(ev.Id, ev.Data);
            });
        }
    }

    public Task StopAsync()
    {
        if (SubscriberAllServiceNodes != null)
        {
            SubscriberAllServiceNodes.Unsubscribe(RedisChannel.Literal(ChannelNameAllServiceNodes));
            SubscriberAllServiceNodes = null;
        }

        return Task.CompletedTask;
    }

    public Task Publish2AllServiceNodes(string id, string data)
    {
        if (string.IsNullOrEmpty(ChannelNameAllServiceNodes))
        {
            Logger.LogError("ChannelNameAllServiceNodes为空，不支持Publish2AllServiceNodes！");

            return Task.CompletedTask;
        }

        ServicePubSubEvent ev = new()
        {
            Id = id,
            Data = data,
        };
        var s = Newtonsoft.Json.JsonConvert.SerializeObject(ev);

        return DbClientRedis.DB.PublishAsync(RedisChannel.Literal(ChannelNameAllServiceNodes), s);
    }

    //public async Task Test()
    //{
    //    ISubscriber sub = DbClientRedis.CM.GetSubscriber();
    //    await sub.SubscribeAsync(ChannelNameAllServiceNodes, (RedisChannel c, RedisValue data) =>
    //    {
    //        Console.WriteLine(data);

    //        ServiceListener?.OnStreamRecvedServiceNodeEvent(data);
    //    });

    //    await DbClientRedis.DB.PublishAsync(ChannelNameAllServiceNodes, "asdfasdf");
    //}
}