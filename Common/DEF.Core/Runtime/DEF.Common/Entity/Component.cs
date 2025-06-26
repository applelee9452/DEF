#if !DEF_CLIENT 
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Orleans;
#endif
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DEF
{
    public enum EntityStateSourceType
    {
        None = 0,
        ProtoBuf,
        BsonDocument
    }

    public abstract class Component : EventListener
    {
        public virtual bool DbSyncFlag { get; protected set; } = true;
        public string Name { get; set; }
        public Scene Scene { get; private set; }
        public Entity Entity { get; private set; }
        public Dictionary<string, MethodInfo> MapEntityMethod { get; set; } = new();
#if !DEF_CLIENT
        public ILogger Logger { get; set; }
#endif
        Dictionary<string, List<Delegate>> MapEventListener { get; set; }

        public void ListenEvent<T>(Action<T> action) where T : DEF.Event
        {
            MapEventListener ??= new();

            var name = typeof(T).Name;

            MapEventListener.TryGetValue(name, out var list_eventlistner);
            if (list_eventlistner == null)
            {
                list_eventlistner = new List<Delegate>();
                MapEventListener[name] = list_eventlistner;
            }
            list_eventlistner.Add(action);

            EventContext.ListenEvent<T>(action);
        }

        public void ListenEvent<T>() where T : Event
        {
            EventContext.ListenEvent<T>(this);
        }

        public void UnListenAllEvent()
        {
            if (MapEventListener != null)
            {
                foreach (var i in MapEventListener)
                {
                    foreach (var j in i.Value)
                    {
                        EventContext.UnListenEvent(i.Key, j);
                    }
                }

                MapEventListener.Clear();
            }

            EventContext.UnListenAllEvent(this);
        }

        public T GenEvent<T>() where T : Event, new()
        {
            return EventContext.GenEvent<T>();
        }

        public bool IsValid(Component c)
        {
            if (c == null || c.Entity == null || c.Entity.IsDestroy)
            {
                return false;
            }

            return true;
        }

        public NodeGraph GetNodeGraph(string nodegraph_name)
        {
            return Scene.NodeGraphContext.GetNodeGraph(nodegraph_name);
        }

        public T GetContainerRpc<T>() where T : IContainerRpc
        {
#if DEF_CLIENT
            var rpc = Scene.Client.GetContainerRpc<T>(string.Empty);
            return (T)rpc;
#else
            var rpc = Scene.Service.GetContainerRpc<T>(string.Empty);
            return (T)rpc;
#endif
        }

        public T GetContainerRpc<T>(string container_id) where T : IContainerRpc
        {
#if DEF_CLIENT
            var rpc = Scene.Client.GetContainerRpc<T>(container_id);
            return (T)rpc;
#else
            var rpc = Scene.Service.GetContainerRpc<T>(container_id);
            return (T)rpc;
#endif
        }

        public T GetEntityRpc<T>() where T : IComponentRpc
        {
            if (Entity.Id == 0
                || string.IsNullOrEmpty(Entity.ContainerType)
                || string.IsNullOrEmpty(Entity.ContainerId)
                || string.IsNullOrEmpty(Name))
            {
                return default;
            }

#if DEF_CLIENT
            return Scene.Client.GetEntityRpc<T>(Entity.ContainerType, Entity.ContainerId, Entity.Id, Name);
#else
            return Scene.Service.GetEntityRpc<T>(Entity.ContainerType, Entity.ContainerId, Entity.Id, Name);
#endif
        }

#if !DEF_CLIENT
        public T GetContainerRpcObserver<T>(string gateway_guid, string session_guid) where T : IContainerRpcObserver
        {
            return Scene.Service.GetContainerObserverRpc<T>(gateway_guid, session_guid);
        }

        public T GetEntityRpcObserver<T>(string gateway_guid, string session_guid) where T : IComponentRpcObserver
        {
            if (Entity.Id == 0 || string.IsNullOrEmpty(Entity.ContainerId) || string.IsNullOrEmpty(Name))
            {
                return default;
            }

            return Scene.Service.GetEntityObserverRpc<T>(
                Entity.ContainerType, Entity.ContainerId, Entity.Id, Name,
                gateway_guid, session_guid);
        }
#endif

        internal abstract void Create(EntityStateSourceType source_type, object source, Dictionary<string, object> create_params);

        internal abstract void Destroy(string reason, byte[] user_data = null);

        public abstract void Awake(Dictionary<string, object> create_params);

        public abstract void OnStart();

        public abstract void OnDestroy(string reason = null, byte[] user_data = null);

        public virtual void OnEnqueePool() { }

        public virtual void OnDequeePool() { }

        public virtual void OnParentChanged(Entity new_parent, Entity old_parent) { }

        public abstract void HandleSelfEvent(SelfEvent ev);

        public abstract IComponentState GetState();

        public virtual void OnClientApplyDirtyStatesDone(HashSet<string> keys) { }

        internal void SetSceneAndEntity(Scene scene, Entity entity)
        {
            Scene = scene;
            EventContext = Scene.EventContext;
            Entity = entity;
            Scene._AddComponent(this);
#if !DEF_CLIENT
            Logger = Scene.Logger;
#endif
        }
    }

    public abstract class ComponentLocal : Component
    {
        internal override void Create(EntityStateSourceType source_type, object source, Dictionary<string, object> create_params)
        {
        }

        internal override void Destroy(string reason = null, byte[] user_data = null)
        {
            OnDestroy(reason, user_data);

            Scene._RemoveComponent(this);
        }

        public override IComponentState GetState()
        {
            return null;
        }
    }

    public abstract class Component<TState> : Component where TState : IComponentState
    {
        public EntityStateSourceType StateSourceType { get; private set; }// 用于区分New还是Load
        public TState State { get; private set; }

        internal override void Create(EntityStateSourceType source_type, object source, Dictionary<string, object> create_params)
        {
            StateSourceType = source_type;

#if DEF_CLIENT
            var entitystate_factory = Scene.Client.GetEntityStateFactory2(typeof(TState).Name);
#else
            var entitystate_factory = Scene.Service.GetEntityStateFactory2(typeof(TState).Name);
#endif
            State = (TState)entitystate_factory.CreateState(this, source_type, source);

            var t = GetType();
            var arr = t.GetInterfaces();

            if (arr != null)
            {
                foreach (var i in arr)
                {
#if !DEF_CLIENT
                    var callee1 = Scene.Service.GetEntityRpcCallee(i.Name);
                    if (callee1 != null)
                    {
                        foreach (var j in callee1)
                        {
                            if (MapEntityMethod.ContainsKey(j.Key))
                            {
                                //Logger.LogError($"函数{j.Key}名称重复！");
                            }
                            else
                            {
                                MapEntityMethod[j.Key] = j.Value;
                            }
                        }
                    }
#else
                    //var callee1 = Scene.Client.getEnti.GetEntityRpcCallee(i.Name);
                    //if (callee1 != null)
                    //{
                    //    foreach (var j in callee1)
                    //    {
                    //        if (MapEntityMethod.ContainsKey(j.Key))
                    //        {
                    //            //Logger.LogError($"函数{j.Key}名称重复！");
                    //        }
                    //        else
                    //        {
                    //            MapEntityMethod[j.Key] = j.Value;
                    //        }
                    //    }
                    //}
#endif
                }
            }
        }

        internal override void Destroy(string reason = null, byte[] user_data = null)
        {
            OnDestroy(reason, user_data);

            State.Release();

            Scene._RemoveComponent(this);

            State = default;
        }

        public override IComponentState GetState()
        {
            return State;
        }
    }

    public abstract class ComponentFactory
    {
        public abstract string GetName();

        public abstract Component CreateComponent();
    }

    public class ComponentFactory<TComponent> : ComponentFactory where TComponent : Component, new()
    {
        string Name { get; set; }

        public override string GetName()
        {
            Name = DEFUtils.GetComponentName<TComponent>();

            return Name;
        }

        public override Component CreateComponent()
        {
            TComponent c = new()
            {
                Name = Name
            };
            return c;
        }
    }
}