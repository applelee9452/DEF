using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DEF
{
    public sealed partial class Entity
    {
        public Scene Scene { get; private set; }
        public long Id { get; private set; } = 0;
        public string ContainerType { get; private set; }
        public string ContainerId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public bool IsDestroy { get; private set; } = false;
        public bool ExportEntityData { get; set; } = true;// 是否导出EntityData
        public bool Export4Bson { get; set; } = true;// 是否导出EntityDef4Bson
        public bool EntityTagSyncFlag { get; set; } = true;// 开关，是否同步Tag的变更，默认同步，同时也受NetworkSyncFlag的双重判定
        public Entity Parent { get; private set; } = null;
        internal List<Entity> Children { get; private set; }
        internal Dictionary<string, Component> MapComponent { get; private set; }// Key是Attribute中定义的名字
        public List<Component> ListComponent { get; private set; }
        HashSet<string> HashSetLocalTag { get; set; } = null;
        EntitySyncTag EntitySyncTag { get; set; }
        Dictionary<string, object> MapStateBalckboard { get; set; }// todo，有拆箱装箱性能损失，如何提高可观测性
        bool OnStart { get; set; } = false;
        bool OnDestroy { get; set; } = false;
        bool CreateFromPool { get; set; } = false;// 是否池化

        //Dictionary<string, dynamic> MapFunc { get; set; }// todo，待评估是否需要该功能

        internal Entity(string name, Entity parent, Scene scene, Dictionary<string, object> create_params, string client_sub_filter,
            params Type[] arr_com_type)
        {
            Scene = scene;
            Name = name;
            ContainerType = scene.ContainerType;
            ContainerId = scene.ContainerId;
            EntitySyncTag = new(Scene, this);

#if !DEF_CLIENT
            if (parent != null)
            {
                NetworkSyncFlag = parent.NetworkSyncFlag;
            }
            else
            {
                NetworkSyncFlag = Scene.SyncFlagDefault;
            }

            Id = Scene.IdGen.NewLong();
            ClientSubFilter = client_sub_filter;
#endif

            Parent = parent;
            SetParent(parent);
            Scene._AddEntity(this);

            if (arr_com_type != null && arr_com_type.Length > 0)
            {
                foreach (var i in arr_com_type)
                {
                    AddComponent(i, create_params);
                }
            }
        }

        internal Entity(ref EntityData entity_data, Entity parent, Scene scene, Dictionary<string, object> create_params)
        {
            Scene = scene;
            Id = entity_data.Id;
            ContainerType = entity_data.ContainerType;
            ContainerId = entity_data.ContainerId;
            Name = entity_data.Name;

            Parent = parent;
            SetParent(parent);
            Scene._AddEntity(this);

            EntitySyncTag = new(Scene, this, entity_data.Tags, entity_data.TagsRefCount);

#if !DEF_CLIENT
            if (parent != null)
            {
                NetworkSyncFlag = parent.NetworkSyncFlag;
            }
            else
            {
                NetworkSyncFlag = Scene.SyncFlagDefault;
            }
#endif

            if (entity_data.EntityStates != null)
            {
                if (MapComponent == null)
                {
                    MapComponent = new Dictionary<string, Component>();
                    ListComponent = new();
                }

                foreach (var i in entity_data.EntityStates)
                {
                    if (string.IsNullOrEmpty(i.Key)) continue;

                    ComponentFactory component_factory = null;
#if DEF_CLIENT
                    component_factory = Scene.Client.GetComponentFactory(i.Key);
                    if (component_factory == null)
                    {
                        UnityEngine.Debug.LogError($"找不到Name={i.Key}的ComponentFactory！");
                        continue;
                    }
#else
                    component_factory = Scene.Service.GetComponentFactory(i.Key);
                    if (component_factory == null)
                    {
                        // todo，LogError
                        // UnityEngine.Debug.LogError($"找不到Name={i.Key}的ComponentFactory！");
                        continue;
                    }
#endif
                    var component = component_factory.CreateComponent();
                    component.SetSceneAndEntity(Scene, this);
                    MapComponent[i.Key] = component;
                    ListComponent.Add(component);
                    component.Create(EntityStateSourceType.ProtoBuf, i.Value, null);

                    component.Awake(create_params);
                }
            }

            if (entity_data.Children != null && entity_data.Children.Count > 0)
            {
                Children ??= new List<Entity>(entity_data.Children.Count);

                for (int i = 0; i < entity_data.Children.Count; i++)
                {
                    var entity_data_child = entity_data.Children[i];

                    new Entity(ref entity_data_child, this, Scene, create_params);
                }
            }
        }

        internal Entity(AssetPrefab prefab, Entity parent, Scene scene)
        {
            Scene = scene;
            ContainerType = scene.ContainerType;
            ContainerId = scene.ContainerId;
            Name = prefab.Name;

            Parent = parent;
            SetParent(parent);
            Scene._AddEntity(this);

            EntitySyncTag = new(Scene, this);// todo，从初始化Tag

#if !DEF_CLIENT
            if (parent != null)
            {
                NetworkSyncFlag = parent.NetworkSyncFlag;
            }
            else
            {
                NetworkSyncFlag = Scene.SyncFlagDefault;
            }

            Id = Scene.IdGen.NewLong();
#endif

            if (prefab.Components != null)
            {
                if (MapComponent == null)
                {
                    MapComponent = new Dictionary<string, Component>();
                    ListComponent = new();
                }

                foreach (var i in prefab.Components)
                {
                    ComponentFactory component_factory = null;
#if DEF_CLIENT
                    component_factory = Scene.Client.GetComponentFactory(i.ComponentName);
                    if (component_factory == null)
                    {
                        UnityEngine.Debug.LogError($"找不到Name={i.ComponentName}的ComponentFactory！");
                        continue;
                    }
#else
                    component_factory = Scene.Service.GetComponentFactory(i.ComponentName);
                    if (component_factory == null)
                    {
                        // todo，LogError
                        // UnityEngine.Debug.LogError($"找不到Name={i.Name}的ComponentFactory！");
                        continue;
                    }
#endif
                    var component = component_factory.CreateComponent();
                    component.SetSceneAndEntity(Scene, this);
                    MapComponent[i.ComponentName] = component;
                    ListComponent.Add(component);
                    component.Create(EntityStateSourceType.ProtoBuf, i.States, null);

                    component.Awake(null);
                }
            }

            if (prefab.Children != null)
            {
                Children ??= new List<Entity>();

                foreach (var i in prefab.Children)
                {
                    new Entity(i, this, Scene);
                }
            }
        }

        internal Entity(EntityDef entity_def, Entity parent, Scene scene, Dictionary<string, object> create_params)
        {
            Scene = scene;
            ContainerType = scene.ContainerType;
            ContainerId = scene.ContainerId;
            Name = entity_def.Name;
            EntitySyncTag = new(Scene, this);

#if !DEF_CLIENT
            if (parent != null)
            {
                NetworkSyncFlag = parent.NetworkSyncFlag;
            }
            else
            {
                NetworkSyncFlag = Scene.SyncFlagDefault;
            }

            Id = Scene.IdGen.NewLong();
#endif

            Parent = parent;
            SetParent(parent);
            Scene._AddEntity(this);

            if (entity_def.Components != null)
            {
                if (MapComponent == null)
                {
                    MapComponent = new Dictionary<string, Component>();
                    ListComponent = new();
                }

                foreach (var t in entity_def.Components)
                {
                    _CheckAndCreateRequiredComponents(t);

                    var s = DEFUtils.GetComponentName(t);

#if DEF_CLIENT
                    var component_factory = Scene.Client.GetComponentFactory(s);
#else
                    var component_factory = Scene.Service.GetComponentFactory(s);
#endif
                    if (component_factory == null)
                    {
                        // todo，LogError
                        continue;
                    }

                    var component = component_factory.CreateComponent();
                    component.SetSceneAndEntity(Scene, this);
                    MapComponent[s] = component;
                    ListComponent.Add(component);
                    component.Create(EntityStateSourceType.None, null, null);

                    component.Awake(create_params);
                }
            }

            if (entity_def.Children != null)
            {
                Children ??= new List<Entity>();

                foreach (var i in entity_def.Children)
                {
                    new Entity(i, this, scene, create_params);
                }
            }
        }

#if !DEF_CLIENT
        internal Entity(MongoDB.Bson.BsonDocument entity_data, Entity parent, Scene scene)
        {
            Scene = scene;
            ContainerType = scene.ContainerType;
            ContainerId = scene.ContainerId;
            Id = entity_data["EntityId"].AsInt64;
            Name = entity_data["EntityName"].AsString;
            EntitySyncTag = new(Scene, this);

#if !DEF_CLIENT
            if (parent != null)
            {
                NetworkSyncFlag = parent.NetworkSyncFlag;
            }
            else
            {
                NetworkSyncFlag = Scene.SyncFlagDefault;
            }
#endif

            Parent = parent;
            SetParent(parent);
            Scene._AddEntity(this);

            if (!entity_data["States"].IsBsonNull)
            {
                if (MapComponent == null)
                {
                    MapComponent = new Dictionary<string, Component>();
                    ListComponent = new();
                }

                var components = entity_data["States"].AsBsonArray;
                foreach (var i in components)
                {
                    var s = i["StateName"].AsString;//DEFUtils.GetComponentName(t.Name);

#if DEF_CLIENT
                    var component_factory = Scene.Client.GetComponentFactory(s);
#else
                    var component_factory = Scene.Service.GetComponentFactory(s);
#endif
                    if (component_factory == null)
                    {
                        // todo，LogError
                        continue;
                    }

                    var component = component_factory.CreateComponent();
                    component.SetSceneAndEntity(Scene, this);
                    MapComponent[s] = component;
                    ListComponent.Add(component);

                    if (!i.IsBsonNull && !i["State"].IsBsonNull)
                    {
                        var entity_state = i["State"].AsBsonDocument;
                        component.Create(EntityStateSourceType.BsonDocument, entity_state, null);

                        component.Awake(null);
                    }
                    else
                    {
                        component.Create(EntityStateSourceType.None, null, null);

                        component.Awake(null);
                    }
                }
            }

            if (!entity_data["Children"].IsBsonNull)
            {
                Children ??= new List<Entity>();

                var children = entity_data["Children"].AsBsonArray;
                foreach (var i in children)
                {
                    new Entity(i.AsBsonDocument, this, scene);
                }
            }
        }
#endif

        public void Start()
        {
            if (OnStart) return;
            OnStart = true;

            if (ListComponent != null)
            {
                List<Component> list_component = new(ListComponent);
                foreach (var i in list_component)
                {
                    i.OnStart();
                }
            }

            if (Children != null)
            {
                List<Entity> children = new(Children);
                foreach (var i in children)
                {
                    i.Start();
                }
            }
        }

        public void Destroy(string reason = null, byte[] user_data = null, bool sync_network = true)
        {
            if (OnDestroy) return;
            OnDestroy = true;

            if (Children != null && Children.Count > 0)
            {
                List<Entity> children = new(Children);
                foreach (var i in children)
                {
                    i.Destroy(reason, user_data, false);
                }

                Children.Clear();
            }

#if !DEF_CLIENT
            DestroyServer(reason, user_data, sync_network);
#else
            DestroyClient();
#endif

            if (MapComponent != null)
            {
                for (int i = ListComponent.Count - 1; i >= 0; i--)
                {
                    ListComponent[i].Destroy(reason, user_data);
                }

                ListComponent?.Clear();
                MapComponent?.Clear();
            }

            if (Parent != null)
            {
                Parent.RemoveChild(this);
                Parent = null;
            }

            if (HashSetLocalTag != null)
            {
                HashSetLocalTag.Clear();
                HashSetLocalTag = null;
            }

            if (MapStateBalckboard != null)
            {
                MapStateBalckboard.Clear();
                MapStateBalckboard = null;
            }

            if (EntitySyncTag != null)
            {
                EntitySyncTag.Destroy();
                EntitySyncTag = null;
            }

            IsDestroy = true;
            MapComponent = null;
            ListComponent = null;
            Children = null;
            Scene = null;
        }

        public void DestroyChildren(string reason = null, byte[] user_data = null, bool sync_network = true)
        {
            if (Children != null && Children.Count > 0)
            {
                List<Entity> children = new(Children);
                foreach (var i in children)
                {
                    i.Destroy(reason, user_data, sync_network);
                }

                Children.Clear();
            }
        }

        public T AppendComponentLocal<T>(Dictionary<string, object> create_params = null) where T : ComponentLocal
        {
            if (MapComponent == null)
            {
                MapComponent = new Dictionary<string, Component>();
                ListComponent = new();
            }

            string s = DEFUtils.GetComponentName<T>();

#if DEF_CLIENT
            var component_factory = Scene.Client.GetComponentFactory(s);
#else
            var component_factory = Scene.Service.GetComponentFactory(s);
#endif
            if (component_factory == null)
            {
                // todo，LogError
                return null;
            }

            var component = component_factory.CreateComponent();

            component.SetSceneAndEntity(Scene, this);
            MapComponent[s] = component;
            ListComponent.Add(component);

            component.Create(EntityStateSourceType.None, null, create_params);

            component.Awake(create_params);

            return (T)component;
        }

        public bool ExistComponent<T>() where T : Component
        {
            if (MapComponent == null)
            {
                return false;
            }

            var s = DEFUtils.GetComponentName<T>();

            MapComponent.TryGetValue(s, out var b);
            return b != null;
        }

        public Component GetComponent(string component_name)
        {
            if (MapComponent == null)
            {
                return null;
            }

            MapComponent.TryGetValue(component_name, out var c);
            return c;
        }

        public T GetComponent<T>() where T : Component
        {
            if (MapComponent == null)
            {
                return null;
            }

            var s = DEFUtils.GetComponentName<T>();

            MapComponent.TryGetValue(s, out var c);
            return (T)c;
        }

        public void ChangeParent(Entity new_parent)
        {
            Entity old_parent = Parent;

            if (Parent != null)
            {
                Parent.RemoveChild(this);
                Parent = null;
            }

            Parent = new_parent;
            new_parent.Children ??= new List<Entity>();
            new_parent.Children.Add(this);

            if (ListComponent != null)
            {
                List<Component> list_component = new(ListComponent);
                foreach (var i in list_component)
                {
                    i.OnParentChanged(new_parent, old_parent);
                }
            }

#if !DEF_CLIENT
            if (NetworkSyncFlag)
            {
                Scene.WriteNetworkSyncBinlogChangeEntityParent(ClientSubFilter, Id, Parent != null ? Parent.Id : 0);
            }
#endif
        }

        internal void SetParent(Entity et)
        {
            et?.AddChild(this);
        }

        internal void AddChild(Entity et)
        {
            if (et.Parent != null)
            {
                et.Parent.RemoveChild(et);
                et.Parent = null;
            }

            Children ??= new List<Entity>();

            et.Parent = this;
            Children.Add(et);
        }

        internal void RemoveChild(Entity et)
        {
            if (Children == null)
            {
                return;
            }

            Children.Remove(et);
        }

        public uint GetChildCount()
        {
            if (Children == null) return 0;
            else return (uint)Children.Count;
        }

        public List<Entity> GetChildrenRef()
        {
            return Children;
        }

        public List<Entity> GetChildrenClone()
        {
            if (Children == null || Children.Count == 0)
            {
                return Children;
            }
            else
            {
                return new List<Entity>(Children);
            }
        }

        public Entity GetFirstChild()
        {
            if (Children == null || Children.Count == 0)
            {
                return null;
            }

            return Children[0];
        }

        public Entity FindChild(string entity_name)
        {
            if (Children == null)
            {
                return null;
            }

            foreach (var i in Children)
            {
                if (i.Name == entity_name)
                {
                    return i;
                }
            }

            return null;
        }

        public void AddLocalTag(string tag)
        {
            HashSetLocalTag ??= new HashSet<string>();

            HashSetLocalTag.Add(tag);

            Scene._AddEntityByLocalTag(tag, this);
        }

        public void RemoveLocalTag(string tag)
        {
            HashSetLocalTag?.Remove(tag);

            Scene._RemoveEntityByLocalTag(tag, this);
        }

        public bool ExistLocalTag(string tag)
        {
            if (HashSetLocalTag != null)
            {
                return HashSetLocalTag.Contains(tag);
            }

            return false;
        }

        public bool ContainAllSyncTag(SyncTagBase tag)
        {
            return EntitySyncTag.ContainAllSyncTag(tag);
        }

        public bool ContainNoneSyncTag(SyncTagBase tag)
        {
            return EntitySyncTag.ContainNoneSyncTag(tag);
        }

        public void AddSyncTag(SyncTagBase tag)
        {
            EntitySyncTag.AddSyncTag(tag);
        }

        public void RemoveSyncTag(SyncTagBase tag)
        {
            EntitySyncTag.RemoveSyncTag(tag);
        }

        // Tag必须先存在才能Set
        public void SetSyncTagValue(SyncTagBase tag, byte[] v)
        {
            EntitySyncTag.SetSyncTagValue(tag, v);
        }

        // Tag必须先存在才能Set，否则Set后值不会关联上去，直接丢弃
        public void SetSyncTagValueAsInt(SyncTagBase tag, int v)
        {
            EntitySyncTag.SetSyncTagValueAsInt(tag, v);
        }

        // Tag必须先存在，否则返回null
        public int GetSyncTagValueAsInt(SyncTagBase tag)
        {
            return EntitySyncTag.GetSyncTagValueAsInt(tag);
        }

        // Tag必须先存在才能Set，否则Set后值不会关联上去，直接丢弃
        public void SetSyncTagValueAsLong(SyncTagBase tag, long v)
        {
            EntitySyncTag.SetSyncTagValueAsLong(tag, v);
        }

        // Tag必须先存在，否则返回null
        public long GetSyncTagValueAsLong(SyncTagBase tag)
        {
            return EntitySyncTag.GetSyncTagValueAsLong(tag);
        }

        // Tag必须先存在才能Set，否则Set后值不会关联上去，直接丢弃
        public void SetSyncTagValueAsString(SyncTagBase tag, string v)
        {
            EntitySyncTag.SetSyncTagValueAsString(tag, v);
        }

        // Tag必须先存在，否则返回null
        public string GetTagValueAsString(SyncTagBase tag)
        {
            return EntitySyncTag.GetSyncTagValueAsString(tag);
        }

        public void OnSyncTagEvent(SyncTagBase tag, Action<SyncTagBase> action_add, Action<SyncTagBase> action_remove, Action<SyncTagBase> action_valuechanged)
        {
            EntitySyncTag.OnSyncTagEvent(tag, action_add, action_remove, action_valuechanged);
        }

        public void SetStateBalckboard(string key, object value)
        {
            MapStateBalckboard ??= new();

            MapStateBalckboard[key] = value;
        }

        public object GetStateBalckboard(string key)
        {
            if (MapStateBalckboard == null)
            {
                return default;
            }

            if (MapStateBalckboard.TryGetValue(key, out var value))
            {
                return value;
            }

            return default;
        }

        public EntityData? GetEntityData(string client_sub_guid, bool hierarchy = true)
        {
            if (!ExportEntityData)
            {
                return null;
            }

#if !DEF_CLIENT
            if (string.IsNullOrEmpty(ClientSubFilter))
            {
                // 全部订阅
            }
            else if (string.IsNullOrEmpty(client_sub_guid))
            {
                // 全部订阅
            }
            else if (ClientSubFilter != client_sub_guid)
            {
                // 单个订阅，但是订阅者不在订阅过滤器范围内

                return null;
            }
#endif

            EntityData entity_data = new()
            {
                Id = Id,
                Name = Name,
                ContainerType = ContainerType,
                ContainerId = ContainerId,
                Tags = EntitySyncTag?.GetSyncTags(),
                TagsRefCount = EntitySyncTag?.GetSyncTagsRefCount(),
                EntityStates = null,
                Children = null,
            };

            if (MapComponent != null && ListComponent != null)
            {
                entity_data.EntityStates = new KeyValuePair<string, byte[]>[ListComponent.Count];

                for (int i = 0; i < ListComponent.Count; i++)
                {
                    var c = ListComponent[i];
                    var state = c.GetState();
                    if (state == null)
                    {
                        // 网络不同步ComponentLocal
                        //entity_data.EntityStates.Add(new KeyValuePair<string, byte[]>(i.Name, null));

                        var s = new KeyValuePair<string, byte[]>(string.Empty, null);

                        entity_data.EntityStates[i] = s;
                    }
                    else
                    {
                        using var ms = new MemoryStream();
                        Serializer.Serialize(ms, state);
                        var entity_state = ms.ToArray();

                        var s = new KeyValuePair<string, byte[]>(c.Name, entity_state);

                        entity_data.EntityStates[i] = s;
                    }
                }
            }

            if (hierarchy && Children != null)
            {
                entity_data.Children = new List<EntityData>(Children.Count);

                for (int i = 0; i < Children.Count; i++)
                {
                    var entity_data_child = Children[i].GetEntityData(client_sub_guid);

                    if (entity_data_child != null)
                    {
                        entity_data.Children.Add((EntityData)entity_data_child);
                    }
                }
            }

            return entity_data;
        }

        public AssetPrefab GetAssetPrefab()
        {
            AssetPrefab asset_prefab = new()
            {
                Name = Name,
                Tags = null,
            };

            if (MapComponent != null)
            {
                asset_prefab.Components = new();

                if (ListComponent != null)
                {
                    foreach (var i in ListComponent)
                    {
                        ComponentPrefab component_prefab = new()
                        {
                            ComponentName = i.Name,
                        };

                        var state = i.GetState();
                        if (state != null)
                        {
                            component_prefab.States = Newtonsoft.Json.JsonConvert.SerializeObject(state);
                        }

                        // 通过反射获取所有变量的Key&Value
                    }
                }
            }

            if (Children != null)
            {
                asset_prefab.Children = new();
                foreach (var i in Children)
                {
                    var asset_prefab_child = i.GetAssetPrefab();
                    asset_prefab.Children.Add(asset_prefab_child);
                }
            }

            return asset_prefab;
        }

        public T GenEvent<T>() where T : Event, new()
        {
            return Scene.EventContext.GenEvent<T>();
        }

        public T GenSelfEvent<T>() where T : SelfEvent, new()
        {
            T ev = new();
            ev.SetEntity(this);
            return ev;
        }

        public void _HandleSelfEvent(SelfEvent ev)
        {
            if (ListComponent != null && ListComponent.Count > 0)
            {
                var list_component = new List<Component>(ListComponent);
                foreach (var i in list_component)
                {
                    i.HandleSelfEvent(ev);
                }
            }
        }

        public string ToDebugString()
        {
            StringBuilder sb = new(256);

            sb.Append($"Name: {Name}\n");
            sb.Append($"Id: {Id}\n");
            sb.Append($"ContainerId: {ContainerId}\n");

            if (MapComponent != null)
            {
                foreach (var i in MapComponent)
                {
                    sb.Append($"组件名: {i.Key}\n");
                }
            }

            if (Children != null)
            {
                foreach (var i in Children)
                {
                    //sb.AppendLine();

                    var s = i.ToDebugString();
                    sb.Append(s);
                }
            }

            return sb.ToString();
        }

        Component AddComponent(Type t, Dictionary<string, object> create_params = null)
        {
            if (MapComponent == null)
            {
                MapComponent = new Dictionary<string, Component>();
                ListComponent = new();
            }

            _CheckAndCreateRequiredComponents(t);

            var s = DEFUtils.GetComponentName(t);

            if (!MapComponent.ContainsKey(s))
            {
                // todo，改由工厂类创建组件
                var c = (Component)Activator.CreateInstance(t);
                c.SetSceneAndEntity(Scene, this);
                MapComponent[s] = c;
                ListComponent.Add(c);
                c.Name = s;

                c.Create(EntityStateSourceType.None, null, create_params);

                c.Awake(create_params);

                return c;
            }
            else
            {
                MapComponent.TryGetValue(s, out var c);
                return c;
            }
        }

        T AddComponent<T>(Dictionary<string, object> create_params = null) where T : Component, new()
        {
            if (MapComponent == null)
            {
                MapComponent = new Dictionary<string, Component>();
                ListComponent = new();
            }

            _CheckAndCreateRequiredComponents(typeof(T));

            var s = DEFUtils.GetComponentName<T>();

            if (!MapComponent.ContainsKey(s))
            {
                // todo，改由工厂类创建组件
                var c = new T();

                c.SetSceneAndEntity(Scene, this);
                MapComponent[s] = c;
                ListComponent.Add(c);
                c.Name = s;

                c.Create(EntityStateSourceType.None, null, create_params);

                c.Awake(create_params);

                return c;
            }
            else
            {
                MapComponent.TryGetValue(s, out var c);
                return (T)c;
            }
        }

        void _CheckAndCreateRequiredComponents(Type component_type)
        {
            if (!component_type.IsDefined(typeof(ComponentRequiredAttribute)))
            {
                return;
            }

            var attr = component_type.GetCustomAttribute<ComponentRequiredAttribute>();

            if (attr.RequiredComponents == null || attr.RequiredComponents.Length == 0)
            {
                return;
            }

            foreach (var required_component_type in attr.RequiredComponents)
            {
                if (MapComponent.ContainsKey(DEFUtils.GetComponentName(required_component_type)))
                {
                    continue;
                }

                AddComponent(required_component_type);
            }
        }
    }
}