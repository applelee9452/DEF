#if DEF_CLIENT

#if DEF_LOCALIZATION
using I2.Loc;
#endif
using System.Threading.Tasks;
using UnityEngine;
using YooAsset;

public class UpdateState5UpdateData : SimpleState2
{
    enum UpdateState
    {
        None = 0,
        WaitingConfirm,
        Updating,
        Done
    }

    Updater Updater { get; set; }
    UiLaunch UiLaunch { get; set; }
    UpdateState UpdateState1 { get; set; } = UpdateState.None;
    string RemoteDataVersion { get; set; }

    public UpdateState5UpdateData(Updater updater)
    {
        Updater = updater;
        UiLaunch = Updater.UiLaunch;
    }

    public override string GetName()
    {
        return "UpdateData";
    }

    public override async void Enter()
    {
        Debug.Log("UpdateState5UpdateData.Enter()");

        UpdateState1 = UpdateState.None;

        RemoteDataVersion = GetRemoteDataVersion();

        Updater.TryGetCfgCenter("DataUpdateUrl", out string data_update_url);

        Debug.Log($"UpdaterMode={Client.ClientCfg4Runtime.UpdaterMode}");
        Debug.Log($"DataUpdateUrl={data_update_url}");
        Debug.Log($"RemoteDataVersion={RemoteDataVersion}");

        // 初始化YooAsset
        await YooAssetWrapper.Init(Client.ClientCfg4Runtime, RemoteDataVersion, data_update_url);

        Debug.Log($"YooAsset.Init() Done!");

#if DEF_LOCALIZATION
        string tips = LocalizationManager.GetTranslation("Launch.ReadyUpdateData");
        UiLaunch?.UpdateDesc(tips);
#else
        //UiLaunch?.UpdateDesc($"更新资源，{RemoteDataVersion}，{data_update_url}");
        UiLaunch?.UpdateDesc($"更新资源");
#endif
        UiLaunch?.UpdateLoadingProgress(0, 100);
        UiLaunch?.RefreshVersionInfo();

        Debug.Log($"当前平台：{Application.platform}");

        await DownloadRemoteData();

        Debug.Log($"UpdateState5UpdateData.Enter() Done!");
    }

    public override void Exit()
    {
        Debug.Log("UpdateState5UpdateData.Exit()");
    }

    public override string Update(float tm)
    {
        if (UpdateState1 == UpdateState.Done)
        {
            return "UpdateCfg";
        }
        //else if (UpdateState1 == UpdateState.None)
        //{
        //    UpdateState1 = UpdateState.WaitingConfirm;

        //    DownloadRemoteData();
        //}

        return string.Empty;
    }

    public override string OnEvent(string ev_name, string ev_param)
    {
        if (ev_name == "EvReTry")
        {
        }

        return string.Empty;
    }

    public override string OnEvent(string ev_name, object ev_param)
    {
        return string.Empty;
    }

    public async Task DownloadRemoteData()
    {
        var op_updatepackage_manifest = YooAssetWrapper.AssetsPackage.UpdatePackageManifestAsync(RemoteDataVersion, 30);
        await op_updatepackage_manifest.Task;

        if (op_updatepackage_manifest.Status != EOperationStatus.Succeed)
        {
            Debug.LogError($"YooAsset.UpdatePackageManifestAsync Error: {op_updatepackage_manifest.Error}");
        }

        var downloader = YooAssetWrapper.AssetsPackage.CreateResourceDownloader(3, 3, 120);
//#if WEIXINMINIGAME && !UNITY_EDITOR
//            Debug.Log("-------Wechat nonot need downnload all resource-----");
//            UpdateState1 = UpdateState.Done;
//            return;
//#endif
        // 没有需要下载的资源
        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("没有需要下载的资源");
            UpdateState1 = UpdateState.Done;
            return;
        }

        // 需要下载的文件总数和总大小
        int total_download_count = downloader.TotalDownloadCount;
        long total_download_bytes = downloader.TotalDownloadBytes;

        // 注册回调方法
        downloader.DownloadErrorCallback = OnDownloadError;
        downloader.DownloadUpdateCallback = DownloadUpdate;
        downloader.DownloadFinishCallback = OnDownloadFinish;
        downloader.DownloadFileBeginCallback = OnDownloadFileBegin;

        // 弹框让玩家选择，更新Data
        var msg_info = $"当前版本：{YooAssetWrapper.AssetsPackage.GetPackageVersion()}\n待更新版本：{RemoteDataVersion}\n总文件下载数量：{total_download_count}\n待下载文件大小：{Updater.DownloadSizeToStr(total_download_bytes)}";
        var view_msgbox = UiMgr.GetOrCreateUi<UiMsgBox>();

        view_msgbox.Show(msg_info,
            async () =>
            {
                UiMgr.DestroyUi<UiMsgBox>();

                UpdateState1 = UpdateState.Updating;

                // 开启下载
                UiLaunch.SetProgressVisable(true);
                downloader.BeginDownload();
                await downloader.Task;

                // 检测下载结果
                if (downloader.Status == EOperationStatus.Succeed)
                {
                    // 下载成功
                    //IsDone = true;
                }
                else
                {
                    // 下载失败
                }
            },
            () =>
            {
                UiMgr.DestroyUi<UiMsgBox>();
                Application.Quit();
            });
    }

    public string GetRemoteDataVersion()
    {
        string new_data_version1 = string.Empty;
        string new_data_version2 = string.Empty;

        if (Updater.TryGetCfgCenter($"UpdateData_anybundle", out var v1))
        {
            new_data_version1 = ParseDataVersion(v1);
        }

        if (Updater.TryGetCfgCenter($"UpdateData_{Application.version}", out var v2))
        {
            new_data_version2 = ParseDataVersion(v2);
        }

        if (!string.IsNullOrEmpty(new_data_version1) && !string.IsNullOrEmpty(new_data_version2))
        {
            return new_data_version2;
        }
        else if (!string.IsNullOrEmpty(new_data_version1))
        {
            return new_data_version1;
        }
        else if (!string.IsNullOrEmpty(new_data_version2))
        {
            return new_data_version2;
        }

        return string.Empty;
    }

    string ParseDataVersion(string v)
    {
        if (!string.IsNullOrEmpty(v))
        {
            var arr = v.Split('_');
            if (arr != null && arr.Length >= 2)
            {
                var action = arr[1];
                if (action == "keep")
                {
                    return string.Empty;
                }
                else if (action == "update")
                {
                    string new_data_version = arr[2];
                    return new_data_version;
                }
            }
        }

        return string.Empty;
    }

    void OnDownloadFileBegin(DownloadFileData data)
    {
        const int maxFilenameLength = 40;
        if (data.FileName.Length > maxFilenameLength)
        {
            data.FileName = data.FileName.Substring(0, maxFilenameLength) + "...";
        }

#if DEF_LOCALIZATION
#else
        UiLaunch?.UpdateDesc("下载资源:" + data.FileName);
#endif
    }

    void OnDownloadFinish(DownloaderFinishData data)
    {
        UpdateState1 = UpdateState.Done;

#if DEF_LOCALIZATION
#else
        UiLaunch?.UpdateDesc("下载资源完成");
#endif
        Debug.Log("下载资源完成！");
    }

    void DownloadUpdate(DownloadUpdateData data)
    {
        UiLaunch.UpdateLoadingProgress(data.CurrentDownloadCount, data.TotalDownloadCount);
    }

    void OnDownloadError(DownloadErrorData data)
    {
        Debug.LogError($"下载资源失败！FileName={data.FileName} Error={data.ErrorInfo}");
    }
}

#endif