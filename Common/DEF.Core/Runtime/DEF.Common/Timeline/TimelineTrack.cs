using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DEF
{
    public class TimelineTrack
    {
        public int Index { get; private set; }
        public ReadOnlyCollection<TimelineClip> ListClip { get; set; }

        public TimelineTrack(TimelineTrackData data)
        {
            List<TimelineClip> clips = new List<TimelineClip>();
            Index = data.Index;
            if (data != null && data.ListClipData != null)
            {
                foreach (var i in data.ListClipData)
                {
                    var clip = TimelineFactory.CreateClip(i);
                    clips.Add(clip);
                }

                // Sort，按StartPos排序
            }
            ListClip = clips.AsReadOnly();
        }
    }
}