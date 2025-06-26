namespace DEF;

public class Rpcer4Service : IRpcer
{
    public Task RequestResponse(
        IRpcInfo rpcinfo, string method_name)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name);
                    }

                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1>(
        IRpcInfo rpcinfo, string method_name, T1 obj1)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1, T2>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1, obj2);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1, T2, T3>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1, obj2, obj3);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3);
                    }

                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3);
                    }

            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3);
                    }

                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1, T2, T3, T4>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1, obj2, obj3, obj4);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1, T2, T3, T4, T5>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1, obj2, obj3, obj4, obj5);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1, T2, T3, T4, T5, T6>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1, obj2, obj3, obj4, obj5, obj6);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                    }

                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                    }

            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1, T2, T3, T4, T5, T6, T7>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                    string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public Task RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.IsObserver)
        {
            if (!string.IsNullOrEmpty(rpcinfo2.ObserverGatewayGuid) && !string.IsNullOrEmpty(rpcinfo2.ObserverSessionGuid))
            {
                ObserverInfo observer_info = new()
                {
                    ContainerStateType = rpcinfo2.ContainerStateType,
                    ContainerType = rpcinfo2.ContainerType,
                    ContainerId = rpcinfo2.ContainerId,
                    EntityId = rpcinfo2.EntityId,
                    ComponentName = rpcinfo2.ComponentName,
                };

                var grain = rpcinfo2.GrainFactory.GetGrain<IGrainServiceClient>(rpcinfo2.ObserverGatewayGuid);
                return grain.Notify(observer_info, rpcinfo2.ObserverSessionGuid,
                    method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                    }
            }

            return Task.CompletedTask;
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        return grain.OnContainerRpcNoResult(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                    }
            }

            return Task.CompletedTask;
        }
    }

    public async Task<TResult> RequestResponse<TResult>(
        IRpcInfo rpcinfo, string method_name)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<TResult>(
                            method_name);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<TResult>(
                            method_name);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<TResult>(
                            method_name);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<TResult>(
                            method_name);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<TResult>(
                            method_name);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<TResult>(
                            method_name);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<TResult>(
                            method_name);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<TResult>(
                            method_name);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, TResult>(
                            method_name, obj1);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, TResult>(
                            method_name, obj1);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, TResult>(
                            method_name, obj1);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, TResult>(
                            method_name, obj1);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, TResult>(
                            method_name, obj1);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, TResult>(
                            method_name, obj1);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, TResult>(
                            method_name, obj1);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, TResult>(
                            method_name, obj1);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, T2, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, TResult>(
                            method_name, obj1, obj2);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, TResult>(
                            method_name, obj1, obj2);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, TResult>(
                            method_name, obj1, obj2);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, TResult>(
                            method_name, obj1, obj2);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, TResult>(
                            method_name, obj1, obj2);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, TResult>(
                            method_name, obj1, obj2);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, TResult>(
                            method_name, obj1, obj2);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, TResult>(
                            method_name, obj1, obj2);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, T2, T3, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, TResult>(
                            method_name, obj1, obj2, obj3);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, TResult>(
                            method_name, obj1, obj2, obj3);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, TResult>(
                            method_name, obj1, obj2, obj3);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, TResult>(
                            method_name, obj1, obj2, obj3);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, TResult>(
                            method_name, obj1, obj2, obj3);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, TResult>(
                            method_name, obj1, obj2, obj3);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, TResult>(
                            method_name, obj1, obj2, obj3);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, TResult>(
                            method_name, obj1, obj2, obj3);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, T2, T3, T4, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, TResult>(
                            method_name, obj1, obj2, obj3, obj4);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, TResult>(
                            method_name, obj1, obj2, obj3, obj4);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, TResult>(
                            method_name, obj1, obj2, obj3, obj4);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, TResult>(
                            method_name, obj1, obj2, obj3, obj4);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, TResult>(
                            method_name, obj1, obj2, obj3, obj4);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, TResult>(
                            method_name, obj1, obj2, obj3, obj4);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, TResult>(
                            method_name, obj1, obj2, obj3, obj4);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, TResult>(
                            method_name, obj1, obj2, obj3, obj4);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, T2, T3, T4, T5, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8);
                        return obj;
                    }
            }
        }

        return default;
    }

    public async Task<TResult> RequestResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
        IRpcInfo rpcinfo, string method_name, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9)
    {
        var rpcinfo2 = (RpcInfo4Service)rpcinfo;

        if (rpcinfo2.SourceServiceName == rpcinfo2.TargetServiceName)
        {
            // 相同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.GrainFactory.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                        return obj;
                    }
            }
        }
        else
        {
            // 不同服务

            switch (rpcinfo2.ContainerStateType)
            {
                case ContainerStateType.Stateless:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateless>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                        return obj;
                    }
                case ContainerStateType.Stateful:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStateful>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                        return obj;
                    }
                case ContainerStateType.StatelessNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatelessNoReentrant>(rpcinfo2.ContainerType);
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                        return obj;
                    }
                case ContainerStateType.StatefulNoReentrant:
                    {
                        var grain = rpcinfo2.Client.GetGrain<IGrainContainerStatefulNoReentrant>(
                            string.Format("{0}_{1}", rpcinfo2.ContainerType, rpcinfo2.ContainerId));
                        var obj = await grain.OnContainerRpc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(
                            method_name, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9);
                        return obj;
                    }
            }
        }

        return default;
    }
}