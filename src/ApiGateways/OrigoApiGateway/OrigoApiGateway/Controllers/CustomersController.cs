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

        [Route("{customerId:Guid}/AssetCategoryLifecycleType/get")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoAssetCategoryLifecycleType), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoAssetCategoryLifecycleType>>> GetAssetCategoryLifecycleTypesForCustomer(Guid customerId)
        {
            try
            {
                var assetCategoryLifecycleTypes = await CustomerServices.GetAssetCategoryLifecycleTypesForCustomerAsync(customerId);
                return assetCategoryLifecycleTypes != null ? Ok(assetCategoryLifecycleTypes) : NotFound();            
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

        [Route("{customerId:Guid}/AssetCategoryLifecycleType/add")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoAssetCategoryLifecycleType), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoAssetCategoryLifecycleType>> CreateAssetCategoryLifecycleTypeForCustomer([FromBody] NewAssetCategoryLifecycleType newAssetCategoryLifecycleType)
        {
            try
            {
                var createdAssetCategoryLifecycleType = await CustomerServices.AddAssetCategoryLifecycleTypeForCustomerAsync(newAssetCategoryLifecycleType);
                if (createdAssetCategoryLifecycleType == null)
                {
                    return BadRequest();
                }

                return CreatedAtAction(nameof(CreateAssetCategoryLifecycleTypeForCustomer), createdAssetCategoryLifecycleType);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [Route("{customerId:Guid}/modules/{moduleGroupId:Guid}/add")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoProductModuleGroup), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrigoProductModuleGroup>> AddCustomerModules(Guid customerId, Guid moduleGroupId)
        {
            try
            {
                var productGroup = await CustomerServices.AddProductModulesAsync(customerId, moduleGroupId);
                return productGroup != null ? Ok(productGroup) : NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/modules/{moduleGroupId:Guid}/remove")]
        [HttpPatch]
        [ProducesResponseType(typeof(OrigoProductModuleGroup), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrigoProductModuleGroup>> RemoveCustomerModules(Guid customerId, Guid moduleGroupId)
        {
            try
            {
                var productGroup = await CustomerServices.RemoveProductModulesAsync(customerId, moduleGroupId);
                return productGroup != null ? Ok(productGroup) : NotFound();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}