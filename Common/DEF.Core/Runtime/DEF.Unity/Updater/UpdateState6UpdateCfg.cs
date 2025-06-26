#if DEF_CLIENT

#if DEF_LOCALIZATION
using I2.Loc;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.SharpZipLib.Zip;
using UnityEngine;

// 目前是每次都下载，没有比较版本号
public class UpdateState6UpdateCfg : SimpleState2
{
    Updater Updater { get; set; }
    UiLaunch UiLaunch { get; set; }
    bool IsDone { get; set; } = false;

    public UpdateState6UpdateCfg(Updater updater)
    {
        Updater = updater;
        UiLaunch = Updater.UiLaunch;
    }

    public override string GetName()
    {
        return "UpdateCfg";
    }

    public override async void Enter()
    {
        Debug.Log("UpdateState6UpdateCfg.Enter()");

        IsDone = false;

#if DEF_LOCALIZATION
        string tips = LocalizationManager.GetTranslation("Launch.ReadyUpdateData");
        UiLaunch?.UpdateDesc(tips);
#else
        UiLaunch?.UpdateDesc("更新配置");
#endif
        UiLaunch?.RefreshVersionInfo();

        await DownloadRemoteCfg();
    }

    public override void Exit()
    {
        Debug.Log("UpdateState6UpdateCfg.Exit()");
    }

    public override string Update(float tm)
    {
        if (IsDone)
        {
            return "LoadDll";
        }

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

    string GetRemoteCfgVersion()
    {
        // 渠道通用的最新版本
        Updater.TryGetCfgCenter($"{Client.GetPlatformAndChannel()}_CfgVersion", out string channel_cfg_version);
        if (!string.IsNullOrEmpty(channel_cfg_version))
        {
            return channel_cfg_version;
        }

        // 通用的最新版本
        Updater.TryGetCfgCenter($"CfgVersion", out string cfg_version);
        if (!string.IsNullOrEmpty(cfg_version))
        {
            return cfg_version;
        }

        return string.Empty;
    }

    async Task DownloadRemoteCfg()
    {
        // 比对版本号
        string remove_cfg_version = GetRemoteCfgVersion();
        if (string.IsNullOrEmpty(remove_cfg_version))
        {
            IsDone = true;
            return;
        }

        string local_cfg_version = PlayerPrefs.GetString("CfgVersion");

        //if (Updater.UpdaterMode == UpdaterMode.HostPlayMode && local_cfg_version != remove_cfg_version)
        //{
        //    // 非真机模式强制更新，真机模式版本号不同则更新
        //    IsDone = true;
        //    return;
        //}

        // 拼接Url
        Updater.TryGetCfgCenter("CfgUpdateUrl", out string update_cfg_url);
        string url = string.Format(update_cfg_url, remove_cfg_version);

        // 下载Zip文件
        //url = "https://guaji-shanghai.oss-cn-shanghai.aliyuncs.com/Guaji/Cfg/2024070501/Cfg.zip";
        var data = await HttpClient2.GetRawData(url);

        // 写文件到Persistent目录
        //{
        //    var dir = Client.GetPersistentPath();
        //    if (!Directory.Exists(dir))
        //    {
        //        Directory.CreateDirectory(dir);
        //    }
        //    var p1 = Path.Combine(dir, "Cfg.zip");
        //    Debug.Log(p1);
        //    File.WriteAllBytes(p1, data);
        //}

        // 从内存压缩数据中解析配置
        {
            System.Diagnostics.Stopwatch sw = new();
            sw.Start();
            Client.MapCfg.Clear();
            UnZip(data, Client.MapCfg);
            sw.Stop();
            UnityEngine.Debug.Log($"UnZip Time={sw.ElapsedMilliseconds}");
        }

        IsDone = true;
    }

    bool UnZip(byte[] data2unzip, Dictionary<string, byte[]> map_cfg)
    {
        bool result = true;
        ZipEntry zip_entry = null;

        try
        {
            using var ms = new MemoryStream(data2unzip);
            using var zip_stream = new ZipInputStream(ms);

            while ((zip_entry = zip_stream.GetNextEntry()) != null)
            {
                if (string.IsNullOrEmpty(zip_entry.Name)) continue;

                if (zip_entry.Name.EndsWith("\\") || zip_entry.Size == 0) continue;

                string file_name = Path.GetFileNameWithoutExtension(zip_entry.Name);

                byte[] data = new byte[zip_entry.Size];
                zip_stream.Read(data, 0, data.Length);

                //using var fs = File.Create(file_name);

                //int size = 2048;
                //byte[] data = new byte[size];
                //while (true)
                //{

                //    size = zip_stream.Read(data, 0, data.Length);
                //    if (size > 0)
                //    {
                //        fs.Write(data, 0, size);
                //    }
                //    else break;
                //}

                map_cfg[file_name] = data;

                //Debug.Log($"Unzip FileName={file_name} Size={zip_entry.Size}");
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogException(ex);

            result = false;
        }
        finally
        {
            if (zip_entry != null)
            {
                zip_entry = null;
            }
        }

        return result;
    }
}

#endif