using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Services;
using BackendModels = OrigoApiGateway.Models.HardwareServiceOrder.Backend;

#nullable enable

namespace OrigoApiGateway.Controllers
{
    /// <summary>
    ///     Backoffice administration APIs used for handling/configuring hardware-services.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/backoffice/hardware-service")]
    [Tags("Hardware Service: Backoffice")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returned when the user is not authenticated.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned if the system encounter an unexpected problem.")]
    public class HardwareServiceBackofficeController : ControllerBase
    {
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly IAssetServices _assetServices;


        /// <summary>
        ///     Initializes a new <see cref="HardwareServiceBackofficeController"/> class utilizing injections.
        /// </summary>
        /// <param name="hardwareServiceOrderService"> The injected <see cref="IHardwareServiceOrderService"/> implementation. </param>
        /// <param name="assetServices"> The injected <see cref="IAssetServices"/> implementation. </param>
        public HardwareServiceBackofficeController(IHardwareServiceOrderService hardwareServiceOrderService,
                                                   IAssetServices assetServices)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _assetServices = assetServices;
        }


        /// <summary>
        ///     Retrieves all service-providers.
        /// </summary>
        /// <remarks>
        ///     Retrieves every service-providers that exist in the system, including lists containing their supported/offered service-types and service-addons.
        /// </remarks>
        /// <returns> An <see cref="ActionResult"/> containing the HTTP-response. </returns>
        [HttpGet("service-provider")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returned when the request was successful.", typeof(IEnumerable<BackendModels.ServiceProvider>))]
        public async Task<ActionResult<IEnumerable<BackendModels.ServiceProvider>>> GetAllProvidersAsync()
        {
            var providers = await _hardwareServiceOrderService.GetAllServiceProvidersAsync(true, true);
            return Ok(providers);
        }
    }
}
