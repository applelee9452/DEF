

using System;

namespace DEF
{
    public enum BehaviorReturnCode
    {
        Failure,
        Success,
        Running
    }

    public delegate BehaviorReturnCode BehaviorReturn();

    public class BehaviorTree
    {

        RootSelector mRoot;
        BehaviorReturnCode mReturnCode;
        Blackboard mBlackboard = new();


        public BehaviorReturnCode ReturnCode
        {
            get { return mReturnCode; }
            set { mReturnCode = value; }
        }

        public Blackboard Blackboard { get { return mBlackboard; } }


        public BehaviorTree()
        {
        }


        public void setRoot(RootSelector root)
        {
            mRoot = root;
        }


        public void setRoot(BehaviorComponent behavior)
        {
            RootSelector root = new(this, delegate () { return 0; }, behavior);
            mRoot = root;
        }


        public BehaviorReturnCode Behave()
        {
            try
            {
                switch (mRoot.Behave())
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
                        ReturnCode = BehaviorReturnCode.Running;
                        return ReturnCode;
                }
            }
            catch (Exception)
            {
                ReturnCode = BehaviorReturnCode.Failure;
                return ReturnCode;
            }
        }
    }
}