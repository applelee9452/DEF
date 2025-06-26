#if DEF_CLIENT

#if DEF_LOCALIZATION
using I2.Loc;
#endif
using UnityEngine;

public class UpdateState4UpdateBundle : SimpleState2
{
    Updater Updater { get; set; }
    UiLaunch UiLaunch { get; set; }
    bool IsDone { get; set; }
    string RemoteBundleVersion { get; set; }
    string AppVersion { get; set; }

    public UpdateState4UpdateBundle(Updater updater)
    {
        Updater = updater;
        UiLaunch = Updater.UiLaunch;
    }

    public override string GetName()
    {
        return "UpdateBundle";
    }

    public override void Enter()
    {
        Debug.Log("UpdateState4UpdateBundle.Enter()");

        IsDone = false;

        // 检测是否需要更新Bundle
        RemoteBundleVersion = GetRemoteBundleVersion(out var is_disable);

        if (is_disable)
        {
            var msg_info1 = string.Format("该版本已停用，请下载最新版本");
            var view_msgbox1 = UiMgr.GetOrCreateUi<UiMsgBox>();
            view_msgbox1.Show(msg_info1,
            () =>
            {
                UiMgr.DestroyUi<UiMsgBox>();
                Application.Quit();
            },
            () =>
            {
                UiMgr.DestroyUi<UiMsgBox>();
                Application.Quit();
            });
        }

        // 无需更新
        if (string.IsNullOrEmpty(RemoteBundleVersion) || AppVersion == RemoteBundleVersion)
        {
            IsDone = true;
            return;
        }

        // 更新Bundle
#if DEF_LOCALIZATION
        string tips = LocalizationManager.GetTranslation("Launch.ReadyUpdateBundle");
        UiLaunch?.UpdateDesc(tips);
#else
        UiLaunch?.UpdateDesc("更新Bundle");
#endif
        UiLaunch?.UpdateLoadingProgress(0, 100);
        UiLaunch?.RefreshVersionInfo();

        // 弹框让玩家选择，更新Bundle Apk
        var msg_info = string.Format("安装包更新\n当前版本：{0}\n待更新版本：{1}", AppVersion, RemoteBundleVersion);
        var view_msgbox = UiMgr.GetOrCreateUi<UiMsgBox>();

        // 如果是编辑器，并不真正更新
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.LinuxEditor:
            case RuntimePlatform.OSXEditor:
                UpdateBundleEditorSimulator(view_msgbox, msg_info);
                break;
            case RuntimePlatform.Android:
                UpdateBundleAndroid(view_msgbox, msg_info);
                break;
            case RuntimePlatform.IPhonePlayer:
                UpdateBundleiOS(view_msgbox, msg_info);
                break;
        }
    }

    public override void Exit()
    {
        Debug.Log("UpdateState4UpdateBundle.Exit()");
    }

    public override string Update(float tm)
    {
        if (IsDone)
        {
            PlayerPrefs.SetString("VersionBundlePersistent", AppVersion);
            return "UpdateData";
        }

        return string.Empty;
    }

    public override string OnEvent(string ev_name, string ev_param)
    {
        return string.Empty;
    }

    public override string OnEvent(string ev_name, object ev_param)
    {
        return string.Empty;
    }

    public void UpdateBundleEditorSimulator(UiMsgBox view_msgbox, string msg_info)
    {
        view_msgbox.Show(msg_info,
            () =>
            {
                UiLaunch.UpdateDesc("正在更新安装包..."); // 正在更新安装包...
                UiLaunch.UpdateLoadingProgress(0, 100);
                UiMgr.DestroyUi<UiMsgBox>();
                IsDone = true;
            },
            () =>
            {
                UiMgr.DestroyUi<UiMsgBox>();
                IsDone = true;
            });
    }

    public void UpdateBundleAndroid(UiMsgBox view_msgbox, string msg_info)
    {
        view_msgbox.Show(msg_info,
            () =>
            {
                UiMgr.DestroyUi<UiMsgBox>();
                Updater.TryGetCfgCenter($"{Client.GetPlatformAndChannel()}_BundleUrl",
                    out string remote_bundle_url);
                if (remote_bundle_url.Contains("{0}"))
                {
                    remote_bundle_url = string.Format(remote_bundle_url, RemoteBundleVersion);
                }

                // 用浏览器打开Android下载链接
                Application.OpenURL(remote_bundle_url);
                Application.Quit();
            },
            () =>
            {
                UiMgr.DestroyUi<UiMsgBox>();
                Application.Quit();
            });

        // Android平台，如果是商店，则跳转链接；如果是自建Web，则直接下载安装包，安装后重启
        // view_msgbox.Show(msg_info,
        //     () =>
        //     {
        //         string http_url = Config.GetBundleUpdateUrlAndroid(VersionMgr.BundleVersionRemote);
        //         //Debug.Log(http_url);
        //         using (var http_request = new BestHTTP.HTTPRequest(new Uri(http_url), (request, response) =>
        //                {
        //                    switch (request.State)
        //                    {
        //                        case BestHTTP.HTTPRequestStates.Finished:
        //                            if (response.IsSuccess)
        //                            {
        //                                //string apk_path = PathMgr.DirUserRoot + "KingTexas.apk";
        //                                //File.WriteAllBytes(apk_path, response.Data);
        //
        //                                //NativeMgr.RequestInstallAPK(apk_path);
        //
        //                                Application.Quit(); // todo，更优的是重启
        //                            }
        //                            else
        //                            {
        //                                string update_bundle_fail =
        //                                    ComLaunch.GetLanguageString(
        //                                        "UpdateBundleFail"); // 更新安装包错误：StatusCode={0} Message={1}
        //                                UiLaunch.UpdateDesc(string.Format(update_bundle_fail, response.StatusCode,
        //                                    response.Message)); // 更新安装包错误：StatusCode={0} Message={1}
        //                            }
        //
        //                            IsDone = true;
        //                            break;
        //                        case BestHTTP.HTTPRequestStates.Error:
        //                            string update_bundle_error =
        //                                ComLaunch.GetLanguageString("UpdateBundleError"); // 更新安装包错误：
        //                            UiLaunch.UpdateDesc(update_bundle_error + request.Exception != null
        //                                ? request.Exception.Message
        //                                : "No Exception");
        //                            break;
        //                        case BestHTTP.HTTPRequestStates.Aborted:
        //                            UiLaunch.UpdateDesc(
        //                                ComLaunch.GetLanguageString("UpdateBundleCancel")); // 更新安装包被取消！ 
        //                            break;
        //                        case BestHTTP.HTTPRequestStates.ConnectionTimedOut:
        //                            UiLaunch.UpdateDesc(
        //                                ComLaunch.GetLanguageString("UpdateBundleConnectionTimeOut")); // 更新安装包连接超时！
        //                            break;
        //                        case BestHTTP.HTTPRequestStates.TimedOut:
        //                            UiLaunch.UpdateDesc(ComLaunch.GetLanguageString("UpdateBundleTimeOut"));
        //                            break;
        //                    }
        //                }))
        //         {
        //             http_request.Timeout = TimeSpan.FromMinutes(60);
        //             http_request.EnableTimoutForStreaming = false;
        //             //http_request.UseStreaming = false;
        //             //http_request.EnableTimoutForStreaming = false;
        //             //http_request.StreamFragmentSize = 1024 * 1024 * 1;
        //             http_request.DisableCache = true;
        //             http_request.OnDownloadProgress += ((BestHTTP.HTTPRequest originalRequest, long downloaded,
        //                 long downloadLength) =>
        //             {
        //                 if (downloadLength == 0) UiLaunch.UpdateLoadingProgress(0, 100);
        //                 else UiLaunch.UpdateLoadingProgress((int)downloaded, (int)downloadLength);
        //             });
        //             http_request.Send();
        //         }
        //
        //         UiMgr.DestroyUi<UiMsgBox>();
        //     },
        //     () =>
        //     {
        //         UiMgr.DestroyUi<UiMsgBox>();
        //         Application.Quit();
        //     });
    }

    public void UpdateBundleiOS(UiMsgBox view_msgbox, string msg_info)
    {
        //iOS平台，如果是商店，则跳转链接；如果是自建Web，则直接下载安装包，安装后重启
        // view_msgbox.Show(msg_info,
        //     () =>
        //     {
        //         string str_desc = UiLaunch.GetLanguageString("UpdatingBundle");
        //         UiLaunch.UpdateDesc(str_desc); // 正在更新安装包...
        //         UiLaunch.UpdateLoadingProgress(0, 100);
        //
        //         UiMgr.DestroyUi<UiMsgBox>();
        //
        //         string iosBundleUrl = up;
        //         // 用浏览器打开iOS下载链接
        //         Application.OpenURL(Client.Config.GetBundleUpdateUrl());
        //         Application.Quit();
        //     },
        //     () =>
        //     {
        //         UiMgr.DestroyUi<UiMsgBox>();
        //         Application.Quit();
        //     });
    }

    public string GetRemoteBundleVersion(out bool is_disable)
    {
        string new_bundle_version1 = string.Empty;
        string new_bundle_version2 = string.Empty;

        is_disable = false;
        bool is_disable1 = false;
        bool is_disable2 = false;

        if (Updater.TryGetCfgCenter($"UpdateBundle_anybundle", out var v1))
        {
            new_bundle_version1 = ParseBundleVersion(v1, out is_disable1);
        }

        if (Updater.TryGetCfgCenter($"UpdateBundle_{Application.version}", out var v2))
        {
            new_bundle_version2 = ParseBundleVersion(v2, out is_disable2);
        }

        if (!string.IsNullOrEmpty(new_bundle_version1) && !string.IsNullOrEmpty(new_bundle_version2))
        {
            is_disable = is_disable2;
            return new_bundle_version2;
        }
        else if (!string.IsNullOrEmpty(new_bundle_version1))
        {
            is_disable = is_disable1;
            return new_bundle_version1;
        }
        else if (!string.IsNullOrEmpty(new_bundle_version2))
        {
            is_disable = is_disable2;
            return new_bundle_version2;
        }

        return string.Empty;
    }

    string ParseBundleVersion(string v, out bool is_disable)
    {
        is_disable = false;

        if (!string.IsNullOrEmpty(v))
        {
            var arr = v.Split('_');
            if (arr != null && arr.Length >= 2)
            {
                var action = arr[1];
                if (action == "disable")
                {
                    is_disable = true;
                    return string.Empty;
                }
                else if (action == "keep")
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
}

#endif