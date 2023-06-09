﻿using System.Net;
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
using OrigoApiGateway.Models;
using OrigoApiGateway.Filters;

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
    [ServiceFilter(typeof(ErrorExceptionFilter))]
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
        ///     Checks if the authenticated user should have access to the given Asset
        /// </summary>
        /// <param name="asset"> The Asset that User trying to do some operation on <see cref="OrigoAsset"/></param>
        /// <param name="organizationId"> Organization Identifier </param>
        /// <returns> Returns <see langword="true"/> if the user has access. Otherwise it returns <see langword="false"/>. </returns>
        private bool CheckUserHasAccessToAsset(Guid organizationId, HardwareSuperType asset)
        {
            var userRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

            if (!Guid.TryParse(userId, out Guid userIdGuid))
                return false;

            // If the User Role is SystemAdmin, then he/she is allowed to access the Asset.
            if (userRole == PredefinedRole.SystemAdmin.ToString())
                return true;

            // User will be able to access his/her own Asset
            if (userIdGuid == asset.AssetHolderId)
                return true;

            // For the DepartmentManager/Manager, the accessList will contain list of Departments.
            // So if the Asset belongs to the same Department of the DepartmentManager/Manager.
            // then he/she is allowed to access the Asset.
            if ((userRole == PredefinedRole.DepartmentManager.ToString() || userRole == PredefinedRole.Manager.ToString()) &&
                asset.ManagedByDepartmentId is not null &&
                accessList.Contains(asset.ManagedByDepartmentId.ToString()))
            {
                return true;
            }

            // For the CustomerAdmin/Admin, the accessList will contain the OrganizationId that the CustomerAdmin belongs to.
            // For the PartnerAdmin, the accessList will contain the list of OrganizationIds that the PartnerAdmin has access to.
            // So here, we are checking whether the PartnerAdmin/CustomerAdmin has access to the desired Organization
            if ((userRole == PredefinedRole.CustomerAdmin.ToString() || userRole == PredefinedRole.Admin.ToString() || userRole == PredefinedRole.PartnerAdmin.ToString()) &&
                accessList.Contains(organizationId.ToString()))
            {
                return true;
            }

            // For any unexpected or not implemented scenario, the access should be denied.
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

            await _hardwareServiceOrderService.AddServiceAddonFromCustomerPortalAsync(organizationId, serviceProviderId, serviceOrderAddonIds);
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
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RemoveServiceAddonAsync([FromRoute] Guid organizationId, [FromRoute] int serviceProviderId, [FromBody][Required] ISet<int> serviceOrderAddonIds)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            await _hardwareServiceOrderService.RemoveServiceAddonFromCustomerPortalAsync(organizationId, serviceProviderId, serviceOrderAddonIds);
            return NoContent();
        }



        /// <summary>
        ///     Retrieves a paginated list that contains a organization's service-orders.
        /// </summary>
        /// <param name="organizationId"> The organization ID to retrieve service-orders for. </param>
        /// <param name="filterOptions"> A class containing the various query-parameter filters. </param>
        /// <param name="userId"> When provided, filters the results to only contain this user. </param>
        /// <param name="serviceTypeId"> When provided, filters the results to only contain this service-type. </param>
        /// <param name="activeOnly"> 
        ///     When <c><see langword="true"/></c>, only active/ongoing service-orders are retrieved. This takes precedence over <paramref name="userId"/>. 
        ///     When <c><see langword="false"/></c>, the filter is ignored. </param>
        /// <param name="myOrders"> When <see langword="true"/>, only the requester's own orders are retrieved.  When <c><see langword="false"/></c>, the filter is ignored. </param>
        /// <param name="search"> 
        ///     If a value is provided, a lightweight "contains" search is applied to a few select key-properties.
        ///     <br/><br/>
        ///     The following properties is searched for: <br/>
        ///     - Order-number
        ///     - First Name
        ///     - Last Name
        ///     - E-mail
        ///     - Asset Product name
        ///     - Asset Brand
        ///     <br/><br/>
        ///     When the value is <c><see langword="null"/>, the filter is ignored.</c>
        /// </param>
        /// <param name="page"> The paginated page that should be retrieved. </param>
        /// <param name="limit"> The number of items to retrieve per <paramref name="page"/>. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("organization/{organizationId:Guid}/orders")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin,DepartmentManager,Manager,EndUser")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(PagedModel<HardwareServiceOrder>))]
        public async Task<ActionResult> GetAllServiceOrdersForOrganizationAsync([FromRoute] Guid organizationId, [FromQuery] FilterOptionsForHardwareServiceOrder filterOptions, [FromQuery] Guid? userId, [FromQuery] int? serviceTypeId,
            [FromQuery] bool activeOnly = false, [FromQuery] bool myOrders = false, [FromQuery(Name = "q")] string? search = null, [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int limit = 25)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            // Frontend don't always have the user ID, so we have a special parameter that let's us know to filter the userID to the current user.
            if (myOrders)
                userId = GetCallerId();
            else if (GetUserRole() == PredefinedRole.EndUser.ToString()) //Todo: Need to check whether this logic should be exported to a separate method
                userId = GetCallerId();

            PagedModel<HardwareServiceOrder> results = await _hardwareServiceOrderService.GetAllServiceOrdersForOrganizationAsync(organizationId, filterOptions, userId, serviceTypeId, activeOnly, search, page, limit);
            return Ok(results);
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


        /// <summary>
        /// Creates a hardware service order for Service Type <c>Repair(SUR)</c>
        /// </summary>
        /// <remarks>
        ///     The Service Provider provides different Services such as Repair(SUR), Remarketing etc. This endpoint will create a <c>Repair(SUR)</c> type of Service Order.
        /// </remarks>
        /// <param name="organizationId">Customer Identifier</param>
        /// <param name="model">Service Order details</param>
        /// <returns>New hardware service order</returns>
        [HttpPost("organization/{organizationId:Guid}/orders/repair")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin,DepartmentManager,Manager,EndUser")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(HardwareServiceOrder))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHardwareServiceOrderForSURAsync(Guid organizationId, [FromBody] NewHardwareServiceOrder model)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (!Guid.TryParse(userId, out Guid userIdGuid))
                return Unauthorized();

            var asset = (HardwareSuperType)await _assetServices.GetAssetForCustomerAsync(organizationId, model.AssetId, null, true, true, includeContractHolderUser: true);

            if (asset is null)
                throw new ArgumentException($"Asset does not exist with ID {model.AssetId}", nameof(model.AssetId));

            if (!CheckUserHasAccessToAsset(organizationId, asset))
                return Forbid();

            // Todo: The Integer value "3" represents ServiceType "SUR" which can be found inside enum ServiceTypeEnum. Later on this should be converted into actual Enum
            var newOrder = await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(organizationId, userIdGuid, 3, model, asset);

            return Ok(newOrder);
        }

        /// <summary>
        /// Creates a hardware service order for Service Type <c>Remarketing</c>
        /// </summary>
        /// <remarks>
        ///     The Service Provider provides different Services such as SUR, Remarketing etc. This endpoint will create a <c>Remarketing</c> type of Service Order.
        /// </remarks>
        /// <param name="organizationId">Customer Identifier</param>
        /// <param name="model">Service Order details</param>
        /// <returns>New hardware service order</returns>
        [HttpPost("organization/{organizationId:Guid}/orders/remarketing")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin,PartnerReadOnlyAdmin,GroupAdmin,CustomerAdmin,Admin,DepartmentManager,Manager,EndUser")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(HardwareServiceOrder))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateHardwareServiceOrderForRemarketingAsync(Guid organizationId, [FromBody] NewHardwareAftermarketOrder model)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (!Guid.TryParse(userId, out Guid userIdGuid))
                return Unauthorized();

            var asset = (HardwareSuperType)await _assetServices.GetAssetForCustomerAsync(organizationId, model.AssetId, null, true, true, includeContractHolderUser: true);

            if (asset is null)
                throw new ArgumentException($"Asset does not exist with ID {model.AssetId}", nameof(model.AssetId));

            if (!CheckUserHasAccessToAsset(organizationId, asset))
                return Forbid();

            // Todo: The Integer value "2" represents ServiceType "Remarketing" which can be found inside enum ServiceTypeEnum. Later on this should be converted into actual Enum
            var newOrder = await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(organizationId, userIdGuid, 2, model, asset);

            return Ok(newOrder);
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

            var result = await _hardwareServiceOrderService.GetCustomerSettingsAsync(organizationId);

            if (result is null)
                return NotFound("No customer-settings exist for this organization.");
            else
                return Ok(result);
        }


        /// <summary>
        ///     Retrieves a organization's loan-device settings.
        /// </summary>
        /// <remarks>
        ///     Retrieves a given customer's loan-device settings.
        /// </remarks>
        /// <param name="organizationId"> The organization identifier. </param>
        /// <returns> A task containing the appropriate action-result. </returns>
        [HttpGet("configuration/organization/{organizationId:Guid}/loan-device")]
        [SwaggerResponse(StatusCodes.Status200OK, null, typeof(LoanDeviceSettings))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Returned if the requested loan-device settings don't exist, or has not yet been configured.")]
        public async Task<IActionResult> GetCustomerLoanDeviceSettingsAsync([FromRoute] Guid organizationId)
        {
            if (!AuthenticatedUserHasAccessToOrganization(organizationId))
                return Forbid();

            var result = await _hardwareServiceOrderService.GetCustomerLoanDeviceSettingsAsync(organizationId);

            if (result is null)
                return NotFound("The requested loan-device settings don't exist, or has not yet been configured.");
            else
                return Ok(result);

        }
    }
}
