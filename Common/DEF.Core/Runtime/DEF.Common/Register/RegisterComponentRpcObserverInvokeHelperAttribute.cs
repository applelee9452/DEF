using System;

namespace DEF
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RegisterComponentRpcObserverInvokeHelperAttribute : Attribute
    {
        public string ComponentName;

        public RegisterComponentRpcObserverInvokeHelperAttribute(string component_name)
        {
            ComponentName = component_name;
        }
    }
}