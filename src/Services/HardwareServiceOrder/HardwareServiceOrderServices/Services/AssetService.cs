using Common.Enums;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Common.Extensions;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles asset related service. <see cref="IAssetService"/>
    /// </summary>
    public class AssetService : IAssetService
    {
        private readonly AssetConfiguration _config;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes the AssetService which will be used to call the Asset microservice in order to Update asset lifecycle.
        /// </summary>
        /// <param name="options">Configuration for asset microservice</param>
        /// <param name="httpClient">HttpClient for calling the endpoints of asset microservice</param>
        public AssetService(IOptions<AssetConfiguration> options, HttpClient httpClient)
        {
            _config = options.Value;
            _httpClient = httpClient;
        }

        /// <summary>
        ///     Send a HTTP-request to specified URL as an asynchronous operation.
        ///     <para>In this case, the Http calls most likely will be made to Asset Microservice to update the asset lifecycle</para>
        /// </summary>
        /// <param name="httpMethod"> The HTTP-method to use for the request. </param>
        /// <param name="requestUri"> The requested URL. </param>
        /// <param name="inputValue"> The item that should be serialized and to the HTTP-request. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        /// <exception cref="HttpRequestException"> Thrown if the HTTP request resulted in an error-code. </exception>
        private async Task SendRequestAsync<TInput>(HttpMethod httpMethod, string requestUri, TInput inputValue) where TInput : notnull
        {
            HttpResponseMessage response;

            string constructedRequestUrl = $"{_config.ApiPath}/{requestUri}";

            using var request = new HttpRequestMessage(httpMethod, constructedRequestUrl);

            request.Content = JsonContent.Create(inputValue);

            response = await _httpClient.SendAsync(request);

            string? responseBodyAsString = await response.Content.ReadAsStringAsync();

             response.EnsureSuccessStatusCode();
        }

        /// <inheritdoc/>
        public async Task UpdateAssetLifeCycleStatusAsync<TRequest>(HttpMethod httpMethod, string endpointSuffix, Guid assetLifecycleId, TRequest data) where TRequest : notnull
        {
            await SendRequestAsync(httpMethod, $"{assetLifecycleId}/{endpointSuffix}", data);
        }


        /// <inheritdoc/>
        public async Task UpdateAssetLifeCycleStatusForSURAsync(Guid assetLifecycleId, ServiceStatusEnum newServiceStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
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
                    await SendRequestAsync(HttpMethod.Patch, $"{assetLifecycleId}/send-to-repair", Guid.Empty.SystemUserId());
                    break;

                // Service Complete (regular repair)
                case ServiceStatusEnum.Canceled:
                case ServiceStatusEnum.CompletedNotRepaired:
                case ServiceStatusEnum.CompletedRepaired:
                case ServiceStatusEnum.CompletedRepairedOnWarranty:
                    await SendRequestAsync(HttpMethod.Put, $"{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = false });
                    break;

                // Service Complete: Device was replaced/swapped
                case ServiceStatusEnum.CompletedReplaced:
                case ServiceStatusEnum.CompletedReplacedOnWarranty:
                    await SendRequestAsync(HttpMethod.Put, $"{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = false, NewImei = newImeis, NewSerialNumber = newSerialNumber });
                    break;

                // Service Complete: Asset was discarded
                case ServiceStatusEnum.CompletedDiscarded:
                    await SendRequestAsync(HttpMethod.Put, $"{assetLifecycleId}/repair-completed", new { CallerId = Guid.Empty.SystemUserId(), Discarded = true });
                    break;

                // No assigned value: Throw an error!
                case ServiceStatusEnum.Null:
                    throw new ArgumentNullException(nameof(newServiceStatus), "The service status was not provided.");

                // Default & misc. unsupported actions: Throws an error!
                case ServiceStatusEnum.CompletedCredited:
                default:
                    throw new NotImplementedException("This is currently not supported.");
            }
        }

        /// <inheritdoc/>
        public async Task UpdateAssetLifeCycleStatusForRemarketingAsync(Guid assetLifecycleId, ServiceStatusEnum newServiceStatus, ISet<string>? newImeis, string? newSerialNumber)
        {
            switch (newServiceStatus)
            {
                case ServiceStatusEnum.Canceled:
                    await SendRequestAsync(HttpMethod.Patch, $"{assetLifecycleId}/cancel-return", Guid.Empty.SystemUserId());
                    break;

                case ServiceStatusEnum.CompletedDiscarded:
                    await SendRequestAsync(HttpMethod.Patch, $"{assetLifecycleId}/recycle", new { CallerId = Guid.Empty.SystemUserId(), AssetLifecycleStatus = AssetLifecycleStatus.Recycled });
                    break;

                case ServiceStatusEnum.Null:
                    throw new ArgumentNullException(nameof(newServiceStatus), "The service status was not provided.");

                default:
                    throw new NotImplementedException("This is currently not supported.");
            }

        }
    }
}
