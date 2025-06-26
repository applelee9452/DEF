#if DEF_CLIENT

using System;
using System.Text;
using System.Threading.Tasks;
using DEF.Unity.Updater;
using UnityEngine;
using UnityEngine.Networking;

namespace DEF
{
    public class RpcHttpClient
    {
        string Url { get; set; }
        IClient Client { get; set; }

        public RpcHttpClient(IClient client, string url)
        {
            Client = client;
            Url = url;
        }

        public Task Request<TSerializeObj>(RpcInfoClientUnityILR rpcinfo, string method_name, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            string url;

            if (rpcinfo.ContainerOrEntity)
            {
                if (rpcinfo.ContainerStateType == ContainerStateType.Stateless)
                {
                    string r = $"{rpcinfo.TargetServiceName.ToLower()}|{(int)rpcinfo.ContainerStateType}|{rpcinfo.ContainerType}|{method_name}";
                    url = string.Format("{0}/api/ccr2?r={1}", Url, r);
                }
                else
                {
                    string r = $"{rpcinfo.TargetServiceName.ToLower()}|{(int)rpcinfo.ContainerStateType}|{rpcinfo.ContainerType}|{rpcinfo.ContainerId}|{method_name}";
                    url = string.Format("{0}/api/ccr2?r={1}", Url, r);
                }
            }
            else
            {
                string r = $"{rpcinfo.TargetServiceName.ToLower()}|{(int)rpcinfo.ContainerStateType}|{rpcinfo.ContainerType}|{rpcinfo.ContainerId}|{rpcinfo.EntityId}|{rpcinfo.ComponentName}|{method_name}";
                url = string.Format("{0}/api/cer2?r={1}", Url, r);
            }

            return Request<TSerializeObj>(url, so);
        }

        async Task Request<TSerializeObj>(string url, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            Debug.Log(url);

            byte[] bytes = null;
            if (so != null)
            {
                bytes = EntitySerializer.Serialize(Client.Config.SerializerType, so);
            }

            using var req = UnityWebRequest.PostWwwForm(url, string.Empty);
            if (bytes != null)
            {
                req.uploadHandler = (UploadHandler)new UploadHandlerRaw(bytes);
            }

            await req.SendWebRequest();
        }

        public Task<TResult> RequestResponse<TResult, TSerializeObj>(RpcInfoClientUnityILR rpcinfo, string method_name, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            string url;

            if (rpcinfo.ContainerOrEntity)
            {
                if (rpcinfo.ContainerStateType == ContainerStateType.Stateless)
                {
                    string r = $"{rpcinfo.TargetServiceName.ToLower()}|{(int)rpcinfo.ContainerStateType}|{rpcinfo.ContainerType}|{method_name}";
                    url = string.Format("{0}/api/ccr?r={1}", Url, r);
                }
                else
                {
                    string r = $"{rpcinfo.TargetServiceName.ToLower()}|{(int)rpcinfo.ContainerStateType}|{rpcinfo.ContainerType}|{rpcinfo.ContainerId}|{method_name}";
                    url = string.Format("{0}/api/ccr?r={1}", Url, r);
                }
            }
            else
            {
                string r = $"{rpcinfo.TargetServiceName.ToLower()}|{(int)rpcinfo.ContainerStateType}|{rpcinfo.ContainerType}|{rpcinfo.ContainerId}|{rpcinfo.EntityId}|{rpcinfo.ComponentName}|{method_name}";
                url = string.Format("{0}/api/cer?r={1}", Url, r);
            }

            return RequestResponse<TResult, TSerializeObj>(url, so);
        }

        async Task<TResult> RequestResponse<TResult, TSerializeObj>(string url, TSerializeObj so) where TSerializeObj : SerializeObj
        {
            Debug.Log(url);

            byte[] request_bytes = null;
            if (so != null)
            {
                request_bytes = EntitySerializer.Serialize(Client.Config.SerializerType, so);
            }

            WWWForm form = new();
            form.AddField("Accept", "application/octet-stream");
            form.AddField("Accept-Encoding", "identity");
            form.AddField("Accept-Charset", "utf-8");

            using var req = UnityWebRequest.PostWwwForm(url, form.ToString());
            if (request_bytes != null)
            {
                req.uploadHandler = (UploadHandler)new UploadHandlerRaw(request_bytes);
            }

            byte[] raw_data = null;

            try
            {
                await req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(req.error);
                }
                else
                {
                    raw_data = req.downloadHandler.data;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            if (raw_data == null)
            {
                return default;
            }

            var result_s = Encoding.UTF8.GetString(raw_data);
            result_s = result_s[1..^1];

            var result_bytes = Convert.FromBase64String(result_s);

            var r = EntitySerializer.Deserialize<TResult>(Client.Config.SerializerType, result_bytes);

            return r;
        }
    }
}

#endif
