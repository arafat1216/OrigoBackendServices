using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Asset.API.ViewModels;
using AssetServices;
using AssetServices.Models;
using AssetServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Common.Exceptions;
using System.Text;

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
        [ProducesResponseType(typeof(PagedAssetList), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PagedAssetList>> Get(Guid customerId, CancellationToken cancellationToken, [FromQuery(Name = "q")] string search = "", int page = 1, int limit = 1000)
        {
            var pagedAssetResult = await _assetServices.GetAssetsForCustomerAsync(customerId, search, page, limit, cancellationToken);
            if (pagedAssetResult == null)
            {
                return NotFound();
            }

            var assetList = new List<ViewModels.Asset>();
            foreach (var asset in pagedAssetResult.Items)
            {
                var assetToReturn = new ViewModels.Asset(asset);
                assetList.Add(assetToReturn);
            }

            return Ok(new PagedAssetList
            {
                CurrentPage = pagedAssetResult.CurrentPage,
                TotalItems = pagedAssetResult.TotalItems,
                TotalPages = pagedAssetResult.TotalPages,
                Assets = assetList
            });
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

        [Route("lifecycles")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.AssetLifecycle>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetLifecycles()
        {
            try
            {
                var lifecycles = await _assetServices.GetLifecycles();
                if (lifecycles == null)
                {
                    return NotFound();
                }
                var lifecycleList = new List<ViewModels.AssetLifecycle>();
                foreach (var lifecycle in lifecycles) lifecycleList.Add(new ViewModels.AssetLifecycle() { Name = lifecycle.Name, EnumValue = lifecycle.EnumValue });

                return Ok(lifecycleList);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/ChangeLifecycleType/{newLifecycleType:int}")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ChangeLifecycleTypeOnAsset(Guid customerId, Guid assetId, int newLifecycleType)
        {
            try
            {
                // Check if given int is within valid range of values
                if (!Enum.IsDefined(typeof(LifecycleType), newLifecycleType))
                {
                    Array arr = Enum.GetValues(typeof(LifecycleType));
                    StringBuilder errorMessage = new StringBuilder(string.Format("The given value for lifecycle: {0} is out of bounds.\nValid options for lifecycle are:\n", newLifecycleType));
                    foreach (LifecycleType e in arr)
                    {
                        errorMessage.Append($"    -{(int)e} ({e})\n");
                    }
                    throw new InvalidLifecycleTypeException(errorMessage.ToString());
                }
                LifecycleType lifecycleType = (LifecycleType)newLifecycleType;
                var updatedAsset = await _assetServices.ChangeAssetLifecycleTypeForCustomerAsync(customerId, assetId, lifecycleType);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                return Ok(new ViewModels.Asset(updatedAsset));
            }
            catch (InvalidLifecycleTypeException ex)
            {
                return UnprocessableEntity(ex.Message);
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
        public async Task<ActionResult> UpdateAsset(Guid customerId, Guid assetId, [FromBody] UpdateAsset asset)
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

        [Route("{assetId:Guid}/customer/{customerId:guid}/assign")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AssignAsset(Guid customerId, Guid assetId, Guid? userId)
        {
            try
            {
                var updatedAsset = await _assetServices.AssignAsset(customerId, assetId, userId);
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

        [Route("assets/categories")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.AssetCategory), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.AssetCategory>>> GetAssetCategories()
        {
            var asset = await _assetServices.GetAssetCategoriesAsync();
            if (asset == null)
            {
                return NotFound();
            }
            return Ok(asset);
        }
    }
}