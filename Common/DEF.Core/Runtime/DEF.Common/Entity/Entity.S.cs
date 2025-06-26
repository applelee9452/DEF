#if !DEF_CLIENT

using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF
{
    public sealed partial class Entity
    {
        public bool NetworkSyncFlag { get; private set; }// 开关，是否收集网络变更
        public string ClientSubFilter { get; private set; }// Client订阅过滤器，为空表示所有Client均订阅。可以设为指定的Client SessionGuid，表示该Entity所有网络变更只推送给该Client

        // 不可以随意开关，需要预先规划好
        public void SetNetworkSyncFlag(bool network_sync_flag)
        {
            NetworkSyncFlag = network_sync_flag;

            if (Children != null)
            {
                foreach (var i in Children)
                {
                    i.SetNetworkSyncFlag(NetworkSyncFlag);
                }
            }
        }

        public void SetClientSubFilter(string session_guid)
        {
            ClientSubFilter = session_guid;
        }

        public Task SyncDelta2Db(IMongoDatabase db, string collection_name)
        {
            return SyncDelta2Db(db, collection_name, Scene.ContainerId);
        }

        public async Task SyncDelta2Db(IMongoDatabase db, string collection_name, string id)
        {
            EntityDef4Bson? def = GetEntityDef4Bson();

            var entity_states = def.ToBsonDocument();
            entity_states.Add("_id", id);

            var collection = db.GetCollection<BsonDocument>(collection_name);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", id);
            var options = new ReplaceOptions()
            {
                IsUpsert = true,
            };

            await collection.ReplaceOneAsync(filter, entity_states, options);
        }

        //public static Task SyncDirtyStates2Db(this Entity entity, IMongoDatabase db, string collection_name)
        //{
        //    //var collection = db.GetCollection<BsonDocument>(collection_name);

        //    //var filter = Builders<BsonDocument>.Filter.Eq("_id", entity.Scene.ContainerId);

        //    //var updater = Builders<BsonDocument>.Update.Set("_id", "asdfasdf");

        //    //await collection.UpdateOneAsync(filter, updater);

        //    return Task.CompletedTask;
        //}

        public EntityDef4Bson? GetEntityDef4Bson()
        {
            if (!Export4Bson)
            {
                return null;
            }

            EntityDef4Bson def = new()
            {
                EntityId = Id,
                EntityName = Name
            };

            if (ListComponent != null && ListComponent.Count > 0)
            {
                def.States = new List<StateInfo>(ListComponent.Count);

                foreach (var i in ListComponent)
                {
                    if (!i.DbSyncFlag) continue;

                    var state_name = i.Name;
                    var state = i.GetState();

                    if (state == null) continue;// ComponentLocal不保存Db

                    var state_info = new StateInfo()
                    {
                        StateName = state_name,
                        State = state,
                    };

                    def.States.Add(state_info);
                }
            }

            if (Children != null && Children.Count > 0)
            {
                def.Children = [];

                foreach (var i in Children)
                {
                    var def_child = i.GetEntityDef4Bson();

                    if (def_child != null)
                    {
                        def.Children.Add((EntityDef4Bson)def_child);
                    }
                }
            }

            return def;
        }

        void DestroyServer(string reason, byte[] user_data, bool sync_network)
        {
            if (NetworkSyncFlag && sync_network)
            {
                Scene.WriteNetworkSyncBinlogRemoveEntity(ClientSubFilter, Id, reason, user_data);
            }
        }
    }
}

#endif