#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoryPack;
using MongoDB.Driver;
using StackExchange.Redis;

namespace DEF.IM;

public partial class ComIMPlayerMailBox : IComponentRpcIMPlayerMailBox
{
    void AwakeServer(Dictionary<string, object> create_params)
    {
        Entity.Export4Bson = true;
        Entity.ExportEntityData = true;
        Entity.SetNetworkSyncFlag(true);

        Scene.Add2Blackboard(this);
        State.ListMail ??= [];
    }

    void OnStartServer()
    {
    }

    void OnDestroyServer(string reason, byte[] user_data)
    {
        Scene.RemoveFromBlackboard(this);
    }

    void HandleEventServer(DEF.Event ev)
    {
    }

    // 请求发送邮件
    Task IComponentRpcIMPlayerMailBox.RequestSendMail(Mail mail)
    {
        State.ListMail.Add(mail);
        State.ListMail = State.ListMail;

        return Task.CompletedTask;
    }

    // 已读邮件
    Task<int> IComponentRpcIMPlayerMailBox.RequestReadMails(string mail_guid)
    {
        foreach (var item in State.ListMail)
        {
            if (mail_guid == item.MailGuid && item.State == 0)
            {
                item.State = 1;
                break;
            }
        }
        State.ListMail = State.ListMail;
        return Task.FromResult(0);
    }

    // 请求领取附件奖励，-1是全部领取
    async Task<List<Mail>> IComponentRpcIMPlayerMailBox.RequestGetReward(string mail_guid)
    {
        List<DEF.IM.Mail> list = [];

        var com_im_player = Scene.GetComponentFromBlackboard<ComIMPlayer>();
        PlayerInfo player_info = com_im_player.GetPlayerInfo();

        ChannelMail channel_mail = new()
        {
            PlayerGuid = player_info.PlayerGuid,
            AttachmentList = []
        };

        foreach (var item in State.ListMail)
        {
            if (item.State == (int)MailStateType.Recived)
                continue;

            if (mail_guid == "-1" || item.MailGuid == mail_guid)
            {
                list.Add(item);
                channel_mail.AttachmentList.AddRange(item.Attachments);
                item.State = (int)MailStateType.Recived;
            }
        }

        State.ListMail = State.ListMail;

        byte[] data_bytes = MemoryPackSerializer.Serialize(channel_mail);
        var subpub_event = new SubPubEvent();
        subpub_event.EvType = (int)IMSubPubEventType.GetMailAttachment;
        subpub_event.Data = data_bytes;

        var redis_channel = RedisChannel.Literal(IMSubPubChannel.IMMail.ToString());
        byte[] event_bytes = MemoryPackSerializer.Serialize(subpub_event);
        await IMContext.Instance.DbClientRedis.DB.PublishAsync(redis_channel, event_bytes);

        return list;
    }

    // 删除已读邮件
    Task<int> IComponentRpcIMPlayerMailBox.RequestDeleteMails()
    {
        for (int i = State.ListMail.Count - 1; i >= 0; i--)
        {
            if (State.ListMail[i].State == (int)MailStateType.Recived)
            {
                State.ListMail.RemoveAt(i);
            }
        }
        State.ListMail = State.ListMail;
        return Task.FromResult(0);
    }

    public async Task GetLastestSystemMailList()
    {
        var mongo = IMContext.Instance.Mongo;

        var com_im_player = Scene.GetComponentFromBlackboard<ComIMPlayer>();
        PlayerInfo player_info = com_im_player.GetPlayerInfo();
        Region region = com_im_player.GetRegionInfo();

        var filter = Builders<DbEvPlayerRecvSystemMail>.Filter
                .Where(dbev_recv_system_mail => dbev_recv_system_mail.PlayerGuid.Equals(player_info.PlayerGuid));
        var sort = Builders<DbEvPlayerRecvSystemMail>.Sort.Descending(ev => ev.EventTm);// 按照 EventTm 字段降序排序
        var options = new FindOptions<DbEvPlayerRecvSystemMail>
        {
            Sort = sort,
            Limit = 1 // 只取第一条记录
        };
        var collection = IMContext.Instance.Mongo.GetCollection<DbEvPlayerRecvSystemMail>(StringDef.DbCollectionEvPlayerRecvSystemMail);
        var ev_list = await collection.FindAsync(filter, options).Result.ToListAsync();

        DateTime last_recv_date = ev_list.Count == 0 ? DateTime.MinValue : ev_list.First().EventTm;
        var container_system_mailbox = GetContainerRpc<IContainerStatefulIMSystemMailBox>();
        var system_mail_list = await container_system_mailbox.RequestGetLastestMailList(player_info.PlayerGuid, region.RegionGuid, last_recv_date);

        List<Task> list_task = [];
        for (int i = 0; i < system_mail_list.Count; i++)
        {
            var system_mail = system_mail_list[i];
            var t = AddMailFromSystemMail(system_mail);
            list_task.Add(t);
        }
        await Task.WhenAll(list_task);
    }

    public void AddMail(Mail mail)
    {
        State.ListMail.Add(mail);

        State.ListMail = State.ListMail;
    }

    public Task AddMailFromSystemMail(SystemMail system_mail)
    {
        AddMail(system_mail.Mail);

        DbEvPlayerRecvSystemMail dbev_player_recv_system_mail = new()
        {
            _id = Guid.NewGuid().ToString(),
            EventType = "EvPlayerRecvSystemMail",
            EventTm = DateTime.UtcNow,
            SystenMailGuid = system_mail.MailGuid,
            MailTitle = system_mail.Mail.Title
        };

        var com_im_player = Scene.GetComponentFromBlackboard<ComIMPlayer>();
        PlayerInfo player_info = com_im_player.GetPlayerInfo();

        dbev_player_recv_system_mail.PlayerGuid = player_info.PlayerGuid;
        dbev_player_recv_system_mail.PlayerNickName = player_info.NickName;

        Region region = com_im_player.GetRegionInfo();
        dbev_player_recv_system_mail.RegionGuid = region.RegionGuid;
        dbev_player_recv_system_mail.RegionName = region.RegionName;

        var mongo = IMContext.Instance.Mongo;
        return mongo.InsertAsync(StringDef.DbCollectionEvPlayerRecvSystemMail, dbev_player_recv_system_mail);
    }
}

#endif