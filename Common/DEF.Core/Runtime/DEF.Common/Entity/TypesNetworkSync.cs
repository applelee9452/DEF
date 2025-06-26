using System.Collections.Generic;

namespace DEF
{
    public enum NetworkSyncMode
    {
        None = 0,// 不同步
        StateSync,// 状态同步
        StateFrameSync,// 状态帧同步
    }
}