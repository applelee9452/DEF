#if DEF_CLIENT

using System;
using UnityEngine;

public class UGuiWidget : MonoBehaviour
{
    public Action ActionExcute { get; set; }

    public Action ActionOnDestroy { get; set; }
    public void Excute()
    {
        ActionExcute?.Invoke();
        ActionExcute = null;
    }
    private void OnDestroy()
    {
        ActionOnDestroy?.Invoke();
        ActionOnDestroy = null;
    }
}

#endif