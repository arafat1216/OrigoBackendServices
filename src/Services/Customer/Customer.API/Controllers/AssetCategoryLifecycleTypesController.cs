using Customer.API.ViewModels;
using CustomerServices;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/customers/{customerId:Guid}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class AssetCategoryLifecycleTypesController : ControllerBase
    {
        private readonly IAssetCategoryLifecycleTypeServices _assetCategoryLifecycleTypeServices;
        private readonly ILogger<AssetCategoryLifecycleTypesController> _logger;

        public AssetCategoryLifecycleTypesController(ILogger<AssetCategoryLifecycleTypesController> logger, IAssetCategoryLifecycleTypeServices assetCategoryLifecycleTypeServices)
        {
            _logger = logger;
            _assetCategoryLifecycleTypeServices = assetCategoryLifecycleTypeServices;
        }
        /*
        [HttpGet]
        [ProducesResponseType(typeof(List<AssetCategoryLifecycleType>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<AssetCategoryLifecycleType>>> GetAllAssetCategoryLifecycleTypes(Guid customerId)
        {
            var assetCategoryLifecycleTypes = await _assetCategoryLifecycleTypeServices.GetAllAssetCategoryLifecycleTypesForCustomerAsync(customerId);
            if (assetCategoryLifecycleTypes == null) return NotFound();
            return Ok(assetCategoryLifecycleTypes);
        }
        

        [HttpPost]
        [ProducesResponseType(typeof(AssetCategoryLifecycleType), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<AssetCategoryLifecycleType>> CreateAssetCategoryLifecycleTypeForCustomer(Guid customerId, [FromBody] AssetCategoryLifecycleType assetCategoryLifecycleType)
        {
            try
            {
                var createdAssetCategoryLifecycleType = await _assetCategoryLifecycleTypeServices.AddAssetCategoryLifecycleTypeForCustomerAsync(assetCategoryLifecycleType.CustomerId, assetCategoryLifecycleType.AssetCategoryId, assetCategoryLifecycleType.LifecycleType);

                return CreatedAtAction(nameof(CreateAssetCategoryLifecycleTypeForCustomer), new { id = createdAssetCategoryLifecycleType.Id }, createdAssetCategoryLifecycleType);
            }
            catch (CustomerNotFoundException)
            {
                return BadRequest("Customer not found");
            }
            catch
            {
                return BadRequest("Unable to save assetCategoryLifecycleType");
            }
        }
        */
    }
}
