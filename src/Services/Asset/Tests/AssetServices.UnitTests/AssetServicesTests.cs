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
using System.Collections.Generic;
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
        public async void GetAssetsCount_ForCompany_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID);

            // Assert
            Assert.Equal(3, assetsFromCompany);
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
        public async void SetAssetStatus_ForUserOne_Active()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);
            var assetsFromUser = await assetService.GetAssetsForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

            IList<Guid> assetGuidList = new List<Guid>();
            foreach (Asset asset in assetsFromUser)
            {
                assetGuidList.Add(asset.ExternalId);
            }

            // Act
            IList<Asset> updatedAssets = await assetService.UpdateMultipleAssetsStatus(COMPANY_ID, assetGuidList, AssetStatus.Active);

            // Assert
            Assert.Equal(2, updatedAssets.Count);
            Assert.Equal(AssetStatus.Active, updatedAssets[0].Status);
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
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), ASSETHOLDER_ONE_ID, new List<long>() { 014239898330525 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), AssetStatus.Active, "Test note", "tag", "description");
            var newAssetRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

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
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, new List<long>() { 993100473611389 }, "a3:21:99:5d:a7:a0", null, AssetStatus.Active, "Unassigned asset", "tag", "description");
            var newAssetRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

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
        public void ValidateImei_invalid_empty_single()
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
        public void ValidateImei_invalid_single()
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
        public void ValidateImei_valid_single()
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
        public void ValidateImei_valid__multiple()
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
        public void ValidateImei_invalid__multiple()
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
            Asset asset = new MobilePhone(Guid.NewGuid(), COMPANY_ID, "alias_0", assetCategory, "4543534535344", "iPhone", "iPhone X", LifecycleType.BYOD,
                new DateTime(2020, 1, 1), null, new List<AssetImei>() { new AssetImei(111111987863622) }, "a3:21:99:5d:a7:a0", AssetStatus.Active, "note", "tag", "description", null);
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
            Asset asset = new MobilePhone(Guid.NewGuid(), COMPANY_ID, "alias_1", assetCategory, "4543534535344", "iPhone", "iPhone X", LifecycleType.BYOD,
                new DateTime(2020, 1, 1), null, new List<AssetImei>() { new AssetImei(357879702624426) }, "a3:21:99:5d:a7:a0", AssetStatus.Active, "note", "tag", "description", null);
            var attribute = new ImeiValidationAttribute();

            // Act
            bool valid = attribute.IsValid(asset);

            // Asset
            Assert.True(valid);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void CreateLabelsForCustomer()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);
            
            IList<Label> labelsToAdd = new List<Label>();
            labelsToAdd.Add(new Label("Repair", LabelColor.Red));
            labelsToAdd.Add(new Label("Field", LabelColor.Blue));

            // Act
            await assetService.AddLabelsForCustomerAsync(COMPANY_ID, Guid.Empty, labelsToAdd);

            IList<CustomerLabel> savedLabels = await assetService.GetLabelsForCustomerAsync(COMPANY_ID);

            // Asset
            Assert.Equal(4, savedLabels.Count); // 2 made here, 2 made in AssetBaseTest
            Assert.Equal("Repair", savedLabels[2].Label.Text);
            Assert.Equal(LabelColor.Blue, savedLabels[3].Label.Color);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void DeleteLabelsForCustomer()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            await assetService.DeleteLabelsForCustomerAsync(COMPANY_ID, new List<Guid> { LABEL_ONE_ID, LABEL_TWO_ID });

            IList<CustomerLabel> savedLabels = await assetService.GetLabelsForCustomerAsync(COMPANY_ID);

            // Asset
            Assert.Equal(0, savedLabels.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UpdateLabelsForCustomer()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            IList<CustomerLabel> labels = await assetService.GetLabelsForCustomerAsync(COMPANY_ID);
            labels[0].PatchLabel(Guid.Empty, new Label("Deprecated", LabelColor.Orange));
            labels[1].PatchLabel(Guid.Empty, new Label("Lost", LabelColor.Gray));
            
            // Act
            await assetService.UpdateLabelsForCustomerAsync(COMPANY_ID, labels);

            IList<CustomerLabel> savedLabels = await assetService.GetLabelsForCustomerAsync(COMPANY_ID);

            // Asset
            Assert.Equal(2, savedLabels.Count);
            Assert.Equal("Deprecated", savedLabels[0].Label.Text);
            Assert.Equal(LabelColor.Gray, savedLabels[1].Label.Color);
        }
    }
}