using AssetServices.Models;
using CustomerServices.SeedData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices
{
    public class AssetTestDataService : IAssetTestDataService
    {
        private readonly IAssetLifecycleRepository _assetLifecycleRepository;
        private readonly ILogger<AssetTestDataService> _logger;
        private static Guid OrganizationId1 { get; set; } = new Guid("A19EA756-86F2-423C-9B10-11CB10181858");
        private static Guid OrganizationId2 { get; set; } = new Guid("F2B5B8E5-78E1-4643-B97B-49239DAC74C2");
        private readonly Guid _callerId = new("D0326090-631F-4138-9CD2-85249AD24BBB");

        public AssetTestDataService(ILogger<AssetTestDataService> logger, IAssetLifecycleRepository assetLifecycleRepository)
        {
            _assetLifecycleRepository = assetLifecycleRepository;
            _logger = logger;
        }

        public async Task<string> CreateAssetTestData()
        {
            StringBuilder builder = new();
            try
            {
                builder.Append(await CreateAssetLables());
                builder.Append(await CreateAssetsData());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return builder.ToString();
        }

        private async Task<string> CreateAssetLables()
        {
            string errorMessage = string.Empty;
            Guid[] organizationIds = new Guid[] { OrganizationId1, OrganizationId2 };
            try
            {
                foreach (Guid organizationId in organizationIds)
                {
                    var customerLabels = Seed.GetCustomerLables(organizationId);
                    var existingLabels = await _assetLifecycleRepository.GetCustomerLabelsForCustomerAsync(organizationId, true);
                    foreach (var label in customerLabels)
                    {
                        if (existingLabels.FirstOrDefault(l => l.ExternalId == label.ExternalId) == null)
                        {
                            await _assetLifecycleRepository.AddCustomerLabelsForCustomerAsync(organizationId, new List<CustomerLabel>() { label });
                        }
                        else
                        {
                            await _assetLifecycleRepository.UpdateCustomerLabelsForCustomerAsync(organizationId, new List<CustomerLabel>() { label });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                errorMessage = "Customer Label creation exception\r\n";
            }
            return errorMessage;
        }

        private async Task<string> CreateAssetsData()
        {
            string errorMessage = string.Empty;
            try
            {
                // 1 = Mobile Phone, 2 = Tablet 
                var assets = Seed.GetAssetData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                errorMessage = "Asset creation exception\r\n";
            }
            return errorMessage;
        }
    }
}
