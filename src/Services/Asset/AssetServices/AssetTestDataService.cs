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
                builder.Append(await CreateAssetsData());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return builder.ToString();
        }

        public async Task<string> CreateAssetsData()
        {
            string errorMessage = string.Empty;
            try
            {
                var categories = await _assetRepository.GetAssetCategoriesAsync();
                // 1 = Mobile Phone, 2 = Tablet 
                var assets = Seed.GetCustomersData(categories.First(c => c.Id == 1), categories.First(c => c.Id == 2));
                foreach (var asset in assets)
                {
                    var existingAsset = await _assetRepository.GetAssetAsync(asset.CustomerId, asset.ExternalId);
                    if (existingAsset == null)
                    {
                        await _assetRepository.AddAsync(asset);
                    }
                    else
                    {
                        existingAsset.UpdateBrand(asset.Brand);
                        existingAsset.UpdateProductName(asset.ProductName);
                        existingAsset.ChangePurchaseDate(asset.PurchaseDate);
                        existingAsset.UpdateNote(asset.Note);
                        existingAsset.UpdateTag(asset.AssetTag);
                        existingAsset.UpdateDescription(asset.Description);
                        existingAsset.AssignAssetToUser(asset.AssetHolderId);
                        existingAsset.UpdateAssetStatus(asset.Status);
                        existingAsset.SetLifeCycleType(asset.LifecycleType);
                        if (asset.AssetCategory.Id == 1)
                        {
                            MobilePhone phone = existingAsset as MobilePhone;
                            var a = asset as MobilePhone;
                            phone.ChangeSerialNumber(a.SerialNumber);
                            phone.SetImei(a.Imeis.Select(i => i.Imei).ToList());
                        }
                        else if (asset.AssetCategory.Id == 2)
                        {
                            Tablet tablet = existingAsset as Tablet;
                            var a = asset as Tablet;
                            tablet.ChangeSerialNumber(a.SerialNumber);
                            tablet.SetImei(a.Imeis.Select(i => i.Imei).ToList());
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
    }
}
