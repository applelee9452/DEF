#if UNITY_EDITOR

using UnityEditor;

public class DEFMenus
{
    [MenuItem("DEF/DEF编辑器 _F1", false, 4)]
    private static void DEFEditor()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        var dlg = EditorWindow.GetWindow<DEFHierarchy>("DEF.Hierarchy");
        dlg.Show();
    }
}

#endif