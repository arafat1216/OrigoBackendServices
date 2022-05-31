using Common.Enums;
using HardwareServiceOrderServices.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

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
        public async Task UpdateAssetLifeCycleStatusAsync(Guid customerId, Guid assetLifeCycleId, AssetLifecycleStatus status)
        {
            var requestBody = new List<Guid> { assetLifeCycleId };

            try
            {
                var request = await _httpClient.PostAsJsonAsync($"{_config.ApiPath}/customers/{customerId}/assetStatus/{(int)status}", requestBody);
                request.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
