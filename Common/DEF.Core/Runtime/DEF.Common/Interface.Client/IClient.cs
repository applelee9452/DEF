using System.Collections.Generic;

namespace DEF
{
    public interface IClient
    {
        Config Config { get; set; }

        Dictionary<string, Scene> MapScene { get; set; }

        EventContext EventContext { get; set; }

        void Close();

        void RegComponentHookFactory<T>(IComponentHookFactory<T> factory) where T : Component;

        IComponentHookFactory<T> TryGetComponentHookFactory<T>() where T : Component;

        T GetContainerRpc<T>(string container_id) where T : IContainerRpc;

        T GetEntityRpc<T>(string container_type, string container_id, long entity_id, string component_name) where T : IComponentRpc;

        ComponentStateFactory GetEntityStateFactory(string name);

        ComponentStateFactory GetEntityStateFactory2(string name);

        ComponentFactory GetComponentFactory(string name);

        void DispatchContainerObserverRpc(string container_type, string method_name, byte[] method_data);

        void DispatchEntityObserverRpc(string container_type, string container_id, long entity_id, string component_name, string method_name, byte[] method_data);
    }
}