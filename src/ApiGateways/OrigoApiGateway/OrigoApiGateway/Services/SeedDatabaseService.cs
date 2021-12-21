using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Services
{
    public class SeedDatabaseService : ISeedDatabaseService
    {
        private readonly ILogger<SeedDatabaseService> _logger;
        private IAssetServices _assetServices { get; set; }
        private ICustomerServices _customerServices { get; set; }

        public SeedDatabaseService(ILogger<SeedDatabaseService> logger, IAssetServices assetServices, ICustomerServices customerServices)
        {
            _logger = logger;
            _assetServices = assetServices;
            _customerServices = customerServices;
        }

        public async Task<string> CreateTestData()
        {
            string result = "Unkown error";
            try
            {
                result = await _customerServices.CreateOrganizationSeedData();
                result += await _assetServices.CreateAssetSeedData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return result;
        }
    }
}
