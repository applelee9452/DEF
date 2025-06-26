#if DEF_CLIENT

using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;
#if WEIXINMINIGAME
using WeChatWASM;
#elif DOUYINMINIGAME
#else
#endif

public class RemoteServices : IRemoteServices
{
    string DefalutUrl { get; set; }
    string FallbackUrl { get; set; }

    public RemoteServices(string default_url, string fallback_url)
    {
        DefalutUrl = default_url;
        FallbackUrl = fallback_url;
    }

    public string GetRemoteMainURL(string fileName)
    {
        return $"{DefalutUrl}/{fileName}";
    }

    public string GetRemoteFallbackURL(string fileName)
    {
        return $"{FallbackUrl}/{fileName}";
    }
}

public class YooAssetWrapper
{
    public static ResourcePackage AssetsPackage;

    // 初始化YooAsset
    public static async Task Init(ClientCfg4Runtime cfg, string data_version = "", string data_update_url = "")
    {
        BetterStreamingAssets.Initialize();

        YooAssets.Initialize();

        // 初始化YooAsset
        //await YooAsset.Init(Client.ClientCfg4Runtime);

        AssetsPackage = YooAssets.TryGetPackage("DefaultPackage");
        if (AssetsPackage == null)
        {
            AssetsPackage = YooAssets.CreatePackage("DefaultPackage");
        }

        YooAssets.SetDefaultPackage(AssetsPackage);

        InitializeParameters init_parameters = null;

        UpdaterMode updater_mode = UpdaterMode.EditorPlayMode;
#if UNITY_EDITOR
        updater_mode = Client.ClientCfg4User.UpdaterMode;
#else
        updater_mode = Client.ClientCfg4Runtime.UpdaterMode;
#endif

        if (updater_mode == UpdaterMode.EditorPlayMode || updater_mode == UpdaterMode.None)
        {
#if UNITY_EDITOR
            string platform;
#if UNITY_STANDALONE_WIN
            platform = "StandaloneWindows64";
#elif UNITY_ANDROID && UNITY_EDITOR
            platform = "Android";
#elif UNITY_ANDROID
            platform = "Android";
#elif UNITY_IPHONE
            platform = "iOS";
#elif UNITY_MINIGAME
            platform = "MiniGame";
#elif UNITY_WEBGL
            platform = "WebGL";
#endif

            string dir = Path.GetDirectoryName(Application.dataPath);
            string manifest_root_dir = Path.Combine(dir, $"./Bundles/{platform}/DefaultPackage/Simulate");
            manifest_root_dir = Path.GetFullPath(manifest_root_dir);

            // C:/Work/Mao/UnityMao/Bundles/Android/DefaultPackage/Simulate

            //var simulate_build_param = new EditorSimulateBuildParam()
            //{
            //    PackageName = "DefaultPackage" // 指定包裹名称
            //};
            //var simulate_build_result = EditorSimulateModeHelper.SimulateBuild(simulate_build_param);

            //var simulate_build_result = new EditorSimulateBuildResult()
            //{
            //    PackageRootDirectory = manifest_root_dir,
            //};

            var editor_filesystem_params = FileSystemParameters.CreateDefaultEditorFileSystemParameters(manifest_root_dir);

            init_parameters = new EditorSimulateModeParameters
            {
                EditorFileSystemParameters = editor_filesystem_params
            };

#elif UNITY_WEBGL
            if (Client.ClientCfg4Runtime.Channel == "Wechat")
            {
                //YooAssets.SetCacheSystemDisableCacheOnWebGL();
            }

            string GetHostServerURL()
            {
                string package_name = "DefaultPackage";
                return $"{data_update_url}/{package_name}/{data_version}";
            }

            string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();

            IRemoteServices remote_services = new RemoteServices(defaultHostServer, fallbackHostServer);
            var web_server_filesystem_params = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
            var web_remote_filesystem_params = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remote_services);// 支持跨域下载
    
            init_parameters = new WebPlayModeParameters
            {
                WebServerFileSystemParameters = web_server_filesystem_params,
                WebRemoteFileSystemParameters = web_remote_filesystem_params
            };
#else
            init_parameters = new OfflinePlayModeParameters
            {
            };
#endif
        }
        else if (updater_mode == UpdaterMode.OfflinePlayMode)// || string.IsNullOrEmpty(data_version))
        {
            //if (string.IsNullOrEmpty(data_version))
            //{
            //    Debug.Log($"DataVersion为空，自动转为OfflinePlayModeParameters");
            //}

            // CCenter版本号填 "" 的时候进入OfflinePlayMode

            var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
            init_parameters = new OfflinePlayModeParameters
            {
                BuildinFileSystemParameters = buildinFileSystemParams
            };
        }
        else if (updater_mode == UpdaterMode.HostPlayMode)
        {
            string GetHostServerURL()
            {
                string package_name = "DefaultPackage";
                return $"{data_update_url}/{package_name}/{data_version}";
            }

            string defaultHostServer = GetHostServerURL();
            string fallbackHostServer = GetHostServerURL();

#if UNITY_WEBGL && WEIXINMINIGAME
            //if (Client.ClientCfg4Runtime.Channel == "Wechat")
            //{
            //    //YooAssets.SetCacheSystemDisableCacheOnWebGL();
            //}

            // 创建远程服务类
            var remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);

