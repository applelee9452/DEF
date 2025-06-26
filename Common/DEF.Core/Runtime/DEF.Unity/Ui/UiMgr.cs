#if DEF_CLIENT

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiMgr : IDisposable
{
    static Dictionary<string, MonoBehaviour> MapUi { get; set; } = new();

    public UiMgr()
    {
        MapUi[typeof(UiLaunch).Name] = Client.Config.UiLaunch;
        MapUi[typeof(UiMsgBox).Name] = Client.Config.UiMsgBox;
    }

    void IDisposable.Dispose()
    {
        MapUi.Clear();

        Debug.Log("UiMgr.Dispose()");
    }

    public static T CreateUi<T>() where T : MonoBehaviour
    {
        var n = typeof(T).Name;
        MapUi.TryGetValue(n, out MonoBehaviour ui);

        if (ui == null)
        {
            return default;
        }

        ui.gameObject.SetActive(true);

        //Debug.Log($"{n} CreateUi");

        return (T)ui;
    }

    public static void DestroyUi<T>() where T : MonoBehaviour
    {
        var n = typeof(T).Name;
        MapUi.TryGetValue(n, out MonoBehaviour ui);

        if (ui == null)
        {
            return;
        }

        ui.gameObject.SetActive(false);

        //Debug.Log($"{n} Destroy");
    }

    public static T GetUi<T>() where T : MonoBehaviour
    {
        var n = typeof(T).Name;
        MapUi.TryGetValue(n, out MonoBehaviour ui);

        if (ui == null)
        {
            return default;
        }

        if (!ui.gameObject.activeSelf)
        {
            return default;
        }

        return (T)ui;
    }

    public static T GetOrCreateUi<T>() where T : MonoBehaviour
    {
        var n = typeof(T).Name;
        MapUi.TryGetValue(n, out MonoBehaviour ui);

        if (ui == null)
        {
            return default;
        }

        ui.gameObject.SetActive(true);

        //Debug.Log($"{n} GetOrCreateUi");

        return (T)ui;
    }
}

#endif