namespace DEF;

public class ServiceDiscoverListenService
{
    public string ServiceName { get; set; } = string.Empty;
    public int OrleansGatewayPort { get; set; }
    public string AssemblyInterfacePath { get; set; } = string.Empty;
    public string AssemblyInterfaceName { get; set; } = string.Empty;
    public bool IsStateful { get; set; } = false;
}

public class ServiceClientOptions
{
    public const string Key = "ServiceClient";

    public List<ServiceDiscoverListenService> ServiceDiscoverListenServices { get; set; }
}