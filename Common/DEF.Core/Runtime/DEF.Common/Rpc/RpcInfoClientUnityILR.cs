namespace DEF
{
    public class RpcInfoClientUnityILR : IRpcInfo
    {
        public bool IsUnity { get; set; }
        public string SourceServiceName { get; set; }
        public string TargetServiceName { get; set; }
        public bool ContainerOrEntity { get; set; }
        public ContainerStateType ContainerStateType { get; set; }
        public string ContainerType { get; set; }
        public string ContainerId { get; set; }
        public long EntityId { get; set; }
        public string ComponentName { get; set; }
    }
}