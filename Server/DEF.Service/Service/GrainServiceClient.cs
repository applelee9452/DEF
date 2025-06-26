using Microsoft.Extensions.Logging;
using Orleans.Concurrency;

namespace DEF;

// 一个DEF.Gateway进程实例对应一个GrainServiceClient实例
[Reentrant]
public class GrainServiceClient : Grain, IGrainServiceClient
{
    ILogger Logger { get; set; }
    IServiceListener Listener { get; set; }
    Service Service { get; set; }
    IGrainServiceClientObserver Observer { get; set; }

    public GrainServiceClient(ILogger<GrainServiceClient> logger,
        Service def_service,
        IServiceListener listener)
    {
        Logger = logger;
        Service = def_service;
        Listener = listener;
    }

    public override Task OnActivateAsync(CancellationToken cancellation_token)
    {
        Logger.LogInformation("GrainServiceClient.OnActivateAsync，GrainId={GrainId}", this.GetPrimaryKeyString());

        return base.OnActivateAsync(cancellation_token);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellation_token)
    {
        Logger.LogInformation("GrainServiceClient.OnDeactivateAsync，GrainId={GrainId} Reason={Reason}", this.GetPrimaryKeyString(), reason);

        return base.OnDeactivateAsync(reason, cancellation_token);
    }

    Task IGrainServiceClient.Touch()
    {
        return Task.CompletedTask;
    }

    Task IGrainServiceClient.Sub(IGrainServiceClientObserver sub)
    {
        Observer = sub;

        return Task.CompletedTask;
    }

    Task IGrainServiceClient.Unsub(IGrainServiceClientObserver sub)
    {
        Observer = null;

        DeactivateOnIdle();

        return Task.CompletedTask;
    }

    Task IGrainServiceClient.Notify(ObserverInfo observer_info, string session_guid,
        string method_name)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name);
    }

    Task IGrainServiceClient.Notify<T1>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1);
    }

    Task IGrainServiceClient.Notify<T1, T2>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1, obj2);
    }

    Task IGrainServiceClient.Notify<T1, T2, T3>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1, obj2, obj3);
    }

    Task IGrainServiceClient.Notify<T1, T2, T3, T4>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1, obj2, obj3, obj4);
    }

    Task IGrainServiceClient.Notify<T1, T2, T3, T4, T5>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1, obj2, obj3, obj4, obj5);
    }

    Task IGrainServiceClient.Notify<T1, T2, T3, T4, T5, T6>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1, obj2, obj3, obj4, obj5, obj6);
    }

    Task IGrainServiceClient.Notify<T1, T2, T3, T4, T5, T6, T7>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
    }

    Task IGrainServiceClient.Notify<T1, T2, T3, T4, T5, T6, T7, T8>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
    }

    Task IGrainServiceClient.Notify<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ObserverInfo observer_info, string session_guid,
        string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9)
    {
        if (Observer == null)
        {
            // todo，log error
            return Task.CompletedTask;
        }

        return Observer.NotifySession(observer_info, session_guid, method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
    }

    // 网关给后端所有有状态服务广播连接成功&认证成功
    Task IGrainServiceClient.OnSessionConnectedAndAuthed(Gateway.GatewayAuthedInfo info, string extra_data)
    {
        return Listener.OnSessionConnectedAndAuthed(info, extra_data);
    }

    // 网关给后端所有有状态服务器广播连接断开
    Task IGrainServiceClient.OnSessionDisConnect(string player_guid, string session_guid)
    {
        return Listener.OnSessionDisConnect(player_guid, session_guid);
    }
}