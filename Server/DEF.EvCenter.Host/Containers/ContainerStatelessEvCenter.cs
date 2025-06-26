using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DEF.EvCenter;

public class ContainerStatelessEvCenter : ContainerStateless, IContainerStatelessEvCenter
{
    public override Task OnCreate()
    {
        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    Task IContainerStatelessEvCenter.ClientCrashReport(CrashReportInfo info, string client_ip)
    {
        var c = GetContainerRpc<IContainerStatelessEvClientException>();
        return c.ClientCrashReport(info, client_ip);
    }
}