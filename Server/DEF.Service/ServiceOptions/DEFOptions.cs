using Serilog.Events;

namespace DEF;

public class DEFOptions
{
    public const string Key = "DEF";

    public string Env { get; set; } = "Pro";
    public string Cluster { get; set; } = "Cluster";
    public SerializerType SerializerType { get; set; } = SerializerType.MemoryPack;// Protobuf LitJson
    public string Timezone { get; set; } = "Asia/Shanghai";
    public string LocalIpPrefix { get; set; } = "192.168.";
    public string SecureUrl { get; set; } = "http://localhost:5005";
    public string ZkConnectString { get; set; } = "127.0.0.1:2181";
    public string RedisConnectString { get; set; } = "127.0.0.1:6379";
    public string RedisName { get; set; } = "1";
    public string MQConnectString { get; set; }
    public bool LogEnableOpenObserve { get; set; } = false;// 是否启用OpenObserve日志
    public string OpenObserveUrl { get; set; }
    public string OpenObserveLogin { get; set; }
    public string OpenObserveKey { get; set; }
    public string LogMongoDBConnectString { get; set; } = "mongodb://localhost:27017";
    public string LogMongoDBName { get; set; } = "Logs";
    public LogEventLevel LogMinimumLevel { get; set; } = LogEventLevel.Debug;
    public string DirAssets { get; set; } = "./";
    public int ResponseTimeoutWithDebugger { get; set; } = 6000;
}