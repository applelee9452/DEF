#if !DEF_CLIENT

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MemoryPack;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// 测试玩家
public class ContainerStatefulIMTestPlayer : ContainerStateful, IContainerStatefulIMTestPlayer
{
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    ContainerStatefulStreamSub<SStreamInfo> StreamSubMarquee { get; set; }

    public override async Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefulTestPlayer.OnCreate() PlayerGuid={ContainerId}", ContainerId);

        StreamSubMarquee = await CreateStreamSubAsync<SStreamInfo>(
            StringDef.StreamNameSpaceMarquee, StringDef.StreamGuidMarquee, OnStreamMarquee);

        StopwatchUpdate = new Stopwatch();
        StopwatchUpdate.Start();
        TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(),
            null, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(200));
    }

    public override async Task OnDestroy()
    {
        if (TimerHandleUpdate != null)
        {
            TimerHandleUpdate.Dispose();
            TimerHandleUpdate = null;
        }

        if (StopwatchUpdate != null)
        {
            StopwatchUpdate.Stop();
            StopwatchUpdate = null;
        }

        if (StreamSubMarquee != null)
        {
            await StreamSubMarquee.UnsubAsync();
            StreamSubMarquee = null;
        }

        Logger.LogDebug("ContainerStatefullIMPlayer.OnDestroy() PlayerGuid={ContainerId}", ContainerId);
    }

    Task IContainerStatefulIMTestPlayer.Touch()
    {
        Logger.LogDebug("ContainerStatefullTestPlayer.Touch()");

        return Task.CompletedTask; ;
    }

    Task OnStreamMarquee(SStreamInfo s, Orleans.Streams.StreamSequenceToken token = null)
    {
        if (s.Id == SStreamId.Marquee)
        {
            var im_marquee = MemoryPackSerializer.Deserialize<BIMMarquee>(s.Data);

            //Console.WriteLine($"OnStreamMarquee() Id={ContainerId} Msg={im_marquee.Msg}");
        }

        return Task.CompletedTask;
    }

    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        return Task.CompletedTask;
    }
}

#endif