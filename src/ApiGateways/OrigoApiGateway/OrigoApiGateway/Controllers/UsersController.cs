using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
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
        private readonly IUserServices _customerServices;

        public UsersController(ILogger<UsersController> logger, IUserServices customerServices)
        {
            _logger = logger;
            _customerServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrigoUser>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<OrigoUser>>> GetAllUsers(Guid customerId)
        {
            var users = await _customerServices.GetAllUsersAsync(customerId);
            if (users == null) return NotFound();
            return Ok(users);
        }

        [Route("{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoUser), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<OrigoUser>> GetUser(Guid customerId, Guid userId)
        {
            var user = await _customerServices.GetUserAsync(customerId, userId);
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
                var updatedUser = await _customerServices.AddUserForCustomerAsync(customerId, newUser);

                return CreatedAtAction(nameof(CreateUserForCustomer), new { id = updatedUser.Id }, updatedUser);
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
        public async Task<ActionResult<OrigoUser>> UpdateUserForCustomer(Guid customerId, Guid userId, [FromBody] OrigoUpdateUser updateUser)
        {
            try
            {
                var updatedUser = await _customerServices.UpdateUserAsync(customerId, userId, updateUser);
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
        public async Task<ActionResult<OrigoUser>> DeleteUserForCustomer(Guid customerId, Guid userId)
        {
            try
            {
                var deletedUser = await _customerServices.DeleteUserAsync(customerId, userId);
                if (deletedUser == null)
                    return NotFound();

                return Ok(deletedUser);
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
                var updatedUser = await _customerServices.AssignUserToDepartment(customerId, userId, departmentId);

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
                var updatedUser = await _customerServices.UnassignUserFromDepartment(customerId, userId, departmentId);

                return Ok(updatedUser);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}