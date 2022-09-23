using System.Security.Claims;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;
using OrigoApiGateway.Services;
using BackendModels = OrigoApiGateway.Models.HardwareServiceOrder.Backend;

#nullable enable

namespace OrigoApiGateway.Controllers
{
    /// <summary>
    ///     Backoffice administration APIs used for handling/configuring hardware-services.
    /// </summary>
    [Authorize(Roles = "SystemAdmin,PartnerAdmin")]
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
        ///     Checks if the authenticated user should have access to the given organization ID.
        /// </summary>
        /// <param name="organizationId"> The organization we are checking for access. </param>
        /// <returns> Returns <see langword="true"/> if the user has access. Otherwise it returns <see langword="false"/>. </returns>
        private bool AuthenticatedUserHasAccessToOrganization(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

            // Note:    For security reasons, we should always do "true" checks rather then "false" checks when granting
            //          access, as we'd much rather let someone be rejected then allowed if we were to make a mistake.
            if (role == PredefinedRole.SystemAdmin.ToString())
            {
                return true;
            }

            if (accessList.Contains(organizationId.ToString()))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        ///     Retrieves all service-providers.
        /// </summary>
        /// <remarks>
        ///     Retrieves every service-providers that exist in the system, including lists containing their supported/offered service-types and service-addons.
        /// </remarks>
        /// <param name="includeSupportedServiceTypes"> When <c><see langword="true"/></c>, the <c>SupportedServiceTypeIds</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <param name="includeOfferedServiceOrderAddons"> When <c><see langword="true"/></c>, the <c>OfferedServiceOrderAddons</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <returns> An <see cref="ActionResult"/> containing the HTTP-response. </returns>
        [HttpGet("service-provider")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returned when the request was successful.", typeof(IEnumerable<BackendModels.ServiceProvider>))]
        public async Task<ActionResult<IEnumerable<BackendModels.ServiceProvider>>> GetAllProvidersAsync([FromQuery] bool includeSupportedServiceTypes = false, [FromQuery] bool includeOfferedServiceOrderAddons = false)
        {
            var providers = await _hardwareServiceOrderService.GetAllServiceProvidersAsync(includeSupportedServiceTypes, includeOfferedServiceOrderAddons);
            return Ok(providers);
        }


        /// <summary>
        ///     Retrieves all customer-service-provider configurations for a customer.
        /// </summary>
        /// <remarks>
        ///     Retrieves all <c>CustomerServiceProvider</c> configurations for a given customer.
        /// </remarks>
        /// <param name="organizationId"> The organization you are retrieving the <c>CustomerServiceProvider</c>'s for. </param>
        /// <param name="includeApiCredentialIndicators"> When <c><see langword="true"/></c>, the <c>ApiCredentials</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <param name="includeActiveServiceOrderAddons"> When <c><see langword="true"/></c>, the <c>ActiveServiceOrderAddons</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("configuration/organization/{organizationId:Guid}/service-provider")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(IEnumerable<CustomerServiceProvider>))]
        public async Task<ActionResult> GetCustomerServiceProvidersAsync([FromRoute] Guid organizationId, [FromQuery] bool includeApiCredentialIndicators = false, [FromQuery] bool includeActiveServiceOrderAddons = false)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            var result = await _hardwareServiceOrderService.GetCustomerServiceProvidersAsync(organizationId, includeApiCredentialIndicators, includeActiveServiceOrderAddons);
            return Ok(result);
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
        [HttpPut("configuration/organization/{organizationId:Guid}/service-provider/{serviceProviderId:int}/credentials")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddOrUpdateApiCredentialsAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromBody] NewApiCredential apiCredential)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            await _hardwareServiceOrderService.AddOrUpdateApiCredentialAsync(organizationId, serviceProviderId, apiCredential);
            return NoContent();
        }


        /// <summary>
        ///     Removes an API credential from a customer's service-provider configuration.
        /// </summary>
        /// <remarks>
        ///     Removes an existing API credential from a customer's service-provider configuration (customer-service-provider).
        /// </remarks>
        /// <param name="organizationId"> The customer/organization the API credentials is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credentials is attached to. </param>
        /// <param name="serviceTypeId"> The service-type the API credentials is attached to. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpDelete("configuration/organization/{organizationId:Guid}/service-provider/{serviceProviderId:int}/credentials")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteApiCredentialsAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromQuery] int? serviceTypeId)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            await _hardwareServiceOrderService.DeleteApiCredentialsAsync(organizationId, serviceProviderId, serviceTypeId);
            return NoContent();
        }


        /// <summary>
        ///     Add a service-order addon to a customer's service-provider configuration.
        /// </summary>
        /// <remarks>
        ///     Adds new service-order addons to a customer's service-provider configuration. (customer-service-provider). Pre-existing items will not be affected.
        ///     
        ///     <br/><br/>
        ///     You may only add service-order addons that is provided by the corresponding <c><paramref name="serviceProviderId"/></c>.
        /// </remarks>
        /// <param name="organizationId"> The customer/organization that's being configured. </param>
        /// <param name="serviceProviderId"> The service-provider that's being configured. </param>
        /// <param name="serviceOrderAddonIds"> A list containing the service-order IDs that should be added. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpPatch("configuration/organization/{organizationId:Guid}/service-provider/{serviceProviderId:int}/addons")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            await _hardwareServiceOrderService.AddServiceAddonFromBackofficeAsync(organizationId, serviceProviderId, serviceOrderAddonIds);
            return NoContent();
        }


        /// <summary>
        ///     Remove a service-order addon from a customer's service-provider configuration.
        /// </summary>
        /// <remarks>
        ///     Removes service-order addons from a customer's service-provider configuration. (customer-service-provider).
        /// </remarks>
        /// <param name="organizationId"> The customer/organization that's being configured. </param>
        /// <param name="serviceProviderId"> The service-provider that's being configured. </param>
        /// <param name="serviceOrderAddonIds"> A list containing the service-order IDs that should be removed. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpDelete("configuration/organization/{organizationId:Guid}/service-provider/{serviceProviderId:int}/addons")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RemoveServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            await _hardwareServiceOrderService.RemoveServiceAddonFromBackofficeAsync(organizationId, serviceProviderId, serviceOrderAddonIds);
            return NoContent();
        }

    }
}
