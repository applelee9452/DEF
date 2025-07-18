﻿

using System;

namespace DEF
{
    public class Selector : BehaviorComponent
    {

        protected BehaviorComponent[] _Behaviors;


        // Selects among the given behavior components
        // Performs an OR-Like behavior and will "fail-over" to each successive component until Success is reached or Failure is certain
        // -Returns Success if a behavior component returns Success
        // -Returns Running if a behavior component returns Running
        // -Returns Failure if all behavior components returned Failure
        public Selector(BehaviorTree bt, params BehaviorComponent[] behaviors)
            : base(bt)
        {
            _Behaviors = behaviors;
        }


        public override BehaviorReturnCode Behave()
        {
            for (int i = 0; i < _Behaviors.Length; i++)
            {
                try
                {
                    switch (_Behaviors[i].Behave())
                    {
                        case BehaviorReturnCode.Failure:
                            continue;
                        case BehaviorReturnCode.Success:
                            ReturnCode = BehaviorReturnCode.Success;
                            return ReturnCode;
                        case BehaviorReturnCode.Running:
                            ReturnCode = BehaviorReturnCode.Running;
                            return ReturnCode;
                        default:
                            continue;
                    }
                }
                catch (Exception)
                {
                    //EbLog.Error(e.ToString());
                    continue;
                }
            }

            ReturnCode = BehaviorReturnCode.Failure;
            return ReturnCode;
        }
    }
}