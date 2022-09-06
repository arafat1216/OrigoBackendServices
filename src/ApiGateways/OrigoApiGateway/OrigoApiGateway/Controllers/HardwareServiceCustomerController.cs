using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;
using OrigoApiGateway.Services;
using System.Security.Claims;

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


        /// <summary>
        ///     Checks if the authenticated user should have access to the given organization ID.
        /// </summary>
        /// <param name="organizationId"> The organization we are checking for access. </param>
        /// <returns> Returns <see langword="true"/> if the user has access. Otherwise it returns <see langword="false"/>. </returns>
        private bool AuthenticatedUserHasAccessToOrganization(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Note:    For security reasons, we should always do "true" checks rather then "false" checks when granting
            //          access, as we'd much rather let someone be rejected then allowed if we were to make a mistake.
            if (role == PredefinedRole.SystemAdmin.ToString())
            {
                return true;
            }
            // TODO: Add the partner-admin checks here once it's been implemented.
            else
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList is not null && accessList.Contains(organizationId.ToString()))
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        ///     Retrieves all service-providers.
        /// </summary>
        /// <remarks>
        ///     Retrieves every service-providers that exist in the system, including lists containing their supported/offered service-types and service-addons.
        /// </remarks>
        /// <param name="includeOfferedServiceOrderAddons"> When <c><see langword="true"/></c>, the <c>OfferedServiceOrderAddons</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <param name="includeSupportedServiceTypes"> When <c><see langword="true"/></c>, the <c>SupportedServiceTypeIds</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <returns> An <see cref="ActionResult"/> containing the HTTP-response. </returns>
        [HttpGet("service-provider")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,CustomerAdmin")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returned when the request was successful.", typeof(IEnumerable<CustomerPortalServiceProvider>))]
        public async Task<ActionResult> GetAllServiceProvidersAsync([FromQuery] bool includeSupportedServiceTypes = false, [FromQuery] bool includeOfferedServiceOrderAddons = false)
        {
            // TODO:    Eventually, when the 'customer-service-provider <--> asset-category' mapping is in place, we should refactor
            //          this (likely needs to be done through the entire call chain, all the way down to the EF models) so it only
            //          includes the service-providers the customer actually has assigned to them right now. For now though, we need to
            //          return the global 'master list', since we don't really have any way of knowing what service-provider's is actually
            //          used (although in reality it's fixed to "Conmodo" for now).

            var providers = await _hardwareServiceOrderService.CustomerPortalGetAllServiceProvidersAsync(includeSupportedServiceTypes, includeOfferedServiceOrderAddons);
            return Ok(providers);
        }


        /// <summary>
        ///     Retrieves all customer-service-provider configurations for a customer (My Business).
        /// </summary>
        /// <remarks>
        ///     Retrieves all <c>CustomerServiceProvider</c> configurations for a given customer.
        ///     
        ///     <para>
        ///     The results is filtered to only include the data that's relevant for the customer-portal's (My Business) configuration pages. </para>
        /// </remarks>
        /// <param name="organizationId"> The organization you are retrieving the <c>CustomerServiceProvider</c> for. </param>
        /// <param name="includeActiveServiceOrderAddons"> When <c><see langword="true"/></c>, the <c>ActiveServiceOrderAddons</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("configuration/organization/{organizationId:Guid}")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,CustomerAdmin")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(IEnumerable<CustomerPortalCustomerServiceProvider>))]
        public async Task<ActionResult> CustomerPortalGetCustomerServiceProvidersAsync([FromRoute] Guid organizationId, [FromQuery] bool includeActiveServiceOrderAddons = false)
        {
            // TODO:    Eventually, when the 'customer-service-provider <--> asset-category' mapping is in place, we should refactor
            //          this (likely needs to be done through the entire call chain, all the way down to the EF models) so it only
            //          includes the service-providers the customer actually has assigned to them right now.

            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            var result = await _hardwareServiceOrderService.CustomerPortalGetCustomerServiceProvidersAsync(organizationId, includeActiveServiceOrderAddons);
            return Ok(result);
        }


        /// <summary>
        ///     Retrieves all customer-service-provider configurations for a customer (EndUser).
        /// </summary>
        /// <remarks>
        ///     Retrieves all <c>CustomerServiceProvider</c> configurations for a given customer. 
        ///     
        ///     <para>
        ///     The results is filtered to only include the data that's relevant for the order-forms, and the user-portal's (My Page) configuration pages. </para>
        /// </remarks>
        /// <param name="organizationId"> The organization you are retrieving the <c>CustomerServiceProvider</c> for. </param>
        /// <param name="includeActiveServiceOrderAddons"> When <c><see langword="true"/></c>, the <c>ActiveServiceOrderAddons</c> property is
        ///     loaded/included in the retrieved data. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("configuration/organization/{organizationId:Guid}/user")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(IEnumerable<UserPortalCustomerServiceProvider>))]
        public async Task<ActionResult> UserPortalGetCustomerServiceProvidersAsync([FromRoute] Guid organizationId, [FromQuery] bool includeActiveServiceOrderAddons = false)
        {
            // TODO:    Eventually, when the 'customer-service-provider <--> asset-category' mapping is in place, we should refactor
            //          this (likely needs to be done through the entire call chain, all the way down to the EF models) so it only
            //          includes the service-providers the customer actually has assigned to them right now.

            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            var result = await _hardwareServiceOrderService.UserPortalGetCustomerServiceProvidersAsync(organizationId, includeActiveServiceOrderAddons);
            return Ok(result);
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
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,CustomerAdmin")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            try
            {
                // TODO: Improve the error/exception messages from this call
                await _hardwareServiceOrderService.AddServiceAddonFromCustomerPortalAsync(organizationId, serviceProviderId, serviceOrderAddonIds);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }

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
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            try
            {
                // TODO: Improve the error/exception messages from this call
                await _hardwareServiceOrderService.RemoveServiceAddonFromCustomerPortalAsync(organizationId, serviceProviderId, serviceOrderAddonIds);
                return NoContent();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }
    }
}
