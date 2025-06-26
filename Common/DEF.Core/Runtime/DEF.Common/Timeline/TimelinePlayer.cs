using System.Collections.Generic;

namespace DEF
{
    public class TimelinePlayer
    {
        public float CurrentPos { get; set; }// 当前播放到什么位置了
        public Timeline Timeline { get; private set; }
        public Dictionary<string, object> Blackboard { get; private set; } = new();

        public class TrackData
        {
            public int ClipIndex { get; set; }
            public float ClipTime { get; set; }
        }

        public TrackData[] TrackPlayers;

        public TimelinePlayer(Timeline timeline)
        {
            this.Timeline = timeline;

            TrackPlayers = new TrackData[timeline.ListTrack.Count];
            for (int i = 0; i < TrackPlayers.Length; ++i)
            {
                TrackPlayers[i] = new TrackData();
            }
        }

        public void SetBlackboard(string key, object value)
        {
            Blackboard[key] = value;
        }

        public void Play()
        {
            CurrentPos = 0;
            var ListTrack = Timeline.ListTrack;

            for (int i = 0; i < TrackPlayers.Length; ++i)
            {
                TrackData t = TrackPlayers[i];
                t.ClipTime = 0;
                t.ClipIndex = 0;
                var clip = ListTrack[i].ListClip[0];
                StartClip(clip, t);
            }
        }

        public void Cancel()
        {
        }

        public bool Update(float tm)
        {
            CurrentPos += tm;
            int flag = 0;
            var ListTrack = Timeline.ListTrack;
            for (int i = 0; i < ListTrack.Count; ++i)
            {
                int f = 1 << i;
                flag |= f;
                var track = ListTrack[i];
                var player = TrackPlayers[i];
                player.ClipTime += tm;
                if (UpdateTrack(track, player, tm))
                {
                    flag &= ~f;
                }
            }
            if (flag == 0)
            {
                return true;
            }
            return false;
        }

        private bool UpdateTrack(TimelineTrack track, TrackData player, float tm)
        {
            var ClipIndex = player.ClipIndex;
            var ListClip = track.ListClip;
            if (ClipIndex < ListClip.Count)
            {
                var clip = ListClip[ClipIndex];
                clip.OnUpdate(this, tm);
                if (player.ClipTime > clip.Data.ClipLength)
                {
                    EndClip(clip);
                    player.ClipIndex = ++ClipIndex;
                    if (ClipIndex < ListClip.Count)
                    {
                        var clipNew = ListClip[ClipIndex];
                        StartClip(clipNew, player);
                    }
                }
            }
            return ClipIndex >= ListClip.Count;
        }

        private void StartClip(TimelineClip clip, TrackData player)
        {
            clip.OnStart(this);
            player.ClipTime = 0;
        }

        private void EndClip(TimelineClip clip)
        {
            clip.OnEnd(this);
        }
    }
}
