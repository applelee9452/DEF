

using System;

namespace DEF
{
    public class Conditional : BehaviorComponent
    {

        private Func<BehaviorTree, object[], bool> mFuncBool;
        private object[] mListParam;


        // Returns a return code equivalent to the test 
        // -Returns Success if true
        // -Returns Failure if false
        // <param name="test">the value to be tested</param>
        public Conditional(BehaviorTree bt, Func<BehaviorTree, object[], bool> func_bool, params object[] list_param)
            : base(bt)
        {
            mFuncBool = func_bool;
            mListParam = list_param;
        }


        public override BehaviorReturnCode Behave()
        {
            try
            {
                if (mFuncBool.Invoke(mBehaviorTree, mListParam))
                {
                    ReturnCode = BehaviorReturnCode.Success;
                    return ReturnCode;
                }
                else
                {
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