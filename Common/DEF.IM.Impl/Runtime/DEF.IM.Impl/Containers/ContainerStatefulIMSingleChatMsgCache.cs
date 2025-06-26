#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DEF.IM;

// 单聊消息缓存，一对玩家一个实例
public class ContainerStatefulIMSingleChatMsgCache : ContainerStateful, IContainerStatefulIMSingleChatMsgCache
{
    List<SingleChatMsgRecv> ListMsgRecv { get; set; } = [];
    string PlayerGuid1 { get; set; }
    PlayerInfo PlayerInfo1 { get; set; }
    string PlayerGuid2 { get; set; }
    PlayerInfo PlayerInfo2 { get; set; }
    ulong CurrentMsgId { get; set; }

    public override async Task OnCreate()
    {
        string grain_key = ContainerId;
        int index = grain_key.IndexOf('_');
        PlayerGuid1 = grain_key.Substring(0, index);
        PlayerGuid2 = grain_key.Substring(index + 1, grain_key.Length - index - 1);
        CurrentMsgId = 0;

        var grain_im_player1 = GetContainerRpc<IContainerStatefulIMPlayer>(PlayerGuid1);
        var t1 = grain_im_player1.GetPlayerInfo();

        var grain_im_player2 = GetContainerRpc<IContainerStatefulIMPlayer>(PlayerGuid2);
        var t2 = grain_im_player2.GetPlayerInfo();

        await Task.WhenAll(t1, t2);

        PlayerInfo1 = t1.Result;
        PlayerInfo2 = t2.Result;

        // 从Db加载最新100条聊天记录
        var filter = Builders<DataSingleChatMsg>.Filter.Where(
            e => (e.SenderGuid == PlayerGuid1 && e.RecverGuid == PlayerGuid2) || (e.SenderGuid == PlayerGuid2 && e.RecverGuid == PlayerGuid1));
        var sorter = Builders<DataSingleChatMsg>.Sort.Descending(e => e.MsgId);
        var coll = IMContext.Instance.Mongo.GetCollection<DataSingleChatMsg>(
            StringDef.DbCollectionDataSingleChatMsg);
        var list_msg_record = await coll.Find(filter)
            .Sort(sorter)
            .Limit(100)
            .ToListAsync();

        if (list_msg_record != null && list_msg_record.Count > 0)
        {
            var chat_msg_record = list_msg_record[0];
            CurrentMsgId = chat_msg_record.MsgId;

            foreach (var i in list_msg_record)
            {
                SingleChatMsgRecv msg_revc = new()
                {
                    MsgId = i.MsgId,
                    SenderGuid = i.SenderGuid,
                    RecverGuid = i.RecverGuid,
                    Msg = i.Msg,
                    Dt = i.Dt
                };

                if (PlayerInfo1.PlayerGuid == i.SenderGuid)
                {
                    msg_revc.SenderNickName = PlayerInfo1.NickName;
                    msg_revc.SenderIcon = PlayerInfo1.Icon;
                    msg_revc.RecverNickName = PlayerInfo2.NickName;
                    msg_revc.RecverIcon = PlayerInfo2.Icon;
                }
                else
                {
                    msg_revc.SenderNickName = PlayerInfo2.NickName;
                    msg_revc.SenderIcon = PlayerInfo2.Icon;
                    msg_revc.RecverNickName = PlayerInfo1.NickName;
                    msg_revc.RecverIcon = PlayerInfo1.Icon;
                }

                ListMsgRecv.Add(msg_revc);
            }
        }
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    public static string GenGrainKey(string player_guid1, string player_guid2)
    {
        if (string.Compare(player_guid1, player_guid2) > 0)
        {
            return string.Format("{0}_{1}", player_guid1, player_guid2);
        }
        else
        {
            return string.Format("{0}_{1}", player_guid2, player_guid1);
        }
    }

    // 添加一条消息到聊天记录
    async Task IContainerStatefulIMSingleChatMsgCache.AddSingleChatMsg(SingleChatMsgSend msg)
    {
        // 保存私聊聊天记录
        ++CurrentMsgId;

        DataSingleChatMsg msg_record = new()
        {
            _id = Guid.NewGuid().ToString(),
            MsgId = CurrentMsgId,
            SenderGuid = msg.SenderGuid,
            RecverGuid = msg.RecverGuid,
            Msg = msg.Msg,
            Dt = DateTime.UtcNow
        };

        await IMContext.Instance.Mongo.InsertAsync(StringDef.DbCollectionDataSingleChatMsg, msg_record);

        // 广播给聊天双方
        SingleChatMsgRecv msg_revc = new()
        {
            MsgId = msg_record.MsgId,
            SenderGuid = msg.SenderGuid,
            RecverGuid = msg.RecverGuid,
            Msg = msg.Msg,
            Dt = DateTime.UtcNow
        };

        if (PlayerInfo1.PlayerGuid == msg.SenderGuid)
        {
            msg_revc.SenderNickName = PlayerInfo1.NickName;
            msg_revc.SenderIcon = PlayerInfo1.Icon;
            msg_revc.RecverNickName = PlayerInfo2.NickName;
            msg_revc.RecverIcon = PlayerInfo2.Icon;
        }
        else
        {
            msg_revc.SenderNickName = PlayerInfo2.NickName;
            msg_revc.SenderIcon = PlayerInfo2.Icon;
            msg_revc.RecverNickName = PlayerInfo1.NickName;
            msg_revc.RecverIcon = PlayerInfo1.Icon;
        }

        // 缓存最新的聊天记录
        ListMsgRecv.Insert(0, msg_revc);
        if (ListMsgRecv.Count > 100) ListMsgRecv.RemoveAt(ListMsgRecv.Count - 1);

        // 发送给双方玩家
        var grain_im1 = GetContainerRpc<IContainerStatefulIMPlayer>(PlayerGuid1);
        var task1 = grain_im1.RecvSingleChatMsg(msg_revc);

        var grain_im2 = GetContainerRpc<IContainerStatefulIMPlayer>(PlayerGuid2);
        var task2 = grain_im2.RecvSingleChatMsg(msg_revc);

        await Task.WhenAll(task1, task2);
    }

    // 从聊天记录找查询与指定玩家的最新聊天记录
    Task<List<SingleChatMsgRecv>> IContainerStatefulIMSingleChatMsgCache.GetSingleChatMsgRecord()
    {
        return Task.FromResult(ListMsgRecv);
    }
}

#endif