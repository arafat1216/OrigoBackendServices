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

namespace AssetServices
{
    public class AssetServices : IAssetServices
    {
        private readonly IAssetLifecycleRepository _assetLifecycleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AssetServices> _logger;

        public AssetServices(ILogger<AssetServices> logger, IAssetLifecycleRepository assetLifecycleRepository, IMapper mapper)
        {
            _logger = logger;
            _assetLifecycleRepository = assetLifecycleRepository;
            _mapper = mapper;
        }

        public async Task<IList<CustomerAssetCount>> GetAllCustomerAssetsCountAsync()
        {
            return await _assetLifecycleRepository.GetAssetLifecyclesCountsAsync();
        }

        public async Task<int> GetAssetsCountAsync(Guid customerId)
        {
            return await _assetLifecycleRepository.GetAssetLifecyclesCountAsync(customerId);
        }
        public async Task<int> GetCustomerAvailableAssetCountAsync(Guid customerId)
        {
            return await _assetLifecycleRepository.GetCustomerAvailableAssetCountAsync(customerId);
        }
        public async Task<int> GetDepartmentAvailableAssetCountAsync(Guid customerId, Guid departmentId)
        {
            return await _assetLifecycleRepository.GetDepartmentAvailableAssetCountAsync(customerId, departmentId);
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

        public async Task<PagedModel<AssetLifecycleDTO>> GetAssetLifecyclesForCustomerAsync(Guid customerId, string search, int page, int limit, AssetLifecycleStatus? status, CancellationToken cancellationToken)
        {
            try
            {
                var pagedAssetLifeCycles = await _assetLifecycleRepository.GetAssetLifecyclesAsync(customerId, search, page, limit, status, cancellationToken);
                var pagedServiceAssetLifecycles = _mapper.Map<PagedModel<AssetLifecycleDTO>>(pagedAssetLifeCycles);
                return pagedServiceAssetLifecycles;
            }
            catch (Exception exception)
            {
                throw new ReadingDataException(exception);
            }
        }

        public async Task<AssetLifecycleDTO?> GetAssetLifecyclesForCustomerAsync(Guid customerId, Guid assetId)
        {
            var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId);
            if (assetLifecycle == null)
            {
                return null;
            }

            return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
        }

