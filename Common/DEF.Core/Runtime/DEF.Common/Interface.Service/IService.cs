#if !DEF_CLIENT

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace DEF;

public enum ServiceState
{
    Stopped = 0,// 已停止
    Starting,// 启动中
    Running,// 运行中
    Stopping// 停止中
}

public partial interface IService
{
    Config Config { get; set; }
    string ServiceName { get; set; }
    ServiceState ServiceState { get; set; }

    Dictionary<string, MethodInfo> GetContainerRpcCallee(string name);

    Dictionary<string, MethodInfo> GetEntityRpcCallee(string name);

    T GetContainerRpc<T>() where T : IContainerRpc;

    T GetContainerRpc<T>(string container_id) where T : IContainerRpc;

    T GetContainerObserverRpc<T>(string gateway_guid, string session_guid) where T : IContainerRpcObserver;

    T GetEntityRpc<T>(string container_type, string container_id, long entity_id, string component_name) where T : IComponentRpc;

    T GetEntityObserverRpc<T>(string container_type, string container_id, long entity_id, string component_name, string gateway_guid, string session_guid) where T : IComponentRpcObserver;

    ComponentStateFactory GetEntityStateFactory(string name);

    ComponentStateFactory GetEntityStateFactory2(string name);

    ComponentFactory GetComponentFactory(string name);

    Task StreamPublish2AllServiceNodes(string id, string data);
}

#endif