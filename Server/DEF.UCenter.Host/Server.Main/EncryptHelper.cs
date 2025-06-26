using System.Security.Cryptography;
using System.Text;

namespace DEF.UCenter;

public class EncryptHelper
{
    static readonly byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

    // DES encrypt method.
    public static string Encrypt(string str, string key)
    {
        key = key.PadRight(8, ' ');
        byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
        byte[] rgbIv = Keys;
        byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
        DES dcsp = DES.Create();// new DESCryptoServiceProvider();
        MemoryStream memoryStream = new MemoryStream();
        CryptoStream cryptoStream = new CryptoStream(memoryStream, dcsp.CreateEncryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
        cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
        cryptoStream.FlushFinalBlock();

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    // DES decrypt method.
    public static string Decrypt(string str, string key)
    {
        try
        {
            key = key.PadRight(8, ' ');
            byte[] rgbKey = Encoding.UTF8.GetBytes(key.Substring(0, 8));
            byte[] rgbIv = Keys;
            byte[] inputByteArray = Convert.FromBase64String(str);
            DES dcsp = DES.Create(); //DESCryptoServiceProvider dcsp = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, dcsp.CreateDecryptor(rgbKey, rgbIv), CryptoStreamMode.Write);
            cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
            cryptoStream.FlushFinalBlock();

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
        catch
        {
            return str;
        }
    }

    public static string SHA256(string str)
    {
        byte[] data = (new UnicodeEncoding()).GetBytes(str);

        byte[] result = (System.Security.Cryptography.SHA256.Create()).ComputeHash(data);
        string hashedValue = Convert.ToBase64String(result);

        return hashedValue;
    }

    public static string MD5(string str)
    {
        byte[] bytes = Encoding.Default.GetBytes(str);

        bytes = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);

        string retValue = string.Empty;
        for (int i = 0; i < bytes.Length; i++)
        {
            retValue += bytes[i].ToString("x").PadLeft(2, '0');
        }

        return retValue;
    }

    public static string ComputeHash(string str)
    {
        return SHA256(str);
    }

    public static bool VerifyHash(string str, string hash)
    {
        return SHA256(str) == hash;
    }
}