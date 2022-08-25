using AutoMapper;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Exceptions;
using HardwareServiceOrderServices.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    /// <summary>
    ///     A controller that handles all organization/customer-specific configurations.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/configuration/organization/{organizationId:Guid}")]
    [Tags("Organization Configuration")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned when the system encountered an unexpected problem.")]
    public class OrganizationConfigurationController : ControllerBase
    {
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly ILogger<OrganizationConfigurationController> _logger;
        private readonly IMapper _mapper;


        /// <summary>
        ///     The default constructor. This is called/handled directly by ASP.NET using DI (dependency injection).
        /// </summary>
        /// <param name="hardwareServiceOrderService"> The injected <see cref="IHardwareServiceOrderService"/> instance. </param>
        /// <param name="logger"> The injected <see cref="ILogger"/> instance. </param>
        /// <param name="mapper"> The injected <see cref="IMapper"/> (automapper) instance. </param>
        /// <param name="apiRequesterService"> A injected <see cref="IApiRequesterService"/> instance. 
        ///     This records relevant information from the incoming HTTP-request, and makes this available outside the controller.
        ///     It must always be included in the controller's constructor, even if it's not utilized. </param>
        public OrganizationConfigurationController(IHardwareServiceOrderService hardwareServiceOrderService,
                                         ILogger<OrganizationConfigurationController> logger,
                                         IMapper mapper,
                                         IApiRequesterService apiRequesterService)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("service-provider")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(IEnumerable<ViewModels.CustomerServiceProvider>))]
        public async Task<ActionResult> GetCustomerServiceProvidersAsync([FromRoute] Guid organizationId, [FromQuery] bool includeApiCredentialIndicators = false)
        {
            var dtoResult = await _hardwareServiceOrderService.GetCustomerServiceProvidersAsync(organizationId, includeApiCredentialIndicators);
            var mappedResult = _mapper.Map<ViewModels.CustomerServiceProvider>(dtoResult);

            return Ok(mappedResult);
        }


        [HttpGet("service-provider/{serviceProviderId:int}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Returned when the system failed to locate the requested <c>CustomerServiceProvider</c>.")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ViewModels.CustomerServiceProvider))]
        public async Task<ActionResult> GetCustomerServiceProviderByIdAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromQuery] bool includeApiCredentialIndicators = false)
        {
            try
            {
                var dtoResults = await _hardwareServiceOrderService.GetCustomerServiceProviderByIdAsync(organizationId, serviceProviderId, includeApiCredentialIndicators);
                var mappedResult = _mapper.Map<ViewModels.CustomerServiceProvider>(dtoResults);

                return Ok(mappedResult);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }


        /// <summary>
        ///     Add or update service-provider API credentials for a customer.
        /// </summary>
        /// <remarks>
        ///     Adds or updates a single service-provider API credential for a given customer.
        ///     
        ///     <para>
        ///     If an existing credential exist (using the same unique combination of <c><paramref name="organizationId"/></c>, 
        ///     <c><paramref name="serviceProviderId"/></c> and <c><paramref name="serviceTypeId"/></c>), then it will be overwritten
        ///     with the new value. </para>
        /// </remarks>
        /// <param name="organizationId"> The organization the API credentials is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credentials is attached to. </param>
        /// <param name="serviceTypeId"> 
        ///     The service-type the API credentials can be used with.
        ///     
        ///     <para>
        ///     Please note that each service-type may only have one API credential. </para>
        /// </param>
        /// <param name="apiCredential"> The new API credentials. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpPut("service-provider/{serviceProviderId:int}/credentials")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Returned when the system failed to locate the specified API credential.")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddApiCredentialsAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromQuery][Required] int serviceTypeId, [FromBody] NewApiCredential apiCredential)
        {
            try
            {
                await _hardwareServiceOrderService.AddOrUpdateApiCredentialAsync(organizationId, serviceProviderId, apiCredential.ServiceTypeId, apiCredential.ApiUsername, apiCredential.ApiPassword);
                return NoContent();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        ///     Remove a service-provider API credential from a customer.
        /// </summary>
        /// <remarks>
        ///     Removes a given service-provider API credential for a given customer.
        /// </remarks>
        /// <param name="organizationId"> The organization the API credentials is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credentials is attached to. </param>
        /// <param name="serviceTypeId"> 
        ///     The service-type the API credentials can be used with.
        ///     
        ///     <para>
        ///     Please note that each service-type may only have one API credential. </para>
        /// </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpDelete("service-provider/{serviceProviderId:int}/credentials")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteApiCredentialsAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromQuery][Required] int serviceTypeId)
        {
            await _hardwareServiceOrderService.DeleteApiCredentialAsync(organizationId, serviceProviderId, serviceTypeId);
            return NoContent();
        }
    }
}
