namespace DEF;

public class ServiceClientObserverListenerDefault : IServiceClientObserverListener
{
    public Task NotifySession(ObserverInfo observer_info, string session_guid,
        string method_name)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1, T2>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1, T2, T3>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1, T2, T3, T4>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1, T2, T3, T4, T5>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1, T2, T3, T4, T5, T6>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1, T2, T3, T4, T5, T6, T7>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1, T2, T3, T4, T5, T6, T7, T8>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
    {
        return Task.CompletedTask;
    }

    public Task NotifySession<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9)
    {
        return Task.CompletedTask;
    }

    public Task DisConnectSession(string session_guid, string reason)
    {
        return Task.CompletedTask;
    }
}