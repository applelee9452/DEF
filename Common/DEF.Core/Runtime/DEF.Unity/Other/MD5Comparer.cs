#if DEF_CLIENT

using System.IO;
using System.Security.Cryptography;
using System.Text;

public class MD5Comparer
{
    static MD5 MD5 { get; set; } = new MD5CryptoServiceProvider();
    static StringBuilder Sb { get; set; } = new StringBuilder(256);

    static public bool IsSameFile(string local_filepath, string remote_filemd5)
    {
        Sb.Clear();
        using (FileStream sr = File.OpenRead(local_filepath))
        {
            byte[] new_bytes = MD5.ComputeHash(sr);
            foreach (var bytes in new_bytes)
            {
                Sb.Append(bytes.ToString("X2"));
            }
        }
        bool is_same = Sb.ToString().Equals(remote_filemd5);
        return is_same;
    }

    static public string GetFileMD5New(string local_filepath, out long file_size)
    {
        Sb.Clear();
        file_size = 0;
        using (FileStream sr = File.OpenRead(local_filepath))
        {
            file_size = sr.Length;
            byte[] new_bytes = MD5.ComputeHash(sr);
            foreach (var bytes in new_bytes)
            {
                Sb.Append(bytes.ToString("X2"));
            }
        }
        return Sb.ToString();
    }

    static public string GetFileMD5(string local_filepath)
    {
        Sb.Clear();
        using (FileStream sr = File.OpenRead(local_filepath))
        {
            byte[] new_bytes = MD5.ComputeHash(sr);
            foreach (var bytes in new_bytes)
            {
                Sb.Append(bytes.ToString("X2"));
            }
        }
        return Sb.ToString();
    }
}

#endif