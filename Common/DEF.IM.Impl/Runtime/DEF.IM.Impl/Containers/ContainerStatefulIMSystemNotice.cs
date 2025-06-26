#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// 系统公告，一般是客户端上线时主动拉取一次，不会主动推送。单实例
public class ContainerStatefulIMSystemNotice : ContainerStateful, IContainerStatefulIMSystemNotice
{
    List<DataNotice> ListDataNotice { get; set; } = [];// 最后一条是最新的
    List<Notice> ListNotice { get; set; } = [];// 最后一条是最新的
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }

    public override async Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefulSystemNotice.OnCreate()");

        var list_data_notice = await IMContext.Instance.Mongo.ReadListAsync<DataNotice>(StringDef.DbCollectionDataNotice);

        if (list_data_notice != null && list_data_notice.Count > 0)
        {
            ListDataNotice.AddRange(list_data_notice);
        }

        foreach (var i in ListDataNotice)
        {
            Notice notice = new()
            {
                Title = i.Title,
                Writer = i.Writer,
                Content = i.Content,
                Dt = i.Dt,
            };

            ListNotice.Add(notice);
        }

        StopwatchUpdate = new Stopwatch();
        StopwatchUpdate.Start();
        TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(),
            null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100));
    }

    public override Task OnDestroy()
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

        Logger.LogDebug("ContainerStatefulSystemNotice.OnDestroy()");

        return Task.CompletedTask;
    }

    // 保活
    Task IContainerStatefulIMSystemNotice.Touch()
    {
        return Task.CompletedTask;
    }

    // 添加最新的系统公告
    async Task IContainerStatefulIMSystemNotice.AddNotice(Notice notice)
    {
        ListNotice.Add(notice);

        DataNotice data_notice = new()
        {
            _id = Guid.NewGuid().ToString(),
            Title = notice.Title,
            Writer = notice.Writer,
            Content = notice.Content,
            Dt = notice.Dt,
        };

        ListDataNotice.Add(data_notice);

        await IMContext.Instance.Mongo.InsertAsync(StringDef.DbCollectionDataNotice, data_notice);
    }

    // 请求拉取最新的系统公告列表
    Task<List<Notice>> IContainerStatefulIMSystemNotice.RequestGetLastestNoticeList()
    {
        if (ListNotice.Count <= 5)
        {
            return Task.FromResult(ListNotice);
        }

        var list = ListNotice.Skip(ListNotice.Count - 5).Take(5).ToList();

        return Task.FromResult(list);
    }

    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        return Task.CompletedTask;
    }
}

#endif