#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace DEF.IM;

// CDKey，礼品码，礼包码管理，单实例
public class ContainerStatefulIMCDKeyMgr : ContainerStateful, IContainerStatefulIMCDKeyMgr
{
    List<CDKey> ListCDKey { get; set; } = [];
    Stopwatch StopwatchUpdate { get; set; }
    IDisposable TimerHandleUpdate { get; set; }

    public override async Task OnCreate()
    {
        Logger.LogDebug("ContainerStatefulIMCDKeyMgr.OnCreate()");

        var list_data_cdkey = await IMContext.Instance.Mongo.ReadListAsync<DataCDKey>(StringDef.DbCollectionDataCDKey);
        for (int i = 0; i < list_data_cdkey?.Count; i++)
        {
            if (list_data_cdkey[i].IsDelete) continue;

            CDKey cdkey = new();
            cdkey.From(list_data_cdkey[i]);
            ListCDKey.Add(cdkey);
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

        Logger.LogDebug("ContainerStatefulIMCDKeyMgr.OnDestroy()");

        return Task.CompletedTask;
    }

    // 定时器更新
    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        return Task.CompletedTask;
    }

    // 保活
    Task IContainerStatefulIMCDKeyMgr.Touch()
    {
        return Task.CompletedTask;
    }

    async Task<CDKey> IContainerStatefulIMCDKeyMgr.CreateCDKey(CDKey cdkey)
    {
        cdkey.Key = cdkey.Key.ToLower();

        var collection = IMContext.Instance.Mongo.GetCollection<DataCDKey>(StringDef.DbCollectionDataCDKey);
        var fliter = Builders<DataCDKey>.Filter.Where(data => data.Key.Equals(cdkey.Key));

        var data_cdkey_list = await collection.Find(fliter, null).ToListAsync();
        if (data_cdkey_list?.Count > 0)
        {
            return null;
        }

        DataCDKey data_cdkey = new();
        data_cdkey.From(cdkey);

        await IMContext.Instance.Mongo.InsertAsync(StringDef.DbCollectionDataCDKey, data_cdkey);

        ListCDKey.Add(cdkey);

        return cdkey;
    }

    Task<List<CDKey>> IContainerStatefulIMCDKeyMgr.GetCDKeyList()
    {
        return Task.FromResult(ListCDKey);
    }

    async Task<bool> IContainerStatefulIMCDKeyMgr.UpdateCDKeyOne(CDKey cdkey)
    {
        var filter = Builders<DataCDKey>.Filter.Where(x => x._id == cdkey.CDKeyGuid);
        var update = Builders<DataCDKey>.Update
            .Set(x => x.Key, cdkey.Key)
            .Set(x => x.ExpireDt, cdkey.ExpireDt)
            .Set(x => x.TargetType, cdkey.TargetType)
            .Set(x => x.RegionIdList, cdkey.RegionIdList)
            .Set(x => x.PlayerIdList, cdkey.PlayerIdList)
            .Set(x => x.MailTitle, cdkey.MailTitle)
            .Set(x => x.MailDesc, cdkey.MailDesc)
            .Set(x => x.CDKeyAttachmentList, cdkey.CDKeyAttachmentList);

        await IMContext.Instance.Mongo.UpdateOneAsync(filter, StringDef.DbCollectionDataCDKey, update);

        for (int i = 0; i < ListCDKey.Count; i++)
        {
            var ck = ListCDKey[i];

            if (ck.CDKeyGuid == cdkey.CDKeyGuid)
            {
                ListCDKey[i] = cdkey;
                break;
            }
        }

        return true;
    }

    async Task<bool> IContainerStatefulIMCDKeyMgr.DeleteCDKeyOne(string guid)
    {
        var filter = Builders<DataCDKey>.Filter.Where(x => x._id == guid);
        var update = Builders<DataCDKey>.Update
            .Set(x => x.IsDelete, true);

        await IMContext.Instance.Mongo.UpdateOneAsync(filter, StringDef.DbCollectionDataCDKey, update);

        ListCDKey.RemoveAll(cdkey => cdkey.CDKeyGuid == guid);

        return true;
    }

