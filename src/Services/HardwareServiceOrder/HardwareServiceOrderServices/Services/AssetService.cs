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

        /// <inheritdoc cref="UpdateAssetLifeCycleStatusAsync(Guid,Guid, AssetLifecycleStatus)"/>
        [Obsolete]
        public async Task UpdateAssetLifeCycleStatusAsync(Guid customerId, Guid assetLifeCycleId, AssetLifecycleStatus status)
        {
            var requestBody = new List<Guid> { assetLifeCycleId };

            throw new NotImplementedException();
            //try
            //{
            //    var request = await _httpClient.PostAsJsonAsync($"{_config.ApiPath}/customers/{customerId}/assetStatus/{(int)status}", requestBody);
            //    request.EnsureSuccessStatusCode();
            //}
            //catch (Exception)
            //{

            //    throw;
            //}
        }

        /// <inheritdoc/>
        public async Task UpdateAssetLifeCycleStatusAsync(Guid assetLifecycleId, ServiceStatusEnum newServiceStatus, IEnumerable<string>? newImeis, string? newSerialNumber)
        {
            HttpResponseMessage? result = null;

            switch (newServiceStatus)
            {
                // No assigned value: Throw an error!
                case ServiceStatusEnum.Null:
                    throw new ArgumentNullException(nameof(newServiceStatus), "The service status was not provided.");

                // New service-registration
                case ServiceStatusEnum.Registered:
                    JsonContent content = JsonContent.Create(Guid.Empty.SystemUserId());
                    result = await _httpClient.PatchAsync($"{_config.ApiPath}/{assetLifecycleId}/send-to-repair", content);
                    break;

                // These don't trigger any changes inside the asset-microservice
                case ServiceStatusEnum.Unknown:
                case ServiceStatusEnum.RegisteredInTransit:
                case ServiceStatusEnum.RegisteredUserActionNeeded:
                case ServiceStatusEnum.Ongoing:
                case ServiceStatusEnum.OngoingUserActionNeeded:
                case ServiceStatusEnum.OngoingInTransit:
                case ServiceStatusEnum.OngoingReadyForPickup:
                    break;

                // Repair completed
                case ServiceStatusEnum.Canceled:
                case ServiceStatusEnum.CompletedNotRepaired:
                case ServiceStatusEnum.CompletedRepaired:
                case ServiceStatusEnum.CompletedRepairedOnWarranty:
                    result = await _httpClient.PutAsJsonAsync($"{_config.ApiPath}/{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = false });
                    break;

                // Device replaced
                case ServiceStatusEnum.CompletedReplaced:
                case ServiceStatusEnum.CompletedReplacedOnWarranty:
                    result = await _httpClient.PutAsJsonAsync($"{_config.ApiPath}/{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = false, NewImei = newImeis, NewSerialNumber = newSerialNumber });
                    break;

                // Device discarded
                case ServiceStatusEnum.CompletedDiscarded:
                    result = await _httpClient.PutAsJsonAsync($"{_config.ApiPath}/{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = true });
                    break;

                // Unsupported & default actions
                case ServiceStatusEnum.CompletedCredited:
                default:
                    throw new NotImplementedException("This is currently not supported.");
            }

            if (result is not null && !result.IsSuccessStatusCode)
            {
                throw new HttpRequestException("The request did not return a success code!");
            }
        }
    }
}
