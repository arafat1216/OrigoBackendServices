using System;
using AssetServices.Exceptions;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Common.Enums;
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
        }
    }
}