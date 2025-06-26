namespace DEF
{
    public abstract class ComponentRpcCallerFactory
    {
        public abstract string GetName();

        public abstract ComponentRpcAttribute GetComponentRpcAttribute();

        public abstract IComponentRpc CreateComponentRpc(IRpcInfo rpcinfo, IRpcer rpcer);
    }
}