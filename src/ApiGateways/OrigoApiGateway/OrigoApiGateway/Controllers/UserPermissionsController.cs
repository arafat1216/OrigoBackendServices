using AutoMapper;
using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

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

        public UserPermissionsController(ILogger<UserPermissionsController> logger, IUserPermissionService customerServices, IMapper mapper, IUserServices userServices)
        {
            _logger = logger;
            _userPermissionServices = customerServices;
            _mapper = mapper;
            _userServices = userServices;
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
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;

                PredefinedRole tryParseResult;
                if (Enum.TryParse<PredefinedRole>(role, out tryParseResult))
                {
                    switch (tryParseResult)
                    {
                        case PredefinedRole.EndUser:
                            if(email != userName) return Forbid();
                            break;
                        case PredefinedRole.DepartmentManager:
                            if (user.DepartmentId == Guid.Empty) return Forbid();
                            else if(accessList == null || !accessList.Any() || !accessList.Contains(user.DepartmentId.ToString())) return Forbid();
                            break;
                        case PredefinedRole.Manager:
                            if (user.DepartmentId == Guid.Empty) return Forbid();
                            else if (accessList == null || !accessList.Any() || !accessList.Contains(user.DepartmentId.ToString())) return Forbid();
                            break;
                        case PredefinedRole.SystemAdmin:
                            break;
                        default:
                            if (accessList == null || !accessList.Any() || !accessList.Contains(user.OrganizationId.ToString())) return Forbid();
                            break;
                    }
                }
                else
                {
                    //could not parse the predefined role
                    return BadRequest($"Could not find a role for user making the claim");
                }

                var userRole = await _userPermissionServices.GetUserPermissionsAsync(userName);
                if (userRole == null)
                {
                    return NotFound();
                }

                return Ok(userRole);
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
        [Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<IList<UserAdminDTO>>> GetAllUserAdmins()
        {
            try
            {
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

                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    //User that is requested access to
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;

                    foreach (var permission in usersPermissions.UserPermissions)
                    {
                        var user = await _userServices.GetUserInfo(null, permission.UserId);
                        if (accessList == null || !accessList.Any() || !accessList.Contains(user.OrganizationId.ToString())) return Forbid();
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

                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
                {
                    return Forbid();
                }

                if (role != PredefinedRole.SystemAdmin.ToString())
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