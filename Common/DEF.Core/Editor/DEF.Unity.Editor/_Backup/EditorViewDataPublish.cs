//using System.Collections.Generic;
//using System.IO;
//using UnityEditor;
//using UnityEngine;

//public class EditorViewDataPublish : UnityEditor.EditorWindow
//{
//    void OnGUI()
//    {
//        if (EditorContext.Instance == null)
//        {
//            new EditorContext();
//        }

//        GUILayout.Space(10);
//        GUILayout.Label("------------------------------------------------------");
//        EditorGUILayout.LabelField("BundleVersion:", Application.version);
//        EditorGUILayout.BeginHorizontal();
//        if (GUILayout.Button("清除所有VersionPersistent", GUILayout.Width(200)))
//        {
//            string app_channel_name = PlayerPrefs.GetString(StringDef.AppChannelNameMeta);
//            string app_channel_child_name = PlayerPrefs.GetString(StringDef.AppChannelChildNameMeta);
//            string country_code = PlayerPrefs.GetString(StringDef.CountryCodeName);
//            string default_language = PlayerPrefs.GetString(StringDef.DefaultLanguageNameMeta);

//            PlayerPrefs.DeleteAll();

//            PlayerPrefs.SetString(StringDef.AppChannelNameMeta, app_channel_name);
//            PlayerPrefs.SetString(StringDef.AppChannelChildNameMeta, app_channel_child_name);
//            PlayerPrefs.SetString(StringDef.CountryCodeName, country_code);
//            PlayerPrefs.SetString(StringDef.DefaultLanguageNameMeta, default_language);

//            AssetDatabase.Refresh();

//            ShowNotification(new GUIContent("删除所有VersionPersistent成功!"));
//        }

//        EditorGUILayout.EndHorizontal();
//        EditorGUILayout.BeginHorizontal();
//        if (GUILayout.Button("清空Persistent目录", GUILayout.Width(200)))
//        {
//            string dir_launch = Application.persistentDataPath + "/Launch";
//            string dir_cs = Application.persistentDataPath + "/Cs";
//            string dir_res = Application.persistentDataPath + "/Res";
//            if (Directory.Exists(dir_launch))
//            {
//                Directory.Delete(dir_launch, true);
//            }
//            if (Directory.Exists(dir_cs))
//            {
//                Directory.Delete(dir_cs, true);
//            }
//            if (Directory.Exists(dir_res))
//            {
//                Directory.Delete(dir_res, true);
//            }
//            ShowNotification(new GUIContent("清空Persistent目录成功!"));
//        }
//        EditorGUILayout.EndHorizontal();

//        GUILayout.Space(50);
//        EditorGUILayout.BeginHorizontal();
//        if (GUILayout.Button("关闭", GUILayout.Width(200), GUILayout.Height(50)))
//        {
//            Close();
//        }
//        EditorGUILayout.EndHorizontal();
//    }
//}

//// 一键生成sqlit
////void _genSqlite(string app_channel_name)
////{
////    // 01.生成目标路径
////    string path_dst = string.Format("{0}Cs\\{1}\\", EditorContext.Instance.PathDataOss, app_channel_name);
////    path_dst = path_dst.Replace(@"\", "/");

////    // 02.读取路径
////    string read_path = string.Empty;
////    string p = Path.Combine(Environment.CurrentDirectory, string.Format("./../Data/TbData{0}/", app_channel_name));
////    var di = new DirectoryInfo(p);
////    read_path = di.FullName;

////    // 03.sqlite 运行路径
////    string sqlite_process_exe = string.Empty;
////    string tool = Path.Combine(Environment.CurrentDirectory, "./Tools/Csv2Sqlite.exe");
////    var di_tool = new DirectoryInfo(tool);
////    sqlite_process_exe = di_tool.FullName;

////    StartProcess(sqlite_process_exe, new string[] { path_dst, read_path });
////}

////public bool StartProcess(string filname, string[] args)
////{
////    try
////    {
////        string param = string.Empty;
////        if (args != null)
////        {
////            for (int i = 0; i < args.Length; i++)
////            {
////                param += args[i];
////                param += " ";
////            }
////        }

////        System.Diagnostics.Process myprocess = new System.Diagnostics.Process();
////        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo(filname, param);
////        myprocess.StartInfo = startInfo;
////        myprocess.StartInfo.UseShellExecute = false;
////        myprocess.Start();
////        return true;
////    }
////    catch (Exception ex)
////    {
////        Debug.Log("出错原因：" + ex.Message);
////    }
////    return false;
////}

////private void SwitchScriptDefine()
////{
////    bool dirty = false;
////    string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);

////    if (CfgUserSettings.GameModeIndex == 0)// 提审版本
////    {
////        if (CfgUserSettings.GoldOptionIndex == 1 || CfgUserSettings.GoldOptionIndex == 2)
////        {
////            dirty = true;
////            if (!defines.Contains("APP_ENJOY"))
////            {
////                if (defines.EndsWith(";", System.StringComparison.Ordinal)) defines += "APP_ENJOY;";
////                else defines += ";" + "APP_ENJOY;";
////            }
////        }
////    }

////    if (dirty)
////    {
////        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
////    }
////}
