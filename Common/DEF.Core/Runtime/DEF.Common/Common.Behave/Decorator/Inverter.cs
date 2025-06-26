namespace NPBehave
{
    public class Inverter : Decorator
    {
        public Inverter(Behave behave, Node decoratee)
            : base(behave, "Inverter", decoratee)
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
            Stopped(!result);
        }
    }
}