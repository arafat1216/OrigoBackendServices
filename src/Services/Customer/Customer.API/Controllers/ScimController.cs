#nullable enable
using System.Net;
using AutoMapper;
using Common.Interfaces;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using CustomerServices.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using User = Customer.API.ViewModels.User;

// using User = Customer.API.ViewModels.User;

// using User = Customer.API.ViewModels.User;

namespace Customer.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/users")]
[SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned when the system encountered an unexpected problem.")]
public class ScimController : ControllerBase
{
    private readonly ILogger<ScimController> _logger;
    private readonly IUserServices _userServices;
    private readonly IMapper _mapper;

    public ScimController(ILogger<ScimController> logger, IUserServices userServices, IMapper mapper)
    {
        _logger = logger;
        _userServices = userServices;
        _mapper = mapper;
    }


    [HttpGet("{userId:Guid}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<User>> GetUser([FromRoute] Guid userId)
    {
        var user = await _userServices.GetUserWithRoleAsync(userId);
        if (user == null) return NotFound();
        return Ok(_mapper.Map<User>(user));
    }


    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(PagedModel<UserDTO>))]
    public async Task<ActionResult<PagedModel<UserDTO>>> GetAllUsers(
        [FromQuery] string? userName,
        CancellationToken cancellationToken,
        [FromQuery] int startIndex = 0,
        [FromQuery] int limit = 25)
    {
        var results = await _userServices.GetAllScimUsersAsync(userName, cancellationToken, startIndex, limit);
        return Ok(results);
    }


    [HttpPost("organizations/{organizationId:Guid}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<User>> CreateUser([FromRoute] Guid organizationId, [FromBody] NewUser newUser)
    {
        try
        {
            var userDto = await _userServices.AddUserForCustomerAsync(organizationId, newUser.FirstName,
                newUser.LastName, newUser.Email, newUser.MobileNumber, newUser.EmployeeId, new CustomerServices.Models.UserPreference(newUser.UserPreference?.Language
                    , newUser.CallerId), newUser.CallerId, newUser.Role, newUser.NeedsOnboarding, newUser.SkipAddingUserToOkta);
            var updatedUserView = _mapper.Map<User>(userDto);

            return CreatedAtAction(nameof(CreateUser), new { id = updatedUserView.Id }, updatedUserView);
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


    [HttpPut("{userId:Guid}/organizations/{organizationId:Guid}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<User>> UpdateUserPut([FromRoute] Guid organizationId, [FromRoute] Guid userId, [FromBody] UpdateUser updateUser)
    {
        try
        {
            var userPreference = updateUser.UserPreference == null ? null : new CustomerServices.Models.UserPreference(updateUser.UserPreference?.Language, updateUser.CallerId);
            var updatedUser = await _userServices.UpdateUserPutAsync(organizationId, userId, updateUser.FirstName,
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


    [HttpDelete("{userId:Guid}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> DeleteUser([FromRoute] Guid userId, [FromBody] Guid callerId, bool softDelete = true)
    {
        try
        {
            var deletedUser = await _userServices.DeleteUserAsync(userId, callerId, softDelete);
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
    
}