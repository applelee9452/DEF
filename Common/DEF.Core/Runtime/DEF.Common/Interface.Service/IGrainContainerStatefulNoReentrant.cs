#if !DEF_CLIENT

using Orleans;
using System.Threading.Tasks;

namespace DEF;

public interface IGrainContainerStatefulNoReentrant : IGrainWithStringKey
{
    Task Touch();

    Task OnContainerRpcNoResult(string method_name);

    Task OnContainerRpcNoResult<T1>(
        string method_name, T1 param1);

    Task OnContainerRpcNoResult<T1, T2>(
        string method_name, T1 param1, T2 param2);

    Task OnContainerRpcNoResult<T1, T2, T3>(
        string method_name, T1 param1, T2 param2, T3 param3);

    Task OnContainerRpcNoResult<T1, T2, T3, T4>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4);

    Task OnContainerRpcNoResult<T1, T2, T3, T4, T5>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);

    Task OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

    Task OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

    Task OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

    Task OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9);

    Task OnContainerRpcNoResult2(string method_name, byte[] method_data);

    Task<TResult> OnContainerRpc<TResult>(string method_name);

    Task<TResult> OnContainerRpc<T1, TResult>(
        string method_name, T1 param1);

    Task<TResult> OnContainerRpc<T1, T2, TResult>(
        string method_name, T1 param1, T2 param2);

    Task<TResult> OnContainerRpc<T1, T2, T3, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3);

    Task<TResult> OnContainerRpc<T1, T2, T3, T4, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4);

    Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);

    Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

    Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

    Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

    Task<TResult> OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9);

    Task<byte[]> OnContainerRpc2(string method_name, byte[] method_data);

    Task OnEntityRpcNoResult(
       long entity_id, string component_name, string method_name);

    Task OnEntityRpcNoResult<T1>(
       long entity_id, string component_name, string method_name, T1 param1);

    Task OnEntityRpcNoResult<T1, T2>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2);

    Task OnEntityRpcNoResult<T1, T2, T3>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3);

    Task OnEntityRpcNoResult<T1, T2, T3, T4>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4);

    Task OnEntityRpcNoResult<T1, T2, T3, T4, T5>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);

    Task OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

    Task OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

    Task OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

    Task OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9);

    Task OnEntityRpcNoResult2(
        long entity_id, string component_name, string method_name, byte[] method_data);

    Task<TResult> OnEntityRpc<TResult>(
        long entity_id, string component_name, string method_name);

    Task<TResult> OnEntityRpc<T1, TResult>(
        long entity_id, string component_name, string method_name, T1 param1);

    Task<TResult> OnEntityRpc<T1, T2, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2);

    Task<TResult> OnEntityRpc<T1, T2, T3, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3);

    Task<TResult> OnEntityRpc<T1, T2, T3, T4, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4);

    Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5);

    Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, T6, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6);

    Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7);

    Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8);

    Task<TResult> OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9);

    Task<byte[]> OnEntityRpc2(
        long entity_id, string component_name, string method_name, byte[] method_data);
}

#endif
