using System.Reflection;
using Microsoft.Extensions.Logging;

namespace DEF;

public class Service : IService
{
    public Config Config { get; set; }
    public string ServiceName { get; set; }
    public ServiceState ServiceState { get; set; }
    Dictionary<string, Dictionary<string, MethodInfo>> MapContainerRpcInvoker { get; set; } = [];
    Dictionary<string, Dictionary<string, MethodInfo>> MapEntityRpcInvoker { get; set; } = [];
    Dictionary<string, ContainerRpcCallerFactory> MapContainerRpcCallerFactory { get; set; } = [];
    Dictionary<string, ContainerRpcObserverCallerFactory> MapContainerObserverRpcCallerFactory { get; set; } = [];
    Dictionary<string, ComponentRpcCallerFactory> MapComponentRpcCallerFactory { get; set; } = [];
    Dictionary<string, ComponentRpcObserverCallerFactory> MapEntityObserverRpcCallerFactory { get; set; } = [];
    Dictionary<string, ComponentStateFactory> MapEntityStateFactory { get; set; } = [];
    Dictionary<string, ComponentStateFactory> MapEntityStateFactory2 { get; set; } = [];
    Dictionary<string, ComponentFactory> MapComponentFactory { get; set; } = [];
    Dictionary<string, ContainerStatefulFactory> MapContainerStatefulFactory { get; set; } = [];
    Dictionary<string, ContainerStatelessFactory> MapContainerStatelessFactory { get; set; } = [];
    Dictionary<string, ContainerStatefulNoReentrantFactory> MapContainerStatefulNoReentrantFactory { get; set; } = [];
    Dictionary<string, ContainerStatelessNoReentrantFactory> MapContainerStatelessNoReentrantFactory { get; set; } = [];
    ILogger Logger { get; set; }
    IServiceProvider ServiceProvider { get; set; }
    IGrainFactory GrainFactory { get; set; }
    Rpcer4Service Rpcer { get; set; }
    ServicePubSub ServicePubSub { get; set; }

    public Service(ILogger<Service> logger,
        IServiceProvider service_provider,
        Rpcer4Service rpcer,
        ServicePubSub service_pubsub)
    {
        Logger = logger;
        ServiceProvider = service_provider;
        Rpcer = rpcer;
        ServicePubSub = service_pubsub;

        GrainFactory = (IGrainFactory)ServiceProvider?.GetService(typeof(IGrainFactory));

        Config = new Config();
    }

    public void Setup(string service_name, params Assembly[] assemblys)
    {
        ServiceName = service_name;

        List<Assembly> list_ass = [];
        if (assemblys != null)
        {
            list_ass.AddRange(assemblys);
        }

        foreach (var i in list_ass)
        {
            if (i == null) continue;

            Type[] arr_type = i.GetTypes();

            foreach (Type t in arr_type)
            {
                // 注册ComponentStateFactory
                if (t.BaseType != null && t.BaseType.FullName == typeof(ComponentStateFactory).FullName)
                {
                    RegisterEntityStateFactory(t);
                }

                // 注册ContainerRpcCaller
                if (t.BaseType != null && t.BaseType.FullName == typeof(ContainerRpcCallerFactory).FullName)
                {
                    RegisterContainerRpcCaller(t);
                }

                // 注册ContainerObserverRpcCaller
                if (t.BaseType != null && t.BaseType.FullName == typeof(ContainerRpcObserverCallerFactory).FullName)
                {
                    RegisterContainerObserverRpcCaller(t);
                }

                // 注册ComponentRpcCaller
                if (t.BaseType != null && t.BaseType.FullName == typeof(ComponentRpcCallerFactory).FullName)
                {
                    RegisterEntityRpcCaller(t);
                }

                // 注册ComponentObserverRpcCaller
                if (t.BaseType != null && t.BaseType.FullName == typeof(ComponentRpcObserverCallerFactory).FullName)
                {
                    RegisterEntityObserverRpcCaller(t);
                }

                // 注册ContainerStatefulFactory
                if (t.BaseType != null && t.BaseType.FullName == typeof(ContainerStateful).FullName)
                {
                    RegisterContainerStatefulFactory(t);
                }

                // 注册ContainerStatelessFactory
                if (t.BaseType != null && t.BaseType.FullName == typeof(ContainerStateless).FullName)
                {
                    RegisterContainerStatelessFactory(t);
                }

                // 注册ContainerStatefulNoReentrantFactory
                if (t.BaseType != null && t.BaseType.FullName == typeof(ContainerStatefulNoReentrant).FullName)
                {
                    RegisterContainerStatefulNoReentrantFactory(t);
                }

                // 注册ContainerStatelessNoReentrantFactory
                if (t.BaseType != null && t.BaseType.FullName == typeof(ContainerStatelessNoReentrant).FullName)
                {
                    RegisterContainerStatelessNoReentrantFactory(t);
                }

                // 注册ContainerRpcInvoker
                var att_containerrpc = t.GetCustomAttribute<ContainerRpcAttribute>();
                if (att_containerrpc != null && att_containerrpc.ServiceName == ServiceName)
                {
                    RegisterContainerRpcInvoker(t);
                }

                // 注册ComponentRpcInvoker
                var att_entityrpc = t.GetCustomAttribute<ComponentRpcAttribute>();
                if (att_entityrpc != null && att_entityrpc.ServiceName == ServiceName)
                {
                    RegisterEntityRpcInvoker(t);
                }

                // 注册ComponentFactory
                var att_component_impl = t.GetCustomAttribute<ComponentImplAttribute>();
                if (att_component_impl != null)
                {
                    RegisterComponentFactory(t);
                }
            }
        }
    }

