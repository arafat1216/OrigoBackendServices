using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
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
    [Route("api/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class AssetsController : ControllerBase
    {
        private readonly IAssetServices _assetServices;
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices)
        {
            _logger = logger;
            _assetServices = assetServices;
        }

        [Route("customers/{customerId:guid}/users/{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> GetAssetsForUser(Guid customerId, Guid userId)
        {
            var assets = await _assetServices.GetAssetsForUserAsync(customerId, userId);
            if (assets == null)
            {
                return NotFound();
            }
            var assetList = new List<ViewModels.Asset>();
            foreach (var asset in assets)
            {
                var assetToReturn = new ViewModels.Asset(asset);
                assetList.Add(assetToReturn);
            }

            return Ok(assetList);
        }

        [Route("customers/{customerId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> Get(Guid customerId, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 50)
        {
            var assets = await _assetServices.GetAssetsForCustomerAsync(customerId, search, page, limit, cancellationToken);
            if (assets == null)
            {
                return NotFound();
            }

            var assetList = new List<ViewModels.Asset>();
            foreach (var asset in assets)
            {
                var assetToReturn = new ViewModels.Asset(asset);
                assetList.Add(assetToReturn);
            }

            return Ok(assetList);
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}")]
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

        [Route("customers/{customerId:guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAsset(Guid customerId, [FromBody] NewAsset asset)
        {
            try
            {
                var updatedAsset = await _assetServices.AddAssetForCustomerAsync(customerId, asset.SerialNumber,
                    asset.AssetCategoryId, asset.Brand, asset.Model, asset.LifecycleType, asset.PurchaseDate,
                    asset.AssetHolderId, asset.IsActive, asset.ManagedByDepartmentId);
                var updatedAssetView = new ViewModels.Asset(updatedAsset);

                return CreatedAtAction(nameof(CreateAsset), new { id = updatedAssetView.AssetId }, updatedAssetView);

            }
            catch (AssetCategoryNotFoundException)
            {
                return BadRequest("Unable to find Asset CategoryId");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/Activate/{isActive:bool}")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> SetActiveStatusOnAsset(Guid customerId, Guid assetId, bool isActive)
        {
            try
            {
                var updatedAsset = await _assetServices.UpdateActiveStatus(customerId, assetId, isActive);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                return Ok(new ViewModels.Asset(updatedAsset));

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/Update")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> UpdateAsset(Guid customerId,Guid assetId, [FromBody] UpdateAsset asset)
        {
            try
            {
                var updatedAsset = await _assetServices.UpdateAssetAsync(customerId, assetId, asset.SerialNumber, asset.Brand, asset.Model, asset.PurchaseDate);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                return Ok(new ViewModels.Asset(updatedAsset));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}