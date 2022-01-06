using Common.Enums;
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
    [ApiVersion("1.0")]
    //[Authorize]
    [Route("origoapi/v{version:apiVersion}/Customers/{organizationId:guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserServices _userServices;

        public UsersController(ILogger<UsersController> logger, IUserServices userServices)
        {
            _logger = logger;
            _userServices = userServices;
        }

        [Route("count")]
        [HttpGet]
        [ProducesResponseType(typeof(List<OrigoUser>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<int>> GetUsersCount(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                // Check if caller has access to this organization
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }
            var count = await _userServices.GetUsersCountAsync(organizationId);
            return Ok(new { organizationId, count });
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrigoUser>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<List<OrigoUser>>> GetAllUsers(Guid organizationId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                // Check if caller has access to this organization
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }

            var users = await _userServices.GetAllUsersAsync(organizationId);
            if (users == null) return NotFound();
            return Ok(users);
        }

        [Route("{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        public async Task<ActionResult<OrigoUser>> GetUser(Guid organizationId, Guid userId)
        {

            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                // Check if caller has access to this organization
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                {
                    return Forbid();
                }
            }

            var user = await _userServices.GetUserAsync(organizationId, userId);
            if (user == null) return NotFound();
            return Ok(user);
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
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var newUserDTO = new NewUserDTO();
                newUserDTO.EmployeeId = newUser.EmployeeId;
                newUserDTO.Email = newUser.Email;
                newUserDTO.LastName = newUser.LastName;
                newUserDTO.FirstName = newUser.FirstName;
                newUserDTO.MobileNumber = newUser.MobileNumber;
                newUserDTO.UserPreference = newUser.UserPreference;
                newUserDTO.CallerId = callerId;
                newUserDTO.Role = newUser.Role;


                var updatedUser = await _userServices.AddUserForCustomerAsync(organizationId, newUserDTO);

                return CreatedAtAction(nameof(CreateUserForCustomer), new { id = updatedUser.Id }, updatedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var user = await _userServices.SetUserActiveStatusAsync(organizationId, userId, isActive,callerId);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
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
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var updateUserDTO = new UpdateUserDTO();
                updateUserDTO.EmployeeId = updateUser.EmployeeId;
                updateUserDTO.Email = updateUser.Email;
                updateUserDTO.LastName = updateUser.LastName;
                updateUserDTO.FirstName = updateUser.FirstName;
                updateUserDTO.UserPreference = updateUser.UserPreference;
                updateUserDTO.CallerId = callerId;


                var updatedUser = await _userServices.PutUserAsync(organizationId, userId, updateUserDTO);
                if (updatedUser == null)
                    return NotFound();

                return Ok(updatedUser);
            }
            catch
            {
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
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var updateUserDTO = new UpdateUserDTO();
                updateUserDTO.EmployeeId = updateUser.EmployeeId;
                updateUserDTO.Email = updateUser.Email;
                updateUserDTO.LastName = updateUser.LastName;
                updateUserDTO.FirstName = updateUser.FirstName;
                updateUserDTO.UserPreference = updateUser.UserPreference;
                updateUserDTO.CallerId = callerId;


                var updatedUser = await _userServices.PatchUserAsync(organizationId, userId, updateUserDTO);
                if (updatedUser == null)
                    return NotFound();

                return Ok(updatedUser);
            }
            catch
            {
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
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var deletedUser = await _userServices.DeleteUserAsync(organizationId, userId, softDelete, callerId);
                if (!deletedUser)
                    return NotFound();

                return NoContent();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUser>> AssignDepartmentForCustomer(Guid organizationId, Guid userId, Guid departmentId)
        {
            try
            {
                // Check if caller has access to this organization
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var updatedUser = await _userServices.AssignUserToDepartment(organizationId, userId, departmentId, callerId);

                return Ok(updatedUser);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<OrigoUser>> RemoveAssignedDepartmentForCustomer(Guid organizationId, Guid userId, Guid departmentId)
        {
            try
            {
                // Check if caller has access to this organization
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString())
                {
                    return Forbid();
                }
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                var updatedUser = await _userServices.UnassignUserFromDepartment(organizationId, userId, departmentId, callerId);

                return Ok(updatedUser);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}/manager")]
        [HttpPost]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                await _userServices.AssignManagerToDepartment(organizationId, userId, departmentId,callerId);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}/manager")]
        [HttpDelete]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
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
                    var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                    if (accessList == null || !accessList.Any() || !accessList.Contains(organizationId.ToString()))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);

                await _userServices.UnassignManagerFromDepartment(organizationId, userId, departmentId,callerId);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}