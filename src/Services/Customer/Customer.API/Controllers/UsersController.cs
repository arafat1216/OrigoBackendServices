using AutoMapper;
using Common.Interfaces;
using Customer.API.ViewModels;
using CustomerServices;
using CustomerServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
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
        private readonly IMapper _mapper;

        public UsersController(ILogger<UsersController> logger, IUserServices userServices, IMapper mapper)
        {
            _logger = logger;
            _userServices = userServices;
            _mapper = mapper;
        }

        [Route("count")]
        [HttpGet]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<int>> GetUsersCount(Guid customerId)
        {
            var count = await _userServices.GetUsersCountAsync(customerId);

            return Ok(count);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PagedModel<User>>> GetAllUsers(Guid customerId, CancellationToken cancellationToken, [FromQuery(Name = "q")] string search = "", int page = 1, int limit = 1000)
        {
            var users = await _userServices.GetAllUsersAsync(customerId, cancellationToken, search, page, limit);
            if (users.Items == null || users.Items.Count == 0) return NotFound();
            var response = new PagedModel<User>()
            {
                Items = _mapper.Map<IList<User>>(users.Items),
                CurrentPage = users.CurrentPage,
                PageSize = users.PageSize,
                TotalItems = users.TotalItems,
                TotalPages = users.TotalPages
            };
            return Ok(response);
        }

        [Route("{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User>> GetUser(Guid customerId, Guid userId)
        {
            var user = await _userServices.GetUserWithRoleAsync(customerId, userId);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<User>(user));
        }

        [Route("/api/v{version:apiVersion}/organizations/[controller]/{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User>> GetUser(Guid userId)
        {
            var user = await _userServices.GetUserWithRoleAsync(userId);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<User>(user));
        }

        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<User>> CreateUserForCustomer(Guid customerId, [FromBody] NewUser newUser)
        {
            try
            {
                var updatedUser = await _userServices.AddUserForCustomerAsync(customerId, newUser.FirstName,
                    newUser.LastName, newUser.Email, newUser.MobileNumber, newUser.EmployeeId, new CustomerServices.Models.UserPreference(newUser.UserPreference?.Language, newUser.CallerId), newUser.CallerId, newUser.Role);
                var updatedUserView = _mapper.Map<User>(updatedUser);

                return CreatedAtAction(nameof(CreateUserForCustomer), new { id = updatedUserView.Id }, updatedUserView);
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch (OktaException)
            {
                return BadRequest("Okta failed to activate user.");
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidPhoneNumberException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UserNameIsInUseException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex);
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
                var userPreference = updateUser.UserPreference == null ? null : new CustomerServices.Models.UserPreference(updateUser.UserPreference?.Language, updateUser.CallerId);
                var updatedUser = await _userServices.UpdateUserPutAsync(customerId, userId, updateUser.FirstName,
                    updateUser.LastName, updateUser.Email, updateUser.EmployeeId, updateUser.MobileNumber, userPreference, updateUser.CallerId);
                if (updatedUser == null)
                    return NotFound();

                var updatedUserView = _mapper.Map<User>(updatedUser);
                return Ok(updatedUserView);
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch (InvalidPhoneNumberException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UserNameIsInUseException ex)
            {
                return Conflict(ex.Message);
            }
            catch
            {
                return BadRequest("Unable to save user");
            }
        }

        [Route("{userId:Guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<User>> UpdateUserPatch(Guid customerId, Guid userId, [FromBody] UpdateUser updateUser)
        {
            try
            {
                var userPreference = updateUser.UserPreference == null ? null : new CustomerServices.Models.UserPreference(updateUser.UserPreference?.Language, updateUser.CallerId);
                var updatedUser = await _userServices.UpdateUserPatchAsync(customerId, userId, updateUser.FirstName,
                    updateUser.LastName, updateUser.Email, updateUser.EmployeeId, updateUser.MobileNumber, userPreference, updateUser.CallerId);
                if (updatedUser == null)
                    return NotFound();

                return Ok(_mapper.Map<User>(updatedUser));
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch (InvalidPhoneNumberException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UserNameIsInUseException ex)
            {
                return Conflict(ex.Message);
            }
            catch
            {
                return BadRequest("Unable to save user");
            }
        }

        /// <summary>
        /// If this is true then the entity will only be soft-deleted (isDeleted or any equivalent value). This is the default handling that is used by all user-initiated calls.
        /// When it is false, the entry is permanently deleted from the system.This should only be run under very specific circumstances by the automated cleanup tools, and only on assets that is already soft-deleted.
        /// Default value : true
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="callerId"></param>
        /// <param name="softDelete"></param>
        /// <returns cref="HttpStatusCode.NoContent"></returns>
        /// <returns cref="HttpStatusCode.BadRequest"></returns>
        /// <returns cref="HttpStatusCode.NotFound"></returns>
        [Route("{userId:Guid}")]
        [HttpDelete]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> DeleteUser(Guid userId, [FromBody] Guid callerId, bool softDelete = true)
        {
            try
            {
                var deletedUser = await _userServices.DeleteUserAsync(userId, callerId, softDelete);
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

        [Route("{userId:Guid}/activate/{isActive:bool}")]
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> SetUserActiveStatus(Guid customerId, Guid userId, bool isActive, [FromBody] Guid callerId)
        {
            try
            {
                var user = await _userServices.SetUserActiveStatus(customerId, userId, isActive, callerId);
                if (user == null)
                    return NotFound();
                return Ok(_mapper.Map<User>(user));
            }
            catch (UserNotFoundException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (Exception)
            {
                return BadRequest("Unable to deactivate user");
            }
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<User>> AssignDepartment(Guid customerId, Guid userId, Guid departmentId, [FromBody] Guid callerId)
        {
            var user = await _userServices.AssignDepartment(customerId, userId, departmentId, callerId);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<User>(user));
        }

        [Route("{userId:Guid}/department/{departmentId:Guid}/manager")]
        [HttpPost]
        [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, [FromBody] Guid callerId)
        {
            try
            {
                await _userServices.AssignManagerToDepartment(customerId, userId, departmentId, callerId);
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
        public async Task<ActionResult> UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, [FromBody] Guid callerId)
        {
            try
            {
                await _userServices.UnassignManagerFromDepartment(customerId, userId, departmentId, callerId);
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
        public async Task<ActionResult<User>> RemoveAssignedDepartment(Guid customerId, Guid userId, Guid departmentId, [FromBody] Guid callerId)
        {
            var user = await _userServices.UnassignDepartment(customerId, userId, departmentId, callerId);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<User>(user));
        }
    }
}