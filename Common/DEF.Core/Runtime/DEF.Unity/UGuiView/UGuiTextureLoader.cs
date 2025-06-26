#if DEF_CLIENT

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class UGuiTextureLoader : MonoBehaviour
{
    public void Load(string url, Action<string, Texture> cb)
    {
        StartCoroutine(DownloadTexture(url, cb));
    }

    IEnumerator DownloadTexture(string url, Action<string, Texture> cb)
    {
        using (var request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogErrorFormat("DownloadTexture Error: {0}，Url={1}", request.error, url);
                yield break;
            }

            cb?.Invoke(url, ((DownloadHandlerTexture)request.downloadHandler).texture);
        }
    }
}

#endif