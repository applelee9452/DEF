namespace NPBehave
{
    public class Wait : Task
    {
        private System.Func<float> function = null;
        private string blackboardKey = null;
        private float seconds = -1f;
        private float randomVariance;

        public float RandomVariance
        {
            get
            {
                return randomVariance;
            }
            set
            {
                randomVariance = value;
            }
        }

        public Wait(Behave behave, float seconds, float randomVariance)
            : base(behave, "Wait")
        {
            //UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);
            this.seconds = seconds;
            this.randomVariance = randomVariance;
        }

        public Wait(Behave behave, float seconds)
            : base(behave, "Wait")
        {
            this.seconds = seconds;
            this.randomVariance = this.seconds * 0.05f;
        }

        public Wait(Behave behave, string blackboardKey, float randomVariance = 0f)
            : base(behave, "Wait")
        {
            this.blackboardKey = blackboardKey;
            this.randomVariance = randomVariance;
        }

        public Wait(Behave behave, System.Func<float> function, float randomVariance = 0f)
            : base(behave, "Wait")
        {
            this.function = function;
            this.randomVariance = randomVariance;
        }

        protected override void DoStart()
        {
            float seconds = this.seconds;
            if (seconds < 0)
            {
                if (this.blackboardKey != null)
                {
                    seconds = Blackboard.Get<float>(this.blackboardKey);
                }
                else if (this.function != null)
                {
                    seconds = this.function();
                }
            }

            //UnityEngine.Assertions.Assert.IsTrue(seconds >= 0);

            if (seconds < 0)
            {
                seconds = 0;
            }

            if (randomVariance >= 0f)
            {
                Clock.AddTimer(seconds, randomVariance, 0, onTimer);
            }
            else
            {
                Clock.AddTimer(seconds, 0, onTimer);
            }
        }

        protected override void DoStop()
        {
            Clock.RemoveTimer(onTimer);
            this.Stopped(false);
        }

        private void onTimer()
        {
            Clock.RemoveTimer(onTimer);
            this.Stopped(true);
        }
    }
}