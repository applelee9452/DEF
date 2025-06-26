using System.Threading.Tasks;

namespace DEF
{
    public interface IRpcer
    {
        Task RequestResponse(IRpcInfo rpcinfo, string method_name);

        Task RequestResponse<T1>(IRpcInfo rpcinfo, string method_name, T1 obj1);

        Task RequestResponse<T1, T2>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2);

        Task RequestResponse<T1, T2, T3>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3);

        Task RequestResponse<T1, T2, T3, T4>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4);

        Task RequestResponse<T1, T2, T3, T4, T5>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5);

        Task RequestResponse<T1, T2, T3, T4, T5, T6>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6);

        Task RequestResponse<T1, T2, T3, T4, T5, T6, T7>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7);

        Task RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8);

        Task RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9);

        Task<TResult> RequestResponse<TResult>(IRpcInfo rpcinfo, string method_name);

        Task<TResult> RequestResponse<T1, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1);

        Task<TResult> RequestResponse<T1, T2, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2);

        Task<TResult> RequestResponse<T1, T2, T3, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3);

        Task<TResult> RequestResponse<T1, T2, T3, T4, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4);

        Task<TResult> RequestResponse<T1, T2, T3, T4, T5, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5);

        Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6);

        Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7);

        Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8);

        Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9);
    }
}