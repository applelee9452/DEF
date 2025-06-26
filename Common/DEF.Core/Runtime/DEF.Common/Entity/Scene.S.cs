#if !DEF_CLIENT

using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF
{
    public sealed partial class Scene
    {
        public IService Service { get; private set; }
        public ILogger Logger { get; private set; }
        internal bool SyncFlagDefault { get; set; } = false;
        internal IdGenerator.IIdGenerator IdGen { get; private set; }

        public Scene(string scene_name,
            string container_type,
            string container_id,
            IService entitymodule_listener_4_service,
            ILogger logger)
        {
            var idgenerator_options = new IdGenerator.IdGeneratorOptions()
            {
                Method = 1,
                WorkerId = 1,
                WorkerIdBitLength = 6,
                SeqBitLength = 12,
                //TopOverCostCount = 2000,
                //DataCenterIdBitLength = 0,
                //TimestampType = 1,
                // MinSeqNumber = 1,
                // MaxSeqNumber = 200,
                // BaseTime = DateTime.Now.AddYears(-10),
            };

            IdGen = new IdGenerator.DefaultIdGenerator(idgenerator_options);

            ContainerType = container_type;
            ContainerId = container_id;
            MapAllEntity = new Dictionary<long, Entity>();
            EventContext = new EventContext();
            NodeGraphContext = new NodeGraphContext();

            RootEntity = new Entity(scene_name, null, this, null, null);
            RootEntity.Start();

            Service = entitymodule_listener_4_service;
            IsServer = true;
            Logger = logger;
        }

        public Entity CreateEntity(MongoDB.Bson.BsonDocument entity_data)
        {
            var et = new Entity(entity_data, RootEntity, this);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public Entity CreateEntity(MongoDB.Bson.BsonDocument entity_data, Entity parent)
        {
            var et = new Entity(entity_data, parent, this);

#if !DEF_CLIENT
            if (et.NetworkSyncFlag)
            {
                WriteNetworkSyncBinlogAddEntity(et.ClientSubFilter, et.Parent != null ? et.Parent.Id : 0, et.GetEntityData(et.ClientSubFilter));
            }
#endif

            et.Start();

            return et;
        }

        public static Scene Load(string scene_name, string container_type, string container_id, IService service, ILogger logger)
        {
            return new Scene(scene_name, container_type, container_id, service, logger);
        }

        public static Scene New(string scene_name, string container_type, string container_id, IService service, ILogger logger)
        {
            return new Scene(scene_name, container_type, container_id, service, logger);
        }

        public Task DispatchEntityRpcNoResult(
            long entity_id, string component_name, string method_name)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, null);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1>(
            long entity_id, string component_name, string method_name, T1 param1)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1 };

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1, T2>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2 };

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1, T2, T3>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3 };

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1, T2, T3, T4>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3, param4 };

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1, T2, T3, T4, T5>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = [param1, param2, param3, param4, param5];

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1, T2, T3, T4, T5, T6>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6];

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7];

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7, param8];

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7, param8, param9];

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_obj);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return Task.CompletedTask;
        }

        public Task DispatchEntityRpcNoResult2(
            long entity_id, string component_name, string method_name, byte[] method_data)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return Task.CompletedTask;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m == null)
            {
                //Logger.LogError($"ContainerRpc2 Error, Not Exist MethodName={method_name}!");
                return Task.CompletedTask;
            }

            Config config = null;
#if !DEF_CLIENT
            config = Service.Config;
#else
            config = Client.Config;
