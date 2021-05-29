﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
    [Route("origoapi/v{version:apiVersion}/Customers/{customerId:guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUserServices _customerServices;

        public UsersController(ILogger<UsersController> logger, IUserServices customerServices)
        {
            _logger = logger;
            _customerServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrigoUser>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<OrigoUser>>> GetAllUsers(Guid customerId)
        {
            var users = await _customerServices.GetAllUsersAsync(customerId);
            if (users == null) return NotFound();
            return Ok(users);
        }

        [Route("{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoUser), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<OrigoUser>> GetUser(Guid customerId, Guid userId)
        {
            var user = await _customerServices.GetUserAsync(customerId, userId);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrigoUser), (int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoUser>> CreateUserForCustomer(Guid customerId, [FromBody] NewUser newUser)
        {
            try
            {
                var updatedUser = await _customerServices.AddUserForCustomerAsync(customerId, newUser);

                return CreatedAtAction(nameof(CreateUserForCustomer), new {id = updatedUser.Id}, updatedUser);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}