using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using CustomerServices.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Customer.API.Controllers;

/// <summary>
///     Handles permission settings for a user
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organizations/users/{userName:length(6,255)}/permissions")]
public class UserPermissionsController : ControllerBase
{
    private readonly IUserPermissionServices _userPermissionServices;
    private readonly ILogger<UserPermissionsController> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    ///     Needs access to user services and organization services.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="userServices"></param>
    /// <param name="mapper"></param>
    public UserPermissionsController(ILogger<UserPermissionsController> logger, IUserPermissionServices userServices,
        IMapper mapper)
    {
        _logger = logger;
        _userPermissionServices = userServices;
        _mapper = mapper;
    }

    /// <summary>
    ///     Retrieves the permissions given to this user also checking if this is an invited user which will then initiate
    ///     onboarding.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<UserPermissions>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<List<UserPermissions>>> GetUserPermissions(string userName)
    {
        var userPermissions = await _userPermissionServices.GetUserPermissionsAsync(userName);
        if (userPermissions == null) return NotFound();

        return Ok(_mapper.Map<List<UserPermissions>>(userPermissions));
    }

    /// <summary>
    /// Returns all system and partner admin users.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/api/v{version:apiVersion}/organizations/admins")]
    [ProducesResponseType(typeof(List<UserAdmin>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<List<UserAdmin>>> GetUserAdmins([FromQuery] Guid? partnerId = null)
    {
        var userPermissions = await _userPermissionServices.GetUserAdminsAsync(partnerId);
        if (!userPermissions.Any()) return NotFound();
        var returnedUser = userPermissions.Select(userPermission => new UserAdmin(userPermission.User.UserId,
            userPermission.User.FirstName, userPermission.User.LastName, userPermission.User.Email,
            userPermission.User.MobileNumber, userPermission.Role.Name, userPermission.AccessList.ToList())).ToList();

        return Ok(returnedUser);
    }

    /// <summary>
    /// Returns all customer admins.
    /// </summary>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/api/v{version:apiVersion}/organizations/{organizationId:guid}/customer-admins")]
    [ProducesResponseType(typeof(List<UserAdmin>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<List<UserAdmin>>> GetCustomerAdmins(Guid organizationId)
    {
        var userPermissions = await _userPermissionServices.GetCustomerAdminsAsync(organizationId);
        if (userPermissions == null) return NotFound();
        var returnedUser = userPermissions.Select(userPermission => new UserAdmin(userPermission.User.UserId,
            userPermission.User.FirstName, userPermission.User.LastName, userPermission.User.Email,
            userPermission.User.MobileNumber, userPermission.Role.Name, userPermission.AccessList.ToList())).ToList();

        return Ok(returnedUser);
    }

    /// <summary>
    /// Returns all roles in the system.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/api/v{version:apiVersion}/organizations/roles")]
    [ProducesResponseType(typeof(List<UserPermissions>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<string>>> GetAllRoles()
    {
        var allRoles = await _userPermissionServices.GetAllRolesAsync();
        return Ok(allRoles);
    }

    /// <summary>
    /// Assign user permission for one user.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="userRole"></param>
    /// <returns></returns>
    [Obsolete("Will be replaced with AssignUsersPermissions")]
    [HttpPut]
    [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UserPermissions>> AssignUserPermissions(string userName,
        [FromBody] NewUserPermission userRole)
    {
        try
        {
            var userPermission = await _userPermissionServices.AssignUserPermissionsAsync(userName, userRole.Role,
                userRole.AccessList, userRole.CallerId);
            if (userPermission == null) return NotFound();
            return Ok(userPermission);
        }
        catch (UserNameDoesNotExistException userEx)
        {
            _logger.LogError("{0}", userEx);
            return NotFound($"User with user name: {userName}. Not found.");
        }
        catch (InvalidRoleNameException ex)
        {
            _logger.LogError("{0}", ex);
            return NotFound($"Role {userRole.Role} not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError("{0}", ex);
            throw;
        }
    }

    /// <summary>
    ///     Assigning users permissions and roles
    /// </summary>
    /// <param name="newUserPermissions">List of users to get role and permissions</param>
    /// <returns></returns>
    [Route("/api/v{version:apiVersion}/organizations/users/permissions")]
    [HttpPut]
    [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UsersPermissions>> AssignUsersPermissions(
        [FromBody] NewUsersPermission newUserPermissions)
    {
        try
        {
            var userPermission =
                await _userPermissionServices.AssignUsersPermissionsAsync(newUserPermissions,
                    newUserPermissions.CallerId);
            if (userPermission.UserPermissions.Count == 0) return NotFound();

            return Ok(_mapper.Map<UsersPermissions>(userPermission));
        }
        catch (DuplicateException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError("{0}", ex);
            throw;
        }
    }

    /// <summary>
    /// Removes parts of a permission to a user or the complete permission given.
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="userRole"></param>
    /// <returns></returns>
    [HttpDelete]
    [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UserPermissions>> RemoveUserPermissions(string userName,
        [FromBody] NewUserPermission userRole)
    {
        try
        {
            var userPermission = await _userPermissionServices.RemoveUserPermissionsAsync(userName, userRole.Role,
                userRole.AccessList, userRole.CallerId);
            if (userPermission == null) return NotFound();

            var permissionNames = new List<string>();

            foreach (var roleGrantedPermission in userPermission.Role.GrantedPermissions)
            {
                permissionNames.AddRange(roleGrantedPermission.Permissions.Select(p => p.Name));
            }

            var userPermissionRemoved = new UserPermissions(new ReadOnlyCollection<string>(permissionNames),
                new ReadOnlyCollection<Guid>(userPermission.AccessList), userPermission.Role.Name,
                userPermission.User.UserId, Guid.Empty);
            return Ok(userPermissionRemoved);
        }
        catch (UserNameDoesNotExistException userEx)
        {
            _logger.LogError("{0}", userEx);
            return NotFound($"User with user name: {userName}. Not found.");
        }
        catch (InvalidRoleNameException ex)
        {
            _logger.LogError("{0}", ex);
            return NotFound($"Role {userRole.Role} not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError("{0}", ex);
            throw;
        }
    }
}