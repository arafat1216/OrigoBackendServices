using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/customers/users/{userName:length(6,255)}/permissions")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UserPermissionsController : ControllerBase
    {
        private readonly ILogger<UserPermissionsController> _logger;
        private readonly IUserPermissionService _userPermissionServices;

        public UserPermissionsController(ILogger<UserPermissionsController> logger, IUserPermissionService customerServices)
        {
            _logger = logger;
            _userPermissionServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<ClaimsIdentity>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ClaimsIdentity>> GetPermissions(string sub, string userName, CancellationToken cancellationToken)
        {
            try
            {
                var userRole = await _userPermissionServices.GetUserPermissionsIdentityAsync(sub, userName, cancellationToken);
                if (userRole == null)
                {
                    return NotFound();
                }
                
                return Ok(JsonSerializer.Serialize(userRole, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve }));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(ClaimsIdentity), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AddUserPermission(string userName, [FromBody] NewUserPermissions userPermissions)
        {
            try
            {
                var addedRole = await _userPermissionServices.AddUserPermissionsForUserAsync(userName, userPermissions);
                if (addedRole != null)
                {
                    return CreatedAtAction(nameof(AddUserPermission), new { id = addedRole.Name }, 
                        JsonSerializer.Serialize(addedRole, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve, WriteIndented = true }));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(ClaimsIdentity), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> RemoveUserPermission(string userName, [FromBody] NewUserPermissions userPermissions)
        {
            try
            {
                var removedRole = await _userPermissionServices.RemoveUserPermissionsForUserAsync(userName, userPermissions);
                if (removedRole != null)
                {
                    return Ok(JsonSerializer.Serialize(removedRole, new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve, WriteIndented = true }));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
