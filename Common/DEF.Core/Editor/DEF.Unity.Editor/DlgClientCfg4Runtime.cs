using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DlgClientCfg4Runtime : OdinEditorWindow
{
    [OnInspectorInit]
    void Init()
    {
        EditorContext.Instance.EditorCfg.LoadClientCfg4Runtime();

        GatewayUriList = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.ListGatewayUriInfo;
        CurrentEnvList = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.GetGatewayUriKeyList();
        CurrentEnv = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.CurrentEnv;
        UpdateMode = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.UpdaterMode;
        RuntimeMode = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.RuntimeMode;
        EntryClassName = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.EntryClassName;
        ChannelList = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.ListChannel;
        CurrentChannel = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.Channel;
        DouyinBundleVersion = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.DouyinBundleVersion;

        YooAssetCopyBuildinFileOption = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.YooAssetCopyBuildinFileOption;

        ReGenEncrypt = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.ReGenEncrypt;
        IsEncrypt = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.IsEncrypt;
        EncryptKey = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.EncryptKey;
        EncryptNonce = EditorContext.Instance.EditorCfg.ClientCfg4Runtime.EncryptNonce;
    }

    [OnInspectorDispose]
    void Destroy()
    {
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.ListGatewayUriInfo = GatewayUriList;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.EntryClassName = EntryClassName;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.UpdaterMode = UpdateMode;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.RuntimeMode = RuntimeMode;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.CurrentEnv = CurrentEnv;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.ListChannel = ChannelList;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.Channel = CurrentChannel;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.DouyinBundleVersion = DouyinBundleVersion;

        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.YooAssetCopyBuildinFileOption = YooAssetCopyBuildinFileOption;

        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.ReGenEncrypt = ReGenEncrypt;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.IsEncrypt = IsEncrypt;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.EncryptKey = EncryptKey;
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.EncryptNonce = EncryptNonce;

        EditorContext.Instance.EditorCfg.SaveClientCfg4Runtime();

        AssetDatabase.Refresh();
    }

    [EnumToggleButtons, OnValueChanged("OnUpdateMode")]
    public UpdaterMode UpdateMode;
    private void OnUpdateMode()
    {
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.UpdaterMode = UpdateMode;
        EditorContext.Instance.EditorCfg.SaveClientCfg4Runtime();
    }

    [EnumToggleButtons, OnValueChanged("OnRuntimeModeChanged")]
    public RuntimeMode RuntimeMode;
    private void OnRuntimeModeChanged()
    {
        Debug.Log($"RuntimeMode={RuntimeMode}");
        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.RuntimeMode = RuntimeMode;
        EditorContext.Instance.EditorCfg.SaveClientCfg4Runtime();

        switch (RuntimeMode)
        {
            case RuntimeMode.HybridCLR:
                {
                    string string_dir = "Assets/Scripts/S/";
                    string dll_path = Path.Combine(string_dir, "Script.dll");
                    string pdb_path = Path.Combine(string_dir, "Script.pdb");
                    if (File.Exists(dll_path)) File.Delete(dll_path);
                    if (File.Exists(pdb_path)) File.Delete(pdb_path);
                    if (Directory.Exists(string_dir)) Directory.Delete(string_dir, true);
                    if (File.Exists("Assets/Scripts/S.meta")) File.Delete("Assets/Scripts/S.meta");
                }
                break;
            case RuntimeMode.None:
                {
                }
                break;
        }

        AssetDatabase.Refresh();
    }

    [LabelText("CurrentEnv")]
    [ValueDropdown("CurrentEnvList"), OnValueChanged("OnCurrentEnvChanged")]
    public string CurrentEnv;
    private static List<string> CurrentEnvList;

    private void OnCurrentEnvChanged()
    {
        Debug.Log($"CurrentEnv={CurrentEnv}");

        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.CurrentEnv = CurrentEnv;

        EditorContext.Instance.EditorCfg.SaveClientCfg4Runtime();

        AssetDatabase.Refresh();
    }

    [LabelText("CurrentChannel")]
    [ValueDropdown("ChannelList"), OnValueChanged("OnCurrentChannelChanged")]
    public string CurrentChannel;

    private void OnCurrentChannelChanged()
    {
        Debug.Log($"CurrentChannel={CurrentChannel}");

        EditorContext.Instance.EditorCfg.ClientCfg4Runtime.Channel = CurrentChannel;

        EditorContext.Instance.EditorCfg.SaveClientCfg4Runtime();

        AssetDatabase.Refresh();
    }

    [LabelText("EntryClassName")]
    public string EntryClassName;

    [LabelText("DouyinBundleVersion")]
    public string DouyinBundleVersion;

    [LabelText("Channel列表")]
    public List<string> ChannelList;

    [LabelText("GatewayUri列表")]
    public List<GatewayUriInfo> GatewayUriList;

    [OnInspectorGUI]
    private void Space0() { GUILayout.Space(30); }

    [LabelText("YooAssetCopyBuildinFileOption")]
    public bool YooAssetCopyBuildinFileOption;

    [OnInspectorGUI]
    private void Space1() { GUILayout.Space(30); }

    [HorizontalGroup]
    [LabelText("IsEncrypt")]
    public bool IsEncrypt;

    [HorizontalGroup]
    [LabelText("ReGenEncrypt")]
    public bool ReGenEncrypt;

    [HorizontalGroup]
    [EnableIf("ReGenEncrypt")]
    [Button("生成热更dll加密key", ButtonSizes.Small)]
    public void ButtonGenEncrypt()
    {
        if (!IsEncrypt) return;

        System.Random rng = new();

        byte[] key = new byte[32];
        byte[] nonce = new byte[12];

        rng.NextBytes(key);
        rng.NextBytes(nonce);

        EncryptKey = System.Convert.ToBase64String(key);
        EncryptNonce = System.Convert.ToBase64String(nonce);
    }

    [LabelText("EncryptKey")]
    public string EncryptKey;

    [LabelText("EncryptNonce")]
    public string EncryptNonce;

    [OnInspectorGUI]
    private void Space3() { GUILayout.Space(30); }

    [Button("关闭", ButtonSizes.Gigantic), GUIColor(0, 1f, 0)]
    public void ButtonClose()
    {
        Close();
    }
}