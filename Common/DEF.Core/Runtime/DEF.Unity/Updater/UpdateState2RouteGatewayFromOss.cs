#if DEF_CLIENT

#if DEF_LOCALIZATION
using I2.Loc;
#endif
using DEF.Unity.Updater;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public enum GetVersionInfoState
{
    Todo = 0,
    Doing,
    Done,// 已完成，继续向后推进
    Waitting4RetryOrExit,// 等待玩家选择重试或者退出，不在继续向后推进
}

public class UpdateState2RouteGatewayFromOss : SimpleState2
{
    Updater Updater { get; set; }
    UiLaunch UiLaunch { get; set; }
    GetVersionInfoState State { get; set; } = GetVersionInfoState.Todo;

    public UpdateState2RouteGatewayFromOss(Updater updater)
    {
        Updater = updater;
        UiLaunch = Updater.UiLaunch;
    }

    public override string GetName()
    {
        return "RouteGatewayFromOss";
    }

    public override void Enter()
    {
        Debug.Log("UpdateState2RouteGatewayFromOss.Enter()");

        State = GetVersionInfoState.Done;

#if UNITY_EDITOR
        if (Client.ClientCfg4User.CurrentEnv == "Oss") State = GetVersionInfoState.Todo;
#else
        if (Client.ClientCfg4Runtime.CurrentEnv == "Oss") State = GetVersionInfoState.Todo;
#endif

        if (State == GetVersionInfoState.Done)
        {
            Debug.Log("Editor中手动指定GatewayUri");

            string gatewayuri = string.Empty;
#if UNITY_EDITOR
            gatewayuri = Client.ClientCfg4Runtime.GetGatewayUri(Client.ClientCfg4User.CurrentEnv);
#else
            gatewayuri = Client.ClientCfg4Runtime.GetGatewayUri(Client.ClientCfg4Runtime.CurrentEnv);
#endif
            Client.GatewayUrl = gatewayuri;
            Client.GatewayPortHttp = 5000;
            Client.GatewayPortHttps = 5001;
            Client.GatewayPortTcp = 5002;

            Debug.Log($"GatewayUrlFromEditor={Client.GatewayUrl}");
        }
        else
        {
            Debug.Log("准备从Oss中获取GatewayUri");

#if DEF_LOCALIZATION
            string tips = LocalizationManager.GetTranslation("Launch.InitRes");
            UiLaunch?.UpdateDesc(tips);
#else
            UiLaunch?.UpdateDesc("获取网关入口");
#endif
            UiLaunch?.RefreshVersionInfo();
        }
    }

    public override void Exit()
    {
        Debug.Log("UpdateState2RouteGatewayFromOss.Exit()");
    }

    public override string Update(float tm)
    {
        if (State == GetVersionInfoState.Done)
        {
            if (Updater.UpdaterMode == UpdaterMode.None)
            {
                return "LoadDll";
            }
            else
            {
                return "GetCfgFromCCenter";
            }
        }
        else if (State == GetVersionInfoState.Todo)
        {
#pragma warning disable CS4014
            GetOssInfo();
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

    async Task GetOssInfo()
    {
        try
        {
            State = GetVersionInfoState.Doing;

            var oss_url1 = Client.ClientCfg4Runtime.GetOssUri();

            string oss_url = $"https://{oss_url1}/{Client.GetPlatformAndChannel()}_{Client.GetBundleVersion()}.txt";

            var data = await UnityWebRequestAwaiter.RequestGetRaw(oss_url);

            var data_s = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log(data_s);
            Dictionary<string, string> m = ParseMap(data_s);

            foreach (var i in m)
            {
                Debug.Log($"Oss，Key={i.Key} Value={i.Value}");
            }

            // 网关Url
            if (m.TryGetValue("GatewayUrl", out var gateway_url))
            {
                Client.GatewayUrl = gateway_url;
            }

            // 网关端口，Http
            Client.GatewayPortHttp = 5000;
            if (m.TryGetValue("GatewayPortHttp", out var gateway_port_http_s))
            {
                if (int.TryParse(gateway_port_http_s, out var port))
                {
                    Client.GatewayPortHttp = port;
                }
            }

            // 网关端口，Https
            Client.GatewayPortHttps = 5001;
            if (m.TryGetValue("GatewayPortHttps", out var gateway_port_https_s))
            {
                if (int.TryParse(gateway_port_https_s, out var port))
                {
                    Client.GatewayPortHttps = port;
                }
            }

            // 网关端口，Tcp
            Client.GatewayPortTcp = 5002;
            if (m.TryGetValue("GatewayPortTcp", out var gateway_port_tcp_s))
            {
                if (int.TryParse(gateway_port_tcp_s, out var port))
                {
                    Client.GatewayPortTcp = port;
                }
            }

            // 服务器正在维护中

            m.TryGetValue("MaintenanceInfo", out var maintenance_info);

            if (m.TryGetValue("MaintenanceEnable", out var maintenance_enable))
            {
                int.TryParse(maintenance_enable, out var maintenance_enable_i);
                if (maintenance_enable_i == 1)
                {
                    State = GetVersionInfoState.Waitting4RetryOrExit;

                    if (!string.IsNullOrEmpty(maintenance_info))
                    {
                        ShowUiMsgBox(maintenance_info);
                    }
                    else
                    {
                        ShowUiMsgBox("游戏正在维护中，请稍后再试！");
                    }

                    return;
                }
            }

            // 是否开启盾

            if (m.TryGetValue("ShieldEnable", out var shield_enable))
            {
                int.TryParse(shield_enable, out var shield_enable_i);
                if (shield_enable_i == 1)
                {
                    var c = GameObject.FindFirstObjectByType<ClientBehaviour>();
                    if (c != null)
                    {
                        Debug.Log("广播GameObject消息，ShieldEnable");

                        // 获取当前场景所有GameObject
                        var arr_go = c.gameObject.scene.GetRootGameObjects();
                        if (arr_go != null && arr_go.Length > 0)
                        {
                            foreach (var go in arr_go)
                            {
                                go.BroadcastMessage("ShieldEnable", null, SendMessageOptions.DontRequireReceiver);
                            }
                        }
                    }
                }
            }

            State = GetVersionInfoState.Done;
        }
        catch (Exception ex)
        {
            ShowUiMsgBox(ex.ToString());
        }
    }

    Dictionary<string, string> ParseMap(string data)
    {
        Dictionary<string, string> m = new();

        if (string.IsNullOrEmpty(data))
        {
            return m;
        }

        string[] p = new[] { "\n", "\r\n" };
        var arr = data.Split(p, StringSplitOptions.RemoveEmptyEntries);
        if (arr == null || arr.Length == 0)
        {
            return m;
        }

        foreach (var item in arr)
        {
            if (string.IsNullOrEmpty(item)) continue;
            var arr2 = item.Split('|');
            if (arr2 == null || arr2.Length != 2) continue;
            m[arr2[0]] = arr2[1];
        }

        return m;
    }

    void ShowUiMsgBox(string content)
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