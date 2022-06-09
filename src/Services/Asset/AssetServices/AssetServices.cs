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
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.ServiceModel;
using AutoMapper;
using AssetServices.Email;

namespace AssetServices
{
    public class AssetServices : IAssetServices
    {
        private readonly IAssetLifecycleRepository _assetLifecycleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetServices> _logger;
        private readonly IEmailService _emailService;

        public AssetServices(ILogger<AssetServices> logger, 
            IAssetLifecycleRepository assetLifecycleRepository, 
            IMapper mapper,
            IEmailService emailService)
        {
            _logger = logger;
            _assetLifecycleRepository = assetLifecycleRepository;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync()
        {
            return await _assetLifecycleRepository.GetAssetLifecyclesCountsAsync();
        }

        public async Task<int> GetAssetsCountAsync(Guid customerId, AssetLifecycleStatus? assetLifecycleStatus, Guid? departmentId = null)
        {
            return await _assetLifecycleRepository.GetAssetLifecyclesCountAsync(customerId, departmentId, assetLifecycleStatus);
        }

        public async Task<decimal> GetCustomerTotalBookValue(Guid customerId)
        {
            return await _assetLifecycleRepository.GetCustomerTotalBookValue(customerId);
        }

        public async Task<IList<AssetLifecycleDTO>> GetAssetLifecyclesForUserAsync(Guid customerId, Guid userId)
        {
            var assetLifecyclesForUser = await _assetLifecycleRepository.GetAssetLifecyclesForUserAsync(customerId, userId);
            return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecyclesForUser);
        }

        public async Task UnAssignAssetLifecyclesForUserAsync(Guid customerId, Guid userId, Guid departmentId, Guid callerId)
        {
            await _assetLifecycleRepository.UnAssignAssetLifecyclesForUserAsync(customerId: customerId, userId: userId, departmentId: departmentId, callerId: callerId);
        }

        public async Task<PagedModel<AssetLifecycleDTO>> GetAssetLifecyclesForCustomerAsync(Guid customerId,string? userId, IList<AssetLifecycleStatus>? status, IList<Guid?>? department, int[]? category,
           Guid[]? label, string search, int page, int limit, CancellationToken cancellationToken)
        {
            try
            {
                var pagedAssetLifeCycles = await _assetLifecycleRepository.GetAssetLifecyclesAsync(customerId,userId, status, department, category, label, search, page, limit, cancellationToken);
                var pagedServiceAssetLifecycles = _mapper.Map<PagedModel<AssetLifecycleDTO>>(pagedAssetLifeCycles);
                return pagedServiceAssetLifecycles;
            }
            catch (Exception exception)
            {
                throw new ReadingDataException(exception);
            }
        }

        public async Task<AssetLifecycleDTO?> GetAssetLifecycleForCustomerAsync(Guid customerId, Guid assetId)
        {
            var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId);
            return assetLifecycle == null ? null : _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
        }

