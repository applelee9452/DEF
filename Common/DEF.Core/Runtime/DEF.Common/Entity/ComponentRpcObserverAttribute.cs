using System;

namespace DEF
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ComponentRpcObserverAttribute : Attribute
    {
        public string ComponentName;

        public ComponentRpcObserverAttribute(string component_name)
        {
            ComponentName = component_name;
        }
    }
}