using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Services;

#nullable enable

namespace OrigoApiGateway.Controllers
{
    /*
     * Customer API endpoints:
     * 
     * These endpoints should not offer any additional functionality/data outside what's available in the user/customer-portal.
     * If partner/system-admins, or the backoffice requires additional functionality/data/access, this should be added into a
     * separate backoffice version of the endpoint.
     */

    /// <summary>
    ///     Customer APIs used for configuring, placing and handling service-requests.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/hardware-service")]
    [Tags("Hardware Service: Customer")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Returned when the user is not authenticated.")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned if the system encounter an unexpected problem.")]
    public class HardwareServiceCustomerController : ControllerBase
    {
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly IAssetServices _assetServices;


        /// <summary>
        ///     Initializes a new <see cref="HardwareServiceCustomerController"/> class utilizing injections.
        /// </summary>
        /// <param name="hardwareServiceOrderService"> The injected <see cref="IHardwareServiceOrderService"/> implementation. </param>
        /// <param name="assetServices"> The injected <see cref="IAssetServices"/> implementation. </param>
        public HardwareServiceCustomerController(IHardwareServiceOrderService hardwareServiceOrderService,
                                                 IAssetServices assetServices)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _assetServices = assetServices;
        }

    }
}
