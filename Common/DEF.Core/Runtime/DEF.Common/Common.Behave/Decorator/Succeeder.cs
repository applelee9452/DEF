namespace NPBehave
{
    public class Succeeder : Decorator
    {
        public Succeeder(Behave behave, Node decoratee)
            : base(behave, "Succeeder", decoratee)
        {
        }

        protected override void DoStart()
        {
            Decoratee.Start();
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Stopped(true);
        }
    }
}