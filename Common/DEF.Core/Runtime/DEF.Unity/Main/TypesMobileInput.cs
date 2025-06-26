#if DEF_CLIENT

using UnityEngine;

public enum MobileInputType
{
    None = 0,
    PointerDown,
    PointerUp,
    BeginDrag,
    OnDrag,
    EndDrag,
    OnActiveSkill,
    OnCastSkill,    
    OnDragSkill,
    OnCancelSkill,
}

public class MobileInputInfo
{
    public MobileInputType InputType { get; set; }
    public string KeyName { get; set; }
    public int Key { get; set; }
    public Vector3 Dir { get; set; }

    public override string ToString()
    {
        return $"InputType={InputType},KeyName={KeyName},Key={Key},Dir={Dir}";
    }
}

#endif