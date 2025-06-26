using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DEF
{
    // todo，需要绑定在一个Entity上，Entity提供Timeline所需的依赖
    // todo，高性能，Timeline池化，每次获取Timeline实例不需要new
    public class Timeline
    {
        public ReadOnlyCollection<TimelineTrack> ListTrack { get; set; }

        public Timeline(TimelineData data)
        {
            List<TimelineTrack> tracks = new List<TimelineTrack>();
            if (data != null && data.ListTrackData != null)
            {
                foreach (var i in data.ListTrackData)
                {
                    var track = new TimelineTrack(i);
                    tracks.Add(track);
                }
            }
            ListTrack = tracks.AsReadOnly();
        }
    }
}