#if !DEF_CLIENT

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEF
{
    public sealed partial class Scene
    {
        public enum SessionFilterType
        {
            All = 0,// 广播给所有Client
            One,// 广播给指定Client
            List,// 广播给指定Client列表
        }

        public class ClientFilterInfo
        {
            public SessionFilterType FilterType { get; set; }
            public string OneSessionGuid { get; set; }
            public List<string> ListSessionGuid { get; set; } = [];

            public void Clear()
            {
                FilterType = SessionFilterType.All;
                OneSessionGuid = string.Empty;
                ListSessionGuid.Clear();
            }
        }

        public class NetworkSyncInfo
        {
            public ClientFilterInfo FilterInfo { get; set; } = new();
            public NetworkSyncBinlog Binlog { get; set; } = new();
        }

        public bool NetworkSyncFlag { get; set; } = false;// 开关，是否收集网络变更
        internal Dictionary<string, string> MapClientSub { get; private set; }// 所有订阅的Client信息, Key=SessionGuid, Value=GatewayGuid
        internal List<NetworkSyncInfo> ListNetworkSync { get; private set; } = new();
        internal Queue<NetworkSyncInfo> QueNetworkSyncPool { get; private set; } = new();

        // 0表示成功同步但是没有可同步包，>0表示成功同步了n个包，-1表示同步失败
        public async Task<int> SyncDelta2Client()
        {
            if (ListNetworkSync.Count == 0)
            {
                return 0;
            }

            int count = ListNetworkSync.Count;

            while (ListNetworkSync.Count > 0)
            {
                var info = ListNetworkSync[0];
                ListNetworkSync.RemoveAt(0);

                if (MapClientSub != null)
                {
                    try
                    {
                        byte[] data = EntitySerializer.Serialize(SerializerType, info.Binlog);

                        if (info.FilterInfo.FilterType == SessionFilterType.All)
                        {
                            List<Task> list_task = new(MapClientSub.Count);
                            foreach (var i in MapClientSub)
                            {
                                if (!string.IsNullOrEmpty(i.Key) && !string.IsNullOrEmpty(i.Value))
                                {
                                    //Logger.LogWarning("Scene.SyncDelta2ClientAll() Index={Index} GatewayGuid={GatewayGuid}, SessionGuid={SessionGuid}", list_task.Count, i.Value, i.Key);

                                    var ob = Service.GetContainerObserverRpc<IContainerObserverDEF>(i.Value, i.Key);
                                    var t = ob.SyncDelta2Client(Name, data);
                                    list_task.Add(t);
                                }
                            }
                            if (list_task.Count > 0)
                            {
                                await Task.WhenAll(list_task);
                            }
                        }
                        else if (info.FilterInfo.FilterType == SessionFilterType.One)
                        {
                            if (MapClientSub.TryGetValue(info.FilterInfo.OneSessionGuid, out var gateway_guid))
                            {
                                if (!string.IsNullOrEmpty(info.FilterInfo.OneSessionGuid) && !string.IsNullOrEmpty(gateway_guid))
                                {
                                    //Logger.LogWarning("Scene.SyncDelta2ClientOne() GatewayGuid={GatewayGuid}, SessionGuid={SessionGuid}", gateway_guid, info.FilterInfo.OneSessionGuid);

                                    var ob = Service.GetContainerObserverRpc<IContainerObserverDEF>(gateway_guid, info.FilterInfo.OneSessionGuid);
                                    await ob.SyncDelta2Client(Name, data);
                                }
                            }
                        }
                        else if (info.FilterInfo.FilterType == SessionFilterType.List)
                        {
                        }
                    }
                    catch (Orleans.Runtime.ClientNotAvailableException ex)
                    {
                        Logger.LogError(ex, "Scene.SyncDelta2Client()");

                        return -1;
                    }
                    catch (System.Exception ex)
                    {
                        Logger.LogError(ex, "Scene.SyncDelta2Client()");

                        return -1;
                    }
                }

                info.FilterInfo.Clear();
                info.Binlog.Clear();
                QueNetworkSyncPool.Enqueue(info);
            }

            return count;
        }

        public void AddSubClient(string session_guid, string gateway_guid)
        {
            if (string.IsNullOrEmpty(gateway_guid) || string.IsNullOrEmpty(session_guid))
            {
                return;
            }

            MapClientSub ??= [];
            MapClientSub[session_guid] = gateway_guid;
        }

        public void RemoveSubClient(string session_guid)
        {
            if (string.IsNullOrEmpty(session_guid))
            {
                return;
            }

            MapClientSub?.Remove(session_guid);
        }

        public string TryGetSubClientGatewayGuid(string session_guid)
        {
            if (string.IsNullOrEmpty(session_guid))
            {
                return string.Empty;
            }

            if (MapClientSub == null)
            {
                return string.Empty;
            }

            MapClientSub.TryGetValue(session_guid, out var gateway_guid);

            return gateway_guid;
        }

        public void ClearAllSubClient()
        {
            MapClientSub?.Clear();
        }

        public void ClearNetworkSyncInfo()
        {
            if (ListNetworkSync.Count == 0)
            {
                return;
            }

            while (ListNetworkSync.Count > 0)
            {
                var info = ListNetworkSync[0];
                ListNetworkSync.RemoveAt(0);

                info.FilterInfo.Clear();
                info.Binlog.Clear();
                QueNetworkSyncPool.Enqueue(info);
            }
        }

        public void WriteNetworkSyncBinlogAddEntity(string one_session_guid, long parent_id, EntityData? entity_data)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            if (entity_data == null)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogAddEntity item = new()
            {
                ParentId = parent_id,
                EntityData = (EntityData)entity_data
            };
            last.Binlog.ListAddEntity.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.AddEntity,
                Index = (ushort)(last.Binlog.ListAddEntity.Count - 1)
            };
            last.Binlog.ListOp.Add(op);

            //Logger.LogInformation($"AddEntity EntityId={entity_data?.Id}");
        }

        public void WriteNetworkSyncBinlogRemoveEntity(string one_session_guid, long entity_id, string reason, byte[] user_data)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogDestroyEntity item = new()
            {
                EntityId = entity_id,
                Reason = reason,
                UserData = user_data,
            };
            last.Binlog.ListRemoveEntity.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.DestroyEntity,
                Index = (ushort)(last.Binlog.ListRemoveEntity.Count - 1)
            };
            last.Binlog.ListOp.Add(op);

            //Logger.LogInformation($"RemoveEntity EntityId={entity_id}");
        }

        public void WriteNetworkSyncBinlogChangeEntityParent(string one_session_guid, long entity_id, long new_parent_id)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogChangeEntityParent item = new()
            {
                EntityId = entity_id,
                NewParentId = new_parent_id,
            };
            last.Binlog.ListChangeEntityParent.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.ChangeEntityParent,
                Index = (ushort)(last.Binlog.ListChangeEntityParent.Count - 1)
            };
            last.Binlog.ListOp.Add(op);

            //Logger.LogInformation($"ChangeEntityParent EntityId={entity_id}");
        }

        // State更新，一次写一个变量
        public void WriteNetworkSyncBinlogUpdateState(string one_session_guid, long entity_id, string com_name, string key, byte[] value)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogUpdateState item = new()
            {
                EntityId = entity_id,
                Name = com_name,
                Key = key,
                Value = value
            };
            last.Binlog.ListUpdateState.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.UpdateState,
                Index = (ushort)(last.Binlog.ListUpdateState.Count - 1)
            };
            last.Binlog.ListOp.Add(op);
        }

        public void WriteNetworkSyncBinlogCumstomState(string one_session_guid, long entity_id, string component_name, string state_key, byte cmd, byte[] param)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogCustomState item = new()
            {
                EntityId = entity_id,
                ComponentName = component_name,
                StateKey = state_key,
                Cmd = cmd,
                Param = param,
            };
            last.Binlog.ListCustomState.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.CustomStateOp,
                Index = (ushort)(last.Binlog.ListCustomState.Count - 1)
            };
            last.Binlog.ListOp.Add(op);
        }

        public void WriteNetworkSyncBinlogCumstom(string one_session_guid, string cmd, string param1, byte[] param2)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogCustom item = new()
            {
                Cmd = cmd,
                Param1 = param1,
                Param2 = param2
            };
            last.Binlog.ListCustom.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.CustomOp,
                Index = (ushort)(last.Binlog.ListCustom.Count - 1)
            };
            last.Binlog.ListOp.Add(op);
        }

        public void WriteNetworkSyncBinlogAddEntityTag(string one_session_guid, long entity_id, int tag)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogAddEntityTag item = new()
            {
                EntityId = entity_id,
                Tag = tag
            };
            last.Binlog.ListAddEntityTag.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.AddEntityTag,
                Index = (ushort)(last.Binlog.ListAddEntityTag.Count - 1)
            };
            last.Binlog.ListOp.Add(op);
        }

        public void WriteNetworkSyncBinlogRemoveEntityTag(string one_session_guid, long entity_id, int tag)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogRemoveEntityTag item = new()
            {
                EntityId = entity_id,
                Tag = tag
            };
            last.Binlog.ListRemoveEntityTag.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.RemoveEntityTag,
                Index = (ushort)(last.Binlog.ListRemoveEntityTag.Count - 1)
            };
            last.Binlog.ListOp.Add(op);
        }

        public void WriteNetworkSyncBinlogSetEntityTagValue(string one_session_guid, long entity_id, int tag, byte[] v)
        {
            if (!NetworkSyncFlag)
            {
                return;
            }

            DealCurrentNetworkSync(one_session_guid);

            var last = ListNetworkSync[^1];

            BinlogSetEntityTagValue item = new()
            {
                EntityId = entity_id,
                Tag = tag,
                Value = v
            };
            last.Binlog.ListSetEntityTagValue.Add(item);

            BinlogOp op = new()
            {
                Op = BinlogOpType.SetEntityTagValue,
                Index = (ushort)(last.Binlog.ListSetEntityTagValue.Count - 1)
            };
            last.Binlog.ListOp.Add(op);
        }

        void DealCurrentNetworkSync(string one_session_guid)
        {
            NetworkSyncInfo last;

            if (ListNetworkSync.Count == 0)
            {
                if (QueNetworkSyncPool.Count > 0)
                {
                    NetworkSyncInfo info = QueNetworkSyncPool.Dequeue();
                    ListNetworkSync.Add(info);
                    last = info;
                }
                else
                {
                    NetworkSyncInfo info = new();
                    ListNetworkSync.Add(info);
                    last = info;
                }

                if (string.IsNullOrEmpty(one_session_guid))
                {
                    last.FilterInfo.FilterType = SessionFilterType.All;
                }
                else
                {
                    last.FilterInfo.FilterType = SessionFilterType.One;
                    last.FilterInfo.OneSessionGuid = one_session_guid;
                }
            }
            else
            {
                last = ListNetworkSync[^1];
            }

            bool need_next_item = false;
            if (string.IsNullOrEmpty(one_session_guid))
            {
                if (last.FilterInfo.FilterType != SessionFilterType.All)
                {
                    need_next_item = true;
                }
            }
            else
            {
                if (last.FilterInfo.FilterType == SessionFilterType.All)
                {
                    need_next_item = true;
                }
            }

            if (!string.IsNullOrEmpty(one_session_guid))
            {
                if (last.FilterInfo.OneSessionGuid != one_session_guid)
                {
                    need_next_item = true;
                }
            }

            if (need_next_item)
            {
                if (QueNetworkSyncPool.Count > 0)
                {
                    NetworkSyncInfo info = QueNetworkSyncPool.Dequeue();
                    ListNetworkSync.Add(info);
                    last = info;
                }
                else
                {
                    NetworkSyncInfo info = new();
                    ListNetworkSync.Add(info);
                    last = info;
                }

                if (string.IsNullOrEmpty(one_session_guid))
                {
                    last.FilterInfo.FilterType = SessionFilterType.All;
                }
                else
                {
                    last.FilterInfo.FilterType = SessionFilterType.One;
                    last.FilterInfo.OneSessionGuid = one_session_guid;
                }
            }
        }
    }
}

#endif