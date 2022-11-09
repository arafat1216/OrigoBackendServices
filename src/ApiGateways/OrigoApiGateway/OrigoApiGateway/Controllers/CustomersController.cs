using AutoMapper;
using Common.Enums;
using Common.Exceptions;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.SubscriptionManagement;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;
using OrigoApiGateway.Models.TechstepCore;
using OrigoApiGateway.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;

// ReSharper disable RouteTemplates.RouteParameterConstraintNotResolved

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class CustomersController : ControllerBase
    {
        private ILogger<CustomersController> Logger { get; }
        private ICustomerServices CustomerServices { get; }
        private ISubscriptionManagementService SubscriptionManagementService { get; }
        private readonly IMapper Mapper;

        public CustomersController(
            ILogger<CustomersController> logger,
            ICustomerServices customerServices,
            IMapper mapper,
            ISubscriptionManagementService subscriptionManagementService
            )
        {
            Logger = logger;
            CustomerServices = customerServices;
            Mapper = mapper;
            SubscriptionManagementService = subscriptionManagementService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<Organization>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<Organization>>> Get([FromQuery] Guid? partnerId = null, [FromQuery] bool includePreferences = true)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString() && role != PredefinedRole.PartnerAdmin.ToString()) return Forbid();
            if (role == PredefinedRole.PartnerAdmin.ToString())
            {
                var access = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (Guid.TryParse(access, out var parsedGuid))
                {
                    partnerId = parsedGuid;
                }
                else return Forbid();

            }

            var customers = await CustomerServices.GetCustomersAsync(partnerId, includePreferences);

            return customers != null ? Ok(customers) : NotFound();
        }

        /// <summary>
        /// Get all organizations.
        /// </summary>
        /// <remarks>
        /// Retrieves a paginated set containing all matching organizations.
        /// </remarks>
        /// <param name="cancellationToken"> A dependency-injected <see cref="CancellationToken"/>. </param>
        /// <param name="partnerId"> If provided, the results will be filtered to only include organizations belonging to this partner. </param>
        /// <param name="search">
        ///     If a value is provided, a lightweight "contains" search is applied to the following key-properties:
        ///     <br/><br/>
        ///     - Name
        /// </param>        
        /// <param name="includePreferences"> When <c><see langword="true"/></c>, the <c>Preferences</c> property is
        ///     loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para></param>        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The highest number of items that can be added in a single page. </param>
        /// <returns> The asynchronous task. The task results contains the corresponding <see cref="ActionResult{TValue}"/>. </returns>
        [HttpGet("pagination")]
        [ProducesResponseType(typeof(PagedModel<Organization>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<PagedModel<Organization>>> GetPaginatedOrganizationsAsync(CancellationToken cancellationToken, [FromQuery][SwaggerSchema(Nullable = true)] Guid? partnerId = null, [FromQuery(Name = "q")][SwaggerSchema(Nullable = true)] string? search = null, [FromQuery] bool includePreferences = false, [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int limit = 25)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString() && role != PredefinedRole.PartnerAdmin.ToString()) return Forbid();
            if (role == PredefinedRole.PartnerAdmin.ToString())
            {
                var access = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (Guid.TryParse(access, out var parsedGuid))
                {
                    partnerId = parsedGuid;
                }
                else
                {
                    return Forbid();
                }
            }

            var customers = await CustomerServices.GetPaginatedCustomersAsync(cancellationToken, page, limit, partnerId, search: search, includePreferences: includePreferences);

            return customers != null ? Ok(customers) : NotFound();
        }

        [Route("{organizationId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<Organization>>> Get(Guid organizationId)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var customer = await CustomerServices.GetCustomerAsync(organizationId);
            return customer != null ? Ok(customer) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> CreateCustomer([FromBody] NewOrganization newCustomer)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role != PredefinedRole.SystemAdmin.ToString() && role != PredefinedRole.PartnerAdmin.ToString())
            {
                return Forbid();
            }
            if (role == PredefinedRole.PartnerAdmin.ToString())
            {
                var access = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (Guid.TryParse(access, out var parsedGuid))
                {
                    if (newCustomer.PartnerId == null) newCustomer.PartnerId = parsedGuid;
                    if (newCustomer.PartnerId != parsedGuid) return Forbid();

                }
            }

            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid.TryParse(actor, out Guid callerId);

            var createdCustomer = await CustomerServices.CreateCustomerAsync(newCustomer, callerId);
            if (createdCustomer == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(CreateCustomer), new { id = createdCustomer.OrganizationId }, createdCustomer);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> UpdateOrganization([FromBody] UpdateOrganization organizationToChange)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }
            // If role is not System admin, check access list
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationToChange.OrganizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var organizationToChangeDTO = Mapper.Map<UpdateOrganizationDTO>(organizationToChange);


            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);
            organizationToChangeDTO.CallerId = callerId;

            var updateOrganization = await CustomerServices.UpdateOrganizationAsync(organizationToChangeDTO);
            if (updateOrganization == null)
            {
                return BadRequest();
            }

            return Ok(updateOrganization);
        }

        [Route("userCount")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerUserCount>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<IList<CustomerUserCount>>> GetOrganizationUserCount()
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var filterOptions = new FilterOptionsForUser();
            if (role == PredefinedRole.PartnerAdmin.ToString())
            {
                var partnerIdFromAccessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (!string.IsNullOrEmpty(partnerIdFromAccessList) && Guid.TryParse(partnerIdFromAccessList, out var partnerId))
                {
                    filterOptions.PartnerId = partnerId;
                }
                else
                {
                    return Forbid();
                }
            }
            else if (role == PredefinedRole.CustomerAdmin.ToString() || role == PredefinedRole.Admin.ToString())
            {
                var customerIdFromAccessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (!string.IsNullOrEmpty(customerIdFromAccessList) && Guid.TryParse(customerIdFromAccessList, out var customerId))
                {
                    filterOptions.AssignedToDepartments = new List<Guid>();
                    filterOptions.AssignedToDepartments.Add(customerId);
                }
                else
                {
                    return Forbid();
                }
            }
            else if (role == PredefinedRole.SystemAdmin.ToString())
            {
                // No filter options needed.
            }
            else
            {
                return Forbid();
            }

            var organizationUserCounts = await CustomerServices.GetCustomerUsersAsync(filterOptions);
            return organizationUserCounts != null ? Ok(organizationUserCounts) : NotFound();
        }

        [HttpPatch]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> PatchOrganization([FromBody] UpdateOrganization organizationToChange)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationToChange.OrganizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var organizationToChangeDTO = Mapper.Map<UpdateOrganizationDTO>(organizationToChange);

            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);
            organizationToChangeDTO.CallerId = callerId;

            var updateOrganization = await CustomerServices.PatchOrganizationAsync(organizationToChangeDTO);
            if (updateOrganization == null)
            {
                return BadRequest();
            }

            return Ok(updateOrganization);
        }

        [Route("{organizationId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanDeleteCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Organization>> DeleteOrganization(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.PartnerAdmin.ToString() && role != PredefinedRole.SystemAdmin.ToString())
            {
                return Forbid();
            }
            // If role is not System admin, check access list
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);

            var deletedOrganization = await CustomerServices.DeleteOrganizationAsync(organizationId, callerId);
            if (deletedOrganization == null)
            {
                return NotFound();
            }

            return Ok(deletedOrganization);
        }

        [Route("{organizationId:Guid}/location")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<Location>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<Location>>> GetOrganizationLocations(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var locations = await CustomerServices.GetAllCustomerLocations(organizationId);
            return locations != null ? Ok(locations) : NotFound();
        }

        [Route("{organizationId:Guid}/location")]
        [HttpPost]
        [ProducesResponseType(typeof(Location), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<Location>> CreateLocation([FromBody] OfficeLocation newLocation, Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid.TryParse(actor, out Guid callerId);

            var createdLocation = await CustomerServices.CreateLocationAsync(newLocation, organizationId, callerId);
            if (createdLocation == null)
            {
                return BadRequest();
            }

            return CreatedAtAction(nameof(CreateLocation), createdLocation);
        }

        [Route("{organizationId:Guid}/location/{locationId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(IList<Location>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<Location>>> DeleteLocation(Guid organizationId, Guid locationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);

            var allLocations = await CustomerServices.DeleteLocationAsync(organizationId, locationId, callerId);
            return Ok(allLocations);
        }

        [Route("{organizationId:Guid}/initiate-onboarding")]
        [HttpPost]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.OnAndOffboarding)]

        public async Task<ActionResult<Organization>> InitiateOnboarding(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }

            var customer = await CustomerServices.InitiateOnbardingAsync(organizationId);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        [Route("webshopUrl")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult<string> GetCustomerWebshopUrl()
        {
            return Ok("https://www.google.com/");
        }

        /// <summary>
        /// Get all operators a customer
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/operators")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<OrigoOperator>), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> GetAllOperatorsForCustomer(Guid organizationId)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var customersOperators = await SubscriptionManagementService.GetAllOperatorsForCustomerAsync(organizationId);
            return Ok(customersOperators);
        }

        /// <summary>
        /// Create operator list for customer
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="operators">List of operator identifier</param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/operators")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> CreateOperatorListForCustomerAsync(Guid organizationId, [FromBody] List<int> operators)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);

            NewOperatorListDTO newOperatorListDTO = new NewOperatorListDTO();
            newOperatorListDTO.Operators = operators;
            newOperatorListDTO.CallerId = callerId;

            await SubscriptionManagementService.AddOperatorForCustomerAsync(organizationId, newOperatorListDTO);

            return NoContent();
        }

        /// <summary>
        /// Delete customer's operator
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="id">Operator identifier</param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/operators/{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanDeleteCustomer, Permission.SubscriptionManagement)]
        public async Task<ActionResult> DeleteFromCustomersOperatorList(Guid organizationId, int id)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }
            await SubscriptionManagementService.DeleteOperatorForCustomerAsync(organizationId, id);
            return NoContent();
        }

        [Route("{organizationId:Guid}/subscription-products")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoSubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult<IList<OrigoSubscriptionProduct>>> GetSubscriptionProductsForCustomer(Guid organizationId, [FromQuery] bool includeOperator = true)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var subscriptionProductList = await SubscriptionManagementService.GetAllSubscriptionProductForCustomerAsync(organizationId, includeOperator);
            return Ok(subscriptionProductList);
        }


        [Route("{organizationId:Guid}/subscription-products")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoSubscriptionProduct), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> CreateSubscriptionProductForCustomer(Guid organizationId, [FromBody] NewSubscriptionProduct newSubscriptionProduct)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var subscriptionProduct = await SubscriptionManagementService.AddSubscriptionProductForCustomerAsync(organizationId, newSubscriptionProduct);
            return CreatedAtAction(nameof(CreateSubscriptionProductForCustomer), subscriptionProduct);
        }

        [HttpPatch]
        [Route("{organizationId:Guid}/subscription-products/{subscriptionProductId}")]
        [ProducesResponseType(typeof(OrigoSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> UpdateOperatorSubscriptionProductForCustomer(Guid organizationId, int subscriptionProductId, [FromBody] UpdateSubscriptionProduct subscriptionProduct)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var updatedSubscriptionProducts = await SubscriptionManagementService.UpdateOperatorSubscriptionProductForCustomerAsync(organizationId, subscriptionProductId, subscriptionProduct);

            //return the updated subscription product
            return Ok(updatedSubscriptionProducts);
        }

        [Route("{organizationId:Guid}/subscription-products/{subscriptionProductId}")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> DeleteSubscriptionProductsForCustomer(Guid organizationId, int subscriptionProductId)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var subscriptionProductList = await SubscriptionManagementService.DeleteSubscriptionProductForCustomerAsync(organizationId, subscriptionProductId);
            return Ok(subscriptionProductList);
        }

        /// <summary>
        /// Get list of customer operator accounts
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <returns>list of customer operator accounts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoCustomerOperatorAccount>), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<IActionResult> GetAllOperatorAccountsForCustomer(Guid organizationId)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            var customerOperatorAccounts = await SubscriptionManagementService.GetAllOperatorAccountsForCustomerAsync(organizationId);

            return Ok(customerOperatorAccounts);
        }

        /// <summary>
        /// Setup customer account
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="customerOperatorAccount">Details of customer operator account</param>
        /// <returns>new customer operator account</returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrigoCustomerOperatorAccount), (int)HttpStatusCode.Created)]
        [Route("{organizationId:Guid}/operator-accounts")]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<IActionResult> AddOperatorAccountForCustomer(Guid organizationId, [FromBody] NewOperatorAccount customerOperatorAccount)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }
            var operatorAccount = await SubscriptionManagementService.AddOperatorAccountForCustomerAsync(organizationId, customerOperatorAccount);

            return CreatedAtAction(nameof(AddOperatorAccountForCustomer), operatorAccount);
        }

        /// <summary>
        /// Delete customer operator account
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="accountNumber">Account number</param>
        /// <param name="operatorId">Operator id</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoCustomerOperatorAccount), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<IActionResult> DeleteOperatorAccountForCustomer(Guid organizationId, [FromQuery] string accountNumber, [FromQuery] int operatorId)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }

            await SubscriptionManagementService.DeleteOperatorAccountForCustomerAsync(organizationId, accountNumber, operatorId);

            return Ok();
        }

        /// <summary>
        /// Creates a transfer subscription order
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{organizationId:Guid}/subscription-transfer-to-business")]
        [ProducesResponseType(typeof(OrigoTransferToBusinessSubscriptionOrder), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> TransferSubscriptionToBusiness(Guid organizationId, [FromBody] TransferToBusinessSubscriptionOrder order)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            Guid.TryParse(actor, out Guid callerId);
            var response = await SubscriptionManagementService.TransferToBusinessSubscriptionOrderForCustomerAsync(organizationId, order, callerId);
            return Ok(response);
        }

        [HttpPost]
        [Route("{organizationId:Guid}/subscription-transfer-to-private")]
        [ProducesResponseType(typeof(OrigoTransferToPrivateSubscriptionOrder), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> TransferSubscriptionToPrivate(Guid organizationId, [FromBody] TransferToPrivateSubscriptionOrder order)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            Guid.TryParse(actor, out Guid callerId);
            var response = await SubscriptionManagementService.TransferToPrivateSubscriptionOrderForCustomerAsync(organizationId, order, callerId);
            return Ok(response);
        }

        /// <summary>
        /// Order new sim
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrigoOrderSim), (int)HttpStatusCode.Created)]
        [Route("{organizationId:Guid}/order-sim")]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<IActionResult> OrderSim(Guid organizationId, [FromBody] OrderSim order)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            Guid.TryParse(actor, out var callerId);
            var response = await SubscriptionManagementService.OrderSimCardForCustomerAsync(organizationId, order, callerId);

            return CreatedAtAction(nameof(OrderSim), response);
        }

        /// <summary>
        /// Cancels a subscription
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{organizationId:Guid}/subscription-cancel")]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> CancelSubscription(Guid organizationId, [FromBody] CancelSubscriptionOrder order)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            Guid.TryParse(actor, out Guid callerId);
            var dto = Mapper.Map<CancelSubscriptionOrderDTO>(order);
            dto.CallerId = callerId;

            var response = await SubscriptionManagementService.CancelSubscriptionOrderForCustomerAsync(organizationId, dto);

            return CreatedAtAction(nameof(CancelSubscription), response);
        }

        /// <summary>
        /// Change subscription product.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{organizationId:Guid}/change-subscription")]
        [ProducesResponseType(typeof(OrigoChangeSubscriptionOrder), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> ChangeSubscriptionOrder(Guid organizationId,
            [FromBody] ChangeSubscriptionOrder order)
        {

            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }

            var requestModel = Mapper.Map<ChangeSubscriptionOrderPostRequest>(order);

            Guid.TryParse(actor, out var callerId);
            requestModel.CallerId = callerId;

            var changeSubscription =
                await SubscriptionManagementService.ChangeSubscriptionOrderAsync(organizationId, requestModel);

            return CreatedAtAction(nameof(ChangeSubscriptionOrder), changeSubscription);

        }
        /// <summary>
        /// Activate sim card
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{organizationId:Guid}/activate-sim")]
        [ProducesResponseType(typeof(OrigoActivateSimOrder), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> ActivateSimCard(Guid organizationId, [FromBody] NewActivateSimOrder order)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            Guid.TryParse(actor, out Guid callerId);

            var requestModel = Mapper.Map<ActivateSimOrderPostRequest>(order);
            requestModel.CallerId = callerId;

            var response = await SubscriptionManagementService.ActivateSimCardForCustomerAsync(organizationId, requestModel);
            return CreatedAtAction(nameof(ActivateSimCard), response);
        }
        /// <summary>
        /// Order new subscription
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{organizationId:Guid}/new-subscription")]
        [ProducesResponseType(typeof(OrigoNewSubscriptionOrder), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]

        public async Task<ActionResult> NewSubscriptionOrder(Guid organizationId, [FromBody] NewSubscriptionOrder order)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            Guid.TryParse(actor, out Guid callerId);

            var requestModel = Mapper.Map<NewSubscriptionOrderPostRequest>(order);
            requestModel.CallerId = callerId;

            var response = await SubscriptionManagementService.NewSubscriptionOrder(organizationId, requestModel);
            return CreatedAtAction(nameof(NewSubscriptionOrder), response);
        }

        [Obsolete]
        [HttpGet]
        [Route("{organizationId:Guid}/subscription-orders")]
        [ProducesResponseType(typeof(IList<OrigoSubscriptionOrderListItem>), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> GetSubscriptionOrders(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var response = await SubscriptionManagementService.GetSubscriptionOrders(organizationId);

            return Ok(response);
        }

        /// <summary>
        /// Gets a Paginated list of all subscription orders for a customer
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filterOptions"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{organizationId:Guid}/subscription-orders/pagination")]
        [ProducesResponseType(typeof(PagedModel<OrigoSubscriptionOrderListItem>), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> GetAllSubscriptionOrders([FromRoute]Guid organizationId, [FromQuery] FilterOptionsForSubscriptionOrder filterOptions, CancellationToken cancellationToken,
            [FromQuery(Name = "q")] string search = "", [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int limit = 25)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var response = await SubscriptionManagementService.GetAllSubscriptionOrders(organizationId, cancellationToken, filterOptions, search, page, limit);

            return Ok(response);
        }

        [Route("{organizationId:Guid}/subscription-orders/count")]
        [ProducesResponseType(typeof(OrigoSubscriptionOrdersCount), (int)HttpStatusCode.OK)]
        [HttpGet]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> GetSubscriptionOrdersCount(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var count = await SubscriptionManagementService.GetSubscriptionOrdersCount(organizationId);
            return Ok(new OrigoSubscriptionOrdersCount() { OrganizationId = organizationId, Count = count });
        }

        [Route("{organizationId:Guid}/subscription-orders-detail-view/{orderId:Guid}/{orderType:int}")]
        [ProducesResponseType(typeof(OrigoSubscriptionOrderDetailView), (int)HttpStatusCode.OK)]
        [HttpGet]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> GetSubscriptionOrderDetailView(Guid organizationId, Guid orderId, int orderType)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var response = await SubscriptionManagementService.GetSubscriptionOrderDetailViewAsync(organizationId, orderId, orderType);

            return Ok(response);
        }

        [Route("{organizationId:Guid}/standard-private-subscription-products")]
        [ProducesResponseType(typeof(IList<OrigoCustomerStandardPrivateSubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [HttpGet]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> GetCustomerStandardPrivateSubscriptionProduct(Guid organizationId)
        {

            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);

            var response = await SubscriptionManagementService.GetCustomerStandardPrivateSubscriptionProductAsync(organizationId);

            return Ok(response);
        }
        [HttpPost]
        [Route("{organizationId:Guid}/standard-private-subscription-products")]
        [ProducesResponseType(typeof(OrigoCustomerStandardPrivateSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> PostCustomerStandardPrivateSubscriptionProduct(Guid organizationId, [FromBody] NewStandardPrivateProduct standardPrivateProduct)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);

            var mapped = Mapper.Map<NewStandardPrivateProductDTO>(standardPrivateProduct);
            mapped.CallerId = callerId;


            var response = await SubscriptionManagementService.PostCustomerStandardPrivateSubscriptionProductAsync(organizationId, mapped);

            return Ok(response);
        }

        [HttpDelete]
        [Route("{organizationId:Guid}/standard-private-subscription-products/{operatorId:Int}")]
        [ProducesResponseType(typeof(OrigoCustomerStandardPrivateSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> DeleteCustomerStandardPrivateSubscriptionProduct(Guid organizationId, int operatorId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            Guid callerId;
            Guid.TryParse(actor, out callerId);

            var response = await SubscriptionManagementService.DeleteCustomerStandardPrivateSubscriptionProductAsync(organizationId, operatorId, callerId);

            return Ok(response);
        }

        /// <summary>
        /// Gets the configured standard business products.
        /// </summary>
        /// <param name="organizationId">Customer identifier.</param>
        /// <returns>A list of standard business subscription products that has been configured for given customer across each operators.</returns>
        [HttpGet]
        [Route("{organizationId:Guid}/standard-business-subscription-products")]
        [ProducesResponseType(typeof(IList<OrigoCustomerStandardBusinessSubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult<IList<OrigoCustomerStandardBusinessSubscriptionProduct>>> GetCustomerStandardBusinessSubscriptionProduct(Guid organizationId)
        {

            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }

            var response = await SubscriptionManagementService.GetCustomerStandardBusinessSubscriptionProductAsync(organizationId);

            return Ok(response);
        }

        /// <summary>
        /// Configures a standard business products for the specified operator.
        /// </summary>
        /// <param name="organizationId">Customer identifier.</param>
        /// <param name="standardBusinessProduct">New standard business subscription product to be configured for the customer.</param>
        /// <returns>The added standard business subscription products that has been configured for given customer.</returns>
        [HttpPost]
        [Route("{organizationId:Guid}/standard-business-subscription-products")]
        [ProducesResponseType(typeof(OrigoCustomerStandardBusinessSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult<OrigoCustomerStandardBusinessSubscriptionProduct>> AddStandardBusinessSubscriptionProducts(Guid organizationId, [FromBody] NewStandardBusinessSubscriptionProduct standardBusinessProduct)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }

            var response = await SubscriptionManagementService.PostCustomerStandardBusinessSubscriptionProductAsync(organizationId, standardBusinessProduct);

            return Ok(response);
        }

        /// <summary>
        /// Deletes the standard business products for the specified operator.
        /// </summary>
        /// <param name="organizationId">Customer identifier.</param>
        /// <param name="operatorId">Operator id of the standard business subscription product.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{organizationId:Guid}/standard-business-subscription-products/{operatorId:Int}")]
        [ProducesResponseType(typeof(OrigoCustomerStandardPrivateSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.SubscriptionManagement)]
        public async Task<ActionResult> DeleteStandardBusinessSubscriptionProducts(Guid organizationId, int operatorId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var response = await SubscriptionManagementService.DeleteCustomerStandardBusinessSubscriptionProductAsync(organizationId, operatorId);

            return Ok(response);
        }


        [Route("{organizationId:Guid}/assetsTotalBookValue")]
        [ProducesResponseType(typeof(CustomerAssetsTotalBookValue), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer)]
        [HttpGet]
        [PermissionAuthorize(Permission.AssetBookValue)]
        public async Task<ActionResult> GetAssetsTotalBookValue(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }

            var mockObject = new CustomerAssetsTotalBookValue { OrganizationId = organizationId, AssetsTotalBookValue = 12345 };
            return Ok(mockObject);
        }

        [Route("techstep-customer-search")]
        [ProducesResponseType(typeof(TechstepCustomers), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        [HttpGet]
        public async Task<ActionResult> GetTechstepCustomers([FromQuery] string searchString)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString() && role != PredefinedRole.PartnerAdmin.ToString())
            {
                return Forbid();
            }

            var techstepCustomers = await CustomerServices.GetTechstepCustomers(searchString);

            return Ok(techstepCustomers);
        }
    }
}