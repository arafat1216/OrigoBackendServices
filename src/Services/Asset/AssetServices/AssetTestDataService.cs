using AssetServices.Models;
using CustomerServices.SeedData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices
{
    public class AssetTestDataService : IAssetTestDataService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly ILogger<AssetTestDataService> _logger;
        private static Guid OrganizationId1 { get; set; } = new Guid("A19EA756-86F2-423C-9B10-11CB10181858");
        private static Guid OrganizationId2 { get; set; } = new Guid("F2B5B8E5-78E1-4643-B97B-49239DAC74C2");
        private readonly Guid _callerId = new("D0326090-631F-4138-9CD2-85249AD24BBB");

        public AssetTestDataService(ILogger<AssetTestDataService> logger, IAssetRepository assetRepository)
        {
            _assetRepository = assetRepository;
            _logger = logger;
        }

        public async Task<string> CreateAssetTestData()
        {
            StringBuilder builder = new();
            try
            {
                builder.Append(await CreateAssetLables());
                builder.Append(await CreateAssetsData());
                builder.Append(await AssignLabelsToAssets());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return builder.ToString();
        }

        private async Task<string> CreateAssetLables()
        {
            string errorMessage = string.Empty;
            Guid[] organizationIds = new Guid[] { OrganizationId1, OrganizationId2 };
            try
            {
                foreach (Guid organizationId in organizationIds)
                {
                    var customerLabels = Seed.GetCustomerLables(organizationId);
                    var existingLabels = await _assetRepository.GetCustomerLabelsForCustomerAsync(organizationId);
                    foreach (var label in customerLabels)
                    {
                        if (existingLabels.FirstOrDefault(l => l.ExternalId == label.ExternalId) == null)
                        {
                            await _assetRepository.AddCustomerLabelsForCustomerAsync(organizationId, new List<CustomerLabel>() { label });
                        }
                        else
                        {
                            await _assetRepository.UpdateCustomerLabelsForCustomerAsync(organizationId, new List<CustomerLabel>() { label });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                errorMessage = "Customer Label creation exception\r\n";
            }
            return errorMessage;
        }

        private async Task<string> CreateAssetsData()
        {
            string errorMessage = string.Empty;
            try
            {
                var categories = await _assetRepository.GetAssetCategoriesAsync();
                // 1 = Mobile Phone, 2 = Tablet 
                var assets = Seed.GetAssetData(categories.First(c => c.Id == 1), categories.First(c => c.Id == 2));
                foreach (var asset in assets)
                {
                    var existingAsset = await _assetRepository.GetAssetAsync(asset.CustomerId, asset.ExternalId);
                    if (existingAsset == null)
                    {
                        await _assetRepository.AddAsync(asset);
                    }
                    else
                    {
                        existingAsset.UpdateBrand(asset.Brand, _callerId);
                        existingAsset.UpdateProductName(asset.ProductName, _callerId);
                        existingAsset.ChangePurchaseDate(asset.PurchaseDate, _callerId);
                        existingAsset.UpdateNote(asset.Note, _callerId);
                        existingAsset.UpdateTag(asset.AssetTag, _callerId);
                        existingAsset.UpdateDescription(asset.Description, _callerId);
                        existingAsset.AssignAssetToUser(asset.AssetHolderId, _callerId);
                        existingAsset.UpdateAssetStatus(asset.Status, _callerId);
                        existingAsset.SetLifeCycleType(asset.LifecycleType, _callerId);
                        if (asset.AssetCategory.Id == 1)
                        {
                            MobilePhone phone = existingAsset as MobilePhone;
                            var a = asset as MobilePhone;
                            phone.ChangeSerialNumber(a.SerialNumber, _callerId);
                            phone.SetImei(a.Imeis.Select(i => i.Imei).ToList(), _callerId);
                        }
                        else if (asset.AssetCategory.Id == 2)
                        {
                            Tablet tablet = existingAsset as Tablet;
                            var a = asset as Tablet;
                            tablet.ChangeSerialNumber(a.SerialNumber, _callerId);
                            tablet.SetImei(a.Imeis.Select(i => i.Imei).ToList(), _callerId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                errorMessage = "Asset creation exception\r\n";
            }
            return errorMessage;
        }

        private async Task<string> AssignLabelsToAssets()
        {
            string errorMessage = string.Empty;
            try
            {
                var assetsAndLabels = Seed.LabelsForAssets();
                IList<AssetLabel> newLabels = new List<AssetLabel>();
                var categories = await _assetRepository.GetAssetCategoriesAsync();
                // 1 = Mobile Phone, 2 = Tablet 
                var assets = Seed.GetAssetData(categories.First(c => c.Id == 1), categories.First(c => c.Id == 2));
                foreach (var assetId in assetsAndLabels.Keys)
                {
                    var asset = await _assetRepository.GetAssetAsync(assets.FirstOrDefault(a => a.ExternalId == assetId).CustomerId, assetId);
                    var customerLabel = await _assetRepository.GetCustomerLabelAsync(assetsAndLabels[assetId]);
                    if (asset != null && customerLabel != null)
                    {
                        var assetLabel = await _assetRepository.GetAssetLabelForAssetAsync(asset.Id, customerLabel.Id);
                        if (assetLabel == null)
                        {
                            newLabels.Add(new AssetLabel(asset.Id, customerLabel.Id, _callerId));
                        }
                    }
                }
                await _assetRepository.AddAssetLabelsForAsset(newLabels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                errorMessage = "Asset Label assignment exception\r\n";
            }
            return errorMessage;
        }
    }
}
