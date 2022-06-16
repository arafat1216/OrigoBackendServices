using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AssetServices.Attributes;
using AssetServices.Email;
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
    public async Task GetAssetsForUser_ForUserOne_CheckCount()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        // Act
        var assetsFromUser = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

            // Assert
            Assert.Equal(1, assetsFromUser.Count);
        }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetsCount_ForCompany_CheckCount()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        // Act
        var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID, null);

            // Assert
            Assert.Equal(6, assetsFromCompany);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAssetsCount_ForCompanyWithDepartment_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

            // Act
            var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID, null, departmentId: DEPARTMENT_ID);

            // Assert
            Assert.Equal(2, assetsFromCompany);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAssetsCount_ForCompanyWithStatus_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

            // Act
            var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID, AssetLifecycleStatus.InUse);

            // Assert
            Assert.Equal(2, assetsFromCompany);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAssetsCount_ForCompanyWithDepartmentAndStatus_CheckCount()
        {
            // Arrange
            await using var context = new AssetsContext(ContextOptions);
            var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

            // Act
            var assetsFromCompany = await assetService.GetAssetsCountAsync(COMPANY_ID, AssetLifecycleStatus.InUse, DEPARTMENT_ID);

            // Assert
            Assert.Equal(1, assetsFromCompany);
        }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetsForCustomer_ForOneCustomer_CheckCount()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        // Act
        var assetsFromUser =
            await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID,null, null, null, null, null, string.Empty, 1, 10,
                new CancellationToken());

        // Assert
        Assert.Equal(7, assetsFromUser.Items.Count);


        //filter by UserId

        string userId = "6d16a4cb-4733-44de-b23b-0eb9e8ae6590";


        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, userId, null, null, null, null, null, 1, 10,
             new CancellationToken());

        Assert.Equal(1, assetsFromUser.Items.Count);


        // search with Filter Options


        //filter data only label

        Guid[] labels = new Guid[] { new Guid("D3EF00AB-C3B6-4751-982F-BF66738BC068") };


        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, labels, null, 1, 10,
             new CancellationToken());

        Assert.Equal(1, assetsFromUser.Items.Count);



        //filter data only category

        int[] category = new int[] { 1 };


        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, category, null, null, 1, 10,
             new CancellationToken());

        Assert.Equal(6, assetsFromUser.Items.Count);



        //filter data only department

        IList<Guid?> depts = new List<Guid?> { new Guid("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72") };

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, depts, null, null, null, 1, 10, new CancellationToken());

        Assert.Equal(2, assetsFromUser.Items.Count);


        //filter data only status

        IList<AssetLifecycleStatus> status = new List<AssetLifecycleStatus>{AssetLifecycleStatus.Active ,
            AssetLifecycleStatus.Recycled};

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, status, null, null, null, null, 1, 10,
            new CancellationToken());


        Assert.Equal(2, assetsFromUser.Items.Count);


        //filter data all options


        status = new List<AssetLifecycleStatus>{AssetLifecycleStatus.Active ,
            AssetLifecycleStatus.Recycled};

        category = new int[] { 1 };

        labels = new Guid[] { new Guid("D3EF00AB-C3B6-4751-982F-BF66738BC068") };

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, status, depts, category, labels, null, 1, 10,
             new CancellationToken());

        Assert.Equal(1, assetsFromUser.Items.Count);




        // search with serial key
        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, null, "123456789012399", 1, 10,
             new CancellationToken());

        // Assert 
        Assert.Equal(5, assetsFromUser.Items.Count);

        // search with IMEI
        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, null, "512217111821626", 1, 10,
             new CancellationToken());

        // Assert 
        Assert.Equal(5, assetsFromUser.Items.Count);


    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task SaveAssetForCustomer_WithAssetHolderAndDepartmentAssigned()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task AddAssetForCustomer_WithNoRuntimeSetForTransactional_IsSaved()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepositoryMock = new Mock<IAssetLifecycleRepository>();
        assetRepositoryMock.Setup(r => r.GetCustomerSettingsAsync(COMPANY_ID))
            .ReturnsAsync(new CustomerSettings(COMPANY_ID));
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepositoryMock.Object, _mapper, new Mock<IEmailService>().Object);
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
            Note = "Test note",
            Description = "description"
        };

        // Act
        _ = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        assetRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AssetLifecycle>()));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task SaveAssetForCustomer_WithNoLifecycleSet()
    {
        // Arrange
        var companyWithoutCustomerSettingId = new Guid("5af684aa-e7e5-11ec-b531-00155d3daa66");
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository = new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
            AssetHolderId = ASSETHOLDER_ONE_ID,
            Imei = new List<long> { 458718920164666 },
            MacAddress = "5e:c4:33:df:61:70",
            ManagedByDepartmentId = Guid.NewGuid(),
            Note = "Test note",
            Description = "description"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(companyWithoutCustomerSettingId, newAssetDTO);
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(companyWithoutCustomerSettingId, newAsset.ExternalId);

        // Assert
        Assert.NotNull(newAssetRead);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task SaveAssetForCustomer_WithoutAssetHolderAndDepartmentAssigned()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task SaveAssetForCustomer_WithPaidByCompany()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task SaveAssetForCustomer_WithoutPaidByCompany()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task AddAssetForCustomerAsync_NewAssetNoDepartment_AssetStatusShouldBeInUse()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "trh45yhtghnb",
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
    public async Task AddAssetForCustomerAsync_NewAssetTransactional_AssetStatusShouldInUse()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task AddAssetForCustomerAsync_NewAssetNoLifecycle_AssetStatusShouldBeInUse()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task AddAssetForCustomerAsync_IMEINot15Digits_ShouldThrowInvalidAssetDataException()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task AddAssetForCustomerAsync_IMEIWithNoElementInList_ShouldReturnInputRequired()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task ImeiValidationAttribute_Invalid()
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
    public async Task ImeiValidationAttribute_Valid()
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
    public async Task CreateLabelsForCustomer()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

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
    public async Task DeleteLabelsForCustomer()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        // Act
        await assetService.DeleteLabelsForCustomerAsync(COMPANY_ID, new List<Guid> { LABEL_ONE_ID, LABEL_TWO_ID });

        var savedLabels = await assetService.GetCustomerLabelsForCustomerAsync(COMPANY_ID);

        // Assert
        Assert.Equal(0, savedLabels.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task UpdateLabelsForCustomer()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

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
    public async Task AssignLabelsForAsset()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

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
    public async Task UnAssignLabelsForAsset()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

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
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

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
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var newAssetDTO = new NewAssetDTO
        {
            CallerId = Guid.Empty,
            Alias = "alias",
            SerialNumber = "4543534535344",
            AssetCategoryId = ASSET_CATEGORY_ID,
            Brand = "iPhone",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.Transactional,
            PurchaseDate = purchaseDate,
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
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
        await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO1);
        await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO2);
        var totalBookValue = await assetService.GetCustomerTotalBookValue(COMPANY_ID);

        // Assert
        Assert.Equal(2333.33M + 1963.29M, totalBookValue);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task MakeAssetAvailableAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var assetLifecyclesFromUser = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID);

        var assetGuid = assetLifecyclesFromUser.FirstOrDefault()!.ExternalId;

        // Act
        var data = new MakeAssetAvailableDTO()
        {
            AssetLifeCycleId = assetGuid,
            CallerId = Guid.Empty
        };
        var updatedAssetsLifecycles = await assetService.MakeAssetAvailableAsync(COMPANY_ID, data);

        // Assert
        Assert.True(updatedAssetsLifecycles.ContractHolderUserId == null);
        Assert.True(updatedAssetsLifecycles.Labels == null || !updatedAssetsLifecycles.Labels.Any());
        Assert.Equal(AssetLifecycleStatus.Available, updatedAssetsLifecycles.AssetLifecycleStatus);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task UpdateLifeCycleSettingForCustomerAsync_NotFound()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task UpdateLifeCycleSettingForCustomerAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            CustomerId = COMPANY_ID,
            BuyoutAllowed = false,
            AssetCategoryId = 1
        };

        // Act 
        await assetService.UpdateLifeCycleSettingForCustomerAsync(lifeCycleSetting.CustomerId, lifeCycleSetting, Guid.Empty);
        var updatedSetting = await assetService.GetLifeCycleSettingByCustomer(lifeCycleSetting.CustomerId);

        // Assert
        Assert.True(updatedSetting.FirstOrDefault(x=>x.AssetCategoryId == lifeCycleSetting.AssetCategoryId)!.BuyoutAllowed == lifeCycleSetting.BuyoutAllowed);
    }


    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddLifeCycleSettingForCustomerAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            CustomerId = Guid.NewGuid(),
            BuyoutAllowed = true,
            Runtime = 32
        };

        // Act
        var addedSetting = await assetService.AddLifeCycleSettingForCustomerAsync(lifeCycleSetting.CustomerId, lifeCycleSetting, Guid.Empty);

        // Assert
        Assert.Equal(lifeCycleSetting.BuyoutAllowed, addedSetting.BuyoutAllowed);
        Assert.Equal(DateTime.UtcNow.Date, addedSetting.CreatedDate.Date);
        Assert.Equal(lifeCycleSetting.Runtime, addedSetting.Runtime);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task SetLifeCycleSettingForCustomerAsync_NotFound()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
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
    public async Task SetLifeCycleSettingForCustomerAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            AssetCategoryId = 2,
            MinBuyoutPrice=800M,
            Runtime = 24
        };

        // Act
        await assetService.AddLifeCycleSettingForCustomerAsync(COMPANY_ID, lifeCycleSetting, Guid.Empty);
        var allSetting = await assetService.GetLifeCycleSettingByCustomer(COMPANY_ID);
        var addedSetting = allSetting.FirstOrDefault(x => x.AssetCategoryId == lifeCycleSetting.AssetCategoryId);

        // Assert
        Assert.Equal(2, allSetting.Count);
        Assert.Equal(lifeCycleSetting.MinBuyoutPrice, addedSetting!.MinBuyoutPrice);
        Assert.Equal("Tablet", addedSetting!.AssetCategoryName);
        Assert.Equal(24, addedSetting!.Runtime);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task UpdateCategorySettingForCustomerAsync()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var lifeCycleSetting = new LifeCycleSettingDTO()
        {
            AssetCategoryId = 1,
            MinBuyoutPrice = 800M
        };

        // Act
        await assetService.UpdateLifeCycleSettingForCustomerAsync(COMPANY_ID, lifeCycleSetting, Guid.Empty);
        var allSetting = await assetService.GetLifeCycleSettingByCustomer(COMPANY_ID);
        var addedSetting = allSetting.FirstOrDefault(x => x.AssetCategoryId == lifeCycleSetting.AssetCategoryId);

        // Assert
        Assert.True(addedSetting!.MinBuyoutPrice == lifeCycleSetting.MinBuyoutPrice);
        Assert.True(addedSetting!.AssetCategoryName == "Mobile phone");
    }
    
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssignAssetLifeCycleToHolder_AssignToDepartment()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var assignAssetDTO = new AssignAssetDTO
        {
            UserId = Guid.Empty, DepartmentId = DEPARTMENT_ID, CallerId = CALLER_ID
        };

        // Act
        var asset = await assetService.AssignAssetLifeCycleToHolder(COMPANY_ID, ASSETLIFECYCLE_ONE_ID, assignAssetDTO);
       
        // Assert
        Assert.False(asset!.IsPersonal);
        Assert.Equal(DEPARTMENT_ID, asset.ManagedByDepartmentId);
        Assert.Null(asset.ContractHolderUserId);

    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssignAssetLifeCycleToHolder_IsPersonalAndAssignedOnlyToUser()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var assignAssetDTO = new AssignAssetDTO
        {
            UserId = ASSETHOLDER_ONE_ID,
            Personal = true,
            DepartmentId = Guid.Empty,
            CallerId = CALLER_ID
        };

        // Act
        var asset = await assetService.AssignAssetLifeCycleToHolder(COMPANY_ID, ASSETLIFECYCLE_ONE_ID, assignAssetDTO);

        // Assert
        Assert.True(asset!.IsPersonal);
        Assert.Equal(ASSETHOLDER_ONE_ID, asset.ContractHolderUserId);
        Assert.Null(asset.ManagedByDepartmentId);
    }
}