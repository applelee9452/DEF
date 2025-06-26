

namespace DEF
{
    public abstract class ContainerRpcCallerFactory
    {
        public abstract string GetName();

        public abstract ContainerRpcAttribute GetContainerRpcAttribute();

        public abstract IContainerRpc CreateContainerRpc(IRpcInfo rpcinfo, IRpcer rpcer);
    }
}