using Customer.API.ViewModels;
using CustomerServices;
using CustomerServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/organizations/{customerId:Guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UsersController : ControllerBase
    {

        private readonly IUserServices _userServices;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IUserServices userServices)
        {
            _logger = logger;
            _userServices = userServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<User>>> GetAllUsers(Guid customerId)
        {
            var users = await _userServices.GetAllUsersAsync(customerId);
            if (users == null) return NotFound();
            var foundUsers = new List<User>();
            foreach (var user in users)
            {
                foundUsers.Add(new User(user));

            }
            return Ok(foundUsers);
        }

        [Route("{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User>> GetUser(Guid customerId, Guid userId)
        {
            var user = await _userServices.GetUserAsync(customerId, userId);
            if (user == null) return NotFound();
            return Ok(new User(user));
        }

        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<User>> CreateUserForCustomer(Guid customerId, [FromBody] NewUser newUser)
        {
            try
            {
                var updatedUser = await _userServices.AddUserForCustomerAsync(customerId, newUser.FirstName,
                    newUser.LastName, newUser.Email, newUser.MobileNumber, newUser.EmployeeId);
                var updatedUserView = new User(updatedUser);

                return CreatedAtAction(nameof(CreateUserForCustomer), new { id = updatedUserView.Id }, updatedUserView);
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch
            {
                return BadRequest("Unable to save user");
            }
        }

        [Route("{userId:Guid}")]
        [HttpPut]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<User>> UpdateUserPut(Guid customerId, Guid userId, [FromBody] UpdateUser updateUser)
        {
            try
            {
                var updatedUser = await _userServices.UpdateUserPostAsync(customerId, userId, updateUser.FirstName,
                    updateUser.LastName, updateUser.Email, updateUser.EmployeeId);
                if (updatedUser == null)
                    return NotFound();

                var updatedUserView = new User(updatedUser);
                return Ok(updatedUserView);
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch
            {
                return BadRequest("Unable to save user");
            }
        }

        [Route("{userId:Guid}")]
        [HttpPatch]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<User>> UpdateUserPatch(Guid customerId, Guid userId, [FromBody] UpdateUser updateUser)
        {
            try
            {
                var updatedUser = await _userServices.UpdateUserPostAsync(customerId, userId, updateUser.FirstName,
                    updateUser.LastName, updateUser.Email, updateUser.EmployeeId);
                if (updatedUser == null)
                    return NotFound();

                var updatedUserView = new User(updatedUser);
                return Ok(updatedUserView);
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch
            {
                return BadRequest("Unable to save user");
            }
        }

        /// <summary>
        /// If this is true then the entity will only be soft-deleted (isDeleted or any equivalent value). This is the default handling that is used by all user-initiated calls.
        /// When it is false, the entry is permanently deleted from the system.This should only be run under very spesific circumstances by the automated cleanup tools, and only on assets that is already soft-deleted.
        /// Default value : true
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softDelete"></param>
        /// <returns cref="HttpStatusCode.NoContent"></returns
        /// <returns cref="HttpStatusCode.BadRequest"></returns>
        /// <returns cref="HttpStatusCode.NotFound"></returns>
        [Route("{userId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<User>> DeleteUser(Guid userId, bool softDelete = true)
        {
            try
            {
                var deletedUser = await _userServices.DeleteUserAsync(userId, softDelete);
                if (deletedUser == null)
                    return NotFound("The requested resource don't exist.");
                // TODO: Ask about this status code 302. Does this make sense??
                // The resource was deleted successfully.
                return NoContent();
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch (UserDeletedException)
            {
                // TODO: 410 result?
                return NotFound("The requested resource have already been deleted (soft-delete).");
            }
            catch
            {
                return BadRequest("Unable to delete user");
            }
        }
        [Route("{userId:Guid}/department/{departmentId:Guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User>> AssingDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await _userServices.AssignDepartment(customerId, userId, departmentId);
            if (user == null) return NotFound();
            return Ok(new User(user));
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}/manager")]
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            try
            {
                await _userServices.AssignManagerToDepartment(customerId, userId, departmentId);
                return Ok();
            }
            catch (DepartmentNotFoundException exception)
            {

                return BadRequest(exception.Message);
            }
            catch (UserNotFoundException exception)
            {

                return BadRequest(exception.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}/manager")]
        [HttpDelete]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            try
            {
                await _userServices.UnassignManagerFromDepartment(customerId, userId, departmentId);
                return Ok();
            }
            catch (DepartmentNotFoundException exception)
            {

                return BadRequest(exception.Message);
            }
            catch (UserNotFoundException exception)
            {

                return BadRequest(exception.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User>> RemoveAssignedDepartment(Guid customerId, Guid userId, Guid departmentId)
        {
            var user = await _userServices.UnassignDepartment(customerId, userId, departmentId);
            if (user == null) return NotFound();
            return Ok(new User(user));
        }
    }
}