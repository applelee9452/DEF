#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DEF.IM;

// 系统邮箱。单实例
public class ContainerStatefulIMSystemMailBox : ContainerStateful, IContainerStatefulIMSystemMailBox
{
    public List<SystemMail> SystemMailList { get; set; } = [];// 按时间升序排序
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }

    public override async Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefulSystemMailBox.OnCreate()");

        // TODO 过期的不要
        var list_data = await IMContext.Instance.Mongo.ReadListAsync<DataSystemMail>(StringDef.DbCollectionDataSystemMail);
        for (int i = 0; i < list_data?.Count; i++)
        {
            SystemMail system_email = new() { };
            system_email.From(list_data[i]);

            SystemMailList.Add(system_email);
        }

        SystemMailList.Sort((a, b) => { return a.Dt.CompareTo(b.Dt); });

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

        Logger.LogDebug("ContainerStatefulSystemMailBox.OnDestroy()");

        return Task.CompletedTask;
    }

    // 保活
    Task IContainerStatefulIMSystemMailBox.Touch()
    {
        return Task.CompletedTask;
    }

    // 请求收取最新的系统邮件列表
    Task<List<SystemMail>> IContainerStatefulIMSystemMailBox.RequestGetLastestMailList(
        string region_id, string player_guid, DateTime last_recv_time)
    {
        List<SystemMail> lastest_mail_list = [];

        for (int i = SystemMailList.Count - 1; i >= 0; i--)
        {
            var system_mail = SystemMailList[i];
            bool need_send = false;
            switch (system_mail.TargetType)
            {
                case IMTargetType.All:
                    need_send = true;
                    break;
                case IMTargetType.Region:
                    need_send = system_mail.RegionIdList.Contains(region_id);
                    break;
                case IMTargetType.Player:
                    need_send = system_mail.PlayerIdList.Contains(player_guid);
                    break;
            }

            if (!need_send)
            {
                continue;
            }

            if (system_mail.Dt > last_recv_time)
            {
                lastest_mail_list.Add(system_mail);
            }
            else
            {
                break;
            }
        }

        return Task.FromResult(lastest_mail_list);
    }

    // 添加一封新邮件
    async Task IContainerStatefulIMSystemMailBox.AddMail(SystemMail system_mail)
    {
        List<string> region_list = [];

        // 广播给离线玩家
        if (system_mail.TargetType == IMTargetType.All)
        {
            var container = GetContainerRpc<IContainerStatefulIMRegionMgr>();
            var region_list2 = await container.GetRegionIdList();

            region_list.AddRange(region_list2);
        }

        if (system_mail.TargetType == IMTargetType.Region)
        {
            region_list = system_mail.RegionIdList;
        }

        List<Task> list_task = [];
        for (int i = 0; i < region_list.Count; i++)
        {
            string region_id = region_list[i];
            var c = GetContainerRpc<IContainerStatefulIMRegion>(region_id);
            var t = c.SendRegionSystemMail(system_mail);
            list_task.Add(t);
        }
        await Task.WhenAll(list_task);

        // 将 SystemMail 加入列表，并保存到数据库
        bool need_save = system_mail.TargetType == IMTargetType.All || system_mail.TargetType == IMTargetType.Region;
        if (need_save)
        {
            SystemMailList.Add(system_mail);

            DataSystemMail data_system_mail = new();
            data_system_mail.From(system_mail);
            data_system_mail._id = Guid.NewGuid().ToString();

            await IMContext.Instance.Mongo.InsertAsync(StringDef.DbCollectionDataSystemMail, data_system_mail);
        }
        else
        {
            // 针对个人邮件直接发给个人

            List<Task> list_task1 = [];
            for (int i = 0; i < system_mail.PlayerIdList.Count; i++)
            {
                string player_guid = system_mail.PlayerIdList[i];
                var container_player = GetContainerRpc<IContainerStatefulIMPlayer>(player_guid);
                var t = container_player.AddMail(system_mail.Mail);
                list_task1.Add(t);
            }
            await Task.WhenAll(list_task1);
        }
    }

    // 所有分区所有玩家广播发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task IContainerStatefulIMSystemMailBox.SendMailToAllRegionPlayers(Mail mail)
    {
        return Task.CompletedTask;
    }

    // 向指定分区所有玩家广播发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task IContainerStatefulIMSystemMailBox.SendMailToOneRegionPlayers(Mail mail, int region_id)
    {
        return Task.CompletedTask;
    }

    // 向指定分区列表的所有玩家广播发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task IContainerStatefulIMSystemMailBox.SendMailToRegionListPlayers(Mail mail, List<int> list_region_id)
    {
        return Task.CompletedTask;
    }

    // 向指定玩家发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task IContainerStatefulIMSystemMailBox.SendMailToPlayer(Mail mail, string player_guid)
    {
        return Task.CompletedTask;
    }

    // 向指定玩家列表发送邮件；离线玩家上线时读取，在线玩家直接收到
    Task IContainerStatefulIMSystemMailBox.SendMailToPlayers(Mail mail, List<string> list_player_guid)
    {
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