    void RegisterEntityStateFactory(Type t)
    {
        dynamic factory = Activator.CreateInstance(t);
        string s1 = factory.GetName();
        if (s1 == string.Empty)
        {
            Logger?.LogError($"RegisterEntityStateFactory Error! FactoryName={factory.GetName2()}");
        }
        MapEntityStateFactory[factory.GetName()] = factory;
        MapEntityStateFactory2[factory.GetName2()] = factory;
    }

    void RegisterContainerRpcCaller(Type t)
    {
        dynamic factory = Activator.CreateInstance(t);
        string s1 = factory.GetName();
        if (string.IsNullOrEmpty(s1))
        {
            Logger?.LogError("RegisterContainerRpcCallerFactory Error!");
        }

        MapContainerRpcCallerFactory[s1] = factory;
    }

    void RegisterContainerObserverRpcCaller(Type t)
    {
        dynamic factory = Activator.CreateInstance(t);
        string s1 = factory.GetName();
        if (string.IsNullOrEmpty(s1))
        {
            Logger?.LogError("RegisterContainerRpcObserverCallerFactory Error!");
        }

        MapContainerObserverRpcCallerFactory[s1] = factory;
    }

    void RegisterEntityRpcCaller(Type t)
    {
        dynamic factory = Activator.CreateInstance(t);
        string s1 = factory.GetName();
        if (string.IsNullOrEmpty(s1))
        {
            Logger?.LogError("RegisterEntityRpcCallerFactory Error!");
        }

        MapComponentRpcCallerFactory[s1] = factory;
    }

    void RegisterEntityObserverRpcCaller(Type t)
    {
        dynamic factory = Activator.CreateInstance(t);
        string s1 = factory.GetName();
        if (string.IsNullOrEmpty(s1))
        {
            Logger?.LogError("RegisterEntityRpcObserverCallerFactory Error!");
        }

        MapEntityObserverRpcCallerFactory[s1] = factory;
    }

    void RegisterContainerRpcInvoker(Type t)
    {
        Dictionary<string, MethodInfo> map = [];
        var arr_method = t.GetMethods();
        if (arr_method != null)
        {
            foreach (var m in arr_method)
            {
                map[m.Name] = m;
            }
        }

        MapContainerRpcInvoker[t.Name] = map;
    }

    void RegisterEntityRpcInvoker(Type t)
    {
        Dictionary<string, MethodInfo> map = [];
        var arr_method = t.GetMethods();
        if (arr_method != null)
        {
            foreach (var m in arr_method)
            {
                map[m.Name] = m;
            }
        }

        MapEntityRpcInvoker[t.Name] = map;
    }

    void RegisterComponentFactory(Type t)
    {
        var t1 = typeof(ComponentFactory<>);
        var t2 = t1.MakeGenericType(t);
        dynamic factory = Activator.CreateInstance(t2);

        MapComponentFactory[factory.GetName()] = factory;
    }

