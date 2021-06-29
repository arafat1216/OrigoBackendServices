using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        public IModuleServices ModuleServices { get; }

        public ModulesController(ILogger<ModulesController> logger, IModuleServices customerServices)
        {
            Logger = logger;
            ModuleServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoProductModule>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<OrigoProductModule>>> GetModules()
        {
            try
            {
                var modules = await ModuleServices.GetModulesAsync();
                return modules != null ? Ok(modules) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
