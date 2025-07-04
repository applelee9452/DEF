﻿namespace NPBehave
{
    public class Root : Decorator
    {
        private Node mainNode;

        //private Node inProgressNode;

        private Blackboard blackboard;
        public override Blackboard Blackboard
        {
            get
            {
                return blackboard;
            }
        }

        private Clock clock;
        public override Clock Clock
        {
            get
            {
                return clock;
            }
        }

#if UNITY_EDITOR
        public int TotalNumStartCalls = 0;
        public int TotalNumStopCalls = 0;
        public int TotalNumStoppedCalls = 0;
#endif

        public Root(Behave behave, Node mainNode)
            : base(behave, "Root", mainNode)
        {
            this.mainNode = mainNode;
            this.clock = Behave.Clock;
            this.blackboard = new Blackboard(this.clock);
            this.SetRoot(this);
        }

        public Root(Behave behave, Blackboard blackboard, Node mainNode)
            : base(behave, "Root", mainNode)
        {
            this.blackboard = blackboard;
            this.mainNode = mainNode;
            this.clock = Behave.Clock;
            this.SetRoot(this);
        }

        public Root(Behave behave, Blackboard blackboard, Clock clock, Node mainNode)
            : base(behave, "Root", mainNode)
        {
            this.blackboard = blackboard;
            this.mainNode = mainNode;
            this.clock = clock;
            this.SetRoot(this);
        }

        public override void SetRoot(Root rootNode)
        {
            //Assert.AreEqual(this, rootNode);
            base.SetRoot(rootNode);
            this.mainNode.SetRoot(rootNode);
        }

        override protected void DoStart()
        {
            this.blackboard.Enable();
            this.mainNode.Start();
        }

        override protected void DoStop()
        {
            if (this.mainNode.IsActive)
            {
                this.mainNode.Stop();
            }
            else
            {
                this.clock.RemoveTimer(this.mainNode.Start);
            }
        }

        override protected void DoChildStopped(Node node, bool success)
        {
            if (!IsStopRequested)
            {
                // wait one tick, to prevent endless recursions
                this.clock.AddTimer(0, 0, this.mainNode.Start);
            }
            else
            {
                this.blackboard.Disable();
                Stopped(success);
            }
        }
    }
}