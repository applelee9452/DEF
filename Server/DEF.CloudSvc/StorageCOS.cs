using System.IO;
using System.Threading.Tasks;

namespace DEF.Cloud;

public class StorageCOS : IStorage
{
    Task<string> IStorage.UploadBlobAsync(string bucket_name, string blob_name, Stream stream)
    {
        return Task.FromResult(string.Empty);
    }

    Task<string> IStorage.CopyBlobAsync(string bucket_name, string source_blob_name, string target_blob_name)
    {
        return Task.FromResult(string.Empty);
    }
}