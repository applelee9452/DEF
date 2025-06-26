using System.Collections.Generic;

namespace DEF
{
    // 表现层剪辑，仅客户端播放，服务端忽略
    // 逻辑层剪辑，双端播放
    // 可通过Json，Bson序列化

    // 动画剪辑
    // 声音剪辑
    // 粒子特效剪辑
    // Entity事件剪辑

    public class TimelineClipData
    {
        public string ClipType { get; set; }// 剪辑类型
        public float ClipLength { get; set; }// 剪辑时长，秒数
        public Dictionary<string, string> UserData { get; set; }// todo，后续优化
    }

    public class TimelineTrackData
    {
        public byte Index { get; set; }// 轨道索引
        public List<TimelineClipData> ListClipData { get; set; }
    }

    public class TimelineData
    {
        public List<TimelineTrackData> ListTrackData { get; set; }
    }
}