using AutoMapper;
using Common.Enums;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using CustomerServices.ServiceModels;
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
        private readonly IMapper _mapper;

        public UserPermissionsController(ILogger<UserPermissionsController> logger, IUserPermissionServices userServices, IMapper mapper)
        {
            _logger = logger;
            _userPermissionServices = userServices;
            _mapper = mapper;
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
        [Route("/api/v{version:apiVersion}/organizations/admins")]
        [ProducesResponseType(typeof(List<UserAdmin>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<UserAdmin>>> GetUserAdmins()
        {
            var userPermissions = await _userPermissionServices.GetUserAdminsAsync();
            if (userPermissions == null) return NotFound();
            var returnedUser = new List<UserAdmin>();
            foreach (var userPermission in userPermissions)
            {
                returnedUser.Add(new UserAdmin(userPermission.User.UserId,
                    userPermission.User.FirstName, userPermission.User.LastName, userPermission.User.Email,
                    userPermission.User.MobileNumber, userPermission.Role.Name,
                    userPermission.Role.Name == PredefinedRole.PartnerAdmin.ToString() ?
                    userPermission.AccessList.ToList() : null));
            }
            return Ok(returnedUser);
        }

        [HttpGet]
        [Route("/api/v{version:apiVersion}/organizations/roles")]
        [ProducesResponseType(typeof(List<UserPermissions>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<string>>> GetAllRoles()
        {
            var allRoles = await _userPermissionServices.GetAllRolesAsync();
            return Ok(allRoles);
        }
        [Obsolete("Will be replaced with AssignUsersPermissions")]
        [HttpPut]
        [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ViewModels.UserPermissions>> AssignUserPermissions(string userName, [FromBody] NewUserPermission userRole)
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
        /// Assignig users permissions and roles
        /// </summary>
        /// <param name="newUserPermissions">List of users to get role and permissions</param>
        /// <returns></returns>
        [Route("/api/v{version:apiVersion}/organizations/users/permissions")]
        [HttpPut]
        [ProducesResponseType(typeof(UserPermissions), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]

        public async Task<ActionResult<ViewModels.UsersPermissions>> AssignUsersPermissions([FromBody] NewUsersPermission newUserPermissions)
        {
            try
            {
                var userPermission = await _userPermissionServices.AssignUsersPermissionsAsync(newUserPermissions, newUserPermissions.CallerId);
                if(userPermission.UserPermissions.Count == 0) return NotFound();
                
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
}