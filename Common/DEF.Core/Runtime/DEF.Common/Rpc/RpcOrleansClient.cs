#if !DEF_CLIENT

using Orleans;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEF;

public class RpcOrleansClient
{
    readonly IService Service;
    IClusterClient OrleansClient;

    public RpcOrleansClient(IService service, IClusterClient orleans_client)
    {
        Service = service;
        OrleansClient = orleans_client;
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
}

#endif
