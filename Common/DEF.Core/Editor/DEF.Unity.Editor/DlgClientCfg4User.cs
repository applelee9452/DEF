using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DlgClientCfg4User : OdinEditorWindow
{
    [OnInspectorInit]
    void Init()
    {
        CurrentEnvList = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.GetGatewayUriKeyList();
        CurrentEnv = EditorContext.Instance.EditorCfg.ClientCfg4User.CurrentEnv;
        CurrentChannelList = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.ListChannel;
        CurrentChannel = EditorContext.Instance.EditorCfg.ClientCfg4User.CurrentChannel;
        UpdateMode = EditorContext.Instance.EditorCfg.ClientCfg4User.UpdaterMode;
        TestMode = EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode;
        TestMode1Params = EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode1Params;
        TestMode2Params = EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode2Params;
    }

    [OnInspectorDispose]
    void Destroy()
    {
        EditorContext.Instance.EditorCfg.ClientCfg4User.CurrentEnv = CurrentEnv;
        EditorContext.Instance.EditorCfg.ClientCfg4User.CurrentChannel = CurrentChannel;
        EditorContext.Instance.EditorCfg.ClientCfg4User.UpdaterMode = UpdateMode;
        EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode = TestMode;
        EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode1Params = TestMode1Params;
        EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode2Params = TestMode2Params;

        EditorContext.Instance.EditorCfg.SaveClientCfg4User();

        AssetDatabase.Refresh();
    }

    [EnumToggleButtons, OnValueChanged("OnUpdateMode")]
    public UpdaterMode UpdateMode;
    private void OnUpdateMode()
    {
        EditorContext.Instance.EditorCfg.ClientCfg4User.UpdaterMode = UpdateMode;
        EditorContext.Instance.EditorCfg.SaveClientCfg4User();
    }

    [LabelText("CurrentEnv")]
    [ValueDropdown("CurrentEnvList"), OnValueChanged("OnCurrentEnvChanged")]
    public string CurrentEnv;
    private static List<string> CurrentEnvList;

    private void OnCurrentEnvChanged()
    {
        Debug.Log($"CurrentEnv={CurrentEnv}");

        EditorContext.Instance.EditorCfg.ClientCfg4User.CurrentEnv = CurrentEnv;
        EditorContext.Instance.EditorCfg.SaveClientCfg4User();
        AssetDatabase.Refresh();
    }

    [LabelText("CurrentChannel")]
    [ValueDropdown("CurrentChannelList"), OnValueChanged("OnCurrentChannelChanged")]
    public string CurrentChannel;
    private static List<string> CurrentChannelList;

    private void OnCurrentChannelChanged()
    {
        Debug.Log($"CurrentChannel={CurrentChannel}");

        EditorContext.Instance.EditorCfg.ClientCfg4User.CurrentChannel = CurrentChannel;
        EditorContext.Instance.EditorCfg.SaveClientCfg4User();
        AssetDatabase.Refresh();
    }

    [EnumToggleButtons, OnValueChanged("OnTestModeChanged")]
    public TestMode TestMode = EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode;
    private void OnTestModeChanged()
    {
        Debug.Log($"TestMode={TestMode}");

        EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode = TestMode;
        EditorContext.Instance.EditorCfg.SaveClientCfg4User();
        AssetDatabase.Refresh();
    }

    [LabelText("TestMode1Params"), OnValueChanged("OnTestMode1ParamsChanged")]
    public string TestMode1Params = EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode1Params;
    private void OnTestMode1ParamsChanged()
    {
        EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode1Params = TestMode1Params;
        EditorContext.Instance.EditorCfg.SaveClientCfg4User();
        AssetDatabase.Refresh();
    }

    [LabelText("TestMode2Params"), OnValueChanged("OnTestMode2ParamsChanged")]
    public string TestMode2Params = EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode2Params;
    private void OnTestMode2ParamsChanged()
    {
        EditorContext.Instance.EditorCfg.ClientCfg4User.TestMode2Params = TestMode2Params;
        EditorContext.Instance.EditorCfg.SaveClientCfg4User();
        AssetDatabase.Refresh();
    }

    [OnInspectorGUI]
    private void Space3() { GUILayout.Space(30); }

    [Button("关闭", ButtonSizes.Gigantic), GUIColor(0, 1f, 0)]
    public void ButtonClose()
    {
        Close();
    }
}