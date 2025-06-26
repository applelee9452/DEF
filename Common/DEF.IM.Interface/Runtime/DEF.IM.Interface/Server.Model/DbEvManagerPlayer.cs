#if !DEF_CLIENT

namespace DEF.IM;

// 管理平台修改指定玩家GM
public class EvManagerChangePlayerGM : EventBase
{
    public string PlayerGuid { get; set; }
    public string ManagerUserId { get; set; }// 管理平台操作人Id，可以为空
    public bool SetIsGM { get; set; }
    public bool IsGMAfterChange { get; set; }
}

// 管理平台修改指定玩家封号
public class EvManagerChangePlayerForbidden : EventBase
{
    public string PlayerGuid { get; set; }
    public string ManagerUserId { get; set; }// 管理平台操作人Id，可以为空
    public bool SetIsForbidden { get; set; }
    public bool IsForbiddenAfterChange { get; set; }
}

#endif