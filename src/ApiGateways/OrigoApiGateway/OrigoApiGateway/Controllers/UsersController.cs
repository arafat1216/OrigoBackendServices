using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models;
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
    [Route("origoapi/v{version:apiVersion}/Customers/{customerId:guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserServices _userServices;

        public UsersController(ILogger<UsersController> logger, IUserServices customerServices)
        {
            _logger = logger;
            _userServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrigoUser>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<OrigoUser>>> GetAllUsers(Guid customerId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.CustomerAdmin.ToString() || role == PredefinedRole.GroupAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList").Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                {
                    return Forbid();
                }
            }

            var users = await _userServices.GetAllUsersAsync(customerId);
            if (users == null) return NotFound();
            return Ok(users);
        }

        [Route("{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<OrigoUser>> GetUser(Guid customerId, Guid userId)
        {
            var user = await _userServices.GetUserAsync(customerId, userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoUser>> CreateUserForCustomer(Guid customerId, [FromBody] NewUser newUser)
        {
            try
            {
                var updatedUser = await _userServices.AddUserForCustomerAsync(customerId, newUser);

                return CreatedAtAction(nameof(CreateUserForCustomer), new { id = updatedUser.Id }, updatedUser);
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
        public async Task<ActionResult<OrigoUser>> AssignDepartmentForCustomer(Guid customerId, Guid userId, Guid departmentId)
        {
            try
            {
                var updatedUser = await _userServices.AssignUserToDepartment(customerId, userId, departmentId);

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
        public async Task<ActionResult<OrigoUser>> RemoveAssignedDepartmentForCustomer(Guid customerId, Guid userId, Guid departmentId)
        {
            try
            {
                var updatedUser = await _userServices.UnassignUserFromDepartment(customerId, userId, departmentId);

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
        public async Task<ActionResult> AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            try
            {
                await _userServices.AssignManagerToDepartment(customerId, userId, departmentId);
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
        public async Task<ActionResult> UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            try
            {
                await _userServices.UnassignManagerFromDepartment(customerId, userId, departmentId);
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}