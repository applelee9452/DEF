using System.IO;
using System.Threading.Tasks;

namespace DEF.Cloud;

public interface IStorage
{
    Task<string> UploadBlobAsync(string bucket_name, string blob_name, Stream stream);

    Task<string> CopyBlobAsync(string bucket_name, string source_blob_name, string target_blob_name);
}