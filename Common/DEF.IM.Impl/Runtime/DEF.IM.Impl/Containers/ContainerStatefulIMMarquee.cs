#if !DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MemoryPack;
using Orleans;
using ProtoBuf;

namespace DEF.IM;

public enum SStreamId
{
    None = 0,
    GroupChatMsg,// 群聊消息广播
    Marquee,// 跑马灯消息广播
    RegionChatMsg,// 区域消息广播
    RegionSystemMail,// 区域邮件
}

[ProtoContract]
#if !DEF_CLIENT
[GenerateSerializer]
#endif
public struct SStreamInfo
{
    [ProtoMember(1)]
#if !DEF_CLIENT
    [Id(0)]
#endif
    public SStreamId Id { get; set; }

    [ProtoMember(2)]
#if !DEF_CLIENT
    [Id(1)]
#endif
    public byte[] Data { get; set; }
}

// 跑马灯，全部在线玩家广播，不区分服务器
public class ContainerStatefulIMMarquee : ContainerStateful, IContainerStatefulIMMarquee
{
    ContainerStatefulStream<SStreamInfo> StreamIMMarquee { get; set; }
    IDisposable TimerHandleUpdate { get; set; }
    Stopwatch StopwatchUpdate { get; set; }
    Queue<IMMarqueeEx> QueMarqueeDestory { get; set; } = new Queue<IMMarqueeEx>();
    HashSet<IMMarqueeEx> SetMarquee { get; set; } = new HashSet<IMMarqueeEx>();

    public override Task OnCreate()
    {
        StreamIMMarquee = CreateStream<SStreamInfo>(StringDef.StreamNameSpaceMarquee, StringDef.StreamGuidMarquee);

        StopwatchUpdate = new Stopwatch();
        StopwatchUpdate.Start();
        TimerHandleUpdate = RegisterTimer((_) => TimerUpdate(),
            null, TimeSpan.FromMilliseconds(1000), TimeSpan.FromMilliseconds(1000));

        return Task.CompletedTask;
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

        return Task.CompletedTask;
    }

    // 保活
    Task IContainerStatefulIMMarquee.Touch()
    {
        return Task.CompletedTask;
    }

    // 跑马灯广播，直接广播发送
    async Task IContainerStatefulIMMarquee.BroadcastMarquee(BIMMarquee im_marquee)
    {
        // 敏感词过滤
        //var grain = GrainFactory.GetGrain<IGrainSensitiveWordFilter>(StringDef.GrainSensitiveWordFilter);
        //string msg_new = await grain.Replace(im_marquee.Msg);
        //im_marquee.Msg = msg_new;

        SStreamInfo stream_info = new()
        {
            Id = SStreamId.Marquee,
            Data = MemoryPackSerializer.Serialize(im_marquee)
        };

        await StreamIMMarquee.OnNextAsync(stream_info);
    }

    // 跑马灯广播，可定制发送次数，延迟发送时间，发送间隔（单位：秒）
    Task IContainerStatefulIMMarquee.BroadcastMarqueeEx(BIMMarquee im_marquee, int broadcast_count, float broadcast_due, float broadcast_period)
    {
        if (broadcast_count < 0) broadcast_count = 0;
        if (broadcast_count > 100) broadcast_count = 100;

        if (broadcast_due < 0f) broadcast_due = 0f;
        if (broadcast_due > 3600f * 24) broadcast_due = 3600f * 24;

        if (broadcast_period < 5f) broadcast_period = 5f;
        if (broadcast_period > 3600f * 24) broadcast_period = 3600f * 24;

        for (int i = 0; i < broadcast_count; i++)
        {
            IMMarqueeEx im_marquee_ex = new()
            {
                im_marquee = im_marquee,
                total_tm = broadcast_due + i * broadcast_period,
                elapsed_tm = 0f
            };

            SetMarquee.Add(im_marquee_ex);
        }

        return Task.CompletedTask;
    }

    // 添加跑马灯预设，会保存到DB中
    Task IContainerStatefulIMMarquee.AddMarqueePrefab(DataMarquee data_im_marquee)
    {
        //TexasContext.Instance.Mongo.ReplaceOneData(StringDef.DbCollectionDataIMMarquee, data_im_marquee._id, data_im_marquee);

        return Task.CompletedTask;
    }

    // 删除跑马灯预设
    Task IContainerStatefulIMMarquee.RemoveMarqueePrefab(string data_im_marque_guid)
    {
        //TexasContext.Instance.Mongo.DeleteOneAsync<DataIMMarquee>(
        //   StringDef.DbCollectionDataIMMarquee, data_im_marque_guid);

        return Task.CompletedTask;
    }

    // 获取所有跑马灯预设
    async Task<List<DataMarquee>> IContainerStatefulIMMarquee.GetAllMarqueePrefab()
    {
        //var list_marquee = await TexasContext.Instance.Mongo.ReadListAsync<DataIMMarquee>(
        //     e => !string.IsNullOrEmpty(e._id), StringDef.DbCollectionDataIMMarquee);

        //if (list_marquee == null)
        //{
        //    list_marquee = new List<DataIMMarquee>();
        //}

        //return list_marquee;

        List<DataMarquee> list = [];

        await Task.Delay(1);

        return list;
    }

    Task TimerUpdate()
    {
        float tm = (float)StopwatchUpdate.Elapsed.TotalSeconds;
        StopwatchUpdate.Restart();

        if (SetMarquee.Count > 0)
        {
            foreach (var i in SetMarquee)
            {
                i.elapsed_tm += tm;

                if (i.elapsed_tm > i.total_tm)
                {
                    QueMarqueeDestory.Enqueue(i);

                    SStreamInfo stream_info = new()
                    {
                        Id = SStreamId.Marquee,
                        Data = MemoryPackSerializer.Serialize(i)
                    };

                    StreamIMMarquee.OnNextAsync(stream_info);
                }
            }
        }

        while (QueMarqueeDestory.Count > 0)
        {
            var i = QueMarqueeDestory.Dequeue();
            SetMarquee.Remove(i);
        }

        return Task.CompletedTask;
    }
}

#endif