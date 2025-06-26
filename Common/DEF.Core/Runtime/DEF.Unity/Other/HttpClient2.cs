#if DEF_CLIENT

using DEF.Unity.Updater;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class HttpClient2
{
    public static async Task<TResult> Get<TResult>(string url)
    {
        Debug.Log(url);

        using var req = UnityWebRequest.Get(url);

        await req.SendWebRequest();

        var result_s = req.downloadHandler.text;
        if (string.IsNullOrEmpty(result_s))
        {
            Debug.Log($"Get<TResult> Error={req.error}");

            return default;
        }
        else
        {
            Debug.Log($"Get<TResult>={result_s}");

            var r = DEF.LitJson.JsonMapper.ToObject<TResult>(result_s);

            Debug.Log($"Get<TResult>={result_s}, {r}");

            return r;
        }
    }

    public static async Task<TResult> Post<TResult>(string url)
    {
        Debug.Log(url);

#if UNITY_2022_1_OR_NEWER
        using var req = UnityWebRequest.PostWwwForm(url, "");
#else
        using var req = UnityWebRequest.Post(url,string.Empty);
#endif

        await req.SendWebRequest();

        var result_s = req.downloadHandler.text;
        if (string.IsNullOrEmpty(result_s))
        {
            return default;
        }
        else
        {
            var r = DEF.LitJson.JsonMapper.ToObject<TResult>(result_s);
            return r;
        }
    }

    public static async Task<byte[]> GetRawData(string url)
    {
        Debug.Log(url);

        using var req = UnityWebRequest.Get(url);

        await req.SendWebRequest();

        var data = req.downloadHandler.data;

        return data;
    }
}

#endif