using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Exceptions;
using AssetServices.Models;
using Common.Enums;
using Common.Interfaces;
using Common.Logging;
using Common.Models;
using Microsoft.Extensions.Logging;

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

        public async Task<Asset> GetAssetForCustomerAsync(Guid customerId, Guid assetId)
        {
            return await _assetRepository.GetAssetAsync(customerId, assetId);
        }

        public async Task<Asset> AddAssetForCustomerAsync(Guid customerId, string serialNumber, Guid assetCategoryId, string brand,
            string model, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, string imei, string macAddress,
            Guid? managedByDepartmentId, AssetStatus status, string note)
        {
            var assetCategory = await _assetRepository.GetAssetCategoryAsync(assetCategoryId);
            if (assetCategory == null)
            {
                throw new AssetCategoryNotFoundException();
            }

            var newAsset = new Asset(Guid.NewGuid(), customerId, serialNumber, assetCategory, brand, model,
                lifecycleType, purchaseDate, assetHolderId, imei, macAddress, status, note, managedByDepartmentId);

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
            Array arr = Enum.GetValues(typeof(LifecycleType));
            List<AssetLifecycle> assetLifecycles = new List<AssetLifecycle>();

            foreach (LifecycleType e in arr)
            {
                assetLifecycles.Add(new AssetLifecycle()
                {
                    Name = Enum.GetName(typeof(LifecycleType), e),
                    EnumValue = (int)e
                });
            }

            return assetLifecycles;
        }

        public async Task<Asset> ChangeAssetLifecycleTypeForCustomerAsync(Guid customerId, Guid assetId, LifecycleType newLifecycleType)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }

            asset.SetLifeCycleType(newLifecycleType);
            await _assetRepository.SaveEntitiesAsync();
            return asset;
        }

        public async Task<Asset> UpdateAssetStatus(Guid customerId, Guid assetId, AssetStatus status)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }

            asset.UpdateAssetStatus(status);
            await _assetRepository.SaveEntitiesAsync();
            return asset;
        }

        public async Task<IList<Asset>> UpdateMultipleAssetsStatus(Guid customerId, IList<Guid> assetGuidList, AssetStatus status)
        {
            var assets = await _assetRepository.GetAssetsFromListAsync(customerId, assetGuidList);
            if (assets == null)
            {
                return null;
            }

            foreach (Asset asset in assets)
            {
                asset.UpdateAssetStatus(status);
            }

            await _assetRepository.SaveEntitiesAsync();
            return assets;
        }

        public async Task<Asset> UpdateAssetAsync(Guid customerId, Guid assetId, string serialNumber, string brand, string model, DateTime purchaseDate, string note, string imei)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);

            if (asset == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(serialNumber) && asset.SerialNumber != serialNumber)
            {
                asset.ChangeSerialNumber(serialNumber);
            }
            if (!string.IsNullOrWhiteSpace(brand) && asset.Brand != brand)
            {
                asset.UpdateBrand(brand);
            }
            if (!string.IsNullOrWhiteSpace(model) && asset.Model != model)
            {
                asset.UpdateModel(model);
            }
            if (purchaseDate != default && asset.PurchaseDate != purchaseDate)
            {
                asset.ChangePurchaseDate(purchaseDate);
            }
            if (note != default && asset.Note != note)
            {
                asset.UpdateNote(note);
            }
            if (!string.IsNullOrWhiteSpace(imei) && asset.Imei != imei)
            {
                asset.SetImei(imei);
            }

            await _assetRepository.SaveEntitiesAsync();
            return asset;
        }

        public async Task<Asset> AssignAsset(Guid customerId, Guid assetId, Guid? userId)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }
            asset.AssignAssetToUser(userId);
            await _assetRepository.SaveEntitiesAsync();
            return asset;
        }

        public async Task<IList<AssetCategory>> GetAssetCategoriesAsync()
        {
            try
            {
                return await _assetRepository.GetAssetCategoriesAsync();
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
                var auditLog = new AssetAuditLog(transactionGuid, @event.Id, logEventEntry.CreationTime, "N/A",
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