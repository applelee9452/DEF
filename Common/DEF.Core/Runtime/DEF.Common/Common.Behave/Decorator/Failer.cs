namespace NPBehave
{
    public class Failer : Decorator
    {
        public Failer(Behave behave, Node decoratee)
            : base(behave, "Failer", decoratee)
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
            Stopped(false);
        }
    }
}