    void RegisterContainerStatefulFactory(Type t)
    {
        var interfaces = t.GetInterfaces();
        if (interfaces.Length == 0)
        {
            // log error
            return;
        }

        string container_type = string.Empty;
        foreach (var i in interfaces)
        {
            var attrs = i.GetCustomAttributes();
            if (!attrs.Any()) continue;
            foreach (var attr in attrs)
            {
                if (attr is ContainerRpcAttribute attr_containerrpc)
                {
                    container_type = attr_containerrpc.ContainerType;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(container_type))
        {
            // log error
            return;
        }

        var t1 = typeof(ContainerStatefulFactory<>);
        var t2 = t1.MakeGenericType(t);
        dynamic factory = Activator.CreateInstance(t2);

        MapContainerStatefulFactory[container_type] = factory;
    }

    void RegisterContainerStatelessFactory(Type t)
    {
        var interfaces = t.GetInterfaces();
        if (interfaces.Length == 0)
        {
            // log error
            return;
        }

        string container_type = string.Empty;
        foreach (var i in interfaces)
        {
            var attrs = i.GetCustomAttributes();
            if (!attrs.Any()) continue;
            foreach (var attr in attrs)
            {
                if (attr is ContainerRpcAttribute attr_containerrpc)
                {
                    container_type = attr_containerrpc.ContainerType;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(container_type))
        {
            // log error
            return;
        }

        var t1 = typeof(ContainerStatelessFactory<>);
        var t2 = t1.MakeGenericType(t);
        dynamic factory = Activator.CreateInstance(t2);

        MapContainerStatelessFactory[container_type] = factory;
    }

    void RegisterContainerStatefulNoReentrantFactory(Type t)
    {
        var interfaces = t.GetInterfaces();
        if (interfaces.Length == 0)
        {
            // log error
            return;
        }

        string container_type = string.Empty;
        foreach (var i in interfaces)
        {
            var attrs = i.GetCustomAttributes();
            if (!attrs.Any()) continue;
            foreach (var attr in attrs)
            {
                if (attr is ContainerRpcAttribute attr_containerrpc)
                {
                    container_type = attr_containerrpc.ContainerType;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(container_type))
        {
            // log error
            return;
        }

        var t1 = typeof(ContainerStatefulNoReentrantFactory<>);
        var t2 = t1.MakeGenericType(t);
        dynamic factory = Activator.CreateInstance(t2);

        MapContainerStatefulNoReentrantFactory[container_type] = factory;
    }

    void RegisterContainerStatelessNoReentrantFactory(Type t)
    {
        var interfaces = t.GetInterfaces();
        if (interfaces.Length == 0)
        {
            // log error
            return;
        }

        string container_type = string.Empty;
        foreach (var i in interfaces)
        {
            var attrs = i.GetCustomAttributes();
            if (!attrs.Any()) continue;
            foreach (var attr in attrs)
            {
                if (attr is ContainerRpcAttribute attr_containerrpc)
                {
                    container_type = attr_containerrpc.ContainerType;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(container_type))
        {
            // log error
            return;
        }

        var t1 = typeof(ContainerStatelessNoReentrantFactory<>);
        var t2 = t1.MakeGenericType(t);
        dynamic factory = Activator.CreateInstance(t2);

        MapContainerStatelessNoReentrantFactory[container_type] = factory;
    }

    Dictionary<string, MethodInfo> IService.GetContainerRpcCallee(string name)
    {
        MapContainerRpcInvoker.TryGetValue(name, out var map);
        return map;
    }

    Dictionary<string, MethodInfo> IService.GetEntityRpcCallee(string name)
    {
        MapEntityRpcInvoker.TryGetValue(name, out var map);
        return map;
    }

    T IService.GetContainerRpc<T>()
    {
        return ((IService)this).GetContainerRpc<T>(string.Empty);
    }

    T IService.GetContainerRpc<T>(string container_id)
    {
        var t = typeof(T);

        MapContainerRpcCallerFactory.TryGetValue(t.Name, out var factory);
        if (factory == null)
        {
            return default;
        }

        var ca = factory.GetContainerRpcAttribute();

        var rpcinfo = new RpcInfo4Service()
        {
            GrainFactory = GrainFactory,
            SourceServiceName = ServiceName,
            TargetServiceName = ca.ServiceName,
            ContainerOrEntity = true,
            ContainerStateType = ca.ContainerStateType,
            ContainerType = ca.ContainerType,
            ContainerId = (ca.ContainerStateType == ContainerStateType.Stateful) || (ca.ContainerStateType == ContainerStateType.StatefulNoReentrant) ? container_id : string.Empty,
            EntityId = 0,
            ComponentName = string.Empty,
            IsObserver = false,
            ObserverGatewayGuid = string.Empty,
            ObserverSessionGuid = string.Empty,
        };

        if (rpcinfo.SourceServiceName != rpcinfo.TargetServiceName)
        {
            var service_client = ServiceProvider?.GetService(typeof(ServiceClient));
            if (service_client != null)
            {
                rpcinfo.Client = ((ServiceClient)service_client).GetOrleansClient(rpcinfo.TargetServiceName);
            }
        }

        return (T)factory.CreateContainerRpc(rpcinfo, Rpcer);
    }

    T IService.GetContainerObserverRpc<T>(string gateway_guid, string session_guid)
    {
        var t = typeof(T);

        MapContainerObserverRpcCallerFactory.TryGetValue(t.Name, out var factory);
        if (factory == null)
        {
            return default;
        }

        var ca = factory.GetContainerRpcObserverAttribute();

        var rpcinfo = new RpcInfo4Service()
        {
            GrainFactory = GrainFactory,
            IsUnity = false,
            SourceServiceName = ServiceName,
            TargetServiceName = string.Empty,
            ContainerOrEntity = true,
            ContainerStateType = ContainerStateType.Stateless,
            ContainerType = ca.ContainerType,
            ContainerId = string.Empty,
            EntityId = 0,
            ComponentName = string.Empty,
            IsObserver = true,
            ObserverGatewayGuid = gateway_guid,
            ObserverSessionGuid = session_guid,
        };

        return (T)factory.CreateContainerRpcObserver(rpcinfo, Rpcer);
    }

    T IService.GetEntityRpc<T>(string container_type, string container_id, long entity_id, string component_name)
    {
        var t = typeof(T);

        MapComponentRpcCallerFactory.TryGetValue(t.Name, out var factory);
        if (factory == null)
        {
            return default;
        }

        IRpcInfo rpcinfo = new RpcInfo4Service()
        {
            GrainFactory = GrainFactory,
            IsUnity = false,
            SourceServiceName = ServiceName,
            ContainerOrEntity = false,
            ContainerStateType = ContainerStateType.Stateful,
            ContainerType = container_type,
            ContainerId = container_id,
            EntityId = entity_id,
            ComponentName = component_name,
        };

        return (T)factory.CreateComponentRpc(rpcinfo, Rpcer);
    }

    T IService.GetEntityObserverRpc<T>(string container_type, string container_id, long entity_id, string component_name, string gateway_guid, string session_guid)
    {
        var t = typeof(T);

        MapEntityObserverRpcCallerFactory.TryGetValue(t.Name, out var factory);
        if (factory == null)
        {
            return default;
        }

        var rpcinfo = new RpcInfo4Service()
        {
            GrainFactory = GrainFactory,
            IsUnity = false,
            SourceServiceName = ServiceName,
            TargetServiceName = "",
            ContainerOrEntity = true,
            ContainerStateType = ContainerStateType.Stateful,
            ContainerType = container_type,
            ContainerId = container_id,
            EntityId = entity_id,
            ComponentName = component_name,

            IsObserver = true,
            ObserverGatewayGuid = gateway_guid,
            ObserverSessionGuid = session_guid,
        };

        return (T)factory.CreateComponentRpcObserver(rpcinfo, Rpcer);
    }

    public ComponentFactory GetComponentFactory(string name)
    {
        MapComponentFactory.TryGetValue(name, out ComponentFactory factory);
        return factory;
    }

    Task IService.StreamPublish2AllServiceNodes(string id, string data)
    {
        return ServicePubSub.Publish2AllServiceNodes(id, data);
    }

    public ComponentStateFactory GetEntityStateFactory(string name)
    {
        MapEntityStateFactory.TryGetValue(name, out ComponentStateFactory factory);
        return factory;
    }

    public ComponentStateFactory GetEntityStateFactory2(string name)
    {
        MapEntityStateFactory2.TryGetValue(name, out ComponentStateFactory factory);
        return factory;
    }

    public ContainerStateful CreateContainerStateful(ILogger logger, GrainContainerStateful grain, IGrainFactory grain_factory, string container_type, string container_id)
    {
        MapContainerStatefulFactory.TryGetValue(container_type, out var c);

        if (c == null)
        {
            return null;
        }

        return c.Create(logger, this, grain, grain_factory, container_type, container_id);
    }

    public ContainerStateless CreateContainerStateless(ILogger logger, GrainContainerStateless grain, IGrainFactory grain_factory, string container_type)
    {
        MapContainerStatelessFactory.TryGetValue(container_type, out var c);

        if (c == null)
        {
            return null;
        }

        return c.Create(logger, this, grain, grain_factory, container_type);
    }

    public ContainerStatefulNoReentrant CreateContainerStatefulNoReentrant(ILogger logger, GrainContainerStatefulNoReentrant grain, IGrainFactory grain_factory, string container_type, string container_id)
    {
        MapContainerStatefulNoReentrantFactory.TryGetValue(container_type, out var c);

        if (c == null)
        {
            return null;
        }

        return c.Create(logger, this, grain, grain_factory, container_type, container_id);
    }

    public ContainerStatelessNoReentrant CreateContainerStatelessNoReentrant(ILogger logger, GrainContainerStatelessNoReentrant grain, IGrainFactory grain_factory, string container_type)
    {
        MapContainerStatelessNoReentrantFactory.TryGetValue(container_type, out var c);

        if (c == null)
        {
            return null;
        }

        return c.Create(logger, this, grain, grain_factory, container_type);
    }
}