using Aliyun.OSS;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DEF.Cloud;

public class StorageOss : IStorage
{
    OssClient OssClient { get; set; }
    string Uri { get; set; }

    public StorageOss()
    {
        //OssClient = new OssClient(
        //    Config.ConfigUCenter.StorageAliOssEndpoint,
        //    Config.ConfigUCenter.StorageAliOssAccessKeyId,
        //    Config.ConfigUCenter.StorageAliOssAccessKeySecret);

        ////string s = string.Format(Config.ConfigCommon.OssRootDir, Config.ConfigCommon.Cluster + Config.ConfigCommon.Env);
        //string s = Config.ConfigCommon.Cluster + Config.ConfigCommon.Env;
        //Uri = string.Format(Config.ConfigUCenter.ImageContainerName, s);
    }

    async Task<string> IStorage.UploadBlobAsync(string bucket_name, string blob_name, Stream stream)
    {
        string key = Uri + blob_name;
        //Console.WriteLine(key);

        var result = await Task<PutObjectResult>.Factory.FromAsync(
            OssClient.BeginPutObject, OssClient.EndPutObject,
            bucket_name, key, stream, null);

        //Console.WriteLine("StorageAliyun.UploadBlobAsync() Endpoint={0} PutObjectResult={1}",
        //   Config.ConfigUCenter.StorageAliOssEndpoint, result.HttpStatusCode);

        //if (result.ResponseMetadata != null)
        //{
        //    foreach (var i in result.ResponseMetadata)
        //    {
        //        Console.WriteLine("Key={0} Value={1}", i.Key, i.Value);
        //    }
        //}

        var uri = OssClient.GeneratePresignedUri(bucket_name, key);
        //Console.WriteLine("Uri=" + uri);

        string s = uri.GetLeftPart(UriPartial.Path);

        s = Path.GetFileNameWithoutExtension(s);

        return s;
    }

    Task<string> IStorage.CopyBlobAsync(string bucket_name, string source_blob_name, string target_blob_name)
    {
        return Task.FromResult(string.Empty);
    }
}
