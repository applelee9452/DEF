

using System;

namespace DEF
{
    public class Sequence : BehaviorComponent
    {

        private BehaviorComponent[] _behaviors;


        // attempts to run the behaviors all in one cycle
        // -Returns Success when all are successful
        // -Returns Failure if one behavior fails or an error occurs
        // -Returns Running if any are running
        public Sequence(BehaviorTree bt, params BehaviorComponent[] behaviors)
            : base(bt)
        {
            _behaviors = behaviors;
        }


        public override BehaviorReturnCode Behave()
        {
            //add watch for any running behaviors
            bool any_running = false;

            for (int i = 0; i < _behaviors.Length; i++)
            {
                try
                {
                    switch (_behaviors[i].Behave())
                    {
                        case BehaviorReturnCode.Failure:
                            ReturnCode = BehaviorReturnCode.Failure;
                            return ReturnCode;
                        case BehaviorReturnCode.Success:
                            continue;
                        case BehaviorReturnCode.Running:
                            any_running = true;
                            continue;
                        default:
                            ReturnCode = BehaviorReturnCode.Success;
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

            // if none running, return success, otherwise return running
            ReturnCode = !any_running ? BehaviorReturnCode.Success : BehaviorReturnCode.Running;
            return ReturnCode;
        }
    }
}