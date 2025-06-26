#if DEF_CLIENT

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Reflection;
using UnityEngine;

public static class GameViewUtils
{
    public enum GameViewSizeType
    {
        AspectRatio,
        FixedResolution,
    }

    static object GameViewSizesInstance;
    static MethodInfo MethodGetGroup;

#if UNITY_EDITOR
    static GameViewUtils()
    {

        var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        var instanceProp = singleType.GetProperty("instance");
        MethodGetGroup = sizesType.GetMethod("GetGroup");
        GameViewSizesInstance = instanceProp.GetValue(null, null);
    }

    //[MenuItem("Test/AddSize")]
    //public static void AddTestSize()
    //{
    //    AddCustomSize(GameViewSizeType.AspectRatio, GameViewSizeGroupType.Standalone, 123, 456, "Test size");
    //}

    //[MenuItem("Test/SizeTextQuery")]
    //public static void SizeTextQueryTest()
    //{
    //    Debug.Log(SizeExists(GameViewSizeGroupType.Standalone, "Test size"));
    //}

    //[MenuItem("Test/Query16:9Test")]
    //public static void WidescreenQueryTest()
    //{
    //    Debug.Log(SizeExists(GameViewSizeGroupType.Standalone, "16:9"));
    //}

    //[MenuItem("Test/Set16:9")]
    //public static void SetWidescreenTest()
    //{
    //    SetSize(FindSize(GameViewSizeGroupType.Standalone, "16:9"));
    //}

    //[MenuItem("Test/SetTestSize")]
    //public static void SetTestSize()
    //{
    //    int idx = FindSize(GameViewSizeGroupType.Standalone, 123, 456);
    //    if (idx != -1)
    //        SetSize(idx);
    //}

    public static void SetSize(int index)
    {
#if UNITY_EDITOR
        var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
#endif
    }

    //    [MenuItem("Test/SizeDimensionsQuery")]
    //    public static void SizeDimensionsQueryTest()
    //    {
    //#if UNITY_EDITOR
    //        Debug.Log(SizeExists(GameViewSizeGroupType.Standalone, 123, 456));
    //#endif
    //    }

    public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string text)
    {
#if UNITY_EDITOR
        var group = GetGroup(sizeGroupType);
        var addCustomSize = MethodGetGroup.ReturnType.GetMethod("AddCustomSize");// or group.GetType().
        var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
        var ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
        var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, text });
        addCustomSize.Invoke(group, new object[] { newSize });
#endif
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text)
    {
        return FindSize(sizeGroupType, text) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, string text)
    {
#if UNITY_EDITOR
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // string[] texts = group.GetDisplayTexts();
        // for loop...

        var group = GetGroup(sizeGroupType);
        var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
        var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
        for (int i = 0; i < displayTexts.Length; i++)
        {
            string display = displayTexts[i];
            // the text we get is "Name (W:H)" if the size has a name, or just "W:H" e.g. 16:9
            // so if we're querying a custom size text we substring to only get the name
            // You could see the outputs by just logging
            // Debug.Log(display);
            int pren = display.IndexOf('(');
            if (pren != -1)
                display = display.Substring(0, pren - 1); // -1 to remove the space that's before the prens. This is very implementation-depdenent
            if (display == text)
                return i;
        }

#endif
        return -1;
    }

    public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        return FindSize(sizeGroupType, width, height) != -1;
    }

    public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
#if UNITY_EDITOR
        // goal:
        // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
        // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
        // iterate through the sizes via group.GetGameViewSize(int index)

        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }
#endif

        return -1;
    }

    static object GetGroup(GameViewSizeGroupType type)
    {
        return MethodGetGroup.Invoke(GameViewSizesInstance, new object[] { (int)type });
    }

    //[MenuItem("Test/LogCurrentGroupType")]
    //public static void LogCurrentGroupType()
    //{
    //    Debug.Log(GetCurrentGroupType());
    //}
    public static GameViewSizeGroupType GetCurrentGroupType()
    {
        var getCurrentGroupTypeProp = GameViewSizesInstance.GetType().GetProperty("currentGroupType");
        return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(GameViewSizesInstance, null);
    }

#endif

    public static void SwitchOrientation()
    {
#if UNITY_EDITOR
        int width = Screen.height;
        int height = Screen.width;
        int index = FindSize(GetCurrentGroupType(), width, height);
        if (index == -1)
        {
            AddCustomSize(GameViewSizeType.FixedResolution, GetCurrentGroupType(), width, height, "");
            index = FindSize(GetCurrentGroupType(), width, height);
        }
        if (index != -1)
        {
            SetSize(index);
        }
        else
        {
            Debug.LogError("switchOrientation failed, can not find or add resoulution for " + width.ToString() + "*" + height.ToString());
        }
#endif
    }

    public static void Set1080Portrait()
    {
#if UNITY_EDITOR
        int width = Math.Min(Screen.width, Screen.height);
        int height = Math.Max(Screen.width, Screen.height);
        int index = FindSize(GetCurrentGroupType(), width, height);
        if (index == -1)
        {
            AddCustomSize(GameViewSizeType.FixedResolution, GetCurrentGroupType(), width, height, "");
            index = FindSize(GetCurrentGroupType(), width, height);
        }
        if (index != -1)
        {
            SetSize(index);
        }
        else
        {
            Debug.LogError("switchOrientation failed, can not find or add resoulution for " + width.ToString() + "*" + height.ToString());
        }
#endif
    }

    public static void Set1080Landscape()
    {
#if UNITY_EDITOR
        int width = Math.Max(Screen.width, Screen.height);
        int height = Math.Min(Screen.width, Screen.height);
        int index = FindSize(GetCurrentGroupType(), width, height);
        if (index == -1)
        {
            AddCustomSize(GameViewSizeType.FixedResolution, GetCurrentGroupType(), width, height, "");
            index = FindSize(GetCurrentGroupType(), width, height);
        }
        if (index != -1)
        {
            SetSize(index);
        }
        else
        {
            Debug.LogError("switchOrientation failed, can not find or add resoulution for " + width.ToString() + "*" + height.ToString());
        }
#endif
    }
}

#endif