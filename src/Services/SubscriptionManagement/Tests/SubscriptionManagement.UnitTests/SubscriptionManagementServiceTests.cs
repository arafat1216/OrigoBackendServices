using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using Xunit;

namespace SubscriptionManagement.UnitTests;

public class SubscriptionManagementServiceTests : SubscriptionManagementServiceBaseTests
{
    private readonly SubscriptionManagementContext _subscriptionManagementContext;
    private readonly ISubscriptionManagementRepository _subscriptionManagementRepository;
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly IMapper? _mapper;

    public SubscriptionManagementServiceTests() : base(new DbContextOptionsBuilder<SubscriptionManagementContext>()
        // ReSharper disable once StringLiteralTypo
        .UseSqlite("Data Source=sqlitecustomerunittests.db").Options)
    {
        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddMaps(Assembly.GetAssembly(typeof(SubscriptionProduct)));
            });
            _mapper = mappingConfig.CreateMapper();
        }

        _subscriptionManagementContext = new SubscriptionManagementContext(ContextOptions);
        _subscriptionManagementRepository = new SubscriptionManagementRepository(_subscriptionManagementContext);
        var customerSettingsRepository = new CustomerSettingsRepository(_subscriptionManagementContext);
        _subscriptionManagementService = new SubscriptionManagementService(_subscriptionManagementRepository,
            customerSettingsRepository,
            Options.Create(new TransferSubscriptionDateConfiguration
            {
                MinDaysForNewOperatorWithSIM = 10,
                MinDaysForNewOperator = 4,
                MaxDaysForAll = 30,
                MinDaysForCurrentOperator = 1
            }), _mapper);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddSubscriptionProductForCustomer_Valid()
    {
        var added = await _subscriptionManagementService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID,
            1, "ProductName", new List<string> { "s1", "s2" }, Guid.NewGuid());
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAllOperatorAccountsForCustomer_Check_Total()
    {
        var accounts = await _subscriptionManagementService.GetAllOperatorAccountsForCustomerAsync(ORGANIZATION_ONE_ID);
        Assert.Equal(3, accounts.Count());
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddCustomerOperatorAccount_Valid()
    {
        await _subscriptionManagementService.AddOperatorAccountForCustomerAsync(ORGANIZATION_ONE_ID, "AcNum4",
            "AC_Name_4", 1, Guid.NewGuid());
        Assert.Equal(4, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task DeleteCustomerOperatorAccount_Valid()
    {
        await _subscriptionManagementService.DeleteCustomerOperatorAccountAsync(ORGANIZATION_ONE_ID, "AC_NUM1");
        Assert.Equal(2, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task DeleteCustomerOperatorAccount_InValid()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.DeleteCustomerOperatorAccountAsync(ORGANIZATION_ONE_ID, "AC_NUM11"));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(
            $"No customer operator account with organization ID ({ORGANIZATION_ONE_ID}) and account name AC_NUM11 exists.",
            exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddCustomerOperatorAccount_SameObject()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.AddOperatorAccountForCustomerAsync(ORGANIZATION_ONE_ID, "AC_NUM1", "AC_NUM1",
                1, Guid.NewGuid()));

        Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(
            $"A customer operator account with organization ID ({ORGANIZATION_ONE_ID}) and account number AC_NUM1 already exists.",
            exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddCustomerOperatorAccount_InValid()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.AddOperatorAccountForCustomerAsync(ORGANIZATION_ONE_ID, "AcNum4",
                "AC_Name_4", 11, Guid.NewGuid()));

        Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("No operator exists with ID 11", exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddSubscriptionOrder_Valid()
    {
        //await _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(ORGANIZATION_ONE_ID, 1, 1, 1,
        //    CALLER_ONE_ID, string.Empty);
        //Assert.Equal(1, _subscriptionManagementContext.SubscriptionOrders.Count());
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddSubscriptionOrder_InValid_SubscriptionProduct()
    {
        //var exception = await Record.ExceptionAsync(() =>
        //    _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(ORGANIZATION_ONE_ID, 10, 1, 1,
        //        CALLER_ONE_ID, string.Empty));

        //Assert.Equal(0, _subscriptionManagementContext.SubscriptionOrders.Count());
        //Assert.NotNull(exception);
        //Assert.IsType<ArgumentException>(exception);
        //Assert.Equal("No subscription product exists with ID 10", exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddSubscriptionOrder_InValid_OperatorAccount()
    {
        //var exception = await Record.ExceptionAsync(() =>
        //    _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(ORGANIZATION_ONE_ID, 1, 10, 1,
        //        CALLER_ONE_ID, string.Empty));

        //Assert.Equal(0, _subscriptionManagementContext.SubscriptionOrders.Count());
        //Assert.NotNull(exception);
        //Assert.IsType<ArgumentException>(exception);
        //Assert.Equal("No operator account exists with ID 10", exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddSubscriptionOrder_InValid_DataPackage()
    {
        //var exception = await Record.ExceptionAsync(() =>
        //    _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(ORGANIZATION_ONE_ID, 1, 1, 10,
        //        CALLER_ONE_ID, string.Empty));

        //Assert.Equal(0, _subscriptionManagementContext.SubscriptionOrders.Count());
        //Assert.NotNull(exception);
        //Assert.IsType<ArgumentException>(exception);
        //Assert.Equal("No DataPackage exists with ID 10", exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAllOperatorsForCustomer()
    {
        var customerOperators = await _subscriptionManagementService.GetAllOperatorsForCustomerAsync(ORGANIZATION_ONE_ID);
        Assert.Equal(1, customerOperators.Count());
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Same_Operator_EmptySIM()
    {
        var exception_one_day = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1),
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    OperatorAccountId = 1
                }
                ));

        Assert.NotNull(exception_one_day);
        Assert.IsType<ArgumentException>(exception_one_day);
        Assert.Equal("SIM card number is required.", exception_one_day.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Same_Operator_InvalidDate()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow,
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "[SIMCardNumber]",
                    OperatorAccountId = 1
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid transfer date. 1 workday ahead or more is allowed.", exception.Message);

        var exception_thirty_day = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(30.5),
                    OperatorAccountId = 1,
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "[SIMCardNumber]"
                }
                ));

        Assert.NotNull(exception_thirty_day);
        Assert.IsType<ArgumentException>(exception_thirty_day);
        Assert.Equal("Invalid transfer date. 1 workday ahead or more is allowed.", exception_thirty_day.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Diff_Operator_InvalidDate()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1),
                    OperatorAccount = new NewOperatorAccountRequested
                    {
                        OperatorAccountPayer = "[OperatorAccountPayer]",
                        OperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "[SIMCardNumber]"
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid transfer date. 4 workdays ahead or more allowed.", exception.Message);

        var exception_thirty_day = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(30.5),
                    OperatorAccount = new NewOperatorAccountRequested
                    {
                        OperatorAccountPayer = "[OperatorAccountPayer]",
                        OperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "[SIMCardNumber]"
                }
                ));

        Assert.NotNull(exception_thirty_day);
        Assert.IsType<ArgumentException>(exception_thirty_day);
        Assert.Equal("Invalid transfer date. 4 workdays ahead or more allowed.", exception_thirty_day.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Diff_Operator_No_SIM_InvalidDate()
    {
        var exception = await Record.ExceptionAsync(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1),
                    OperatorAccount = new NewOperatorAccountRequested
                    {
                        OperatorAccountPayer = "[OperatorAccountPayer]",
                        OperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = ""
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exception.Message);

        var exception_thirty_day = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(30.5),
                    OperatorAccount = new NewOperatorAccountRequested
                    {
                        OperatorAccountPayer = "[OperatorAccountPayer]",
                        OperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = ""
                }
                ));

        Assert.NotNull(exception_thirty_day);
        Assert.IsType<ArgumentException>(exception_thirty_day);
        Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exception_thirty_day.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Same_Operator_Valid()
    {
        var order = await _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1.5),
                    OperatorAccountId = 1,
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]"
                    },
                    SIMCardNumber = "[SIMCardNumber]",
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "[MobileNumber]",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceField> { }
                }
                );

        Assert.NotNull(order);
        Assert.NotEmpty(order.SIMCardAction);
        Assert.NotEmpty(order.MobileNumber);
        Assert.NotEmpty(order.SimCardNumber);
        Assert.NotEmpty(order.FirstName);
        Assert.NotEmpty(order.LastName);
        Assert.Equal("[]", order.CustomerReferenceFields);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Diff_Operator_New_SIM()
    {
        var order = await _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    OperatorAccount = new NewOperatorAccountRequested
                    {
                        OperatorAccountPayer = "[OperatorAccountPayer]",
                        OperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Telia - NO1",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]"
                    },
                    SIMCardNumber = "[SIMCardNumber]",
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "[MobileNumber]",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceField> { }
                }
                );

        Assert.NotNull(order);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Invalid_CustomerRef()
    {
        var exception = await Record.ExceptionAsync(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    OperatorAccount = new NewOperatorAccountRequested
                    {
                        OperatorAccountPayer = "[OperatorAccountPayer]",
                        OperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Telia - NO1",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]"
                    },
                    SIMCardNumber = "[SIMCardNumber]",
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "[MobileNumber]",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceField> { new NewCustomerReferenceField { Name = "X", Type = "Y" } }
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The field name X is not valid for this customer.", exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_EmptyAddOnProduct()
    {
        var order = await _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new PrivateToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    OperatorAccount = new NewOperatorAccountRequested
                    {
                        OperatorAccountPayer = "[OperatorAccountPayer]",
                        OperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    TransferFromPrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Telia - NO1",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]"
                    },
                    SIMCardNumber = "[SIMCardNumber]",
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "[MobileNumber]",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package"
                });

        Assert.NotNull(order);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetAllOperator()
    {
        var operators = await _subscriptionManagementService.GetAllOperatorsAsync();
        Assert.NotNull(operators);
        Assert.Equal(7, operators.Count);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetOperator()
    {
        var operator1 = await _subscriptionManagementService.GetOperatorAsync(1);
        Assert.NotNull(operator1);

        var operator2 = await _subscriptionManagementService.GetOperatorAsync(100);
        Assert.Null(operator2);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOperatorSubscriptionProductForCustomerAsync_WithGlobalProduct()
    {
        var newDataPackages = new List<string>
        {
            "Data Package"
        };
        var operators = await _subscriptionManagementRepository.GetAllOperatorsAsync();
        var operatorId = operators.FirstOrDefault(a => a.OperatorName == "Op1");


        var subscriptionProductForCustomer = await _subscriptionManagementService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID, operatorId.Id, "SubscriptionName", newDataPackages, CALLER_ONE_ID);
        Assert.NotNull(subscriptionProductForCustomer);
        Assert.Equal(1, subscriptionProductForCustomer.Datapackages.Count);
        Assert.True(subscriptionProductForCustomer.isGlobal);
        Assert.Equal("SubscriptionName", subscriptionProductForCustomer.Name);
        Assert.Equal(operatorId.Id, subscriptionProductForCustomer.OperatorId);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOperatorSubscriptionProductForCustomerAsync_WithCustomProduct()
    {
        var newDataPackages = new List<string>
        {
            "New Datapackage",
            "New Datapackage 2"
        };
        var operators = await _subscriptionManagementRepository.GetAllOperatorsAsync();
        var operatorId = operators.FirstOrDefault(a => a.OperatorName == "Op1");


        var subscriptionProductForCustomer = await _subscriptionManagementService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID, operatorId.Id, "Custom Product", newDataPackages, CALLER_ONE_ID);
        Assert.NotNull(subscriptionProductForCustomer);
        Assert.Equal(2, subscriptionProductForCustomer.Datapackages.Count);
        Assert.False(subscriptionProductForCustomer.isGlobal);
        Assert.Equal("Custom Product", subscriptionProductForCustomer.Name);
        Assert.Equal(operatorId.Id, subscriptionProductForCustomer.OperatorId);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOperatorSubscriptionProductForCustomerAsync_GlobalProductWithWrongDatapackages()
    {
        var newDataPackages = new List<string>
        {
            "New Datapackage"
        };

        var operators = await _subscriptionManagementRepository.GetAllOperatorsAsync();
        var operatorId = operators.FirstOrDefault(a => a.OperatorName == "Op1");


        var subscriptionProductForCustomer = await _subscriptionManagementService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID, operatorId.Id, "SubscriptionName", newDataPackages, CALLER_ONE_ID);
        Assert.NotNull(subscriptionProductForCustomer);
        Assert.Equal(0, subscriptionProductForCustomer.Datapackages.Count);
        Assert.True(subscriptionProductForCustomer.isGlobal);
        Assert.Equal("SubscriptionName", subscriptionProductForCustomer.Name);
        Assert.Equal(operatorId.Id, subscriptionProductForCustomer.OperatorId);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOperatorSubscriptionProductForCustomerAsync_AddingSubscriptionProductWithouCustomerSettings()
    {
        var newDataPackages = new List<string>
        {
            "Data Package"
        };

        var operators = await _subscriptionManagementRepository.GetAllOperatorsAsync();
        var operatorId = operators.FirstOrDefault(a => a.OperatorName == "Op1");


        var subscriptionProductForCustomer = await _subscriptionManagementService.AddOperatorSubscriptionProductForCustomerAsync(new Guid("00000000-0000-0000-0000-000000000000"), operatorId.Id, "SubscriptionName", newDataPackages, CALLER_ONE_ID);
        Assert.NotNull(subscriptionProductForCustomer);
        Assert.Equal(1, subscriptionProductForCustomer.Datapackages.Count);
        Assert.True(subscriptionProductForCustomer.isGlobal);
        Assert.Equal("SubscriptionName", subscriptionProductForCustomer.Name);
        Assert.Equal(operatorId.Id, subscriptionProductForCustomer.OperatorId);
    }
}