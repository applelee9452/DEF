using System;

namespace DEF
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterContainerRpcObserverInvokeHelperAttribute : Attribute
    {
        public string ContainerType;

        public RegisterContainerRpcObserverInvokeHelperAttribute(string container_type)
        {
            ContainerType = container_type;
        }
    }
}