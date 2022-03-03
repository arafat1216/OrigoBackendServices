using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// <param name="organizationId"></param>
        /// <param name="formFile">A file sent with HttpRequest, to be stored on Azure storage as a blob</param>
        /// <returns>void</returns>
        /// <exception cref="ResourceNotFoundException">Throws the exception if the file dont exist</exception>
        /// /// <exception cref="Azure.RequestFailedException">Throws the exception if Azure had any problems handling the request</exception>
        public async Task UploadAssetsFileAsync(Guid organizationId, IFormFile formFile)
        {
            try
            {
                string fileName = organizationId.ToString() + "/" + formFile.FileName;
                var containerName = _configuration.GetSection("Storage:ContainerName").Value;
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                //containerClient.
                var blobClient = containerClient.GetBlobClient(fileName);
               

                using var stream = formFile.OpenReadStream();
                var response = await blobClient.UploadAsync(stream, true);
                if (response.GetRawResponse().Status != 201)
                {
                    throw new Azure.RequestFailedException(response.GetRawResponse().ReasonPhrase);
                }
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "UploadAssetsFile failed with ResourceNotFoundException.");
                throw;
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
        /// <param name="organizationId"></param>
        /// <returns>Returns the filebody as a System.IO.Stream</returns>
        /// <exception cref="ResourceNotFoundException">Throws the exception if the file don't exist</exception>
        /// /// <exception cref="Azure.RequestFailedException">Throws the exception if Azure had any problems handling the request</exception>
        public async Task<Stream> GetAssetsFileAsStreamAsync(Guid organizationId, string filename)
        {
            try
            {
                var containerName = _configuration.GetSection("Storage:ContainerName").Value;
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                string path_file = organizationId.ToString() + "/" + filename;
                var blobClient = containerClient.GetBlobClient(path_file);

                if (!blobClient.Exists())
                    throw new ResourceNotFoundException(string.Format("The requested resource {0} was not found.", filename), _logger);

                Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadInfo> response = await blobClient.DownloadAsync();

                   
                if (response.GetRawResponse().Status != 200)
                {
                    throw new Azure.RequestFailedException(response.GetRawResponse().ReasonPhrase);
                }

                return response.Value.Content;
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "GetAssetsFileAsStreamAsync failed with ResourceNotFoundException.");
                throw;
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
        /// <returns>Returns the blob file names as a List of strings</returns>
        /// <exception cref="ResourceNotFoundException">Throws the exception if the file dont exist</exception>
        /// <exception cref="Azure.RequestFailedException">Throws the exception if Azure had any problems handling the request</exception>
        public async Task<IEnumerable<string>> GetBlobsAsync(Guid organizationId)
        {
            try
            {
                string orgId = organizationId.ToString();
                var containerName = _configuration.GetSection("Storage:ContainerName").Value;
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                //var blobClient = containerClient.Get(orgId + "/");
                var blobNames = new List<string>();
                
               
                
                // Find all files for organizationId
                await foreach (var blobItem in containerClient.GetBlobsAsync(BlobTraits.None, BlobStates.None, orgId + "/"))
                {
                    blobNames.Add(blobItem.Name.Split("/")[1]); // Do not include custId as name of file.
                }

                // Check if item was found
                if (blobNames.Count == 0)
                {
                    throw new ResourceNotFoundException(string.Format("The requested resource {0} was not found.", orgId), _logger);
                }

                return blobNames;
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogError(ex, "GetBlogsAsync failed with ResourceNotFoundException.");
                throw;
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
