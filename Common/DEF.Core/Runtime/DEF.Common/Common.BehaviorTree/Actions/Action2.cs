

namespace DEF
{
    public class Action2 : BehaviorComponent
    {

        public Action2(BehaviorTree bt)
            : base(bt)
        {
        }


        public override BehaviorReturnCode Behave()
        {
            return BehaviorReturnCode.Success;
        }
    }
}