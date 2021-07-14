using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;

namespace CustomerServices
{
    public class AssetCategoryLifecycleTypeServices : IAssetCategoryLifecycleTypeServices
    {
        private readonly ILogger<AssetCategoryLifecycleTypeServices> _logger;
        private readonly ICustomerRepository _customerRepository;

        public AssetCategoryLifecycleTypeServices(ILogger<AssetCategoryLifecycleTypeServices> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<AssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, Guid assetCategoryId, int lifecycleType)
        {
            var customer = await _customerRepository.GetCustomerAsync(customerId);
            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            var newAssetCategoryLifecycleType = new AssetCategoryLifecycleType(customerId, assetCategoryId, lifecycleType);

            return await _customerRepository.AddAssetCategoryLifecycleTypeAsync(newAssetCategoryLifecycleType);
        }

        public Task<IList<AssetCategoryLifecycleType>> GetAllAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId)
        {
            return _customerRepository.GetAllAssetCategoryLifecycleTypesAsync(customerId);
        }
    }
}
