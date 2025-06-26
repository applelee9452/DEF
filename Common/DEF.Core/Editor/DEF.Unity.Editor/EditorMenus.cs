#if UNITY_EDITOR 

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using YooAsset;

public class EditorMenu
{
    //[MenuItem("打包/ChannelJson", false, 1)]
    //private static void BuildChannelJson()
    //{
    //    string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Plugins/Android/gradleTemplate.properties");
    //    string full_filename = Path.GetFullPath(p);
    //    Debug.Log(full_filename);

    //    if (File.Exists(full_filename))
    //    {
    //        string s = string.Empty;

    //        {
    //            using StreamReader sr = File.OpenText(full_filename);
    //            s = sr.ReadToEnd();
    //            Debug.Log(s);

    //            bool container_key = false;
    //            var arr_s = s.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
    //            for (int i = 0; i < arr_s.Length; i++)
    //            {
    //                string line = arr_s[i].Trim();

    //                if (!line.Contains('=')) continue;
    //                Debug.Log(line);
    //                var arr_s2 = line.Split('=');

    //                if (arr_s2[0] == "channel")
    //                {
    //                    arr_s[i] = "channel=json";
    //                    container_key = true;
    //                }
    //            }

    //            List<string> list = new(arr_s);
    //            if (!container_key)
    //            {
    //                list.Add("channel=json");
    //            }

    //            StringBuilder sb = new(512);
    //            foreach (string line in list)
    //            {
    //                sb.Append(line);
    //                sb.AppendLine();
    //            }
    //            s = sb.ToString();
    //        }

    //        if (!string.IsNullOrEmpty(s))
    //        {
    //            using StreamWriter sw = File.CreateText(full_filename);
    //            sw.Write(s);
    //        }
    //    }
    //}

    [MenuItem("DEF/客户端配置（用于编辑器，不同步Git）", false, 1)]
    private static void ClientCfg4User()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        var dlg = EditorWindow.GetWindow<DlgClientCfg4User>("客户端配置（不同步Git）");
        dlg.Show();
    }

    [MenuItem("DEF/客户端配置（用于真机）", false, 2)]
    private static void ClientCfg4Runtime()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        var dlg = EditorWindow.GetWindow<DlgClientCfg4Runtime>("客户端配置（真机）");
        dlg.Show();
    }

    [MenuItem("DEF/客户端配置（项目）", false, 3)]
    private static void ClientCfg4Project()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        var dlg = EditorWindow.GetWindow<DlgClientCfg4Project>("客户端配置（项目）");
        dlg.Show();
    }

    [MenuItem("DEF/Ability编辑器", false, 5)]
    private static void AbilityEditor()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        Debug.Log("Ability编辑器");
    }

    [MenuItem("DEF/Tilemap3D编辑器", false, 6)]
    private static void Tilemap3DEditor()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        Debug.Log("Tilemap3D编辑器");
    }

    [MenuItem("DEF/NodeGraph编辑器", false, 7)]
    private static void NodeGraphEditor()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        Debug.Log("NodeGraph编辑器");
    }

    [MenuItem("DEF/Timeline编辑器", false, 8)]
    private static void TimelineEditor()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        Debug.Log("Timeline编辑器");
    }

    [MenuItem("DEF/发布UWP配置", false, 9)]
    private static void ConfigForUWP()
    {
        var proj_dir = Path.GetDirectoryName(Application.dataPath);
        var dir = $"{proj_dir}/Assets/HybridCLRData";
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }

        dir = $"{proj_dir}/Library/PackageCache/com.code-philosophy.hybridclr@7d58454fdd";
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }

        dir = $"{proj_dir}/HybridCLRData";
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, true);
        }
    }

    [MenuItem("DEF/删除所有PlayerPrefs", false, 10)]
    public static void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log($"删除所有PlayerPrefs完成");
    }

    [MenuItem("DEF/CopyAotDll", false, 11)]
    private static void CopyAotDll()
    {
        var proj_dir = Path.GetDirectoryName(Application.dataPath);
        var dir_source = $"{proj_dir}/HybridCLRData/AssembliesPostIl2CppStrip/{EditorUserBuildSettings.activeBuildTarget}";

        var list_file = Directory.GetFiles(dir_source, "*.dll");
        var list_dllname = new List<string>(list_file.Length);
        var dir_aot = $"{proj_dir}/Assets/Arts/Aot/";
        if (!Directory.Exists(dir_aot))
        {
            Directory.CreateDirectory(dir_aot);
        }
        foreach (var file_name in list_file)
        {
            string dll_name = Path.GetFileName(file_name);
            list_dllname.Add(dll_name);

            if (File.Exists(file_name))
            {
                File.Copy($"{file_name}", $"Assets/Arts/Aot/{dll_name}.bytes", true);
            }
            else
            {
                Debug.LogError($"Not found aot dll {file_name}");
            }
        }

        string file_name_aotlist = $"{proj_dir}/Assets/Arts/Aot/AotDllList.txt";
        var str = Newtonsoft.Json.JsonConvert.SerializeObject(list_dllname, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(file_name_aotlist, str);

        AssetDatabase.Refresh();

        Debug.Log($"CopyAotDlls完成，共{list_file.Length}个");
    }

#if DEF_TILEMAP3D
    [MenuItem("DEF/导出Tilemaps", false, 12)]
    private static void ExportTilemaps()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        var dlg = EditorWindow.GetWindow<DEF.DlgExportTilemaps>("ExportTilemaps");
        dlg.Show();
    }
#endif

#if DEF_SCENE
    [MenuItem("DEF/导出Scene", false, 13)]
    private static void ExportScene()
    {
        if (EditorContext.Instance == null)
        {
            new EditorContext();
        }

        var dlg = EditorWindow.GetWindow<DEF.DlgExportScene>("ExportScene");
        dlg.Show();
    }
#endif

    [MenuItem("DEF/导出NavMesh", false, 14)]
    private static void ExportNavMesh()
    {
        EditorExportNavMesh.Export();
    }

    [MenuItem("DEF/构建YooAsset模拟包 _F4")]
    private static void YooAssetSimulateBuild()
    {
        var simulate_build_result = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");

        Debug.Log("YooAsset模拟包构建完成");
        Debug.Log(simulate_build_result.ToString());
    }

    [MenuItem("DEF/编译脚本(Debug) _F5")]
    private static void BuildCodeDebug()
    {
        EditorBuildCodes.BuildCodeDebug();
    }
}

#endif