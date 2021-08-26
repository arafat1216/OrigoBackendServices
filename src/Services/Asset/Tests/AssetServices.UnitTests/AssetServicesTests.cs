using AssetServices.Attributes;
using AssetServices.Infrastructure;
using AssetServices.Models;
using AssetServices.Utility;
using Common.Enums;
using Common.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
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
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
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
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
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
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), ASSETHOLDER_ONE_ID, true, "014239898330525", "5e:c4:33:df:61:70",
                Guid.NewGuid(), AssetStatus.Active, "Test note");
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
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "993100473611389", "a3:21:99:5d:a7:a0", null, AssetStatus.Active, "Unassigned asset");
            var newAssetRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, newAsset.AssetId);

            // Assert
            Assert.NotNull(newAssetRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public void CreateAsset_ValidateAssetCategoryData()
        {
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void ValidateImei_invalid_empty_single()
        {
            // Arrange
            string imei = "";

            // Act
            bool valid = AssetValidatorUtility.ValidateImei(imei);

            // Asset
            Assert.True(!valid);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void ValidateImei_invalid_single()
        {
            // Arrange
            string imei = "111111111111111";

            // Act
            bool valid = AssetValidatorUtility.ValidateImei(imei);

            // Asset
            Assert.False(valid);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void ValidateImei_valid_single()
        {
            // Arrange
            string imei = "532618333994628";

            // Act
            bool valid = AssetValidatorUtility.ValidateImei(imei);

            // Asset
            Assert.True(valid);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void ValidateImei_valid__multiple()
        {
            // Arrange
            string imeis = "337047052140527,548668589912669,010708141304465";

            // Act
            bool valid = AssetValidatorUtility.ValidateImeis(imeis);

            // Asset
            Assert.True(valid);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void ValidateImei_invalid__multiple()
        {
            // Arrange
            string imeis = "33704705214052,548668589912669,0107081413044651";

            // Act
            bool valid = AssetValidatorUtility.ValidateImeis(imeis);

            // Asset
            Assert.False(valid);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void ImeiValidationAttribute_Invalid()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetCategory = await assetRepository.GetAssetCategoryAsync(ASSET_CATEGORY_ID);
            Asset asset = new Asset(Guid.NewGuid(), COMPANY_ID, "4543534535344", assetCategory,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "", "a3:21:99:5d:a7:a0", AssetStatus.Active, null, null);
            var attribute = new ImeiValidationAttribute();

            // Act
            bool valid = attribute.IsValid(asset);

            // Asset
            Assert.False(valid);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void ImeiValidationAttribute_Valid()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetCategory = await assetRepository.GetAssetCategoryAsync(ASSET_CATEGORY_ID);
            Asset asset = new Asset(Guid.NewGuid(), COMPANY_ID, "4543534535344", assetCategory,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, true, "993100473611389", "a3:21:99:5d:a7:a0", AssetStatus.Active, null, null);
            var attribute = new ImeiValidationAttribute();

            // Act
            bool valid = attribute.IsValid(asset);

            // Asset
            Assert.True(valid);
        }
    }
}