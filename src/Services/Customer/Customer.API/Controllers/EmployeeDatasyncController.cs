using System.Net;
using System.Text.Json;
using AutoMapper;
using Common.Extensions;
using Common.Interfaces;
using Common.Model.EventModels.DatasyncModels;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

#nullable enable

namespace Customer.API.Controllers;

/// <summary>
/// Customer Data Sync Employe endpoints
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("/")]
[Tags("Customer Data Sync API")]
[SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned when the system encountered an unexpected problem.")]
public class EmployeeDataSyncController : ControllerBase
{
    private readonly ILogger<EmployeeDataSyncController> _logger;
    private readonly IUserServices _userServices;
    private readonly IDepartmentsServices _departmentServices;
    private readonly IMapper _mapper;

    /// <summary>
    /// The controller needs access to the logger service, the user service.
    /// </summary>
    /// <param name="logger"> The injected <see cref="ILogger"/> instance. </param>
    /// <param name="userServices"> The injected <see cref="IUserServices"/> instance. </param>
    /// <param name="departmentServices"> The injected <see cref="IDepartmentsServices"/> instance. </param>
    /// <param name="mapper"> The injected <see cref="IMapper"/> (automapper) instance. </param>
    public EmployeeDataSyncController(ILogger<EmployeeDataSyncController> logger, IUserServices userServices, IDepartmentsServices departmentServices, IMapper mapper)
    {
        _logger = logger;
        _userServices = userServices;
        _departmentServices = departmentServices;
        _mapper = mapper;
    }


    /// <summary>
    /// Get a User detail under a Customer
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("users/{userId:Guid}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> GetUser([FromRoute] Guid userId)
    {
        var user = await _userServices.GetUserWithRoleAsync(userId);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<User>(user));
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
    [ProducesResponseType(typeof(PagedModel<User>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<PagedModel<User>>> GetAllEmployees([FromRoute] Guid customerId,
        [FromQuery(Name = "filterOptions")] string? filterOptionsAsJsonString,
        [FromQuery(Name = "q")] string? search,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 25)
    {
        FilterOptionsForUser? filterOptions = null;
        if (!string.IsNullOrEmpty(filterOptionsAsJsonString))
        {
            filterOptions = JsonSerializer.Deserialize<FilterOptionsForUser>(filterOptionsAsJsonString);
        }

        var users = await _userServices.GetAllUsersAsync(customerId,
            filterOptions?.Roles,
            filterOptions?.AssignedToDepartments,
            filterOptions?.UserStatuses,
            cancellationToken,
            search ?? string.Empty,
            page,
            limit);

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


    /// <summary>
    /// Handles the create employee event by creating a user for the customer given as id 
    /// </summary>
    /// <param name="createEmployeeEvent"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("create-employee")]
    [Topic("customer-datasync-pub-sub", "create-employee")]
    [SwaggerResponse(StatusCodes.Status201Created)]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateEmployeeEndUserForCustomer([FromBody] CreateEmployeeEvent createEmployeeEvent)
    {
        try
        {
            // TODO: Look into using automapper in a better way here?

            var newUser = await _userServices.AddUserForCustomerAsync(
                createEmployeeEvent.CustomerId,
                createEmployeeEvent.FirstName,
                createEmployeeEvent.LastName,
                createEmployeeEvent.Email,
                createEmployeeEvent.MobileNumber,
                null,
                null,
                Guid.Empty.PubsubUserId(),
                null,
                false,
                false);
            return Created(String.Empty, newUser);
        }
        catch (CustomerNotFoundException ex)
        {
            _logger.LogError(ex, "Customer not found");
        }
        catch (OktaException ex)
        {
            _logger.LogError(ex, "Okta failed to activate user.");
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogError(ex, "User not found.");
        }
        catch (InvalidPhoneNumberException ex)
        {
            _logger.LogError(ex, "Invalid phone number.");
        }
        catch (UserNameIsInUseException ex)
        {
            _logger.LogError(ex, "User name is already in use.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to save user.");
        }

        return BadRequest();
    }


    /// <summary>
    /// Update User information
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="updateUser"></param>
    /// <returns></returns>
    [HttpPut("users/{userId:Guid}")]
    [Topic("customer-datasync-pub-sub", "update-employee")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<User>> UpdateUserPut([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromBody] UpdateUser updateUser)
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


    /// <summary>
    /// Delete a User
    /// </summary>
    /// <remarks>
    /// If this is true then the entity will only be soft-deleted (isDeleted or any equivalent value). This is the default handling that is used by all user-initiated calls.
    /// When it is false, the entry is permanently deleted from the system.This should only be run under very specific circumstances by the automated cleanup tools, and only on assets that is already soft-deleted.
    /// Default value : true
    /// </remarks>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="callerId"></param>
    /// <param name="softDelete"></param>
    /// <returns cref="HttpStatusCode.NoContent"></returns>
    /// <returns cref="HttpStatusCode.BadRequest"></returns>
    /// <returns cref="HttpStatusCode.NotFound"></returns>
    [HttpDelete("users/{userId:Guid}")]
    [Topic("customer-datasync-pub-sub", "delete-employee")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> DeleteUser([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromBody] Guid callerId, bool softDelete = true)
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


    /// <summary>
    /// Assign Department to a User
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpPost("users/{userId:Guid}/department/{departmentId:Guid}")]
    [Topic("customer-datasync-pub-sub", "employee-assign-department")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> AssignDepartment([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
    {
        var user = await _userServices.AssignDepartment(customerId, userId, departmentId, callerId);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<User>(user));
    }


    /// <summary>
    /// Assign Manager to a Department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpPost("users/{userId:Guid}/department/{departmentId:Guid}/manager")]
    [Topic("customer-datasync-pub-sub", "assign-department-manager")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> AssignManagerToDepartment([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
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


    /// <summary>
    /// Remove a User from a Department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpDelete("users/{userId:Guid}/department/{departmentId:Guid}")]
    [Topic("customer-datasync-pub-sub", "unassign-department")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> UnnassignDepartment([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
    {
        var user = await _userServices.UnassignDepartment(customerId, userId, departmentId, callerId);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<User>(user));
    }


    /// <summary>
    /// Remove a Manager from a Department
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="userId"></param>
    /// <param name="departmentId"></param>
    /// <param name="callerId"></param>
    /// <returns></returns>
    [HttpDelete("users/{userId:Guid}/department/{departmentId:Guid}/manager")]
    [Topic("customer-datasync-pub-sub", "unnassign-department-manager")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> UnassignManagerFromDepartment([FromRoute] Guid customerId, [FromRoute] Guid userId, [FromRoute] Guid departmentId, [FromBody] Guid callerId)
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


}