using Asset.API.Filters;
using Asset.API.ViewModels;
using AssetServices;
using AssetServices.Exceptions;
using AssetServices.Models;
using AssetServices.ServiceModel;
using AutoMapper;
using Common.Enums;
using Common.Extensions;
using Common.Interfaces;
using Common.Model.EventModels;
using Common.Models;
using Dapr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AssetCategory = Asset.API.ViewModels.AssetCategory;
using AssetLifecycleType = Asset.API.ViewModels.AssetLifecycleType;
using DisposeSetting = Asset.API.ViewModels.DisposeSetting;
using Label = Asset.API.ViewModels.Label;
using LifeCycleSetting = Asset.API.ViewModels.LifeCycleSetting;
using ReturnLocation = Asset.API.ViewModels.ReturnLocation;

namespace Asset.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
// Assets should only be available through a given customer
[Route("api/v{version:apiVersion}/[controller]")]
[ServiceFilter(typeof(ErrorExceptionFilter))]
[SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
[SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
public class AssetsController : ControllerBase
{
    private readonly IAssetServices _assetServices;
    private readonly IMapper _mapper;
    private readonly IFeatureManager _featureManager;

    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(ILogger<AssetsController> logger, IAssetServices assetServices, IMapper mapper,
        IFeatureManager featureManager)
    {
        _logger = logger;
        _assetServices = assetServices;
        _mapper = mapper;
        _featureManager = featureManager;
    }

    /// <summary>
    /// Get a count of all assets per customer in pagedModel. If customerIds given it will be filtered on those.
    /// We need to use POST since the number of customerIds may be too long for it to be a GET query command.
    /// </summary>
    /// <param name="role"></param>
    /// <param name="customerIds"></param>
    /// <returns></returns>
    [Route("customers/count/pagination")]
    [HttpPost]
    [ProducesResponseType(typeof(PagedModel<CustomerAssetCount>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<PagedModel<CustomerAssetCount>>> GetAllCount(CancellationToken cancellationToken, [FromBody] List<Guid>? customerIds = null, [FromQuery] int page = 1, [FromQuery] int limit = 25)
    {
        var assetCountList = await _assetServices.GetAllCustomerAssetsCountAsync(customerIds ?? new List<Guid>(), page, limit,
            cancellationToken);
        return Ok(assetCountList);
    }

    [Obsolete("Should be removed when frontend adapt to pagination")]
    [Route("customers/count")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<CustomerAssetCount>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IList<CustomerAssetCount>>> GetAllCount([FromBody] List<Guid>? customerIds = null)
    {
        var assetCountList = await _assetServices.GetAllCustomerAssetsCountAsync(customerIds ?? new List<Guid>());
        return Ok(assetCountList);
    }

    [Route("customers/{customerId:guid}/count")]
    [HttpGet]
    [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<int>> GetCount([FromRoute] Guid customerId, [FromQuery] Guid? departmentId,
        [FromQuery] AssetLifecycleStatus? assetLifecycleStatus)
    {
        var count = await _assetServices.GetAssetsCountAsync(customerId, assetLifecycleStatus, departmentId);

        return Ok(count);
    }

    [Route("customers/{customerId:guid}/total-book-value")]
    [HttpGet]
    [ProducesResponseType(typeof(decimal), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<decimal>> GetCustomerTotalBookValue([FromRoute] Guid customerId)
    {
        var totalBookValue = await _assetServices.GetCustomerTotalBookValue(customerId);

        return Ok(totalBookValue);
    }


    [Route("customers/{customerId:guid}/users/{userId:Guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(IList<ViewModels.Asset>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> GetAssetsForUser([FromRoute] Guid customerId, [FromRoute] Guid userId,
        [FromQuery] bool includeAsset = true, bool includeImeis = true, bool includeContractHolderUser = true)
    {
        var assetLifecycles = await _assetServices.GetAssetLifecyclesForUserAsync(customerId, userId, includeAsset, includeImeis, includeContractHolderUser);

        return Ok(_mapper.Map<IList<ViewModels.Asset>>(assetLifecycles));
    }

    [Route("customers/{customerId:guid}/users/{userId:Guid}")]
    [HttpPatch]
    public async Task<ActionResult> UnAssignAssetsFromUser([FromRoute] Guid customerId, [FromRoute] Guid userId,
        [FromBody] UnAssignAssetToUser data)
    {
        await _assetServices.UnAssignAssetLifecyclesForUserAsync(customerId, userId, data.DepartmentId, data.CallerId);
        return Accepted();
    }


    [Route("customers/{customerId:guid}/labels")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<Label>>> CreateLabelsForCustomer([FromRoute] Guid customerId,
        [FromBody] AddLabelsData data)
    {
        var labels = _mapper.Map<IList<AssetServices.Models.Label>>(data.NewLabels);

        var labelsAdded = await _assetServices.AddLabelsForCustomerAsync(customerId, data.CallerId, labels);

        if (labelsAdded == null)
        {
            return BadRequest("Unable to add labels.");
        }

        var labelsView = new List<object>();
        foreach (var label in labelsAdded)
        {
            labelsView.Add(new Label(label));
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        return Ok(JsonSerializer.Serialize<object>(labelsView, options));
    }

    [Route("customers/{customerId:guid}/labels")]
    [HttpGet]
    [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> GetLabelsForCustomer([FromRoute] Guid customerId)
    {
        var labels = await _assetServices.GetCustomerLabelsForCustomerAsync(customerId);
        if (labels == null)
        {
            return NotFound("No labels found on customer. Did you enter the correct customerId?");
        }

        var labelList = new List<object>();
        foreach (var label in labels)
        {
            labelList.Add(new Label(label));
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
    public async Task<ActionResult> DeleteLabelsForCustomer([FromRoute] Guid customerId,
        [FromBody] DeleteCustomerLabelsData customerLabelsData)
    {
        var assetList = await _assetServices.DeleteLabelsForCustomerAsync(customerId, customerLabelsData.CallerId,
            customerLabelsData.LabelGuids);
        return Ok(_mapper.Map<IList<Label>>(assetList));
    }

    [Route("customers/{customerId:guid}/labels/update")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<Label>>> UpdateLabelsForCustomer([FromRoute] Guid customerId,
        [FromBody] UpdateCustomerLabelsData data)
    {
        IList<CustomerLabel> customerLabels = new List<CustomerLabel>();

        foreach (var label in data.Labels)
        {
            customerLabels.Add(new CustomerLabel(label.Id, customerId, data.CallerId,
                new AssetServices.Models.Label(label.Text, label.Color)));
        }

        var updatedLabels = await _assetServices.UpdateLabelsForCustomerAsync(customerId, customerLabels);
        var labelList = new List<object>();
        foreach (var label in updatedLabels)
        {
            labelList.Add(new Label(label));
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        return Ok(JsonSerializer.Serialize<object>(labelList, options));
    }

    [Route("customers/{customerId:guid}/labels/assign")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> AssignLabelsToAssets([FromRoute] Guid customerId,
        [FromBody] AssetLabels assetLabels)
    {
        var assets = await _assetServices.AssignLabelsToAssetsAsync(customerId, assetLabels.CallerId,
            assetLabels.AssetGuids, assetLabels.LabelGuids);
        return Ok(_mapper.Map<IList<ViewModels.Asset>>(assets));
    }

    [Route("customers/{customerId:guid}/labels/unassign")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<ViewModels.Asset>>> UnAssignLabelsToAssets([FromRoute] Guid customerId,
        [FromBody] AssetLabels assetLabels)
    {
        var assets = await _assetServices.UnAssignLabelsToAssetsAsync(customerId, assetLabels.CallerId,
            assetLabels.AssetGuids, assetLabels.LabelGuids);
        return Ok(_mapper.Map<IList<ViewModels.Asset>>(assets));
    }

    [Route("customers/{customerId:guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(PagedAssetList), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<PagedAssetList>> Get([FromRoute] Guid customerId, CancellationToken cancellationToken,
        [FromQuery(Name = "q")] string? search, int page = 1, int limit = 25,
        [FromQuery(Name = "filterOptions")] string? filterOptionsAsJsonString = null,
        [FromQuery] bool includeAsset = true, bool includeImeis = true, bool includeLabels = true, bool includeContractHolderUser = true)
    {
        FilterOptionsForAsset? filterOptions = null;
        if (!string.IsNullOrEmpty(filterOptionsAsJsonString))
        {
            filterOptions = JsonSerializer.Deserialize<FilterOptionsForAsset>(filterOptionsAsJsonString);
        }

        var pagedAssetResult = await _assetServices.GetAssetLifecyclesForCustomerAsync(customerId,
            filterOptions?.UserId, filterOptions?.Status, filterOptions?.Department, filterOptions?.Category,
            filterOptions?.Label, filterOptions?.IsActiveState, filterOptions?.IsPersonal,
            filterOptions?.EndPeriodMonth, filterOptions?.purchaseMonth, search ?? string.Empty, page, limit,
            cancellationToken, includeAsset, includeImeis, includeLabels, includeContractHolderUser);
        var pagedAssetList = _mapper.Map<PagedAssetList>(pagedAssetResult);
        return Ok(pagedAssetList);
    }

    [Route("{assetId:Guid}/customers/{customerId:guid}")]
    [HttpGet]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<ViewModels.Asset>> Get([FromRoute] Guid customerId, [FromRoute] Guid assetId,
        [FromQuery(Name = "filterOptions")] string? filterOptionsAsJsonString = null,
        [FromQuery] bool includeAsset = true, bool includeImeis = true, bool includeLabels = true, bool includeContractHolderUser = true)
    {
        FilterOptionsForAsset? filterOptions = null;
        if (!string.IsNullOrEmpty(filterOptionsAsJsonString))
        {
            filterOptions = JsonSerializer.Deserialize<FilterOptionsForAsset>(filterOptionsAsJsonString);
        }

        var assetLifecycle = await _assetServices.GetAssetLifecycleForCustomerAsync(customerId, assetId,
            filterOptions?.UserId, filterOptions?.Department, includeAsset, includeImeis, includeLabels, includeContractHolderUser);
        return Ok(_mapper.Map<ViewModels.Asset>(assetLifecycle));
    }

    [Route("customers/{customerId:guid}/lifecycle-setting")]
    [HttpGet]
    [ProducesResponseType(typeof(IList<LifeCycleSetting>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GetLifeCycleSetting([FromRoute] Guid customerId)
    {
        var setting = await _assetServices.GetLifeCycleSettingByCustomer(customerId);
        if (setting == null)
        {
            return Ok(new object());
        }

        return Ok(_mapper.Map<IList<LifeCycleSetting>>(setting));
    }

    [Route("customers/{customerId:guid}/lifecycle-setting")]
    [HttpPost]
    [ProducesResponseType(typeof(LifeCycleSetting), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CreateLifeCycleSetting([FromRoute] Guid customerId, [FromBody] NewLifeCycleSetting setting)
    {
        var newSettingDTO = _mapper.Map<LifeCycleSettingDTO>(setting);
        var createdSetting =
            await _assetServices.AddLifeCycleSettingForCustomerAsync(customerId, newSettingDTO, setting.CallerId);
        return CreatedAtAction(nameof(CreateLifeCycleSetting), new { id = createdSetting.ExternalId },
            _mapper.Map<LifeCycleSetting>(createdSetting));
    }

    [Route("customers/{customerId:guid}/lifecycle-setting")]
    [HttpPut]
    [ProducesResponseType(typeof(LifeCycleSetting), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> UpdateLifeCycleSetting([FromRoute] Guid customerId, [FromBody] NewLifeCycleSetting setting)
    {
        var newSettingDTO = _mapper.Map<LifeCycleSettingDTO>(setting);
        var updatedSetting =
            await _assetServices.UpdateLifeCycleSettingForCustomerAsync(customerId, newSettingDTO, setting.CallerId);
        return Ok(_mapper.Map<LifeCycleSetting>(updatedSetting));
    }

    [Route("customers/{customerId:guid}/dispose-setting")]
    [HttpGet]
    [ProducesResponseType(typeof(DisposeSetting), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GetDisposeSetting([FromRoute] Guid customerId)
    {
        var setting = await _assetServices.GetDisposeSettingByCustomer(customerId);
        if (setting == null)
        {
            return Ok(new DisposeSetting());
        }

        return Ok(_mapper.Map<DisposeSetting>(setting));
    }

    [Route("customers/{customerId:guid}/dispose-setting")]
    [HttpPost]
    [ProducesResponseType(typeof(DisposeSetting), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CreateDisposeSetting([FromRoute] Guid customerId, [FromBody] NewDisposeSetting setting)
    {
        var newSettingDTO = _mapper.Map<DisposeSettingDTO>(setting);
        var createdSetting =
            await _assetServices.AddDisposeSettingForCustomerAsync(customerId, newSettingDTO, setting.CallerId);
        return CreatedAtAction(nameof(CreateDisposeSetting), new { id = createdSetting.ExternalId },
            _mapper.Map<DisposeSetting>(createdSetting));
    }

    [Route("customers/{customerId:guid}/dispose-setting")]
    [HttpPut]
    [ProducesResponseType(typeof(DisposeSetting), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> UpdateDisposeSetting([FromRoute] Guid customerId, [FromBody] NewDisposeSetting setting)
    {
        var newSettingDTO = _mapper.Map<DisposeSettingDTO>(setting);
        var updatedSetting =
            await _assetServices.UpdateDisposeSettingForCustomerAsync(customerId, newSettingDTO, setting.CallerId);
        return Ok(_mapper.Map<DisposeSetting>(updatedSetting));
    }

    [Route("customers/{customerId:guid}/return-location")]
    [HttpGet]
    [ProducesResponseType(typeof(IList<ReturnLocation>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GetReturnLocations([FromRoute] Guid customerId)
    {
        var locations = await _assetServices.GetReturnLocationsByCustomer(customerId);
        return Ok(_mapper.Map<IList<ReturnLocation>>(locations));
    }

    [Route("customers/{customerId:guid}/return-location")]
    [HttpPost]
    [ProducesResponseType(typeof(ReturnLocation), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> AddReturnLocation([FromRoute] Guid customerId, [FromBody] NewReturnLocation data)
    {
        var newLocationDTO = _mapper.Map<ReturnLocationDTO>(data);
        var addedLocation =
            await _assetServices.AddReturnLocationsByCustomer(customerId, newLocationDTO, data.CallerId);
        return Ok(_mapper.Map<ReturnLocation>(addedLocation));
    }

    [Route("customers/{customerId:guid}/return-location/{returnLocationId:guid}")]
    [HttpPut]
    [ProducesResponseType(typeof(ReturnLocation), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> UpdateReturnLocation([FromRoute] Guid customerId, [FromRoute] Guid returnLocationId,
        [FromBody] NewReturnLocation data)
    {
        var newLocationDTO = _mapper.Map<ReturnLocationDTO>(data);
        var updatedLocations =
            await _assetServices.UpdateReturnLocationsByCustomer(customerId, returnLocationId, newLocationDTO,
                data.CallerId);
        return Ok(_mapper.Map<ReturnLocation>(updatedLocations));
    }

    [Route("customers/{customerId:guid}/return-location/{returnLocationId:guid}")]
    [HttpDelete]
    [ProducesResponseType(typeof(IList<ReturnLocation>), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> RemoveReturnLocation([FromRoute] Guid customerId, [FromRoute] Guid returnLocationId)
    {
        var updatedLocations =
            await _assetServices.RemoveReturnLocationsByCustomer(customerId, returnLocationId, Guid.Empty);
        return Ok(_mapper.Map<IList<ReturnLocation>>(updatedLocations));
    }

    [Route("customers/{customerId:guid}")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CreateAsset([FromRoute] Guid customerId, [FromBody] NewAsset asset)
    {
        var newAssetDTO = _mapper.Map<NewAssetDTO>(asset);
        var updatedAsset = await _assetServices.AddAssetLifecycleForCustomerAsync(customerId, newAssetDTO);
        return CreatedAtAction(nameof(CreateAsset), new { id = updatedAsset.ExternalId },
            _mapper.Map<ViewModels.Asset>(updatedAsset));
    }


    [Route("customers/{customerId:guid}/assetStatus/{assetStatus:int}")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<ViewModels.Asset>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> SetAssetStatusOnAssets([FromRoute] Guid customerId, [FromBody] UpdateAssetsStatus data,
        int assetStatus)
    {
        var updatedAssets = await _assetServices.UpdateStatusForMultipleAssetLifecycles(customerId, data.CallerId,
            data.AssetGuidList, (AssetLifecycleStatus)assetStatus);
        return Ok(_mapper.Map<IList<ViewModels.Asset>>(updatedAssets));
    }

    [Route("customers/{customerId:guid}/make-available")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> MakeAssetAvailable([FromRoute] Guid customerId, [FromBody] MakeAssetAvailable data)
    {
        var dataDTO = _mapper.Map<MakeAssetAvailableDTO>(data);
        var updatedAssets = await _assetServices.MakeAssetAvailableAsync(customerId, dataDTO);
        return Ok(_mapper.Map<ViewModels.Asset>(updatedAssets));
    }

    [Route("customers/{customerId:guid}/make-expire")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> MakeAssetExpired([FromRoute] Guid customerId, [FromBody] MakeAssetExpired data)
    {
        var updatedAssets =
            await _assetServices.MakeAssetExpiredAsync(customerId, data.AssetLifeCycleId, data.CallerId);
        return Ok(_mapper.Map<ViewModels.Asset>(updatedAssets));
    }

    [Route("customers/{customerId:guid}/make-expiressoon")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> MakeAssetExpiresSoon([FromRoute] Guid customerId, [FromBody] MakeAssetExpired data)
    {
        var updatedAssets =
            await _assetServices.MakeAssetExpiresSoonAsync(customerId, data.AssetLifeCycleId, data.CallerId);
        return Ok(_mapper.Map<ViewModels.Asset>(updatedAssets));
    }

    [Route("customers/{customerId:guid}/return-device")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> ReturnDeviceAsync([FromRoute] Guid customerId, [FromBody] ReturnDevice data)
    {
        var dataDTO = _mapper.Map<ReturnDeviceDTO>(data);
        var updatedAssets = await _assetServices.ReturnDeviceAsync(customerId, dataDTO);
        return Ok(_mapper.Map<ViewModels.Asset>(updatedAssets));
    }

    [Route("customers/{customerId:guid}/buyout-device")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> BuyoutDeviceAsync([FromRoute] Guid customerId, [FromBody] BuyoutDeviceDTO data)
    {
        var updatedAssets = await _assetServices.BuyoutDeviceAsync(customerId, data);
        return Ok(_mapper.Map<ViewModels.Asset>(updatedAssets));
    }

    [Route("customers/{customerId:guid}/pending-buyout")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> PendingBuyoutDeviceAsync([FromRoute] Guid customerId, [FromBody] PendingBuyoutDeviceDTO data)
    {
        var updatedAssets = await _assetServices.PendingBuyoutDeviceAsync(customerId, data);
        return Ok(_mapper.Map<ViewModels.Asset>(updatedAssets));
    }

    [Route("customers/{customerId:guid}/confirm-buyout")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> ConfirmPendingBuyoutDeviceAsync([FromRoute] Guid customerId, [FromBody] BuyoutDeviceDTO data)
    {
        var updatedAssets = await _assetServices.ConfirmBuyoutDeviceAsync(customerId, data);
        return Ok(_mapper.Map<ViewModels.Asset>(updatedAssets));
    }

    [Route("customers/{customerId:guid}/report-device")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> ReportDeviceAsync([FromRoute] Guid customerId, [FromBody] ReportDevice data)
    {
        var dataDTO = _mapper.Map<ReportDeviceDTO>(data);
        var updatedAssets = await _assetServices.ReportDeviceAsync(customerId, dataDTO);
        return Ok(_mapper.Map<ViewModels.Asset>(updatedAssets));
    }

    [Route("{assetId:Guid}/customers/{customerId:guid}/re-assignment")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> ReAssignAssetLifeCycleToHolder([FromRoute] Guid customerId, [FromRoute] Guid assetId,
        [FromBody] ReAssignAsset postData)
    {
        if ((postData.Personal && (postData.DepartmentId == Guid.Empty || postData.UserId == Guid.Empty)) ||
            (!postData.Personal && postData.DepartmentId == Guid.Empty)) return BadRequest();

        var dataDTO = _mapper.Map<AssignAssetDTO>(postData);
        var updatedAsset = await _assetServices.AssignAssetLifeCycleToHolder(customerId, assetId, dataDTO);
        var mapped = _mapper.Map<ViewModels.Asset>(updatedAsset);
        return Ok(mapped);
    }

    [Route("lifecycles")]
    [HttpGet]
    [ProducesResponseType(typeof(IList<AssetLifecycleType>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public ActionResult GetLifecycles()
    {
        var lifecycles = _assetServices.GetLifecycles();
        var lifecycleList = lifecycles.Where(l => l.EnumValue is 0 or 2).Select(lifecycle =>
            new AssetLifecycleType { Name = lifecycle.Name, EnumValue = lifecycle.EnumValue }).ToList();
        return Ok(lifecycleList);
    }

    [Route("min-buyout-price")]
    [HttpGet]
    [ProducesResponseType(typeof(IList<MinBuyoutPriceBaseline>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public ActionResult GetBaseMinBuyoutPrice([FromQuery] string? country, [FromQuery] int? assetCategoryId)
    {
        var baseBuyoutPrice = _assetServices.GetBaseMinBuyoutPrice(country, assetCategoryId);
        return Ok(baseBuyoutPrice);
    }

    [Route("{assetId:Guid}/customers/{customerId:guid}/ChangeLifecycleType")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.UnprocessableEntity)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> ChangeLifecycleTypeOnAsset([FromRoute] Guid customerId, [FromRoute] Guid assetId,
        [FromBody] UpdateAssetLifecycleType data)
    {
        // Check if given int is within valid range of values
        if (!Enum.IsDefined(typeof(LifecycleType), data.LifecycleType))
        {
            var arr = Enum.GetValues(typeof(LifecycleType));
            var errorMessage = new StringBuilder(string.Format(
                "The given value for lifecycle: {0} is out of bounds.\nValid options for lifecycle are:\n",
                data.LifecycleType));
            foreach (LifecycleType e in arr)
            {
                errorMessage.Append($"    -{(int)e} ({e})\n");
            }

            throw new InvalidLifecycleTypeException(errorMessage.ToString(), Guid.Parse(""));
        }

        var lifecycleType = (LifecycleType)data.LifecycleType;
        var updatedAsset =
            await _assetServices.ChangeAssetLifecycleTypeForCustomerAsync(customerId, data.AssetId, data.CallerId,
                lifecycleType);
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
    public async Task<IActionResult> UpdateAsset([FromRoute] Guid customerId, [FromRoute] Guid assetId, [FromBody] UpdateAsset asset)
    {
        var updatedAsset = await _assetServices.UpdateAssetAsync(customerId, assetId, asset.CallerId, asset?.Alias,
            asset?.SerialNumber, asset?.Brand, asset?.ProductName, asset?.PurchaseDate, asset?.Note, asset?.AssetTag,
            asset?.Description, asset?.Imei, asset?.MacAddress);

        var value = _mapper.Map<ViewModels.Asset>(updatedAsset);
        return Ok(value);
    }

    [Route("{assetId:Guid}/customer/{customerId:guid}/assign")]
    [HttpPost]
    [ProducesResponseType(typeof(ViewModels.Asset), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> AssignAssetLifeCycleToHolder([FromRoute] Guid customerId, [FromRoute] Guid assetId,
        [FromBody] AssignAssetToUser asset)
    {
        if ((asset.UserId == Guid.Empty && asset.DepartmentId == Guid.Empty) ||
            (asset.UserId != Guid.Empty && asset.DepartmentId != Guid.Empty)) return BadRequest();

        var assignAssetDTO = new AssignAssetDTO
        {
            UserId = asset.UserId,
            UserAssigneToDepartment = asset.UserAssigneToDepartment,
            DepartmentId = asset.DepartmentId,
            Personal = asset.UserId != Guid.Empty,
            CallerId = asset.CallerId
        };
        var updatedAsset = await _assetServices.AssignAssetLifeCycleToHolder(customerId, assetId, assignAssetDTO);
        var mapped = _mapper.Map<ViewModels.Asset>(updatedAsset);
        return Ok(mapped);
    }

    [Route("categories")]
    [HttpGet]
    [ProducesResponseType(typeof(AssetCategory), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public ActionResult<IEnumerable<AssetCategory>> GetAssetCategories([FromQuery] bool hierarchical = false,
        [FromQuery] string language = "EN")
    {
        if (language.Length != 2)
        {
            return BadRequest("Language code is too long or too short.");
        }

        var assetCategories = _assetServices.GetAssetCategories(language);
        if (!hierarchical)
        {
            return assetCategories.Select(ac => new AssetCategory(ac)).ToList();
        }

        IList<AssetCategory> results = assetCategories.Where(a => a.ParentAssetCategory == null)
            .Select(ac => new AssetCategory(ac, assetCategories)).ToList();
        return Ok(results);
    }

    [Route("auditlog/{assetId:Guid}/{callerId:Guid}/{role}")]
    [HttpGet]
    [ProducesResponseType(typeof(IList<AssetAuditLog>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<IEnumerable<AssetAuditLog>>> GetAssetAuditLog([FromRoute] Guid assetId, [FromRoute] Guid callerId,
        [FromRoute] string role)
    {
        // use asset id to find asset log: Mock example just takes any id and returns dummy data
        if (assetId == Guid.Empty)
        {
            return NotFound();
        }

        var assetLogList = await _assetServices.GetAssetAuditLog(assetId, callerId, role);

        return Ok(assetLogList);
    }

    [Route("{assetLifecycleId:Guid}/repair-completed")]
    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> RepairCompleted([FromRoute] Guid assetLifecycleId, [FromBody] AssetLifeCycleRepairCompleted assetLifeCycle)
    {
        if (assetLifecycleId == Guid.Empty)
        {
            return BadRequest("AssetLifeCycleRepairCompleted assetLifecycleId is a empty Guid");
        }

        await _assetServices.AssetLifeCycleRepairCompleted(assetLifecycleId, assetLifeCycle);

        return Ok();
    }

    [Route("{assetLifecycleId:Guid}/send-to-repair")]
    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> SendToRepair([FromRoute] Guid assetLifecycleId, [FromBody] Guid callerId)
    {
        if (assetLifecycleId == Guid.Empty)
        {
            return BadRequest("AssetLifeCycleChangeStatusToRepair assetLifecycleId is a empty Guid");
        }

        await _assetServices.AssetLifeCycleSendToRepair(assetLifecycleId, callerId);

        return Ok();
    }

    [Route("customers/{customerId:guid}/assets-counter")]
    [HttpGet]
    [ProducesResponseType(typeof(CustomerAssetsCounter), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<CustomerAssetsCounter>> GetAssetLifecycleCounters([FromRoute] Guid customerId,
        [FromQuery(Name = "filter")] string? json = null)
    {
        var filterOptions = new FilterOptionsForAsset();


        try
        {
            if (json != null) filterOptions = JsonSerializer.Deserialize<FilterOptionsForAsset>(json);
        }
        catch (JsonException ex)
        {
            _logger?.LogError("GetCustomerAssetsCount returns JsonException", ex.Path);
            return BadRequest($"GetCustomerAssetsCount returns JsonException with message {ex.Path}");
        }


        Guid.TryParse(filterOptions?.UserId, out var userId);

        var assetCounter = await _assetServices.GetAssetLifecycleCountersAsync(customerId, filterOptions?.Status,
            filterOptions?.Department, userId);

        return Ok(assetCounter);
    }

    [Route("customers/{customerId:guid}/activate")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<ViewModels.Asset>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IList<ViewModels.Asset>>> ActivateAssetStatusOnAssetLifecycle([FromRoute] Guid customerId,
        [FromBody] ChangeAssetStatus assetLifecycle)
    {
        var activateAssets = await _assetServices.ActivateAssetLifecycleStatus(customerId, assetLifecycle);
        return Ok(_mapper.Map<IList<ViewModels.Asset>>(activateAssets));
    }

    [Route("customers/{customerId:guid}/deactivate")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<ViewModels.Asset>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<IList<ViewModels.Asset>>> DeactivateAssetStatusOnAssetLifecycle([FromRoute] Guid customerId,
        [FromBody] ChangeAssetStatus assetLifecycle)
    {
        var activateAssets = await _assetServices.DeactivateAssetLifecycleStatus(customerId, assetLifecycle);
        return Ok(_mapper.Map<IList<ViewModels.Asset>>(activateAssets));
    }

    [Route("customers/{customerId:guid}/import")]
    [HttpPost]
    [ProducesResponseType(typeof(IList<Label>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> ImportAssets([FromRoute] Guid customerId,
        [FromForm] IFormFile assetImportFile, [FromQuery] LifecycleType lifeCycleType, [FromQuery] bool validateOnly = true)
    {
        if (assetImportFile.Length <= 0)
        {
            return BadRequest("Empty file");
        }

        return Ok(await _assetServices.ImportAssetsFromFile(customerId, assetImportFile, validateOnly, lifeCycleType));
    }

    [Topic("customer-pub-sub", "offboard-cancelled")]
    [HttpPost("customers/cancel-employee-offboarding")]
    public async Task CancelUserOffboarding([FromBody] UserEvent userEvent)
    {
        await _assetServices.CancelUserOffboarding(userEvent.CustomerId, userEvent.UserId, Guid.Empty.SystemUserId());
    }
    /// <summary>
    /// Responsible for updating the asset lifecycle based on the requirements for when a return order gets cancelled.
    /// Based on other assets lifecycle values the assets lifecycle status gets set to the right active state. Also checks if the asset lifecycle has expired or is expiring soon based on its end period. 
    /// </summary>
    /// <param name="assetLifecycleId">The id of the assetlifecycle to be updated.</param>
    /// <param name="callerId">The id of the user making the request.</param>
    /// <returns>Returns a ActionResult.The ActionResult types represent various HTTP status codes.</returns>
    [Route("{assetLifecycleId:Guid}/cancel-return")]
    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> CancelReturn([FromRoute] Guid assetLifecycleId, [FromBody] Guid callerId)
    {
        if (assetLifecycleId == Guid.Empty)
        {
            return BadRequest("CancelReturn assetLifecycleId is a empty Guid");
        }

        await _assetServices.CancelReturn(assetLifecycleId, callerId);

        return Ok();
    }

    /// <summary>
    /// Responsible for updating the asset lifecycle based on the requirements for a recycle order.
    /// Sets assets staus "Pending recycle" or "Recycled" based on the status being passed in the parameter. 
    /// </summary>
    /// <param name="assetLifecycleId">The id of the assetlifecycle to be updated.</param>
    /// <param name="assetLifecycle">Additional information to be able to recycle a asset lifecycle. Need the status that the asset lifecycle should be changed to and the caller id.</param>
    /// <returns>Returns a ActionResult.The ActionResult types represent various HTTP status codes.</returns>
    [Route("{assetLifecycleId:Guid}/recycle")]
    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> RecycleAssetLifecycle([FromRoute] Guid assetLifecycleId, [FromBody] RecycleAssetLifecycle assetLifecycle)
    {
        if (assetLifecycleId == Guid.Empty)
        {
            return BadRequest("RecycleAssetLifecycle assetLifecycleId is a empty Guid");
        }
        if (!Enum.IsDefined(typeof(AssetLifecycleStatus), assetLifecycle.AssetLifecycleStatus))
        {
            return BadRequest($"RecycleAssetLifecycle failed because the enum value {assetLifecycle.AssetLifecycleStatus} does not exist");
        }

        await _assetServices.RecycleAssetLifecycle(assetLifecycleId, assetLifecycle.AssetLifecycleStatus, assetLifecycle.CallerId);

        return Ok();
    }

    /// <summary>
    ///     Search for assets.
    /// </summary>
    /// <remarks>
    ///     An advanced search that retrieves all <c>Assets</c> that matches the given criteria.
    /// </remarks>
    /// <param name="searchParameters"> A class containing all the search-parameters. </param>
    /// <param name="cancellationToken"> A injected <see cref="CancellationToken"/>. </param>
    /// <param name="page"> The current page number. </param>
    /// <param name="limit"> The highest number of items that can be added in a single page. </param>
    /// <param name="includeImeis">
    ///     When <c><see langword="true"/></c>, the <c>IMEI</c> property is loaded/included in the retrieved data. 
    ///     <para>This property will not be included unless it's explicitly requested. </para>
    /// </param>
    /// <param name="includeLabels">
    ///     When <c><see langword="true"/></c>, the <c>Labels</c> property is loaded/included in the retrieved data. 
    ///     <para>This property will not be included unless it's explicitly requested. </para>
    /// </param>
    /// <param name="includeContractHolderUser">
    ///     When <c><see langword="true"/></c>, information about the user is loaded/included in the retrieved data. 
    ///     <para>This property will not be included unless it's explicitly requested. </para>
    /// </param>
    /// <returns> The asynchronous task. The task results contains the corresponding <see cref="ActionResult{TValue}"/>. </returns>
    [Route("search/assets")]
    [SwaggerResponse(StatusCodes.Status200OK, null, typeof(PagedAssetList))]
    [HttpPost]
    public async Task<IActionResult> AssetAdvancedSearch([FromBody] SearchParameters searchParameters, CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int limit = 25, [FromQuery] bool includeImeis = false, [FromQuery] bool includeLabels = false, [FromQuery] bool includeContractHolderUser = false)
    {
        var results = await _assetServices.AdvancedSearch(searchParameters, page, limit, cancellationToken, includeAsset: true, includeImeis: includeImeis, includeLabels: includeLabels, includeContractHolderUser: includeContractHolderUser);
        var pagedAssetList = _mapper.Map<PagedAssetList>(results);

        return Ok(pagedAssetList);
    }

}