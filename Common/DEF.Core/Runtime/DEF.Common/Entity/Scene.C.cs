#if DEF_CLIENT

using System.Collections.Generic;

namespace DEF
{
    public sealed partial class Scene
    {
        public IClient Client { get; private set; }

        Scene(EventContext event_context, string scene_name, IClient client, Dictionary<string, object> create_params)
        {
            ContainerId = string.Empty;
            MapAllEntity = new Dictionary<long, Entity>();
            EventContext = event_context;
            Client = client;
            IsServer = false;

            client.MapScene[scene_name] = this;

            RootEntity = new Entity(scene_name, null, this, create_params, null);
            RootEntity.Start();
        }

        Scene(EventContext event_context, string scene_name, ref EntityData entity_data, IClient client, Dictionary<string, object> create_params)
        {
            ContainerType = entity_data.ContainerType;
            ContainerId = entity_data.ContainerId;
            MapAllEntity = new Dictionary<long, Entity>();
            EventContext = event_context;
            Client = client;
            IsServer = false;

            client.MapScene[scene_name] = this;

            RootEntity = new Entity(scene_name, null, this, create_params, null);
            RootEntity.Start();

            CreateEntity(ref entity_data, create_params);
        }

        public void UpdateClient(float tm)
        {
            UpdateLerp(tm);
        }

        public static Scene New(EventContext event_context, string scene_name, ref EntityData entity_data, IClient client, Dictionary<string, object> create_params = null)
        {
            return new Scene(event_context, scene_name, ref entity_data, client, create_params);
        }

        public static Scene New(EventContext event_context, string scene_name, IClient client, Dictionary<string, object> create_params = null)
        {
            return new Scene(event_context, scene_name, client, create_params);
        }
    }
}

#endif