using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizations/seed")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class OrganizationTestDataController : ControllerBase
    {
        private readonly IOrganizationTestDataService _testServices;
        private readonly ILogger<OrganizationTestDataController> _logger;
        public OrganizationTestDataController(ILogger<OrganizationTestDataController> logger, IOrganizationTestDataService testService)
        {
            _logger = logger;
            _testServices = testService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<bool>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> CreateTestData()
        {
            try
            {
                var result = await _testServices.CreateOrganizationTestData();
                if (result != string.Empty)
                {
                    _logger.LogError("CreateTestData failed");
                }
                return Ok(new { result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }
    }
}
