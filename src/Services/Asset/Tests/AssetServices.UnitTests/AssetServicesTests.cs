using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Attributes;
using AssetServices.Exceptions;
using AssetServices.Infrastructure;
using AssetServices.Models;
using AssetServices.ServiceModel;
using AssetServices.Utility;
using AutoMapper;
using Common.Enums;
using Common.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AssetServices.UnitTests;

public class AssetServicesTests : AssetBaseTest
{
    private static IMapper _mapper;

    public AssetServicesTests() : base(new DbContextOptionsBuilder<AssetsContext>()
        .UseSqlite("Data Source=sqliteunittests.db").Options)
    {
        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetAssembly(typeof(AssetDTO))); });
            _mapper = mappingConfig.CreateMapper();
        }
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void GetAssetsForUser_ForUserOne_CheckCount()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
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
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

        // Act
        var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID);

            // Assert
            Assert.Equal(0, assetsFromCompany);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAssetsCount_ForCompanyWithDepartment_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID, DEPARTMENT_ID);

            // Assert
            Assert.Equal(0, assetsFromCompany);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAssetsCount_ForCompanyWithStatus_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID, null, AssetLifecycleStatus.InUse);

            // Assert
            Assert.Equal(6, assetsFromCompany);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetAssetsCount_ForCompanyWithDepartmentAndStatus_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

            // Act
            var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID, DEPARTMENT_ID, AssetLifecycleStatus.InUse);

            // Assert
            Assert.Equal(2, assetsFromCompany);
        }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void GetAssetsForCustomer_ForOneCustomer_CheckCount()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

        // Act
        var assetsFromUser =
            await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, string.Empty, 1, 10, null,
                new CancellationToken());

            // Assert
            Assert.Equal(6, assetsFromUser.Items.Count);

        // search with serial key
        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, "123456789012399", 1, 10,
            null, new CancellationToken());

            // Assert 
            Assert.Equal(4, assetsFromUser.Items.Count);

        // search with IMEI
        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, "512217111821626", 1, 10,
            null, new CancellationToken());

            // Assert 
            Assert.Equal(4, assetsFromUser.Items.Count);
        }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void UpdateMultipleAssetsStatus_StatusChange_InputRequired()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var assetLifecyclesFromUser = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

        IList<Guid> assetGuidList =
            assetLifecyclesFromUser.Select(assetLifecycle => assetLifecycle.ExternalId).ToList();

        // Act
        var updatedAssetsLifecycles =
            await assetService.UpdateStatusForMultipleAssetLifecycles(COMPANY_ID, Guid.Empty, assetGuidList,
                AssetLifecycleStatus.Active);

            // Assert
            Assert.Equal(3, updatedAssetsLifecycles.Count);
            Assert.Equal("alias_0", updatedAssetsLifecycles[0].Alias);
            Assert.Equal("alias_3", updatedAssetsLifecycles[1].Alias);
            Assert.Equal("alias_4", updatedAssetsLifecycles[2].Alias);
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
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.BYOD,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 458718920164666 },
            MacAddress = "5e:c4:33:df:61:70",
            ManagedByDepartmentId = Guid.NewGuid(),
            Note = "Test note",
            Description = "description"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

        // Assert
        Assert.NotNull(newAssetRead);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void SaveAssetForCustomer_WithoutAssetHolderAndDepartmentAssigned()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.BYOD,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 993100473611389 },
            MacAddress = "a3:21:99:5d:a7:a0",
            ManagedByDepartmentId = null,
            Note = "Unassigned asset",
            Description = "description"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

        // Assert
        Assert.NotNull(newAssetRead);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void SaveAssetForCustomer_WithPaidByCompany()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.BYOD,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 458718920164666 },
            MacAddress = "5e:c4:33:df:61:70",
            ManagedByDepartmentId = Guid.NewGuid(),
            Note = "Test note",
            Description = "description",
            PaidByCompany = 20.33m
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

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
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.BYOD,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 458718920164666 },
            MacAddress = "5e:c4:33:df:61:70",
            ManagedByDepartmentId = Guid.NewGuid(),
            Note = "Test note",
            Description = "description"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId);

        // Assert
        Assert.NotNull(newAssetRead);
        Assert.Equal(0, newAssetRead.PaidByCompany);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AddAssetForCustomerAsync_NewAssetNoDepartment_AssetStatusShouldBeInUse()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 993100473611389 },
            MacAddress = "a3:21:99:5d:a7:a0",
            ManagedByDepartmentId = null,
            Note = "Unassigned asset",
            Description = "description"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        Assert.Equal(AssetLifecycleStatus.InUse, newAsset.AssetLifecycleStatus);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AddAssetForCustomerAsync_NewAssetTransactional_AssetStatusShouldInUse()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 993100473611389 },
            MacAddress = "a3:21:99:5d:a7:a0",
            ManagedByDepartmentId = DEPARTMENT_ID,
            Note = "Unassigned asset",
            Description = "description",
            AssetTag = "A4020",
            LifecycleType = LifecycleType.Transactional

        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        Assert.Equal(AssetLifecycleStatus.InUse, newAsset.AssetLifecycleStatus);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AddAssetForCustomerAsync_NewAssetNoLifcycle_AssetStatusShouldBeInUse()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 993100473611389 },
            MacAddress = "a3:21:99:5d:a7:a0",
            ManagedByDepartmentId = null,
            Note = "Unassigned asset",
            Description = "description"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        Assert.Equal(AssetLifecycleStatus.InUse, newAsset.AssetLifecycleStatus);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AddAssetForCustomerAsync_NewAssetWithLeasingLifecycle_AssetStatusShouldInputRequired()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.Leasing,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = null,
            Imei = new List<long> { 993100473611389 },
            MacAddress = "a3:21:99:5d:a7:a0",
            ManagedByDepartmentId = null,
            Note = "Unassigned asset",
            Description = "description"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        Assert.Equal(AssetLifecycleStatus.InputRequired, newAsset.AssetLifecycleStatus);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AddAssetForCustomerAsync_IMEINot15Digits_ShouldThrowInvalidAssetDataException()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.NoLifecycle,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = null,
            Imei = new List<long> { 45871892016466 },
            MacAddress = "a3:21:99:5d:a7:a0",
            ManagedByDepartmentId = null,
            Note = "Unassigned asset",
            Description = "description"
        };

        // Act and assert
        await Assert.ThrowsAsync<InvalidAssetDataException>(() =>
            assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AddAssetForCustomerAsync_IMEIWithNoElementInList_ShouldReturnInputRequired()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.NoLifecycle,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = null,
            Imei = new List<long>(),
            MacAddress = "a3:21:99:5d:a7:a0",
            ManagedByDepartmentId = null,
            Note = "Unassigned asset",
            Description = "description"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        Assert.Equal(AssetLifecycleStatus.InputRequired, newAsset.AssetLifecycleStatus);
    }


    [Fact]
    [Trait("Category", "UnitTest")]
    public void MakeUniqueIMEIList_WithDuplicatedValues_OnlyReturn2()
    {
        var number1 = 106699671963280;
        var number2 = 102274227461256;
        var listOfImeis = new List<long>
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
        var imei = "";

        // Act
        var valid = AssetValidatorUtility.ValidateImei(imei);

        // Assert
        Assert.True(!valid);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public void ValidateImei_invalid_single()
    {
        // Arrange
        var imei = "111111111111111";

        // Act
        var valid = AssetValidatorUtility.ValidateImei(imei);

        // Assert
        Assert.False(valid);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public void ValidateImei_valid_single()
    {
        // Arrange
        var imei = "532618333994628";

        // Act
        var valid = AssetValidatorUtility.ValidateImei(imei);

        // Assert
        Assert.True(valid);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public void ValidateImei_valid__multiple()
    {
        // Arrange
        var imeis = "337047052140527,548668589912669,010708141304465";

        // Act
        var valid = AssetValidatorUtility.ValidateImeis(imeis);

        // Assert
        Assert.True(valid);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public void ValidateImei_invalid__multiple()
    {
        // Arrange
        var imeis = "33704705214052,548668589912669,0107081413044651";

        // Act
        var valid = AssetValidatorUtility.ValidateImeis(imeis);

        // Assert
        Assert.False(valid);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void ImeiValidationAttribute_Invalid()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        Asset asset = new MobilePhone(Guid.NewGuid(), Guid.Empty, "4543534535344", "iPhone", "iPhone X",
            new List<AssetImei> { new(111111987863622) }, "a3:21:99:5d:a7:a0");
        var attribute = new ImeiValidationAttribute();

        // Act
        var valid = attribute.IsValid(asset);

        // Assert
        Assert.False(valid);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void ImeiValidationAttribute_Valid()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        Asset asset = new MobilePhone(Guid.NewGuid(), Guid.Empty, "4543534535344", "iPhone", "iPhone X",
            new List<AssetImei> { new(357879702624426) }, "a3:21:99:5d:a7:a0");
        var attribute = new ImeiValidationAttribute();

        // Act
        var valid = attribute.IsValid(asset);

        // Assert
        Assert.True(valid);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void CreateLabelsForCustomer()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

        IList<Label> labelsToAdd = new List<Label>();
        labelsToAdd.Add(new Label("Repair", LabelColor.Red));
        labelsToAdd.Add(new Label("Field", LabelColor.Blue));

        // Act
        await assetService.AddLabelsForCustomerAsync(COMPANY_ID, Guid.Empty, labelsToAdd);

        var savedLabels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);

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
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

        // Act
        await assetService.DeleteLabelsForCustomerAsync(COMPANY_ID, new List<Guid> { LABEL_ONE_ID, LABEL_TWO_ID });

        var savedLabels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);

        // Assert
        Assert.Equal(0, savedLabels.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void UpdateLabelsForCustomer()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

        var labels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);
        labels[0].PatchLabel(Guid.Empty, new Label("Deprecated", LabelColor.Orange));
        labels[1].PatchLabel(Guid.Empty, new Label("Lost", LabelColor.Gray));

        // Act
        await assetService.UpdateLabelsForCustomerAsync(COMPANY_ID, labels);

        var savedLabels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);

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
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

        var labels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);
        IList<Guid> labelGuids = new List<Guid>();
        foreach (var label in labels)
        {
            labelGuids.Add(label.ExternalId);
        }

        var assetLifecycle = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_THREE_ID);
        IList<Guid> assetGuids = new List<Guid> { assetLifecycle!.ExternalId };

        // Act
        assetLifecycle =
            (await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids))[0];

        // Assert
        Assert.Equal(labelGuids.Count, assetLifecycle.Labels.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void UnAssignLabelsForAsset()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

        var labels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);
        IList<Guid> labelGuids = labels.Select(label => label.ExternalId).ToList();

        var assetLifecycle = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_THREE_ID);
        IList<Guid> assetGuids = new List<Guid> { assetLifecycle!.ExternalId };

        await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids);


        // Act
        assetLifecycle =
            (await assetService.UnAssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids))[0];

        //// Assert
        Assert.Empty(assetLifecycle.Labels);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task UnAssignAssetLifecyclesForUser_Valid()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
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
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 458718920164666 },
            MacAddress = "5e:c4:33:df:61:70",
            ManagedByDepartmentId = Guid.NewGuid(),
            Note = "Test note",
            Description = "description",
            PaidByCompany = paidByCompany
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        Assert.True(newAsset.BookValue >= 0);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task BookValueCalculation_ValidCalculation()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = DateTime.UtcNow.AddMonths(-24),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 458718920164666 },
            MacAddress = "5e:c4:33:df:61:70",
            ManagedByDepartmentId = Guid.NewGuid(),
            Note = "Test note",
            Description = "description",
            PaidByCompany = 7000
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        Assert.True(newAsset.BookValue == 2333.33m);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetCustomerTotalBookValue()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var newAssetDTO1 = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = DateTime.UtcNow.AddMonths(-24),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 458718920164666 },
            MacAddress = "5e:c4:33:df:61:70",
            ManagedByDepartmentId = Guid.NewGuid(),
            Note = "Test note",
            Description = "description",
            PaidByCompany = 7000
        };

        var newAssetDTO2 = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = DateTime.UtcNow.AddMonths(-24),
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 458718920164666 },
            MacAddress = "5e:c4:33:df:61:70",
            ManagedByDepartmentId = Guid.NewGuid(),
            Note = "Test note",
            Description = "description",
            PaidByCompany = 5889.88m
        };

        // Act
        var newAsset1 = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO1);
        var newAsset2 = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO2);
        await assetService.UpdateStatusForMultipleAssetLifecycles(COMPANY_ID, Guid.Empty,
            new List<Guid> { newAsset1.ExternalId, newAsset2.ExternalId }, AssetLifecycleStatus.Active);
        var totalBookValue = await assetService.GetCustomerTotalBookValue(COMPANY_ID);

        // Assert
        Assert.True(2333.33M + 1963.29M == totalBookValue);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void MakeAssetAvailableAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var assetLifecyclesFromUser = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

        var assetGuid = assetLifecyclesFromUser.FirstOrDefault()!.ExternalId;

        // Act
        var updatedAssetsLifecycles = await assetService.MakeAssetAvailableAsync(COMPANY_ID, Guid.Empty, assetGuid);

        // Assert
        Assert.True(updatedAssetsLifecycles.ContractHolderUserId == null);
        Assert.True(updatedAssetsLifecycles.Labels == null || !updatedAssetsLifecycles.Labels.Any());
        Assert.Equal(AssetLifecycleStatus.Available, updatedAssetsLifecycles.AssetLifecycleStatus);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void UpdateLifeCycleSettingForCustomerAsync_NotFound()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            CustomerId = Guid.NewGuid(),
            BuyoutAllowed = false,
        };

        // Act and assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            assetService.UpdateLifeCycleSettingForCustomerAsync(lifeCycleSetting.CustomerId, lifeCycleSetting, Guid.Empty));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void UpdateLifeCycleSettingForCustomerAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            CustomerId = COMPANY_ID,
            BuyoutAllowed = false,
            AssetCategoryId = 1
        };

        // Act 
        var setting = await assetService.UpdateLifeCycleSettingForCustomerAsync(lifeCycleSetting.CustomerId, lifeCycleSetting, Guid.Empty);
        var updatedSetting = await assetService.GetLifeCycleSettingByCustomer(lifeCycleSetting.CustomerId);

        // Assert
        Assert.True(updatedSetting.FirstOrDefault(x=>x.AssetCategoryId == lifeCycleSetting.AssetCategoryId)!.BuyoutAllowed == lifeCycleSetting.BuyoutAllowed);
        Assert.True(updatedSetting.FirstOrDefault(x => x.AssetCategoryId == lifeCycleSetting.AssetCategoryId)!.CustomerId == lifeCycleSetting.CustomerId);
    }


    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AddLifeCycleSettingForCustomerAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            CustomerId = Guid.NewGuid(),
            BuyoutAllowed = true,
        };

        // Act
        var addedSetting = await assetService.AddLifeCycleSettingForCustomerAsync(lifeCycleSetting.CustomerId, lifeCycleSetting, Guid.Empty);

        // Assert
        Assert.True(addedSetting.BuyoutAllowed == lifeCycleSetting.BuyoutAllowed);
        Assert.True(addedSetting.CustomerId == lifeCycleSetting.CustomerId);
        Assert.True(addedSetting.CreatedDate.Date == DateTime.UtcNow.Date);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void SetLifeCycleSettingForCustomerAsync_NotFound()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            AssetCategoryId = 1,
            MinBuyoutPrice=800M
        };

        // Act and assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() =>
            assetService.UpdateLifeCycleSettingForCustomerAsync(Guid.NewGuid(), lifeCycleSetting, Guid.Empty));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void SetLifeCycleSettingForCustomerAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            AssetCategoryId = 2,
            MinBuyoutPrice=800M
        };

        // Act
        var setting = await assetService.AddLifeCycleSettingForCustomerAsync(COMPANY_ID, lifeCycleSetting, Guid.Empty);
        var allSetting = await assetService.GetLifeCycleSettingByCustomer(COMPANY_ID);
        var addedSetting = allSetting.FirstOrDefault(x => x.AssetCategoryId == lifeCycleSetting.AssetCategoryId);

        // Assert
        Assert.True(allSetting.Count == 2);
        Assert.True(addedSetting!.MinBuyoutPrice == lifeCycleSetting.MinBuyoutPrice);
        Assert.True(addedSetting!.AssetCategoryName == "Tablet");
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async void UpdateCategorySettingForCustomerAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            AssetCategoryId = 1,
            MinBuyoutPrice = 800M
        };

        // Act
        var setting = await assetService.UpdateLifeCycleSettingForCustomerAsync(COMPANY_ID, lifeCycleSetting, Guid.Empty);
        var allSetting = await assetService.GetLifeCycleSettingByCustomer(COMPANY_ID);
        var addedSetting = allSetting.FirstOrDefault(x => x.AssetCategoryId == lifeCycleSetting.AssetCategoryId);

        // Assert
        Assert.True(addedSetting!.MinBuyoutPrice == lifeCycleSetting.MinBuyoutPrice);
        Assert.True(addedSetting!.AssetCategoryName == "Mobile phone");
    }
    
    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AssignAssetLifeCycleToHolder_AssigneToDepartment()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);
        
        var asset = await assetService.AssignAssetLifeCycleToHolder(COMPANY_ID, ASSETLIFECYCLE_ONE_ID, Guid.Empty, DEPARTMENT_ID, CALLER_ID);
       
        Assert.False(asset.IsPersonal);
        Assert.Equal(DEPARTMENT_ID, asset.ManagedByDepartmentId);
        Assert.Null(asset.ContractHolderUserId);

    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async void AssignAssetLifeCycleToHolder_AssigneToUser()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper);

        var asset = await assetService.AssignAssetLifeCycleToHolder(COMPANY_ID, ASSETLIFECYCLE_ONE_ID, ASSETHOLDER_ONE_ID, Guid.Empty, CALLER_ID);

        Assert.True(asset.IsPersonal);
        Assert.Equal(ASSETHOLDER_ONE_ID, asset.ContractHolderUserId);
        Assert.Null(asset.ManagedByDepartmentId);
    }

}