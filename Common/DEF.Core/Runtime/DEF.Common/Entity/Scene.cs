#if !DEF_CLIENT
using Microsoft.Extensions.Logging;
using Orleans;
#endif
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DEF
{
    // 多线程安全
    public sealed partial class Scene
    {
        public string Name { get { return RootEntity.Name; } }
        public string ContainerType { get; private set; }
        public string ContainerId { get; private set; }
        public Entity RootEntity { get; private set; }
        public EventContext EventContext { get; private set; }
        public NodeGraphContext NodeGraphContext { get; private set; }
        public SerializerType SerializerType { get; private set; } = SerializerType.MemoryPack;
        public bool IsServer { get; private set; }
        public bool Destroying { get; private set; } = false;// Scene是否正在销毁中
        internal Dictionary<long, Entity> MapAllEntity { get; private set; }
        Dictionary<string, HashSet<Entity>> MapAllEntityExistLocalTag { get; set; } = new();
        Dictionary<string, Component> BlackboardComponent { get; set; } = new();// 黑板Component，需要能被公共访问的Component都放到这里
        Dictionary<string, HashSet<Component>> MapAllComponent { get; set; } = new();
        HashSet<string> HashsetLocalTag { get; set; }// 自定义标签
        Dictionary<string, Queue<Entity>> MapEntityPool { get; set; }

        public Entity CreateEntity(string entity_name, Dictionary<string, object> create_params = null, string client_sub_filter = null)
        {
            var et = new Entity(entity_name, RootEntity, this, create_params, client_sub_filter);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(string entity_name, Entity parent, Dictionary<string, object> create_params = null, string client_sub_filter = null)
        {
            var et = new Entity(entity_name, parent, this, create_params, client_sub_filter);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1>(string entity_name, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component
        {
            var et = new Entity(entity_name, RootEntity, this, create_params, client_sub_filter, typeof(T1));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1>(string entity_name, Entity parent, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component
        {
            var et = new Entity(entity_name, parent, this, create_params, client_sub_filter, typeof(T1));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2>(string entity_name, Entity parent, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component
        {
            var et = new Entity(entity_name, parent, this, create_params, client_sub_filter, typeof(T1), typeof(T2));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2>(string entity_name, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component
        {
            var et = new Entity(entity_name, RootEntity, this, create_params, client_sub_filter, typeof(T1), typeof(T2));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3>(string entity_name, Entity parent, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component
        {
            var et = new Entity(entity_name, parent, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3>(string entity_name, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component
        {
            var et = new Entity(entity_name, RootEntity, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3, T4>(string entity_name, Entity parent, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component where T4 : Component
        {
            var et = new Entity(entity_name, parent, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3), typeof(T4));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3, T4>(string entity_name, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component where T4 : Component
        {
            var et = new Entity(entity_name, RootEntity, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3), typeof(T4));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3, T4, T5>(string entity_name, Entity parent, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component
        {
            var et = new Entity(entity_name, parent, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3, T4, T5, T6>(string entity_name, Entity parent, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component
        {
            var et = new Entity(entity_name, parent, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3, T4, T5>(string entity_name, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component
        {
            var et = new Entity(entity_name, RootEntity, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3, T4, T5, T6>(string entity_name, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component
        {
            var et = new Entity(entity_name, RootEntity, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity<T1, T2, T3, T4, T5, T6, T7>(string entity_name, Entity parent, Dictionary<string, object> create_params = null, string client_sub_filter = null)
            where T1 : Component where T2 : Component where T3 : Component where T4 : Component where T5 : Component where T6 : Component where T7 : Component
        {
            var et = new Entity(entity_name, parent, this, create_params, client_sub_filter, typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7));

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(ref EntityData entity_data, Dictionary<string, object> create_params = null)
        {
            var et = new Entity(ref entity_data, RootEntity, this, create_params);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(ref EntityData entity_data, Entity parent, Dictionary<string, object> create_params = null)
        {
            var et = new Entity(ref entity_data, parent, this, create_params);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(AssetPrefab prefab)
        {
            var et = new Entity(prefab, RootEntity, this);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(AssetPrefab prefab, Entity parent)
        {
            var et = new Entity(prefab, parent, this);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(EntityDef entity_def)
        {
            var et = new Entity(entity_def, RootEntity, this, null);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(EntityDef entity_def, Entity parent)
        {
            var et = new Entity(entity_def, parent, this, null);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(EntityDef entity_def, Dictionary<string, object> create_params)
        {
            var et = new Entity(entity_def, RootEntity, this, create_params);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(EntityDef entity_def, Entity parent, Dictionary<string, object> create_params)
        {
            var et = new Entity(entity_def, parent, this, create_params);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        // 注册需要池化的Entity
        public void RegEntityPool(string key, EntityDef entity_def, int init_count = 1)
        {
            MapEntityPool ??= new();
        }

        // 从池中获取Entity
        public Entity GenEntityFromPool(string key, Entity parent, Dictionary<string, object> create_params)
        {
            MapEntityPool ??= new();

            return null;
        }

        // 从池中获取Entity
        public Entity GenEntityFromPool(string key, Entity parent)
        {
            return GenEntityFromPool(key, parent, null);
        }

        // 从池中获取Entity
        public Entity GenEntityFromPool(string key, Dictionary<string, object> create_params)
        {
            return GenEntityFromPool(key, null, create_params);
        }

        // 从池中获取Entity
        public Entity GenEntityFromPool(string key)
        {
            return GenEntityFromPool(key, null, null);
        }

        // 释放Entity到池中
        public void FreeEntityToPool(Entity entity)
        {
            MapEntityPool ??= new();
        }

        // 卸载场景
        public void Unload()
        {
            Destroying = true;

            string name = string.Empty;

            if (RootEntity != null)
            {
                name = RootEntity.Name;
                RootEntity.Destroy();
                RootEntity = null;
            }

            if (BlackboardComponent != null)
            {
                BlackboardComponent.Clear();
                BlackboardComponent = null;
            }

#if DEF_CLIENT
            if (Client != null && !string.IsNullOrEmpty(name) && Client.MapScene.ContainsKey(name))
            {
                Client.MapScene.Remove(name);
            }
#endif
        }

        public List<Entity> GetRootEntitiesRef()
        {
            return RootEntity.GetChildrenRef();
        }

        public List<Entity> GetRootEntitiesClone()
        {
            return RootEntity.GetChildrenClone();
        }

        public void RemoveEntity(long entity_id)
        {
            MapAllEntity.TryGetValue(entity_id, out Entity entity);

            entity?.Destroy();
        }

        public void RemoveEntity(long entity_id, string reason, byte[] user_data)
        {
            MapAllEntity.TryGetValue(entity_id, out Entity entity);

            entity?.Destroy(reason, user_data);
        }

        public Entity FindEntity(long entity_id)
        {
            MapAllEntity.TryGetValue(entity_id, out var entity);

            return entity;
        }

        public Component FindComponent(long entity_id, string component_name)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                return null;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                return null;
            }

            return c;
        }

        public List<Entity> GetChildren()
        {
            return RootEntity.Children;
        }

        public Entity GetFirstChild()
        {
            if (RootEntity.Children == null || RootEntity.Children.Count == 0)
            {
                return null;
            }

            return RootEntity.Children[0];
        }

        public Entity FindChild(string entity_name)
        {
            if (RootEntity.Children == null)
            {
                return null;
            }

            foreach (var i in RootEntity.Children)
            {
                if (i.Name == entity_name)
                {
                    return i;
                }
            }

            return null;
        }

        public void Add2Blackboard(string name, Component component)
        {
            BlackboardComponent[name] = component;
        }

        public void RemoveFromBlackboard(string name)
        {
            BlackboardComponent.Remove(name);
        }

        public Component GetComponentFromBlackboard(string name)
        {
            if (!BlackboardComponent.TryGetValue(name, out var c))
            {
                return null;
            }

            return c;
        }

        public void Add2Blackboard<T>(T component) where T : Component
        {
            string name = typeof(T).Name;

            BlackboardComponent[name] = component;
        }

        public void RemoveFromBlackboard<T>() where T : Component
        {
            string name = typeof(T).Name;

            BlackboardComponent.Remove(name);
        }

        public void RemoveFromBlackboard<T>(T component) where T : Component
        {
            string name = typeof(T).Name;

            BlackboardComponent.Remove(name);
        }

        public T GetComponentFromBlackboard<T>() where T : Component
        {
            string name = typeof(T).Name;

            if (!BlackboardComponent.TryGetValue(name, out var c))
            {
                return null;
            }

            return (T)c;
        }

        // 获取场景中所有指定类型的Component。注意：这里返回的HashSet是只读的，不能修改
        public HashSet<T> GetComponents<T>() where T : Component
        {
            string name = typeof(T).Name;

            if (!MapAllComponent.TryGetValue(name, out var set))
            {
                return null;
            }

            HashSet<T> list = new();
            foreach (var i in set)
            {
                list.Add((T)i);
            }

            return list;
        }

        internal void _AddComponent(Component component)
        {
            string name = component.GetType().Name;
            if (!MapAllComponent.TryGetValue(name, out var set))
            {
                set = new HashSet<Component>();
                MapAllComponent[name] = set;
            }

            set.Add(component);
        }

        internal void _RemoveComponent(Component component)
        {
            string name = component.GetType().Name;
            if (!MapAllComponent.TryGetValue(name, out var set))
            {
                return;
            }

            set.Remove(component);
        }

        public HashSet<Entity> GetEntitiesByLocalTag(string local_tag)
        {
            if (!MapAllEntityExistLocalTag.TryGetValue(local_tag, out var set))
            {
                return null;
            }

            return set;
        }

        internal void _AddEntityByLocalTag(string local_tag, Entity et)
        {
            if (!MapAllEntityExistLocalTag.TryGetValue(local_tag, out var set))
            {
                set = new HashSet<Entity>();
                MapAllEntityExistLocalTag[local_tag] = set;
            }

            set.Add(et);
        }

        internal void _RemoveEntityByLocalTag(string local_tag, Entity et)
        {
            if (!MapAllEntityExistLocalTag.TryGetValue(local_tag, out var set))
            {
                return;
            }

            set.Remove(et);
        }

        public NodeGraph GetNodeGraph(string nodegraph_name)
        {
            return NodeGraphContext.GetNodeGraph(nodegraph_name);
        }

        public T GenEvent<T>() where T : Event, new()
        {
            return EventContext.GenEvent<T>();
        }

        public void AddLocalTag(string tag)
        {
            HashsetLocalTag ??= new();
            HashsetLocalTag.Add(tag);
        }

        public bool ExistLocalTag(string tag)
        {
            if (HashsetLocalTag == null)
            {
                return false;
            }

            return HashsetLocalTag.Contains(tag);
        }

        public void RemoveLocalTag(string tag)
        {
            if (HashsetLocalTag != null)
            {
                HashsetLocalTag.Remove(tag);
            }
        }

        internal void _AddEntity(Entity et)
        {
            if (et.Id > 0)
            {
                MapAllEntity[et.Id] = et;
            }
        }
    }
}