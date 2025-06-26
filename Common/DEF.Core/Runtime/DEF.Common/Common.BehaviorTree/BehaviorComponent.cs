

namespace DEF
{
    public abstract class BehaviorComponent
    {

        protected BehaviorReturnCode ReturnCode;
        protected BehaviorTree mBehaviorTree;


        public BehaviorTree BehaviorTree { get { return mBehaviorTree; } }


        public BehaviorComponent(BehaviorTree bt) { mBehaviorTree = bt; }


        public abstract BehaviorReturnCode Behave();
    }
}