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
    [Route("api/v{version:apiVersion}/customers/{customerId:Guid}/[controller]")]
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

                return CreatedAtAction(nameof(CreateUserForCustomer), new {id = updatedUserView.Id}, updatedUserView);
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