﻿namespace NPBehave
{
    public class TimeMax : Decorator
    {
        private float limit = 0.0f;
        private float randomVariation;
        private bool waitForChildButFailOnLimitReached = false;
        private bool isLimitReached = false;

        public TimeMax(Behave behave, float limit, bool waitForChildButFailOnLimitReached, Node decoratee)
            : base(behave, "TimeMax", decoratee)
        {
            this.limit = limit;
            this.randomVariation = limit * 0.05f;
            this.waitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
            //Assert.IsTrue(limit > 0f, "limit has to be set");
        }

        public TimeMax(Behave behave, float limit, float randomVariation, bool waitForChildButFailOnLimitReached, Node decoratee)
            : base(behave, "TimeMax", decoratee)
        {
            this.limit = limit;
            this.randomVariation = randomVariation;
            this.waitForChildButFailOnLimitReached = waitForChildButFailOnLimitReached;
            //Assert.IsTrue(limit > 0f, "limit has to be set");
        }

        protected override void DoStart()
        {
            this.isLimitReached = false;
            Clock.AddTimer(limit, randomVariation, 0, TimeoutReached);
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Clock.RemoveTimer(TimeoutReached);
            if (Decoratee.IsActive)
            {
                Decoratee.Stop();
            }
            else
            {
                Stopped(false);
            }
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Clock.RemoveTimer(TimeoutReached);
            if (isLimitReached)
            {
                Stopped(false);
            }
            else
            {
                Stopped(result);
            }
        }

        private void TimeoutReached()
        {
            if (!waitForChildButFailOnLimitReached)
            {
                Decoratee.Stop();
            }
            else
            {
                isLimitReached = true;
                //Assert.IsTrue(Decoratee.IsActive);
            }
        }
    }
}