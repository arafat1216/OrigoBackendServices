using Asset.API.ViewModels;
using AssetServices;
using AssetServices.Exceptions;
using Common.Enums;
using Common.Exceptions;
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
using System.Net.Http;

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

        [Route("customers/{customerId:guid}/count")]
        [HttpGet]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<int>> GetCount(Guid customerId)
        {
            var count = await _assetServices.GetAssetsCountAsync(customerId);

            return Ok(count);
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
            var assetList = new List<object>();
            foreach (var asset in assets)
            {
                ViewModels.Asset assetToReturn;
                var phone = asset as AssetServices.Models.MobilePhone;
                var tablet = asset as AssetServices.Models.Tablet;

                if (phone != null)
                    assetToReturn = new MobilePhone(phone);
                else if (tablet != null)
                    assetToReturn = new Tablet(tablet);
                else
                    assetToReturn = new ViewModels.Asset(asset);
                assetList.Add(assetToReturn);
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            return Ok(JsonSerializer.Serialize<object>(assetList, options));
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
            catch (Exception ex)
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

        [Route("customers/{customerId:guid}/labels/delete")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> DeleteLabelsForCustomer(Guid customerId, [FromBody] DeleteCustomerLabelsData data)
        {
            try
            {
                var customerLabels = await _assetServices.GetCustomerLabelsAsync(data.LabelGuids);
                
                var labels = await _assetServices.SoftDeleteLabelsForCustomerAsync(customerId, data.CallerId, data.LabelGuids);

                IList<int> labelInts = new List<int>();
                foreach (AssetServices.Models.CustomerLabel label in customerLabels)
                {
                    labelInts.Add(label.Id);
                }

                await _assetServices.SoftDeleteAssetLabelsAsync(data.CallerId, labelInts);

                
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
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
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
            catch(ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
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
            try
            {
                IList<Guid> assetGuids = assetLabels.AssetGuids;
                IList<Guid> labelGuids = assetLabels.LabelGuids;

                IList<AssetServices.Models.Asset> assets = await _assetServices.AssignLabelsToAssetsAsync(customerId, assetLabels.CallerId, assetGuids, labelGuids);
                
                var assetList = new List<object>();
                foreach (var asset in assets)
                {
                    ViewModels.Asset assetToReturn;
                    var phone = asset as AssetServices.Models.MobilePhone;
                    var tablet = asset as AssetServices.Models.Tablet;

                    if (phone != null)
                        assetToReturn = new MobilePhone(phone);
                    else if (tablet != null)
                        assetToReturn = new Tablet(tablet);
                    else
                        assetToReturn = new ViewModels.Asset(asset);
                    assetList.Add(assetToReturn);
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(assetList, options));

            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Route("customers/{customerId:guid}/labels/unassign/{callerId:guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.Label>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> UnAssignLabelsToAssets(Guid customerId, Guid callerId, [FromBody] AssetLabels assetLabels)
        {
            try
            {
                IList<Guid> assetGuids = assetLabels.AssetGuids;
                IList<Guid> labelGuids = assetLabels.LabelGuids;

                IList<AssetServices.Models.Asset> assets = await _assetServices.UnAssignLabelsToAssetsAsync(customerId, callerId, assetGuids, labelGuids);
                if (assets == null)
                    return NotFound("No assets with given Ids where found. Did you enter the correct customerId?");
                var assetList = new List<object>();
                foreach (var asset in assets)
                {
                    ViewModels.Asset assetToReturn;
                    var phone = asset as AssetServices.Models.MobilePhone;
                    var tablet = asset as AssetServices.Models.Tablet;

                    if (phone != null)
                        assetToReturn = new MobilePhone(phone);
                    else if (tablet != null)
                        assetToReturn = new Tablet(tablet);
                    else
                        assetToReturn = new ViewModels.Asset(asset);
                    assetList.Add(assetToReturn);
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
                return Ok(JsonSerializer.Serialize<object>(assetList, options));

            }
            catch(ResourceNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
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

            var assetList = new List<object>();
            foreach (var asset in pagedAssetResult.Items)
            {
                ViewModels.Asset assetToReturn;
                var phone = asset as AssetServices.Models.MobilePhone;
                var tablet = asset as AssetServices.Models.Tablet;

                if (phone != null)
                    assetToReturn = new MobilePhone(phone);
                else if (tablet != null)
                    assetToReturn = new Tablet(tablet);
                else
                    assetToReturn = new ViewModels.Asset(asset);
                assetList.Add(assetToReturn);
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };
            return Ok(JsonSerializer.Serialize<object>(
            new PagedAssetList
            {
                CurrentPage = pagedAssetResult.CurrentPage,
                TotalItems = pagedAssetResult.TotalItems,
                TotalPages = pagedAssetResult.TotalPages,
                Assets = assetList
            }, options));
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ViewModels.Asset>> Get(Guid customerId, Guid assetId)
        {
            var asset = await _assetServices.GetAssetForCustomerAsync(customerId, assetId);
            if (asset == null)
            {
                return NotFound();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };

            var phone = asset as AssetServices.Models.MobilePhone;
            if (phone != null)
                return Ok(JsonSerializer.Serialize<object>(new MobilePhone(phone), options));

            var tablet = asset as AssetServices.Models.Tablet;
            if (tablet != null)
                return Ok(JsonSerializer.Serialize<object>(new Tablet(tablet), options));

            return Ok(JsonSerializer.Serialize<object>(new ViewModels.Asset(asset), options));
        }

        [Route("customers/{customerId:guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateAsset(Guid customerId, [FromBody] NewAsset asset)
        {
            try
            {
                if (!Enum.IsDefined(typeof(AssetStatus), asset.AssetStatus))
                {
                    Array arr = Enum.GetValues(typeof(AssetStatus));
                    StringBuilder errorMessage = new StringBuilder(string.Format("The given value for asset status: {0} is out of bounds.\nValid options for asset statuses are:\n", asset.AssetStatus));
                    foreach (AssetStatus e in arr)
                    {
                        errorMessage.Append($"    -{(int)e} ({e})\n");
                    }
                    throw new InvalidAssetDataException(errorMessage.ToString());
                }

                var updatedAsset = await _assetServices.AddAssetForCustomerAsync(customerId, asset.CallerId,  asset.Alias, asset.SerialNumber,
                    asset.AssetCategoryId, asset.Brand, asset.ProductName, asset.LifecycleType, asset.PurchaseDate,
                    asset.AssetHolderId, asset.Imei, asset.MacAddress, asset.ManagedByDepartmentId, (AssetStatus)asset.AssetStatus, asset.Note, asset.AssetTag, asset.Description);

                var phone = updatedAsset as AssetServices.Models.MobilePhone;
                if (phone != null)
                    return CreatedAtAction(nameof(CreateAsset), new { id = phone.ExternalId }, new MobilePhone(phone));

                var tablet = updatedAsset as AssetServices.Models.Tablet;
                if (tablet != null)
                    return CreatedAtAction(nameof(CreateAsset), new { id = tablet.ExternalId }, new Tablet(tablet));

                var updatedAssetView = new ViewModels.Asset(updatedAsset);

                return CreatedAtAction(nameof(CreateAsset), new { id = updatedAssetView.Id }, updatedAssetView);
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


        [Route("customers/{customerId:guid}/assetStatus")]
        [HttpPost]
        [ProducesResponseType(typeof(IList<ViewModels.Asset>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> SetAssetStatusOnAssets(Guid customerId, [FromBody] UpdateAssetsStatus data)
        {
            try
            {
                if (!Enum.IsDefined(typeof(AssetStatus), data.AssetStatus))
                {
                    string statusString = "";
                    foreach (int i in Enum.GetValues(typeof(AssetStatus)))
                    {
                        statusString += i + " - " + Enum.GetName(typeof(AssetStatus), i) + "\n";
                    }
                    return BadRequest("Invalid AssetStatus, possible values are: " + statusString);
                }

                var updatedAssets = await _assetServices.UpdateMultipleAssetsStatus(customerId, data.CallerId, data.AssetGuidList, (AssetStatus)data.AssetStatus);
                if (updatedAssets == null)
                {
                    return NotFound("Given organization does not exist or none of the assets were found");
                }

                var assetList = new List<object>();
                foreach (var asset in updatedAssets)
                {
                    ViewModels.Asset assetToReturn;
                    var phone = asset as AssetServices.Models.MobilePhone;
                    var tablet = asset as AssetServices.Models.Tablet;

                    if (phone != null)
                        assetToReturn = new MobilePhone(phone);
                    else if (tablet != null)
                        assetToReturn = new Tablet(tablet);
                    else
                        assetToReturn = new ViewModels.Asset(asset);
                    assetList.Add(assetToReturn);
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };

                return Ok(JsonSerializer.Serialize<object>(assetList, options));

            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }



        [Route("lifecycles")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<AssetLifecycle>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public ActionResult GetLifecycles()
        {
            try
            {
                var lifecycles = _assetServices.GetLifecycles();
                if (lifecycles == null)
                {
                    return NotFound();
                }
                var lifecycleList = new List<AssetLifecycle>();
                foreach (var lifecycle in lifecycles) lifecycleList.Add(new AssetLifecycle() { Name = lifecycle.Name, EnumValue = lifecycle.EnumValue });

                return Ok(lifecycleList);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{assetId:Guid}/customers/{customerId:guid}/ChangeLifecycleType")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> ChangeLifecycleTypeOnAsset(Guid customerId, Guid assetId, [FromBody] UpdateAssetLifecycleType data)
        {
            try
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
                LifecycleType lifecycleType = (LifecycleType)data.LifecycleType;
                var updatedAsset = await _assetServices.ChangeAssetLifecycleTypeForCustomerAsync(customerId, data.AssetId, data.CallerId, lifecycleType);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                var phone = updatedAsset as AssetServices.Models.MobilePhone;
                if (phone != null)
                    return Ok(new MobilePhone(phone));

                var tablet = updatedAsset as AssetServices.Models.Tablet;
                if (tablet != null)
                    return Ok(new Tablet(tablet));

                return Ok(new ViewModels.Asset(updatedAsset));
            }
            catch (InvalidLifecycleTypeException ex)
            {
                return UnprocessableEntity(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("{0}", ex.Message);
                return BadRequest();
            }
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
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                var phone = updatedAsset as AssetServices.Models.MobilePhone;
                if (phone != null)
                    return Ok(new MobilePhone(phone));

                var tablet = updatedAsset as AssetServices.Models.Tablet;
                if (tablet != null)
                    return Ok(new Tablet(tablet));

                return Ok(new ViewModels.Asset(updatedAsset));
            }
            catch(InvalidAssetDataException ex)
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

        [Route("{assetId:Guid}/customer/{customerId:guid}/{callerId:guid}/assign/")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> AssignAsset(Guid customerId, Guid assetId, [FromBody] AssignAssetToUser data)
        {
            try
            {
                var updatedAsset = await _assetServices.AssignAsset(customerId, assetId, data.UserId, data.CallerId);
                if (updatedAsset == null)
                {
                    return NotFound();
                }

                var phone = updatedAsset as AssetServices.Models.MobilePhone;
                if (phone != null)
                    return Ok(new MobilePhone(phone));

                var tablet = updatedAsset as AssetServices.Models.Tablet;
                if (tablet != null)
                    return Ok(new Tablet(tablet));

                return Ok(new ViewModels.Asset(updatedAsset));

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("categories")]
        [HttpGet]
        [ProducesResponseType(typeof(AssetCategory), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<AssetCategory>>> GetAssetCategories(bool hierarchical = false, string language = "EN")
        {
            try
            {
                var assetCategories = await _assetServices.GetAssetCategoriesAsync(language);
                if (assetCategories == null)
                {
                    return NotFound();
                }
                else if (language.Length != 2)
                {
                    return BadRequest("Language code is too long or too short.");
                }
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

        [Route("auditlog/{assetId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<AssetAuditLog>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<AssetAuditLog>>> GetAssetAuditLogMock(Guid assetId)
        {
            try
            {
                // use asset id to find asset log: Mock example just takes any id and returns dummy data
                if (assetId == Guid.Empty)
                {
                    return NotFound();
                }

                var assetLogList = await _assetServices.GetAssetAuditLog(assetId);

                return Ok(assetLogList);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }
    }
}