        public async Task<IList<CustomerLabel>> AddLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Label> labels)
        {
            try
            {
                List<CustomerLabel> customerLabels = new List<CustomerLabel>();
                foreach (Label label in labels)
                {
                    customerLabels.Add(new CustomerLabel(customerId, callerId, label));
                }

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
                IList<CustomerLabel> customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelGuids);
                if (customerLabels == null || customerLabels.Count == 0)
                {
                    throw new ResourceNotFoundException("No CustomerLabels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                IList<int> labelIds = new List<int>();
                foreach (CustomerLabel label in customerLabels)
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
                //var assetLifeCyclesForCustomer = _assetLifecycleRepository
                var customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelIds);

                if (customerLabels == null || customerLabels.Count == 0)
                {
                    throw new ResourceNotFoundException("No CustomerLabels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                foreach (CustomerLabel label in customerLabels)
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

                IList<CustomerLabel> customerLabels = await _assetLifecycleRepository.GetCustomerLabelsFromListAsync(labelGuids);
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

        public async Task<AssetLifecycleDTO> AddAssetLifecycleForCustomerAsync(Guid customerId, Guid callerId, string? alias, string? serialNumber, int assetCategoryId, string brand,
            string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<long> imei, string? macAddress,
            Guid? managedByDepartmentId, string? note, string? description, decimal? PaidByCompany)
        {
            AssetLifecycleStatus lifecycleStatus = AssetLifecycleStatus.Active;

            if (lifecycleType != LifecycleType.NoLifecycle)
            {
                lifecycleStatus = AssetLifecycleStatus.InputRequired;
            }

            var uniqueImeiList = new List<long>();
            //Validate list of IMEI and making sure that they are not duplicated for both MOBILE AND TABLET 
            if (imei.Any())
            {
                uniqueImeiList = AssetValidatorUtility.MakeUniqueIMEIList(imei);
                foreach (var i in uniqueImeiList)
                {
                    if (!AssetValidatorUtility.ValidateImei(i.ToString()))
                    {
                        throw new InvalidAssetDataException($"Invalid imei: {i}");
                    }
                }
            }
            var assetLifecycle = new AssetLifecycle
            {
                CustomerId = customerId,
                Alias = alias ?? string.Empty,
                AssetLifecycleStatus = lifecycleStatus,
                AssetLifecycleType = lifecycleType,
                PurchaseDate = purchaseDate,
                Note = note ?? string.Empty,
                Description = description ?? string.Empty,
                PaidByCompany = PaidByCompany ?? 0
            };

            Asset asset = assetCategoryId == 1
                ? new MobilePhone(Guid.NewGuid(), callerId, serialNumber ?? string.Empty, brand, productName,
                    uniqueImeiList.Select(i => new AssetImei(i)).ToList(), macAddress ?? string.Empty)
                : new Tablet(Guid.NewGuid(), callerId, serialNumber ?? string.Empty, brand, productName,
                    uniqueImeiList.Select(i => new AssetImei(i)).ToList(), macAddress ?? string.Empty);
            assetLifecycle.AssignAsset(asset, callerId);
            if (assetHolderId != null)
            {
                var user = await _assetLifecycleRepository.GetUser(assetHolderId.Value);
                assetLifecycle.AssignContractHolder(user != null ? user : new User { ExternalId = assetHolderId.Value },
                    callerId);
            }

            if (managedByDepartmentId != null)
            {
                assetLifecycle.AssignDepartment(managedByDepartmentId.Value, callerId);
            }

            //if (!newAsset.AssetPropertiesAreValid)
            //{
            //    StringBuilder exceptionMsg = new StringBuilder();
            //    foreach (string errorMsg in newAsset.ErrorMsgList)
            //    {
            //        if (errorMsg.Contains("Imei"))
            //        {
            //            exceptionMsg.Append($"Asset {errorMsg}" + " is invalid.\n");
            //        }
            //        else
            //        {
            //            exceptionMsg.Append($"Minimum asset data requirements not set: {errorMsg}" + ".\n");
            //        }
            //    }
            //    throw new InvalidAssetDataException(exceptionMsg.ToString());
            //}
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

        public async Task<AssetLifecycleDTO?> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, Guid callerId, LifecycleType newLifecycleType)
        {
            var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId);

            assetLifecycle.AssignLifecycleType(newLifecycleType, callerId);
            await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<AssetLifecycleDTO>(assetLifecycle);
        }

        public async Task<IList<AssetLifecycleDTO>> UpdateStatusForMultipleAssetLifecycles(Guid customerId, Guid callerId, IList<Guid> assetLifecycleIdList, AssetLifecycleStatus lifecycleStatus)
        {
            var assetLifecycles = await _assetLifecycleRepository.GetAssetLifecyclesFromListAsync(customerId, assetLifecycleIdList);
            if (assetLifecycles.Count == 0)
            {
                return new List<AssetLifecycleDTO>();
            }

            foreach (var assetLifecycle in assetLifecycles)
            {
                assetLifecycle.UpdateAssetStatus(lifecycleStatus, callerId);
            }

            await _assetLifecycleRepository.SaveEntitiesAsync();
            return _mapper.Map<IList<AssetLifecycleDTO>>(assetLifecycles);
        }

        public async Task<AssetLifecycleDTO> MakeAssetAvailableAsync(Guid customerId, Guid callerId, Guid assetLifeCycleId)
        {
            var updatedAssetLifeCycle = await _assetLifecycleRepository.MakeAssetAvailableAsync(customerId, callerId, assetLifeCycleId);          
            if(updatedAssetLifeCycle == null)
                throw new ResourceNotFoundException("No assets were found using the given AssetId. Did you enter the correct customer Id?", _logger);

            return _mapper.Map<AssetLifecycleDTO>(updatedAssetLifeCycle);
        }


        public async Task<AssetLifecycleDTO> UpdateAssetAsync(Guid customerId, Guid assetId, Guid callerId, string alias, string serialNumber, string brand, string model, DateTime purchaseDate, string note, string tag, string description, IList<long> imei)
        {
            var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId);
            var asset = assetLifecycle.Asset;

            if (!string.IsNullOrWhiteSpace(brand) && asset.Brand != brand)
            {
                asset.UpdateBrand(brand, callerId);
            }
            if (!string.IsNullOrWhiteSpace(model) && asset.ProductName != model)
            {
                asset.UpdateProductName(model, callerId);
            }

            if (!asset.AssetPropertiesAreValid)
            {
                StringBuilder exceptionMsg = new StringBuilder();
                foreach (string errorMsg in asset.ErrorMsgList)
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

        private void UpdateDerivedAssetType(Asset asset, string serialNumber, IList<long> imei, Guid callerId)
        {
            MobilePhone phone = asset as MobilePhone;
            if (phone != null)
            {
                if (phone.SerialNumber != serialNumber)
                {
                    phone.ChangeSerialNumber(serialNumber, callerId);
                }
                if (imei != null && phone.Imeis != imei)
                {
                    var uniqueListOfImeis = AssetValidatorUtility.MakeUniqueIMEIList(imei);
                    if (uniqueListOfImeis != null)
                    {
                        phone.SetImei(uniqueListOfImeis, callerId);
                    }
                }
            }

            Tablet tablet = asset as Tablet;
            if (tablet != null)
            {
                if (!string.IsNullOrWhiteSpace(serialNumber) && tablet.SerialNumber != serialNumber)
                {
                    tablet.ChangeSerialNumber(serialNumber, callerId);
                }
                if (imei != null && tablet.Imeis != imei)
                {
                    var uniqueListOfImeis = AssetValidatorUtility.MakeUniqueIMEIList(imei);
                    if (uniqueListOfImeis != null)
                    {
                        tablet.SetImei(uniqueListOfImeis, callerId);
                    }

                }
            }
        }

        public async Task<AssetLifecycleDTO?> AssignAsset(Guid customerId, Guid assetId, Guid userId, Guid callerId)
        {
            var assetLifecycle = await _assetLifecycleRepository.GetAssetLifecycleAsync(customerId, assetId);
            if (assetLifecycle == null) return null;
            var user = await _assetLifecycleRepository.GetUser(userId);
            assetLifecycle.AssignContractHolder(user != null ? user : new User { ExternalId = userId }, callerId);
            await _assetLifecycleRepository.SaveEntitiesAsync();
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
                    var auditLog = new AssetAuditLog(transactionGuid, @event.AssetLifecycle.ExternalId, @event.AssetLifecycle.CustomerId, logEventEntry.CreationTime, callerId,
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

        // From https://stackoverflow.com/a/9956981
        private static bool PropertyExist(dynamic dynamicObject, string name)
        {
            if (dynamicObject is ExpandoObject)
                return ((IDictionary<string, object>)dynamicObject).ContainsKey(name);

            return dynamicObject.GetType().GetProperty(name) != null;
        }
    }
}