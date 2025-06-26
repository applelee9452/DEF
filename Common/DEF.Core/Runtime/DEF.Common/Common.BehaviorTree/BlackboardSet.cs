using NPBehave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEF
{
    public class BlackboardSet : NPBehave.Task
    {
        private string key;
        private object value;

        public BlackboardSet(Behave behave, string key, object value) : base(behave, "Timeline")
        {
            this.key = key;
            this.value = value;
        }

        protected override void DoStart()
        {
            base.DoStart();
            RootNode.Blackboard.Set(key, value);
            Stopped(true);

        }

        //private double LastElapsedTime;
        protected override void DoStop()
        {
            base.DoStop();
            Stopped(false);
        }
    }
}