namespace NPBehave
{
    public class WaitUntilStopped : Task
    {
        private bool sucessWhenStopped;

        public WaitUntilStopped(Behave behave, bool sucessWhenStopped = false)
            : base(behave, "WaitUntilStopped")
        {
            this.sucessWhenStopped = sucessWhenStopped;
        }

        protected override void DoStop()
        {
            this.Stopped(sucessWhenStopped);
        }
    }
}