    async Task<IMErrorCode> IContainerStatefulIMCDKeyMgr.ExchangeCdkey(string player_guid, string cd_key)
    {
        // 1 检测 cdkey 有效性
        if (string.IsNullOrEmpty(cd_key))
        {
            return IMErrorCode.Error;
        }

        cd_key = cd_key.ToLower();

        CDKey cdkey = null;
        foreach (var ck in ListCDKey)
        {
            if (ck.Key == cd_key)
            {
                cdkey = ck;
            }
        }

        if (cdkey is null)
        {
            return IMErrorCode.NoHasCDKey;
        }

        // todo，已过期
        if (cdkey.ExpireDt < DateTime.UtcNow)
        {

        }

        // 2 检测 身份有效性
        var im_player = GetContainerRpc<IContainerStatefulIMPlayer>(player_guid);
        Region region = await im_player.GetRegionInfo();

        if (cdkey.TargetType != IMTargetType.All)
        {
            if (cdkey.RegionIdList is null || !cdkey.RegionIdList.Contains(region.RegionGuid))
            {
                return IMErrorCode.WrongRegion;
            }
        }

        if (cdkey.TargetType == IMTargetType.Player)
        {
            if (cdkey.PlayerIdList is null || !cdkey.PlayerIdList.Contains(player_guid))
            {
                return IMErrorCode.WrongPlayerGuid;
            }
        }

        // 3 检测 领取记录
        var collection = IMContext.Instance.Mongo.GetCollection<EvExchangeCDKey>(StringDef.DbCollectionDataEvExchangeCDKey);
        var fliter = Builders<EvExchangeCDKey>.Filter.Where(
            data_ev => data_ev.PlayerGuid.Equals(player_guid)
            && data_ev.CDKeyGuid == cdkey.CDKeyGuid
            && data_ev.RegionGuid == region.RegionGuid
        );
        var ev_exchang_cdkey_list = await collection.Find(fliter, null).ToListAsync();

        if (ev_exchang_cdkey_list?.Count > 0)
        {
            return IMErrorCode.Exchange_Before;
        }

        // 4 保存领取记录
        PlayerInfo player_info = await im_player.GetPlayerInfo();

        EvExchangeCDKey exchange_cdkey = new()
        {
            _id = Guid.NewGuid().ToString(),
            RegionGuid = region.RegionGuid,
            PlayerGuid = player_guid,
            CDKeyGuid = cdkey.CDKeyGuid,
            EventType = typeof(EvExchangeCDKey).Name,
            EventTm = DateTime.UtcNow,
            RegionName = region.RegionName,
            PlayerNickName = player_info.NickName,
            CDKey = cd_key
        };

        await IMContext.Instance.Mongo.InsertAsync(StringDef.DbCollectionDataEvExchangeCDKey, exchange_cdkey);

        // 5 发送邮件
        List<DEF.IM.MailAttachment> list = new();
        if (cdkey.CDKeyAttachmentList is not null)
        {
            foreach (var attachment in cdkey.CDKeyAttachmentList)
            {
                var mail_attackment = new MailAttachment();
                mail_attackment.ItemId = attachment.ItemId;
                mail_attackment.ItemCount = attachment.ItemCount;
                mail_attackment.ItemFromType = 1;
                list.Add(mail_attackment);
            }
        }

        DEF.IM.Mail mail = new()
        {
            MailGuid = Guid.NewGuid().ToString(),
            SenderMail = "",
            SenderNickName = "系统",
            SenderIcon = "",
            Title = cdkey.MailTitle,
            Msg = cdkey.MailDesc,
            Dt = DateTime.UtcNow,
            Attachments = list,
            State = 0
        };

        //SystemMail system_mail = new SystemMail();
        //system_mail.MailGuid = Guid.NewGuid().ToString();
        //system_mail.Mail = mail;
        //system_mail.TargetType = IMTargetType.Player;
        //system_mail.PlayerIdList = cdkey.PlayerIdList;
        //system_mail.RegionIdList = cdkey.RegionIdList;
        //system_mail.Dt = cdkey.CreateDt;
        //system_mail.ExpireDt = cdkey.ExpireDt;

        //var container_system_mailbox = GetContainerRpc<IContainerStatefulIMSystemMailBox>();
        //await container_system_mailbox.AddMail(system_mail);

        var container_player = GetContainerRpc<IContainerStatefulIMPlayer>(player_guid);
        await container_player.AddMail(mail);

        return IMErrorCode.NoError;
    }
}

#endif