using Microsoft.AspNetCore.Http;
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
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using OrigoApiGateway.Models.BackendDTO;
using AutoMapper;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/customers/users/{userName:length(6,255)}/permissions")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UserPermissionsController : ControllerBase
    {
        private readonly ILogger<UserPermissionsController> _logger;
        private readonly IUserPermissionService _userPermissionServices;
        private readonly IMapper _mapper;

        public UserPermissionsController(ILogger<UserPermissionsController> logger, IUserPermissionService customerServices,IMapper mapper)
        {
            _logger = logger;
            _userPermissionServices = customerServices;
            _mapper = mapper;
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

        [HttpGet]
        [Route("/origoapi/v{version:apiVersion}/roles")]
        [ProducesResponseType(typeof(IList<string>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<string>>> GetRoles()
        {
            try
            {
                return Ok(await _userPermissionServices.GetAllRolesAsync());
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
                var userPermissionsDTO = _mapper.Map<NewUserPermissionsDTO>(userPermissions);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                userPermissionsDTO.CallerId = callerId;

                var addedRole = await _userPermissionServices.AddUserPermissionsForUserAsync(userName, userPermissionsDTO);
                return CreatedAtAction(nameof(AddUserPermission), addedRole);
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
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
                var userPermissionsDTO = _mapper.Map<NewUserPermissionsDTO>(userPermissions);

                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid callerId;
                Guid.TryParse(actor, out callerId);
                userPermissionsDTO.CallerId = callerId;

                var removedRole = await _userPermissionServices.RemoveUserPermissionsForUserAsync(userName, userPermissionsDTO);
                if (removedRole != null)
                {
                    return Ok(removedRole);
                }
                return NotFound();
            }
            catch (BadHttpRequestException ex)
            {
                _logger.LogError("{0}", ex.Message);
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
