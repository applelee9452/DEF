

using System;

namespace DEF
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ContainerRpcObserverAttribute : Attribute
    {
        public string ContainerType;

        public ContainerRpcObserverAttribute(string container_type)
        {
            ContainerType = container_type;
        }
    }
}
