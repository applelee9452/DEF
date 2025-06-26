#if !DEF_CLIENT

using Orleans;
using System.Threading.Tasks;

namespace DEF;

public class RpcOrleansGrainFactory
{
    readonly IService Service;
    IGrainFactory GrainFactory;

    public RpcOrleansGrainFactory(IService service, IGrainFactory grain_factory)
    {
        Service = service;
        GrainFactory = grain_factory;
    }

    Task<TResult> ContainerRpc<TResult>(
        string method_name)
    {
        return default;
    }

    Task<TResult> ContainerRpc<T1, TResult>(
        string method_name, T1 param1)
    {
        return default;
    }

    Task<TResult> ContainerRpc<T1, T2, TResult>(
        string method_name, T1 param1, T2 param2)
    {
        return default;
    }

    Task<TResult> ContainerRpc<T1, T2, T3, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3)
    {
        return default;
    }

    Task<TResult> ContainerRpc<T1, T2, T3, T4, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        return default;
    }

    Task<TResult> ContainerRpc<T1, T2, T3, T4, T5, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        return default;
    }

    Task<TResult> EntityRpc<TResult>(
        long entity_id, string component_name, string method_name)
    {
        return default;
    }

    Task<TResult> EntityRpc<T1, TResult>(
        long entity_id, string component_name, string method_name, T1 param1)
    {
        return default;
    }

    Task<TResult> EntityRpc<T1, T2, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2)
    {
        return default;
    }

    Task<TResult> EntityRpc<T1, T2, T3, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3)
    {
        return default;
    }

    Task<TResult> EntityRpc<T1, T2, T3, T4, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        return default;
    }

    Task<TResult> EntityRpc<T1, T2, T3, T4, T5, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        return default;
    }

    async Task<TResult> RequestResponse<TResult>(int container_type, string container_id,
        long entity_id, string component_name,
        string method_name, SerializeObj so)
    {
        byte[] method_data = null;
        if (so != null)
        {
            method_data = EntitySerializer.Serialize(Service.Config.SerializerType, so);
        }

        byte[] response_data = null;
        if (container_type == 0)
        {
            var grain = GrainFactory.GetGrain<IGrainContainerStateless>(container_id);
            response_data = await grain.OnContainerRpc2(method_name, method_data);
        }
        else if (entity_id > 0 && !string.IsNullOrEmpty(component_name))
        {
            var grain = GrainFactory.GetGrain<IGrainContainerStateful>(container_id);
            response_data = await grain.OnEntityRpc2(entity_id, component_name, method_name, method_data);
        }
        else
        {
            var grain = GrainFactory.GetGrain<IGrainContainerStateful>(container_id);
            response_data = await grain.OnContainerRpc2(method_name, method_data);
        }

        if (response_data == null)
        {
            return default;
        }

        var obj = EntitySerializer.Deserialize<SerializeObj<TResult>>(Service.Config.SerializerType, response_data);
        return obj.obj1;
    }
}

#endif
