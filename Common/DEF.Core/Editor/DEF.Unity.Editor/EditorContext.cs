using System;
using System.IO;
using UnityEngine;

public class EditorContext
{
    public static EditorContext Instance { get; internal set; }
    public string PathResources { get; private set; }// Resources目录
    public string PathSettings { get; private set; }// Settings目录
    public string PathSettingsUser { get; private set; }// SettingsUser目录
    public string PathPersistent { get; private set; }// Persistent根目录
    public EditorCfg EditorCfg { get; private set; }

    public EditorContext()
    {
        Instance = this;

        // Resources目录
        {
            string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Resources/");
            var di = new DirectoryInfo(p);
            PathResources = di.FullName;
        }

        // PathSettingsProj
        {
            string p = Path.Combine(Environment.CurrentDirectory, "./Assets/Settings/");
            var di = new DirectoryInfo(p);
            PathSettings = di.FullName;
        }

        // PathSettingsUser
        {
            string p = Path.Combine(Environment.CurrentDirectory, "./Assets/SettingsUser/");
            var di = new DirectoryInfo(p);
            PathSettingsUser = di.FullName;
        }

        // PersistentDataPath
        {
            PathPersistent =
#if UNITY_STANDALONE_WIN && UNITY_EDITOR
            Application.persistentDataPath +"/PC";
#elif UNITY_ANDROID && UNITY_EDITOR
            Application.persistentDataPath + "/ANDROID";
#elif UNITY_IPHONE && UNITY_EDITOR
            Application.persistentDataPath + "/IOS";
#elif UNITY_ANDROID
            Application.persistentDataPath;
#elif UNITY_IPHONE
            Application.persistentDataPath;
#else
            string.Empty;
#endif
        }

        EditorCfg = new EditorCfg();
    }
}