﻿namespace NPBehave
{
    public abstract class ObservingDecorator : Decorator
    {
        protected Stops stopsOnChange;
        private bool isObserving;

        public ObservingDecorator(Behave behave, string name, Stops stopsOnChange, Node decoratee)
            : base(behave, name, decoratee)
        {
            this.stopsOnChange = stopsOnChange;
            this.isObserving = false;
        }

        protected override void DoStart()
        {
            if (stopsOnChange != Stops.NONE)
            {
                if (!isObserving)
                {
                    isObserving = true;
                    StartObserving();
                }
            }

            if (!IsConditionMet())
            {
                Stopped(false);
            }
            else
            {
                Decoratee.Start();
            }
        }

        override protected void DoStop()
        {
            Decoratee.Stop();
        }

        protected override void DoChildStopped(Node child, bool result)
        {
            //Assert.AreNotEqual(this.CurrentState, State.INACTIVE);

            if (stopsOnChange == Stops.NONE || stopsOnChange == Stops.SELF)
            {
                if (isObserving)
                {
                    isObserving = false;
                    StopObserving();
                }
            }
            Stopped(result);
        }

        override protected void DoParentCompositeStopped(Composite parentComposite)
        {
            if (isObserving)
            {
                isObserving = false;
                StopObserving();
            }
        }

        protected void Evaluate()
        {
            if (IsActive && !IsConditionMet())
            {
                if (stopsOnChange == Stops.SELF || stopsOnChange == Stops.BOTH || stopsOnChange == Stops.IMMEDIATE_RESTART)
                {
                    // Debug.Log( this.key + " stopped self ");
                    this.Stop();
                }
            }
            else if (!IsActive && IsConditionMet())
            {
                if (stopsOnChange == Stops.LOWER_PRIORITY || stopsOnChange == Stops.BOTH || stopsOnChange == Stops.IMMEDIATE_RESTART || stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART)
                {
                    // Debug.Log( this.key + " stopped other ");
                    Container parentNode = this.ParentNode;
                    Node childNode = this;
                    while (parentNode != null && !(parentNode is Composite))
                    {
                        childNode = parentNode;
                        parentNode = parentNode.ParentNode;
                    }

                    //Assert.IsNotNull(parentNode, "NTBtrStops is only valid when attached to a parent composite");
                    //Assert.IsNotNull(childNode);

                    if (parentNode is Parallel)
                    {
                        //Assert.IsTrue(stopsOnChange == Stops.IMMEDIATE_RESTART, "On Parallel Nodes all children have the same priority, thus Stops.LOWER_PRIORITY or Stops.BOTH are unsupported in this context!");
                    }

                    if (stopsOnChange == Stops.IMMEDIATE_RESTART || stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART)
                    {
                        if (isObserving)
                        {
                            isObserving = false;
                            StopObserving();
                        }
                    }

                    ((Composite)parentNode).StopLowerPriorityChildrenForChild(childNode, stopsOnChange == Stops.IMMEDIATE_RESTART || stopsOnChange == Stops.LOWER_PRIORITY_IMMEDIATE_RESTART);
                }
            }
        }

        protected abstract void StartObserving();

        protected abstract void StopObserving();

        protected abstract bool IsConditionMet();
    }
}