            // 小游戏缓存根目录
            // 注意：此处代码根据微信插件配置来填写！
            string packageRoot = $"{WeChatWASM.WX.env.USER_DATA_PATH}/__GAME_FILE_CACHE/StreamingAssets";// WebGLWechat";
            Debug.Log($"微信YooAsset资源下载路径:{packageRoot}");

            // 创建初始化参数
            var createParameters = new WebPlayModeParameters();
            createParameters.WebServerFileSystemParameters = WechatFileSystemCreater.CreateFileSystemParameters(packageRoot, remoteServices);
            init_parameters = createParameters;
#elif UNITY_WEBGL && DOUYINMINIGAME
#elif UNITY_WEBGL
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
            var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
            var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); // 支持跨域下载

            init_parameters = new WebPlayModeParameters
            {
                WebServerFileSystemParameters = webServerFileSystemParams,
                WebRemoteFileSystemParameters = webRemoteFileSystemParams
            };

            //if (Client.ClientCfg4Runtime.YooAssetCopyBuildinFileOption)
            //{
            //    var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

            //    init_parameters = new HostPlayModeParameters
            //    {
            //        BuildinFileSystemParameters = buildinFileSystemParams,
            //        CacheFileSystemParameters = cacheFileSystemParams,
            //    };
            //}
            //else
            //{
            //    init_parameters = new HostPlayModeParameters
            //    {
            //        BuildinFileSystemParameters = null,
            //        CacheFileSystemParameters = cacheFileSystemParams,
            //    };
            //}
#else
            IRemoteServices remoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
            var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);

            if (Client.ClientCfg4Runtime.YooAssetCopyBuildinFileOption)
            {
                var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

                init_parameters = new HostPlayModeParameters
                {
                    BuildinFileSystemParameters = buildinFileSystemParams,
                    CacheFileSystemParameters = cacheFileSystemParams,
                };
            }
            else
            {
                init_parameters = new HostPlayModeParameters
                {
                    BuildinFileSystemParameters = null,
                    CacheFileSystemParameters = cacheFileSystemParams,
                };
            }
#endif
        }


        Debug.Log($"YooAsset InitializeParameters Type={init_parameters.GetType().Name}");

        var op = AssetsPackage.InitializeAsync(init_parameters);
        await op.Task;

        if (op.Status == EOperationStatus.Succeed)
        {
            Debug.Log("YooAsset 资源包初始化成功！~~~~~");
        }
        else
        {
            Debug.LogError($"YooAsset 资源包初始化失败：{op.Error}");
        }
    }

    public static IEnumerator Destroy()
    {
        if (AssetsPackage != null)
        {
            DestroyOperation operation = AssetsPackage.DestroyAsync();
            yield return operation;

            YooAssets.RemovePackage("DefaultPackage");

            AssetsPackage = null;
        }
    }
}

#endif