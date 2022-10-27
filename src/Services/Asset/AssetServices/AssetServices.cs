using AssetServices.Exceptions;
using AssetServices.Models;
using AssetServices.Utility;
using Common.Enums;
using Common.Interfaces;
using Common.Logging;
using Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.ServiceModel;
using AutoMapper;
using AssetServices.Email;
using AssetServices.Infrastructure;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AssetServices;

public class AssetServices : IAssetServices
{
    private readonly IAssetLifecycleRepository _assetLifecycleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AssetServices> _logger;
    private readonly IEmailService _emailService;
    private readonly ITechstepCoreProductsRepository? _techstepCoreProductsRepository;

    public AssetServices(ILogger<AssetServices> logger, 
        IAssetLifecycleRepository assetLifecycleRepository, 
        IMapper mapper,
        IEmailService emailService,
        ITechstepCoreProductsRepository? techstepCoreProductsRepository = null)
    {
        _logger = logger;
        _assetLifecycleRepository = assetLifecycleRepository;
        _mapper = mapper;
        _emailService = emailService;
        _techstepCoreProductsRepository = techstepCoreProductsRepository;
    }

    public async Task<PagedModel<CustomerAssetCount>> GetAllCustomerAssetsCountAsync(List<Guid> customerIds, int page, int limit, CancellationToken cancellationToken)
    {
        return await _assetLifecycleRepository.GetAssetLifecyclesCountsAsync(customerIds, page, limit, cancellationToken);
    }
    public async Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync(List<Guid> customerIds)
    {
        return await _assetLifecycleRepository.GetAssetLifecyclesCountsAsync(customerIds);
    }

    public async Task<int> GetAssetsCountAsync(Guid customerId, AssetLifecycleStatus? assetLifecycleStatus, Guid? departmentId = null)
    {
        return await _assetLifecycleRepository.GetAssetLifecyclesCountAsync(customerId, departmentId, assetLifecycleStatus);
    }

    public async Task<decimal> GetCustomerTotalBookValue(Guid customerId)
    {
        return await _assetLifecycleRepository.GetCustomerTotalBookValue(customerId);
    }

