using System;

namespace DEF
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ComponentAttribute : Attribute
    {
        public string ComponentName;

        public ComponentAttribute(string component_name)
        {
            ComponentName = component_name;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentImplAttribute : Attribute
    {
        public ComponentImplAttribute()
        {
        }
    }
}