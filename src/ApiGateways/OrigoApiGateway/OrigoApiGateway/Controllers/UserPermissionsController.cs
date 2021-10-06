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
        [ProducesResponseType(typeof(IList<OrigoUserPermissions>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoUserPermissions>>> GetPermissions(string userName)
        {
            try
            {
                var userRole = await _userPermissionServices.GetUserPermissionsAsync(userName);
                if (userRole == null)
                {
                    return NotFound();
                }

                return Ok(userRole);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(OrigoUserPermissions), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoUserPermissions>> AddUserPermission(string userName, [FromBody] NewUserPermissions userPermissions)
        {
            try
            {
                var addedRole = await _userPermissionServices.AddUserPermissionsForUserAsync(userName, userPermissions);
                if (addedRole != null)
                {
                    return CreatedAtAction(nameof(AddUserPermission), addedRole);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(OrigoUserPermissions), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoUserPermissions>> RemoveUserPermission(string userName, [FromBody] NewUserPermissions userPermissions)
        {
            try
            {
                var removedRole = await _userPermissionServices.RemoveUserPermissionsForUserAsync(userName, userPermissions);
                if (removedRole != null)
                {
                    return Ok(removedRole);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }
    }
}