    public async Task<IList<AssetLifecycleDTO>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId, bool includeAsset = false, bool includeImeis = false, bool includeContractHolderUser = false)
    {
        var assetLifecyclesForUser = await _assetLifecycleRepository.GetAssetLifecyclesForUserAsync(customerId, userId,
            includeAsset: includeAsset,
            includeImeis: includeImeis,
            includeContractHolderUser: includeContractHolderUser,
            asNoTracking: true);
        return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecyclesForUser);
    }

    public async Task UnAssignAssetLifecyclesForUserAsync(Guid customerId, Guid userId, Guid? departmentId, Guid callerId)
    {
        await _assetLifecycleRepository.UnAssignAssetLifecyclesForUserAsync(customerId: customerId, userId: userId, departmentId: departmentId, callerId: callerId);
    }

    public async Task<PagedModel<AssetLifecycleDTO>> GetAssetLifecyclesForCustomerAsync(Guid customerId, string? userId, IList<AssetLifecycleStatus>? status, IList<Guid?>? department, int[]? category,
        Guid[]? label, bool? isActiveState, bool? isPersonal, DateTime? endPeriodMonth, DateTime? purchaseMonth, string? search, int page, int limit, CancellationToken cancellationToken, bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false)
    {
        var pagedAssetLifeCycles = await _assetLifecycleRepository.GetAssetLifecyclesAsync(customerId, userId, status, department, category, label, isActiveState, isPersonal, endPeriodMonth, purchaseMonth, search, page, limit, cancellationToken,
            includeAsset, includeImeis, includeLabels, includeContractHolderUser);
        var pagedServiceAssetLifecycles = _mapper.Map<PagedModel<AssetLifecycleDTO>>(pagedAssetLifeCycles);
        return pagedServiceAssetLifecycles;
    }

    public async Task CancelUserOffboarding(Guid customerId, Guid userId, Guid callerId)
    {
        var assets = await _assetLifecycleRepository.GetAssetLifecyclesForUserAsync(customerId, userId,
            includeAsset: false,
            includeContractHolderUser: false,
            includeImeis: false,
            asNoTracking: false);

        if (assets == null) return;

        assets = assets.Where(x =>
            x.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn ||
            x.AssetLifecycleStatus == AssetLifecycleStatus.PendingBuyout).ToList();
        if (!assets.Any()) return;

        foreach (var asset in assets)
        {
            asset.OffboardingCancelled(callerId);
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
    }


    public async Task<AssetLifecycleDTO?> GetAssetLifecycleForCustomerAsync(Guid customerId, Guid assetId, string? userId, IList<Guid?>? department,
        bool includeAsset = false, bool includeImeis = false, bool includeLabels = false, bool includeContractHolderUser = false)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId, userId, department, 
            includeAsset, includeImeis, includeLabels, includeContractHolderUser, asNoTracking: true);
        return assetLifecycle == null ? null : _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    public async Task<IList<CustomerLabel>> AddLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Label> labels)
    {
        var customerLabels = labels.Select(label => new CustomerLabel(customerId, callerId, label)).ToList();
        return await _assetLifecycleRepository.AddCustomerLabelsForCustomerAsync(customerId, customerLabels);            
    }

    public async Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId)
    {
        return await _assetLifecycleRepository.GetCustomerLabelsForCustomerAsync(customerId, true);
    }

    public async Task<IList<CustomerLabel>> GetCustomerLabelsAsync(IList<Guid> customerLabelGuids, Guid customerId)
    {
        return await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(customerLabelGuids, customerId);
    }

    /// <summary>
    /// Delete label for a customer. This will also delete any assignments for this label to asset lifecycles.
    /// </summary>
    /// <param name="customerId">External Id of customer whose labels we are soft deleting</param>
    /// <param name="callerId">Id of user who called endpoint to delete labels</param>
    /// <param name="labelIds">External id of labels we are soft deleting</param>
    /// <returns></returns>
    public async Task<IList<CustomerLabel>> DeleteLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Guid> labelIds)
    {
        var customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelIds, customerId);

        if (customerLabels == null || customerLabels.Count == 0)
        {
            throw new ResourceNotFoundException("No CustomerLabels were found using the given LabelIds. Did you enter the correct customer Id?", Guid.Parse("892612b2-8951-4ff0-9fca-b4351d6707d0"));
        }

        await _assetLifecycleRepository.DeleteCustomerLabelsForCustomerAsync(customerId, customerLabels);

        foreach (var label in customerLabels)
        {
            label.Delete(callerId);
        }
        await _assetLifecycleRepository.SaveEntitiesAsync();
        return await _assetLifecycleRepository.GetCustomerLabelsForCustomerAsync(customerId, true);

    }

    public async Task<IList<CustomerLabel>> UpdateLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> updateLabels)
    {
        return await _assetLifecycleRepository.UpdateCustomerLabelsForCustomerAsync(customerId, updateLabels);
    }

    public async Task<IList<AssetLifecycleDTO>> AssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids, IList<Guid> labelGuids)
    {
        var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetGuids,
            includeAsset: true,
            includeLabels: true,
            includeContractHolderUser: true,
            includeImeis: true,
            asNoTracking: false);
        if (assetLifecycles == null || assetLifecycles.Count == 0)
        {
            throw new ResourceNotFoundException("No assets were found using the given AssetIds. Did you enter the correct customer Id?", Guid.Parse("453ccd21-e0ee-4ef3-a609-aa95924e0f9e"));
        }

        var customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelGuids, customerId);
        if (customerLabels == null)
        {
            throw new ResourceNotFoundException("No labels were found using the given LabelIds. Did you enter the correct customer Id?", Guid.Parse("15b9cc1e-b577-49ae-be4f-192309d3176b"));
        }

        foreach (var assetLifecycle in assetLifecycles)
        {
            foreach (var customerLabel in customerLabels.Where(
                         customerLabel => assetLifecycle.Labels.All(l => l.ExternalId != customerLabel.ExternalId))
                    )
            {
                assetLifecycle.AssignCustomerLabel(customerLabel, callerId);
            }
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();

        return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecycles);

    }

    public async Task<IList<AssetLifecycleDTO>> UnAssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids, IList<Guid> labelGuids)
    {
        var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetGuids,
            includeAsset: false,
            includeLabels: true,
            includeContractHolderUser: false,
            includeImeis: false,
            asNoTracking: false);
        if (assetLifecycles == null || assetLifecycles.Count == 0)
        {
            throw new ResourceNotFoundException("No assets were found using the given AssetIds. Did you enter the correct customer Id?", Guid.Parse("0c275a26-5765-48fe-91a2-20a43cb72863"));
        }

        var customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelGuids, customerId);
        if (customerLabels == null || customerLabels.Count == 0)
        {
            throw new ResourceNotFoundException("No labels were found using the given LabelIds. Did you enter the correct customer Id?", Guid.Parse("22d6c83d-32b5-42f3-ad77-11bec2d89f8a"));
        }

        foreach (var assetLifecycle in assetLifecycles)
        {
            foreach (var customerLabel in customerLabels)
            {
                if (assetLifecycle.Labels.Any(l => l.ExternalId == customerLabel.ExternalId))
                {
                    assetLifecycle.RemoveCustomerLabel(customerLabel, callerId);
                }
            }
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
        var assetLifecyclesFromList = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetGuids,
            includeAsset: true,
            includeLabels: true,
            includeContractHolderUser: true,
            includeImeis: true,
            asNoTracking: true);
        return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecyclesFromList);

    }

    public async Task<AssetLifecycleDTO> AddAssetLifecycleForCustomerAsync(Guid customerId, NewAssetDTO newAssetDTO)
    {
        var lifeCycleSetting = await _assetLifecycleRepository.GetCustomerLifeCycleSettingAssetCategory(customerId, newAssetDTO.AssetCategoryId);
        if (!string.IsNullOrEmpty(newAssetDTO.AssetTag) && (newAssetDTO.AssetTag.Contains("A4010") ||  //Non personal with leasing/as a service
                                                            newAssetDTO.AssetTag.Contains("A4020")))   //Non personal purchased transactional
        {
            newAssetDTO.IsPersonal = false;
        }
            
        var uniqueImeiList = new List<long>();
        //Validate list of IMEI and making sure that they are not duplicated for both MOBILE AND TABLET 
        if (newAssetDTO.Imei != null && newAssetDTO.Imei.Any())
        {
            uniqueImeiList = AssetValidatorUtility.MakeUniqueIMEIList(newAssetDTO.Imei);
            foreach (var i in uniqueImeiList)
            {
                if (!AssetValidatorUtility.ValidateImei(i.ToString()))
                {
                    throw new InvalidAssetImeiException(i.ToString(), Guid.Parse("7fa65f5a-c8c1-48b3-a3cb-e33a8870d2bc"));
                }
            }
        }

        var sourceConverted = AssetLifeCycleSource.Unknown;
        if (Enum.TryParse(typeof(AssetLifeCycleSource), newAssetDTO.Source, out var sourceTryConverted))
        {
            sourceConverted = (AssetLifeCycleSource)(sourceTryConverted ?? AssetLifeCycleSource.Unknown);
        }

        var newAssetLifecycle = _mapper.Map<CreateAssetLifecycleDTO>(newAssetDTO);
        newAssetLifecycle.Source = sourceConverted;
        newAssetLifecycle.CustomerId = customerId;
        if (newAssetDTO.LifecycleType == LifecycleType.Transactional)
        {
            newAssetLifecycle.Runtime = lifeCycleSetting != null && lifeCycleSetting.Runtime.HasValue ? lifeCycleSetting!.Runtime.Value : 36;
        }
        var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(newAssetLifecycle);

        Models.Asset asset = newAssetDTO.AssetCategoryId == 1
            ? new MobilePhone(Guid.NewGuid(), newAssetDTO.CallerId, newAssetDTO.SerialNumber ?? string.Empty, newAssetDTO.Brand ?? string.Empty, newAssetDTO.ProductName ?? string.Empty,
                uniqueImeiList.Select(i => new AssetImei(i)).ToList(), newAssetDTO.MacAddress ?? string.Empty)
            : new Tablet(Guid.NewGuid(), newAssetDTO.CallerId, newAssetDTO.SerialNumber ?? string.Empty, newAssetDTO.Brand ?? string.Empty, newAssetDTO.ProductName ?? string.Empty,
                uniqueImeiList.Select(i => new AssetImei(i)).ToList(), newAssetDTO.MacAddress ?? string.Empty);
        assetLifecycle.AssignAsset(asset, newAssetDTO.CallerId);


        if (newAssetDTO.AssetHolderId != null && newAssetDTO.AssetHolderId != Guid.Empty)
        {
            var user = await _assetLifecycleRepository.GetUser(newAssetDTO.AssetHolderId.Value, asNoTracking: false);
            assetLifecycle.AssignAssetLifecycleHolder(user != null ? user : new User { ExternalId = newAssetDTO.AssetHolderId.Value }, newAssetDTO.ManagedByDepartmentId,
                newAssetDTO.CallerId);
        }

        if (newAssetDTO.ManagedByDepartmentId != null && newAssetDTO.ManagedByDepartmentId != Guid.Empty)
        {
            assetLifecycle.AssignAssetLifecycleHolder(null, newAssetDTO.ManagedByDepartmentId.Value, newAssetDTO.CallerId);
        }

        if (newAssetDTO.Labels != null && newAssetDTO.Labels.Any())
        {
            var exitingLabels = await _assetLifecycleRepository.GetCustomerLabelsForCustomerAsync(customerId, false);
            //already created labels - only needs to assign them
            var existInBoth = exitingLabels.Where(x => newAssetDTO.Labels.Contains(x.Label.Text)).ToList();
            //New labels - need to create them and then assign them
            var newLabels = newAssetDTO.Labels.Except(exitingLabels.Select(a => a.Label.Text)).ToList();
            //All the labels to add to asset lifecycle
            List<CustomerLabel> labelsToAssign = new List<CustomerLabel>();

            //add the labels that does not need to be made
            if (existInBoth != null && existInBoth.Any()) labelsToAssign.AddRange(existInBoth);

            //create labels
            if (newLabels != null && newLabels.Any())
            {
                var labels = newLabels.Select(a => new Label(a, LabelColor.Gray)).ToList();
                var customerLabels = labels.Select(label => new CustomerLabel(customerId, newAssetDTO.CallerId, label)).ToList();
                var createLabels = await _assetLifecycleRepository.AddCustomerLabelsForCustomerAsync(customerId, customerLabels);
                labelsToAssign.AddRange(customerLabels);
            }
            //Assign the labels to the asset lifecycle
            foreach (var label in labelsToAssign)
            {
                assetLifecycle.AssignCustomerLabel(label, newAssetDTO.CallerId);
            }
        }

        var assetLifeCycle = await _assetLifecycleRepository.AddAsync(assetLifecycle);

        return _mapper.Map<AssetLifecycleDTO>(assetLifeCycle);
    }

    public IList<(string Name, int EnumValue)> GetLifecycles()
    {
        var lifecycleEnumArray = Enum.GetValues(typeof(LifecycleType));
        IList<(string Name, int EnumValue)> lifecycleList = new List<(string Name, int EnumValue)>();

        foreach (LifecycleType lifecycleTypeElement in lifecycleEnumArray)
        {
            lifecycleList.Add((Enum.GetName(typeof(LifecycleType), lifecycleTypeElement), (int)lifecycleTypeElement));
        }

        return lifecycleList;
    }

    public IList<MinBuyoutPriceBaseline> GetBaseMinBuyoutPrice(string? country, int? assetCategoryId)
    {
        var minBuyoutPrices = MinBuyoutConfiguration.BaselineAmounts;

        if (!string.IsNullOrEmpty(country))
            minBuyoutPrices = minBuyoutPrices.Where(x => x.Country.ToLower() == country.ToLower()).ToList();

        if (assetCategoryId != null)
            minBuyoutPrices = minBuyoutPrices.Where(x => x.AssetCategoryId == assetCategoryId).ToList();
        return minBuyoutPrices;
    }


    public async Task<AssetLifecycleDTO?> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, Guid callerId, LifecycleType newLifecycleType)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId, null, null);
        if (assetLifecycle == null) return null;
        assetLifecycle.AssignLifecycleType(newLifecycleType, callerId);
        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    [Obsolete("This will be deleted in the future")]
    public async Task<IList<AssetLifecycleDTO>> UpdateStatusForMultipleAssetLifecycles(Guid customerId, Guid callerId, IList<Guid> assetLifecycleIdList, AssetLifecycleStatus lifecycleStatus)
    {
        var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetLifecycleIdList, false);
        if (assetLifecycles.Count == 0)
        {
            return new List<AssetLifecycleDTO>();
        }

        //foreach (var assetLifecycle in assetLifecycles)
        //{
        //    assetLifecycle.UpdateAssetStatus(lifecycleStatus, callerId);fv
        //}

        //await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecycles);
    }

    public async Task<AssetLifecycleDTO> MakeAssetAvailableAsync(Guid customerId, MakeAssetAvailableDTO data)
    {
        var updatedAssetLifeCycle = await _assetLifecycleRepository.MakeAssetAvailableAsync(customerId, data.CallerId, data.AssetLifeCycleId);
        if (updatedAssetLifeCycle == null)
            throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", Guid.Parse("fc64478-8011-4900-afb8-06b4b624aaeb"));
        
        if (data.PreviousUser != null && (!string.IsNullOrEmpty(data.PreviousUser?.Email) && !string.IsNullOrEmpty(data.PreviousUser?.Name)))
        {
            await _emailService.UnassignedFromUserEmailAsync(new Email.Model.UnassignedFromUserNotification()
            {
                FirstName = data.PreviousUser.Name,
                Recipient = data.PreviousUser.Email
            }, string.IsNullOrEmpty(data.PreviousUser.PreferedLanguage) ? "en" : data.PreviousUser.PreferedLanguage);
        }

        return _mapper.Map<AssetLifecycleDTO>(updatedAssetLifeCycle);
    }

    public async Task<AssetLifecycleDTO> MakeAssetExpiredAsync(Guid customerId, Guid assetId, Guid callerId)
    {
        var existingAssetLifeCycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId, null, null);
        if (existingAssetLifeCycle == null)
            throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", Guid.Parse("d038d1e5-dbe8-4479-b31f-7028a6c650e7"));

        existingAssetLifeCycle.MakeAssetExpired(callerId);
        await _assetLifecycleRepository.SaveEntitiesAsync();

        return _mapper.Map<AssetLifecycleDTO>(existingAssetLifeCycle);
    }
    public async Task<AssetLifecycleDTO> MakeAssetExpiresSoonAsync(Guid customerId, Guid assetId, Guid callerId)
    {
        var existingAssetLifeCycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId, null, null);
        if (existingAssetLifeCycle == null)
            throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", Guid.Parse("1a8b22d3-004b-4ddc-9941-30d18e7e42ca"));

        existingAssetLifeCycle.MakeAssetExpiresSoon(callerId);
        await _assetLifecycleRepository.SaveEntitiesAsync();

        return _mapper.Map<AssetLifecycleDTO>(existingAssetLifeCycle);
    }

    public async Task<AssetLifecycleDTO> ReturnDeviceAsync(Guid customerId, ReturnDeviceDTO data)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, data.AssetLifeCycleId, null, null,
            includeContractHolderUser: true, includeAsset: true, includeImeis: true);
        if (assetLifecycle == null)
            throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", Guid.Parse("05f5d961-2cb4-4067-bd2f-125e5740ef3e"));

        var returnLocations = await GetReturnLocationsByCustomer(customerId);
        var returnLocation = returnLocations.FirstOrDefault(x => x.ExternalId == data.ReturnLocationId);
        if(!assetLifecycle.IsPersonal || data.Role != PredefinedRole.EndUser.ToString())
        {
            // Confirm Return
            if (returnLocation == null)
            {
                throw new ReturnDeviceRequestException($"Return Location not found to confirm return!!! asset Id: {data.AssetLifeCycleId}", Guid.Parse("a8af69cc-7b17-4044-b34b-218f7ca5ff98"));
            }
            assetLifecycle.ConfirmReturnDevice(data.CallerId, returnLocation.Name, returnLocation.ReturnDescription);

            if(assetLifecycle.IsPersonal && data.User != null)
            {
                // Email To User thath Asset Returned on his Behalf
                var emailData = new Email.Model.ManagerOnBehalfReturnNotification()
                {
                    FirstName = data.User.Name,
                    AssetName = $"{assetLifecycle.Asset!.Brand} {assetLifecycle.Asset!.ProductName}",
                    AssetId = assetLifecycle.ExternalId.ToString(),
                    ReturnDate = DateTime.UtcNow.ToShortDateString(),
                    Recipient = data.User.Email
                };
                await _emailService.ManagerReturnEmailAsync(emailData, string.IsNullOrEmpty(data.User.PreferedLanguage) ? "en" : data.User.PreferedLanguage);

            }
        }
        else if (assetLifecycle.IsPersonal && assetLifecycle.AssetLifecycleStatus != AssetLifecycleStatus.PendingReturn)
        {
            // Pending Return
            assetLifecycle.MakeReturnRequest(data.CallerId);

            // To Manager(s)
            if(data.Managers != null && data.Managers.Any())
            {
                await _emailService.PendingReturnEmailAsync(new Email.Model.PendingReturnNotification()
                {
                    FirstName = string.Empty,
                    Recipients = data.Managers.Select(x=>x.Email).ToList(),
                    CustomerId = customerId.ToString(),
                    AssetLifecycleId = assetLifecycle.ExternalId.ToString(),
                }, "en");
            }
        }
        else if(assetLifecycle.IsPersonal && assetLifecycle.AssetLifecycleStatus == AssetLifecycleStatus.PendingReturn)
        {
            // Confirm Return
            if(returnLocation == null)
            {
                throw new ReturnDeviceRequestException($"Return Location not found to confirm pending return!!! asset Id: {data.AssetLifeCycleId}", Guid.Parse("78d06bfc-3897-497e-af90-8d9e3f0cc2e7"));
            }
            assetLifecycle.ConfirmReturnDevice(data.CallerId, returnLocation.Name, returnLocation.ReturnDescription);
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    /// <inheritdoc/>
    public async Task<AssetLifecycleDTO> PendingBuyoutDeviceAsync(Guid customerId, PendingBuyoutDeviceDTO data)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, data.AssetLifeCycleId, null, null, includeContractHolderUser: true);
        if (assetLifecycle == null)
            throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", Guid.Parse("47308bc8-ceeb-4f79-8910-75224e65ab0d"));

        assetLifecycle.RequestPendingBuyout(data.LasWorkingDay, data.CallerId);

        await _assetLifecycleRepository.SaveEntitiesAsync();

        // Email to User
        if (string.IsNullOrEmpty(data.Role) && data.Role != PredefinedRole.EndUser.ToString() && data.User != null)
        {
            var emailData = new Email.Model.ManagerOnBehalfBuyoutNotification()
            {
                FirstName = data.User.Name,
                ManagerName = data.ManagerName,
                AssetName = $"{assetLifecycle.Asset!.Brand} {assetLifecycle.Asset!.ProductName}",
                AssetId = assetLifecycle.ExternalId.ToString(),
                BuyoutDate = data.LasWorkingDay.ToShortDateString(),
                BuyoutPrice = assetLifecycle.OffboardBuyoutPrice.ToString(),
                Currency = data.Currency,
                Recipient = data.User.Email
            };
            await _emailService.ManagerBuyoutEmailAsync(emailData, string.IsNullOrEmpty(data.User.PreferedLanguage) ? "en" : data.User.PreferedLanguage);
        }

        return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    public async Task<AssetValidationResult> ImportAssetsFromFile(Guid customerId, IFormFile file, bool validateOnly, ProductSeedDataValues productId)
    {
        IList<AssetFromCsvFile> assetsFromFileRecords;
        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HeaderValidated = null,
            MissingFieldFound = null
        };
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            using (var csv = new CsvReader(reader, csvConfiguration))
            {
                assetsFromFileRecords = csv.GetRecords<AssetFromCsvFile>().ToList();
            }
        }

        var existingImeis = await _assetLifecycleRepository.GetActiveImeisList(assetsFromFileRecords
            .Where(x => !string.IsNullOrEmpty(x.Imei)).Select(x => x.Imei).ToList());

        var assetValidationResult = new AssetValidationResult();
        
        foreach (var assetFromCsv in assetsFromFileRecords)
        {
            var asset = long.TryParse(assetFromCsv.Imei, out var imei)
                ? new MobilePhone(Guid.NewGuid(), Guid.Empty, assetFromCsv.SerialNumber, assetFromCsv.Brand,
                    assetFromCsv.ProductName, new List<AssetImei> { new(imei) }, assetFromCsv.MacAddress)
                : new MobilePhone(Guid.NewGuid(), Guid.Empty, assetFromCsv.SerialNumber, assetFromCsv.Brand,
                    assetFromCsv.ProductName, new List<AssetImei>(), assetFromCsv.MacAddress);
            var emailValidator = new EmailAddressAttribute();
            var errors = new List<string>();
            if (string.IsNullOrEmpty(assetFromCsv.PurchaseType))
            {
                errors.Add($"Missng Purchase Type Value");
            }
            if (!string.IsNullOrEmpty(assetFromCsv.UserEmail) && !emailValidator.IsValid(assetFromCsv.UserEmail))
            {
                errors.Add($"Invalid e-mail: {assetFromCsv.UserEmail}");
            }
            if (!DateTime.TryParseExact(assetFromCsv.PurchaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var purchaseDate))
            {
                errors.Add($"Invalid purchase date - expected format yyyy-MM-dd (2022-03-21): {assetFromCsv.PurchaseDate}");
            }
            if (assetsFromFileRecords.Where(x => !string.IsNullOrEmpty(assetFromCsv.Imei) && x.Imei == assetFromCsv.Imei).Count() > 1)
            {
                errors.Add($"Duplicate IMEI in the file - expected UNIQUE IMEI: {assetFromCsv.Imei}");
            }
            if (existingImeis.Any(x => !string.IsNullOrEmpty(assetFromCsv.Imei) && x == assetFromCsv.Imei))
            {
                errors.Add($"Duplicate Active IMEI in the System - expected UNIQUE IMEI: {assetFromCsv.Imei}");
            }
            if (string.IsNullOrEmpty(assetFromCsv.PurchasePrice) || !decimal.TryParse(assetFromCsv.PurchasePrice, out decimal decimalPrice))
            {
                var message = string.IsNullOrEmpty(assetFromCsv.PurchasePrice) ? $"PurchasePrice is a required field and needs to be a number." : $"Invalid amount for Purchase price: {assetFromCsv.PurchasePrice} -  expected number.";
                errors.Add(message);
            }
            if (asset.ValidateAsset() && !errors.Any())
            {
                var importedAsset = CreateImportedAsset(asset, assetFromCsv, purchaseDate, true); 
                assetValidationResult.ValidAssets.Add(importedAsset);
            }
            else
            {
                errors.AddRange(asset.ErrorMsgList);
                var importedAsset = CreateImportedAsset(asset, assetFromCsv, purchaseDate, false) as InvalidImportedAsset;
                importedAsset!.Errors = errors;
                assetValidationResult.InvalidAssets.Add(importedAsset);
            }
        }

        if (validateOnly)
        {
            return assetValidationResult;
        }

        foreach (var validAsset in assetValidationResult.ValidAssets)
        {
            var newAssetDto = _mapper.Map<NewAssetDTO>(validAsset);
            if(newAssetDto.AssetTag!.ToUpper() == "A4020" || newAssetDto.AssetTag!.ToUpper() == "A3094")
            {
                newAssetDto.LifecycleType = productId switch
                {
                    ProductSeedDataValues.Implement => LifecycleType.NoLifecycle,
                    ProductSeedDataValues.TransactionalDeviceLifecycleManagement => LifecycleType.Transactional,
                    _ => LifecycleType.NoLifecycle,
                };
            }
            await AddAssetLifecycleForCustomerAsync(customerId, newAssetDto);
        }
        return assetValidationResult;
    }

    private ImportedAsset CreateImportedAsset(HardwareAsset asset, AssetFromCsvFile assetFromCsv, DateTime purchaseDate, bool isValid)
    {
        var importedAsset = isValid ? new ImportedAsset() : new InvalidImportedAsset();

        importedAsset.SerialNumber = asset.SerialNumber ?? string.Empty;
        importedAsset.Brand = asset.Brand;
        importedAsset.ProductName = asset.ProductName;
        importedAsset.Imeis = string.Join(",", asset.Imeis.Select(a => a.Imei));
        importedAsset.ImportedUser = new ImportedUser
        {
            FirstName = assetFromCsv.UserFirstName,
            LastName = assetFromCsv.UserLastName,
            Email = assetFromCsv.UserEmail,
            PhoneNumber = assetFromCsv.PhoneNumber
        };
        importedAsset.PurchaseDate = purchaseDate;
        importedAsset.Label = assetFromCsv.Label;
        importedAsset.Alias = assetFromCsv.Alias;
        importedAsset.PurchasePrice = assetFromCsv.PurchasePrice;
        importedAsset.PurchaseType = assetFromCsv.PurchaseType;
        return importedAsset;
    }

    public async Task<AssetLifecycleDTO> BuyoutDeviceAsync(Guid customerId, BuyoutDeviceDTO data)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, data.AssetLifeCycleId, null, null,
            includeContractHolderUser: true, includeAsset: true, includeImeis: true);
        if (assetLifecycle == null)
            throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", Guid.Parse("47308bc8-ceeb-4f79-8910-75224e65ab0d"));


        if (string.IsNullOrEmpty(data.PayrollContactEmail))
        {
            throw new BuyoutDeviceRequestException($"Payroll responsible email need to set first to do buyout for CustomerId: {customerId}", Guid.Parse("3f0b292c-99a5-4277-a695-4c57acd225b9"));
        }

        assetLifecycle.BuyoutDevice(data.CallerId);

        // Email to Pay-roll responsible contact
        var emailData = new Email.Model.AssetBuyoutNotification()
        {
            UserName = assetLifecycle.ContractHolderUser!.Name,
            AssetName = $"{assetLifecycle.Asset!.Brand} {assetLifecycle.Asset!.ProductName}",
            AssetId = assetLifecycle.ExternalId.ToString(),
            BuyoutDate = DateTime.UtcNow.ToString("dd MMMM, yyyy"),
            BuyoutPrice = assetLifecycle.BuyoutPrice.ToString(),
            Recipient = data.PayrollContactEmail
        };
        await _emailService.AssetBuyoutEmailAsync(emailData, "en");

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    public async Task<AssetLifecycleDTO> ConfirmBuyoutDeviceAsync(Guid customerId, BuyoutDeviceDTO data)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, data.AssetLifeCycleId, null, null,
            includeContractHolderUser: true, includeAsset: true, includeImeis: true);
        if (assetLifecycle == null)
            throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", Guid.Parse("47308bc8-ceeb-4f79-8910-75224e65ab0d"));

        assetLifecycle.ConfirmBuyoutDevice(data.CallerId);

        // Email to Pay-roll responsible contact
        if (!string.IsNullOrEmpty(data.PayrollContactEmail))
        {
            var emailData = new Email.Model.AssetBuyoutNotification()
            {
                UserName = assetLifecycle.ContractHolderUser!.Name,
                AssetName = $"{assetLifecycle.Asset!.Brand} {assetLifecycle.Asset!.ProductName}",
                AssetId = assetLifecycle.ExternalId.ToString(),
                BuyoutDate = DateTime.UtcNow.ToString("dd MMMM, yyyy"),
                BuyoutPrice = assetLifecycle.BuyoutPrice.ToString(),
                Recipient = data.PayrollContactEmail
            };
            await _emailService.AssetBuyoutEmailAsync(emailData, "en");
        }  

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    public async Task<AssetLifecycleDTO> ReportDeviceAsync(Guid customerId, ReportDeviceDTO data)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, data.AssetLifeCycleId, null, null,
            includeContractHolderUser: true, includeAsset: true, includeImeis: true);
        if (assetLifecycle == null)
            throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", Guid.Parse("bc644bf3-9f85-475b-bd8c-18f08c5c8cd0"));

        assetLifecycle.ReportDevice(data.ReportCategory, data.CallerId);

        var emailData = new Email.Model.ReportAssetNotification()
        {
            ReportType = data.ReportCategory.ToString(),
            AssetName = $"{assetLifecycle.Asset!.Brand} {assetLifecycle.Asset!.ProductName}",
            AssetId = assetLifecycle.ExternalId.ToString(),
            ReportDate = DateTime.UtcNow.ToShortDateString(),
            ReportedBy = data.ReportedBy,
            Description = data.Description,
            DateFrom = data.TimePeriodFrom.ToShortDateString(),
            DateTo = data.TimePeriodTo.ToShortDateString(),
            Address = $"{data.Address} {data.City} {data.PostalCode} {data.Country}"
        };

        // To User (personal)
        if (assetLifecycle.IsPersonal && data.ContractHolderUser != null)
        {
            emailData.FirstName = data.ContractHolderUser.Name;
            emailData.Recipients = new List<string> { data.ContractHolderUser.Email };
            await _emailService.ReportAssetEmailAsync(emailData, string.IsNullOrEmpty(data.ContractHolderUser.PreferedLanguage) ? "en" : data.ContractHolderUser.PreferedLanguage);
        }

        // To Manager(s)
        if(data.Managers != null && data.Managers.Any())
        {
            emailData.FirstName = string.Empty;
            emailData.Recipients = data.Managers.Select(x => x.Email).ToList();
            await _emailService.ReportAssetEmailAsync(emailData, "en");
        }

        // To Customer (Non-personal)
        if (!assetLifecycle.IsPersonal && data.CustomerAdmins != null && data.CustomerAdmins.Any())
        {
            emailData.FirstName = string.Empty;
            emailData.Recipients = data.CustomerAdmins.Select(x => x.Email).ToList();
            await _emailService.ReportAssetEmailAsync(emailData, "en");
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    public async Task<AssetLifecycleDTO> UpdateAssetAsync(Guid customerId, Guid assetId, Guid callerId, string? alias, string? serialNumber, string? brand, string? model, DateTime? purchaseDate, string? note, string? tag, string? description, IList<long>? imei, string? macAddress)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId, null, null,
            includeAsset: true, includeImeis: true, includeLabels: true, includeContractHolderUser: true);
        if (assetLifecycle == null)
        {
            throw new InvalidAssetDataException($"Asset Lifecycle {assetId} not found",Guid.Parse("175d952c-d23e-44ed-82c2-ca47112958ac"));
        }
        var asset = assetLifecycle.Asset;
        if (asset == null)
        {
            //Should have a AssetNotFoundException?
            throw new InvalidAssetDataException($"Asset for asset life cycle {assetId} not found",Guid.Parse("bb2d2355-b141-44f2-aecd-32026d62877f"));
        }

        if (!string.IsNullOrWhiteSpace(brand) && asset.Brand != brand)
        {
            asset.UpdateBrand(brand, callerId);
        }
        if (!string.IsNullOrWhiteSpace(model) && asset.ProductName != model)
        {
            asset.UpdateProductName(model, callerId);
        }
        if (alias != null && assetLifecycle.Alias != alias.Trim())
        {
            assetLifecycle.UpdateAlias(alias.Trim(), callerId);
        }
 
        if (purchaseDate != null && assetLifecycle.PurchaseDate != purchaseDate)
        {
            assetLifecycle.UpdatePurchaseDate(purchaseDate.Value, callerId);
        }
        if (note != null && assetLifecycle.Note != note)
        {
            assetLifecycle.UpdateNote(note, callerId);
        }
        
        if (!asset.ValidateAsset())
        {
            var exceptionMsg = new StringBuilder();
            foreach (var errorMsg in asset.ErrorMsgList)
            {
                if (errorMsg.Contains("Imei"))
                {
                    exceptionMsg.Append($"Asset {errorMsg}" + " is invalid.\n");
                }
                else
                {
                    exceptionMsg.Append($"Minimum asset data requirements not set: {errorMsg}" + ".\n");
                }
            }

            throw new InvalidAssetDataException(exceptionMsg.ToString(), Guid.Parse("cd8694a9-a5fb-4bd6-b1ba-032bbb52ba33"));
        }

        UpdateDerivedAssetType(asset, serialNumber, imei, macAddress, callerId);

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    private void UpdateDerivedAssetType(Models.Asset asset, string? serialNumber, IList<long>? imei, string? macAddress,Guid callerId)
    {
        var phone = asset as MobilePhone;
        if (phone != null)
        {
            if (!string.IsNullOrEmpty(serialNumber) && phone.SerialNumber != serialNumber)
            {
                phone.ChangeSerialNumber(serialNumber, callerId);
            }

            if (imei != null)
            {
                var uniqueListOfImeis = AssetValidatorUtility.MakeUniqueIMEIList(imei);
                if (phone.Imeis.Select(i => i.Imei).SequenceEqual(uniqueListOfImeis))
                {
                    phone.SetImei(uniqueListOfImeis, callerId);
                }
            }
            if (!string.IsNullOrWhiteSpace(macAddress) && phone.MacAddress != macAddress)
            {
                phone.SetMacAddress(macAddress, callerId);
            }
        }

        var tablet = asset as Tablet;
        if (tablet == null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(serialNumber) && tablet.SerialNumber != serialNumber)
        {
            tablet.ChangeSerialNumber(serialNumber, callerId);
        }

        if (imei != null)
        {
            var uniqueListOfImeis = AssetValidatorUtility.MakeUniqueIMEIList(imei);
            if (tablet.Imeis.Select(i => i.Imei).SequenceEqual(uniqueListOfImeis))
            {
                tablet.SetImei(uniqueListOfImeis, callerId);
            }
        }

        if (!string.IsNullOrWhiteSpace(macAddress) && tablet.MacAddress != macAddress)
        {
            tablet.SetMacAddress(macAddress, callerId);   
        }
    }

    public async Task<AssetLifecycleDTO> AssignAssetLifeCycleToHolder(Guid customerId, Guid assetId, AssignAssetDTO assignAssetDTO)
    {
        var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId, null, null,
            includeContractHolderUser: true, includeAsset: true, includeImeis: true);
        if (assetLifecycle == null) throw new ResourceNotFoundException("No asset were found using the given AssetId. Did you enter the correct Asset Id?", Guid.Parse("d10ebba0-97d1-4065-b302-f580b1604e61"));
        if (assignAssetDTO.UserId != Guid.Empty)
        {
            var user = await _assetLifecycleRepository.GetUser(assignAssetDTO.UserId, asNoTracking: true) ?? new User { ExternalId = assignAssetDTO.UserId };
            if (user == null) throw new ResourceNotFoundException("No User were found using the given UserId. Did you enter the correct User Id?", Guid.Parse("bc855ef2-9445-4e22-a658-bf195b6202ae"));

            if (assignAssetDTO.UserAssigneToDepartment != Guid.Empty) assetLifecycle.AssignAssetLifecycleHolder(user, assignAssetDTO.UserAssigneToDepartment, assignAssetDTO.CallerId);
            else assetLifecycle.AssignAssetLifecycleHolder(user, null, assignAssetDTO.CallerId);


        }
        else
        {
            assetLifecycle.AssignAssetLifecycleHolder(null, assignAssetDTO.DepartmentId, assignAssetDTO.CallerId);
        }
        await _assetLifecycleRepository.SaveEntitiesAsync();

        // Send emails
        if (assignAssetDTO.PreviousUser != null)
        {
            await _emailService.UnassignedFromUserEmailAsync(new Email.Model.UnassignedFromUserNotification()
            {
                FirstName = assignAssetDTO.PreviousUser.Name,
                Recipient = assignAssetDTO.PreviousUser.Email
            }, string.IsNullOrEmpty(assignAssetDTO.PreviousUser.PreferedLanguage) ? "en" : assignAssetDTO.PreviousUser.PreferedLanguage);
        }
        if (assignAssetDTO.NewUser != null)
        {
            await _emailService.ReAssignedToUserEmailAsync(new Email.Model.ReAssignedToUserNotification()
            {
                FirstName = assignAssetDTO.NewUser.Name,
                Recipient = assignAssetDTO.NewUser.Email,
                CustomerId = customerId.ToString(),
                AssetLifecycleId = assetLifecycle.ExternalId.ToString()

            }, string.IsNullOrEmpty(assignAssetDTO.NewUser.PreferedLanguage) ? "en" : assignAssetDTO.NewUser.PreferedLanguage);
        }

        if (assignAssetDTO.PreviousManagers != null && assignAssetDTO.PreviousManagers.Any())
        {
            await _emailService.UnassignedFromManagerEmailAsync(new Email.Model.UnassignedFromManagerNotification()
            {
                Recipient = assignAssetDTO.PreviousManagers.Select(x => x.Email).ToList(),
            }, string.IsNullOrEmpty(assignAssetDTO.PreviousManagers.FirstOrDefault()!.PreferedLanguage) ? "en" : assignAssetDTO.PreviousManagers.FirstOrDefault()!.PreferedLanguage);
        }
        return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
    }

    public IList<AssetCategory> GetAssetCategories(string language = "EN")
    {
        return new List<AssetCategory>
        {
            new(1, null,
                new List<AssetCategoryTranslation> { new(1, "EN", "Mobile phones", string.Empty) }),
            new(2, null, new List<AssetCategoryTranslation> { new(1, "EN", "Tablets", string.Empty) })
        };
    }

    public async Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId, Guid userId, string role)
    {
        if (role == PredefinedRole.EndUser.ToString())
        {
            var usersAssets = await _assetLifecycleRepository.GetAssetForUser(userId);
            var hasAsset = usersAssets.FirstOrDefault(a => a.ExternalId == assetId);
            if (hasAsset == null) throw new ResourceNotFoundException($"No connected assets to user with id: {userId}.",Guid.Parse("7d8a072c-a8a9-4b46-888d-2ad97bffaf9e"));
        }


        var logEventEntries = await _assetLifecycleRepository.GetAuditLog(assetId);
        var assetLogList = new List<AssetAuditLog>();

        foreach (var logEventEntry in logEventEntries)
        {
            try
            {
                if (string.IsNullOrEmpty(logEventEntry.Content) || string.IsNullOrEmpty(logEventEntry.EventTypeName))
                {
                    continue;
                }
                var eventType = Type.GetType(logEventEntry.EventTypeName);
                if (eventType == null)
                {
                    continue;
                }

                dynamic @event = JsonSerializer.Deserialize(logEventEntry.Content, eventType) as IEvent;
                if (@event == null)
                {
                    continue;
                }

                if (!Guid.TryParse(logEventEntry.TransactionId, out var transactionGuid))
                {
                    continue;
                }
                   
                var previousStatus = PropertyExist(@event, "PreviousStatus")
                    ? @event.PreviousStatus.ToString()
                    : @event.AssetLifecycle.AssetLifecycleStatus.ToString();

                // All events should have callerId, but if an event forgot it, we handle it here
                var callerId = PropertyExist(@event, "CallerId")
                    ? @event.CallerId.ToString()
                    : "N/A";
                var auditLog = new AssetAuditLog(transactionGuid, logEventEntry.EventId, @event.AssetLifecycle.CustomerId, logEventEntry.CreationTime, callerId,
                    ((IEvent)@event).EventMessage(), logEventEntry.EventTypeShortName, previousStatus, @event.AssetLifecycle.AssetLifecycleStatus.ToString());
                assetLogList.Add(auditLog);
            }
            catch (Exception ex)
            {
                // Log error but don't stop request
                _logger?.LogError("{0}", ex.Message);
            }
        }
        return assetLogList;
    }
    #region LifeCycleSetting
    public async Task<LifeCycleSettingDTO> AddLifeCycleSettingForCustomerAsync(Guid customerId, LifeCycleSettingDTO lifeCycleSettingDTO, Guid CallerId)
    {
        var lifeCycleSetting = new LifeCycleSetting(lifeCycleSettingDTO.AssetCategoryId, lifeCycleSettingDTO.BuyoutAllowed, lifeCycleSettingDTO.MinBuyoutPrice, lifeCycleSettingDTO.Runtime, CallerId);
        var setting = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId);
        if (setting == null)
        {
            setting = new CustomerSettings(customerId, CallerId);
            await _assetLifecycleRepository.AddCustomerSettingAsync(setting, customerId);
        }
        setting.AddLifeCycleSetting(lifeCycleSetting, CallerId);
        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<LifeCycleSettingDTO>(lifeCycleSetting);
    }

    public async Task<LifeCycleSettingDTO> UpdateLifeCycleSettingForCustomerAsync(Guid customerId, LifeCycleSettingDTO lifeCycleSettingDTO, Guid CallerId)
    {
        var existingSettings = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId, asNoTracking: false);

        if (existingSettings == null || existingSettings.LifeCycleSettings.Count < 1)
        {
            throw new ResourceNotFoundException("No LifeCycletSetting were found using the given Customer. Did you enter the correct customer Id?", Guid.Parse("b3fbf0f4-40e2-4562-84a5-8bc8c84824d4"));
        }
        var lifeCycleSetting = existingSettings.LifeCycleSettings.FirstOrDefault(x=>x.AssetCategoryId == lifeCycleSettingDTO.AssetCategoryId);
            
        if (lifeCycleSetting == null)
        {
            throw new ResourceNotFoundException("No LifeCycletSetting were found for this AssetCategory", Guid.Parse("33643f96-5446-4f82-b79d-4e53f1daa0b0"));
        }

        if (lifeCycleSetting.BuyoutAllowed != lifeCycleSettingDTO.BuyoutAllowed)
        {
            lifeCycleSetting.SetBuyoutAllowed(lifeCycleSettingDTO.BuyoutAllowed, CallerId);
        }

        if (lifeCycleSetting.MinBuyoutPrice != lifeCycleSettingDTO.MinBuyoutPrice)
        {
            lifeCycleSetting.SetMinBuyoutPrice(lifeCycleSettingDTO.MinBuyoutPrice.Amount, CallerId);
        }

        if (lifeCycleSettingDTO.Runtime.HasValue && lifeCycleSettingDTO.Runtime != 0 && lifeCycleSetting.Runtime != lifeCycleSettingDTO.Runtime)
        {
            lifeCycleSetting.SetLifeCycleRuntime(lifeCycleSettingDTO.Runtime.Value, CallerId);
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<LifeCycleSettingDTO>(lifeCycleSetting);
    }
    public async Task<IList<LifeCycleSettingDTO>> GetLifeCycleSettingByCustomer(Guid customerId)
    {
        var existingSetting = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId, asNoTracking: true);
        if (existingSetting == null)
        {
            existingSetting = await _assetLifecycleRepository.AddCustomerSettingAsync(new CustomerSettings(customerId, Guid.Empty), customerId);
        }
        return _mapper.Map<IList<LifeCycleSettingDTO>>(existingSetting.LifeCycleSettings);
    }

    #endregion

    #region DisposeSetting
    public async Task<DisposeSettingDTO> AddDisposeSettingForCustomerAsync(Guid customerId, DisposeSettingDTO disposeSettingDTO, Guid CallerId)
    {
        var disposeSetting = new DisposeSetting(CallerId);
        var customerSetting = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId);
        if(customerSetting == null)
        {
            customerSetting = new CustomerSettings(customerId, CallerId);
            await _assetLifecycleRepository.AddCustomerSettingAsync(customerSetting, customerId);
        }
        if(customerSetting.DisposeSetting != null) throw new InvalidOperationException();
        customerSetting.AddDisposeSetting(disposeSetting, CallerId);
        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<DisposeSettingDTO>(disposeSetting);
    }

    public async Task<DisposeSettingDTO> UpdateDisposeSettingForCustomerAsync(Guid customerId, DisposeSettingDTO disposeSettingDTO, Guid CallerId)
    {
        var customerSetting = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId, asNoTracking: true);

        if (customerSetting == null || customerSetting.DisposeSetting == null)
        {
            throw new ResourceNotFoundException("No DisposeSetting were found using the given Customer. Did you enter the correct customer Id?", Guid.Parse("23ea63e9-2528-459d-93ab-e7c107779902"));
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<DisposeSettingDTO>(customerSetting.DisposeSetting);
    }
    public async Task<DisposeSettingDTO> GetDisposeSettingByCustomer(Guid customerId)
    {
        var existingSetting = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId, asNoTracking: true);
        if (existingSetting == null)
        {
            existingSetting = await _assetLifecycleRepository.AddCustomerSettingAsync(new CustomerSettings(customerId, Guid.Empty), customerId);
        }
        return _mapper.Map<DisposeSettingDTO>(existingSetting.DisposeSetting);
    }
    public async Task<ReturnLocationDTO> AddReturnLocationsByCustomer(Guid customerId, ReturnLocationDTO returnLocationDTO, Guid callerId)
    {
        var customerSettings = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId);
        if (customerSettings == null)
        {
            customerSettings = await _assetLifecycleRepository.AddCustomerSettingAsync(new CustomerSettings(customerId, Guid.Empty), customerId);
        }
        var returnLocation = new ReturnLocation(returnLocationDTO.Name, returnLocationDTO.ReturnDescription, returnLocationDTO.LocationId);
        if(customerSettings.DisposeSetting == null)
        {
            customerSettings.AddDisposeSetting(new DisposeSetting(),callerId);
        }
        customerSettings.DisposeSetting!.AddReturnLocation(returnLocation, callerId, customerId);

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<ReturnLocationDTO>(returnLocation);
    }
    public async Task<IList<ReturnLocationDTO>> GetReturnLocationsByCustomer(Guid customerId)
    {
        var customerSettings = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId, asNoTracking: true);
        if (customerSettings is null || customerSettings.DisposeSetting is null || !customerSettings.DisposeSetting.ReturnLocations.Any())
        {
            return new List<ReturnLocationDTO>();
        }
        return _mapper.Map<IList<ReturnLocationDTO>>(customerSettings.DisposeSetting.ReturnLocations);
    }

    public async Task<ReturnLocationDTO> UpdateReturnLocationsByCustomer(Guid customerId, Guid returnLocationId, ReturnLocationDTO returnLocationDTO, Guid callerId)
    {
        var customerSettings = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId);
        if(customerSettings is null || customerSettings.DisposeSetting is null || !customerSettings.DisposeSetting.ReturnLocations.Any())
        {
            throw new ResourceNotFoundException("No DisposeSetting were found using the given Customer. Did you enter the correct customer Id?", Guid.Parse("da38e153-18b8-4b57-8ae4-c2f490875c16"));
        }
        var returnLocation = customerSettings.DisposeSetting.ReturnLocations.FirstOrDefault(x => x.ExternalId == returnLocationId);
        if (returnLocation == null)
        {
            throw new ResourceNotFoundException("No Return Location were found using the given Location. Did you enter the correct Return Location Id?", Guid.Parse("ac5cf843-3e57-4bda-af08-3d63f3c3ad77"));
        }
        var updatedReturnLocatio = new ReturnLocation(returnLocationDTO.Name, returnLocationDTO.ReturnDescription, returnLocationDTO.LocationId);
        customerSettings.DisposeSetting.UpdateReturnLocation(callerId, returnLocationId, updatedReturnLocatio);

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<ReturnLocationDTO>(returnLocation);
    }
    public async Task<IList<ReturnLocationDTO>> RemoveReturnLocationsByCustomer(Guid customerId, Guid returnLocationId, Guid callerId)
    {
        var customerSettings = await _assetLifecycleRepository.GetCustomerSettingsAsync(customerId);
        if (customerSettings is null || customerSettings.DisposeSetting is null || !customerSettings.DisposeSetting.ReturnLocations.Any())
        {
            throw new ResourceNotFoundException("No DisposeSetting were found using the given Customer. Did you enter the correct customer Id?", Guid.Parse("e1f6ff7f-3122-4cc7-9b15-c61abb8afad9"));
        }
        var returnLocation = customerSettings.DisposeSetting.ReturnLocations.FirstOrDefault(x => x.ExternalId == returnLocationId);
        if (returnLocation == null)
        {
            throw new ResourceNotFoundException("No Return Location were found using the given Location. Did you enter the correct Return Location Id?", Guid.Parse("8d93deba-7786-4625-9952-e947de06f2cc]"));
        }
        customerSettings!.DisposeSetting.RemoveReturnLocation(returnLocation, callerId, customerId);

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<IList<ReturnLocationDTO>>(customerSettings.DisposeSetting.ReturnLocations);
    }

    #endregion


    // From https://stackoverflow.com/a/9956981
    private static bool PropertyExist(dynamic dynamicObject, string name)
    {
        if (dynamicObject is ExpandoObject)
            return ((IDictionary<string, object>)dynamicObject).ContainsKey(name);

        return dynamicObject.GetType().GetProperty(name) != null;
    }

    public async Task AssetLifeCycleRepairCompleted(Guid assetLifecycleId, AssetLifeCycleRepairCompleted assetLifeCycleRepairCompleted)
    {
        var assetLifeCycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(assetLifecycleId);
        if (assetLifeCycle == null)
        {
            throw new ResourceNotFoundException($"Asset lifecycle with id {assetLifecycleId} not found", Guid.Parse("75a7f22c-d243-4d9f-b96b-4afd7d53d1f6"));
        }

        //Change status
        assetLifeCycle.RepairCompleted(assetLifeCycleRepairCompleted.CallerId, assetLifeCycleRepairCompleted.Discarded);
           
            
        //Add new asset if new values are added 
        long imei = 0;
        List<AssetImei>? imeiList = new List<AssetImei>();

        if (assetLifeCycleRepairCompleted.NewImei.Any())
        {
            foreach (var asset in assetLifeCycleRepairCompleted.NewImei)
            {
                if (asset != null && long.TryParse(asset, out imei))
                {
                    if (AssetValidatorUtility.ValidateImei(asset))
                    {
                        imeiList.Add(new AssetImei(imei));

                    }
                    else throw new InvalidAssetImeiException($"{asset}", Guid.Parse("14e873e0-fa74-4387-af6f-cb2687a67ec7"));
                }
                else throw new InvalidAssetImeiException($"NOT IMEI NUMBER", Guid.Parse("1fe8b6ff-14dc-4d24-b98d-61c78e203e23"));

            }
        }

        if (imeiList.Any() || !string.IsNullOrWhiteSpace(assetLifeCycleRepairCompleted.NewSerialNumber))
        {
            if (assetLifeCycle.Asset != null)
            {


                if (!imeiList.Any()) imeiList = (assetLifeCycle.Asset as HardwareAsset).Imeis.ToList();

                Models.Asset newAsset = assetLifeCycle.AssetCategoryId == 1
                    ? new MobilePhone(Guid.NewGuid(), assetLifeCycleRepairCompleted.CallerId, assetLifeCycleRepairCompleted.NewSerialNumber ?? (assetLifeCycle.Asset as HardwareAsset).SerialNumber, assetLifeCycle.Asset.Brand, assetLifeCycle.Asset.ProductName,
                        imeiList, (assetLifeCycle.Asset as HardwareAsset).MacAddress)
                    : new Tablet(Guid.NewGuid(), assetLifeCycleRepairCompleted.CallerId, assetLifeCycleRepairCompleted.NewSerialNumber ?? (assetLifeCycle.Asset as HardwareAsset).SerialNumber, assetLifeCycle.Asset.Brand, assetLifeCycle.Asset.ProductName,
                        imeiList, (assetLifeCycle.Asset as HardwareAsset).MacAddress);

                assetLifeCycle.AssignAsset(newAsset, assetLifeCycleRepairCompleted.CallerId);
            }
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
    }

    public async Task AssetLifeCycleSendToRepair(Guid assetLifecycleId, Guid callerId)
    {
        var assetLifeCycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(assetLifecycleId);

        if(assetLifeCycle == null)
        {
            throw new ResourceNotFoundException($"Asset lifecycle with id {assetLifecycleId} not found", Guid.Parse("9925087e-78e2-4ea3-b7df-b18a0f6c4782"));
        }

        assetLifeCycle.IsSentToRepair(callerId);

        await _assetLifecycleRepository.SaveEntitiesAsync();
    }

    public async Task<CustomerAssetsCounterDTO> GetAssetLifecycleCountersAsync(Guid customerId, IList<AssetLifecycleStatus>? filterStatus, IList<Guid?>? departments, Guid? userId)
    {
        if (userId != Guid.Empty)
        {
            CustomerAssetsCounterDTO endUser = new CustomerAssetsCounterDTO();
            endUser.UsersPersonalAssets = await _assetLifecycleRepository.GetAssetLifecycleCountForUserAsync(customerId, userId);
            endUser.OrganizationId = customerId;
            return endUser;

        }
        if (filterStatus == null || !filterStatus.Any())
        {
            filterStatus = new List<AssetLifecycleStatus>() { 
                AssetLifecycleStatus.Active, AssetLifecycleStatus.Available, 
                AssetLifecycleStatus.InUse, AssetLifecycleStatus.InputRequired, 
                AssetLifecycleStatus.Expired, AssetLifecycleStatus.Repair, 
                AssetLifecycleStatus.PendingReturn,AssetLifecycleStatus.ExpiresSoon,
                AssetLifecycleStatus.PendingRecycle
            };
        }
           
        if (!filterStatus.Select(x => x).All(x => Enum.IsDefined(typeof(AssetLifecycleStatus), x)))
        {
            throw new ResourceNotFoundException("Not a valid asset status", Guid.Parse("b7a42841-bef7-4138-a3d1-d5f7d3b5aab1"));
        }

        if (departments != null && departments.Any(d => d == Guid.Empty)) departments = departments.Where(x => x != Guid.Empty).ToList();

        if (departments != null && departments.Any())
        {
            return await _assetLifecycleRepository.GetAssetCountForDepartmentAsync(customerId,userId, filterStatus, departments);
        }


        return await _assetLifecycleRepository.GetAssetLifecycleCountForCustomerAsync(customerId, userId, filterStatus);
    }

    public async Task<IList<AssetLifecycleDTO>> ActivateAssetLifecycleStatus(Guid customerId, ChangeAssetStatus assetLifecyclesId)
    {
        var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetLifecyclesId.AssetLifecycleId,
            includeAsset: true,
            includeLabels: false,
            includeContractHolderUser: true,
            includeImeis: true,
            asNoTracking: false);
        if (assetLifecycles.Count == 0)
        {
            return new List<AssetLifecycleDTO>();
        }

        foreach (var assetLifecycle in assetLifecycles)
        {
            assetLifecycle.SetActiveStatus(assetLifecyclesId.CallerId);
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecycles);
    }

    public async Task<IList<AssetLifecycleDTO>> DeactivateAssetLifecycleStatus(Guid customerId, ChangeAssetStatus assetLifecyclesId)
    {
        var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetLifecyclesId.AssetLifecycleId,
            includeAsset: true,
            includeLabels: true,
            includeContractHolderUser: true,
            includeImeis: true,
            asNoTracking: false);
        if (assetLifecycles.Count == 0)
        {
            return new List<AssetLifecycleDTO>();
        }
        foreach (var assetLifecycle in assetLifecycles)
        {
            assetLifecycle.SetInactiveStatus(assetLifecyclesId.CallerId);
        }

        await _assetLifecycleRepository.SaveEntitiesAsync();
        return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecycles);
    }

    public async Task SyncDepartmentForUserToAssetLifecycleAsync(Guid customerId, Guid userId, Guid? departmentId, Guid callerId)
    {
        var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesForUserAsync(customerId, userId,
            includeAsset: false,
            includeImeis: false,
            includeContractHolderUser: true,
            asNoTracking: false);

        var user = await _assetLifecycleRepository.GetUser(userId, asNoTracking: true);
        if (user == null)
        {
            // We don't care if the user exists in Assets database.
            user = new User { ExternalId = userId };
        }

        foreach (var assetLifecycle in assetLifecycles)
        {
            assetLifecycle.AssignAssetLifecycleHolder(user, departmentId, callerId);
        }
    }
    /// <inheritdoc/>
    public async Task CancelReturn(Guid assetLifecycleId, Guid callerId)
    {
        var assetLifeCycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(assetLifecycleId);

        if (assetLifeCycle == null)
        {
            throw new ResourceNotFoundException($"Asset lifecycle with id {assetLifecycleId} not found", Guid.Parse("d413ae4c-0cac-47b4-b9a3-afcdc030153c"));
        }

        assetLifeCycle.CancelReturn(callerId, DateTime.UtcNow);

        await _assetLifecycleRepository.SaveEntitiesAsync();
    }

    /// <inheritdoc/>
    public async Task RecycleAssetLifecycle(Guid assetLifecycleId, int assetLifecycleStatus, Guid callerId)
    {
        var assetLifeCycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(assetLifecycleId);

        if (assetLifeCycle == null)
        {
            throw new ResourceNotFoundException($"Asset lifecycle with id {assetLifecycleId} not found", Guid.Parse("ba1bb151-7676-470a-a864-431a53ab756f"));
        }

        var changed = false;
        if ((AssetLifecycleStatus)assetLifecycleStatus == AssetLifecycleStatus.Recycled)
        {
            assetLifeCycle.Recycle(callerId);
            changed = true;
        }
        else if ((AssetLifecycleStatus)assetLifecycleStatus == AssetLifecycleStatus.PendingRecycle)
        { 
            assetLifeCycle.PendingRecycle(callerId);
            changed = true;
        }

        if (changed) await _assetLifecycleRepository.SaveEntitiesAsync();
    }

    public async Task<IList<TechstepProduct>> FindTechstepProductsAsync(string productSearchString)
    {
        if (_techstepCoreProductsRepository == null)
        {
            throw new TechstepRepositorySetupException($"Techstep Core repository not properly configured", Guid.Parse("fc499152-01e4-408d-97a2-a07818e15755"));
        }
        return await _techstepCoreProductsRepository.GetPartNumbersAsync(productSearchString);
    }
}