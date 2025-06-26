#if DEF_CLIENT

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace DEF.Unity.Updater
{
    public static class ExtensionMethods
    {
        public static TaskAwaiter<object> GetAwaiter(this UnityWebRequestAsyncOperation op)
        {
            var tcs = new TaskCompletionSource<object>();
            op.completed += (obj) =>
            {
                tcs.SetResult(null);
            };
            return tcs.Task.GetAwaiter();
        }
    }

    public class UnityWebRequestAwaiter
    {
        public static async Task<Dictionary<string, string>> RequestPost(string url, string jsonArgs)
        {
            // 创建UnityWebRequest对象
            Debug.Log($"url:{url}");

            var www = UnityWebRequest.PostWwwForm(url, "");
            byte[] postBytes = Encoding.UTF8.GetBytes(jsonArgs);
            www.uploadHandler = new UploadHandlerRaw(postBytes);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // 发送请求并等待响应
            await www.SendWebRequest();

            // 处理响应
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                var str_ret = www.downloadHandler.text;
                return (JsonConvert.DeserializeObject<Dictionary<string, string>>(str_ret));
            }
        }

        public static async Task<Dictionary<string, string>> RequestGet(string url)
        {
            // 创建UnityWebRequest对象
            Debug.Log($"url:{url}");

            var www = UnityWebRequest.Get(url);
            www.downloadHandler = new DownloadHandlerBuffer();

            // 发送请求并等待响应
            await www.SendWebRequest();

            // 处理响应
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                var str_ret = www.downloadHandler.text;
                return (JsonConvert.DeserializeObject<Dictionary<string, string>>(str_ret));
            }
        }

        public static async Task<byte[]> RequestGetRaw(string url)
        {
            // 创建UnityWebRequest对象
            Debug.Log($"url:{url}");

            var www = UnityWebRequest.Get(url);
            www.downloadHandler = new DownloadHandlerBuffer();

            // 发送请求并等待响应
            await www.SendWebRequest();

            // 处理响应
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                var str_ret = www.downloadHandler.data;
                return str_ret;
            }
        }
    }
}

#endif