using System;
using System.IO;

namespace YooAsset.Editor
{
    public class YooAssetEncryption : IEncryptionServices
    {
        public EncryptResult Encrypt(EncryptFileInfo fileInfo)
        {
            var cfg = EditorContext.Instance.EditorCfg.ClientCfg4Runtime;

            var data = File.ReadAllBytes(fileInfo.FileLoadPath);

            if (cfg.IsEncrypt && !string.IsNullOrEmpty(cfg.EncryptKey) && !string.IsNullOrEmpty(cfg.EncryptNonce))
            {
                byte[] bytes_dll = new byte[data.Length];

                byte[] key = Convert.FromBase64String(cfg.EncryptKey);
                byte[] nonce = Convert.FromBase64String(cfg.EncryptNonce);

                DEF.ChaCha20 a = new(key, nonce, 1);
                a.DecryptBytes(bytes_dll, data, data.Length);
                a.Dispose();

                EncryptResult r = new()
                {
                    Encrypted = true,
                    EncryptedData = data
                };

                return r;
            }
            else
            {
                EncryptResult r = new()
                {
                    Encrypted = false,
                    EncryptedData = data
                };

                return r;
            }
        }
    }
}