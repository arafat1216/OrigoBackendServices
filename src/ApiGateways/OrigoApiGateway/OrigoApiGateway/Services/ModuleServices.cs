using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class ModuleServices : IModuleServices
    {
        public ModuleServices(ILogger<ModuleServices> logger, HttpClient httpClient,
           IOptions<ModuleConfiguration> options, ICustomerServices customerServices)
        {
            _logger = logger;
            HttpClient = httpClient;
            _options = options.Value;
            _customerServices = customerServices;
        }

        private readonly ILogger<ModuleServices> _logger;
        private HttpClient HttpClient { get; }
        private readonly ModuleConfiguration _options;
        private readonly ICustomerServices _customerServices;

        public async Task<IList<OrigoProductModule>> GetModulesAsync(Guid? customerId)
        {
            try
            {
                var modules = await HttpClient.GetFromJsonAsync<IList<ModuleDTO>>($"{_options.ApiPath}");
                if (modules == null) return null;
                var moduleList = new List<OrigoProductModule>();
                IList<OrigoProductModuleGroup> activeModules = new List<OrigoProductModuleGroup>();
                if (customerId != null)
                {
                    activeModules = await _customerServices.GetCustomerProductModulesAsync(customerId.Value);
                }
                foreach (var module in modules)
                {
                    OrigoProductModule origoProduct = new OrigoProductModule()
                    {
                        ProductModuleId = module.ProductModuleId,
                        Name = module.Name,
                        ProductModuleGroup = module.ProductModuleGroup.Select(moduleGroup => new OrigoProductModuleGroup(moduleGroup, activeModules)).ToList()
                    };
                    origoProduct.IsChecked = origoProduct?.ProductModuleGroup?.FirstOrDefault(p => p.IsChecked) != null;
                    moduleList.Add(origoProduct);
                }
                return moduleList;
            }
            catch (HttpRequestException exception)
            {
                _logger.LogError(exception, "Get modules failed with HttpRequestException.");
                throw;
            }
            catch (NotSupportedException exception)
            {
                _logger.LogError(exception, "Get modules failed with content type is not valid.");
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Get modules unknown error.");
                throw;
            }
        }
    }
}
