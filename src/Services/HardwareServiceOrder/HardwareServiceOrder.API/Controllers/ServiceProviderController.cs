using AutoMapper;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Exceptions;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    /// <summary>
    ///     A controller that handles service-providers.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/service-provider")]
    [Tags("Service-provider")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public class ServiceProviderController : ControllerBase
    {
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly ILogger<ServiceProviderController> _logger;
        private readonly IMapper _mapper;


        /// <summary>
        ///     The default constructor. This is called/handled directly by ASP.NET using DI (dependency injection).
        /// </summary>
        /// <param name="hardwareServiceOrderService"> DI: The service-implementation that's used for all requests to the service-project. </param>
        /// <param name="logger"></param>
        /// <param name="mapper"> DI: A automapper instance. </param>
        /// <param name="apiRequesterService"> DI: Records relevant information from the incoming HTTP-request, and makes this available outside the controller.
        ///     This must always be included in the controller's constructor, even if it's not utilized. </param>
        public ServiceProviderController(IHardwareServiceOrderService hardwareServiceOrderService,
                                         ILogger<ServiceProviderController> logger,
                                         IMapper mapper,
                                         IApiRequesterService apiRequesterService)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        ///     Retrieves all service-providers
        /// </summary>
        /// <remarks>
        ///     Retrieves the master-list, containing all service-providers in the system. If requested, it is also possible to include 
        ///     additional information about the service-provider's offerings and capabilities.
        /// </remarks>
        /// <param name="includeSupportedServiceTypes">
        ///     When <c><see langword="true"/></c>, the <c>SupportedServiceTypeIds</c> property is loaded/included in the retrieved data. 
        ///     
        ///     <para>
        ///     This property details what kinds of service-types that can be created using this service-provider. </para>
        /// </param>
        /// <param name="includeOfferedServiceOrderAddons">
        ///     When <c><see langword="true"/></c>, the <c>OfferedServiceOrderAddons</c> property is loaded/included in the retrieved data. 
        ///     
        ///     <para>
        ///     This property contains all the service-addons that exist for this service-provider. </para>
        /// </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(IEnumerable<ServiceProviderDTO>))]
        public async Task<ActionResult> GetAllServiceProvidersAsync([FromQuery] bool includeSupportedServiceTypes = false, [FromQuery] bool includeOfferedServiceOrderAddons = false)
        {
            var serviceProviders = await _hardwareServiceOrderService.GetAllServiceProvidersAsync(includeSupportedServiceTypes, includeOfferedServiceOrderAddons);
            return Ok(serviceProviders);
        }


        /// <summary>
        ///     Retrieves a service-provider by it's ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves a service-provider by it's ID. If requested, it is also possible to include 
        ///     additional information about the service-provider's offerings and capabilities.
        /// </remarks>
        /// <param name="serviceProviderId"> The service-provider's ID. </param>
        /// <param name="includeSupportedServiceTypes">
        ///     When <c><see langword="true"/></c>, the <c>SupportedServiceTypeIds</c> property is loaded/included in the retrieved data. 
        ///     
        ///     <para>
        ///     This property details what kinds of service-types that can be created using this service-provider. </para>
        /// </param>
        /// <param name="includeOfferedServiceOrderAddons">
        ///     When <c><see langword="true"/></c>, the <c>OfferedServiceOrderAddons</c> property is loaded/included in the retrieved data. 
        ///     
        ///     <para>
        ///     This property contains all the service-addons that exist for this service-provider. </para>
        /// </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("{serviceProviderId:int}")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ServiceProviderDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetServiceProvidersByIdAsync([FromRoute][EnumDataType(typeof(ServiceProviderEnum))] int serviceProviderId, [FromQuery] bool includeSupportedServiceTypes = false, [FromQuery] bool includeOfferedServiceOrderAddons = false)
        {
            try
            {
                var serviceProvider = await _hardwareServiceOrderService.GetServiceProviderById(serviceProviderId, includeSupportedServiceTypes, includeOfferedServiceOrderAddons);
                return Ok(serviceProvider);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }


    }
}
