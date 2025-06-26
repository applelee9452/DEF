#if DEF_CLIENT

using System;
using System.Collections.Generic;

[Serializable]
public enum UpdaterMode
{
    None = 0,
    EditorPlayMode,
    OfflinePlayMode,
    HostPlayMode,
}

[Serializable]
public struct GatewayUriInfo
{
    public string Name;
    public string Value;
    public bool UseSsl;
}

public class ClientCfg4Runtime
{
    public List<GatewayUriInfo> ListGatewayUriInfo { get; set; }
    public List<string> ListChannel { get; set; }
    public string Channel { get; set; }
    public string CurrentEnv { get; set; }
    public string CCenterUrl { get; set; }
    public string EntryClassName { get; set; }
    public UpdaterMode UpdaterMode { get; set; }
    public RuntimeMode RuntimeMode { get; set; }
    public string DouyinBundleVersion { get; set; }
    public bool YooAssetCopyBuildinFileOption { get; set; }
    public bool ReGenEncrypt { get; set; }
    public bool IsEncrypt { get; set; }
    public string EncryptKey { get; set; }
    public string EncryptNonce { get; set; }

    public void Init()
    {
        ListGatewayUriInfo = new()
        {
            new GatewayUriInfo() { Name = "DevLocal", Value = "127.0.0.1" },
            new GatewayUriInfo() { Name = "Dev", Value = "127.0.0.1" },
            new GatewayUriInfo() { Name = "Test", Value = "127.0.0.1" },
            new GatewayUriInfo() { Name = "Staging", Value = "127.0.0.1" },
            new GatewayUriInfo() { Name = "Pro", Value = "127.0.0.1" },
            new GatewayUriInfo() { Name = "Oss", Value = "" }
        };
        ListChannel = new()
        {
            "Default",
            "Wechat",
        };
        Channel = "Default";// Default，Taptap ...   
        CurrentEnv = "Oss";// 环境 Dev Test Staging Pro。Oss表示从Oss中获取
        CCenterUrl = "http://{0}:{1}/api/ccenter/getcfg?name_space=Client|Client.{2}";
        EntryClassName = string.Empty;
        UpdaterMode = UpdaterMode.HostPlayMode;
        RuntimeMode = RuntimeMode.HybridCLR;

        YooAssetCopyBuildinFileOption = false;
        ReGenEncrypt = false;
        IsEncrypt = false;
        EncryptKey = string.Empty;
        EncryptNonce = string.Empty;
    }

    public List<string> GetGatewayUriKeyList()
    {
        List<string> list = new();

        if (ListGatewayUriInfo != null)
        {
            foreach (var i in ListGatewayUriInfo)
            {
                list.Add(i.Name);
            }
        }

        return list;
    }

    public string GetGatewayUriValue(string key)
    {
        if (key == "Oss")
        {
            return string.Empty;
        }

        string v = string.Empty;

        if (ListGatewayUriInfo != null)
        {
            foreach (var i in ListGatewayUriInfo)
            {
                if (i.Name == key)
                {
                    v = i.Value;
                    break;
                }
            }
        }

        return v;
    }

    public string GetGatewayUri(string key)
    {
        return GetGatewayUriValue(key);
    }

    public string GetOssUri()
    {
        string v = string.Empty;

        if (ListGatewayUriInfo != null)
        {
            foreach (var i in ListGatewayUriInfo)
            {
                if (i.Name == "Oss")
                {
                    v = i.Value;
                    break;
                }
            }
        }

        return v;
    }

    public bool GetUseSsl(string env)
    {
        bool v = false;

        if (ListGatewayUriInfo != null)
        {
            foreach (var i in ListGatewayUriInfo)
            {
                if (i.Name == env)
                {
                    v = i.UseSsl;
                    break;
                }
            }
        }

        return v;
    }
}

#endif