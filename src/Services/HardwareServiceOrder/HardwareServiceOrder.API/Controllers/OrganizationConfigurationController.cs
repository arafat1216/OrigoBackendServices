﻿using AutoMapper;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Exceptions;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
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


        /// <summary>
        ///     Retrieves all customer-service-provider configurations for a customer.
        /// </summary>
        /// <remarks>
        ///     Retrieves all <c>CustomerServiceProvider</c> configurations for a given customer.
        /// </remarks>
        /// <param name="organizationId"> The organization you are retrieving the <c><see cref="CustomerServiceProvider"/></c>'s for. </param>
        /// <param name="includeApiCredentialIndicators"> When <c><see langword="true"/></c>, the <c>ApiCredentials</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("service-provider")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(IEnumerable<ViewModels.CustomerServiceProvider>))]
        public async Task<ActionResult> GetCustomerServiceProvidersAsync([FromRoute] Guid organizationId, [FromQuery] bool includeApiCredentialIndicators = false)
        {
            var dtoResult = await _hardwareServiceOrderService.GetCustomerServiceProvidersAsync(organizationId, includeApiCredentialIndicators);
            var mappedResult = _mapper.Map<IEnumerable<ViewModels.CustomerServiceProvider>>(dtoResult);

            return Ok(mappedResult);
        }


        /// <summary>
        ///     Retrieves a specific customer-service-provider configuration.
        /// </summary>
        /// <remarks>
        ///     Retrieves the <c>CustomerServiceProvider</c> configuration that matches a given organization and service-provider.
        /// </remarks>
        /// <param name="organizationId"> The customer/organization identifier. </param>
        /// <param name="serviceProviderId"> The <c><see cref="ServiceProvider"/></c> identifier. </param>
        /// <param name="includeApiCredentialIndicators"> When <c><see langword="true"/></c>, the <c>ApiCredentials</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("service-provider/{serviceProviderId:int}")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Returned when the system failed to locate the requested <c>CustomerServiceProvider</c>.")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(ViewModels.CustomerServiceProvider))]
        public async Task<ActionResult> GetCustomerServiceProviderByIdAsync([FromRoute] Guid organizationId, [FromRoute][EnumDataType(typeof(ServiceProviderEnum))] int serviceProviderId, [FromQuery] bool includeApiCredentialIndicators = false)
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
        ///     Adds or updates an API credential for a customer's service-provider configuration.
        /// </summary>
        /// <remarks>
        ///     Adds a new API credential to a customer's service-provider configuration (customer-service-provider).
        ///     
        ///     <para>
        ///     If an existing credential already exist (using the same unique combination of <c><paramref name="organizationId"/></c>, 
        ///     <c><paramref name="serviceProviderId"/></c> and <c>ServiceTypeId</c>), then it will be overwritten
        ///     using the new values. </para>
        /// </remarks>
        /// <param name="organizationId"> The customer/organization the API credentials is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credentials is attached to. </param>
        /// <param name="apiCredential"> The new API credentials. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpPut("service-provider/{serviceProviderId:int}/credentials")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddOrUpdateApiCredentialsAsync([FromRoute] Guid organizationId, [FromRoute][EnumDataType(typeof(ServiceProviderEnum))] int serviceProviderId, [FromBody] NewApiCredential apiCredential)
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
        ///     Removes an API credential from a customer's service-provider configuration.
        /// </summary>
        /// <remarks>
        ///     Removes an existing API credential from a customer's service-provider configuration (customer-service-provider).
        /// </remarks>
        /// <param name="organizationId"> The customer/organization the API credentials is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credentials is attached to. When omitted, the default/fallback API credential is deleted. </param>
        /// <param name="serviceTypeId"> The service-type the API credentials is attached to. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpDelete("service-provider/{serviceProviderId:int}/credentials")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteApiCredentialsAsync([FromRoute] Guid organizationId, [FromRoute][EnumDataType(typeof(ServiceProviderEnum))] int serviceProviderId, [FromQuery][EnumDataType(typeof(ServiceTypeEnum))] int? serviceTypeId)
        {
            await _hardwareServiceOrderService.DeleteApiCredentialAsync(organizationId, serviceProviderId, serviceTypeId);
            return NoContent();
        }


        [HttpPatch("service-provider/{serviceProviderId:int}/addons")]
        public async Task<ActionResult> AddServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute][EnumDataType(typeof(ServiceProviderEnum))] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            throw new NotImplementedException();
        }


        [HttpDelete("service-provider/{serviceProviderId:int}/addons")]
        public async Task<ActionResult> RemoveServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute][EnumDataType(typeof(ServiceProviderEnum))] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            throw new NotImplementedException();
        }
    }
}
