#if DEF_CLIENT

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public static class TypeEx
{
    public static T GetDelegate<T>(this Type type, string method) where T : Delegate
    {
        var methodInfo = type.GetMethod(method);
        if (methodInfo == null)
        {
            Debug.LogWarning($"Method [{method}] Not Exsit");
            return null;
        }
        T dele = (T)methodInfo.CreateDelegate(typeof(T));
        if (dele == null)
        {
            Debug.LogWarning($"Method [{method}] Delegate Type Error");
            return null;
        }
        return dele;
    }
}

public class AssemblesWrapper
{
    string EntryClassName;
    readonly object[] Obj = new object[0];
    readonly object[] Obj1 = new object[1];
    bool DelayCreate = false;
    bool Created = false;
    Dictionary<string, string> CreateParams;
    Func<Dictionary<string, string>, Task> FuncOnCreate = null;
    Func<Task> FuncOnDestroy = null;
    Action<float> FuncOnUpdate = null;
    Action FuncOnConfirmQuit = null;
    Action<bool> FuncOnApplicationPause = null;
    Action<bool> FuncOnApplicationFocus = null;
    Action FuncOnTest = null;
    Action<GameObject, GameObject, string> FuncOnUGuiCreateCom = null;
    Action<string, GameObject, BaseEventData> FuncOnUGuiMove = null;
    Action<string, string, GameObject> FuncOnUGuiButtonClick = null;
    Action<string, string, float> FuncOnUGuiSliderValueChange = null;
    Action<GameObject, string> FuncOnUGuiMsg = null;
    Action<GameObject, string> FuncOnUnityMsg0 = null;
    Action<GameObject, string, object> FuncOnUnityMsg1 = null;
    Func<string, string> FuncGetLanguageParam = null;

    public Task Create(bool delay_create, Assembly ass_entry, string entry_class_name, Dictionary<string, string> create_params)
    {
        EntryClassName = entry_class_name;
        CreateParams = create_params;
        DelayCreate = delay_create;

        var ass = ass_entry;

        var t_entryclassname = ass.GetType(EntryClassName);
        FuncOnCreate = t_entryclassname.GetDelegate<Func<Dictionary<string, string>, Task>>("Create");
        FuncOnDestroy = t_entryclassname.GetDelegate<Func<Task>>("Destroy");
        FuncOnUpdate = t_entryclassname.GetDelegate<Action<float>>("Update");
        FuncOnConfirmQuit = t_entryclassname.GetDelegate<Action>("OnConfirmQuit");
        FuncOnApplicationPause = t_entryclassname.GetDelegate<Action<bool>>("OnApplicationPause");
        FuncOnApplicationFocus = t_entryclassname.GetDelegate<Action<bool>>("OnApplicationFocus");
        FuncOnTest = t_entryclassname.GetDelegate<Action>("OnTest");
        FuncOnUGuiCreateCom = t_entryclassname.GetDelegate<Action<GameObject, GameObject, string>>("OnUGuiCreateCom");
        FuncOnUGuiMove = t_entryclassname.GetDelegate<Action<string, GameObject, BaseEventData>>("OnUGuiMove");
        FuncOnUGuiButtonClick = t_entryclassname.GetDelegate<Action<string, string, GameObject>>("OnUGuiButtonClick");
        FuncOnUGuiSliderValueChange = t_entryclassname.GetDelegate<Action<string, string, float>>("OnUGuiSliderValueChange");
        FuncOnUGuiMsg = t_entryclassname.GetDelegate<Action<GameObject, string>>("OnUGuiMsg");
        FuncOnUnityMsg0 = t_entryclassname.GetDelegate<Action<GameObject, string>>("OnUnityMsg0");
        FuncOnUnityMsg1 = t_entryclassname.GetDelegate<Action<GameObject, string, object>>("OnUnityMsg1");
        FuncGetLanguageParam = t_entryclassname.GetDelegate<Func<string, string>>("GetLanguageParam");
        if (DelayCreate)
        {
            return Task.CompletedTask;
        }
        else
        {
            Created = true;

            return (Task)FuncOnCreate(CreateParams);
        }
    }


    public async Task Destroy()
    {
        if (FuncOnDestroy != null)
        {
            await (Task)FuncOnDestroy();

            FuncOnCreate = null;
            FuncOnUpdate = null;
            FuncOnConfirmQuit = null;
            FuncOnApplicationPause = null;
            FuncOnApplicationFocus = null;
            FuncOnTest = null;
            FuncOnUGuiCreateCom = null;
            FuncOnUGuiMove = null;
            FuncOnUGuiButtonClick = null;
            FuncOnUGuiSliderValueChange = null;
            FuncOnUGuiMsg = null;
            FuncGetLanguageParam = null;
            FuncOnDestroy = null;
        }
    }

    public Task OnDelayCreate()
    {
        if (DelayCreate)
        {
            Created = true;

            return (Task)FuncOnCreate(CreateParams);
        }

        return Task.CompletedTask;
    }

    public void OnUpdate(float tm)
    {
        if (FuncOnUpdate != null && Created)
        {
            FuncOnUpdate(tm);
        }
    }

    public void OnApplicationPause(bool pause)
    {
        if (FuncOnApplicationPause != null && Created)
        {
            FuncOnApplicationPause(pause);
        }
    }

    public void OnApplicationFocus(bool focus_status)
    {
        if (FuncOnApplicationFocus != null && Created)
        {
            FuncOnApplicationFocus(focus_status);
        }
    }

    public void OnTest()
    {
        if (FuncOnTest != null && Created)
        {
            FuncOnTest();
        }
    }

    public void OnConfirmQuit()
    {
        if (FuncOnConfirmQuit != null && Created)
        {
            FuncOnConfirmQuit();
        }
    }

    public void OnUGuiCreateCom(GameObject view, GameObject obj, string script_com_name)
    {
        if (FuncOnUGuiCreateCom != null && Created)
        {
            FuncOnUGuiCreateCom(view, obj, script_com_name);
        }
    }

    public void OnUGuiMove(string view_name, GameObject go, BaseEventData data)
    {
        if (FuncOnUGuiMove != null && Created)
        {
            FuncOnUGuiMove(view_name, go, data);
        }
    }

    public void OnUGuiButtonClick(string view_name, string button_name, GameObject go_button)
    {
        if (FuncOnUGuiButtonClick != null && Created)
        {
            FuncOnUGuiButtonClick(view_name, button_name, go_button);
        }
    }

    public void OnUGuiSliderValueChange(string view_name, string silder_name, float value)
    {
        if (FuncOnUGuiSliderValueChange != null && Created)
        {
            FuncOnUGuiSliderValueChange(view_name, silder_name, value);
        }
    }

    public void OnUGuiMsg(GameObject go, string msg)
    {
        if (FuncOnUGuiMsg != null && Created)
        {
            FuncOnUGuiMsg(go, msg);
        }
    }

    public void OnUnityMsg0(GameObject go, string msg)
    {
        if (FuncOnUnityMsg0 != null && Created)
        {
            FuncOnUnityMsg0(go, msg);
        }
    }

    public void OnUnityMsg1(GameObject go, string msg, object param1)
    {
        if (FuncOnUnityMsg1 != null && Created)
        {
            FuncOnUnityMsg1(go, msg, param1);
        }
    }

    public string GetLanguageParam(string key)
    {
        if (FuncGetLanguageParam != null && Created)
        {
            return FuncGetLanguageParam(key);
        }

        return string.Empty;
    }
}

#endif