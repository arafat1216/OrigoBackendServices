using AssetServices;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace Asset.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    // Assets should only be available through a given customer
    [Route("api/v{version:apiVersion}/Assets/seed")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class AssetTestDataController : ControllerBase
    {
        private readonly IAssetTestDataService _testServices;
        private readonly ILogger<AssetTestDataController> _logger;
        public AssetTestDataController(ILogger<AssetTestDataController> logger, IAssetTestDataService testService)
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
                var result = await _testServices.CreateAssetTestData();
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
