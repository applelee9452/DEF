using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DlgClientCfg4Project : OdinEditorWindow
{
    [OnInspectorInit]
    void Init()
    {
        EditorContext.Instance.EditorCfg.LoadClientCfg4Project();

        CodeDirList = EditorContext.Instance.EditorCfg.ClientCfg4Project.ListCodeDir;
        ListExcludeReferences = EditorContext.Instance.EditorCfg.ClientCfg4Project.ListExcludeReferences;
        GenViewCodeDir = EditorContext.Instance.EditorCfg.ClientCfg4Project.GenViewCodeDir;
        ClientNameSpace = EditorContext.Instance.EditorCfg.ClientCfg4Project.ClientNameSpace;
    }

    [OnInspectorDispose]
    void Destroy()
    {
        EditorContext.Instance.EditorCfg.ClientCfg4Project.ListCodeDir = CodeDirList;
        EditorContext.Instance.EditorCfg.ClientCfg4Project.ListExcludeReferences = ListExcludeReferences;
        EditorContext.Instance.EditorCfg.ClientCfg4Project.GenViewCodeDir = GenViewCodeDir;
        EditorContext.Instance.EditorCfg.ClientCfg4Project.ClientNameSpace = ClientNameSpace;

        EditorContext.Instance.EditorCfg.SaveClientCfg4Project();

        AssetDatabase.Refresh();
    }

    [LabelText("CodeDirList")]
    public List<string> CodeDirList;

    [LabelText("ExcludeReferencesList")]
    public List<string> ListExcludeReferences;

    [LabelText("GenViewCodeDir")]
    public string GenViewCodeDir;

    [LabelText("ClientNameSpace")]
    public string ClientNameSpace;

    [OnInspectorGUI]
    private void Space3() { GUILayout.Space(30); }

    [Button("关闭", ButtonSizes.Gigantic), GUIColor(0, 1f, 0)]
    public void ButtonClose()
    {
        Close();
    }
}