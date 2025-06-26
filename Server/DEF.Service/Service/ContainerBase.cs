using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DEF;

public class ContainerBase
{
    public ILogger Logger { get; private set; }
    public Service Service { get; private set; }
    public IGrainFactory GrainFactory { get; private set; }
    public Scene Scene { get; private set; }
    protected Dictionary<string, MethodInfo> MapContainerMethod { get; set; } = [];

    public void Setup2(ILogger logger, string container_type, string container_id, IService service, IGrainFactory grain_factory)
    {
        Logger = logger;
        Service = (Service)service;
        GrainFactory = grain_factory;

        var t = GetType();
        var arr = t.GetInterfaces();

        if (arr != null)
        {
            foreach (var i in arr)
            {
                var m = ((IService)Service).GetContainerRpcCallee(i.Name);
                if (m != null)
                {
                    MapContainerMethod = m;
                    break;
                }
            }
        }

        Scene = Scene.New(container_type, container_type, container_id, service, Logger);
    }

    public Scene NewScene(string scene_name, string container_id)
    {
        return Scene.New(scene_name, scene_name, container_id, Service, Logger);
    }

    public TInterface GetContainerRpc<TInterface>() where TInterface : IContainerRpc
    {
        return GetContainerRpc<TInterface>(string.Empty);
    }

    public TInterface GetContainerRpc<TInterface>(string container_id) where TInterface : IContainerRpc
    {
        return ((IService)Service).GetContainerRpc<TInterface>(container_id);
    }

    public TInterface GetContainerRpcObserver<TInterface>(string gateway_guid, string session_guid) where TInterface : IContainerRpcObserver
    {
        return ((IService)Service).GetContainerObserverRpc<TInterface>(gateway_guid, session_guid);
    }

