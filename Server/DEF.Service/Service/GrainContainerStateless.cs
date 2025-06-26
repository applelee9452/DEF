using Microsoft.Extensions.Logging;
using Orleans.Concurrency;

namespace DEF;

[Reentrant]
[StatelessWorker]
public class GrainContainerStateless : Grain, IGrainContainerStateless
{
    ILogger Logger { get; set; }
    Service Service { get; set; }
    string ContainerType { get; set; }
    ContainerStateless Container { get; set; }

    public GrainContainerStateless(ILogger<GrainContainerStateless> logger, Service def_service)
    {
        Logger = logger;
        Service = def_service;
    }

    public override async Task OnActivateAsync(CancellationToken cancellation_token)
    {
        ContainerType = this.GetPrimaryKeyString();

        Container = Service.CreateContainerStateless(Logger, this, this.GrainFactory, ContainerType);

        if (Container == null)
        {
            Logger.LogError($"CreateContainerStateless Error! ContainerType={ContainerType}");
        }
        else
        {
            // todo，将Rpc接口添加到字典中

            try
            {
                await Container.OnCreate();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStateless.OnActivateAsync Error! ContainerType={ContainerType}");
            }
        }

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
                Logger.LogError(ex, $"GrainContainerStateless.OnDeactivateAsync Error! ContainerType={ContainerType}");
            }

            Container = null;
        }

        await base.OnDeactivateAsync(reason, cancellation_token);
    }

    public IDisposable RegisterTimer2(Func<object, Task> asyncCallback, object state, TimeSpan dueTime, TimeSpan period)
    {
        return this.RegisterGrainTimer(asyncCallback, state, dueTime, period);
    }

    Task IGrainContainerStateless.Touch()
    {
        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult(
        string method_name)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1>(
        string method_name, T1 param1)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1, T2>(
        string method_name, T1 param1, T2 param2)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1, T2, T3>(
        string method_name, T1 param1, T2 param2, T3 param3)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1, T2, T3, T4>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1, T2, T3, T4, T5>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5, param6);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5, param6, param7);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5, param6, param7, param8);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult(method_name, param1, param2, param3, param4, param5, param6, param7, param8, param9);

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task IGrainContainerStateless.OnContainerRpcNoResult2(string method_name, byte[] method_data)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpcNoResult2() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.CompletedTask;
        }

        try
        {
            return Container.OnContainerRpcNoResult2(method_name, method_data);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpcNoResult2 Error! ContainerType={ContainerType}");
        }

        return Task.CompletedTask;
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<TResult>(
        string method_name)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<TResult>(method_name);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, TResult>(
        string method_name, T1 param1)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, TResult>(method_name, param1);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, T2, TResult>(
        string method_name, T1 param1, T2 param2)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, TResult>(method_name, param1, param2);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, T2, T3, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, TResult>(method_name, param1, param2, param3);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, T2, T3, T4, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, TResult>(method_name, param1, param2, param3, param4);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(method_name, param1, param2, param3, param4, param5);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(method_name, param1, param2, param3, param4, param5, param6);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(method_name, param1, param2, param3, param4, param5, param6, param7);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(method_name, param1, param2, param3, param4, param5, param6, param7, param8);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<TResult> IGrainContainerStateless.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        string method_name, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
    {
        if (Container == null)
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult(default(TResult));
        }

        try
        {
            return Container.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(method_name, param1, param2, param3, param4, param5, param6, param7, param8, param9);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc Error! ContainerType={ContainerType}");
        }

        return Task.FromResult(default(TResult));
    }

    Task<byte[]> IGrainContainerStateless.OnContainerRpc2(string method_name, byte[] method_data)
    {
        if (Container != null)
        {
            try
            {
                return Container.OnContainerRpc2(method_name, method_data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GrainContainerStateless.OnContainerRpc2 Error! ContainerType={ContainerType}");
            }

            return Task.FromResult((byte[])null);
        }
        else
        {
            Logger.LogError($"GrainContainerStateless.OnContainerRpc2() Error! Container==null, ContainerType={ContainerType} MethodName={method_name}");

            return Task.FromResult((byte[])null);
        }
    }
}