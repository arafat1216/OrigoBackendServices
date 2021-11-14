using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class StorageService : IStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<StorageService> _logger;

        public StorageService(BlobServiceClient blobServiceClient, ILogger<StorageService> logger, IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Upload a file to Azure storage as a blob
        /// </summary>
        /// <param name="formFile">A file sent with HttpRequest, to be stored on Azure storage as a blob</param>
        /// <returns>void</returns>
        /// <exception cref="StorageException">Throws the exception if the file dont exist, or Azure Storage services encountered any problems</exception>
        public async Task UploadAssetsFileAsync(IFormFile formFile)
        {
            try
            {
                var containerName = _configuration.GetSection("Storage:ContainerName").Value;
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(formFile.FileName);

                using var stream = formFile.OpenReadStream();
                var response = await blobClient.UploadAsync(stream, true);
                if (response.GetRawResponse().Status != 201)
                {
                    throw new Azure.RequestFailedException(response.GetRawResponse().ReasonPhrase);
                }
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogError(ex, "UploadAssetsFile failed with Azure.RequestFailedException.");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "UploadAssetsFile failed with unknown exception.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the blob-file's content as a System.IO.Stream
        /// </summary>
        /// <param name="filename">The name or URI of the blob item</param>
        /// <returns>Returns the filebody as a System.IO.Stream</returns>
        /// <exception cref="StorageException">Throws the exception if the file dont exist, or Azure Storage services encountered any problems</exception>
        public async Task<Stream> GetAssetsFileAsStreamAsync(string fileName)
        {
            try
            {
                var containerName = _configuration.GetSection("Storage:ContainerName").Value;
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(fileName);
                if (!blobClient.Exists())
                    throw new ResourceNotFoundException(string.Format("The requested resource {0} was not found.", fileName), _logger);
              
                var response = await blobClient.DownloadAsync();

                   
                if (response.GetRawResponse().Status != 200)
                {
                    throw new Azure.RequestFailedException(response.GetRawResponse().ReasonPhrase);
                }

                return response.Value.Content;
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogError(ex, "GetAssetsFileAsStreamAsync failed with Azure.RequestFailedException.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAssetsFileAsStreamAsync failed with unknown exception.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the blobs file-names on the storage account
        /// </summary>
        /// <param name="filename">The name or URI of the blob item</param>
        /// <returns>Returns the blob file names as a List of strings</returns>
        /// <exception cref="StorageException">Throws the exception if the file dont exist, or Azure Storage services encountered any problems</exception>
        public async Task<IEnumerable<string>> GetBlobsAsync()
        {
            try
            {
                var containerName = _configuration.GetSection("Storage:ContainerName").Value;
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobNames = new List<string>();

                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    blobNames.Add(blobItem.Name);
                }

                return blobNames;
            }
            catch (Azure.RequestFailedException ex)
            {
                _logger.LogError(ex, "GetBlogsAsync failed with Azure.RequestFailedException.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBlogsAsync failed with unknown exception.");
                throw;
            }
        }
    }
}
