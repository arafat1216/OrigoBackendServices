using Common.Enums;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Common.Extensions;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles asset related service
    /// </summary>
    public class AssetService : IAssetService
    {
        private readonly AssetConfiguration _config;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options">Configuration for asset microservice</param>
        /// <param name="httpClient">HttpClient for calling the endpoints of asset microservice</param>
        public AssetService(IOptions<AssetConfiguration> options, HttpClient httpClient)
        {
            _config = options.Value;
            _httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task UpdateAssetLifeCycleStatusAsync<TRequest>(string endpoint, Guid assetLifecycleId, TRequest data)
        {
            JsonContent content = JsonContent.Create(data);
            await _httpClient.PatchAsync($"{_config.ApiPath}/{assetLifecycleId}/{endpoint}", content);
        }


        /// <inheritdoc/>
        public async Task UpdateAssetLifeCycleStatusForSURAsync(Guid assetLifecycleId, ServiceStatusEnum newServiceStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            HttpResponseMessage? result = null;

            switch (newServiceStatus)
            {
                // Service created/updated
                case ServiceStatusEnum.Registered:
                case ServiceStatusEnum.RegisteredInTransit:
                case ServiceStatusEnum.RegisteredUserActionNeeded:
                case ServiceStatusEnum.Ongoing:
                case ServiceStatusEnum.OngoingUserActionNeeded:
                case ServiceStatusEnum.OngoingInTransit:
                case ServiceStatusEnum.OngoingReadyForPickup:
                    JsonContent content = JsonContent.Create(Guid.Empty.SystemUserId());
                    result = await _httpClient.PatchAsync($"{_config.ApiPath}/{assetLifecycleId}/send-to-repair", content);
                    break;

                // Service Complete (regular repair)
                case ServiceStatusEnum.Canceled:
                case ServiceStatusEnum.CompletedNotRepaired:
                case ServiceStatusEnum.CompletedRepaired:
                case ServiceStatusEnum.CompletedRepairedOnWarranty:
                    result = await _httpClient.PutAsJsonAsync($"{_config.ApiPath}/{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = false });
                    break;

                // Service Complete: Device was replaced/swapped
                case ServiceStatusEnum.CompletedReplaced:
                case ServiceStatusEnum.CompletedReplacedOnWarranty:
                    result = await _httpClient.PutAsJsonAsync($"{_config.ApiPath}/{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = false, NewImei = newImeis, NewSerialNumber = newSerialNumber });
                    break;

                // Service Complete: Asset was discarded
                case ServiceStatusEnum.CompletedDiscarded:
                    result = await _httpClient.PutAsJsonAsync($"{_config.ApiPath}/{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = true });
                    break;

                // No assigned value: Throw an error!
                case ServiceStatusEnum.Null:
                    throw new ArgumentNullException(nameof(newServiceStatus), "The service status was not provided.");

                // Default & misc. unsupported actions: Throws an error!
                case ServiceStatusEnum.CompletedCredited:
                default:
                    throw new NotImplementedException("This is currently not supported.");
            }

            if (result is not null && !result.IsSuccessStatusCode)
            {
                throw new HttpRequestException("The external API-request returned a error-code. The asset lifecycle-status was not updated!");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAssetLifeCycleStatusForRemarketingAsync(Guid assetLifecycleId, ServiceStatusEnum newServiceStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            HttpResponseMessage? result = null;
            JsonContent content;

            switch (newServiceStatus)
            {
                case ServiceStatusEnum.Canceled:
                    content = JsonContent.Create(Guid.Empty.SystemUserId());
                    result = await _httpClient.PatchAsync($"{_config.ApiPath}/{assetLifecycleId}/cancel-return", content);
                    break;

                case ServiceStatusEnum.CompletedDiscarded:
                    content = JsonContent.Create(new { CallerId = Guid.Empty.SystemUserId(), AssetLifecycleStatus = AssetLifecycleStatus.Recycled });
                    result = await _httpClient.PatchAsync($"{_config.ApiPath}/{assetLifecycleId}/recycle", content);
                    break;

                case ServiceStatusEnum.Null:
                    throw new ArgumentNullException(nameof(newServiceStatus), "The service status was not provided.");
                default:
                    throw new NotImplementedException("This is currently not supported.");
            }

            if (result is not null && !result.IsSuccessStatusCode)
            {
                throw new HttpRequestException("The external API-request returned a error-code. The asset lifecycle-status was not updated!");
            }
        }
    }
}
