﻿using Asset.API.ViewModels;
using AssetServices;
using AssetServices.Exceptions;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using AutoMapper;
using Common.Enums;
using Common.Exceptions;
using Dapr;

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
        private readonly IMapper _mapper;

        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices, IMapper mapper)
        {
            _logger = logger;
            _assetServices = assetServices;
            _mapper = mapper;
        }

        [Route("customers/count")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<AssetServices.Models.CustomerAssetCount>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<AssetServices.Models.CustomerAssetCount>>> GetAllCount()
        {
            var assetCountList = await _assetServices.GetAllCustomerAssetsCountAsync();

            return Ok(assetCountList);
        }

        [Route("customers/{customerId:guid}/count")]
        [HttpGet]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<int>> GetCount(Guid customerId)
        {
            var count = await _assetServices.GetAssetsCountAsync(customerId);

            return Ok(count);
        }

        [Route("customers/{customerId:guid}/total-book-value")]
        [HttpGet]
        [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<decimal>> GetCustomerTotalBookValue(Guid customerId)
        {
            var totalBookValue = await _assetServices.GetCustomerTotalBookValue(customerId);

            return Ok(totalBookValue);
        }



        [Route("customers/{customerId:guid}/users/{userId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<ViewModels.Asset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> GetAssetsForUser(Guid customerId, Guid userId)
        {
            var assetLifecycles = await _assetServices.GetAssetLifecyclesForUserAsync(customerId, userId);

            return Ok(_mapper.Map<IList<ViewModels.Asset>>(assetLifecycles));
        }

        [Route("customers/{customerId:guid}/users/{userId:Guid}")]
        [HttpPatch]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Topic("user_deleted_pub_sub", "user_topic")]
        public async Task<ActionResult> UnAssignAssetsFromUser(Guid customerId, Guid userId, [FromBody] UnAssignAssetToUser data)
        {
            await _assetServices.UnAssignAssetLifecyclesForUserAsync(customerId, userId, data.DepartmentId, data.CallerId);
            return Accepted();
        }

        [Route("customers/{customerId:guid}/labels")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Label>>> CreateLabelsForCustomer(Guid customerId, [FromBody] AddLabelsData data)
        {
            try
            {
                List<AssetServices.Models.Label> labels = new List<AssetServices.Models.Label>();
                foreach (NewLabel newLabel in data.NewLabels)
                {
                    labels.Add(new AssetServices.Models.Label(newLabel.Text, newLabel.Color));
                }

                var labelsAdded = await _assetServices.AddLabelsForCustomerAsync(customerId, data.CallerId, labels);

                if (labelsAdded == null)
                    return BadRequest("Unable to add labels.");

                var labelsView = new List<object>();
                foreach (AssetServices.Models.CustomerLabel label in labelsAdded)
                {
                    labelsView.Add(new ViewModels.Label(label));
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(labelsView, options));
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("customers/{customerId:guid}/labels")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<ViewModels.Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> GetLabelsForCustomer(Guid customerId)
        {
            var labels = await _assetServices.GetCustomerLabelsForCustomerAsync(customerId);
            if (labels == null)
                return NotFound("No labels found on customer. Did you enter the correct customerId?");

            var labelList = new List<object>();
            foreach (AssetServices.Models.CustomerLabel label in labels)
            {
                labelList.Add(new ViewModels.Label(label));
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return Ok(JsonSerializer.Serialize<object>(labelList, options));
        }

        [Route("customers/{customerId:guid}/labels")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteLabelsForCustomer(Guid customerId, [FromBody] DeleteCustomerLabelsData customerLabelsData)
        {
            await _assetServices.DeleteLabelsForCustomerAsync(customerId, customerLabelsData.CallerId, customerLabelsData.LabelGuids);
            return Ok();
        }

        [Route("customers/{customerId:guid}/labels/update")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Label>>> UpdateLabelsForCustomer(Guid customerId, [FromBody] UpdateCustomerLabelsData data)
        {
            try
            {
                IList<AssetServices.Models.CustomerLabel> customerLabels = new List<AssetServices.Models.CustomerLabel>();

                foreach (Label label in data.Labels)
                {
                    customerLabels.Add(new AssetServices.Models.CustomerLabel(label.Id, customerId, data.CallerId, new AssetServices.Models.Label(label.Text, label.Color)));
                }

                var updatedLabels = await _assetServices.UpdateLabelsForCustomerAsync(customerId, customerLabels);
                var labelList = new List<object>();
                foreach (AssetServices.Models.CustomerLabel label in updatedLabels)
                {
                    labelList.Add(new ViewModels.Label(label));
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(labelList, options));
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("customers/{customerId:guid}/labels/assign")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> AssignLabelsToAssets(Guid customerId, [FromBody] AssetLabels assetLabels)
        {
            var assets = await _assetServices.AssignLabelsToAssetsAsync(customerId, assetLabels.CallerId, assetLabels.AssetGuids, assetLabels.LabelGuids);
            return Ok(_mapper.Map<IList<ViewModels.Asset>>(assets));
        }

        [Route("customers/{customerId:guid}/labels/unassign")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> UnAssignLabelsToAssets(Guid customerId, [FromBody] AssetLabels assetLabels)
        {
            var assets = await _assetServices.UnAssignLabelsToAssetsAsync(customerId, assetLabels.CallerId, assetLabels.AssetGuids, assetLabels.LabelGuids);
            return Ok(_mapper.Map<IList<ViewModels.Asset>>(assets));
        }

        [Route("customers/{customerId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(PagedAssetList), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PagedAssetList>> Get(Guid customerId, CancellationToken cancellationToken, [FromQuery(Name = "q")] string search = "", int page = 1, int limit = 1000, AssetLifecycleStatus? status = null)
        {
            var pagedAssetResult = await _assetServices.GetAssetLifecyclesForCustomerAsync(customerId, search, page, limit, status, cancellationToken);
            var pagedAssetList = _mapper.Map<PagedAssetList>(pagedAssetResult);
            return Ok(pagedAssetList);
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ViewModels.Asset>> Get(Guid customerId, Guid assetId)
        {
            var assetLifecycle = await _assetServices.GetAssetLifecyclesForCustomerAsync(customerId, assetId);
            return Ok(_mapper.Map<ViewModels.Asset>(assetLifecycle));
        }

        [Route("customers/{customerId:guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAsset(Guid customerId, [FromBody] NewAsset asset)
        {
            try
            {
                var updatedAsset = await _assetServices.AddAssetLifecycleForCustomerAsync(customerId, asset.CallerId, asset.Alias, asset.SerialNumber,
                    asset.AssetCategoryId, asset.Brand, asset.ProductName, asset.LifecycleType, asset.PurchaseDate,
                    asset.AssetHolderId, asset.Imei, asset.MacAddress, asset.ManagedByDepartmentId, asset.Note, asset.Description, asset.PaidByCompany);
                return CreatedAtAction(nameof(CreateAsset), new { id = updatedAsset.ExternalId }, _mapper.Map<ViewModels.Asset>(updatedAsset));
            }
            catch (AssetCategoryNotFoundException)
            {
                return BadRequest("Unable to find Asset CategoryId");
            }
            catch (InvalidAssetDataException ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
        }


        [Route("customers/{customerId:guid}/assetStatus/{assetStatus:int}")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.Asset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> SetAssetStatusOnAssets(Guid customerId, [FromBody] UpdateAssetsStatus data, int assetStatus)
        {
            var updatedAssets = await _assetServices.UpdateStatusForMultipleAssetLifecycles(customerId, data.CallerId, data.AssetGuidList, (AssetLifecycleStatus)assetStatus);
            return Ok(_mapper.Map<IList<ViewModels.Asset>>(updatedAssets));
        }

        [Route("lifecycles")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<AssetLifecycleType>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult GetLifecycles()
        {
            var lifecycles = _assetServices.GetLifecycles();
            var lifecycleList = lifecycles.Where(l => l.EnumValue is 0 or 2)
                .Select(lifecycle => new AssetLifecycleType { Name = lifecycle.Name, EnumValue = lifecycle.EnumValue }).ToList();
            return Ok(lifecycleList);
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/ChangeLifecycleType")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ChangeLifecycleTypeOnAsset(Guid customerId, Guid assetId, [FromBody] UpdateAssetLifecycleType data)
        {
            // Check if given int is within valid range of values
            if (!Enum.IsDefined(typeof(LifecycleType), data.LifecycleType))
            {
                Array arr = Enum.GetValues(typeof(LifecycleType));
                StringBuilder errorMessage = new StringBuilder(string.Format("The given value for lifecycle: {0} is out of bounds.\nValid options for lifecycle are:\n", data.LifecycleType));
                foreach (LifecycleType e in arr)
                {
                    errorMessage.Append($"    -{(int)e} ({e})\n");
                }
                throw new InvalidLifecycleTypeException(errorMessage.ToString());
            }
            var lifecycleType = (LifecycleType)data.LifecycleType;
            var updatedAsset = await _assetServices.ChangeAssetLifecycleTypeForCustomerAsync(customerId, data.AssetId, data.CallerId, lifecycleType);
            if (updatedAsset == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ViewModels.Asset>(updatedAsset));
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/Update")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateAsset(Guid customerId, Guid assetId, [FromBody] UpdateAsset asset)
        {
            try
            {
                var updatedAsset = await _assetServices.UpdateAssetAsync(customerId, assetId, asset.CallerId, asset.Alias, asset.SerialNumber, asset.Brand, asset.ProductName, asset.PurchaseDate, asset.Note, asset.AssetTag, asset.Description, asset.Imei);

                var value = _mapper.Map<ViewModels.Asset>(updatedAsset);
                return Ok(value);
            }
            catch (InvalidAssetDataException ex)
            {
                _logger?.LogError("{0}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger?.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customer/{customerId:guid}/assign")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AssignAsset(Guid customerId, Guid assetId, [FromBody] AssignAssetToUser data)
        {
            var updatedAsset = await _assetServices.AssignAsset(customerId, assetId, data.UserId, data.CallerId);
            return Ok(updatedAsset);
        }

        [Route("categories")]
        [HttpGet]
        [ProducesResponseType(typeof(AssetCategory), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<AssetCategory>>> GetAssetCategories(bool hierarchical = false, string language = "EN")
        {
            try
            {
                if (language.Length != 2)
                {
                    return BadRequest("Language code is too long or too short.");
                }
                var assetCategories = _assetServices.GetAssetCategories(language);
                if (!hierarchical)
                    return assetCategories.Select(ac => new AssetCategory(ac)).ToList();

                IList<AssetCategory> results = assetCategories.Where(a => a.ParentAssetCategory == null).Select(ac => new AssetCategory(ac, assetCategories)).ToList();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
        }

        [Route("auditlog/{assetId:Guid}/{callerId:Guid}/{role}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<AssetAuditLog>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<AssetAuditLog>>> GetAssetAuditLog(Guid assetId, Guid callerId, string role)
        {
            try
            {
                // use asset id to find asset log: Mock example just takes any id and returns dummy data
                if (assetId == Guid.Empty)
                {
                    return NotFound();
                }

                var assetLogList = await _assetServices.GetAssetAuditLog(assetId, callerId, role);

                return Ok(assetLogList);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}