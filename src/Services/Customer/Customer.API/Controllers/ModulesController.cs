using Customer.API.ViewModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleServices _moduleServices;
        private readonly ILogger<ModulesController> _logger;

        public ModulesController(ILogger<ModulesController> logger, IModuleServices userServices)
        {
            _logger = logger;
            _moduleServices = userServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductModule>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductModule>>> GetModules()
        {
            var modules = await _moduleServices.GetModulesAsync();
            List<ProductModule> modulesList = new List<ProductModule>();
            if (modules != null)
                modulesList.AddRange(modules.Select(module => new ProductModule
                {
                    Name = module.Name,
                    ProductModuleGroup = module.ProductModuleGroup.Select(moduleGroup => new ProductModuleGroup 
                    { 
                        Name = moduleGroup.Name,
                        ProductModuleGroupId = moduleGroup.ProductModuleGroupId
                    }).ToList(),
                    ProductModuleId = module.ProductModuleId
                }));

            return Ok(modulesList);
        }
    }
}
