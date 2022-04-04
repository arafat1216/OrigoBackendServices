using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Common.Enums;
using Common.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Exceptions;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using SubscriptionManagementServices.Types;
using SubscriptionManagementServices.Utilities;
using Xunit;

namespace SubscriptionManagement.UnitTests;

public class SubscriptionManagementServiceTests : SubscriptionManagementServiceBaseTests
{
    private readonly ISubscriptionManagementService _subscriptionManagementService;
    private readonly ICustomerSettingsService _customerSettingsService;
    private readonly IMapper? _mapper;

    public SubscriptionManagementServiceTests() : base(new DbContextOptionsBuilder<SubscriptionManagementContext>()
        // ReSharper disable once StringLiteralTypo
        .UseSqlite("Data Source=sqlitesubscriptionmanagementunittests.db").Options)
    {
        if (_mapper == null)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddMaps(Assembly.GetAssembly(typeof(SubscriptionProduct)));
            });
            _mapper = mappingConfig.CreateMapper();
        }

        var subscriptionManagementContext = new SubscriptionManagementContext(ContextOptions);
        ISubscriptionManagementRepository<ISubscriptionOrder> subscriptionManagementRepository = new SubscriptionManagementRepository<ISubscriptionOrder>(subscriptionManagementContext, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var customerSettingsRepository = new CustomerSettingsRepository(subscriptionManagementContext, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var operatorRepository = new OperatorRepository(subscriptionManagementContext);
        _subscriptionManagementService = new SubscriptionManagementService(subscriptionManagementRepository,
            customerSettingsRepository,
            operatorRepository,
            Options.Create(new TransferSubscriptionDateConfiguration
            {
                MinDaysForNewOperatorWithSIM = 10,
                MinDaysForNewOperator = 4,
                MaxDaysForAll = 30,
                MinDaysForCurrentOperator = 2
            }), _mapper, new Mock<IEmailService>().Object);

        _customerSettingsService = new CustomerSettingsService(customerSettingsRepository, operatorRepository, _mapper);
    }
   

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Same_Operator_EmptySIM()
    {
        var exceptionOneDay = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1),
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    OperatorAccountId = 1,
                    MobileNumber = "+4793606565",
                    OperatorId = 10,

                }
                ));

        Assert.NotNull(exceptionOneDay);
        Assert.IsType<ArgumentException>(exceptionOneDay);
        Assert.Equal("Invalid transfer date. 2 workday ahead or more is allowed.", exceptionOneDay.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Same_Operator_InvalidDate()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow,
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "[SIMCardNumber]",
                    OperatorAccountId = 1,
                    MobileNumber = "+4793606565",
                    OperatorId = 10
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid transfer date. 2 workday ahead or more is allowed.", exception.Message);

        var exceptionThirtyDay = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(30.5),
                    OperatorAccountId = 1,
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "[SIMCardNumber]",
                    MobileNumber = "+4793606565",
                    OperatorId = 10
                }
                ));

        Assert.NotNull(exceptionThirtyDay);
        Assert.IsType<ArgumentException>(exceptionThirtyDay);
        Assert.Equal("Invalid transfer date. 2 workday ahead or more is allowed.", exceptionThirtyDay.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Diff_Operator_InvalidDate()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 11, // Op2
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardAction = "Order",
                    SimCardAddress = new SimCardAddressRequestDTO
                    {
                        FirstName = "Peter",
                        LastName = "Pan",
                        Address = "Ocean Road",
                        Country = "Wonderland",
                        PostalCode = "1111",
                        PostalPlace = "Beach"
                    },
                    MobileNumber = "+4745454847",
                    OperatorId = 11
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exception.Message);

        var exceptionThirtyDay = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(30.5),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 11, // Op2
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardAction = "Order",
                    SimCardAddress = new SimCardAddressRequestDTO
                    {
                        FirstName = "Peter",
                        LastName = "Pan",
                        Address = "Ocean Road",
                        Country = "Wonderland",
                        PostalCode = "1111",
                        PostalPlace = "Beach"
                    },
                    MobileNumber = "+4745454847",
                    OperatorId = 11
                }
                ));

        Assert.NotNull(exceptionThirtyDay);
        Assert.IsType<ArgumentException>(exceptionThirtyDay);
        Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exceptionThirtyDay.Message);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_SimCardNumberNullAndSimCardTypeNew_Exception()
    {
        var exception = await Assert.ThrowsAsync<InvalidSimException>(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op2"
                    },
                    OrderExecutionDate = DateTime.UtcNow.AddDays(5),
                    OperatorAccountId = 1,
                    SIMCardNumber = "",
                    SIMCardAction = "New",
                    MobileNumber = "+4748494690",
                    BusinessSubscription = new BusinessSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    OperatorId = 10
                }
                ));

        Assert.NotNull(exception);
        Assert.Equal("SIM card action is New and Sim card number is empty", exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Diff_Operator_No_SIM_InvalidDate()
    {
        var exception = await Record.ExceptionAsync(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 11, // Op2
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "",
                    SIMCardAction = "Order",
                    SimCardAddress = new SimCardAddressRequestDTO
                    {
                        FirstName = "Peter",
                        LastName = "Pan",
                        Address = "Ocean Road",
                        Country = "Wonderland",
                        PostalCode = "1111",
                        PostalPlace = "Beach"
                    },
                    MobileNumber = "+4790262589",
                    OperatorId = 11
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exception.Message);

        var exceptionThirtyDay = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(30.5),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 11, // Op2
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "",
                    SIMCardAction = "Order",
                    SimCardAddress = new SimCardAddressRequestDTO
                    {
                        FirstName = "Peter",
                        LastName = "Pan",
                        Address = "Ocean Road",
                        Country = "Wonderland",
                        PostalCode = "1111",
                        PostalPlace = "Beach"
                    },
                    MobileNumber = "+4792603232",
                    OperatorId = 11,
                }
                ));

        Assert.NotNull(exceptionThirtyDay);
        Assert.IsType<ArgumentException>(exceptionThirtyDay);
        Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exceptionThirtyDay.Message);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_SimActionOrder_NoSimAddress()
    {
        var exception = await Record.ExceptionAsync(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 11, // Op2
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "",
                    SIMCardAction = "Order",
                    MobileNumber = "+4790262589",
                    OperatorId = 11
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<InvalidSimException>(exception);
        Assert.Equal("SIM card action is Order and Sim card address is empty", exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Same_Operator_Valid()
    {
        var order = await _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1.5),
                    OperatorAccountId = 1,
                    PrivateSubscription = new PrivateSubscriptionDTO
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
                    SIMCardNumber = "89722020101228153578",
                    OperatorId = 10,
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "+4790988787",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue>()
                }
                );

        Assert.NotNull(order);
        Assert.NotEmpty(order.SIMCardAction);
        Assert.NotEmpty(order.MobileNumber);
        Assert.NotEmpty(order.SIMCardNumber!);
        Assert.NotNull(order.PrivateSubscription);
        Assert.Null(order.BusinessSubscription);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Diff_Operator_New_SIM()
    {
        var order = await _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 10, // Op1
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op2",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]"
                    },
                    SIMCardNumber = "89722020101228153578",
                    SIMCardAction = "New",
                    MobileNumber = "+4745654842",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    OperatorId = 10,
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue>()
                }
                );

        Assert.NotNull(order);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Invalid_CustomerRef()
    {
        const string missingCustomerReferenceField = "X";
        var exception = await Assert.ThrowsAsync<CustomerReferenceFieldMissingException>(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 10, // Op1
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
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
                    SIMCardNumber = "89722020101228153578",
                    SIMCardAction = "New",
                    MobileNumber = "+4741414141",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    OperatorId = 10,
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue> { new() { Name = missingCustomerReferenceField, Type = CustomerReferenceTypes.User.ToString(), Value = "VAL" } }
                }
                ));
        Assert.Equal($"The field name {missingCustomerReferenceField} is not valid for this customer.", exception.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_EmptyAddOnProduct()
    {
        var order = await _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 10, // Op1
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
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
                    SIMCardNumber = "89722020101228153578",
                    SIMCardAction = "New",
                    MobileNumber = "+4795603669",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    OperatorId = 10
                });

        Assert.NotNull(order);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_CheckingViewModelMapping()
    {
        var order = await _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {

                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    OperatorAccountId = 1,
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]",
                        RealOwner = new PrivateSubscriptionDTO
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
                        }
                    },
                    BusinessSubscription = new BusinessSubscriptionDTO
                    {
                        Name = "Org",
                        Address = "[Address]",
                        Country = "Country",
                        PostalPlace = "[Postal Place]",
                        PostalCode = "[Postal code]",
                        OrganizationNumber = "21212121212"
                    },
                    SIMCardNumber = "89722020101228153578",
                    SIMCardAction = "New",
                    MobileNumber = "+4741454848",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    OperatorId = 10,
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue> { new NewCustomerReferenceValue() { Name = "UserRef1", Type = "User", Value = "VAL" } }
                }
                );

        Assert.NotNull(order);
        Assert.NotNull(order.PrivateSubscription);
        Assert.NotNull(order.PrivateSubscription?.OperatorName);
        Assert.NotNull(order.PrivateSubscription?.FirstName);
        Assert.NotNull(order.PrivateSubscription?.LastName);
        Assert.NotNull(order.PrivateSubscription?.Address);
        Assert.NotNull(order.PrivateSubscription?.Country);
        Assert.NotNull(order.PrivateSubscription?.BirthDate);
        Assert.NotNull(order.PrivateSubscription?.PostalCode);
        Assert.NotNull(order.PrivateSubscription?.PostalPlace);

        Assert.NotNull(order.PrivateSubscription?.RealOwner?.OperatorName);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.FirstName);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.LastName);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.Address);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.Country);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.BirthDate);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.PostalCode);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.PostalPlace);

        Assert.NotNull(order.BusinessSubscription?.Name);
        Assert.NotNull(order.BusinessSubscription?.OrganizationNumber);
        Assert.NotNull(order.BusinessSubscription?.Address);
        Assert.NotNull(order.BusinessSubscription?.Country);
        Assert.NotNull(order.BusinessSubscription?.PostalCode);
        Assert.NotNull(order.BusinessSubscription?.PostalPlace);

        Assert.NotNull(order.SubscriptionProductName);
        Assert.NotEmpty(order.SIMCardAction);
        Assert.NotEmpty(order.MobileNumber);
        Assert.NotEmpty(order.SIMCardNumber);
        Assert.IsType<DateTime>(order.OrderExecutionDate);
        Assert.NotEmpty(order.OrderExecutionDate.ToString());
        Assert.NotNull(order.DataPackage);
        Assert.NotNull(order.OperatorName);
        Assert.Equal(2, order.AddOnProducts.Count);
        Assert.Equal("P1", order.AddOnProducts[0]);
        Assert.Equal("P2", order.AddOnProducts[1]);
        Assert.Equal(1, order.CustomerReferenceFields.Count);
        Assert.Equal("UserRef1", order.CustomerReferenceFields[0].Name);
        Assert.Equal("VAL", order.CustomerReferenceFields[0].Value);
        Assert.Equal("User", order.CustomerReferenceFields[0].Type);

    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task NewSubscriptionOrder_()
    {

        var exception = await Assert.ThrowsAsync<CustomerSettingsException>(() => _subscriptionManagementService.NewSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new NewSubscriptionOrderRequestDTO 
                    { 
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]",
                        RealOwner = new PrivateSubscriptionDTO
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
                        }
                    },
                    BusinessSubscription = new BusinessSubscriptionDTO
                    {
                        Name = "Org",
                        Address = "[Address]",
                        Country = "Country",
                        PostalPlace = "[Postal Place]",
                        PostalCode = "[Postal code]",
                        OrganizationNumber = "21212121212"
                    },
                    OperatorId = 10,
                    SimCardNumber = "89722020101228153578",
                    SimCardAction = "New",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue> { new NewCustomerReferenceValue() { Name = "UserRef1", Type = "User", Value = "VAL" } }
                }));

        Assert.NotNull(exception);
        Assert.Equal("Customer don't have a sufficient billing information", exception.Message);
    }
        [Fact]
    [Trait("Category", "UnitTest")]
    public async Task NewSubscriptionOrder_CheckingViewModelMapping()
    {
        var order = await _subscriptionManagementService.NewSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new NewSubscriptionOrderRequestDTO
                {

                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    OperatorAccountId = 1,
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]",
                        RealOwner = new PrivateSubscriptionDTO
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
                        }
                    },
                    BusinessSubscription = new BusinessSubscriptionDTO
                    {
                        Name = "Org",
                        Address = "[Address]",
                        Country = "Country",
                        PostalPlace = "[Postal Place]",
                        PostalCode = "[Postal code]",
                        OrganizationNumber = "21212121212"
                    },
                    OperatorId = 10,
                    SimCardNumber = "89722020101228153578",
                    SimCardAction = "New",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue> { new NewCustomerReferenceValue() { Name = "UserRef1", Type = "User", Value = "VAL" } }
                }
                );

        Assert.NotNull(order);
        Assert.NotNull(order.PrivateSubscription);
        Assert.NotNull(order.PrivateSubscription?.OperatorName);
        Assert.NotNull(order.PrivateSubscription?.FirstName);
        Assert.NotNull(order.PrivateSubscription?.LastName);
        Assert.NotNull(order.PrivateSubscription?.Address);
        Assert.NotNull(order.PrivateSubscription?.Country);
        Assert.NotNull(order.PrivateSubscription?.BirthDate);
        Assert.NotNull(order.PrivateSubscription?.PostalCode);
        Assert.NotNull(order.PrivateSubscription?.PostalPlace);

        Assert.NotNull(order.PrivateSubscription?.RealOwner?.OperatorName);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.FirstName);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.LastName);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.Address);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.Country);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.BirthDate);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.PostalCode);
        Assert.NotNull(order.PrivateSubscription?.RealOwner?.PostalPlace);

        Assert.NotNull(order.BusinessSubscription?.Name);
        Assert.NotNull(order.BusinessSubscription?.OrganizationNumber);
        Assert.NotNull(order.BusinessSubscription?.Address);
        Assert.NotNull(order.BusinessSubscription?.Country);
        Assert.NotNull(order.BusinessSubscription?.PostalCode);
        Assert.NotNull(order.BusinessSubscription?.PostalPlace);

        Assert.NotNull(order.SubscriptionProductName);
        Assert.NotEmpty(order.SimCardAction);
        Assert.NotEmpty(order.SimCardNumber);
        Assert.IsType<DateTime>(order.OrderExecutionDate);
        Assert.NotEmpty(order.OrderExecutionDate.ToString());
        Assert.NotNull(order.DataPackage);
        Assert.Equal(10,order.OperatorId);
        Assert.Equal(2, order.AddOnProducts.Count);
        Assert.Equal("P1", order.AddOnProducts[0]);
        Assert.Equal("P2", order.AddOnProducts[1]);
        Assert.Equal(1, order.CustomerReferenceFields.Count);
        Assert.Equal("UserRef1", order.CustomerReferenceFields[0].Name);
        Assert.Equal("VAL", order.CustomerReferenceFields[0].Value);
        Assert.Equal("User", order.CustomerReferenceFields[0].Type);

    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Valid_KeepSIM_NoSIMCardNumber_SameOperator()
    {
        var result = await _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 10, // Op1
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
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
                    SIMCardAction = "Keep",
                    MobileNumber = "+4741414141",
                    OperatorId = 10,
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue>()
                }
                );

        Assert.NotNull(result);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Invalid_KeepSIM_WithDifferentOperator()
    {
        var exception = await Assert.ThrowsAsync<InvalidSimException>(() =>
        _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 10, // Op1
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op2",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]"
                    },
                    SIMCardAction = "Keep",
                    MobileNumber = "+4741414141",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    OperatorId = 10,
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue>()
                }));

        Assert.NotNull(exception);
        Assert.Equal("SIM card action is Keep and Sim card number is empty", exception.Message);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_Invalid_PhoneNumber()
    {
        var exception = await Assert.ThrowsAsync<InvalidPhoneNumberException>(() =>
        _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                    NewOperatorAccount = new NewOperatorAccountRequestedDTO
                    {
                        OperatorId = 10, // Op1
                        NewOperatorAccountPayer = "[OperatorAccountPayer]",
                        NewOperatorAccountOwner = "[OperatorAccountOwner]"
                    },
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op2",
                        FirstName = "[FName]",
                        LastName = "[LName]",
                        Address = "[Address]",
                        Country = "Country",
                        BirthDate = DateTime.UtcNow.AddMonths(-100),
                        Email = "email@mail.com",
                        PostalCode = "[Postal code]",
                        PostalPlace = "[Postal Place]"
                    },
                    SIMCardAction = "Keep",
                    MobileNumber = "+474141",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    OperatorId = 10,
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue>()
                }));

        Assert.NotNull(exception);
        Assert.Equal("Phone number +474141 not valid for countrycode No.", exception.Message);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task Transfer2B_NoOperatorAccountOROperatorTelephoneNumberORNewOperatorAccount()
    {
        var exception = await Record.ExceptionAsync(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = DateTime.UtcNow.AddDays(1),
                    PrivateSubscription = new PrivateSubscriptionDTO
                    {
                        OperatorName = "Op1"
                    },
                    SIMCardNumber = "",
                    SIMCardAction = "Order",
                    SimCardAddress = new SimCardAddressRequestDTO
                    {
                        FirstName = "Peter",
                        LastName = "Pan",
                        Address = "Ocean Road",
                        Country = "Wonderland",
                        PostalCode = "1111",
                        PostalPlace = "Beach"
                    },
                    MobileNumber = "+4790262589",
                    OperatorId = 11
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Operator account id, new operator information or operator phone number must be provided.", exception.Message);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task Transfer2B_NoOperatorId_ForNewSubscription()
    {
        var exception = await Record.ExceptionAsync(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                 new TransferToBusinessSubscriptionOrderDTO
                 {
                     OrderExecutionDate = DateTime.UtcNow.AddDays(4.5),
                     NewOperatorAccount = new NewOperatorAccountRequestedDTO
                     {
                         OperatorId = 10, // Op1
                         NewOperatorAccountPayer = "[OperatorAccountPayer]",
                         NewOperatorAccountOwner = "[OperatorAccountOwner]"
                     },
                     PrivateSubscription = new PrivateSubscriptionDTO
                     {
                         OperatorName = "Op2",
                         FirstName = "[FName]",
                         LastName = "[LName]",
                         Address = "[Address]",
                         Country = "Country",
                         BirthDate = DateTime.UtcNow.AddMonths(-100),
                         Email = "email@mail.com",
                         PostalCode = "[Postal code]",
                         PostalPlace = "[Postal Place]"
                     },
                     SIMCardAction = "Keep",
                     MobileNumber = "+4741414545",
                     SubscriptionProductId = 1,
                     DataPackage = "Data Package",
                     AddOnProducts = new List<string> { "P1", "P2" },
                     CustomerReferenceFields = new List<NewCustomerReferenceValue>()
                 }));

        Assert.NotNull(exception);
        Assert.IsType<CustomerSettingsException>(exception);
        Assert.Equal("Customer don't have settings for operator with id 0", exception.Message);
    }

    [Theory]
    [InlineData("89470000100031227032275", false)]
    [InlineData("89722020101228153", false)]
    [InlineData("89148000005339755555", true)]
    [Trait("Category", "UnitTest")]
    public void ValidateSim_CheckLenght(string value, bool expected)
    {
        var result = SIMCardValidation.ValidateSim(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("00470000131227032275", false)]
    [InlineData("89722020101228153578", true)]
    [Trait("Category", "UnitTest")]
    public void ValidateSim_CheckFirstDigits89(string value, bool expected)
    {
        var result = SIMCardValidation.ValidateSim(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("89470000131227032275",true)]
    [InlineData("89722020101228153578",true)]
    [InlineData("89148000005339755555",true)]
    [InlineData("89652021000371234219",true)]
    [Trait("Category", "UnitTest")]
    public void LuhnAlgorithm_ForOddNumbers(string value, bool expected) {
        
        var result = SIMCardValidation.LuhnAlgorithm(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("89470000131227032275", true)]
    [InlineData("89722020101228153578", true)]
    [InlineData("8914800000533975555", false)]
    [InlineData("89652021 000371234219", true)]
    [Trait("Category", "UnitTest")]
    public void ValidateSim_CheckDifferentOutcomes(string value, bool expected)
    {
        var result = SIMCardValidation.ValidateSim(value);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("KeepCurrent", true,false)]
    [InlineData("KEEP CURRENT", true, false)]
    [InlineData("keep current", true, false)]
    [InlineData("keeP Current", false, true)]
    [Trait("Category", "UnitTest")]
    public void ValidateSimAction_False(string action,bool newOperator, bool expected)
    {
        
        var result = SIMCardValidation.ValidateSimAction(action, newOperator);

        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("Main", true)]
    [InlineData("Data", true)]
    [InlineData("Twin", true)]
    [InlineData("k", false)]
    [InlineData("main", true)]
    [InlineData("MAIN", true)]
    [InlineData("", false)]
    [InlineData("            ", false)]
    [InlineData("Dta", false)]
    [InlineData("TwInSim", false)]
    [Trait("Category", "UnitTest")]
    public void ValidateSimType(string simType, bool expected)
    {

        var result = SIMCardValidation.ValidateSimType(simType);

        Assert.Equal(expected, result);
    }
  
    [Theory]
    [InlineData("Keep",SIMAction.Keep)]
    [InlineData("New",SIMAction.New)]
    [InlineData("Order",SIMAction.Order)]
    [InlineData("Keep ", SIMAction.Keep)]
    [InlineData(" New", SIMAction.New)]
    public void GetSimCardAction_ValidSimCardActions(string actionString, SIMAction action)
    {
        var result = SIMCardValidation.GetSimCardAction(actionString);

        Assert.Equal(result, action);
    }
    [Theory]
    [InlineData(" ")]
    [InlineData("N ew")]
    [InlineData("This is not an enum")]
    public void GetSimCardAction_InValidSimCardActions(string actionString)
    {
        var result = SIMCardValidation.GetSimCardAction(actionString);
        Assert.True(Convert.ToInt32(result) == 0);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public void AddSubscriptionProduct()
    {
        var subscriptionProduct = _customerSettingsService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID, 1, "GP1", new List<string> { "5 GB" }, CALLER_ONE_ID);
        Assert.NotNull(subscriptionProduct);
    }

    [Theory]
    [InlineData("2022-04-01T00:00:00.000Z", 2, false, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-04-02T00:00:00.000Z", 2, false, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-04-03T00:00:00.000Z", 2, false, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-04-04T00:00:00.000Z", 2, false, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-04-05T00:00:00.000Z", 2, true, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-05-05T00:00:00.000Z", 2, false, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-04-03T00:00:00.000Z", 2, false, "2022-04-04T12:46:05.944Z")]
    public void ValidDate_Keep(string transfer, int limit, bool excpected, string today)
    {
        var todayDate = DateTime.Parse(today);
        var transferDate = DateTime.Parse(transfer);

        var result = DateValidator.ValidDateForAction(transferDate, todayDate, limit);

        Assert.Equal(result, excpected);
    }
    [Theory]
    [InlineData("2022-04-05T00:00:00.000Z", 4, false, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-04-07T00:00:00.000Z", 4, true, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-05-05T00:00:00.000Z", 4, false, "2022-04-01T12:46:05.944Z")]
    public void ValidDate_New(string transfer, int limit, bool excpected, string today)
    {
        var todayDate = DateTime.Parse(today);
        var transferDate = DateTime.Parse(transfer);

        var result = DateValidator.ValidDateForAction(transferDate, todayDate, limit);

        Assert.Equal(result, excpected);
    }
    [Theory]
    [InlineData("2022-04-15T00:00:00.000Z", 10, true, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-04-14T00:00:00.000Z", 10, false, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-05-03T00:00:00.000Z", 10, false, "2022-04-01T12:46:05.944Z")]
    [InlineData("2022-04-18T00:00:00.000Z", 10, true, "2022-04-04T12:46:05.944Z")]
    public void ValidDate_Order(string transfer, int limit, bool excpected, string today)
    {
        var todayDate = DateTime.Parse(today);
        var transferDate = DateTime.Parse(transfer);

        var result = DateValidator.ValidDateForAction(transferDate, todayDate, limit);

        Assert.Equal(result, excpected);
    }
    [Fact]
    public void CountBusinessDays_TwoWeeks_ShouldBe4BuisnessDays()
    {
        var result = DateValidator.CountBusinessDays(DateTime.UtcNow,DateTime.UtcNow.AddDays(14));

        Assert.Equal(4,result);
    }
}