#endif

            object[] arr_param = null;
            var p = m.GetParameters();

            if (method_data == null && p.Length == 0)
            {
                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, null);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
            else
            {
                method_data ??= new byte[0];

                if (p.Length == 1)
                {
                    var so_t = typeof(SerializeObj<>).MakeGenericType(
                        p[0].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 2)
                {
                    var so_t = typeof(SerializeObj<,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 3)
                {
                    var so_t = typeof(SerializeObj<,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 4)
                {
                    var so_t = typeof(SerializeObj<,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 5)
                {
                    var so_t = typeof(SerializeObj<,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 6)
                {
                    var so_t = typeof(SerializeObj<,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 7)
                {
                    var so_t = typeof(SerializeObj<,,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType,
                        p[6].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 8)
                {
                    var so_t = typeof(SerializeObj<,,,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType,
                        p[6].ParameterType,
                        p[7].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 9)
                {
                    var so_t = typeof(SerializeObj<,,,,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType,
                        p[6].ParameterType,
                        p[7].ParameterType,
                        p[8].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }

                dynamic rr = null;

                try
                {
                    rr = m.Invoke(c, arr_param);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return rr;
            }
        }

        public async Task<TResult> DispatchEntityRpc<TResult>(
            long entity_id, string component_name, string method_name)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, null);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, TResult>(
            long entity_id, string component_name, string method_name, T1 param1)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, T2, TResult>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, T2, T3, TResult>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, T2, T3, T4, TResult>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3, param4 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, T2, T3, T4, T5, TResult>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3, param4, param5 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, T2, T3, T4, T5, T6, TResult>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3, param4, param5, param6 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3, param4, param5, param6, param7 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3, param4, param5, param6, param7, param8 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<TResult> DispatchEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return default;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return default;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m != null)
            {
                object[] arr_obj = new object[] { param1, param2, param3, param4, param5, param6, param7, param8, param9 };

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_obj);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");
                }

                return result;
            }
            else
            {
                //Logger.LogError($"ContainerRpc Error, Not Exist MethodName={method_name}!");
            }

            return default;
        }

        public async Task<byte[]> DispatchEntityRpc2(
            long entity_id, string component_name, string method_name, byte[] method_data)
        {
            MapAllEntity.TryGetValue(entity_id, out var et);
            if (et == null)
            {
                // todo，log error
                return null;
            }

            var c = et.GetComponent(component_name);
            if (c == null)
            {
                // todo，log error
                return null;
            }

            c.MapEntityMethod.TryGetValue(method_name, out var m);
            if (m == null)
            {
                //Logger.LogError($"ContainerRpc2 Error, Not Exist MethodName={method_name}!");
                return null;
            }

            Config config = null;
#if !DEF_CLIENT
            config = Service.Config;
#else
            config = Client.Config;
#endif

            object[] arr_param = null;
            var p = m.GetParameters();

            if (method_data == null && p.Length == 0)
            {
                try
                {
                    dynamic rr = m.Invoke(c, null);

                    await rr;

                    var result = rr.Result;

                    if (result == null)
                    {
                        return default;
                    }

                    return EntitySerializer.Serialize(config.SerializerType, result);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");

                    return default;
                }
            }
            else
            {
                method_data ??= new byte[0];

                if (p.Length == 1)
                {
                    var so_t = typeof(SerializeObj<>).MakeGenericType(
                        p[0].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 2)
                {
                    var so_t = typeof(SerializeObj<,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 3)
                {
                    var so_t = typeof(SerializeObj<,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 4)
                {
                    var so_t = typeof(SerializeObj<,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 5)
                {
                    var so_t = typeof(SerializeObj<,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 6)
                {
                    var so_t = typeof(SerializeObj<,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 7)
                {
                    var so_t = typeof(SerializeObj<,,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType,
                        p[6].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 8)
                {
                    var so_t = typeof(SerializeObj<,,,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType,
                        p[6].ParameterType,
                        p[7].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 9)
                {
                    var so_t = typeof(SerializeObj<,,,,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType,
                        p[6].ParameterType,
                        p[7].ParameterType,
                        p[8].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(config.SerializerType, so_t, method_data);
                }

                dynamic result = null;

                try
                {
                    dynamic rr = m.Invoke(c, arr_param);

                    await rr;

                    result = rr.Result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error");

                    return default;
                }

                if (result == null)
                {
                    return default;
                }

                return EntitySerializer.Serialize(config.SerializerType, result);
            }
        }

        // 该Entity的Id等于Scene.ContainerId
        public Task<Entity> CreateEntityFromDb(IMongoDatabase db, string collection_name)
        {
            return CreateEntityFromDb(db, collection_name, RootEntity, ContainerId);
        }

        // 该Entity的Id等于Scene.ContainerId
        public Task<Entity> CreateEntityFromDb(IMongoDatabase db, string collection_name, string id)
        {
            return CreateEntityFromDb(db, collection_name, RootEntity, id);
        }

        // 该Entity的Id等于Scene.ContainerId
        public async Task<Entity> CreateEntityFromDb(IMongoDatabase db, string collection_name, Entity parent, string id)
        {
            var collection = db.GetCollection<BsonDocument>(collection_name);
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("_id", id);

            var entity_data = await collection.Find(filter).FirstOrDefaultAsync();
            if (entity_data == null)
            {
                return null;
            }

            return CreateEntity(entity_data, parent);
        }
    }
}

#endif