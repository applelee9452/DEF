using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DlgTimelineEditor : OdinEditorWindow
{
    [OnInspectorInit]
    void Init()
    {
        CodeDirList = EditorContext.Instance.EditorCfg.ClientCfg4Project.ListCodeDir;
    }

    [OnInspectorDispose]
    void Destroy()
    {
        EditorContext.Instance.EditorCfg.ClientCfg4Project.ListCodeDir = CodeDirList;

        EditorContext.Instance.EditorCfg.SaveClientCfg4Project();

        AssetDatabase.Refresh();
    }

    [LabelText("CodeDirList")]
    public List<string> CodeDirList;

    [OnInspectorGUI]
    private void Space3() { GUILayout.Space(30); }

    [Button("关闭", ButtonSizes.Gigantic), GUIColor(0, 1f, 0)]
    public void ButtonClose()
    {
        Close();
    }
}