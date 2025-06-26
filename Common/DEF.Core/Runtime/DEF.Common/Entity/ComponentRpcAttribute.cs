using System;

namespace DEF
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ComponentRpcAttribute : Attribute
    {
        public string ServiceName;
        public ContainerStateType ContainerStateType;

        public ComponentRpcAttribute(string service_name, ContainerStateType containerstate_type)
        {
            ServiceName = service_name;
            ContainerStateType = containerstate_type;
        }
    }
}