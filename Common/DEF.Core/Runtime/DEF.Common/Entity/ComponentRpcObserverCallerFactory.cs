namespace DEF
{
    public abstract class ComponentRpcObserverCallerFactory
    {
        public abstract string GetName();

        public abstract ComponentRpcObserverAttribute GetComponentRpcObserverAttribute();

        public abstract IComponentRpcObserver CreateComponentRpcObserver(IRpcInfo rpcinfo, IRpcer rpcer);
    }
}