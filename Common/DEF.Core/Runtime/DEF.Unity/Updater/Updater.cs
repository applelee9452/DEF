#if DEF_CLIENT

using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

public class Updater : ITickable, IDisposable
{
    public UpdaterMode UpdaterMode { get; private set; }
    public UiLaunch UiLaunch { get; private set; }
    Dictionary<string, string> MapCfgFromCCenter { get; set; }
    SimpleFsm2 Fsm { get; set; }

    const float CheckNetworkTimeInterval = 5; // 检查网络间隔时间
    float CheckNetworkTimeEs = 0; // 检查网络当前流逝时间

    void IDisposable.Dispose()
    {
        if (Fsm != null)
        {
            Fsm.Exit();
            Fsm = null;
        }

        //if (UiLaunch != null)
        //{
        //    Debug.Log("UiMgr.DestroyUi<UiLaunch>()");
        //    UiMgr.DestroyUi<UiLaunch>();
        //    UiLaunch = null;
        //}
    }

    public void Fire()
    {
        if (Fsm != null) return;

        // 创建UiLaunch
        UiLaunch = UiMgr.CreateUi<UiLaunch>();

        // 初始化状态机
        Fsm = new SimpleFsm2();

        UpdaterMode = UpdaterMode.EditorPlayMode;
#if UNITY_EDITOR
        UpdaterMode = Client.ClientCfg4User.UpdaterMode;
#else
        UpdaterMode = Client.ClientCfg4Runtime.UpdaterMode;
#endif

        Fsm.AddState(new UpdateState1Idle(this), true);
        Fsm.AddState(new UpdateState2RouteGatewayFromOss(this));
        Fsm.AddState(new UpdateState3GetCfgFromCCenter(this));
        Fsm.AddState(new UpdateState4UpdateBundle(this));
        Fsm.AddState(new UpdateState5UpdateData(this));
        Fsm.AddState(new UpdateState6UpdateCfg(this));
        Fsm.AddState(new UpdateState7LoadDll(this));

        Fsm.Enter();

        Fsm.OnEvent("EvFire", string.Empty);
    }

    //public string GetRemoteBundleVersion(string download_content, string current_version)
    //{
    //    // 渠道有对应自身版本号的回滚优先级最高
    //    MapCfgFromCCenter.TryGetValue(
    //        $"{Client.GetPlatformAndChannel()}_Rollback{download_content}_{current_version}",
    //        out string channelrollback_bundle);
    //    if (!string.IsNullOrEmpty(channelrollback_bundle))
    //    {
    //        return channelrollback_bundle;
    //    }

    //    // 对应自身版本号的回滚优先级最高
    //    string rollback_bundle_version_str = $"Rollback{download_content}_{current_version}";
    //    MapCfgFromCCenter.TryGetValue(rollback_bundle_version_str, out string rollback_bundle);
    //    if (!string.IsNullOrEmpty(rollback_bundle))
    //    {
    //        return rollback_bundle;
    //    }

    //    // 渠道通用的最新版本
    //    MapCfgFromCCenter.TryGetValue(
    //        $"{Client.GetPlatformAndChannel()}_{download_content}Version",
    //        out string channel_bundle_version);
    //    if (!string.IsNullOrEmpty(channel_bundle_version))
    //    {
    //        bool is_newer = CompareVersion(current_version, channel_bundle_version);
    //        if (is_newer)
    //        {
    //            return channel_bundle_version;
    //        }
    //    }

    //    // 通用的最新版本
    //    MapCfgFromCCenter.TryGetValue($"{download_content}Version", out string bundle_version);
    //    if (!string.IsNullOrEmpty(bundle_version))
    //    {
    //        bool is_newer = CompareVersion(current_version, bundle_version);
    //        if (is_newer)
    //        {
    //            return bundle_version;
    //        }
    //    }

    //    return string.Empty;
    //}

    //public bool CompareVersion(string a, string b)
    //{
    //    var (aYear, aMonth, aDay, aCode) = Version2Int(a);
    //    var (bYear, bMonth, bDay, bCode) = Version2Int(b);
    //    if (bYear > aYear) return true;
    //    if (bMonth > aMonth) return true;
    //    if (bDay > aDay) return true;
    //    if (bCode > aCode) return true;

    //    return false;
    //}

    //public (int, int, int, int) Version2Int(string version_tr)
    //{
    //    string[] arr = (version_tr.IndexOf('-') > 0) ? version_tr.Split('-') : version_tr.Split('.');

    //    int.TryParse((arr.Length > 0) ? arr[0] : "0", out int year);
    //    int.TryParse((arr.Length > 1) ? arr[1] : "0", out int month);
    //    int.TryParse((arr.Length > 2) ? arr[2] : "0", out int day);
    //    int.TryParse((arr.Length > 3) ? arr[3] : "0", out int code);

    //    return (year, month, day, code);
    //}

    public string DownloadSizeToStr(long size)
    {
        if (size < 1024)
        {
            return $"{size}B";
        }
        else if (size < 1024 * 1024)
        {
            float size2 = size / 1024f;
            string size3 = size2.ToString("0.0");
            return $"{size3}KB";
        }
        else
        {
            float size2 = size / (1024f * 1024f);
            string size3 = size2.ToString("0.0");
            return $"{size3}MB";
        }
    }

    void ITickable.Tick()
    {
        if (Fsm == null) return;

        if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            // WiFi or cable
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            // CarrierDataNetwork
        }
        else if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 没有网络，弹框提示

            CheckNetworkTimeEs += Time.deltaTime;
            if (CheckNetworkTimeEs > CheckNetworkTimeInterval)
            {
                CheckNetworkTimeEs = 0;

                //string content = GetLanguageString("NotReachableTip");
                //var msg_box = ViewMgr.GetOrCreateView<ViewMsgBox>();
                //msg_box.ShowTwoButton(string.Empty, content,
                //() =>
                //{
                //    Fsm.OnEvent("EvReTry", string.Empty);
                //    ViewMgr.DestroyView<ViewMsgBox>();
                //},
                //() =>
                //{
                //    ViewMgr.DestroyView<ViewMsgBox>();
                //    Application.Quit();
                //});
            }
        }

        // 更新状态机
        Fsm.Update(Time.deltaTime);
    }

    public bool SetCfgCenter(Dictionary<string, string> cfg)
    {
        MapCfgFromCCenter = cfg;

        return MapCfgFromCCenter != null && MapCfgFromCCenter.Count > 0;
    }

    public bool TryGetCfgCenter(string key, out string value)
    {
        if (MapCfgFromCCenter == null)
        {
            value = string.Empty;
            return false;
        }

        string k_version = $"{key}_{Client.GetBundleVersion()}";
        if (MapCfgFromCCenter.TryGetValue(k_version, out value))
        {
            return true;
        }

        return MapCfgFromCCenter.TryGetValue(key, out value);
    }
}

#endif