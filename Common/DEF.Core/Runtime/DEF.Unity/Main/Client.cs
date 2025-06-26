#if DEF_CLIENT

#if DEF_HYBRIDCLR
using HybridCLR;
#endif
#if DEF_LOCALIZATION
using I2.Loc;
#endif
using DEF.LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using YooAsset;
using DG.Tweening;

public class AAA
{
}

public class Client : IStartable, ITickable, IDisposable
{
    public static Client Instance { get; private set; }
    public static string GatewayUrl { get; set; }
    public static int GatewayPortHttp { get; set; }
    public static int GatewayPortHttps { get; set; }
    public static int GatewayPortTcp { get; set; }
    public static IObjectResolver Container { get; private set; }
    public static Dictionary<string, byte[]> MapCfg { get; private set; } = new();
    public static ClientConfig Config { get; private set; }
    public static ClientCfg4Runtime ClientCfg4Runtime { get; private set; }
    public static ClientCfg4User ClientCfg4User { get; private set; }
    public static AssemblesWrapper AssemblesWrapper { get; private set; }
    public static Updater Updater { get; private set; }
    static List<ITickable> ListTickable2Remove { get; set; } = new();
    static List<ITickable> ListTickable { get; set; } = new();

    [Inject]
    TimerShaft TimerShaft { get; set; }
    [Inject]
    UiMgr UiMgr { get; set; }
    ILTypeKeeper ILTypeKeeper { get; set; } = new();
    DEF.ChaCha20 ChaCha20 { get; set; }

    public Client(IObjectResolver container)
    {
        Container = container;
        Instance = this;
        Config = GameObject.FindObjectOfType<ClientConfig>();

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Debug.LogError(e);
        };

        //TaskScheduler.UnobservedTaskException += (sender, e) =>
        //{
        //    Debug.LogError(e.Exception);
        //};
    }

    void IStartable.Start()
    {
        // 是否显示FPS
        //if (Config.CfgSettings.ClientShowFPS)
        //{
        //    MbShowFPS.enabled = true;
        //}
        //else
        //{
        //    MbShowFPS.enabled = false;
        //}

        //string device_id = SystemInfo.deviceUniqueIdentifier;
        //Debug.Log("设备唯一Id=" + device_id);

        // 防裁剪Dll
        AAA a = new();
        Newtonsoft.Json.JsonConvert.SerializeObject(a);
        typeof(DOTween).ToString();
        typeof(Sequence).ToString();
        typeof(Tweener).ToString();

        // 加载ClientCfg4Runtime配置
        var ta = Resources.Load<TextAsset>(StringDef.FileClientCfg4Runtime);
        if (ta == null || string.IsNullOrEmpty(ta.text))
        {
            Debug.LogError("ClientCfg4Runtime加载失败 !!!");
            return;
        }

        ClientCfg4Runtime = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCfg4Runtime>(ta.text);

        // 加载ClientCfg4User配置
#if UNITY_EDITOR
        string p = Path.Combine(Environment.CurrentDirectory, "./Assets/SettingsUser/");
        var di = new DirectoryInfo(p);
        string path_settingsuser = di.FullName;

        string full_filename = Path.Combine(path_settingsuser, StringDef.FileClientCfg4User);
        if (File.Exists(full_filename))
        {
            using StreamReader sr = File.OpenText(full_filename);
            string s = sr.ReadToEnd();
            ClientCfg4User = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientCfg4User>(s);
            ClientCfg4Runtime.Channel = ClientCfg4User.CurrentChannel;
        }
#endif

#if DEF_LOCALIZATION
        // 初始化多语言信息
        LanguageSourceAsset source_asset = ResourceManager.pInstance.GetAsset<LanguageSourceAsset>("I2LanguagesUnity");
        source_asset.mSource.owner = source_asset;
        source_asset.mSource.Awake();

        string lan_key = "Language";
        string language_code;

        if (PlayerPrefs.HasKey(lan_key))
        {
            language_code = PlayerPrefs.GetString(lan_key);
        }
        else
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    language_code = "zh-CN";
                    break;
                default:
                    language_code = "en-US";
                    break;
            }

            PlayerPrefs.SetString(lan_key, language_code);
        }

        LocalizationManager.CurrentLanguageCode = language_code;
