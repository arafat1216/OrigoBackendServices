using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IStorageService
    {
        Task UploadAssetsFileAsync(Guid organizationId, IFormFile formFile);
        Task<Stream> GetAssetsFileAsStreamAsync(Guid organizationId, string fileName);
        Task<IEnumerable<string>> GetBlobsAsync(Guid organizationId);
    }
}
