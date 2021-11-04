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
        private readonly IOktaServices _oktaServices;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, IUserServices userServices, IOktaServices oktaServices)
        {
            _logger = logger;
            _userServices = userServices;
            _oktaServices = oktaServices;
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
                
                var oktaUserId = await _oktaServices.AddOktaUser(updatedUser.UserId, updatedUser.FirstName, updatedUser.LastName, updatedUser.Email, updatedUser.MobileNumber, true);
                updatedUser = await _userServices.AssignOktaUserId(updatedUser.Customer.OrganizationId, updatedUser.UserId, oktaUserId);

                var updatedUserView = new User(updatedUser);
                return CreatedAtAction(nameof(CreateUserForCustomer), new { id = updatedUserView.Id }, updatedUserView);
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch (UserNotFoundException ex)
            {
                // This will happen if the user somehow is not able to be located after creation, when we try to update its OktaUserId
                return BadRequest(ex.Message);
            }
            catch
            {
                return BadRequest("Unable to save user");
            }
        }

        [Route("{userId:Guid}/deactivate")]
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DeactivateUser(Guid customerId, Guid userId)
        {
            try
            {
                await _userServices.DeactivateUser(customerId, userId);
                return Ok();
            }
            catch (UserNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to deactivate user");
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