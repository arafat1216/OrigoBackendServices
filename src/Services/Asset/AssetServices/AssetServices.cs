using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.DomainEvents;
using AssetServices.Exceptions;
using AssetServices.Models;
using Common.Enums;
using Common.Interfaces;
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
            string model, LifecycleType lifecycleType, DateTime purchaseDate, Guid? assetHolderId, bool isActive, string imei, string macAddress,
            Guid? managedByDepartmentId, AssetStatus status)
        {
            var assetCategory = await _assetRepository.GetAssetCategoryAsync(assetCategoryId);
            if (assetCategory == null)
            {
                throw new AssetCategoryNotFoundException();
            }

            var newAsset = new Asset(Guid.NewGuid(), customerId, serialNumber, assetCategory, brand, model,
                lifecycleType, purchaseDate, assetHolderId, isActive, imei, macAddress, status, managedByDepartmentId);

            if (!newAsset.AssetPropertiesAreValid)
            {
                StringBuilder exceptionMsg = new StringBuilder();
                foreach (string errorMsg in newAsset.ErrorMsgList)
                {
                    if (errorMsg.Contains("Imei"))
                    {
                        exceptionMsg.Append(string.Format("Asset {0}", errorMsg) + " is invalid.\n");
                    }
                    else
                    {
                        exceptionMsg.Append(string.Format("Minimum asset data requirements not set: {0}", errorMsg) + ".\n");
                    }
                }
                throw new InvalidAssetDataException(exceptionMsg.ToString());
            }
            newAsset.AddDomainEvent(new AssetCreatedDomainEvent(newAsset));
            return await _assetRepository.AddAsync(newAsset);
        }

        public async Task<IList<AssetLifecycle>> GetLifecycles()
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
            await _assetRepository.SaveChanges();
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
            await _assetRepository.SaveChanges();
            return asset;
        }

        public async Task<Asset> UpdateActiveStatus(Guid customerId, Guid assetId, bool isActive)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);
            if (asset == null)
            {
                return null;
            }

            asset.SetActiveStatus(isActive);
            await _assetRepository.SaveChanges();
            return asset;
        }

        public async Task<Asset> UpdateAssetAsync(Guid customerId, Guid assetId, string serialNumber, string brand, string model, DateTime purchaseDate)
        {
            var asset = await _assetRepository.GetAssetAsync(customerId, assetId);

            if (asset == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(serialNumber))
            {
                asset.ChangeSerialNumber(serialNumber);
            }
            if (!string.IsNullOrWhiteSpace(brand))
            {
                asset.UpdateBrand(brand);
            }
            if (!string.IsNullOrWhiteSpace(model))
            {
                asset.UpdateModel(model);
            }
            if (purchaseDate != default)
            {
                asset.ChangePurchaseDate(purchaseDate);
            }

            await _assetRepository.SaveChanges();
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
            await _assetRepository.SaveChanges();
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

        public async Task<IList<AssetAuditLog>> GetAssetAuditLog()
        {
            Guid assetId = Guid.NewGuid();
            // Get audit / Event log for asset
            var mockAuditData0 = new AssetAuditLog(Guid.NewGuid(),
                                                    assetId,
                                                    DateTime.Now,
                                                   "Mikael",
                                                   "Manual Registration",
                                                   "Procurement",
                                                   "New",
                                                   "Registered");

            var mockAuditData1 = new AssetAuditLog(Guid.NewGuid(),
                                                    assetId,
                                                    DateTime.Now,
                                                   "Mikael",
                                                   "Attribute change - Life Cycle Type: Bring your own device",
                                                   "Change",
                                                   "Registered",
                                                   "Active");

            var mockAuditData2 = new AssetAuditLog(Guid.NewGuid(),
                                                    assetId,
                                                    DateTime.Now,
                                                   "Mikael",
                                                   "Attribute change - Imei: 12345 12345 12345",
                                                   "Change",
                                                   "Active",
                                                   "Active");

            var mockAuditData3 = new AssetAuditLog(Guid.NewGuid(),
                                                    assetId,
                                                    DateTime.Now,
                                                   "Mikael",
                                                   "Reassigned to department: Product",
                                                   "Reassignment",
                                                   "Active",
                                                   "Active");
            var mockAuditData4 = new AssetAuditLog(Guid.NewGuid(),
                                                    assetId,
                                                    DateTime.Now,
                                                   "Mikael",
                                                   "Reassigned to user: Henrik Tveit",
                                                   "Reassignment",
                                                   "Active",
                                                   "Active");

            var mockAuditData5 = new AssetAuditLog(Guid.NewGuid(),
                                                    assetId,
                                                    DateTime.Now,
                                                   "Mikael",
                                                   "Attribute change - Tags: Personal",
                                                   "Change",
                                                   "Active",
                                                   "Active");

            var mockAuditData6 = new AssetAuditLog(Guid.NewGuid(),
                                                    assetId,
                                                    DateTime.Now,
                                                   "Mikael",
                                                   "Sent to repair",
                                                   "Hardware repair",
                                                   "Active",
                                                   "On repair");

            var mockAuditData7 = new AssetAuditLog(Guid.NewGuid(),
                                                    assetId,
                                                    DateTime.Now,
                                                   "Mikael",
                                                   "Status change",
                                                   "Hardware repair",
                                                   "On repair",
                                                   "Active");


            IList<AssetAuditLog> assetLogList = new List<AssetAuditLog>
            {
                mockAuditData0,
                mockAuditData1,
                mockAuditData2,
                mockAuditData3,
                mockAuditData4,
                mockAuditData5,
                mockAuditData6,
                mockAuditData7
            };

            return assetLogList;
        }
    }
}