using System;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AssetServices.UnitTests
{
    public class AssetServicesTests : AssetBaseTest
    {
        public AssetServicesTests() : base(new DbContextOptionsBuilder<AssetsContext>()
            .UseSqlite("Data Source=sqliteunittests.db").Options)
        {
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAssetsForUser_ForUserOne_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context);
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var assetsFromUser = await assetService.GetAssetsForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

            // Assert
            Assert.Equal(2, assetsFromUser.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAssetsForCustomer_ForOneCustomer_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context);
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var assetsFromUser = await assetService.GetAssetsForCustomerAsync(COMPANY_ID, string.Empty, 1, 10, new System.Threading.CancellationToken());

            // Assert
            Assert.Equal(3, assetsFromUser.Items.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void SaveAssetForCustomer_WithAssetHolderAndDepartmentAssigned()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context);
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), ASSETHOLDER_ONE_ID, true, "014239898330525", "5e:c4:33:df:61:70",
                Guid.NewGuid());
            var newAssetRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, newAsset.AssetId);

            // Assert
            Assert.NotNull(newAssetRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void SaveAssetForCustomer_WithoutAssetHolderAndDepartmentAssigned()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context);
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "993100473611389", "a3:21:99:5d:a7:a0", null);
            var newAssetRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, newAsset.AssetId);

            // Assert
            Assert.NotNull(newAssetRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void CreateAsset_ValidateAssetCategoryData()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context);
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            // custid is Guid.Empty
            var assetNoCustId = await assetService.AddAssetForCustomerAsync(Guid.Empty, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "993100473611389", "a3:21:99:5d:a7:a0", null);
            var assetNoCustIdRead = await assetService.GetAssetForCustomerAsync(Guid.Empty, assetNoCustId.AssetId);

            // brand is empty
            var assetBrandIsEmpty = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "993100473611389", "a3:21:99:5d:a7:a0", null);
            var assetBrandIsEmptyRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetBrandIsEmpty.AssetId);

            // brand is null
            var assetBrandIsNull = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                null, "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "993100473611389", "a3:21:99:5d:a7:a0", null);
            var assetBrandIsNullRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetBrandIsNull.AssetId);

            // model is empty
            var assetModelIsEmpty = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "993100473611389", "a3:21:99:5d:a7:a0", null);
            var assetModelIsEmptyRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetModelIsEmpty.AssetId);

            // model is null
            var assetModelIsNull = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", null, LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "993100473611389", "a3:21:99:5d:a7:a0", null);
            var assetModelIsNullRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetModelIsNull.AssetId);

            // Purchase date is DateTime.MinValue
            var assetPurchaseDateIsMinValue = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, DateTime.MinValue, null, true, "993100473611389", "a3:21:99:5d:a7:a0", null);
            var assetPurchaseDateIsMinValueRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetPurchaseDateIsMinValue.AssetId);

            // LifecycleType mobile phones - empty imei
            var assetEmptyImei = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "", "a3:21:99:5d:a7:a0", null);
            var assetEmptyImeiRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetEmptyImei.AssetId);

            // LifecycleType mobile phones - null imei
            var assetNullImei = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, null, "a3:21:99:5d:a7:a0", null);
            var assetNullImeiRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetNullImei.AssetId);

            // LifecycleType mobile phones - Invalid imei
            var assetInvalidImei = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "111111111111111", "a3:21:99:5d:a7:a0", null);
            var assetInvalidImeiRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetInvalidImei.AssetId);

            // LifecycleType mobile phones - Valid imei
            var assetValidImei = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "982513531736571", "a3:21:99:5d:a7:a0", null);
            var assetValidImeiRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, assetValidImei.AssetId);

            // Assert
            Assert.Null(assetNoCustIdRead);
            Assert.Null(assetBrandIsEmptyRead);
            Assert.Null(assetBrandIsNullRead);
            Assert.Null(assetModelIsEmptyRead);
            Assert.Null(assetModelIsNullRead);
            Assert.Null(assetPurchaseDateIsMinValueRead);
            Assert.Null(assetEmptyImeiRead);
            Assert.Null(assetNullImeiRead);
            Assert.Null(assetInvalidImeiRead);

            Assert.NotNull(assetValidImeiRead);
        }
    }
}