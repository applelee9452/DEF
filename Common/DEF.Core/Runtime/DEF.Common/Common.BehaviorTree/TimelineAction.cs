using NPBehave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEF
{
    public class TimelineAction : NPBehave.Task
    {
        private TimelinePlayer TimelinePlayer;
        public TimelineAction(Behave behave, Timeline timeline) : base(behave, "Timeline")
        {
            TimelinePlayer = new TimelinePlayer(timeline);
        }

        protected override void DoStart()
        {
            base.DoStart();
            TimelinePlayer.Play();
            RootNode.Clock.AddUpdateObserver(OnUpdateTimeline);
        }
        private double LastElapsedTime;
        private void OnUpdateTimeline()
        {
            var time = RootNode.Clock.ElapsedTime;
            var delta = time - LastElapsedTime;
            LastElapsedTime = time;
            if (TimelinePlayer.Update((float)delta))
            {
                RootNode.Clock.RemoveUpdateObserver(OnUpdateTimeline);
                Stopped(true);
            }

        }

        protected override void DoStop()
        {
            base.DoStop();
            RootNode.Clock.RemoveUpdateObserver(OnUpdateTimeline);
            TimelinePlayer.Cancel();
            Stopped(false);
        }

        public void SetBlackboard(string key, object value)
        {
            TimelinePlayer.SetBlackboard(key, value);
        }
    }
}
