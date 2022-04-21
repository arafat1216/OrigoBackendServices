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
using System.Linq;
using System.Reflection;
using AssetServices.ServiceModel;
using AutoMapper;
using Xunit;
using System.Threading.Tasks;

namespace AssetServices.UnitTests
{
    public class AssetServicesTests : AssetBaseTest
    {
        private static IMapper _mapper;

        public AssetServicesTests() : base(new DbContextOptionsBuilder<AssetsContext>()
            .UseSqlite("Data Source=sqliteunittests.db").Options)
        {
            if (_mapper == null)
            {
                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddMaps(Assembly.GetAssembly(typeof(AssetDTO)));
                });
                _mapper = mappingConfig.CreateMapper();
            }
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAssetsForUser_ForUserOne_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var assetsFromUser = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

            // Assert
            Assert.Equal(3, assetsFromUser.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAssetsCount_ForCompany_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

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
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, string.Empty, 1, 10, null, new System.Threading.CancellationToken());

            // Assert
            Assert.Equal(4, assetsFromUser.Items.Count);

            // search with serial key
            assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, "123456789012399", 1, 10, null,  new System.Threading.CancellationToken());

            // Assert 
            Assert.Equal(2, assetsFromUser.Items.Count);

            // search with IMEI
            assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, "512217111821626", 1, 10, null, new System.Threading.CancellationToken());

            // Assert 
            Assert.Equal(2, assetsFromUser.Items.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UpdateMultipleAssetsStatus_StatusChange_InputRequired()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
            var assetLifecyclesFromUser = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

            IList<Guid> assetGuidList = assetLifecyclesFromUser.Select(assetLifecycle => assetLifecycle.ExternalId).ToList();

            // Act
            var updatedAssetsLifecycles = await assetService.UpdateStatusForMultipleAssetLifecycles(COMPANY_ID, Guid.Empty, assetGuidList, AssetLifecycleStatus.Active);

            // Assert
            Assert.Equal(3, updatedAssetsLifecycles.Count);
            Assert.Equal("alias_0", updatedAssetsLifecycles[0].Alias);
            Assert.Equal("alias_2", updatedAssetsLifecycles[1].Alias);
            Assert.Equal("alias_3", updatedAssetsLifecycles[2].Alias);
            Assert.Equal(AssetLifecycleStatus.Active, updatedAssetsLifecycles[0].AssetLifecycleStatus);
            Assert.Equal(AssetLifecycleStatus.Active, updatedAssetsLifecycles[1].AssetLifecycleStatus);
            Assert.Equal(AssetLifecycleStatus.Active, updatedAssetsLifecycles[2].AssetLifecycleStatus);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void SaveAssetForCustomer_WithAssetHolderAndDepartmentAssigned()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), ASSETHOLDER_ONE_ID, new List<long>() { 458718920164666 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), "Test note", "description",null);
            var newAssetRead = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

            // Assert
            Assert.NotNull(newAssetRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void SaveAssetForCustomer_WithoutAssetHolderAndDepartmentAssigned()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), null, new List<long>() { 993100473611389 }, "a3:21:99:5d:a7:a0", null, "Unassigned asset", "description", null);
            var newAssetRead = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

            // Assert
            Assert.NotNull(newAssetRead);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void SaveAssetForCustomer_WithPaidByCompany()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), ASSETHOLDER_ONE_ID, new List<long>() { 458718920164666 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), "Test note", "description", 20.33M);
            var newAssetRead = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

            // Assert
            Assert.NotNull(newAssetRead);
            Assert.Equal(20.33M, newAssetRead.PaidByCompany);

        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void SaveAssetForCustomer_WithoutPaidByCompany()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.BYOD, new DateTime(2020, 1, 1), ASSETHOLDER_ONE_ID, new List<long>() { 458718920164666 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), "Test note", "description", null);
            var newAssetRead = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

            // Assert
            Assert.NotNull(newAssetRead);
            Assert.Equal(0, newAssetRead.PaidByCompany);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddAssetForCustomerAsync_NewAssetNoLifeCycle_AssetStatusShouldActive()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.NoLifecycle, new DateTime(2020, 1, 1), null, new List<long>() { 993100473611389 }, "a3:21:99:5d:a7:a2", null, "Unassigned asset", "description", null);

            // Assert
            Assert.Equal(AssetLifecycleStatus.Active, newAsset.AssetLifecycleStatus);
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddAssetForCustomerAsync_NewAssetWithLeasingLifecycle_AssetStatusShouldInputRequired()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.Leasing, new DateTime(2020, 1, 1), null, new List<long>() { 993100473611389 }, "a3:21:99:5d:a7:a1", null, "Unassigned asset", "description", null);

            // Assert
            Assert.Equal(AssetLifecycleStatus.InputRequired, newAsset.AssetLifecycleStatus);
        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddAssetForCustomerAsync_IMEINot15Digits_ShouldGetInputRequired()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);


            // Act and assert
            await Assert.ThrowsAsync<InvalidAssetDataException>(() => assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.NoLifecycle, new DateTime(2020, 1, 1), null, new List<long>() { 45871892016466 }, "a3:21:99:5d:a7:a1", null, "Unassigned asset", "description", null));

        }
        [Fact]
        [Trait("Category", "UnitTest")]
        public async void AddAssetForCustomerAsync_IMEIwithNoElementInList_ShouldReturnInputRequired()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.NoLifecycle, new DateTime(2020, 1, 1), null, new List<long>() { }, "a3:21:99:5d:a7:a1", null, "Unassigned asset", "description", null);

            // Assert
            Assert.Equal(AssetLifecycleStatus.InputRequired, newAsset.AssetLifecycleStatus);
        }


        [Fact]
        [Trait("Category", "UnitTest")]
        public void MakeUniqueIMEIList_WithDuplicatedValues_OnlyReturn2()
        {
            long number1 = 106699671963280;
            long number2 = 102274227461256;
            List<long> listOfImeis = new List<long>
            {
                number1,
                number2,
                number1,
                number1,
                number2

            };

            var uniqueIMEIList = AssetValidatorUtility.MakeUniqueIMEIList(listOfImeis);
            Assert.Equal(2, uniqueIMEIList.Count);
            Assert.Equal(uniqueIMEIList[0], number1);
            Assert.Equal(uniqueIMEIList[1], number2);
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
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetCategory = new AssetCategory();
            Asset asset = new MobilePhone(Guid.NewGuid(), Guid.Empty, "4543534535344", "iPhone", "iPhone X", new List<AssetImei>() { new AssetImei(111111987863622) }, "a3:21:99:5d:a7:a0");
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
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetCategory = new AssetCategory();
            Asset asset = new MobilePhone(Guid.NewGuid(), Guid.Empty, "4543534535344", "iPhone", "iPhone X", new List<AssetImei>() { new AssetImei(357879702624426) }, "a3:21:99:5d:a7:a0");
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
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

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
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

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
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

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
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            IList<CustomerLabel> labels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);
            IList<Guid> labelGuids = new List<Guid>();
            foreach (CustomerLabel label in labels)
            {
                labelGuids.Add(label.ExternalId);
            }

            var assetLifecycle = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_THREE_ID);
            IList<Guid> assetGuids = new List<Guid>
            {
                assetLifecycle.ExternalId
            };

            // Act
            assetLifecycle = (await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids))[0];

            // Assert
            Assert.Equal(labelGuids.Count, assetLifecycle.Labels.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void UnAssignLabelsForAsset()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            IList<CustomerLabel> labels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);
            IList<Guid> labelGuids = new List<Guid>();
            foreach (CustomerLabel label in labels)
            {
                labelGuids.Add(label.ExternalId);
            }

            var assetLifecycle = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_THREE_ID);
            IList<Guid> assetGuids = new List<Guid>
            {
                assetLifecycle.ExternalId
            };

            await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids);


            // Act
            assetLifecycle = (await assetService.UnAssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids))[0];

            //// Assert
            //foreach (AssetLifecycleLabel al in asset.AssetLabels)
            //{
            //    Assert.True(al.IsDeleted);
            //}
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task UnAssignAssetLifecyclesForUser_Valid()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            var departmentId = Guid.NewGuid();

            await assetService.UnAssignAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID, departmentId, CALLER_ID);

            var assetLifeCycles = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

            // Assert
            Assert.Equal(0, assetLifeCycles.Count);
        }

        [Theory]
        [InlineData(0, "2020-10-10")]
        [InlineData(-500, "2020-10-10")]
        [InlineData(699.99, "2020-10-10")]
        [InlineData(699.99, "2090-10-10")]
        [Trait("Category", "UnitTest")]
        public async Task BookValueCalculation_ValidMethod(decimal paidByCompany, DateTime purchaseDate)
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.Transactional, purchaseDate, ASSETHOLDER_ONE_ID, new List<long>() { 458718920164666 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), "Test note", "description", paidByCompany);

            // Assert
            Assert.True(newAsset.BookValue >= 0);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task BookValueCalculation_ValidCalculation()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.Transactional, DateTime.UtcNow.AddMonths(-24) , ASSETHOLDER_ONE_ID, new List<long>() { 458718920164666 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), "Test note", "description", 7000);
            
            // Assert
            Assert.True(newAsset.BookValue == 2333.33M);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetCustomerTotalBookValue()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var newAsset1 = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535344", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.Transactional, DateTime.UtcNow.AddMonths(-24) , ASSETHOLDER_ONE_ID, new List<long>() { 458718920164666 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), "Test note", "description", 7000);

            var newAsset2 = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, Guid.Empty, "alias", "4543534535348", ASSET_CATEGORY_ID,
                "iPhone", "iPhone X", LifecycleType.Transactional, DateTime.UtcNow.AddMonths(-24) , ASSETHOLDER_ONE_ID, new List<long>() { 458718920164666 }, "5e:c4:33:df:61:70",
                Guid.NewGuid(), "Test note", "description", 5889.88M);
            var updateToActive = await assetService.UpdateStatusForMultipleAssetLifecycles(COMPANY_ID, Guid.Empty, new List<Guid>() { newAsset1.ExternalId,newAsset2.ExternalId },AssetLifecycleStatus.Active);
            var totalBookValue = await assetService.GetCustomerTotalBookValue(COMPANY_ID);
            
            // Assert
            Assert.True( 2333.33M+1963.29M == totalBookValue);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void MakeAssetAvailableAsync()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
            var assetLifecyclesFromUser = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

            var assetGuid = assetLifecyclesFromUser.FirstOrDefault().ExternalId;

            // Act
            var updatedAssetsLifecycles = await assetService.MakeAssetAvailableAsync(COMPANY_ID, Guid.Empty, assetGuid);
            var updatedAsset = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, assetGuid);

            // Assert
            Assert.True(null == updatedAsset.ContractHolderUserId);
            Assert.Equal(AssetLifecycleStatus.Available, updatedAsset.AssetLifecycleStatus);
        }


    }
}