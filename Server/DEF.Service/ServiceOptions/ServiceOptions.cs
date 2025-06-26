namespace DEF;

public class ServiceOptions
{
    public const string Key = "Service";

    public string ServiceName { get; set; } = string.Empty;
    public int OrleansGatewayPort { get; set; } = 10061;
    public int OrleansSiloPort { get; set; } = 10062;
    public string MongoDBConnectString { get; set; }
    public string MongoDBName { get; set; }
}