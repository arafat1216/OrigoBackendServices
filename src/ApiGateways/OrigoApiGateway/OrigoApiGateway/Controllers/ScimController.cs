#nullable enable
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.SCIM;
using OrigoApiGateway.Services;
using System.Net;

namespace OrigoApiGateway.Controllers;

[ServiceFilter(typeof(ErrorExceptionFilter))]
[ApiController]
[ApiVersion("1.0")]
[Authorize(Roles = "SystemAdmin")]
[Route("/origoapi/v{version:apiVersion}/[controller]/v2")]
[SwaggerResponse(StatusCodes.Status500InternalServerError, "Returned if the system encounter an unexpected problem.")]
public class ScimController : ControllerBase
{
    private readonly ILogger<ScimController> _logger;
    private readonly IScimService _scimServices;
    private readonly IMapper _mapper;

    public ScimController(ILogger<ScimController> logger,
        IScimService scimService,
        IMapper mapper)
    {
        _logger = logger;
        _scimServices = scimService;
        _mapper = mapper;
    }
    
    
    private string? ParseUserName(string? filter)
    {
        if (filter is null)
            return null;

        string? userName = null;
        var tokes = filter.Trim().Split("eq");
        if (tokes.Length == 2)
        {
            if (tokes[0].Trim() == "userName")
            {
                userName = tokes[1]
                    .Replace("\"", "")
                    .Replace("'", "")
                    .Trim(new[] { ' ' });
            }
        }

        return userName;
    }
    

    [HttpGet("users/{userId:Guid}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
    public async Task<ActionResult<User>> GetUser(Guid userId)
    {
        var user = await _scimServices.GetUserAsync(userId);
        if (user is null)
            return NotFound(new ScimUserNotFound());
        return Ok(user);
    }


    [HttpGet("users")]
    [ProducesResponseType(typeof(ListResponse<User>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [PermissionAuthorize(Permission.CanReadCustomer)]
    public async Task<ActionResult<ListResponse<User>>> GetAllUsers(CancellationToken cancellationToken,
        [FromQuery] string? filter,
        [FromQuery] int startIndex = 1, 
        [FromQuery][Range(1, 100)] int count = 25)
    {
        // filter ex: filter=userName eq "test-user@testdomain.com"
        var userName = ParseUserName(filter);

        // Reducing index by 1. Because Scim provides 1-based index whereas EFCore takes 0-based index
        startIndex--;

        var users = await _scimServices.GetAllUsersAsync(cancellationToken, userName, startIndex, count);
        return Ok(users);
    }


    [HttpPost("users")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
    public async Task<ActionResult<User>> CreateUserForCustomer([FromBody] User scimUser)
    {
        var newUser = _mapper.Map<NewUser>(scimUser);
        var organizationId = Guid.Parse(scimUser.Groups.FirstOrDefault().Value);
        var user = await _scimServices.AddUserForCustomerAsync(organizationId, newUser, Guid.Empty, false);
        return Created(String.Empty, user);
    }


    [HttpPut("users/{userId:Guid}")]
    [ProducesResponseType(typeof(User), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
    public async Task<ActionResult<User>> UpdateUser(Guid userId, [FromBody] User updateScimUser)
    {
        var origoUpdateUser = _mapper.Map<OrigoUpdateUser>(updateScimUser);
        var organizationId = Guid.Parse(updateScimUser.Groups.FirstOrDefault().Value);
        var updatedUser = await _scimServices.PutUserAsync(organizationId, userId, origoUpdateUser, Guid.Empty);
        if (updatedUser == null)
            return NotFound(new ScimUserNotFound());
        return Ok(updatedUser);
    }


    [Route("users/{userId:Guid}")]
    [HttpDelete]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanUpdateCustomer)]
    public async Task<ActionResult> DeleteUser(Guid userId)
    {
        var deletedUser = await _scimServices.DeleteUserAsync(userId, true, Guid.Empty);
        return NoContent();
    }
}
