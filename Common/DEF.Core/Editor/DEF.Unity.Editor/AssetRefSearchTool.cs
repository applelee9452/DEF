using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public static class AssetRefSearchTool
{
    static string[] assetGUIDs;
    static string[] assetPaths;
    static string[] allAssetPaths;
    static Thread thread;

    [MenuItem("Assets/查找资源引用", false)]
    static void FindAssetRefMenu()
    {
        if (Selection.assetGUIDs.Length == 0)
        {
            Debug.Log("请先选择任意一个组件，再击此菜单");
            return;
        }

        assetGUIDs = Selection.assetGUIDs;

        assetPaths = new string[assetGUIDs.Length];

        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            assetPaths[i] = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
        }

        allAssetPaths = AssetDatabase.GetAllAssetPaths();

        thread = new Thread(new ThreadStart(FindAssetRef));
        thread.Start();
    }

    static void FindAssetRef()
    {
        Debug.Log(string.Format("开始查找引用{0}的资源。", string.Join(",", assetPaths)));
        List<string> logInfo = new List<string>();
        string path;
        string log;
        for (int i = 0; i < allAssetPaths.Length; i++)
        {
            path = allAssetPaths[i];
            if (path.EndsWith(".prefab") || path.EndsWith(".unity"))
            {
                string content = File.ReadAllText(path);
                if (content == null)
                {
                    continue;
                }

                for (int j = 0; j < assetGUIDs.Length; j++)
                {
                    if (content.IndexOf(assetGUIDs[j]) > 0)
                    {
                        log = string.Format("{0} 引用了 {1}", path, assetPaths[j]);
                        logInfo.Add(log);
                    }
                }
            }
        }

        for (int i = 0; i < logInfo.Count; i++)
        {
            Debug.Log(logInfo[i]);
        }

        Debug.Log("选择对象引用数量：" + logInfo.Count);

        Debug.Log("查找完成");
    }
}