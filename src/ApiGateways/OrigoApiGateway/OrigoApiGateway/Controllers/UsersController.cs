using AutoMapper;
using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Exceptions;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("origoapi/v{version:apiVersion}/Customers/{organizationId:guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserServices _userServices;
        private readonly IAssetServices _assetServices;
        private readonly ICustomerServices _customerServices;
        private readonly ISubscriptionManagementService _subscriptionManagementService;
        private readonly IProductCatalogServices _productCatalogServices;
        private readonly IMapper _mapper;

        public UsersController(ILogger<UsersController> logger,
            IUserServices userServices,
            IAssetServices assetServices,
            ICustomerServices customerServices,
            IProductCatalogServices productCatalogServices,
            ISubscriptionManagementService subscriptionManagementService,
            IMapper mapper)
        {
            _logger = logger;
            _userServices = userServices;
            _assetServices = assetServices;
            _customerServices = customerServices;
            _productCatalogServices = productCatalogServices;
            _subscriptionManagementService = subscriptionManagementService;
            _mapper = mapper;
        }

        [Route("count")]
        [HttpGet]
        [ProducesResponseType(typeof(CustomerUserCount), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.OnAndOffboarding)]
        public async Task<ActionResult<CustomerUserCount>> GetUsersCount(Guid organizationId)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                FilterOptionsForUser filterOptions = new FilterOptionsForUser { Roles = new string[] { role ?? null } };

                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    // Check if caller has access to this organization
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }

                    if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                    {
                        filterOptions.AssignedToDepartments ??= new List<Guid>();

                        foreach (var departmentId in accessList)
                        {
                            if (Guid.TryParse(departmentId, out var departmentGuid))
                            {
                                filterOptions.AssignedToDepartments.Add(departmentGuid);

                            }
                        }
                    }
                }

                var count = await _userServices.GetUsersCountAsync(organizationId, filterOptions);
                return Ok(count);

            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrigoUser>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<PagedModel<OrigoUser>>> GetAllUsers(Guid organizationId, [FromQuery] FilterOptionsForUser filterOptions, CancellationToken cancellationToken, [FromQuery(Name = "q")] string search = "", [FromQuery] int page = 1, [FromQuery][Range(1, 100)] int limit = 25)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role == PredefinedRole.EndUser.ToString())
            {
                return Forbid();
            }

            var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                // Check if caller has access to this organization
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }

                if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                {
                    filterOptions.AssignedToDepartments ??= new List<Guid>();
                    foreach (var departmentId in accessList)

                    {
                        if (Guid.TryParse(departmentId, out var departmentGuid))
                        {
                            filterOptions.AssignedToDepartments.Add(departmentGuid);

                        }
                    }
                }

            }
            var users = await _userServices.GetAllUsersAsync(organizationId, filterOptions, cancellationToken, search, page, limit);
            return Ok(users);
        }

        /// <summary>
        /// Get user's preference of onboarding tiles with additional condition check
        /// </summary>
        /// <param name="organizationId">Organization id.</param>
        /// <returns>Onboardin tiles preferences</returns>
        [Route("onboarding-tiles-preferences")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.OnAndOffboarding)]
        public async Task<ActionResult<OnboardingTilesPreference>> GetOnBoardingTilesPreferences(Guid organizationId)
        {

            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (!Guid.TryParse(actor, out var userId))
            {
                return NotFound();
            }
            var user = await _userServices.GetUserAsync(organizationId, userId);

            var tileData = new OnboardingTilesPreference()
            {
                ShowAssetTile = !user.UserPreference.IsAssetTileClosed.HasValue || !user.UserPreference.IsAssetTileClosed.Value,
                ShowSubscriptionTile = !user.UserPreference.IsSubscriptionTileClosed.HasValue || !user.UserPreference.IsSubscriptionTileClosed.Value
            };

            var updatePreference = new UserPreference();

            // User did not close the Asset Tiles. Check if User got assigned any new asset
            if (tileData.ShowAssetTile)
            {
                var asset = await _assetServices.GetAssetLifecycleCountersAsync(organizationId, new FilterOptionsForAsset()
                {
                    UserId = userId.ToString()
                });

                // If Assets found close the tiles and hide it
                if (asset.UsersPersonalAssets > 0)
                {
                    tileData.ShowAssetTile = false;
                    updatePreference.IsAssetTileClosed = true;
                }
            }

            // User did not close the Subscription Tiles. Check if User has got any Sub Orders
            if (tileData.ShowSubscriptionTile)
            {
                var subOrders = await _subscriptionManagementService.GetSubscriptionOrdersCount(organizationId, null, user.MobileNumber, true);

                // If Subs Order found close the tiles and hide it
                if (subOrders > 0)
                {
                    tileData.ShowSubscriptionTile = false;
                    updatePreference.IsSubscriptionTileClosed = true;
                }
            }

            // Update userPreferences if the condition checked and changed the Tiles preferences
            if(user.UserPreference.IsAssetTileClosed != updatePreference.IsAssetTileClosed || user.UserPreference.IsSubscriptionTileClosed != updatePreference.IsSubscriptionTileClosed)
            {
                await _userServices.PatchUserAsync(organizationId, userId, new OrigoUpdateUser() { UserPreference = updatePreference }, userId);
            }

            return Ok(tileData);
        }

        [Route("{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<OrigoUser>> GetUser(Guid organizationId, Guid userId)
        {

            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                // Check if caller has access to this organization
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var user = await _userServices.GetUserAsync(organizationId, userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Route("me")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<OrigoUser>> GetLoggedInUser(Guid organizationId)
        {
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            if (!Guid.TryParse(actor, out var userId))
            {
                return NotFound();
            }
            var user = await _userServices.GetUserAsync(organizationId, userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Route("/origoapi/v{version:apiVersion}/me")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoMeUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<OrigoMeUser>> GetLoggedInUser([FromQuery] Guid? organizationId)
        {
            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            if (!Guid.TryParse(actor, out var userId))
            {
                return NotFound();
            }
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
            if (organizationId != null && role != PredefinedRole.SystemAdmin.ToString())
            {
                // Check if caller has access to this organization
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.Value.ToString()))
                {
                    return Forbid();
                }
            }
            var permissions = HttpContext.User.Claims.Where(c => c.Type == "Permissions").Select(c => c.Value).ToList();
            var mainOrganization = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "MainOrganization")?.Value;
            if (!string.IsNullOrEmpty(mainOrganization) && Guid.TryParse(mainOrganization, out Guid mainOrganizationId))
            {
                var user = await _userServices.GetUserWithPermissionsAsync(organizationId, mainOrganizationId, userId, permissions, accessList);
                return user == null ? (ActionResult<OrigoMeUser>)NotFound() : (ActionResult<OrigoMeUser>)Ok(user);

            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUser>> CreateUserForCustomer(Guid organizationId, [FromBody] NewUser newUser)
        {
            try
            {
                // Check if caller has access to this organization
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

                //Mapping the frontend model to backend dto and assigning a caller id
                var newUserDTO = _mapper.Map<NewUserDTO>(newUser);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                _ = Guid.TryParse(actor, out Guid callerId);

                //The customer that has Transactional and the feature OnAndOffboarding handels the creation of user in a different way then the customers with Implement 
                var permissions = HttpContext.User.Claims.Where(c => c.Type == "Permissions").Select(c => c.Value).ToList();
                var includeOnboarding = false;

                if (permissions.Contains(Permission.OnAndOffboarding.ToString()))
                {
                   includeOnboarding = true;
                }

                var updatedUser = await _userServices.AddUserForCustomerAsync(organizationId, newUser, callerId, includeOnboarding);

                return CreatedAtAction(nameof(CreateUserForCustomer), new { id = updatedUser.Id }, updatedUser);
            }
            catch (OktaException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidUserValueException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/activate")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUser>> SetUserActiveStatus(Guid organizationId, Guid userId, bool isActive)
        {
            try
            {
                // Check if caller has access to this organization
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
                _ = Guid.TryParse(actor, out Guid callerId);

                var user = await _userServices.SetUserActiveStatusAsync(organizationId, userId, isActive, callerId);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("{userId:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUser>> PutUserForCustomer(Guid organizationId, Guid userId, [FromBody] OrigoUpdateUser updateUser)
        {
            try
            {
                // Check if caller has access to this organization
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

                //Mapping the frontend model to backend dto and assigning a caller id
                var updateUserDTO = _mapper.Map<UpdateUserDTO>(updateUser);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                _ = Guid.TryParse(actor, out Guid callerId);

                var updatedUser = await _userServices.PutUserAsync(organizationId, userId, updateUser, callerId);
                if (updatedUser == null)
                    return NotFound();

                return Ok(updatedUser);
            }
            catch (InvalidUserValueException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [Route("{userId:Guid}")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUser>> PatchUserForCustomer(Guid organizationId, Guid userId, [FromBody] OrigoUpdateUser updateUser)
        {
            try
            {
                // Check if caller has access to this organization
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                _ = Guid.TryParse(actor, out Guid callerId);

                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                {
                    if (callerId == userId)
                    {
                        updateUser = new OrigoUpdateUser()
                        {
                            UserPreference = updateUser.UserPreference
                        };
                    }
                    else
                    {
                        return Forbid();
                    }
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var updatedUser = await _userServices.PatchUserAsync(organizationId, userId, updateUser, callerId);
                if (updatedUser == null)
                    return NotFound();

                return Ok(updatedUser);
            }
            catch (InvalidUserValueException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [Route("{userId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<bool>> DeleteUserForCustomer(Guid organizationId, Guid userId, bool softDelete = true)
        {
            try
            {
                // Check if caller has access to this organization
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
                _ = Guid.TryParse(actor, out Guid callerId);

                var deletedUser = await _userServices.DeleteUserAsync(organizationId, userId, softDelete, callerId);

                return Ok(deletedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult<OrigoUser>> AssignUserToDepartment(Guid organizationId, Guid userId, Guid departmentId)
        {
            try
            {
                // Check if caller has access to this organization
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
                _ = Guid.TryParse(actor, out Guid callerId);

                var updatedUser = await _userServices.AssignUserToDepartment(organizationId, userId, departmentId, callerId);

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult<OrigoUser>> UnassignUserFromDepartment(Guid organizationId, Guid userId, Guid departmentId)
        {
            try
            {
                // Check if caller has access to this organization
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
                _ = Guid.TryParse(actor, out Guid callerId);

                var updatedUser = await _userServices.UnassignUserFromDepartment(organizationId, userId, departmentId, callerId);

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}/manager")]
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult> AssignManagerToDepartment(Guid organizationId, Guid userId, Guid departmentId)
        {
            try
            {
                // Check if caller has access to this organization
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
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                _ = Guid.TryParse(actor, out Guid callerId);

                await _userServices.AssignManagerToDepartment(organizationId, userId, departmentId, callerId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}/manager")]
        [HttpDelete]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer, Permission.DepartmentStructure)]
        public async Task<ActionResult> UnassignManagerFromDepartment(Guid organizationId, Guid userId, Guid departmentId)
        {
            try
            {
                // Check if caller has access to this organization
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
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                _ = Guid.TryParse(actor, out Guid callerId);

                await _userServices.UnassignManagerFromDepartment(organizationId, userId, departmentId, callerId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("{userId:Guid}/initiate-offboarding")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(PermissionOperator.And,Permission.CanReadCustomer, Permission.OnAndOffboarding)]
        public async Task<ActionResult<OrigoUser>> InitiateOffboarding(Guid organizationId, Guid userId, [FromBody] OffboardInitiate postData)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString())
                return Forbid();

            var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
            var department = new List<Guid>();

            if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
            {
                foreach (var departmentId in accessList)
                {
                    if (Guid.TryParse(departmentId, out var departmentGuid))
                        department.Add(departmentGuid);
                }
            }
            else if (role != PredefinedRole.SystemAdmin.ToString())
            {
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    return Forbid();
            }

            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            _ = Guid.TryParse(actor, out Guid callerId);

            var currency = await _customerServices.GetCurrencyByCustomer(organizationId);
            var lifeCycleSetting = await _assetServices.GetLifeCycleSettingByCustomer(organizationId, currency);
            var user = await _userServices.InitiateOffboarding(organizationId, userId, role, department, postData, lifeCycleSetting, callerId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Route("{userId:Guid}/cancel-offboarding")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.OnAndOffboarding)]
        public async Task<ActionResult<OrigoUser>> CancelOffboarding(Guid organizationId, Guid userId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString())
                return Forbid();

            var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
            var department = new List<Guid>();

            if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
            {
                foreach (var departmentId in accessList)
                {
                    if (Guid.TryParse(departmentId, out var departmentGuid))
                        department.Add(departmentGuid);
                }
            }
            else if (role != PredefinedRole.SystemAdmin.ToString())
            {
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    return Forbid();
            }

            var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
            _ = Guid.TryParse(actor, out Guid callerId);

            var user = await _userServices.CancelOffboarding(organizationId, userId, role, department, callerId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Resend a Origo invitation to multiple users.
        /// </summary>
        /// <param name="organizationId">The organization id.</param>
        /// <param name="users">A list of user ids that should get resent the Origo invitation mail.</param>
        /// <returns>A list of exception messages, if there are any exceptions.</returns>
        [Route("re-send-invitation")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoExceptionMessages), (int)HttpStatusCode.OK)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.OnAndOffboarding)]

        public async Task<ActionResult<OrigoExceptionMessages>> ResendOrigoInvitationMail(Guid organizationId, [FromBody] InviteUsers users)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                FilterOptionsForUser filterOptions = new FilterOptionsForUser { Roles = new string[] { role ?? null } };

                if (role == PredefinedRole.EndUser.ToString())
                    return Forbid();

                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    // Check if caller has access to this organization
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }

                    if ((role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString()) && accessList != null)
                    {
                        filterOptions.AssignedToDepartments ??= new List<Guid>();
                        foreach (var departmentId in accessList)

                        {
                            if (Guid.TryParse(departmentId, out var departmentGuid))
                            {
                                filterOptions.AssignedToDepartments.Add(departmentGuid);

                            }
                        }
                    }

                }

                var exceptionMessages = await _userServices.ResendOrigoInvitationMail(organizationId, users, filterOptions);
                return Ok(exceptionMessages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }

       /// <summary>
       /// Changes user status of the caller to Activated, after onboarding is completed.
       /// </summary>
       /// <param name="organizationId">Organization id.</param>
       /// <returns>User objekt that is made activated. Or exception when required comditions is not met.</returns>
        [Route("onboarding-completed")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.OnAndOffboarding)]

        public async Task<ActionResult<OrigoUser>> CompleteOnboarding(Guid organizationId)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    // Check if caller has access to this organization
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }

                }

                //This is for when user him/her self has completed the onboarding process - find the user id from the token
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                _ = Guid.TryParse(actor, out Guid callerId);

                var user = await _userServices.CompleteOnboardingAsync(organizationId, callerId);

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}