    public Task OnContainerRpcNoResult(
        string method_name)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                dynamic rr = m.Invoke(this, null);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1>(
        string method_name, T1 param1)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1, T2>(
        string method_name, T1 param1, T2 param2)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1, T2, T3>(
        string method_name, T1 param1, T2 param2, T3 param3)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1, T2, T3, T4>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1, T2, T3, T4, T5>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7, param8];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7, param8, param9];

                dynamic rr = m.Invoke(this, arr_obj);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpcNoResult Error, Not Exist MethodName={method_name}!", method_name);
        }

        return Task.CompletedTask;
    }

    public Task OnContainerRpcNoResult2(string method_name, byte[] method_data)
    {
        //Logger.LogInformation($"ContainerBase.OnContainerRpcNoResult2() ContainerId={ContainerId} MethodName={method_name}");

        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m == null)
        {
            Logger.LogError("OnContainerRpc2 Error, Not Exist MethodName={method_name}!", method_name);
            return Task.CompletedTask;
        }

        object[] arr_param = null;
        var p = m.GetParameters();

        if (method_data == null && p.Length == 0)
        {
            try
            {
                dynamic rr = m.Invoke(this, null);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult2 Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            try
            {
                if (method_data == null)
                {
                    method_data = new byte[0];
                }

                if (p.Length == 1)
                {
                    var so_t = typeof(SerializeObj<>).MakeGenericType(
                        p[0].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 2)
                {
                    var so_t = typeof(SerializeObj<,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 3)
                {
                    var so_t = typeof(SerializeObj<,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 4)
                {
                    var so_t = typeof(SerializeObj<,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 5)
                {
                    var so_t = typeof(SerializeObj<,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
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

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
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

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
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

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
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

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }

                dynamic rr = m.Invoke(this, arr_param);

                return rr;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpcNoResult2 Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }

        return Task.CompletedTask;
    }

    public async Task<TResult> OnContainerRpc<TResult>(
        string method_name)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                dynamic rr = m.Invoke(this, null);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, TResult>(
        string method_name, T1 param1)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, T2, TResult>(
        string method_name, T1 param1, T2 param2)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, T2, T3, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, T2, T3, T4, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7, param8];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m != null)
        {
            try
            {
                object[] arr_obj = [param1, param2, param3, param4, param5, param6, param7, param8, param9];

                dynamic rr = m.Invoke(this, arr_obj);

                await rr;

                var result = rr.Result;

                return result;
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            Logger.LogError("OnContainerRpc Error, Not Exist MethodName={method_name}!", method_name);
        }

        return default;
    }

    public async Task<byte[]> OnContainerRpc2(string method_name, byte[] method_data)
    {
        MapContainerMethod.TryGetValue(method_name, out var m);
        if (m == null)
        {
            Logger.LogError("OnContainerRpc2 Error, Not Exist MethodName={method_name}!", method_name);
            return null;
        }

        object[] arr_param = null;
        var p = m.GetParameters();

        if (method_data == null && p.Length == 0)
        {
            try
            {
                dynamic rr = m.Invoke(this, null);

                await rr;

                var result = rr.Result;

                if (result == null)
                {
                    return default;
                }

                return EntitySerializer.Serialize(Service.Config.SerializerType, result);
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc2 Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }
        else
        {
            try
            {
                if (method_data == null)
                {
                    method_data = new byte[0];
                }

                if (p.Length == 1)
                {
                    var so_t = typeof(SerializeObj<>).MakeGenericType(
                        p[0].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 2)
                {
                    var so_t = typeof(SerializeObj<,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 3)
                {
                    var so_t = typeof(SerializeObj<,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 4)
                {
                    var so_t = typeof(SerializeObj<,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 5)
                {
                    var so_t = typeof(SerializeObj<,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
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

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
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

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
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

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }
                else if (p.Length == 9)
                {
                    var so_t = typeof(SerializeObj<,,,,,,,>).MakeGenericType(
                        p[0].ParameterType,
                        p[1].ParameterType,
                        p[2].ParameterType,
                        p[3].ParameterType,
                        p[4].ParameterType,
                        p[5].ParameterType,
                        p[6].ParameterType,
                        p[7].ParameterType,
                        p[8].ParameterType);

                    arr_param = EntitySerializer.DeserializeObj(Service.Config.SerializerType, so_t, method_data);
                }

                dynamic rr = m.Invoke(this, arr_param);

                await rr;

                var result = rr.Result;

                if (result == null)
                {
                    return default;
                }

                return EntitySerializer.Serialize(Service.Config.SerializerType, result);
            }
            catch (Exception ex)
            {
                Logger.LogError("OnContainerRpc2 Exception, \n{Message} \n{StackTrace}!", ex.Message, ex.StackTrace);
            }
        }

        return null;
    }

    public Task OnEntityRpcNoResult(
        long entity_id, string component_name, string method_name)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name);
    }

    public Task OnEntityRpcNoResult<T1>(
        long entity_id, string component_name, string method_name,
        T1 param1)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1);
    }

    public Task OnEntityRpcNoResult<T1, T2>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1, param2);
    }

    public Task OnEntityRpcNoResult<T1, T2, T3>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1, param2, param3);
    }

    public Task OnEntityRpcNoResult<T1, T2, T3, T4>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1, param2, param3, param4);
    }

    public Task OnEntityRpcNoResult<T1, T2, T3, T4, T5>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5);
    }

    public Task OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6);
    }

    public Task OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7);
    }

    public Task OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7, param8);
    }

    public Task OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        return Scene.DispatchEntityRpcNoResult(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7, param8, param9);
    }

    public Task OnEntityRpcNoResult2(
        long entity_id, string component_name, string method_name, byte[] method_data)
    {
        return Scene.DispatchEntityRpcNoResult2(entity_id, component_name, method_name, method_data);
    }

    public Task<TResult> OnEntityRpc<TResult>(
        long entity_id, string component_name, string method_name)
    {
        return Scene.DispatchEntityRpc<TResult>(
            entity_id, component_name, method_name);
    }

    public Task<TResult> OnEntityRpc<T1, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1)
    {
        return Scene.DispatchEntityRpc<T1, TResult>(
            entity_id, component_name, method_name, param1);
    }

    public Task<TResult> OnEntityRpc<T1, T2, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2)
    {
        return Scene.DispatchEntityRpc<T1, T2, TResult>(
            entity_id, component_name, method_name, param1, param2);
    }

    public Task<TResult> OnEntityRpc<T1, T2, T3, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3)
    {
        return Scene.DispatchEntityRpc<T1, T2, T3, TResult>(
            entity_id, component_name, method_name, param1, param2, param3);
    }

    public Task<TResult> OnEntityRpc<T1, T2, T3, T4, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4)
    {
        return Scene.DispatchEntityRpc<T1, T2, T3, T4, TResult>(
            entity_id, component_name, method_name, param1, param2, param3, param4);
    }

    public Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        return Scene.DispatchEntityRpc<T1, T2, T3, T4, T5, TResult>(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5);
    }

    public Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, T6, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        return Scene.DispatchEntityRpc<T1, T2, T3, T4, T5, T6, TResult>(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6);
    }

    public Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        return Scene.DispatchEntityRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7);
    }

    public Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        return Scene.DispatchEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7, param8);
    }

    public Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        long entity_id, string component_name, string method_name,
        T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        return Scene.DispatchEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
            entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7, param8, param9);
    }

    public Task<byte[]> OnEntityRpc2(
        long entity_id, string component_name, string method_name, byte[] method_data)
    {
        return Scene.DispatchEntityRpc2(entity_id, component_name, method_name, method_data);
    }
}