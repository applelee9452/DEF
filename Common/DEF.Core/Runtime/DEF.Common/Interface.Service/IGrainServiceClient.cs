#if !DEF_CLIENT

using Orleans;
using Orleans.Concurrency;
using System.Threading.Tasks;

namespace DEF;

public interface IGrainServiceClient : IGrainWithStringKey
{
    [OneWay]
    Task Touch();

    Task Sub(IGrainServiceClientObserver sub);

    Task Unsub(IGrainServiceClientObserver sub);

    Task Notify(ObserverInfo observer_info, string session_guid, string method_name);

    Task Notify<T1>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1);

    Task Notify<T1, T2>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2);

    Task Notify<T1, T2, T3>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3);

    Task Notify<T1, T2, T3, T4>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4);

    Task Notify<T1, T2, T3, T4, T5>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5);

    Task Notify<T1, T2, T3, T4, T5, T6>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6);

    Task Notify<T1, T2, T3, T4, T5, T6, T7>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7);

    Task Notify<T1, T2, T3, T4, T5, T6, T7, T8>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8);

    Task Notify<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9);

    // 网关给后端所有有状态服务广播连接成功&认证成功
    Task OnSessionConnectedAndAuthed(Gateway.GatewayAuthedInfo info, string extra_data);

    // 网关给后端所有有状态服务器广播连接断开
    Task OnSessionDisConnect(string player_guid, string session_guid);
}

#endif
