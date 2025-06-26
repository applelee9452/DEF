using System;

namespace DEF
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ComponentRequiredAttribute : Attribute
    {
        public Type[] RequiredComponents { get; set; }

        public ComponentRequiredAttribute(params Type[] required_components)
        {
            RequiredComponents = required_components;

            if (RequiredComponents == null || RequiredComponents.Length == 0)
            {
                return;
            }

            foreach (var required_component_type in RequiredComponents)
            {
                if (!required_component_type.IsSubclassOf(typeof(Component)))
                {
                    // todo，log error
                    throw new ArgumentException($"Required component type: {required_component_type.Name} must be a subclass of Component.");
                }
            }
        }
    }
}