        public async Task<IList<CustomerLabel>> AddLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Label> labels)
        {
            try
            {
                var customerLabels = labels.Select(label => new CustomerLabel(customerId, callerId, label)).ToList();
                return await _assetLifecycleRepository.AddCustomerLabelsForCustomerAsync(customerId, customerLabels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to delete CustomerLabels.");
                throw;
            }
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId)
        {
            return await _assetLifecycleRepository.GetCustomerLabelsForCustomerAsync(customerId);
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsAsync(IList<Guid> customerLabelGuids)
        {
            return await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(customerLabelGuids);
        }

        /// <summary>
        /// Deletes labels permanently from table.
        /// Should not be called by users in gateway, but can be used by a cleanup job, or similar for when 
        /// an entity has been IsDelete = 1, for a long enough time.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="callerId"></param>
        /// <param name="labelGuids"></param>
        /// <returns></returns>
        public async Task<IList<CustomerLabel>> DeleteLabelsForCustomerAsync(Guid customerId, IList<Guid> labelGuids)
        {
            try
            {
                var customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelGuids);
                if (customerLabels == null || customerLabels.Count == 0)
                {
                    throw new ResourceNotFoundException("No CustomerLabels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                IList<int> labelIds = new List<int>();
                foreach (var label in customerLabels)
                {
                    labelIds.Add(label.Id);
                }

                return await _assetLifecycleRepository.DeleteCustomerLabelsForCustomerAsync(customerId, customerLabels);
            }
            catch (ResourceNotFoundException)
            {
                throw; // no need to log same exception again
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to delete CustomerLabels.");
                throw;
            }
        }

        /// <summary>
        /// Set IsDeleted = 1 on CustomerLabels found by Id in <paramref name="labelIds"/>.
        /// CustomerLabels still exist in database, but will not show up when fetching labels owned by customer
        /// </summary>
        /// <param name="customerId">External Id of customer whose labels we are soft deleting</param>
        /// <param name="callerId">Id of user who called endpoint to delete labels</param>
        /// <param name="labelIds">External id of labels we are soft deleting</param>
        /// <returns></returns>
        public async Task<IList<CustomerLabel>> DeleteLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Guid> labelIds)
        {
            try
            {
                var customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelIds);

                if (customerLabels == null || customerLabels.Count == 0)
                {
                    throw new ResourceNotFoundException("No CustomerLabels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                foreach (var label in customerLabels)
                {
                    label.SoftDelete(callerId);
                }

                await _assetLifecycleRepository.SaveEntitiesAsync();
                return await _assetLifecycleRepository.GetCustomerLabelsForCustomerAsync(customerId);
            }
            catch (ResourceNotFoundException)
            {
                throw; // no need to log same exception again
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to delete CustomerLabels.");
                throw;
            }
        }

        public async Task<IList<CustomerLabel>> UpdateLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> updateLabels)
        {
            return await _assetLifecycleRepository.UpdateCustomerLabelsForCustomerAsync(customerId, updateLabels);
        }

        public async Task<IList<AssetLifecycleDTO>> AssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids, IList<Guid> labelGuids)
        {
            try
            {
                var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetGuids);
                if (assetLifecycles == null || assetLifecycles.Count == 0)
                {
                    throw new ResourceNotFoundException("No assets were found using the given AssetIds. Did you enter the correct customer Id?", _logger);
                }

                var customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelGuids);
                if (customerLabels == null)
                {
                    throw new ResourceNotFoundException("No labels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                foreach (var assetLifecycle in assetLifecycles)
                {
                    foreach (var customerLabel in customerLabels)
                    {
                        if (assetLifecycle.Labels.All(l => l.ExternalId != customerLabel.ExternalId))
                        {
                            assetLifecycle.AssignCustomerLabel(customerLabel, callerId);
                        }
                    }
                }

                await _assetLifecycleRepository.SaveEntitiesAsync();

                return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecycles);
            }
            catch (ResourceNotFoundException)
            {
                throw; // no need to log same exception again
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to assign given labels to assets.");
                throw;
            }
        }

        public async Task<IList<AssetLifecycleDTO>> UnAssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids, IList<Guid> labelGuids)
        {
            try
            {
                var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetGuids);
                if (assetLifecycles == null || assetLifecycles.Count == 0)
                {
                    throw new ResourceNotFoundException("No assets were found using the given AssetIds. Did you enter the correct customer Id?", _logger);
                }

                var customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelGuids);
                if (customerLabels == null || customerLabels.Count == 0)
                {
                    throw new ResourceNotFoundException("No labels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
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
                var assetLifecyclesFromList = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetGuids);
                return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecyclesFromList);
            }
            catch (ResourceNotFoundException)
            {
                throw; // no need to log same exception again
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to unassign given labels to assets.");
                throw;
            }
        }

        public async Task<AssetLifecycleDTO> AddAssetLifecycleForCustomerAsync(Guid customerId, NewAssetDTO newAssetDTO)
        {
            var lifeCycleSetting = await _assetLifecycleRepository.GetCustomerLifeCycleSettingAssetCategory(customerId, newAssetDTO.AssetCategoryId);
            if (lifeCycleSetting == null && newAssetDTO.LifecycleType == LifecycleType.Transactional)
                throw new AssetLifeCycleSettingNotFoundException();

            if (!string.IsNullOrEmpty(newAssetDTO.AssetTag) && (newAssetDTO.AssetTag.Contains("A4010") ||  //Non personal with leasing/as a service
                                                                 newAssetDTO.AssetTag.Contains("A4020")))   //Non personal purchased transactional
            {
                newAssetDTO.IsPersonal = false;
            }
            
            var uniqueImeiList = new List<long>();
            //Validate list of IMEI and making sure that they are not duplicated for both MOBILE AND TABLET 
            if (newAssetDTO.Imei.Any())
            {
                uniqueImeiList = AssetValidatorUtility.MakeUniqueIMEIList(newAssetDTO.Imei);
                foreach (var i in uniqueImeiList)
                {
                    if (!AssetValidatorUtility.ValidateImei(i.ToString()))
                    {
                        throw new InvalidAssetDataException($"Invalid imei: {i}");
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
                newAssetLifecycle.Runtime = lifeCycleSetting!.Runtime;
            }
            var assetLifecycle = AssetLifecycle.CreateAssetLifecycle(newAssetLifecycle);

            Asset asset = newAssetDTO.AssetCategoryId == 1
                ? new MobilePhone(Guid.NewGuid(), newAssetDTO.CallerId, newAssetDTO.SerialNumber, newAssetDTO.Brand, newAssetDTO.ProductName,
                    uniqueImeiList.Select(i => new AssetImei(i)).ToList(), newAssetDTO.MacAddress)
                : new Tablet(Guid.NewGuid(), newAssetDTO.CallerId, newAssetDTO.SerialNumber, newAssetDTO.Brand, newAssetDTO.ProductName,
                    uniqueImeiList.Select(i => new AssetImei(i)).ToList(), newAssetDTO.MacAddress);
            assetLifecycle.AssignAsset(asset, newAssetDTO.CallerId);


            if (newAssetDTO.AssetHolderId != null && newAssetDTO.AssetHolderId != Guid.Empty)
            {
                var user = await _assetLifecycleRepository.GetUser(newAssetDTO.AssetHolderId.Value);
                assetLifecycle.AssignAssetLifecycleHolder(user != null ? user : new User { ExternalId = newAssetDTO.AssetHolderId.Value }, null,
                    newAssetDTO.CallerId);
            }

            if (newAssetDTO.ManagedByDepartmentId != null && newAssetDTO.ManagedByDepartmentId != Guid.Empty)
            {
                assetLifecycle.AssignAssetLifecycleHolder(null, newAssetDTO.ManagedByDepartmentId.Value, newAssetDTO.CallerId);
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
            var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId);
            if (assetLifecycle == null) return null;
            assetLifecycle.AssignLifecycleType(newLifecycleType, callerId);
            await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
        }

        [Obsolete("This will be deleted in the future")]
        public async Task<IList<AssetLifecycleDTO>> UpdateStatusForMultipleAssetLifecycles(Guid customerId, Guid callerId, IList<Guid> assetLifecycleIdList, AssetLifecycleStatus lifecycleStatus)
        {
            var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetLifecycleIdList);
            if (assetLifecycles.Count == 0)
            {
                return new List<AssetLifecycleDTO>();
            }

            //foreach (var assetLifecycle in assetLifecycles)
            //{
            //    assetLifecycle.UpdateAssetStatus(lifecycleStatus, callerId);
            //}

            //await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecycles);
        }

        public async Task<AssetLifecycleDTO> MakeAssetAvailableAsync(Guid customerId, MakeAssetAvailableDTO data)
        {
            var updatedAssetLifeCycle = await _assetLifecycleRepository.MakeAssetAvailableAsync(customerId, data.CallerId, data.AssetLifeCycleId);
            if (updatedAssetLifeCycle == null)
                throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct asset Id?", _logger);
            if (data.PreviousUser != null)
            {
                await _emailService.UnassignedFromUserEmailAsync(new Email.Model.UnassignedFromUserNotification()
                {
                    FirstName = data.PreviousUser.Name,
                    Recipient = data.PreviousUser.Email
                }, string.IsNullOrEmpty(data.PreviousUser.PreferedLanguage) ? "en" : data.PreviousUser.PreferedLanguage);
            }
                
            return _mapper.Map<AssetLifecycleDTO>(updatedAssetLifeCycle);
        }


        public async Task<AssetLifecycleDTO> UpdateAssetAsync(Guid customerId, Guid assetId, Guid callerId, string? alias, string? serialNumber, string? brand, string? model, DateTime? purchaseDate, string? note, string? tag, string? description, IList<long>? imei)
        {
            var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId);
            if (assetLifecycle == null)
            {
                throw new InvalidAssetDataException($"Asset Lifecycle {assetId} not found");
            }
            var asset = assetLifecycle.Asset;
            if (asset == null)
            {
                throw new InvalidAssetDataException($"Asset for asset life cycle {assetId} not found");
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
            if (!asset.AssetPropertiesAreValid)
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

                throw new InvalidAssetDataException(exceptionMsg.ToString());
            }

            UpdateDerivedAssetType(asset, serialNumber, imei, callerId);

            await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
        }

        private void UpdateDerivedAssetType(Asset asset, string? serialNumber, IList<long>? imei, Guid callerId)
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
        }

        public async Task<AssetLifecycleDTO> AssignAssetLifeCycleToHolder(Guid customerId, Guid assetId, AssignAssetDTO assignAssetDTO)
        {
            var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId);
            if (assetLifecycle == null) throw new ResourceNotFoundException("No asset were found using the given AssetId. Did you enter the correct Asset Id?", _logger);
            if (assignAssetDTO.UserId != Guid.Empty)
            {
                var user = await _assetLifecycleRepository.GetUser(assignAssetDTO.UserId) ?? new User { ExternalId = assignAssetDTO.UserId };
                if (user == null) throw new ResourceNotFoundException("No User were found using the given UserId. Did you enter the correct User Id?", _logger);
                assetLifecycle.AssignAssetLifecycleHolder(user, null, assignAssetDTO.CallerId);
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
                    AssetLink = "https://www.example.com/"
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
            try
            {
                return new List<AssetCategory>
                {
                    new(1, null,
                        new List<AssetCategoryTranslation> { new(1, "EN", "Mobile phones", string.Empty) }),
                    new(2, null, new List<AssetCategoryTranslation> { new(1, "EN", "Tablets", string.Empty) })
                };
            }
            catch (Exception exception)
            {
                throw new ReadingDataException(exception);
            }
        }

        public async Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId, Guid userId, string role)
        {
            if (role == PredefinedRole.EndUser.ToString())
            {
                var usersAssets = await _assetLifecycleRepository.GetAssetForUser(userId);
                var hasAsset = usersAssets.FirstOrDefault(a => a.ExternalId == assetId);
                if (hasAsset == null) throw new ResourceNotFoundException(_logger);
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
            var setting = await _assetLifecycleRepository.GetLifeCycleSettingByCustomerAsync(customerId);
            if (setting == null)
            {
                setting = new CustomerSettings(customerId, CallerId);
                await _assetLifecycleRepository.AddCustomerSettingAsync(setting, customerId);
            }
            setting.AddLifeCycleSetting(lifeCycleSetting);
            await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<LifeCycleSettingDTO>(lifeCycleSetting);
        }

        public async Task<LifeCycleSettingDTO> UpdateLifeCycleSettingForCustomerAsync(Guid customerId, LifeCycleSettingDTO lifeCycleSettingDTO, Guid CallerId)
        {
            var existingSettings = await _assetLifecycleRepository.GetLifeCycleSettingByCustomerAsync(customerId);

            if (existingSettings == null || existingSettings.LifeCycleSettings.Count < 1)
            {
                throw new ResourceNotFoundException("No LifeCycletSetting were found using the given Customer. Did you enter the correct customer Id?", _logger);
            }
            var lifeCycleSetting = existingSettings.LifeCycleSettings.FirstOrDefault(x=>x.AssetCategoryId == lifeCycleSettingDTO.AssetCategoryId);
            
            if (lifeCycleSetting == null)
            {
                throw new ResourceNotFoundException("No LifeCycletSetting were found for this AssetCategory", _logger);
            }

            if (lifeCycleSetting.BuyoutAllowed != lifeCycleSettingDTO.BuyoutAllowed)
            {
                lifeCycleSetting.SetBuyoutAllowed(lifeCycleSettingDTO.BuyoutAllowed, CallerId);
            }

            if (lifeCycleSetting.MinBuyoutPrice != lifeCycleSettingDTO.MinBuyoutPrice)
            {
                lifeCycleSetting.SetMinBuyoutPrice(lifeCycleSettingDTO.MinBuyoutPrice, CallerId);
            }

            if (lifeCycleSetting.Runtime != lifeCycleSettingDTO.Runtime)
            {
                lifeCycleSetting.SetLifeCycleRuntime(lifeCycleSettingDTO.Runtime, CallerId);
            }

            await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<LifeCycleSettingDTO>(lifeCycleSetting);
        }
        public async Task<IList<LifeCycleSettingDTO>> GetLifeCycleSettingByCustomer(Guid customerId)
        {
            var existingSetting = await _assetLifecycleRepository.GetLifeCycleSettingByCustomerAsync(customerId);
            return _mapper.Map<IList<LifeCycleSettingDTO>>(existingSetting.LifeCycleSettings);
        }

        #endregion

        #region DisposeSetting
        public async Task<DisposeSettingDTO> AddDisposeSettingForCustomerAsync(Guid customerId, DisposeSettingDTO disposeSettingDTO, Guid CallerId)
        {
            var disposeSetting = new DisposeSetting(disposeSettingDTO.PayrollContactEmail, CallerId);
            var customerSetting = await _assetLifecycleRepository.GetDisposeSettingByCustomerAsync(customerId);
            if(customerSetting == null)
            {
                customerSetting = new CustomerSettings(customerId, CallerId);
                await _assetLifecycleRepository.AddCustomerSettingAsync(customerSetting, customerId);
            }
            if(customerSetting.DisposeSetting != null) throw new InvalidOperationException();
            customerSetting.AddDisposeSetting(disposeSetting);
            await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<DisposeSettingDTO>(disposeSetting);
        }

        public async Task<DisposeSettingDTO> UpdateDisposeSettingForCustomerAsync(Guid customerId, DisposeSettingDTO disposeSettingDTO, Guid CallerId)
        {
            var disposeSetting = new DisposeSetting(disposeSettingDTO.PayrollContactEmail, CallerId);
            var customerSetting = await _assetLifecycleRepository.GetDisposeSettingByCustomerAsync(customerId);

            if (customerSetting == null || customerSetting.DisposeSetting == null)
            {
                throw new ResourceNotFoundException("No DisposeSetting were found using the given Customer. Did you enter the correct customer Id?", _logger);
            }

            if (customerSetting.DisposeSetting.PayrollContactEmail != disposeSettingDTO.PayrollContactEmail)
            {
                customerSetting.DisposeSetting.SetPayrollContactEmail(disposeSettingDTO.PayrollContactEmail, CallerId);
            }

            await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<DisposeSettingDTO>(customerSetting.DisposeSetting);
        }
        public async Task<DisposeSettingDTO> GetDisposeSettingByCustomer(Guid customerId)
        {
            var existingSettings = await _assetLifecycleRepository.GetDisposeSettingByCustomerAsync(customerId);
            return _mapper.Map<DisposeSettingDTO>(existingSettings.DisposeSetting);
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
                throw new ResourceNotFoundException($"Asset lifecycle with id {assetLifecycleId} not found", _logger);
            }

            //Change status
            try
            {
                assetLifeCycle.RepairCompleted(assetLifeCycleRepairCompleted.CallerId, assetLifeCycleRepairCompleted.Discarded);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidAssetDataException($"Asset life cycle has status {assetLifeCycle.AssetLifecycleStatus}");
            }

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
                        else throw new InvalidAssetDataException($"Invalid imei: {asset}");
                    }
                    else throw new InvalidAssetDataException($"No asset imei");

                }
            }

            if (imeiList.Any() || !string.IsNullOrWhiteSpace(assetLifeCycleRepairCompleted.NewSerialNumber))
            {
                if (assetLifeCycle.Asset != null)
                {


                    if (!imeiList.Any()) imeiList = (assetLifeCycle.Asset as HardwareAsset).Imeis.ToList();

                    Asset newAsset = assetLifeCycle.AssetCategoryId == 1
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
                throw new ResourceNotFoundException($"Asset lifecycle with id {assetLifecycleId} not found", _logger);
            }

            assetLifeCycle.IsSentToRepair(callerId);

            await _assetLifecycleRepository.SaveEntitiesAsync();
        }
    }
}