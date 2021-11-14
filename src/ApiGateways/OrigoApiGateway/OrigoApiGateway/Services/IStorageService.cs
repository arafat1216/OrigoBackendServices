using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IStorageService
    {
        Task UploadAssetsFileAsync(IFormFile formFile);
        Task<Stream> GetAssetsFileAsStreamAsync(string fileName);
        Task<IEnumerable<string>> GetBlobsAsync();
    }
}
