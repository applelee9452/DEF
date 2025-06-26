using DEF;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DEF.EvCenter;

public class ContainerStatefulInitCluster : ContainerStateful, IContainerStatefulInitCluster
{
    public override async Task OnCreate()
    {
        Logger.LogDebug($"ContainerStatefulInitCluster.OnCreate()");

        var c1 = GetContainerRpc<IContainerStatefulInitDb>();
        await c1.Setup();
    }

    public override Task OnDestroy()
    {
        Logger.LogDebug($"ContainerStatefulInitCluster.OnDestroy()");

        return Task.CompletedTask;
    }

    Task IContainerStatefulInitCluster.Touch()
    {
        return Task.CompletedTask;
    }
}