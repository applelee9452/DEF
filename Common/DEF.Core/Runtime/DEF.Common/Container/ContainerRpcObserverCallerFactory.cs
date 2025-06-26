

namespace DEF
{
    public abstract class ContainerRpcObserverCallerFactory
    {
        public abstract string GetName();

        public abstract ContainerRpcObserverAttribute GetContainerRpcObserverAttribute();

        public abstract IContainerRpcObserver CreateContainerRpcObserver(IRpcInfo rpcinfo, IRpcer rpcer);
    }
}