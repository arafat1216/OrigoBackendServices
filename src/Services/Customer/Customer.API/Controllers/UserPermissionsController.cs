using Customer.API.ViewModels;
using CustomerServices;
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
                returnedUserPermissions.Add(new UserPermissions(permissionNames, userPermission.AccessList.ToList(), userPermission.Role.Name));
            }
            return Ok(returnedUserPermissions);
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ViewModels.UserPermissions>> AssignUserPermissions(string userName, [FromBody] NewUserPermission userRole)
        {
            var userPermission = await _userPermissionServices.AssignUserPermissionsAsync(userName, userRole.Role, userRole.AccessList);
            if (userPermission == null) return NotFound();

            var permissionNames = new List<string>();
            foreach (var roleGrantedPermission in userPermission.Role.GrantedPermissions)
            {
                permissionNames.AddRange(roleGrantedPermission.Permissions.Select(p => p.Name));
            }
            var userPermissionAdded = new UserPermissions(new ReadOnlyCollection<string>(permissionNames), new ReadOnlyCollection<Guid>(userPermission.AccessList), userPermission.Role.Name);
            return Ok(userPermissionAdded);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserPermissions>> RemoveUserPermissions(string userName, [FromBody] NewUserPermission userRole)
        {
            var userPermission = await _userPermissionServices.RemoveUserPermissionsAsync(userName, userRole.Role, userRole.AccessList);
            if (userPermission == null) return NotFound();

            var permissionNames = new List<string>();
            foreach (var roleGrantedPermission in userPermission.Role.GrantedPermissions)
            {
                permissionNames.AddRange(roleGrantedPermission.Permissions.Select(p => p.Name));
            }
            var userPermissionAdded = new UserPermissions(new ReadOnlyCollection<string>(permissionNames), new ReadOnlyCollection<Guid>(userPermission.AccessList), userPermission.Role.Name);
            return Ok(userPermissionAdded);
        }
    }
}