#endif

        // 创建更新器
        Updater = Container.Resolve<Updater>();
        Updater.Fire();
        ListTickable.Add(Updater);
    }

    void ITickable.Tick()
    {
        if (ListTickable2Remove.Count > 0)
        {
            foreach (var i in ListTickable2Remove)
            {
                ListTickable.Remove(i);

                ((IDisposable)i).Dispose();
            }
        }

        if (ListTickable.Count > 0)
        {
            foreach (var i in ListTickable)
            {
                i.Tick();
            }
        }

        AssemblesWrapper?.OnUpdate(Time.deltaTime);

        TimerShaft?.ProcessTimer((ulong)(Time.unscaledTime * 1000));
    }

    async void IDisposable.Dispose()
    {
        Updater = null;

        await UnloadDll();

        Debug.Log("Client.Dispose()");
    }

    public static async void Restart()
    {
        await UnloadDll();

        ((IStartable)Client.Instance).Start();
    }

    public static string GetPlatformAndChannel()
    {
        string str =
#if UNITY_STANDALONE_WIN || UNITY_WSA// && UNITY_EDITOR
            $"Win{ClientCfg4Runtime.Channel}";
#elif UNITY_ANDROID// && UNITY_EDITOR
            $"Android{ClientCfg4Runtime.Channel}";
#elif UNITY_IPHONE// && UNITY_EDITOR
            $"iOS{ClientCfg4Runtime.Channel}";
#elif UNITY_WEBGL
            $"WebGL{ClientCfg4Runtime.Channel}";
#else
            string.Empty;
#endif

        return str;
    }

    public static string GetPersistentPath()
    {
        string dir =
#if UNITY_STANDALONE_WIN || UNITY_WSA// && UNITY_EDITOR
            Application.persistentDataPath + "/PC";
#elif UNITY_ANDROID// && UNITY_EDITOR
            Application.persistentDataPath + "/ANDROID";
#elif UNITY_IPHONE// && UNITY_EDITOR
            Application.persistentDataPath + "/IOS";
#elif UNITY_WEBGL
            Application.persistentDataPath + "/WEBGL";
#else
            string.Empty;
#endif

        return dir;
    }

    public static void OnApplicationPause(bool pause)
    {
        if (AssemblesWrapper != null)
        {
            AssemblesWrapper.OnApplicationPause(pause);
        }
    }

    public static void OnApplicationFocus(bool focus_status)
    {
        if (AssemblesWrapper != null)
        {
            AssemblesWrapper.OnApplicationFocus(focus_status);
        }
    }

    public static void ILRTest()
    {
        if (AssemblesWrapper != null)
        {
            AssemblesWrapper.OnTest();
        }
    }

    public static async void ILRDelayCreate()
    {
        if (AssemblesWrapper != null)
        {
            await AssemblesWrapper.OnDelayCreate();
        }
    }

    public static void RemoveTickableAndDispose(object obj)
    {
        ListTickable2Remove.Add((ITickable)obj);
    }

    public static async Task LoadDll()
    {
        switch (ClientCfg4Runtime.RuntimeMode)
        {
            case RuntimeMode.HybridCLR:
                {
                    Debug.Log($"------------------加载DLL: {Updater.UpdaterMode}");
                    Debug.Log($"------------------加载DLL: Assets/Arts/S/Script.dll.byte");

                    var handle_dll = YooAssets.LoadAssetAsync<TextAsset>("Assets/Arts/S/Script.dll.bytes");
                    var handle_pdb = YooAssets.LoadAssetAsync<TextAsset>("Assets/Arts/S/Script.pdb.bytes");
                    await handle_dll.Task;
                    await handle_pdb.Task;

                    Debug.Log($"------------------加载DLL: Done!");

                    var ta_dll = (TextAsset)handle_dll.AssetObject;
                    var ta_pdb = (TextAsset)handle_pdb.AssetObject;

                    Assembly ass_entry;

                    if (ClientCfg4Runtime.IsEncrypt
                        && !string.IsNullOrEmpty(ClientCfg4Runtime.EncryptKey)
                        && !string.IsNullOrEmpty(ClientCfg4Runtime.EncryptNonce))
                    {
                        byte[] bytes_dll = new byte[ta_dll.bytes.Length];
                        byte[] bytes_pdb = new byte[ta_pdb.bytes.Length];

                        byte[] key = Convert.FromBase64String(ClientCfg4Runtime.EncryptKey);
                        byte[] nonce = Convert.FromBase64String(ClientCfg4Runtime.EncryptNonce);

                        DEF.ChaCha20 a = new(key, nonce, 1);
                        a.DecryptBytes(bytes_dll, ta_dll.bytes, ta_dll.bytes.Length);
                        a.DecryptBytes(bytes_pdb, ta_pdb.bytes, ta_pdb.bytes.Length);
                        a.Dispose();

                        ass_entry = Assembly.Load(bytes_dll, bytes_pdb);
                    }
                    else
                    {
                        ass_entry = Assembly.Load(ta_dll.bytes, ta_pdb.bytes);
                    }

                    if (ass_entry == null)
                    {
                        Debug.LogError($"Assembly==null");
                    }

                    await LoadAot();

                    if (ass_entry != null)
                    {
                        await LoadAssembles(ass_entry);
                    }
                }
                break;
            case RuntimeMode.None:
                {
                    Assembly ass_entry = null;
                    var domain = AppDomain.CurrentDomain;
                    Assembly[] arr_assembly = domain.GetAssemblies();
                    foreach (var ass in arr_assembly)
                    {
                        string s = ass.GetName().Name;
                        if (s == "Script")
                        {
                            ass_entry = ass;
                            break;
                        }
                    }
                    Debug.Log($"ass_entry={ass_entry}");
                    await LoadAssembles(ass_entry);
                }
                break;
        }
    }

    public static async Task UnloadDll()
    {
        ListTickable.Clear();
        ListTickable2Remove.Clear();

        if (AssemblesWrapper != null)
        {
            await AssemblesWrapper.Destroy();
            AssemblesWrapper = null;
        }

        YooAssetWrapper.Destroy();
    }

    static Task LoadAssembles(Assembly ass_entry)
    {
        // platform
        string platform = "Android";
#if UNITY_STANDALONE_WIN || UNITY_WEBGL
        platform = "PC";
#elif UNITY_ANDROID && UNITY_EDITOR
        platform = "Android";
#elif UNITY_ANDROID
        platform = "Android";
#elif UNITY_IPHONE
        platform = "iOS";
#endif

        // is_editor
        bool is_editor = false;
#if UNITY_EDITOR
        is_editor = true;
#else
        is_editor = false;
#endif

        // test_mode
        TestMode test_mode = TestMode.None;
        string testmode1_params = string.Empty;
        string testmode2_params = string.Empty;
        if (ClientCfg4User != null)
        {
            test_mode = ClientCfg4User.TestMode;
            testmode1_params = ClientCfg4User.TestMode1Params;
            testmode2_params = ClientCfg4User.TestMode2Params;
        }

        // use_ssl
        string current_env;
        if (ClientCfg4User != null)
        {
            current_env = ClientCfg4User.CurrentEnv;
        }
        else
        {
            current_env = ClientCfg4Runtime.CurrentEnv;
        }
        bool use_ssl = ClientCfg4Runtime.GetUseSsl(current_env);

        var param_list = new Dictionary<string, string>
        {
            { "platform", platform.ToString() },
            { "is_editor", is_editor.ToString() },
            { "gatewayuri", GatewayUrl },
            { "test_mode", test_mode.ToString() },
            { "testmode1_params", testmode1_params },
            { "testmode2_params", testmode2_params },
            { "use_ssl", use_ssl.ToString() }
        };

        AssemblesWrapper = new();
        return AssemblesWrapper.Create(false, ass_entry, ClientCfg4Runtime.EntryClassName, param_list);
    }

    public static async Task LoadAot()
    {
#if DEF_HYBRIDCLR
        var handle_aot_list = YooAssets.LoadAssetAsync<TextAsset>("Assets/Arts/Aot/AotDllList.txt");
        await handle_aot_list.Task;
        var aot_list = (TextAsset)handle_aot_list.AssetObject;

        string[] dll_name_list = JsonMapper.ToObject<string[]>(aot_list.text);

        List<Task> list_task = new(dll_name_list.Length);
        List<AssetHandle> list_ah = new(dll_name_list.Length);

        // 不限补充元数据dll文件的读取方式，可以从Ab、StreamingAssets、或者裸文件下载等办法获得
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var dll_name in dll_name_list)
        {
            var handler = YooAssets.LoadAssetAsync<TextAsset>($"Assets/Arts/Aot/{dll_name}.bytes");
            var t = handler.Task;
            list_task.Add(t);
            list_ah.Add(handler);
        }

        await Task.WhenAll(list_task);

        foreach (var handler in list_ah)
        {
            byte[] dll_bytes = handler.GetAssetObject<TextAsset>().bytes;

            // 获得某个aot dll文件所有字节
            // 加载assembly对应的dll，会自动为它hook。 一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dll_bytes, mode);

            //Debug.Log($"LoadMetadataForAOTAssembly:{dll_name}. Mode:{mode} Result:{err}");
        }
#endif
    }

    public static string GetBundleVersion()
    {
        if (ClientCfg4Runtime.Channel == "Douyin")
        {
            return ClientCfg4Runtime.DouyinBundleVersion;
        }

        return Application.version;
    }
}

#endif