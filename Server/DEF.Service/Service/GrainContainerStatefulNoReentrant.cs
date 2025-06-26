using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace DEF;

public class GrainContainerStatefulNoReentrant : Grain, IGrainContainerStatefulNoReentrant
{
    ILogger Logger { get; set; }
    Service Service { get; set; }
    string ContainerType { get; set; }
    string ContainerId { get; set; }
    ContainerStatefulNoReentrant Container { get; set; }

    public GrainContainerStatefulNoReentrant(ILogger<GrainContainerStatefulNoReentrant> logger, Service def_service)
    {
        Logger = logger;
        Service = def_service;
    }

    public override async Task OnActivateAsync(CancellationToken cancellation_token)
    {
        var s = this.GetPrimaryKeyString();
        var arr = s.Split('_');
        ContainerType = arr[0];
        ContainerId = arr[1];

        Container = Service.CreateContainerStatefulNoReentrant(Logger, this, this.GrainFactory, ContainerType, ContainerId);

        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnActivateAsync Error! ContainerType={ContainerType}");
        }

        try
        {
            await Container.OnCreate();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnActivateAsync Error! ContainerType={ContainerType}");
        }

        DelayDeactivation(TimeSpan.FromSeconds(60));

        await base.OnActivateAsync(cancellation_token);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellation_token)
    {
        if (Container != null)
        {
            try
            {
                await Container.OnDestroy();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnDeactivateAsync Error! ContainerType={ContainerType}");
            }

            Container = null;
        }

        await base.OnDeactivateAsync(reason, cancellation_token);
    }

    public IGrainTimer RegisterTimer2(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
    {
        return this.RegisterGrainTimer(asyncCallback, state, dueTime, period);
    }

    public IGrainTimer RegisterTimer2(Func<object, Task> asyncCallback, object state, GrainTimerCreationOptions options)
    {
        return this.RegisterGrainTimer(asyncCallback, state, options);
    }

    public IStreamProvider GetStreamProvider2()
    {
        return this.GetStreamProvider("StreamProvider");
    }

    public void DeactivateOnIdle1()
    {
        DeactivateOnIdle();
    }

    public void DeactivateOnIdle2(TimeSpan ts)
    {
        DelayDeactivation(ts);
    }

    Task IGrainContainerStatefulNoReentrant.Touch()
    {
        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult(
        string method_name)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1>(
        string method_name, T1 param1)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1, T2>(
        string method_name, T1 param1, T2 param2)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1, T2, T3>(
        string method_name, T1 param1, T2 param2, T3 param3)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1, T2, T3, T4>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1, T2, T3, T4, T5>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5, param6);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5, param6, param7);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5, param6, param7, param8);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStatefulNoReentrant.OnContainerRpcNoResult2(string method_name, byte[] method_data)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult2() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult2(method_name, method_data);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpcNoResult2 Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<TResult>(
        string method_name)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<TResult>(method_name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, TResult>(
          string method_name, T1 param1)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, TResult>(method_name, param1);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, T2, TResult>(
        string method_name, T1 param1, T2 param2)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, TResult>(method_name, param1, param2);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, T2, T3, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, TResult>(method_name, param1, param2, param3);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, T2, T3, T4, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, TResult>(method_name, param1, param2, param3, param4);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(method_name, param1, param2, param3, param4, param5);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(method_name, param1, param2, param3, param4, param5, param6);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(method_name, param1, param2, param3, param4, param5, param6, param7);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(method_name, param1, param2, param3, param4, param5, param6, param7, param8);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(method_name, param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<byte[]> IGrainContainerStatefulNoReentrant.OnContainerRpc2(string method_name, byte[] method_data)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnContainerRpc2(method_name, method_data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnContainerRpc2 Error! ContainerType={ContainerType}");
            }

            return Task.FromResult((byte[])null);
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnContainerRpc2() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult((byte[])null);
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult(
        long entity_id, string component_name, string method_name)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1>(
        long entity_id, string component_name, string method_name, T1 param1)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1, T2>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1, param2);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1, T2, T3>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1, param2, param3);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1, T2, T3, T4>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1, param2, param3, param4);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1, T2, T3, T4, T5>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1, param2, param3, param4, param5);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7, param8);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult(entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7, param8, param9);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task IGrainContainerStatefulNoReentrant.OnEntityRpcNoResult2(
        long entity_id, string component_name, string method_name, byte[] method_data)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpcNoResult2(entity_id, component_name, method_name, method_data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult2 Error! ContainerType={ContainerType}");
            }

            return Task.CompletedTask;
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpcNoResult2() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<TResult>(
        long entity_id, string component_name, string method_name)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<TResult>(
                    entity_id, component_name, method_name);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, TResult>(
        long entity_id, string component_name, string method_name, T1 param1)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, TResult>(
                    entity_id, component_name, method_name, param1);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, T2, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, T2, TResult>(
                    entity_id, component_name, method_name, param1, param2);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, T2, T3, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, T2, T3, TResult>(
                    entity_id, component_name, method_name, param1, param2, param3);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, T2, T3, T4, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, T2, T3, T4, TResult>(
                    entity_id, component_name, method_name, param1, param2, param3, param4);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, T2, T3, T4, T5, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, T2, T3, T4, T5, TResult>(
                    entity_id, component_name, method_name, param1, param2, param3, param4, param5);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, T2, T3, T4, T5, T6, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, T2, T3, T4, T5, T6, TResult>(
                    entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                    entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                    entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7, param8);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<TResult> IGrainContainerStatefulNoReentrant.OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        long entity_id, string component_name, string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                    entity_id, component_name, method_name, param1, param2, param3, param4, param5, param6, param7, param8, param9);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc Error! ContainerType={ContainerType}");
            }

            return Task.FromResult(default(TResult));
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }
    }

    Task<byte[]> IGrainContainerStatefulNoReentrant.OnEntityRpc2(
        long entity_id, string component_name, string method_name, byte[] method_data)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnEntityRpc2(entity_id, component_name, method_name, method_data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStatefulNoReentrant.OnEntityRpc2 Error! ContainerType={ContainerType}");
            }

            return Task.FromResult((byte[])null);
        }
        else
        {
            Logger.LogError($"GrainContainerStatefulNoReentrant.OnEntityRpc2() Error! Container==null, ContainerId={ContainerType} MethodName={method_name}");

            return Task.FromResult((byte[])null);
        }
    }
}