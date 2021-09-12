using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class ModuleServices : IModuleServices
    {
        private readonly ILogger<OrganizationServices> _logger;
        private readonly IOrganizationRepository _customerRepository;

        public ModuleServices(ILogger<OrganizationServices> logger, IOrganizationRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        public async Task<IList<ProductModule>> GetModulesAsync()
        {
            return await _customerRepository.GetProductModulesAsync();
        }
    }
}
