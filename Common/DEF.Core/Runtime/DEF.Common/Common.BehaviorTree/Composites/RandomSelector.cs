

using System;

namespace DEF
{
    // Randomly selects and performs one of the passed behaviors
    // -Returns Success if selected behavior returns Success
    // -Returns Failure if selected behavior returns Failure
    // -Returns Running if selected behavior returns Running
    public class RandomSelector : BehaviorComponent
    {

        private BehaviorComponent[] _Behaviors;
        private Random _Random = new(DateTime.Now.Millisecond);


        public RandomSelector(BehaviorTree bt, params BehaviorComponent[] behaviors)
            : base(bt)
        {
            _Behaviors = behaviors;
        }


        public override BehaviorReturnCode Behave()
        {
            try
            {
                switch (_Behaviors[_Random.Next(0, _Behaviors.Length - 1)].Behave())
                {
                    case BehaviorReturnCode.Failure:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                    case BehaviorReturnCode.Success:
                        ReturnCode = BehaviorReturnCode.Success;
                        return ReturnCode;
                    case BehaviorReturnCode.Running:
                        ReturnCode = BehaviorReturnCode.Running;
                        return ReturnCode;
                    default:
                        ReturnCode = BehaviorReturnCode.Failure;
                        return ReturnCode;
                }
            }
            catch (Exception)
            {
                //EbLog.Error(e.ToString());
                ReturnCode = BehaviorReturnCode.Failure;
                return ReturnCode;
            }
        }
    }
}