using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
// ReSharper disable RouteTemplates.RouteParameterConstraintNotResolved

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class CustomersController : ControllerBase
    {
        private ILogger<CustomersController> Logger { get; }
        public ICustomerServices CustomerServices { get; }

        public CustomersController(ILogger<CustomersController> logger, ICustomerServices customerServices)
        {
            Logger = logger;
            CustomerServices = customerServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoCustomer>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoCustomer>>> Get()
        {
            try
            {
                var customers = await CustomerServices.GetCustomersAsync();
                return customers != null ? Ok(customers) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoCustomer), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<OrigoCustomer>>> Get(Guid customerId)
        {
            try
            {
                var customer = await CustomerServices.GetCustomerAsync(customerId);
                return customer != null ? Ok(customer) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrigoCustomer), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoCustomer>> CreateCustomer([FromBody] OrigoNewCustomer newCustomer)
        {
            try
            {
                var createdCustomer = await CustomerServices.CreateCustomerAsync(newCustomer);
                if (createdCustomer == null)
                {
                    return BadRequest();
                }

                return CreatedAtAction(nameof(CreateCustomer), new { id = createdCustomer.Id }, createdCustomer);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("/module")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoModule>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<OrigoModule>>> GetModules()
        {
            try
            {
                var modules = await CustomerServices.GetModulesAsync();
                return modules != null ? Ok(modules) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}