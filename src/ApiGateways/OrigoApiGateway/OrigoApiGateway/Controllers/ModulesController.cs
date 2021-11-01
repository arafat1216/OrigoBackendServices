using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class ModulesController : ControllerBase
    {
        private ILogger<ModulesController> Logger { get; }
        private IModuleServices ModuleServices { get; }

        public ModulesController(ILogger<ModulesController> logger, IModuleServices customerServices)
        {
            Logger = logger;
            ModuleServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoProductModule>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<OrigoProductModule>>> GetModules(Guid? organizationId = null)
        {
            try
            {
                var modules = await ModuleServices.GetModulesAsync(organizationId);
                return modules != null ? Ok(modules) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
