#if DEF_CLIENT

using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF
{
    public class DirtyComponentInfo1
    {
        public DEF.Component C;
        public HashSet<string> HashSetKey = new();
    }

    public class ContainerObserverDEF : IContainerObserverDEF
    {
        IClient Client { get; set; }
        Dictionary<long, List<DirtyComponentInfo1>> MapDirty { get; set; } = new();

        public ContainerObserverDEF(IClient client)
        {
            Client = client;
        }

        Task IContainerObserverDEF.SyncSceneSnapshot2Client(string scene_name, EntityData entity_data)
        {
            Client.MapScene.TryGetValue(scene_name, out var scene);
            if (scene != null)
            {
                scene.Unload();
            }

            UnityEngine.Debug.Log($"ContainerObserverDEF.SyncSceneSnapshot2Client() SceneName={scene_name}");

            Scene.New(Client.EventContext, scene_name, ref entity_data, Client);

            return Task.CompletedTask;
        }

        Task IContainerObserverDEF.SyncEntitySnapshot2Client(string scene_name, EntityData entity_data)
        {
            Client.MapScene.TryGetValue(scene_name, out var scene);
            if (scene == null)
            {
                // todo，log error

                return Task.CompletedTask;
            }

            scene.CreateEntity(ref entity_data);

            return Task.CompletedTask;
        }

        Task IContainerObserverDEF.SyncDelta2Client(string scene_name, byte[] data)
        {
            Client.MapScene.TryGetValue(scene_name, out var scene);
            if (scene != null)
            {
                return SyncDelta2Client(scene, MapDirty, data);
            }

            return Task.CompletedTask;
        }

        Task SyncDelta2Client(Scene scene, Dictionary<long, List<DirtyComponentInfo1>> map_dirty, byte[] data)
        {
            //Debug.Log($"服务端推送差异集");

            if (data == null)
            {
                return Task.CompletedTask;
            }

            var binlog_info = (NetworkSyncBinlog)EntitySerializer.Deserialize<NetworkSyncBinlog>(scene.SerializerType, data);

            if (binlog_info == null)
            {
                return Task.CompletedTask;
            }

            if (binlog_info.ListOp == null || binlog_info.ListOp.Count == 0)
            {
                return Task.CompletedTask;
            }

            if (scene == null)
            {
                return Task.CompletedTask;
            }

            foreach (var i in binlog_info.ListOp)
            {
                // 0=UpdateState, 1=RemoveEntity, 2=AddEntity
                if (i.Op == BinlogOpType.UpdateState)
                {
                    var update_state = binlog_info.ListUpdateState[i.Index];

                    //Debug.Log($"UpdateState EntityId={update_state.EntityId} Name={update_state.Name} Key={update_state.Key} Value={update_state.Value}");

                    var c = scene.FindComponent(update_state.EntityId, update_state.Name);
                    if (c != null)
                    {
                        var state = c.GetState();
                        if (state != null)
                        {
                            state.ApplyDirtyState(update_state.Key, update_state.Value);
                        }
                    }
                }
                else if (i.Op == BinlogOpType.DestroyEntity)
                {
                    var remove_entity = binlog_info.ListRemoveEntity[i.Index];

                    //UnityEngine.Debug.Log($"SceneName={scene.Name}, RemoveEntity={entity_id}");

                    scene.RemoveEntity(remove_entity.EntityId, remove_entity.Reason, remove_entity.UserData);
                }
                else if (i.Op == BinlogOpType.AddEntity)
                {
                    var add_entity = binlog_info.ListAddEntity[i.Index];

                    //UnityEngine.Debug.Log($"SceneName={scene.Name}, AddEntity={add_entity.EntityData.Name}");

                    Entity parent = null;
                    if (add_entity.ParentId > 0)
                    {
                        parent = scene.FindEntity(add_entity.ParentId);
                    }
                    scene.CreateEntity(ref add_entity.EntityData, parent);
                }
                else if (i.Op == BinlogOpType.ChangeEntityParent)
                {
                    var change_entity_parent = binlog_info.ListChangeEntityParent[i.Index];

                    var et = scene.FindEntity(change_entity_parent.EntityId);
                    if (et != null)
                    {
                        var parent = scene.FindEntity(change_entity_parent.NewParentId);
                        et.ChangeParent(parent);
                    }
                }
                else if (i.Op == BinlogOpType.CustomStateOp)
                {
                    var custom_state_op = binlog_info.ListCustomState[i.Index];

                    var c = scene.FindComponent(custom_state_op.EntityId, custom_state_op.ComponentName);
                    if (c != null)
                    {
                        var state = c.GetState();
                        if (state != null)
                        {
                            state.ApplyDirtyCustomState(custom_state_op.StateKey, custom_state_op.Cmd, custom_state_op.Param);
                        }
                    }
                }
                else if (i.Op == BinlogOpType.CustomOp)
                {
                    var custom_op = binlog_info.ListCustom[i.Index];

                    //if (custom_op.Cmd == "LevelNormalDestroyOre")
                    //{
                    //    string et_name = custom_op.Param1;
                    //    var et = Entity.FindChild("LevelNormal");
                    //    if (et != null)
                    //    {
                    //        var et_ore = et.FindChild(et_name);
                    //        if (et_ore != null)
                    //        {
                    //            et_ore.Destroy();
                    //            et_ore = null;
                    //        }
                    //    }
                    //}
                    //else if (custom_op.Cmd == "LevelActivityDestroyOre")
                    //{
                    //    string et_name = custom_op.Param1;
                    //    var et = Entity.FindChild("LevelActivity");
                    //    if (et != null)
                    //    {
                    //        var et_ore = et.FindChild(et_name);
                    //        if (et_ore != null)
                    //        {
                    //            et_ore.Destroy();
                    //            et_ore = null;
                    //        }
                    //    }
                    //}
                }
                else if (i.Op == BinlogOpType.AddEntityTag)
                {
                    var add_entitytag = binlog_info.ListAddEntityTag[i.Index];

                    Entity et = scene.FindEntity(add_entitytag.EntityId);
                    if (et != null)
                    {
                        var tag = TagManager.FindTag(add_entitytag.Tag);

                        if (tag != null)
                        {
                            et.AddSyncTag(tag);
                        }
                        else
                        {
                            // log error
                        }
                    }
                }
                else if (i.Op == BinlogOpType.RemoveEntityTag)
                {
                    var remove_entitytag = binlog_info.ListRemoveEntityTag[i.Index];

                    Entity et = scene.FindEntity(remove_entitytag.EntityId);
                    if (et != null)
                    {
                        var tag = TagManager.FindTag(remove_entitytag.Tag);

                        if (tag != null)
                        {
                            et.RemoveSyncTag(tag);
                        }
                        else
                        {
                            // log error
                        }
                    }
                }
                else if (i.Op == BinlogOpType.SetEntityTagValue)
                {
                    var set_entitytag_value = binlog_info.ListSetEntityTagValue[i.Index];

                    Entity et = scene.FindEntity(set_entitytag_value.EntityId);
                    if (et != null)
                    {
                        var tag = TagManager.FindTag(set_entitytag_value.Tag);

                        if (tag != null)
                        {
                            et.SetSyncTagValue(tag, set_entitytag_value.Value);
                        }
                        else
                        {
                            // log error
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}

#endif