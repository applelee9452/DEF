#if DEF_CLIENT

using UnityEngine;

// 小红点，待扩展支持数量显示的
public class UGuiRedPoint : MonoBehaviour
{
    public string Id;
    public int Num;

    UGuiView UGuiView { get; set; }

    void Start()
    {
        UGuiView = GetComponent<UGuiView>();
        if (UGuiView == null)
        {
            UGuiView = GetComponentInParent<UGuiView>();
        }
    }
}

#endif