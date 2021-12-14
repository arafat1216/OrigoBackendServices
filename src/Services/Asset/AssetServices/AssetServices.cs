using AssetServices.Exceptions;
using AssetServices.Models;
using Common.Enums;
using Common.Interfaces;
using Common.Logging;
using Common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AssetServices
{
    public class AssetServices : IAssetServices
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ILogger<AssetServices> _logger;

        public AssetServices(ILogger<AssetServices> logger, IAssetRepository assetRepository)
        {
            _logger = logger;
            _assetRepository = assetRepository;
        }

        public async Task<int> GetAssetsCountAsync(Guid customerId)
        {
            return await _assetRepository.GetAssetsCount(customerId);
        }

        public async Task<IList<Asset>> GetAssetsForUserAsync(Guid customerId, Guid userId)
        {
            return await _assetRepository.GetAssetsForUserAsync(customerId, userId);
        }

        public async Task<PagedModel<Asset>> GetAssetsForCustomerAsync(Guid customerId, string search, int page, int limit, CancellationToken cancellationToken)
        {
            try
            {
                return await _assetRepository.GetAssetsAsync(customerId, search, page, limit, cancellationToken);
            }
            catch (Exception exception)
            {
                throw new ReadingDataException(exception);
            }
        }

        public async Task<IList<Asset>> GetAssetsForCustomerFromListAsync(Guid customerId, IList<Guid> assetGuids)
        {
            try
            {
                return await _assetRepository.GetAssetsFromListAsync(customerId, assetGuids);
            }
            catch (Exception exception)
            {
                throw new ReadingDataException(exception);
            }
        }

        public async Task<Asset> GetAssetForCustomerAsync(Guid customerId, Guid assetId)
        {
            return await _assetRepository.GetAssetAsync(customerId, assetId);
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

                return await _assetRepository.AddCustomerLabelsForCustomerAsync(customerId, customerLabels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to delete CustomerLabels.");
                throw;
            }
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsForCustomerAsync(Guid customerId)
        {
            return await _assetRepository.GetCustomerLabelsForCustomerAsync(customerId);
        }

        public async Task<IList<CustomerLabel>> GetCustomerLabelsAsync(IList<Guid> customerLabelGuids)
        {
            return await _assetRepository.GetCustomerLabelsFromListAsync(customerLabelGuids);
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
                IList<CustomerLabel> customerLabels = await _assetRepository.GetCustomerLabelsFromListAsync(labelGuids);
                if (customerLabels == null || customerLabels.Count == 0)
                {
                    throw new ResourceNotFoundException("No CustomerLabels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                IList<int> labelIds = new List<int>();
                foreach (CustomerLabel label in customerLabels)
                {
                    labelIds.Add(label.Id);
                }

                await _assetRepository.DeleteLabelsFromAssetLabels(labelIds);
                return await _assetRepository.DeleteCustomerLabelsForCustomerAsync(customerId, customerLabels);
            }
            catch (ResourceNotFoundException ex)
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
        /// Set IsDeleted = 1 on CustomerLabels found by Id in <paramref name="labelGuids"/>.
        /// CustomerLabels still exist in database, but will not show up when fetching labels owned by customer
        /// </summary>
        /// <param name="customerId">External Id of customer whose labels we are soft deleting</param>
        /// <param name="callerId">Id of user who called endpoint to delete labels</param>
        /// <param name="labelGuids">External id of labels we are soft deleting</param>
        /// <returns></returns>
        public async Task<IList<CustomerLabel>> SoftDeleteLabelsForCustomerAsync(Guid customerId, Guid callerId, IList<Guid> labelGuids)
        {
            try
            {
                IList<CustomerLabel> customerLabels = await _assetRepository.GetCustomerLabelsFromListAsync(labelGuids);

                if (customerLabels == null || customerLabels.Count == 0)
                {
                    throw new ResourceNotFoundException("No CustomerLabels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                foreach (CustomerLabel label in customerLabels)
                {
                    label.SoftDelete(callerId);
                }

                await _assetRepository.SaveEntitiesAsync();
                return await _assetRepository.GetCustomerLabelsForCustomerAsync(customerId);
            }
            catch (ResourceNotFoundException ex)
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
        /// Set IsDeleted to true for Given AssetLabels
        /// </summary>
        /// <param name="callerId">Caller of call</param>
        /// <param name="assetLabelIds">AssetLabels to soft-delete</param>
        /// <returns></returns>
        public async Task SoftDeleteAssetLabelsAsync(Guid callerId, IList<int> assetLabelIds)
        {
            try
            {

                var assetLabels = await _assetRepository.GetAssetLabelsFromListAsync(assetLabelIds);
               
                foreach (AssetLabel label in assetLabels)
                {
                    label.SetActiveStatus(callerId, true);
                }

                await _assetRepository.SaveEntitiesAsync();
            }
            catch (ResourceNotFoundException ex)
            {
                throw; // no need to log same exception again
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to delete given AssetLabels.");
                throw;
            }
        }

        public async Task<IList<CustomerLabel>> UpdateLabelsForCustomerAsync(Guid customerId, IList<CustomerLabel> updateLabels)
        {
            return await _assetRepository.UpdateCustomerLabelsForCustomerAsync(customerId, updateLabels);
        }

        public async Task<IList<Asset>> AssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids, IList<Guid> labelGuids)
        {
            try
            {
                IList<Asset> assets = await _assetRepository.GetAssetsFromListAsync(customerId, assetGuids);
                if (assets == null || assets.Count == 0)
                {
                    throw new ResourceNotFoundException("No assets were found using the given AssetIds. Did you enter the correct customer Id?", _logger);
                }

                IList <CustomerLabel> customerLabels = await _assetRepository.GetCustomerLabelsFromListAsync(labelGuids);
                if (customerLabels == null)
                {
                    throw new ResourceNotFoundException("No labels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                foreach(Asset asset in assets)
                {
                    await AssignLabelsToAssetAsync(asset, customerLabels, callerId);
                }

                return assets;
            }
            catch (ResourceNotFoundException ex)
            {
                throw; // no need to log same exception again
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to assign given labels to assets.");
                throw;
            }
        }

        public async Task AssignLabelsToAssetAsync(Asset asset, IList<CustomerLabel> customerLabels, Guid callerId)
        {
            IList<AssetLabel> newLabels = new List<AssetLabel>();
            foreach (CustomerLabel customerLabel in customerLabels)
            {
                if (customerLabel.IsDeleted == false)
                {
                    var assetLabel = await _assetRepository.GetAssetLabelForAssetAsync(asset.Id, customerLabel.Id);
                    if (assetLabel == null)
                    {
                        newLabels.Add(new AssetLabel(asset.Id, customerLabel.Id, callerId));
                    }
                    else if (assetLabel.IsDeleted == true)
                    {
                        assetLabel.SetActiveStatus(callerId, false);
                    }
                    // else, label already assigned
                }
            }

            await _assetRepository.AddAssetLabelsForAsset(newLabels);
        }

        public async Task<IList<Asset>> UnAssignLabelsToAssetsAsync(Guid customerId, Guid callerId, IList<Guid> assetGuids, IList<Guid> labelGuids)
        {
            try
            {
                IList<Asset> assets = await _assetRepository.GetAssetsFromListAsync(customerId, assetGuids);
                if (assets == null || assets.Count == 0)
                {
                    throw new ResourceNotFoundException("No assets were found using the given AssetIds. Did you enter the correct customer Id?", _logger);
                }

                IList<CustomerLabel> customerLabels = await _assetRepository.GetCustomerLabelsFromListAsync(labelGuids);
                if (customerLabels == null || customerLabels.Count == 0)
                {
                    throw new ResourceNotFoundException("No labels were found using the given LabelIds. Did you enter the correct customer Id?", _logger);
                }

                foreach (Asset asset in assets)
                {
                    await UnAssignLabelsToAssetAsync(asset, customerLabels, callerId);
                }

                await _assetRepository.SaveEntitiesAsync();
                return await _assetRepository.GetAssetsFromListAsync(customerId, assetGuids);
            }
            catch(ResourceNotFoundException ex)
            {
                throw; // no need to log same exception again
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unknown error. Unable to unassign given labels to assets.");
                throw;
            }
        }

        public async Task UnAssignLabelsToAssetAsync(Asset asset, IList<CustomerLabel> customerLabels, Guid callerId)
        {
            foreach (CustomerLabel customerLabel in customerLabels)
            {
                if (customerLabel.IsDeleted == false)
                {
                    var assetLabel = await _assetRepository.GetAssetLabelForAssetAsync(asset.Id, customerLabel.Id);

                    // Update if necessary
                    if (assetLabel != null && assetLabel.IsDeleted == false)
                    {
                        assetLabel.SetActiveStatus(callerId, true); 
                    }
                }
            }
        }

        public async Task<Asset> AddAssetForCustomerAsync(Guid customerId, Guid callerId, string alias, string serialNumber, int assetCategoryId, string brand,
            string productName, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, IList<long> imei, string macAddress,
            Guid? managedByDepartmentId, AssetStatus status, string note, string tag, string description)
        {
            var assetCategory = await _assetRepository.GetAssetCategoryAsync(assetCategoryId);
            if (assetCategory == null)
            {
                throw new AssetCategoryNotFoundException();
            }

            Asset newAsset;
            if (assetCategory.Id == 1)
            {
                newAsset = new MobilePhone(Guid.NewGuid(), customerId, callerId, alias, assetCategory, serialNumber, brand, productName,
                lifecycleType, purchaseDate, assetHolderId, imei?.Select(i => new AssetImei(i)).ToList(), macAddress, status, note, tag, description, managedByDepartmentId);
            }
            else
            {
                newAsset = new Tablet(Guid.NewGuid(), customerId, callerId, alias, assetCategory, serialNumber, brand, productName,
                lifecycleType, purchaseDate, assetHolderId, imei?.Select(i => new AssetImei(i)).ToList(), macAddress, status, note, tag, description, managedByDepartmentId);
            }

            if (!newAsset.AssetPropertiesAreValid)
            {
                StringBuilder exceptionMsg = new StringBuilder();
                foreach (string errorMsg in newAsset.ErrorMsgList)
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
            return await _assetRepository.AddAsync(newAsset);
        }

        public IList<AssetLifecycle> GetLifecycles()
        {
            Array arr = Enum.GetValues(typeof(Common.Enums.LifecycleType));
            IList<AssetLifecycle> assetLifecycles = new List<AssetLifecycle>();

            foreach (Common.Enums.LifecycleType e in arr)
            {
                assetLifecycles.Add(new AssetLifecycle()
                {
                    Name = Enum.GetName(typeof(Common.Enums.LifecycleType), e),
                    EnumValue = (int)e
                });
            }

            return assetLifecycles;
        }

        public async Task<Asset> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, Guid callerId, LifecycleType newLifecycleType)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }

            asset.SetLifeCycleType(newLifecycleType, callerId);
            await _assetRepository.SaveEntitiesAsync();
            return asset;
        }

        public async Task<IList<Asset>> UpdateMultipleAssetsStatus(Guid customerId, Guid callerId, IList<Guid> assetGuidList, AssetStatus status)
        {
            var assets = await _assetRepository.GetAssetsFromListAsync(customerId, assetGuidList);
            if (assets == null || assets.Count == 0)
            {
                return null;
            }

            foreach (Asset asset in assets)
            {
                asset.UpdateAssetStatus(status, callerId);
            }

            await _assetRepository.SaveEntitiesAsync();
            return assets;
        }

        public async Task<Asset> UpdateAssetAsync(Guid customerId, Guid assetId, Guid callerId, string alias, string serialNumber, string brand, string model, DateTime purchaseDate, string note, string tag, string description, IList<long> imei)
        {
            Asset asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(brand) && asset.Brand != brand)
            {
                asset.UpdateBrand(brand, callerId);
            }
            if (!string.IsNullOrWhiteSpace(model) && asset.ProductName != model)
            {
                asset.UpdateProductName(model, callerId);
            }
            if (purchaseDate != default && asset.PurchaseDate != purchaseDate)
            {
                asset.ChangePurchaseDate(purchaseDate, callerId);
            }
            if (note != default && asset.Note != note)
            {
                asset.UpdateNote(note, callerId);
            }
            if (tag != default && asset.AssetTag != tag)
            {
                asset.UpdateTag(tag, callerId);
            }
            if (description != default && asset.Description != description)
            {
                asset.UpdateDescription(description, callerId);
            }
            if (!string.IsNullOrWhiteSpace(alias) && asset.Alias != alias)
            {
                asset.SetAlias(alias, callerId);
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

            await _assetRepository.SaveEntitiesAsync();
            return asset;
        }

        private void UpdateDerivedAssetType(Asset asset, string serialNumber, IList<long> imei, Guid callerId)
        {
            MobilePhone phone = asset as MobilePhone;
            if (phone != null)
            {
                if (!string.IsNullOrWhiteSpace(serialNumber) && phone.SerialNumber != serialNumber)
                {
                    phone.ChangeSerialNumber(serialNumber, callerId);
                }
                if (phone.Imeis != imei)
                {
                    phone.SetImei(imei);
                }
            }

            Tablet tablet = asset as Tablet;
            if (tablet != null)
            {
                if (!string.IsNullOrWhiteSpace(serialNumber) && tablet.SerialNumber != serialNumber)
                {
                    tablet.ChangeSerialNumber(serialNumber, callerId);
                }
                if (tablet.Imeis != imei)
                {
                    tablet.SetImei(imei);
                }
            }
        }

        public async Task<Asset> AssignAsset(Guid customerId, Guid assetId, Guid? userId, Guid callerId)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }
            asset.AssignAssetToUser(userId, callerId);
            await _assetRepository.SaveEntitiesAsync();
            return asset;
        }

        public async Task<IList<AssetCategory>> GetAssetCategoriesAsync(string language = "EN")
        {
            try
            {
                return await _assetRepository.GetAssetCategoriesAsync(language);
            }
            catch (Exception exception)
            {
                throw new ReadingDataException(exception);
            }
        }

        public async Task<IList<AssetAuditLog>> GetAssetAuditLog(Guid assetId)
        {

            var logEventEntries = await _assetRepository.GetAuditLog(assetId);
            var assetLogList = new List<AssetAuditLog>();

            foreach (var logEventEntry in logEventEntries)
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
                    : @event.Asset.Status.ToString();
                var callerId = PropertyExist(@event, "CallerId")
                    ? @event.CallerId.ToString()
                    : "N/A";
                var auditLog = new AssetAuditLog(transactionGuid, @event.Asset.ExternalId, logEventEntry.CreationTime, callerId,
                    ((IEvent)@event).EventMessage(), logEventEntry.EventTypeShortName, previousStatus, @event.Asset.Status.ToString());
                assetLogList.Add(auditLog);
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