#nullable enable
using AutoMapper;
using Common.Interfaces;
using Customer.API.Filters;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Customer.API.Controllers;

/// <summary>
/// User management endpoints
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/organizations/{customerId:Guid}/[controller]")]
[SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
[SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
[ServiceFilter(typeof(ErrorExceptionFilter))]
public class UsersController : ControllerBase
{

    private readonly IUserServices _userServices;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;

    /// <summary>
    /// The controller needs access to the logger service, the user service and the automapper.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="userServices"></param>
    /// <param name="mapper"></param>
    public UsersController(ILogger<UsersController> logger, IUserServices userServices, IMapper mapper)
    {
        _logger = logger;
        _userServices = userServices;
        _mapper = mapper;
    }

    /// <summary>
    /// Count the number of users for this customer
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    [Route("count")]
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<CustomerServices.Models.OrganizationUserCount?>> GetUsersCount(Guid customerId, [FromQuery(Name = "filterOptions")] string? filterOptionsAsJsonString)
    {
        FilterOptionsForUser? filterOptions = null;
        if (!string.IsNullOrEmpty(filterOptionsAsJsonString))
        {
            filterOptions = JsonSerializer.Deserialize<FilterOptionsForUser>(filterOptionsAsJsonString);
        }

        var count = await _userServices.GetUsersCountAsync(customerId, filterOptions?.AssignedToDepartments, filterOptions?.Roles);
        if (count == null)
            return Ok(new CustomerServices.Models.OrganizationUserCount { OrganizationId = customerId});

        return Ok(count);
    }

    /// <summary>
    /// Get all users for a customer with the options to search or filter on different parameters.
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="filterOptionsAsJsonString"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="search"></param>
    /// <param name="page"></param>
    /// <param name="limit"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<User>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<PagedModel<User>>> GetAllUsers(Guid customerId, [FromQuery(Name = "filterOptions")] string? filterOptionsAsJsonString, CancellationToken cancellationToken, [FromQuery(Name = "q")] string? search, int page = 1, int limit = 1000)
    {
        FilterOptionsForUser? filterOptions = null;
        if (!string.IsNullOrEmpty(filterOptionsAsJsonString))
        {
            filterOptions = JsonSerializer.Deserialize<FilterOptionsForUser>(filterOptionsAsJsonString);
        }
            
        var users = await _userServices.GetAllUsersAsync(customerId, filterOptions?.Roles, filterOptions?.AssignedToDepartments, filterOptions?.UserStatuses, cancellationToken, search ?? string.Empty, page, limit);

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
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="callerId"></param>
    /// <param name="softDelete"></param>
    /// <returns cref="HttpStatusCode.NoContent"></returns>
    /// <returns cref="HttpStatusCode.BadRequest"></returns>
    /// <returns cref="HttpStatusCode.NotFound"></returns>
    [Route("{userId:Guid}")]
    [HttpDelete]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> DeleteUser(Guid customerId, Guid userId, [FromBody] Guid callerId, bool softDelete = true)
    {
        try
        {
            var deletedUser = await _userServices.DeleteUserAsync(customerId, userId, callerId, softDelete);
            if (deletedUser == null)
                return NotFound("The requested resource don't exist.");
            // TODO: Ask about this status code 302. Does this make sense??
            // The resource was deleted successfully.
            return Ok(_mapper.Map<User>(deletedUser));
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
            var user = await _userServices.SetUserActiveStatusAsync(customerId, userId, isActive, callerId);
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
            return BadRequest("Unable to change user status.");
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

    [Route("{userId:Guid}/initiate-offboarding")]
    [HttpPost]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> InitiateOffboarding(Guid customerId, Guid userId, [FromBody] OffboardingInitiated offboardData)
    {
        try
        {
            var user = await _userServices.InitiateOffboarding(customerId, userId, offboardData.LastWorkingDay, offboardData.BuyoutAllowed, offboardData.CallerId);
            return Ok(_mapper.Map<User>(user));
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

    [Route("{userId:Guid}/{callerId:Guid}/cancel-offboarding")]
    [HttpPost]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> CancelOffboarding(Guid customerId, Guid userId, Guid callerId)
    {
        try
        {
            var user = await _userServices.CancelOffboarding(customerId, userId, callerId);
            return Ok(_mapper.Map<User>(user));
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

    /// <summary>
    /// Mostly will be used by the scheduler. It is called when Offboarding is overdued
    /// </summary>
    /// <param name="customerId">CustomerId of the user/param>
    /// <param name="userId">ID of the user that is overdued</param>
    /// <param name="callerId">ID of the caller/param>
    /// <returns></returns>
    [Route("{userId:Guid}/overdue-offboarding")]
    [HttpPost]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> OverdueOffboarding(Guid customerId, Guid userId, [FromBody] Guid callerId)
    {
        try
        {
            var user = await _userServices.OverdueOffboarding(customerId, userId, callerId);
            return Ok(_mapper.Map<User>(user));
        }
        catch (UserNotFoundException exception)
        {

            return BadRequest(exception.Message);
        }
        catch (DepartmentNotFoundException exception)
        {

            return BadRequest(exception.Message);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Mostly will be used by the scheduler. It is called when Offboarding is completed for an employee after its last working day
    /// </summary>
    /// <param name="customerId">CustomerId of the user/param>
    /// <param name="userId">ID of the user that is overdued</param>
    /// <param name="callerId">ID of the caller/param>
    /// <returns></returns>
    [Route("{userId:Guid}/complete-offboarding")]
    [HttpPost]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> CompleteOffboarding(Guid customerId, Guid userId, [FromBody] Guid callerId)
    {
        try
        {
            var user = await _userServices.CompleteOffboarding(customerId, userId, callerId);
            return Ok(_mapper.Map<User>(user));
        }
        catch (UserNotFoundException exception)
        {

            return BadRequest(exception.Message);
        }
        catch (DepartmentNotFoundException exception)
        {

            return BadRequest(exception.Message);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    /// <summary>
    /// Only used by userpermission gateway to get info about user to be made a claim for
    /// Either userName or userId
    /// </summary>
    /// <param name="userName">Null or a vaue</param>
    /// <returns></returns>
    [Obsolete("Will be removed when controller for adding one user permission at a time gets removed")]
    [Route("/api/v{version:apiVersion}/organizations/{userName}/users-info")]
    [HttpGet]
    [ProducesResponseType(typeof(UserInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UserInfo>> GetUserFromUserName(string userName)
    {
        var user = await _userServices.GetUserInfoFromUserName(userName);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<UserInfo>(user));
    }
    /// <summary>
    /// Only used by userpermission gateway to get info about user to be made a claim for
    /// </summary>
    /// <param name="userId">Empty Guid or a value</param>
    /// <returns></returns>
    [Route("/api/v{version:apiVersion}/organizations/{userId:Guid}/users-info")]
    [HttpGet]
    [ProducesResponseType(typeof(UserInfo), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<UserInfo>> GetUserFromUserId(Guid userId)
    {
        var user = await _userServices.GetUserInfoFromUserId(userId);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<UserInfo>(user));
    }
    /// <summary>
    /// Resends the Origo invitation mail to a user. 
    /// </summary>
    /// <param name="customerId">Customer id that has the user.</param>
    /// <param name="usersInvitations">A list of user ids to send invitation to.</param>
    /// <param name="filterOptionsAsJsonString">Query string containing information about the role and access of the user making the call.</param>
    /// <returns>Returns a ActionResult.The ActionResult types represent various HTTP status codes.</returns>
    [Route("re-send-invitation")]
    [HttpPost]
    [ProducesResponseType(typeof(ExceptionMessages), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ExceptionMessages>> ResendOrigoInvitationMails(Guid customerId, [FromBody] ResendInvitation usersInvitations, [FromQuery(Name = "filterOptions")] string? filterOptionsAsJsonString)
    {
        FilterOptionsForUser? filterOptions = null;
        if (!string.IsNullOrEmpty(filterOptionsAsJsonString))
        {
            filterOptions = JsonSerializer.Deserialize<FilterOptionsForUser>(filterOptionsAsJsonString);
        }

        var errorMessages = await _userServices.ResendOrigoInvitationMail(customerId, usersInvitations.UserIds, filterOptions?.Roles, filterOptions?.AssignedToDepartments);
        return Ok(_mapper.Map<ExceptionMessages>(errorMessages));
    }
    /// <summary>
    /// Completes the onbaording process and changes user status to Activated if conditions are met.
    /// </summary>
    /// <param name="customerId">Users connected organization.</param>
    /// <param name="userId">User to be activated.</param>
    /// <returns>Returns a ActionResult with User object.The ActionResult types represent various HTTP status codes.</returns>
    [Route("{userId:Guid}/onboarding-completed")]
    [HttpPost]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<User>> CompleteOnboarding(Guid customerId, Guid userId)
    {

        var user = await _userServices.CompleteOnboardingAsync(customerId,userId);

        return Ok(_mapper.Map<User>(user));

    }
}