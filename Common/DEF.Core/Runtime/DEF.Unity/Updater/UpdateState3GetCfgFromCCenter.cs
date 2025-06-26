#if DEF_CLIENT

#if DEF_LOCALIZATION
using I2.Loc;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpdateState3GetCfgFromCCenter : SimpleState2
{
    Updater Updater { get; set; }
    UiLaunch UiLaunch { get; set; }
    GetVersionInfoState State { get; set; } = GetVersionInfoState.Todo;

    public UpdateState3GetCfgFromCCenter(Updater updater)
    {
        Updater = updater;
        UiLaunch = Updater.UiLaunch;
    }

    public override string GetName()
    {
        return "GetCfgFromCCenter";
    }

    public override void Enter()
    {
        Debug.Log("UpdateState3GetCfgFromCCenter.Enter()");

        State = GetVersionInfoState.Todo;

#if DEF_LOCALIZATION
        string tips = LocalizationManager.GetTranslation("Launch.InitRes");
        UiLaunch?.UpdateDesc(tips);
#else
        UiLaunch?.UpdateDesc("获取配置");
#endif
        UiLaunch?.RefreshVersionInfo();
    }

    public override void Exit()
    {
        Debug.Log("UpdateState3GetCfgFromCCenter.Exit()");
    }

    public override string Update(float tm)
    {
        if (State == GetVersionInfoState.Done)
        {
            return "UpdateBundle";
        }
        else if (State == GetVersionInfoState.Todo)
        {
#pragma warning disable CS4014
            State = GetVersionInfoState.Doing;
            GetVersionInfo();
#pragma warning restore CS4014
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

    async Task GetVersionInfo()
    {
        try
        {
            // http://{0}:5000/api/ccenter/getcfg?name_space=Client|Client.{1}
            string ccenter_url = Client.ClientCfg4Runtime.CCenterUrl;

            string current_env;
            if (Client.ClientCfg4User != null)
            {
                current_env = Client.ClientCfg4User.CurrentEnv;
            }
            else
            {
                current_env = Client.ClientCfg4Runtime.CurrentEnv;
            }

            int port = Client.GatewayPortHttps;
            bool use_ssl = Client.ClientCfg4Runtime.GetUseSsl(current_env);
            if (use_ssl)
            {
                port = Client.GatewayPortHttps;
            }
            else
            {
                port = Client.GatewayPortHttp;
                ccenter_url = ccenter_url.Replace("https", "http");
            }

            string url = string.Format(ccenter_url,
                Client.GatewayUrl,
                port,
                Client.GetPlatformAndChannel());

            url += $"&bundle_version={Application.version}";

            Dictionary<string, string> map_cfg = await HttpClient2.Post<Dictionary<string, string>>(url);

            bool res = Updater.SetCfgCenter(map_cfg);

#if DEF_LOCALIZATION
            string tips = LocalizationManager.GetTranslation("Launch.InitRes");
            UiLaunch?.UpdateDesc(tips);
#else
            int cfg_count = map_cfg == null ? 0 : map_cfg.Count;
            if (cfg_count == 0)
            {
                UiLaunch?.UpdateDesc($"获取配置，{url}");
            }
            else
            {
                UiLaunch?.UpdateDesc($"获取配置，Count={cfg_count}");
            }
#endif

            // 是否启用调试控制台
            if (Updater.TryGetCfgCenter("DebugConsoleEnable", out var debug_console_enable_s))
            {
                if (bool.TryParse(debug_console_enable_s, out var debug_console_enable))
                {
                    if (debug_console_enable)
                    {
                        var scene = SceneManager.GetActiveScene();
                        var arr_go = scene.GetRootGameObjects();
                        foreach (var i in arr_go)
                        {
                            if (i.name == "IngameDebugConsole")
                            {
                                i.SetActive(true);
                                break;
                            }
                        }
                    }
                }
            }

            // 如果连接CCenter失败 就往Oss拿公告信息
            if (!res)
            {
                State = GetVersionInfoState.Waitting4RetryOrExit;

                ShowUiMsgBox("连接游戏服务器失败，可能是网络原因，是否重试？");

                return;
            }

            State = GetVersionInfoState.Done;
        }
        catch (Exception ex)
        {
            ShowUiMsgBox(ex.ToString());
        }
    }

    public void ShowUiMsgBox(string content)
    {
        var ui_msgbox = UiMgr.GetOrCreateUi<UiMsgBox>();
        ui_msgbox.Show(content,
            () =>
            {
                // Ok，重试，每次间隔3秒
                State = GetVersionInfoState.Todo;
                UiMgr.DestroyUi<UiMsgBox>();
            },
            () =>
            {
                // Cancel，退出游戏
                UiMgr.DestroyUi<UiMsgBox>();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                Application.Quit();
            });
    }
}

#endif