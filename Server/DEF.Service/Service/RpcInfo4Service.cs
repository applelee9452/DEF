namespace DEF;

public struct RpcInfo4Service : IRpcInfo
{
    public bool IsUnity { get; set; } = false;
    public string SourceServiceName { get; set; }
    public string TargetServiceName { get; set; }
    public IClusterClient Client { get; set; }
    public IGrainFactory GrainFactory { get; set; }
    public bool ContainerOrEntity { get; set; }
    public bool Reentrant { get; set; }
    public ContainerStateType ContainerStateType { get; set; }
    public string ContainerType { get; set; }
    public string ContainerId { get; set; }
    public long EntityId { get; set; }
    public string ComponentName { get; set; }
    public bool IsObserver { get; set; } = false;
    public string ObserverGatewayGuid { get; set; } = string.Empty;
    public string ObserverSessionGuid { get; set; } = string.Empty;

    public RpcInfo4Service()
    {
    }
}