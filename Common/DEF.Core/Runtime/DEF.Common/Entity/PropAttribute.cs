using System;

namespace DEF
{
    public enum PropType : int
    {
        Default = 0,
        SyncObj,
    }

    public enum PropSyncFlag : int
    {
        SyncNone = 0,
        SyncDbOnly,
        SyncNetworkOnly,
        SyncDbAndNetwork
    }

    public enum PropSyncMode : int
    {
        Set = 0,
        Lerp,
    }

    public enum PropCallback : int
    {
        No = 0,
        Yes,
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropAttribute : Attribute
    {
        public PropType PropType;
        public PropSyncFlag PropSyncFlag;
        public PropSyncMode PropSyncMode;
        public PropCallback PropCallback;
        public string DefaultValue;

        public PropAttribute(PropType prop_type, PropSyncFlag sync_flag, PropSyncMode sync_mode, PropCallback cb, string default_value)
        {
            PropType = prop_type;
            PropSyncFlag = sync_flag;
            PropSyncMode = sync_mode;
            PropCallback = cb;
            DefaultValue = default_value;
        }
    }
}