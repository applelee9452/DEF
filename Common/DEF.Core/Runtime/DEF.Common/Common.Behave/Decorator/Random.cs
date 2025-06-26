namespace NPBehave
{
    public class Random : Decorator
    {
        private float probability;
        System.Random Rd = new();// todo，改为整个行为树共享一个实例

        public Random(Behave behave, float probability, Node decoratee)
            : base(behave, "Random", decoratee)
        {
            this.probability = probability;
        }

        protected override void DoStart()
        {
            if (Rd.NextDouble() <= this.probability)
            {
                Decoratee.Start();
            }
            else
            {
                Stopped(false);
            }
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            Stopped(result);
        }
    }
}