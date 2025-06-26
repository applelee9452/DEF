#if DEF_CLIENT

using UnityEngine;
using YooAsset;

public class YooAssetDecryption : IDecryptionServices
{
    ClientCfg4Runtime ClientCfg4Runtime { get; set; }

    public YooAssetDecryption(ClientCfg4Runtime cfg)
    {
        ClientCfg4Runtime = cfg;
    }

    // AssetBundle解密方法
    DecryptResult IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo)
    {
        DecryptResult decryptResult = new();
        decryptResult.ManagedStream = null;
        decryptResult.Result = AssetBundle.LoadFromFile(fileInfo.FileLoadPath, fileInfo.FileLoadCRC, GetFileOffset());
        return decryptResult;
    }

    // AssetBundle解密方法
    DecryptResult IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo)
    {
        DecryptResult decryptResult = new();
        decryptResult.ManagedStream = null;
        decryptResult.CreateRequest = AssetBundle.LoadFromFileAsync(fileInfo.FileLoadPath, fileInfo.FileLoadCRC, GetFileOffset());
        return decryptResult;
    }

    DecryptResult IDecryptionServices.LoadAssetBundleFallback(DecryptFileInfo fileInfo)
    {
        throw new System.NotImplementedException();
    }

    // 原生文件解密方法
    byte[] IDecryptionServices.ReadFileData(DecryptFileInfo fileInfo)
    {
        throw new System.NotImplementedException();
    }

    // 原生文件解密方法
    string IDecryptionServices.ReadFileText(DecryptFileInfo fileInfo)
    {
        throw new System.NotImplementedException();
    }

    private static ulong GetFileOffset()
    {
        return 32;
    }

    //DecryptResult IDecryptionServices.LoadAssetBundle(DecryptFileInfo fileInfo)
    //{
    //    DecryptResult r = new();
    //    return r;

    //    //managedStream = null;

    //    //var data = File.ReadAllBytes(fileInfo.FileLoadPath);
    //    //byte[] data2;

    //    //if (ClientCfg4Runtime.IsEncrypt && !string.IsNullOrEmpty(ClientCfg4Runtime.EncryptKey) && !string.IsNullOrEmpty(ClientCfg4Runtime.EncryptNonce))
    //    //{
    //    //    byte[] bytes_dll = new byte[data.Length];

    //    //    byte[] key = Convert.FromBase64String(ClientCfg4Runtime.EncryptKey);
    //    //    byte[] nonce = Convert.FromBase64String(ClientCfg4Runtime.EncryptNonce);

    //    //    DEF.ChaCha20 a = new(key, nonce, 1);
    //    //    a.DecryptBytes(bytes_dll, data, data.Length);
    //    //    a.Dispose();

    //    //    data2 = bytes_dll;
    //    //}
    //    //else
    //    //{
    //    //    data2 = data;
    //    //}

    //    //return AssetBundle.LoadFromMemory(data2);
    //}

    //DecryptResult IDecryptionServices.LoadAssetBundleAsync(DecryptFileInfo fileInfo)
    //{
    //    DecryptResult r = new();
    //    return r;

    //    //managedStream = null;

    //    //var data = File.ReadAllBytes(fileInfo.FileLoadPath);
    //    //byte[] data2;

    //    //if (ClientCfg4Runtime.IsEncrypt && !string.IsNullOrEmpty(ClientCfg4Runtime.EncryptKey) && !string.IsNullOrEmpty(ClientCfg4Runtime.EncryptNonce))
    //    //{
    //    //    byte[] bytes_dll = new byte[data.Length];

    //    //    byte[] key = Convert.FromBase64String(ClientCfg4Runtime.EncryptKey);
    //    //    byte[] nonce = Convert.FromBase64String(ClientCfg4Runtime.EncryptNonce);

    //    //    DEF.ChaCha20 a = new(key, nonce, 1);
    //    //    a.DecryptBytes(bytes_dll, data, data.Length);
    //    //    a.Dispose();

    //    //    data2 = bytes_dll;
    //    //}
    //    //else
    //    //{
    //    //    data2 = data;
    //    //}

    //    //return AssetBundle.LoadFromMemoryAsync(data2);
    //}

    //byte[] IDecryptionServices.ReadFileData(DecryptFileInfo fileInfo)
    //{
    //    return null;
    //}

    //string IDecryptionServices.ReadFileText(DecryptFileInfo fileInfo)
    //{
    //    return string.Empty;
    //}
}

#endif