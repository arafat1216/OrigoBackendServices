using AssetServices.Attributes;
using AssetServices.Email;
using AssetServices.Exceptions;
using AssetServices.Infrastructure;
using AssetServices.Mappings;
using AssetServices.Models;
using AssetServices.ServiceModel;
using AssetServices.Utility;
using AutoMapper;
using Common.Enums;
using Common.Logging;
using Common.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
            var mappingConfig = new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetAssembly(typeof(AssetLifecycleProfile))); });
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
    public async Task GetAssetsForUser_ForUserOne_ShouldIncludeAsset()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        // Act
        var assetsFromUser = await assetService.GetAssetLifecyclesForUserAsync(COMPANY_ID, ASSETHOLDER_ONE_ID, includeAsset: true);

        // Assert
        Assert.All(assetsFromUser, item => Assert.NotNull(item.Asset));
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
        Assert.Equal(11, assetsFromCompany);
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
        Assert.Equal(7, assetsFromCompany);
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
            await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, null, null, null, null, null, string.Empty, 1, 15,
                new CancellationToken());

        // Assert
        Assert.Equal(13, assetsFromUser.Items.Count);


        //filter by UserId

        string userId = "6d16a4cb-4733-44de-b23b-0eb9e8ae6590";


        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, userId, null, null, null, null, null, null, null, null, null, 1, 15,
             new CancellationToken());

        Assert.Equal(1, assetsFromUser.Items.Count);


        // search with Filter Options

        //filter data only category

        int[] category = new[] { 1 };


        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, category, null, null, null, null, null, null, 1, 15,
             new CancellationToken());

        Assert.Equal(12, assetsFromUser.Items.Count);



        //filter data only department

        IList<Guid?> departments = new List<Guid?> { new Guid("6244c47b-fcb3-4ea1-ad82-e37ebf5d5e72") };

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, departments, null, null, null, null, null, null, null, 1, 15, new CancellationToken());

        Assert.Equal(8, assetsFromUser.Items.Count);


        //filter data only status

        IList<AssetLifecycleStatus> status = new List<AssetLifecycleStatus>{AssetLifecycleStatus.Active ,
            AssetLifecycleStatus.Recycled};

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, status, null, null, null, null, null, null, null, null, 1, 10,
            new CancellationToken());


        Assert.Equal(3, assetsFromUser.Items.Count);

        //filter data only endPeriodMonth

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, null, null, null, DateTime.UtcNow, null, null, 1, 10,
            new CancellationToken());


        Assert.Equal(3, assetsFromUser.Items.Count);


        //filter data all options


        status = new List<AssetLifecycleStatus>{AssetLifecycleStatus.Active ,
            AssetLifecycleStatus.Recycled};

        category = new[] { 1 };

        var labels = new[] { new Guid("D3EF00AB-C3B6-4751-982F-BF66738BC068") };

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, status, departments, category, labels, null, null, null, null, null, 1, 10,
             new CancellationToken());

        Assert.Equal(1, assetsFromUser.Items.Count);




        // search with serial key
        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, null, null, null, null, null, "123456789012399", 1, 10,
             new CancellationToken());

        // Assert 
        Assert.Equal(5, assetsFromUser.Items.Count);

        // search with IMEI
        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, null, null, null, null, null, "512217111821626", 1, 10,
             new CancellationToken());

        // Assert 
        Assert.Equal(5, assetsFromUser.Items.Count);

        // search with combination of IsPersonal and IsActive state

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, null, true, true, null, null, null, 1, 10,
             new CancellationToken());

        // Assert 
        Assert.Equal(2, assetsFromUser.Items.Count);

        assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, null, false, true, null, null, null, 1, 10,
             new CancellationToken());

        // Assert 
        Assert.Equal(1, assetsFromUser.Items.Count);

    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecyclesForCustomer_WithLabelFilter_CheckCount()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        // Act
        Guid[] labels = new[] { LABEL_TWO_ID };
        var assetsFromUser = await assetService.GetAssetLifecyclesForCustomerAsync(COMPANY_ID, null, null, null, null, labels, null, null, null, null, null, 1, 10,
             new CancellationToken());

        // Assert
        Assert.Equal(2, assetsFromUser.Items.Count);
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
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId, null, null);

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
        assetRepositoryMock.Setup(r => r.GetCustomerSettingsAsync(COMPANY_ID, false))
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
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(companyWithoutCustomerSettingId, newAsset.ExternalId, null, null);

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
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId, null, null);

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
            PaidByCompany = new Money(20.33m, CurrencyCode.NOK)
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId, null, null);

        // Assert
        Assert.NotNull(newAssetRead);
        Assert.Equal(20.33M, newAssetRead.PaidByCompany.Amount);
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
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId, null, null);

        // Assert
        Assert.NotNull(newAssetRead);
        Assert.Equal(0, newAssetRead.PaidByCompany.Amount);
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
    public async Task AddAssetForCustomerAsync_FileImportNoUser_ShouldGetInputRequired()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        var newAssetDTO = new NewAssetDTO
        {
            Alias = "",
            SerialNumber = "4543534535344",
            AssetCategoryId = 1,
            Brand = "iPhone",
            Description = "",
            Note = "",
            ProductName = "iPhone X",
            LifecycleType = LifecycleType.NoLifecycle, //this is the lifecycletype the assetlifecycle gets assigned with
            PurchaseDate = new DateTime(2020, 1, 1),
            AssetHolderId = null, //Has no user attached
            Imei = new List<long> { 865822011610467 },
            CallerId = Guid.Empty,
            Source = "FileImport"
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);

        // Assert
        Assert.Equal(AssetLifecycleStatus.InputRequired, newAsset.AssetLifecycleStatus);
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
        await Assert.ThrowsAsync<InvalidAssetImeiException>(() =>
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
    public async Task AddAssetLifecycleForCustomerAsync_CreateAndAssigneLabels()
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
            Labels = new List<string> { "Label_1", "Label_2", "A new one", "Another new one" }
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId, null, null, includeLabels: true);

        // Assert
        Assert.NotNull(newAssetRead);
        Assert.Equal(4,newAssetRead.Labels.Count());
        Assert.Collection(
            newAssetRead.Labels, 
            item => Assert.Equal("Label_1", item.Text),
            item => Assert.Equal("Label_2", item.Text),
            item => Assert.Equal("A new one", item.Text),
            item => Assert.Equal("Another new one", item.Text)
        );
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddAssetLifecycleForCustomerAsync_PurchasedBy()
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
            PaidByCompany = new Money(10000,CurrencyCode.NOK)
        };

        // Act
        var newAsset = await assetService.AddAssetLifecycleForCustomerAsync(COMPANY_ID, newAssetDTO);
        var newAssetRead = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, newAsset.ExternalId, null, null, includeLabels: true);

        // Assert
        Assert.NotNull(newAssetRead);
        Assert.Equal(10000,newAssetRead.PaidByCompany.Amount);
        Assert.Equal("NOK",newAssetRead.PaidByCompany.CurrencyCode);
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
        Assert.Equal(5, savedLabels.Count); // 2 made here, 3 made in AssetBaseTest
        Assert.Equal("Repair", savedLabels[3].Label.Text);
        Assert.Equal(LabelColor.Blue, savedLabels[4].Label.Color);
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
        await assetService.DeleteLabelsForCustomerAsync(COMPANY_ID, Guid.NewGuid(), new List<Guid> { LABEL_ONE_ID, LABEL_TWO_ID, LABEL_THREE_ID });

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
        Assert.Equal(3, savedLabels.Count);
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

        var assetLifecycle = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_THREE_ID, null, null);
        IList<Guid> assetGuids = new List<Guid> { assetLifecycle!.ExternalId };

        // Act
        assetLifecycle =
            (await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, assetGuids, labelGuids))[0];

        // Assert
        Assert.Equal(labelGuids.Count, assetLifecycle.Labels.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssignLabelsToAssets_SameLabelToMultipleAssets_LabelAddedToAssetLifecycles()
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

        var assetLifecycle2 = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_TWO_ID, null, null, includeLabels: true);
        var assetLifecycle3 = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_THREE_ID, null, null, includeLabels: true);

        // Act
        await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, new List<Guid> { assetLifecycle2!.ExternalId }, labelGuids);
        await assetService.AssignLabelsToAssetsAsync(COMPANY_ID, Guid.Empty, new List<Guid> { assetLifecycle3!.ExternalId }, labelGuids);

        assetLifecycle2 = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_TWO_ID, null, null, includeLabels: true);
        assetLifecycle3 = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_THREE_ID, null, null, includeLabels: true);

        // Assert
        Assert.Equal(labelGuids.Count, assetLifecycle2!.Labels.Count);
        Assert.Equal(labelGuids.Count, assetLifecycle3!.Labels.Count);
        Assert.All(assetLifecycle2.Labels, label => Assert.Contains(label.ExternalId, labelGuids));
        Assert.All(assetLifecycle3.Labels, label => Assert.Contains(label.ExternalId, labelGuids));
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

        var assetLifecycle = await assetService.GetAssetLifecycleForCustomerAsync(COMPANY_ID, ASSETLIFECYCLE_THREE_ID, null, null);
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
            PaidByCompany = new Money(paidByCompany, CurrencyCode.NOK)
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
            PaidByCompany = new Money(7000, CurrencyCode.NOK)
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
            PaidByCompany = new Money(7000, CurrencyCode.NOK)
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
            PaidByCompany = new Money(5889.88m, CurrencyCode.NOK)
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
    public async Task MakeAssetAvailableAsync_EmailServiceCalled()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var emailServiceMock = new Mock<IEmailService>();
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, emailServiceMock.Object);

        // Act
        var body = new MakeAssetAvailableDTO
        {
            AssetLifeCycleId = ASSETLIFECYCLE_TWO_ID,
            CallerId = CALLER_ID,
            PreviousUser = new EmailPersonAttributeDTO { Email = "test@test.com", Name = "Test", PreferedLanguage = "no" }
        };

        var makeAvailable = await assetService.MakeAssetAvailableAsync(COMPANY_ID, body);
        emailServiceMock.Verify(e => e.UnassignedFromUserEmailAsync(It.IsAny<Email.Model.UnassignedFromUserNotification>(), It.IsAny<string>()), Times.Once);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task MakeAssetAvailableAsync_EmailServiceNotCalled()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var emailServiceMock = new Mock<IEmailService>();
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, emailServiceMock.Object);

        // Act
        var body = new MakeAssetAvailableDTO
        {
            AssetLifeCycleId = ASSETLIFECYCLE_TWO_ID,
            CallerId = CALLER_ID,
            PreviousUser = new EmailPersonAttributeDTO { Email = "", Name = "Test", PreferedLanguage = "no" }
        };

        var makeAvailable = await assetService.MakeAssetAvailableAsync(COMPANY_ID, body);
        emailServiceMock.Verify(e => e.UnassignedFromUserEmailAsync(It.IsAny<Email.Model.UnassignedFromUserNotification>(), It.IsAny<string>()), Times.Never);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task MakeAssetAvailableAsync_EmailServiceCalled_WithDefaultPreferences()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var emailServiceMock = new Mock<IEmailService>();
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, emailServiceMock.Object);

        // Act
        var body = new MakeAssetAvailableDTO
        {
            AssetLifeCycleId = ASSETLIFECYCLE_TWO_ID,
            CallerId = CALLER_ID,
            PreviousUser = new EmailPersonAttributeDTO { Email = "test@test.com", Name = "Test" }
        };

        var makeAvailable = await assetService.MakeAssetAvailableAsync(COMPANY_ID, body);
        emailServiceMock.Verify(e => e.UnassignedFromUserEmailAsync(It.IsAny<Email.Model.UnassignedFromUserNotification>(), It.IsAny<string>()), Times.Once);
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
        Assert.True(updatedSetting.FirstOrDefault(x => x.AssetCategoryId == lifeCycleSetting.AssetCategoryId)!.BuyoutAllowed == lifeCycleSetting.BuyoutAllowed);
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
            MinBuyoutPrice = new Money(800M, CurrencyCode.NOK)
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
            MinBuyoutPrice = new Money(800M, CurrencyCode.NOK),
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
            MinBuyoutPrice = new Money(800M, CurrencyCode.NOK)
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
            UserId = Guid.Empty,
            DepartmentId = DEPARTMENT_ID,
            CallerId = CALLER_ID
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
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssetLifeCycleSendToRepair_StatusChangeToRepair()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        var assetBefore = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_ONE_ID);
        Assert.Equal(AssetLifecycleStatus.Active, assetBefore!.AssetLifecycleStatus);

        // Act
        await assetService.AssetLifeCycleSendToRepair(ASSETLIFECYCLE_ONE_ID, CALLER_ID);

        var assetAfter = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_ONE_ID);

        // Assert
        Assert.Equal(AssetLifecycleStatus.Repair, assetAfter!.AssetLifecycleStatus);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssetLifeCycleSendToRepair_WhenStatusAlreadyRepair()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        var assetBefore = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_SEVEN_ID);
        Assert.Equal(AssetLifecycleStatus.Repair, assetBefore!.AssetLifecycleStatus);

        // Act
        await assetService.AssetLifeCycleSendToRepair(ASSETLIFECYCLE_SEVEN_ID, CALLER_ID);

        var assetAfter = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_SEVEN_ID);

        // Assert
        Assert.Equal(AssetLifecycleStatus.Repair, assetAfter!.AssetLifecycleStatus);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssetLifeCycleRepairCompleted_ShouldNotChangeStatusOrThrowError_WhenStatusIsNotRepair()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        var assetBefore = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_FIVE_ID);
        Assert.Equal(AssetLifecycleStatus.InUse, assetBefore!.AssetLifecycleStatus);

        var body = new AssetLifeCycleRepairCompleted
        {

            CallerId = CALLER_ID,
            NewSerialNumber = "12345678975212",
            NewImei = new List<string> { "516768095487517" }
        };

        // Act
        await assetService.AssetLifeCycleRepairCompleted(ASSETLIFECYCLE_FIVE_ID, body);

        var assetAfter = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_FIVE_ID);

        // Assert
        Assert.Equal(AssetLifecycleStatus.InUse, assetAfter!.AssetLifecycleStatus);
    }

    [Fact]
    [Trait("Category", "UnitTest")]

    public async Task IsSentToRepair_DifferentStatuses_OnClassMethod()
    {
        // Arrange
        var callerId = Guid.NewGuid();
        await using var context = new AssetsContext(ContextOptions);

        //Active
        var asset1 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_ONE_ID);
        Assert.Equal(AssetLifecycleStatus.Active, asset1!.AssetLifecycleStatus);
        asset1.IsSentToRepair(callerId);
        Assert.Equal(AssetLifecycleStatus.Repair, asset1.AssetLifecycleStatus);

        //InputRequired
        var asset3 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_THREE_ID);
        Assert.Equal(AssetLifecycleStatus.InputRequired, asset3!.AssetLifecycleStatus);
        asset3.IsSentToRepair(callerId);
        Assert.Equal(AssetLifecycleStatus.Repair, asset3.AssetLifecycleStatus);

        //Available
        var asset4 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_FOUR_ID);
        Assert.Equal(AssetLifecycleStatus.Available, asset4!.AssetLifecycleStatus);
        asset4.IsSentToRepair(callerId);
        Assert.Equal(AssetLifecycleStatus.Repair, asset4.AssetLifecycleStatus);

        //InUse
        var asset5 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_FIVE_ID);
        Assert.Equal(AssetLifecycleStatus.InUse, asset5!.AssetLifecycleStatus);
        asset5.IsSentToRepair(callerId);
        Assert.Equal(AssetLifecycleStatus.Repair, asset5.AssetLifecycleStatus);

        //Repair
        var asset7 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_SEVEN_ID);
        Assert.Equal(AssetLifecycleStatus.Repair, asset7!.AssetLifecycleStatus);
        asset7.IsSentToRepair(callerId);
        Assert.Equal(AssetLifecycleStatus.Repair, asset7.AssetLifecycleStatus);


        //InActive
        var asset6 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_SIX_ID);
        asset6!.SetInactiveStatus(CALLER_ID);
        Assert.Equal(AssetLifecycleStatus.Inactive, asset6.AssetLifecycleStatus);
        asset6.IsSentToRepair(callerId);
        Assert.Equal(AssetLifecycleStatus.Repair, asset6.AssetLifecycleStatus);

        //Expired
        asset4.MakeAssetExpiresSoon(callerId);
        asset4.MakeAssetExpired(callerId);
        Assert.Equal(AssetLifecycleStatus.Expired, asset4.AssetLifecycleStatus);
        asset4.IsSentToRepair(callerId);
        Assert.Equal(AssetLifecycleStatus.Repair, asset4.AssetLifecycleStatus);


    }
    [Fact]
    [Trait("Category", "UnitTest")]

    public async Task IsSentToRepair_DifferentStatuses_NOT_ALL()
    {
        // Arrange
        var callerId = Guid.NewGuid();
        await using var context = new AssetsContext(ContextOptions);

        await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_FIVE_ID);

        //Stolen
        var asset1 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_ONE_ID);
        asset1!.HasBeenStolen(callerId);
        Assert.Throws<InvalidAssetDataException>(() => asset1.IsSentToRepair(callerId));
        Assert.Equal(AssetLifecycleStatus.Stolen, asset1.AssetLifecycleStatus);

        //PendingReturn 
        var asset3 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_THREE_ID);
        asset3!.MakeAssetExpiresSoon(callerId);
        asset3.MakeReturnRequest(callerId);
        Assert.Throws<InvalidAssetDataException>(() => asset3.IsSentToRepair(callerId));
        Assert.Equal(AssetLifecycleStatus.PendingReturn, asset3.AssetLifecycleStatus);

        //BoughtByUser
        var asset4 = await context.AssetLifeCycles.Include(x => x.ContractHolderUser).FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_TWO_ID);
        asset4!.MakeAssetExpiresSoon(callerId);
        asset4.BuyoutDevice(callerId);
        Assert.Throws<InvalidAssetDataException>(() => asset4.IsSentToRepair(callerId));
        Assert.Equal(AssetLifecycleStatus.BoughtByUser, asset4.AssetLifecycleStatus);

        //Lost
        var asset5 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_FIVE_ID);
        asset5!.ReportDevice(ReportCategory.Lost, callerId);
        Assert.Throws<InvalidAssetDataException>(() => asset5.IsSentToRepair(callerId));
        Assert.Equal(AssetLifecycleStatus.Lost, asset5.AssetLifecycleStatus);

        //Returned 
        asset3.ConfirmReturnDevice(callerId, "Office", "Broken");
        Assert.Throws<InvalidAssetDataException>(() => asset3.IsSentToRepair(callerId));
        Assert.Equal(AssetLifecycleStatus.Returned, asset3.AssetLifecycleStatus);

        //Discarded
        var asset7 = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_SEVEN_ID);
        asset7!.RepairCompleted(callerId, true);
        Assert.Throws<InvalidAssetDataException>(() => asset7.IsSentToRepair(callerId));
        Assert.Equal(AssetLifecycleStatus.Discarded, asset7.AssetLifecycleStatus);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task DeactivateAssetLifecycleStatus()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        var assetBefore = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_SIX_ID);
        Assert.Equal(AssetLifecycleStatus.Active, assetBefore!.AssetLifecycleStatus);

        var body = new ChangeAssetStatus
        {

            CallerId = CALLER_ID,
            AssetLifecycleId = new List<Guid> { ASSETLIFECYCLE_SIX_ID }
        };

        // Act
        await assetService.DeactivateAssetLifecycleStatus(COMPANY_ID, body);

        var assetAfter = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_SIX_ID);

        // Assert
        Assert.Equal(AssetLifecycleStatus.Inactive, assetAfter!.AssetLifecycleStatus);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task ActivateAssetLifecycleStatus()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);

        var body = new ChangeAssetStatus
        {

            CallerId = CALLER_ID,
            AssetLifecycleId = new List<Guid> { ASSETLIFECYCLE_ONE_ID }
        };

        // Act
        await assetService.DeactivateAssetLifecycleStatus(COMPANY_ID, body);
        var asset = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_ONE_ID);
        Assert.Equal(AssetLifecycleStatus.Inactive, asset!.AssetLifecycleStatus);
        await assetService.ActivateAssetLifecycleStatus(COMPANY_ID, body);
        // Assert
        Assert.Equal(AssetLifecycleStatus.Active, asset.AssetLifecycleStatus);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecyclesFromListAsync_ShouldIncludeAllValues()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        //Act
        var assetList = await assetRepository.GetAssetLifecyclesFromListAsync(COMPANY_ID, new List<Guid> { ASSETLIFECYCLE_TWO_ID },
            includeAsset: true,
            includeLabels: true,
            includeContractHolderUser: true,
            includeImeis: true,
            asNoTracking: true);
        //Assert
        Assert.Equal(1, assetList[0].Labels.Count);
        Assert.Equal(ASSETHOLDER_TWO_ID, assetList[0].ContractHolderUser!.ExternalId);

    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecyclesAsync_ShouldIncludeAllValues()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        //Act
        var assetList = await assetRepository.GetAssetLifecyclesAsync(COMPANY_ID, null, null, null, null, null, null, null, null, null, null, 1, 10, CancellationToken.None,
            includeContractHolderUser: true, includeLabels: true);

        //Assert
        Assert.Equal(1, assetList.Items[2].Labels.Count);
        Assert.NotNull(assetList.Items[2].ContractHolderUser);
        Assert.Equal(ASSETHOLDER_TWO_ID, assetList.Items[2].ContractHolderUser.ExternalId);
        Assert.Equal(ASSETLIFECYCLE_TWO_ID, assetList.Items[2].ExternalId);
    }


    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecycleAsync_GetAssetByUserId()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        // Act
        var userId = ASSETHOLDER_TWO_ID.ToString();
        var asset = await assetRepository.GetAssetLifecycleAsync(COMPANY_ID, ASSETLIFECYCLE_TWO_ID, userId, null);

        // Assert
        Assert.NotNull(asset);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecycleAsync_GetAssetByUserId_EmptyGuid()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        // Act
        var userId = Guid.Empty.ToString();
        var asset = await assetRepository.GetAssetLifecycleAsync(COMPANY_ID, ASSETLIFECYCLE_TWO_ID, userId, null);

        // Assert
        Assert.Null(asset);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecycleAsync_GetAsset()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        // Act
        var asset = await assetRepository.GetAssetLifecycleAsync(COMPANY_ID, ASSETLIFECYCLE_TWO_ID, null, null);

        // Assert
        Assert.NotNull(asset);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecycleAsync_GetAssetForDepartment()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        // Act
        var asset = await assetRepository.GetAssetLifecycleAsync(COMPANY_ID, ASSETLIFECYCLE_FIVE_ID, null, new List<Guid?> { DEPARTMENT_ID });

        // Assert
        Assert.NotNull(asset);
        Assert.Equal(DEPARTMENT_ID, asset.ManagedByDepartmentId);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecyclesAsync_SearchByUsername()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        //Act
        var assetList = await assetRepository.GetAssetLifecyclesAsync(COMPANY_ID, null, null, null, null, null, null, null, null, null, "atish", 1, 10, CancellationToken.None
            ,includeContractHolderUser: true);

        //Assert
        Assert.Equal(1, assetList.Items.Count);
        Assert.NotNull(assetList.Items[0].ContractHolderUser);
        Assert.Equal(ASSETHOLDER_TWO_ID, assetList.Items[0].ContractHolderUser.ExternalId);
        Assert.Equal("Atish", assetList.Items[0].ContractHolderUser.Name);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssetLifecycle_SetPendingRecycledStatus()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);

        //Act
        var asset = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_TWO_ID);
        Assert.Equal(AssetLifecycleStatus.InUse, asset!.AssetLifecycleStatus);
        asset.PendingRecycle(CALLER_ID);

        //Assert
        Assert.Equal(AssetLifecycleStatus.PendingRecycle, asset.AssetLifecycleStatus);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssetLifecycle_SetPendingRecycledStatus_LifecycleNotTransactional_ThrowsException()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);

        //Act
        var asset = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_ONE_ID);

        //Assert
        Assert.Equal(AssetLifecycleStatus.Active, asset!.AssetLifecycleStatus);
        Assert.Throws<InvalidAssetDataException>(() => asset.PendingRecycle(CALLER_ID));
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AssetLifecycle_SetRecycledStatus_NotValidAssetStatus_ThrowsException()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);

        //Act
        var asset = await context.AssetLifeCycles.FirstOrDefaultAsync(a => a.ExternalId == ASSETLIFECYCLE_SEVEN_ID);

        //Assert
        Assert.Equal(AssetLifecycleStatus.Repair, asset!.AssetLifecycleStatus);
        Assert.Throws<InvalidAssetDataException>(() => asset.Recycle(CALLER_ID));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task FindTechstepProductsAsync_CheckWithSearchStrings()
    {
        // Arrange
        var techstepCoreProductsRepositoryMock = new Mock<ITechstepCoreProductsRepository>();
        const string PRODUCT_SEARCH_STRING = "S20";
        techstepCoreProductsRepositoryMock.Setup(p => p.GetPartNumbersAsync(PRODUCT_SEARCH_STRING)).ReturnsAsync(new List<TechstepProduct>
        {
            new() { Description = "SAMSUNG S20 BLUE", TechstepPartNumber = "3030303" },
            new() { Description = "SAMSUNG S20 BLACK", TechstepPartNumber = "3030304" }
        });
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), Mock.Of<IAssetLifecycleRepository>(), _mapper, new Mock<IEmailService>().Object, techstepCoreProductsRepositoryMock.Object);

        // Act
        var techstepProductMatches = await assetService.FindTechstepProductsAsync(PRODUCT_SEARCH_STRING);

        // Assert
        Assert.Equal(2, techstepProductMatches.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task FindTechstepProductsAsync_TechstepRepositoryNotInitialized_ThrowsException()
    {
        // Arrange
        const string PRODUCT_SEARCH_STRING = "S20";
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), Mock.Of<IAssetLifecycleRepository>(), _mapper, new Mock<IEmailService>().Object);

        // Act & Assert
        await Assert.ThrowsAsync<TechstepRepositorySetupException>(async () => await assetService.FindTechstepProductsAsync(PRODUCT_SEARCH_STRING));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetLifecycleCountForCustomerAsync_ForAllAssetLifecyclesWithActiveStates()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        //All the active states at this time
        var statusesInActiveState = new List<AssetLifecycleStatus> {
            AssetLifecycleStatus.Active,
            AssetLifecycleStatus.Repair,
            AssetLifecycleStatus.InUse,
            AssetLifecycleStatus.Available,
            AssetLifecycleStatus.InputRequired,
            AssetLifecycleStatus.ExpiresSoon,     // Not in counter
            AssetLifecycleStatus.PendingReturn,  // Not in counter
            AssetLifecycleStatus.PendingRecycle,// Not in counter
            AssetLifecycleStatus.Expired};

        //Act
        var assetsCounter = await assetRepository.GetAssetLifecycleCountForCustomerAsync(COMPANY_ID, null, statusesInActiveState);

        //Assert
        Assert.Equal(0, assetsCounter.UsersPersonalAssets);
        Assert.Equal(COMPANY_ID, assetsCounter.OrganizationId);

        Assert.Equal(1, assetsCounter?.NonPersonal?.InUse);
        Assert.Equal(2, assetsCounter?.NonPersonal?.Active);
        Assert.Equal(1, assetsCounter?.NonPersonal?.InputRequired);
        Assert.Equal(0, assetsCounter?.NonPersonal?.Available);
        Assert.Equal(1, assetsCounter?.NonPersonal?.Expired);

        Assert.Equal(1, assetsCounter?.NonPersonal?.ExpiresSoon);
        Assert.Equal(1, assetsCounter?.NonPersonal?.PendingReturn);
        Assert.Equal(1, assetsCounter?.NonPersonal?.PendingRecycle);

        Assert.Equal(1, assetsCounter?.Personal?.InUse);
        Assert.Equal(0, assetsCounter?.Personal?.Active);
        Assert.Equal(0, assetsCounter?.Personal?.InputRequired);
        Assert.Equal(1, assetsCounter?.Personal?.Available);
        Assert.Equal(0, assetsCounter?.Personal?.Expired);
        Assert.Equal(0, assetsCounter?.Personal?.Repair);

        Assert.Equal(0, assetsCounter?.Personal?.ExpiresSoon);
        Assert.Equal(0, assetsCounter?.Personal?.PendingReturn);
        Assert.Equal(0, assetsCounter?.Personal?.PendingRecycle);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAssetCountForDepartmentAsync_ForAllAssetLifecyclesWithActiveStates()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());

        //All the active states at this time
        var statusesInActiveState = new List<AssetLifecycleStatus> {
            AssetLifecycleStatus.Active,
            AssetLifecycleStatus.Repair,
            AssetLifecycleStatus.InUse,
            AssetLifecycleStatus.Available,
            AssetLifecycleStatus.InputRequired,
            AssetLifecycleStatus.ExpiresSoon,     // Not in counter
            AssetLifecycleStatus.PendingReturn,  // Not in counter
            AssetLifecycleStatus.PendingRecycle,// Not in counter
            AssetLifecycleStatus.Expired};

        //Act
        var assetsCounter = await assetRepository.GetAssetCountForDepartmentAsync(COMPANY_ID, null, statusesInActiveState, departments: new List<Guid?> { DEPARTMENT_ID });

        //Assert
        Assert.Equal(0, assetsCounter.UsersPersonalAssets);
        Assert.Equal(COMPANY_ID, assetsCounter.OrganizationId);
        Assert.Equal(DEPARTMENT_ID, assetsCounter?.Departments[0]?.DepartmentId);


        Assert.Equal(1, assetsCounter?.Departments[0]?.NonPersonal?.InUse);
        Assert.Equal(1, assetsCounter?.Departments[0]?.NonPersonal?.Active);
        Assert.Equal(0, assetsCounter?.Departments[0]?.NonPersonal?.InputRequired);
        Assert.Equal(0, assetsCounter?.Departments[0]?.NonPersonal?.Available);
        Assert.Equal(1, assetsCounter?.Departments[0]?.NonPersonal?.Expired);

        Assert.Equal(1, assetsCounter?.Departments[0]?.NonPersonal?.ExpiresSoon);
        Assert.Equal(1, assetsCounter?.Departments[0]?.NonPersonal?.PendingReturn);
        Assert.Equal(1, assetsCounter?.Departments[0]?.NonPersonal?.PendingRecycle);

        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.PendingRecycle);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.PendingReturn);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.ExpiresSoon);

        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.InUse);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.Active);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.InputRequired);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.Available);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.Expired);
        Assert.Equal(0, assetsCounter?.Departments[0]?.Personal.Repair);


    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task DeactivateAssetLifecycleStatus_MapImeiToViewModel()
    {
        // Arrange
        await using var context = new AssetsContext(ContextOptions);
        var assetRepository =
            new AssetLifecycleRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var assetService = new AssetServices(Mock.Of<ILogger<AssetServices>>(), assetRepository, _mapper, new Mock<IEmailService>().Object);
        ChangeAssetStatus change = new ChangeAssetStatus { AssetLifecycleId = new List<Guid> { ASSETLIFECYCLE_ONE_ID}, CallerId = Guid.NewGuid() };
        // Act & Assert
        var activateAssets = await assetService.DeactivateAssetLifecycleStatus(COMPANY_ID, change);
        Assert.NotNull(activateAssets);
        Assert.Equal(1, activateAssets.Count);
        Assert.All(activateAssets, assetLifecycle => Assert.Equal(1,assetLifecycle.Asset.Imeis.Count));
        Assert.All(activateAssets, assetLifecycle => Assert.All(assetLifecycle.Asset.Imeis, imei => Assert.Equal(500119468586675, imei)));
    }

}