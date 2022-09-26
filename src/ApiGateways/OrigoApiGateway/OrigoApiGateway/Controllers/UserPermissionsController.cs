using AutoMapper;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/customers/users/{userName:length(6,255)}/permissions")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UserPermissionsController : ControllerBase
    {
        private readonly ILogger<UserPermissionsController> _logger;
        private readonly IUserPermissionService _userPermissionServices;
        private readonly IMapper _mapper;
        private readonly IUserServices _userServices;
        private readonly ICustomerServices _customerServices;

        public UserPermissionsController(ILogger<UserPermissionsController> logger, IUserPermissionService userPermissionServices, IMapper mapper, IUserServices userServices, ICustomerServices customerServices)
        {
            _logger = logger;
            _userPermissionServices = userPermissionServices;
            _mapper = mapper;
            _userServices = userServices;
            _customerServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoUserPermissions>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoUserPermissions>>> GetPermissions(string userName)
        {
            try
            {
                //User that is requested access to
                var user = await _userServices.GetUserInfo(userName, Guid.Empty);

                //User info about the user making the claim
                var email = HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                if (email == userName)
                {
                    // Validation ok
                } else if (Enum.TryParse<PredefinedRole>(role, out var parsedRole))
                {
                    switch (parsedRole)
                    {
                        case PredefinedRole.EndUser:
                            return Forbid();
                        case PredefinedRole.DepartmentManager:
                        case PredefinedRole.Manager:
                            if (user.DepartmentId == Guid.Empty) return Forbid();
                            if(!accessList.Any() || !accessList.Contains(user.DepartmentId.ToString())) return Forbid();
                            break;
                        case PredefinedRole.SystemAdmin:
                            break;
                        default:
                            if (!accessList.Any() || !accessList.Contains(user.OrganizationId.ToString())) return Forbid();
                            break;
                    }
                }
                else
                {
                    //could not parse the predefined role
                    return BadRequest($"Could not find a role for user making the claim");
                }

                var userPermissionsList = await _userPermissionServices.GetUserPermissionsAsync(userName);
                if (role == PredefinedRole.PartnerAdmin.ToString() && userPermissionsList.Any() && userPermissionsList.First().AccessList.Any())
                {
                    userPermissionsList.First().AccessList.RemoveAt(0); // First item is the partnerId for PartnerAdmins.
                }
                return Ok(userPermissionsList);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("/origoapi/v{version:apiVersion}/roles")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<string>>> GetRoles()
        {
            try
            {
                return Ok(await _userPermissionServices.GetAllRolesAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }
        [HttpGet]
        [Route("/origoapi/v{version:apiVersion}/admins")]
        [ProducesResponseType(typeof(IList<UserAdminDTO>), (int)HttpStatusCode.OK)]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin")]
        public async Task<ActionResult<IList<UserAdminDTO>>> GetAllUserAdmins()
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.PartnerAdmin.ToString())
                {
                    var firstItemInAccessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList");
                    if (firstItemInAccessList != null && Guid.TryParse(firstItemInAccessList.Value, out Guid partnerId))
                    {
                        return Ok(await _userPermissionServices.GetAllUserAdminsAsync(partnerId));
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                return Ok(await _userPermissionServices.GetAllUserAdminsAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Obsolete("Will be replaced by AddUsersPermission")]
        [HttpPut]
        [ProducesResponseType(typeof(OrigoUserPermissions), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUserPermissions>> AddUserPermission(string userName, [FromBody] NewUserPermissions userPermissions)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                {
                    return Forbid();
                }
                if(role != PredefinedRole.SystemAdmin.ToString())
                {
                    //User that is requested access to
                    var user = await _userServices.GetUserInfo(userName, Guid.Empty);
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;

                    if (accessList == null || !accessList.Any() || !accessList.Contains(user.OrganizationId.ToString())) return Forbid();
                   
                }

                var userPermissionsDTO = _mapper.Map<NewUserPermissionsDTO>(userPermissions);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                userPermissionsDTO.CallerId = callerId;

                var addedRole = await _userPermissionServices.AddUserPermissionsForUserAsync(userName, userPermissionsDTO);
                return CreatedAtAction(nameof(AddUserPermission), addedRole);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        /// <summary>
        /// Assigning roles and permissions for multiple users
        /// </summary>
        /// <param name="usersPermissions">List of user permissions to be added</param>
        /// <returns></returns>
        [Route("/origoapi/v{version:apiVersion}/customers/users/permissions")]
        [HttpPut]
        [ProducesResponseType(typeof(OrigoUsersPermissions), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUsersPermissions>> AddUsersPermissions([FromBody] NewUsersPermissions usersPermissions)
        {
            try
            {
                
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                    foreach (var permission in usersPermissions.UserPermissions)
                    {
                        var user = await _userServices.GetUserInfo(null, permission.UserId);
                        if (!accessList.Any() || !accessList.Contains(user.OrganizationId.ToString())) return Forbid();

                        if (role == PredefinedRole.Admin.ToString() || role == PredefinedRole.CustomerAdmin.ToString())
                        {
                            //Can only create Admin's and down
                            if (permission.Role != PredefinedRole.Admin.ToString() && 
                                permission.Role != PredefinedRole.CustomerAdmin.ToString() &&
                                permission.Role != PredefinedRole.Manager.ToString() &&
                                permission.Role != PredefinedRole.DepartmentManager.ToString() &&
                                permission.Role != PredefinedRole.EndUser.ToString()) return Forbid();

                        }
                        if (role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                        {
                            //Can only create Managers and down
                            if (permission.Role != PredefinedRole.Manager.ToString() &&
                               permission.Role != PredefinedRole.DepartmentManager.ToString() &&
                               permission.Role != PredefinedRole.EndUser.ToString()) return Forbid(); 

                            //Checks if the user is apart of the department for the manager
                            if (user?.DepartmentId == null || user.DepartmentId == Guid.Empty || !accessList.Contains(user.DepartmentId.ToString())) return Forbid();

                        }
                    }

                }

                var userPermissionsDTO = _mapper.Map<NewUsersPermissionsDTO>(usersPermissions);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                userPermissionsDTO.CallerId = callerId;

                var addedRoles = await _userPermissionServices.AddUsersPermissionsForUsersAsync(userPermissionsDTO);
                return CreatedAtAction(nameof(AddUsersPermissions), addedRoles);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(OrigoUserPermissions), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanDeleteCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUserPermissions>> RemoveUserPermission(string userName, [FromBody] NewUserPermissions userPermissions)
        {
            try
            {
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (role == PredefinedRole.EndUser.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    if (role == PredefinedRole.Admin.ToString() || role == PredefinedRole.CustomerAdmin.ToString())
                    {
                        //Can only remove Admin's and down
                        if (userPermissions.Role == PredefinedRole.GroupAdmin.ToString() ||
                            userPermissions.Role == PredefinedRole.PartnerAdmin.ToString() ||
                            userPermissions.Role == PredefinedRole.PartnerReadOnlyAdmin.ToString() ||
                            userPermissions.Role == PredefinedRole.SystemAdmin.ToString()) return Forbid();
                    }

                    //User that is requested access to
                    var user = await _userServices.GetUserInfo(userName, Guid.Empty);
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();

                    if (accessList == null || !accessList.Any() || !accessList.Contains(user.OrganizationId.ToString())) return Forbid();

                    if (role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                    {
                        //Can only create Managers and down
                        if (userPermissions.Role == PredefinedRole.GroupAdmin.ToString() ||
                           userPermissions.Role == PredefinedRole.PartnerAdmin.ToString() ||
                           userPermissions.Role == PredefinedRole.Admin.ToString() ||
                           userPermissions.Role == PredefinedRole.CustomerAdmin.ToString() ||
                           userPermissions.Role == PredefinedRole.PartnerReadOnlyAdmin.ToString() ||
                           userPermissions.Role == PredefinedRole.SystemAdmin.ToString()) return Forbid();

                        //Checks if the user is apart of the department for the manager
                        if (user?.DepartmentId == null || user.DepartmentId == Guid.Empty || !accessList.Contains(user.DepartmentId.ToString())) return Forbid();

                    }
                }

                var userPermissionsDTO = _mapper.Map<NewUserPermissionsDTO>(userPermissions);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                userPermissionsDTO.CallerId = callerId;

                var removedRole = await _userPermissionServices.RemoveUserPermissionsForUserAsync(userName, userPermissionsDTO);
                if (removedRole != null)
                {
                    return Ok(removedRole);
                }
                return NotFound();
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }
    }
}