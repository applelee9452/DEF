using System;

namespace DEF
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ContainerRpcAttribute : Attribute
    {
        public string ServiceName;
        public string ContainerType;
        public ContainerStateType ContainerStateType;
        
        public ContainerRpcAttribute(string service_name, string container_type, ContainerStateType containerstate_type)
        {
            ServiceName = service_name;
            ContainerType = container_type;
            ContainerStateType = containerstate_type;
        }
    }
}