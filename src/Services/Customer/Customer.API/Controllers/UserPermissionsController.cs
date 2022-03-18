using Customer.API.ApiModels;
using CustomerServices;
using CustomerServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizations/users/{userName:length(6,255)}/permissions")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UserPermissionsController : ControllerBase
    {
        private readonly IUserPermissionServices _userPermissionServices;
        private readonly ILogger<UserPermissionsController> _logger;

        public UserPermissionsController(ILogger<UserPermissionsController> logger, IUserPermissionServices userServices)
        {
            _logger = logger;
            _userPermissionServices = userServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UserPermissions>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<UserPermissions>>> GetUserPermissions(string userName)
        {
            var userPermissions = await _userPermissionServices.GetUserPermissionsAsync(userName);
            if (userPermissions == null) return NotFound();
            var returnedUserPermissions = new List<UserPermissions>();
            foreach (var userPermission in userPermissions)
            {
                var permissionNames = new List<string>();
                foreach (var roleGrantedPermission in userPermission.Role.GrantedPermissions)
                {
                    permissionNames.AddRange(roleGrantedPermission.Permissions.Select(p => p.Name));
                }
                returnedUserPermissions.Add(new UserPermissions(permissionNames, userPermission.AccessList.ToList(), userPermission.Role.Name, userPermission.User.UserId));
            }
            return Ok(returnedUserPermissions);
        }

        [HttpGet]
        [Route("/api/v{version:apiVersion}/organizations/roles")]
        [ProducesResponseType(typeof(List<UserPermissions>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<string>>> GetAllRoles()
        {
            var allRoles = await _userPermissionServices.GetAllRolesAsync();
            return Ok(allRoles);
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ApiModels.UserPermissions>> AssignUserPermissions(string userName, [FromBody] NewUserPermission userRole)
        {
            try
            {
                var userPermission = await _userPermissionServices.AssignUserPermissionsAsync(userName, userRole.Role, userRole.AccessList,userRole.CallerId);
                if (userPermission == null) return NotFound();

                var permissionNames = new List<string>();
                foreach (var roleGrantedPermission in userPermission.Role.GrantedPermissions)
                {
                    permissionNames.AddRange(roleGrantedPermission.Permissions.Select(p => p.Name));
                }
                var userPermissionAdded = new UserPermissions(new ReadOnlyCollection<string>(permissionNames), new ReadOnlyCollection<Guid>(userPermission.AccessList), userPermission.Role.Name, userPermission.User.UserId);
                return Ok(userPermissionAdded);
            }
            catch (UserNameDoesNotExistException userEx)
            {
                _logger.LogError("{0}", userEx);
                return NotFound($"User with user name: {userName}. Not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex);
                throw;
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserPermissions>> RemoveUserPermissions(string userName, [FromBody] NewUserPermission userRole)
        {
            try
            {
                var userPermission = await _userPermissionServices.RemoveUserPermissionsAsync(userName, userRole.Role, userRole.AccessList,userRole.CallerId);
                if (userPermission == null) return NotFound();

                var permissionNames = new List<string>();
                foreach (var roleGrantedPermission in userPermission.Role.GrantedPermissions)
                {
                    permissionNames.AddRange(roleGrantedPermission.Permissions.Select(p => p.Name));
                }
                var userPermissionAdded = new UserPermissions(new ReadOnlyCollection<string>(permissionNames), new ReadOnlyCollection<Guid>(userPermission.AccessList), userPermission.Role.Name, userPermission.User.UserId);
                return Ok(userPermissionAdded);
            }
            catch (UserNameDoesNotExistException userEx)
            {
                _logger.LogError("{0}", userEx);
                return NotFound($"User with user name: {userName}. Not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex);
                throw;
            }
        }
    }
}