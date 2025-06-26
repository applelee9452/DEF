#if DEF_CLIENT

using DEF.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DEF
{
    public class ContainerObserverRpcInfo
    {
        public Dictionary<string, Func<int, IContainerRpcObserver, byte[], Task>> MapMethod { get; set; }
        public IContainerRpcObserver Instance { get; set; }
    }

    public class EntityObserverRpcInfo
    {
        public Dictionary<string, Func<int, IComponentRpcObserver, byte[], Task>> MapMethod { get; set; }
    }

    public partial class ClientUnity : IClient
    {
        public static ClientUnity Instance { get; private set; }
        public Config Config { get; set; }
        public Dictionary<string, Scene> MapScene { get; set; } = new();// Key=ContainerType
        public EventContext EventContext { get; set; }
        public RpcHttpClient RpcHttpClient { get; private set; }
        public RpcKcpClient RpcKcpClient { get; private set; }
        public RpcTcpClient RpcTcpClient { get; private set; }
        public RpcSuperSocketClient RpcSuperSocketClient { get; private set; }
        public RpcNetlyClient RpcNetlyClient { get; private set; }
        public RpcWebSocketClient RpcWebSocketClient { get; private set; }
        public Dictionary<string, ViewFactory> MapViewFactory { get; private set; } = new();
        Dictionary<string, IComponentHookFactory> MapComponentHookFactory { get; set; } = new();
        IRpcer Rpcer { get; set; }
        Dictionary<string, string> MapTargetServiceRpcClient { get; set; } = new();
        Dictionary<string, ContainerRpcCallerFactory> MapContainerRpcCallerFactory { get; set; } = new();
        Dictionary<string, ComponentRpcCallerFactory> MapEntityRpcCallerFactory { get; set; } = new();
        Dictionary<string, ContainerObserverRpcInfo> MapContainerObserverRpcInvoker { get; set; } = new();
        Dictionary<string, EntityObserverRpcInfo> MapEntityObserverRpcInvoker { get; set; } = new();
        Dictionary<string, ComponentStateFactory> MapEntityStateFactory { get; set; } = new();
        Dictionary<string, ComponentStateFactory> MapEntityStateFactory2 { get; set; } = new();
        Dictionary<string, ComponentFactory> MapComponentFactory { get; set; } = new();
        Dictionary<string, ContainerRpcAttribute> MapContainerRpcAttribute { get; set; } = new();

        public ClientUnity(List<Assembly> list_ass, Config config)
        {
            Instance = this;
            EventContext = new EventContext();
            Config = config;

            Dictionary<string, Dictionary<string, Func<int, IContainerRpcObserver, byte[], Task>>> m_containerobservercallee = new();
            Dictionary<string, Dictionary<string, Func<int, IComponentRpcObserver, byte[], Task>>> m_entityobservercallee = new();

            List<Type> loaded_types = new(1000);
            foreach (var ass in list_ass)
            {
                var arr_t = ass.GetTypes();
                foreach (var t in arr_t)
                {
                    loaded_types.Add(t);
                }
            }

            Debug.Log($"类型总数量： {loaded_types.Count} ~~~~~~~~~~~~~");

            foreach (var m in loaded_types)
            {
                Type t = m;

                //if (t.IsInterface)
                //{
                //    var arr = t.GetCustomAttributes(false);
                //    if (arr != null && arr.Length > 0)
                //    {
                //        foreach (var i in arr)
                //        {
                //            if (i is ContainerRpcAttribute att2)
                //            {
                //                string key = att2.ServiceName + "_" + att2.ContainerType;
                //                MapContainerRpcAttribute[key] = att2;

                //                Debug.Log($"{key}, {att2}");
                //            }
                //        }
                //    }
                //}

                if (!t.IsClass && t.IsInterface) continue;

                bool skip = false;
                var arr_att = t.GetCustomAttributes(false);
                if (arr_att != null && arr_att.Length > 0)
                {
                    foreach (var att in arr_att)
                    {
                        if (att is RegisterSkipAttribute)
                        {
                            skip = true;
                            continue;
                        }
                    }

                    if (skip) continue;

                    foreach (var att in arr_att)
                    {
                        if (att is ViewFactoryAttribute)
                        {
                            var factory = (ViewFactory)Activator.CreateInstance(t);
                            MapViewFactory[factory.GetName()] = factory;
                        }
                        else if (att is ComponentImplAttribute)
                        {
                            var att2 = (ComponentImplAttribute)att;

                            //Debug.Log($"注册ComponentFactory：{t.FullName}");

                            var t_factory = typeof(ComponentFactory<>).MakeGenericType(t);
                            var factory = (ComponentFactory)Activator.CreateInstance(t_factory);
                            RegisterComponentFactory(factory);
                        }
                        else if (att is RegisterContainerRpcObserverInvokeHelperAttribute)
                        {
                            var att2 = (RegisterContainerRpcObserverInvokeHelperAttribute)att;

                            string container_type = att2.ContainerType;

                            //Debug.Log($"注册ContainerRpcObserver响应辅助类：{t.Name}");

                            Dictionary<string, Func<int, IContainerRpcObserver, byte[], Task>> m_callee = new();

                            var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
                            foreach (var method in methods)
                            {
                                m_callee[method.Name] = (Func<int, IContainerRpcObserver, byte[], Task>)method.CreateDelegate(typeof(Func<int, IContainerRpcObserver, byte[], Task>));
                            }

                            m_containerobservercallee[container_type] = m_callee;
                        }
                        else if (att is RegisterComponentRpcObserverInvokeHelperAttribute)
                        {
                            var att2 = (RegisterComponentRpcObserverInvokeHelperAttribute)att;

                            //Debug.Log($"注册ComponentRpcObserver响应辅助类：{t.Name}");

                            Dictionary<string, Func<int, IComponentRpcObserver, byte[], Task>> m_callee = new();

                            var methods = t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
                            foreach (var method in methods)
                            {
                                m_callee[method.Name] = (Func<int, IComponentRpcObserver, byte[], Task>)method.CreateDelegate(typeof(Func<int, IComponentRpcObserver, byte[], Task>));
                            }

                            m_entityobservercallee[att2.ComponentName] = m_callee;
                        }
                        else if (att is RegisterComponentStateFactoryAttribute)
                        {
                            var att2 = (RegisterComponentStateFactoryAttribute)att;

                            //Debug.Log($"注册ComponentStateFactory：{t.Name}");

                            var factory = (ComponentStateFactory)Activator.CreateInstance(t);
                            string s1 = factory.GetName();
                            if (string.IsNullOrEmpty(s1))
                            {
                                Debug.LogError("RegisterComponentStateFactory2 Error，factory.GetName()==null！");
                            }

                            MapEntityStateFactory[s1] = factory;
                            MapEntityStateFactory2[factory.GetName2()] = factory;
                        }
                        else if (att is RegisterContainerRpcCallerFactoryAttribute)
                        {
                            var att2 = (RegisterContainerRpcCallerFactoryAttribute)att;

                            //Debug.Log($"注册ContainerRpcCallerFactory：{t.FullName}");

                            var factory = (ContainerRpcCallerFactory)Activator.CreateInstance(t);

                            string s1 = factory.GetName();
                            if (string.IsNullOrEmpty(s1))
                            {
                                Debug.LogError("RegisterContainerRpcCallerFactory Error，factory.GetName()==null！");
                            }

                            MapContainerRpcCallerFactory[s1] = factory;
                        }
                        else if (att is RegisterComponentRpcCallerFactoryAttribute)
                        {
                            var att2 = (RegisterComponentRpcCallerFactoryAttribute)att;

                            //Debug.Log($"注册ComponentRpcCallerFactory：{t.FullName}");

                            var factory = (ComponentRpcCallerFactory)Activator.CreateInstance(t);

                            string s1 = factory.GetName();
                            if (string.IsNullOrEmpty(s1))
                            {
                                Debug.LogError("RegisterComponentRpcCallerFactory Error，factory.GetName()==null！");
                            }

                            MapEntityRpcCallerFactory[s1] = factory;
                        }
                    }
                }

                var arr_interface = t.GetInterfaces();
                if (arr_interface != null && arr_interface.Length > 0)
                {
                    foreach (var iface in arr_interface)
                    {
                        var arr = iface.GetCustomAttributes(false);
                        if (arr != null && arr.Length > 0)
                        {
                            foreach (var i in arr)
                            {
                                //if (i is ContainerRpcAttribute)
                                //{
                                //    int taa = 0;
                                //}

                                if (i is ContainerRpcObserverAttribute)
                                {
                                    var ca = (ContainerRpcObserverAttribute)i;

                                    string container_type = ca.ContainerType;

                                    //Debug.Log($"注册ContainerObserverRpc响应类：{t.Name}，ContainerType={container_type}");

                                    ContainerObserverRpcInfo info = new()
                                    {
                                        MapMethod = new(),
                                        Instance = (IContainerRpcObserver)Activator.CreateInstance(t, new object[] { this }),
                                    };

                                    MapContainerObserverRpcInvoker[container_type] = info;
                                }

                                if (i is ComponentRpcObserverAttribute)
                                {
                                    var ca = (ComponentRpcObserverAttribute)i;

                                    string component_name = ca.ComponentName;

                                    //Debug.Log($"注册ComponentObserverRpc响应类：{t.Name}，ComponentName={component_name}");

                                    EntityObserverRpcInfo info = new()
                                    {
                                        MapMethod = new(),
                                    };

                                    MapEntityObserverRpcInvoker[component_name] = info;
                                }
                            }
                        }
                    }
                }
            }

            Debug.Log($"注册完成！");

            foreach (var i in MapContainerObserverRpcInvoker)
            {
                var m_callee = m_containerobservercallee[i.Key];
                i.Value.MapMethod = m_callee;
            }

            foreach (var i in MapEntityObserverRpcInvoker)
            {
                var m_callee = m_entityobservercallee[i.Key];
                i.Value.MapMethod = m_callee;
            }

            Rpcer = new RpcerClientUnity(this);

            Debug.Log($"ClientUnity初始化完成！");
        }

        public void Update(float tm)
        {
            RpcTcpClient?.Update(tm);

            RpcSuperSocketClient?.Update(tm);

            RpcNetlyClient?.Update(tm);

            RpcKcpClient?.Update(tm);

            RpcWebSocketClient?.Update(tm);

            foreach (var i in MapScene)
            {
                i.Value.UpdateClient(tm);
            }
        }

        public void UseRpcHttpClient(List<string> target_services, string url)
        {
            foreach (var i in target_services)
            {
                MapTargetServiceRpcClient[i.ToLower()] = "HttpClient";
            }

            RpcHttpClient = new(this, url);
        }

        public void UseRpcWebSocketClient(List<string> target_services, string url)
        {
            foreach (var i in target_services)
            {
                MapTargetServiceRpcClient[i.ToLower()] = "WebSocketClient";
            }

            RpcWebSocketClient = new(this, url);
        }

        public void UseRpcTcpClient(List<string> target_services, string host, int port)
        {
            foreach (var i in target_services)
            {
                MapTargetServiceRpcClient[i.ToLower()] = "TcpClient";
            }

            RpcTcpClient = new(this, host, port);
        }

        public void UseRpcSuperSocketClient(List<string> target_services, string host, int port)
        {
            foreach (var i in target_services)
            {
                MapTargetServiceRpcClient[i.ToLower()] = "SuperSocketClient";
            }

            RpcSuperSocketClient = new(this, host, port);
        }

        public void UseRpcNetlyClient(List<string> target_services, string host, int port)
        {
            foreach (var i in target_services)
            {
                MapTargetServiceRpcClient[i.ToLower()] = "NetlyClient";
            }

            RpcNetlyClient = new(this, host, port);
        }

        public void UseRpcKcpClient(List<string> target_services, string host, int port)
        {
            foreach (var i in target_services)
            {
                MapTargetServiceRpcClient[i.ToLower()] = "KcpClient";
            }

            RpcKcpClient = new(this, host, port);
        }

        void IClient.Close()
        {
            if (RpcWebSocketClient != null)
            {
                RpcWebSocketClient.Close();
                RpcWebSocketClient = null;
            }

            if (RpcTcpClient != null)
            {
                RpcTcpClient.Close();
                RpcTcpClient = null;
            }

            if (RpcSuperSocketClient != null)
            {
                RpcSuperSocketClient.Close();
                RpcSuperSocketClient = null;
            }

            if (RpcNetlyClient != null)
            {
                RpcNetlyClient.Close();
                RpcNetlyClient = null;
            }

            if (RpcKcpClient != null)
            {
                RpcKcpClient.Close();
                RpcKcpClient = null;
            }
        }

        void IClient.RegComponentHookFactory<T>(IComponentHookFactory<T> factory)
        {
            MapComponentHookFactory[typeof(T).Name] = factory;
        }

        IComponentHookFactory<T> IClient.TryGetComponentHookFactory<T>()
        {
            MapComponentHookFactory.TryGetValue(typeof(T).Name, out var factory);

            if (factory == null) return null;

            return (IComponentHookFactory<T>)factory;
        }

        T IClient.GetContainerRpc<T>(string container_id)
        {
            var t = typeof(T);

            MapContainerRpcCallerFactory.TryGetValue(t.Name, out var factory);
            if (factory == null)
            {
                Debug.LogError($"没有找到ContainerRpcCallerFactory！Name={t.Name}");
                return default;
            }

            var ca = factory.GetContainerRpcAttribute();

            var info = new RpcInfoClientUnityILR()
            {
                IsUnity = true,
                SourceServiceName = string.Empty,
                TargetServiceName = ca.ServiceName,
                ContainerOrEntity = true,
                ContainerStateType = ca.ContainerStateType,
                ContainerType = ca.ContainerType,
                ContainerId = (ca.ContainerStateType == ContainerStateType.Stateful) || (ca.ContainerStateType == ContainerStateType.StatefulNoReentrant) ? container_id : string.Empty,
                EntityId = 0,
                ComponentName = string.Empty,
            };

            return (T)factory.CreateContainerRpc(info, Rpcer);
        }

        T IClient.GetEntityRpc<T>(string container_type, string container_id, long entity_id, string component_name)
        {
            var t = typeof(T);

            MapEntityRpcCallerFactory.TryGetValue(t.Name, out var factory);
            if (factory == null)
            {
                Debug.LogError($"没有找到EntityRpcCallerFactory！Name={t.Name}");
                return default;
            }

            var ca = factory.GetComponentRpcAttribute();

            var info = new RpcInfoClientUnityILR()
            {
                IsUnity = true,
                SourceServiceName = "Client",
                TargetServiceName = ca.ServiceName,
                ContainerOrEntity = false,
                ContainerStateType = ca.ContainerStateType,
                ContainerType = container_type,
                ContainerId = container_id,
                EntityId = entity_id,
                ComponentName = component_name,
            };

            return (T)factory.CreateComponentRpc(info, Rpcer);
        }

        ComponentStateFactory IClient.GetEntityStateFactory(string name)
        {
            MapEntityStateFactory.TryGetValue(name, out ComponentStateFactory factory);
            return factory;
        }

        ComponentStateFactory IClient.GetEntityStateFactory2(string name)
        {
            MapEntityStateFactory2.TryGetValue(name, out ComponentStateFactory factory);
            return factory;
        }

        ComponentFactory IClient.GetComponentFactory(string name)
        {
            MapComponentFactory.TryGetValue(name, out ComponentFactory factory);
            return factory;
        }

        void IClient.DispatchContainerObserverRpc(string container_type, string method_name, byte[] method_data)
        {
            MapContainerObserverRpcInvoker.TryGetValue(container_type, out var info);
            if (info == null)
            {
                Debug.LogError($"DispatchContainerObserverRpc Error！没有找到ContainerType={container_type} MethodName={method_name}");
                return;
            }

            info.MapMethod.TryGetValue(method_name, out var method);
            if (method == null)
            {
                Debug.LogError($"DispatchContainerObserverRpc Error！没有找到MethodName={method_name}，ContainerType={container_type}");
                return;
            }

            try
            {
                method((int)Config.SerializerType, info.Instance, method_data);
            }
            catch (Exception e)
            {
                Debug.LogError($"DispatchContainerObserverRpc Error！{info.Instance.GetType().Name}，MethodName={method_name}\n{e}");
            }
        }

        void IClient.DispatchEntityObserverRpc(string container_type, string container_id, long entity_id, string component_name, string method_name, byte[] method_data)
        {
            //Sb.Clear();
            //Sb.Append(container_type);
            //Sb.Append(container_id);
            //string scene_key = Sb.ToString();
            //MapScene.TryGetValue(scene_key, out var scene);

            MapScene.TryGetValue(container_type, out var scene);

            if (scene == null)
            {
                Debug.LogWarning($"DispatchEntityObserverRpc Error！没有找到Scene={container_type}，ComponentName={component_name} MethodName={method_name}");
                return;
            }

            var component = scene.FindComponent(entity_id, component_name);
            if (component == null)
            {
                Debug.LogWarning($"DispatchEntityObserverRpc Error！没有找到EntityId={entity_id}，ComponentName={component_name} MethodName={method_name}");
                return;
            }

            MapEntityObserverRpcInvoker.TryGetValue(component_name, out var info);
            if (info == null)
            {
                Debug.LogError($"DispatchEntityObserverRpc Error！没有找到ComponentName={component_name}，EntityId={entity_id}，MethodName={method_name}");
                return;
            }

            info.MapMethod.TryGetValue(method_name, out var method);
            if (method == null)
            {
                Debug.LogError($"DispatchEntityObserverRpc Error！没有找到MethodName={method_name}，EntityId={entity_id}，ComponentName={component_name}");
                return;
            }

            if (component is IComponentRpcObserver ob)
            {
                try
                {
                    method((int)Config.SerializerType, ob, method_data);
                }
                catch (Exception e)
                {
                    Debug.LogError($"DispatchEntityObserverRpc Error！{ob.GetType().Name}，ComponentName={component_name} MethodName={method_name}\n{e}");
                }
            }
            else
            {
                Debug.LogError($"DispatchEntityObserverRpc Error！没有找到实现接口={method_name}，EntityId={entity_id}，ComponentName={component_name}");
            }
        }

        public string GetTargetServiceRpcClient(string target_service)
        {
            MapTargetServiceRpcClient.TryGetValue(target_service, out var rpc_client);
            return rpc_client;
        }

        public void RegisterComponentFactory(ComponentFactory factory)
        {
            string s1 = factory.GetName();
            if (string.IsNullOrEmpty(s1))
            {
                Debug.LogError("RegisterComponentFactory Error，factory.GetName()==null！");
            }

            MapComponentFactory[s1] = factory;
        }
    }
}

#endif