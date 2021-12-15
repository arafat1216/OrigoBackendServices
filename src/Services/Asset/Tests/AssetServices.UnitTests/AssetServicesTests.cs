using AssetServices.Attributes;
using AssetServices.Exceptions;
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
            Assert.Equal(3, assetsFromUser.Count);
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
            Assert.Equal(1, assetsFromCompany);
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
            Assert.Equal(4, assetsFromUser.Items.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UpdateMultipleAssetsStatus_StatusChange_InputRequired()
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
            IList<Asset> updatedAssets = await assetService.UpdateMultipleAssetsStatus(COMPANY_ID, Guid.Empty, assetGuidList, AssetStatus.Active);

            // Assert
            Assert.Equal(3, updatedAssets.Count);
            Assert.Equal("alias_0", updatedAssets[0].Alias);
            Assert.Equal("alias_2", updatedAssets[1].Alias);
            Assert.Equal("alias_3", updatedAssets[2].Alias);
            Assert.Equal(AssetStatus.Active, updatedAssets[0].Status);
            Assert.Equal(AssetStatus.InputRequired, updatedAssets[1].Status);
            Assert.Equal(AssetStatus.InputRequired, updatedAssets[2].Status);
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
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), ASSETHOLDER_ONE_ID, new List<long>() { 458718920164666 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), "Test note", "tag", "description");
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
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, new List<long>() { 993100473611389 }, "a3:21:99:5d:a7:a0", null, "Unassigned asset", "tag", "description");
            var newAssetRead = await assetService.GetAssetForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

            // Assert
            Assert.NotNull(newAssetRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddAssetForCustomerAsync_NewAssetNoLifeCycle_AssetStatusShouldActive()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.NoLifecycle, new DateTime(2020, 1, 1), null, new List<long>() { 993100473611389 }, "a3:21:99:5d:a7:a2", null, "Unassigned asset", "tag", "description");
            
            // Assert
            Assert.Equal(AssetStatus.Active, newAsset.Status);
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddAssetForCustomerAsync_NewAssetWithLeasingLifecycle_AssetStatusShouldInputRequired()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.Leasing, new DateTime(2020, 1, 1), null, new List<long>() { 993100473611389 }, "a3:21:99:5d:a7:a1", null, "Unassigned asset", "tag", "description");

            // Assert
            Assert.Equal(AssetStatus.InputRequired, newAsset.Status);
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddAssetForCustomerAsync_IMEINot15Digits_ShouldGetInputRequired()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

           
            // Act and assert
            Assert.ThrowsAsync<InvalidAssetDataException>(() => assetService.AddAssetForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.NoLifecycle, new DateTime(2020, 1, 1), null, new List<long>() { 458718920164666 }, "a3:21:99:5d:a7:a1", null, "Unassigned asset", "tag", "description"));
           
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddAssetForCustomerAsync_IMEIwithNoElementInList_ShouldRetrunInputRequired()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            // Act
            var newAsset = await assetService.AddAssetForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.NoLifecycle, new DateTime(2020, 1, 1), null, new List<long>() { }, "a3:21:99:5d:a7:a1", null, "Unassigned asset", "tag", "description");

            // Assert
            Assert.Equal(AssetStatus.InputRequired, newAsset.Status);
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

            // Assert
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

            // Assert
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

            // Assert
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

            // Assert
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

            // Assert
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
            Asset asset = new MobilePhone(Guid.NewGuid(), COMPANY_ID, Guid.Empty, "alias_0", assetCategory, "4543534535344", "iPhone", "iPhone X", LifecycleType.BYOD,
                new DateTime(2020, 1, 1), null, new List<AssetImei>() { new AssetImei(111111987863622) }, "a3:21:99:5d:a7:a0", AssetStatus.Active, "note", "tag", "description", null);
            var attribute = new ImeiValidationAttribute();

            // Act
            bool valid = attribute.IsValid(asset);

            // Assert
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
            Asset asset = new MobilePhone(Guid.NewGuid(), COMPANY_ID, Guid.Empty, "alias_1", assetCategory, "4543534535344", "iPhone", "iPhone X", LifecycleType.BYOD,
                new DateTime(2020, 1, 1), null, new List<AssetImei>() { new AssetImei(357879702624426) }, "a3:21:99:5d:a7:a0", AssetStatus.Active, "note", "tag", "description", null);
            var attribute = new ImeiValidationAttribute();

            // Act
            bool valid = attribute.IsValid(asset);

            // Assert
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

            IList<CustomerLabel> savedLabels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);

            // Assert
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

            IList<CustomerLabel> savedLabels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);

            // Assert
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

            IList<CustomerLabel> labels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);
            labels[0].PatchLabel(Guid.Empty, new Label("Deprecated", LabelColor.Orange));
            labels[1].PatchLabel(Guid.Empty, new Label("Lost", LabelColor.Gray));
            
            // Act
            await assetService.UpdateLabelsForCustomerAsync(COMPANY_ID, labels);

            IList<CustomerLabel> savedLabels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);

            // Assert
            Assert.Equal(2, savedLabels.Count);
            Assert.Equal("Deprecated", savedLabels[0].Label.Text);
            Assert.Equal(LabelColor.Gray, savedLabels[1].Label.Color);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AssignLabelsForAsset()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            IList<CustomerLabel> labels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);
            IList<Guid> labelGuids = new List<Guid>();
            foreach (CustomerLabel label in labels)
            {
                labelGuids.Add(label.ExternalId);
            }

            Asset asset = await assetService.GetAssetForCustomerAsync(COMPANY_ID, ASSET_THREE_ID);
            IList<Guid> assetGuids = new List<Guid>
            {
                asset.ExternalId
            };

            // Act
            asset = (await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids))[0];

            // Assert
            Assert.Equal(labelGuids.Count, asset.AssetLabels.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UnAssignLabelsForAsset()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository);

            IList<CustomerLabel> labels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);
            IList<Guid> labelGuids = new List<Guid>();
            foreach (CustomerLabel label in labels)
            {
                labelGuids.Add(label.ExternalId);
            }

            Asset asset = await assetService.GetAssetForCustomerAsync(COMPANY_ID, ASSET_THREE_ID);
            IList<Guid> assetGuids = new List<Guid>
            {
                asset.ExternalId
            };

            await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids);
            
            
            // Act
            asset = (await assetService.UnAssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids))[0];

            // Assert
            foreach (AssetLabel al in asset.AssetLabels)
            {
                Assert.True(al.IsDeleted);
            }
        }
    }
}