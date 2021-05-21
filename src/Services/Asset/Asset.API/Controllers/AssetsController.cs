using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Asset.API.ViewModels;
using AssetServices;
using AssetServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Asset.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    // Assets should only be available through a given customer
    [Route("api/v{version:apiVersion}/Customers/{customerId:guid}/Assets")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetServices _assetServices;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices)
        {
            _logger = logger;
            _assetServices = assetServices;
        }

        [Route("Users/{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.Asset), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> GetAssetsForUser(Guid customerId, Guid userId)
        {
            var assets = await _assetServices.GetAssetsForUserAsync(customerId, userId);

            var assetList = new List<ViewModels.Asset>();
            foreach (var asset in assets)
            {
                var assetToReturn = new ViewModels.Asset(asset);
                assetList.Add(assetToReturn);
            }

            return Ok(assetList);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.Asset), (int) HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> Get(Guid customerId)
        {
            var assets = await _assetServices.GetAssetsForCustomerAsync(customerId);

            var assetList = new List<ViewModels.Asset>();
            foreach (var asset in assets)
            {
                var assetToReturn = new ViewModels.Asset(asset);
                assetList.Add(assetToReturn);
            }

            return Ok(assetList);
        }

        [Route("{assetId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> Get(Guid customerId, Guid assetId)
        {
            var asset = await _assetServices.GetAssetForCustomerAsync(customerId, assetId);

            if (asset == null)
            {
                return NotFound();
            }
            return Ok(new ViewModels.Asset(asset));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAsset(Guid customerId, [FromBody] NewAsset asset)
        {
            try
            {
                var updatedAsset = await _assetServices.AddAssetForCustomerAsync(customerId, asset.SerialNumber,
                    asset.AssetCategoryId, asset.Brand, asset.Model, asset.LifecycleType, asset.PurchaseDate,
                    asset.AssetHolderId, asset.IsActive, asset.ManagedByDepartmentId);
                var updatedAssetView = new ViewModels.Asset(updatedAsset);

                return CreatedAtAction(nameof(CreateAsset), new {id = updatedAssetView.AssetId}, updatedAssetView);

            }
            catch (AssetCategoryNotFoundException)
            {
                return BadRequest("Unable to find assetCategoryId");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}