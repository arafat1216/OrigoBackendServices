using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;
using OrigoApiGateway.Services;
using System.Net;
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
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
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

        private Guid GetCallerId()
        {
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            if (actor is null)
                throw new FormatException();

            return Guid.Parse(actor);
        }

        /// <summary>
        /// Getting the Role of the Currently LoggedIn/Authenticated User
        /// </summary>
        /// <returns>Returns User Role(<see cref="PredefinedRole"/>) or null</returns>
        private string? GetUserRole()
        {
            return HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
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
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin")]
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
        [HttpGet("configuration/organization/{organizationId:Guid}/service-provider")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin")]
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
        [HttpGet("configuration/organization/{organizationId:Guid}/service-provider/user")]
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
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> AddServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

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
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

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



        /// <summary>
        ///     Retrieves a paginated list that contains a organization's service-orders.
        /// </summary>
        /// <param name="organizationId"> The organization ID to retrieve service-orders for. </param>
        /// <param name="userId"> When provided, filters the results to only contain this user. </param>
        /// <param name="serviceTypeId"> When provided, filters the results to only contain this service-type. </param>
        /// <param name="activeOnly"> 
        ///     When <c><see langword="true"/></c>, only active/ongoing service-orders are retrieved. This takes precedence over <paramref name="userId"/>. 
        ///     When <c><see langword="false"/></c>, the filter is ignored. </param>
        /// <param name="myOrders"> When <see langword="true"/>, only the requester's own orders are retrieved.  When <c><see langword="false"/></c>, the filter is ignored. </param>
        /// <param name="page"> The paginated page that should be retrieved. </param>
        /// <param name="limit"> The number of items to retrieve per <paramref name="page"/>. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("organization/{organizationId:Guid}/orders")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin,DepartmentManager,Manager,EndUser")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(PagedModel<HardwareServiceOrder>))]
        public async Task<ActionResult> GetAllServiceOrdersForOrganizationAsync([FromRoute] Guid organizationId, [FromQuery] Guid? userId, [FromQuery] int? serviceTypeId, [FromQuery] bool activeOnly = false, [FromQuery] bool myOrders = false, [FromQuery] int page = 1, [FromQuery] int limit = 25)
        {
            try
            {
                if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                    return Forbid();

                // Frontend don't always have the user ID, so we have a special parameter that let's us know to filter the userID to the current user.
                if (myOrders)
                    userId = GetCallerId();
                else if (GetUserRole() == PredefinedRole.EndUser.ToString()) //Todo: Need to check whether this logic should be exported to a separate method
                    userId = GetCallerId();

                PagedModel<HardwareServiceOrder> results = await _hardwareServiceOrderService.GetAllServiceOrdersForOrganizationAsync(organizationId, userId, serviceTypeId, activeOnly, page, limit);
                return Ok(results);
            }
            catch (HttpRequestException ex)
            {
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return Unauthorized();
                    default:
                        throw;
                }
            }
        }


        /// <summary>
        ///     Retrieve a service-order by it's ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves a service-order that matches a given ID.
        /// </remarks>
        /// <param name="organizationId"> The organization that owns the service-order. </param>
        /// <param name="serviceOrderId"> The service-order ID that should be retrieved. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("organization/{organizationId:Guid}/orders/{serviceOrderId:Guid}")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin,DepartmentManager,Manager,EndUser")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(HardwareServiceOrder))]
        public async Task<IActionResult> GetServiceOrderByIdAndOrganizationAsync([FromRoute] Guid organizationId, [FromRoute] Guid serviceOrderId)
        {
            try
            {
                if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                    return Forbid();

                var result = await _hardwareServiceOrderService.GetServiceOrderByIdAndOrganizationAsync(organizationId, serviceOrderId);
                
                //Todo: Need to check whether this logic should be exported to a separate method
                if (GetUserRole() == PredefinedRole.EndUser.ToString() && result?.Owner.UserId != GetCallerId())
                    return Forbid();

                if (result is null)
                    return Forbid();
                else return
                        Ok(result);
            }
            catch (HttpRequestException ex)
            {
                switch (ex.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return Forbid();
                    default:
                        throw;
                }
            }
        }


        /// <summary>
        /// Creates a hardware service order for Service Type <c>Repair(SUR)</c>
        /// </summary>
        /// <remarks>
        ///     The Service Provider provides different Services such as Repair(SUR), Remarketing etc. This endpoint will create a <c>Repair(SUR)</c> type of Service Order.
        /// </remarks>
        /// <param name="organizationId">Customer Identifier</param>
        /// <param name="model">Order details</param>
        /// <returns>New hardware service order</returns>
        [Route("organization/{organizationId:Guid}/orders/repair")]
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(HardwareServiceOrder))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHardwareServiceOrderForSURAsync(Guid organizationId, [FromBody] NewHardwareServiceOrder model)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

                if (!Guid.TryParse(userId, out Guid userIdGuid))
                    return BadRequest();

                // Todo: The Integer value "3" represents ServiceType "SUR" which can be found inside enum ServiceTypeEnum. Later on this should be converted into actual Enum
                var newOrder = await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(organizationId, userIdGuid, 3, model);

                return Ok(newOrder);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Creates a hardware service order for Service Type <c>Remarketing</c>
        /// </summary>
        /// <remarks>
        ///     The Service Provider provides different Services such as SUR, Remarketing etc. This endpoint will create a <c>Remarketing</c> type of Service Order.
        /// </remarks>
        /// <param name="organizationId">Customer Identifier</param>
        /// <param name="model">Order details</param>
        /// <returns>New hardware service order</returns>
        [Route("organization/{organizationId:Guid}/orders/remarketing")]
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(HardwareServiceOrder))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHardwareServiceOrderForRemarketingAsync(Guid organizationId, [FromBody] NewHardwareServiceOrder model)
        {
            try
            {
                var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

                if (!Guid.TryParse(userId, out Guid userIdGuid))
                    return BadRequest();

                // Todo: The Integer value "2" represents ServiceType "Remarketing" which can be found inside enum ServiceTypeEnum. Later on this should be converted into actual Enum
                var newOrder = await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(organizationId, userIdGuid, 2, model);

                return Ok(newOrder);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                return BadRequest(ex.Message);
            }
        }



        /// <summary>
        ///     Updates a organization's settings.
        /// </summary>
        /// <remarks>
        ///     Updates the global customer-settings (service configurations) for a given organization. If no configuration exists, it is created.
        /// </remarks>
        /// <param name="organizationId"> The organization identifier. </param>
        /// <param name="newCustomerSettings"> The new or updated customer settings. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpPut("configuration/organization/{organizationId:Guid}")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CustomerSettings))]
        public async Task<IActionResult> AddOrUpdateCustomerSettings([FromRoute] Guid organizationId, [FromBody] NewCustomerSettings newCustomerSettings)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            CustomerSettings result = await _hardwareServiceOrderService.AddOrUpdateCustomerSettings(organizationId, newCustomerSettings);
            return Ok(result);
        }


        /// <summary>
        ///     Retrieves a organization's settings.
        /// </summary>
        /// <remarks>
        ///     Retrieves the global customer-settings (service configurations) for a given organization.
        ///     
        ///     <br/><br/>
        ///     This only retrieves the global settings (not tied to any service-providers). If you require the service-provider specific settings,
        ///     you must retrieved these separately.
        /// </remarks>
        /// <param name="organizationId"> The organization identifier. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("configuration/organization/{organizationId:Guid}")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(CustomerSettings))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Returned if no customer-settings exist for the provided organization.")]
        public async Task<IActionResult> GetCustomerSettingsAsync([FromRoute] Guid organizationId)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            try
            {
                var result = await _hardwareServiceOrderService.GetCustomerSettingsAsync(organizationId);

                if (result is null)
                    return NotFound("No customer-settings exist for this organization.");
                else
                    return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return NotFound();
                else
                    throw;
            }
        }
    }
}
