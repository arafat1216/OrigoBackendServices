using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class ModuleServices : IModuleServices
    {
        private readonly ILogger<CustomerServices> _logger;
        private readonly ICustomerRepository _customerRepository;

        public ModuleServices(ILogger<CustomerServices> logger, ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<IList<AssetCategoryType>> GetAllAssetCategoriesAsync()
        {
            return await _customerRepository.GetAssetCategoriesAsync();
        }

        public async Task<IList<ProductModule>> GetModulesAsync()
        {
            return await _customerRepository.GetModulesAsync();
        }
    }
}
