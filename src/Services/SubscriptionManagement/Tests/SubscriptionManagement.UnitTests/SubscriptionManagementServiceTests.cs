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
    private readonly DateTime _todayDateMock;

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
        
        _todayDateMock = new DateTime(2022,03,01);

        var mockDateTimeNow = new Mock<IDateTimeProvider>();
        mockDateTimeNow.Setup(x => x.GetNow()).Returns(_todayDateMock);

        _subscriptionManagementService = new SubscriptionManagementService(subscriptionManagementRepository,
            customerSettingsRepository,
            operatorRepository,
            Options.Create(new TransferSubscriptionDateConfiguration
            {
                MinDaysForNewOperatorWithSIM = 10,
                MinDaysForNewOperator = 4,
                MaxDaysForAll = 30,
                MinDaysForCurrentOperator = 2
            }), _mapper, new Mock<IEmailService>().Object, mockDateTimeNow.Object);

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
                    OrderExecutionDate = _todayDateMock.AddDays(1),
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
    public async Task TransferSubscription_SameOperator_NeedsToBe2OrMoreBuisinessDaysFromToday()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = _todayDateMock,
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
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_SameOperator_NeedsToBefore30BuisinessDaysFromToday()
    {
        var exceptionThirtyDay = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = _todayDateMock.AddDays(52),
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
        Assert.Equal("Invalid date. Needs to be within 30 business days.", exceptionThirtyDay.Message);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_DifferentOperator_InvalidDateTransferDate_NeedsToBe10OrMoreBuisinessDaysFromToday()
    {
        var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = _todayDateMock.AddDays(1),
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
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_DifferentOperator_InvalidDateTransferDate_30BusinessDaysMaxLimit()
    {
        var exceptionThirtyDay = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = _todayDateMock.AddDays(39),
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
        Assert.Equal("Transfer date can not be a Saturday.", exceptionThirtyDay.Message);
        Assert.IsType<ArgumentException>(exceptionThirtyDay);
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
                    OrderExecutionDate = _todayDateMock.AddDays(6),
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
    public async Task TransferSubscription_DifferentOperatorAndNoSIM_LimitedToTransfer10DaysAhead()
    {
        var exception = await Record.ExceptionAsync(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = _todayDateMock.AddDays(1),
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
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_DifferentOperatorAndNoSIM_LimitedToTransferBefore30BuisniessDays()
    {
        var exceptionThirtyDay = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = _todayDateMock.AddDays(39),
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
        Assert.Equal("Transfer date can not be a Saturday.", exceptionThirtyDay.Message);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task TransferSubscription_SimActionOrder_NoSimAddress()
    {
        var exception = await Record.ExceptionAsync(() =>
             _subscriptionManagementService.TransferPrivateToBusinessSubscriptionOrderAsync(ORGANIZATION_ONE_ID,
                new TransferToBusinessSubscriptionOrderDTO
                {
                    OrderExecutionDate = _todayDateMock.AddDays(1),
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
                    OrderExecutionDate = _todayDateMock.AddDays(7),
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
                    OrderExecutionDate = _todayDateMock.AddDays(8),
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
                    OrderExecutionDate = _todayDateMock.AddDays(6),
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
                    OrderExecutionDate = _todayDateMock.AddDays(6),
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

                    OrderExecutionDate = _todayDateMock.AddDays(7),
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
                    OrderExecutionDate = _todayDateMock.AddDays(4.5),
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

                    OrderExecutionDate = _todayDateMock.AddDays(7),
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
                    OrderExecutionDate = _todayDateMock.AddDays(7),
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
                    OrderExecutionDate = _todayDateMock.AddDays(7),
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
                    OrderExecutionDate = _todayDateMock.AddDays(4.5),
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
                    OrderExecutionDate = _todayDateMock.AddDays(1),
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
                     OrderExecutionDate = _todayDateMock.AddDays(4.5),
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
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task CancelSubscriptionOrder()
    {
        using var context = new SubscriptionManagementContext(ContextOptions);
        var subscriptionManagementRepository = new SubscriptionManagementRepository<ISubscriptionOrder>(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var operatorRepository = new OperatorRepository(context);

        var mockDateTimeNow = new Mock<IDateTimeProvider>();
        mockDateTimeNow.Setup(x => x.GetNow()).Returns(new DateTime(2022, 08, 12));

        var subscriptionManagementService = new SubscriptionManagementService(subscriptionManagementRepository,
          new Mock<ICustomerSettingsRepository>().Object,
          operatorRepository,
          Options.Create(new TransferSubscriptionDateConfiguration
          {
              MinDaysForNewOperatorWithSIM = 10,
              MinDaysForNewOperator = 4,
              MaxDaysForAll = 30,
              MinDaysForCurrentOperator = 2
          }), _mapper, new Mock<IEmailService>().Object, mockDateTimeNow.Object);

        var cancelSubscriptionOrder = new NewCancelSubscriptionOrder()
        {
            MobileNumber = "+4791111111",
            DateOfTermination = new DateTime(2022, 09, 14),
            OperatorId = 1,
            CallerId = Guid.NewGuid()
        };
        var cancelSub = await subscriptionManagementService.CancelSubscriptionOrder(ORGANIZATION_ONE_ID, cancelSubscriptionOrder);
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
    [InlineData("8947000010003122703F", false)]
    [InlineData("8947000010003122703*", false)]
    [Trait("Category", "UnitTest")]
    public void ValidateSim_CheckDigitsOnly(string value, bool expected)
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
    [InlineData("2022-03-02T00:00:00.000Z", 2, false)]
    [InlineData("2022-03-01T00:00:00.000Z", 2, false)]
    public void ValidDate_False(string transfer, int limit, bool excpected)
    {
        var transferDate = DateTime.Parse(transfer);
        //_todayDateMock = 01.03.22
        var todayDateOnly = DateOnly.FromDateTime(_todayDateMock);
        var transferDateOnly = DateOnly.FromDateTime(transferDate);

        var result = DateValidator.ValidDateForAction(transferDateOnly, todayDateOnly, limit);

        Assert.Equal(result, excpected);
    }


    [Theory]
    [InlineData("2022-04-03T00:00:00.000Z", 4)]
    [InlineData("2022-04-10T10:46:05.944Z", 4)]
    [InlineData("2022-03-05T10:46:05.944Z", 4)]
    [InlineData("2022-05-03T00:00:00.000Z", 10)]
    [InlineData("2022-04-10T00:00:00.000Z", 2)]
    [InlineData("2022-05-05T00:00:00.000Z", 2)]
    [InlineData("2022-04-09T00:00:00.000Z", 2)]
    public void ValidDate_ThrowException(string transfer, int limit)
    {
        var transferDate = DateTime.Parse(transfer);
        //_todayDateMock = 01.03.22
        var todayDateOnly = DateOnly.FromDateTime(_todayDateMock);
        var transferDateOnly = DateOnly.FromDateTime(transferDate);

        Assert.Throws<ArgumentException>(() => DateValidator.ValidDateForAction(transferDateOnly, todayDateOnly, limit));
    }
    
    
    [Theory]
    [InlineData("2022-03-15T00:00:00.000Z", 10, true)]
    [InlineData("2022-03-17T00:00:00.000Z", 10, true)]
    [InlineData("2022-03-31T00:00:00.000Z", 10, true)]
    [InlineData("2022-03-31T00:00:00.000Z", 4, true)]
    [InlineData("2022-03-17T00:00:00.000Z", 4, true)]
    [InlineData("2022-03-11T10:46:05.944Z", 4, true)]
    [InlineData("2022-03-03T00:00:00.000Z", 2, true)]
    [InlineData("2022-03-04T00:00:00.000Z", 2, true)]
    public void ValidDate_True(string transfer, int limit, bool excpected)
    {
        var transferDate = DateTime.Parse(transfer);
        //_todayDateMock = 01.03.22
        var todayDateOnly = DateOnly.FromDateTime(_todayDateMock);
        var transferDateOnly = DateOnly.FromDateTime(transferDate);

        var result = DateValidator.ValidDateForAction(transferDateOnly, todayDateOnly, limit);

        Assert.Equal(result, excpected);
    }
    [Fact]
    public void ValidDate_LatestPossibleDateADayToMuch_InValid()
    {
        var transferDate = DateTime.Parse("2022-09-13T00:00:00.000Z");
        var todayDateOnly = DateOnly.Parse("2022-08-01");
        var transferDateOnly = DateOnly.FromDateTime(transferDate);
      
        Assert.Throws<ArgumentException>(() => DateValidator.ValidDateForAction(transferDateOnly, todayDateOnly, 2));

    }
    [Theory]
    [InlineData("2022-08-15T00:00:00.000Z", "2022-08-01", 10)]  //Monday
    [InlineData("2022-08-16T00:00:00.000Z", "2022-08-02", 10)]  //Tuesday
    [InlineData("2022-08-17T00:00:00.000Z", "2022-08-03", 10)]  //Wednesday
    [InlineData("2022-08-18T00:00:00.000Z", "2022-08-04", 10)]  //Thursday
    [InlineData("2022-08-19T00:00:00.000Z", "2022-08-05", 10)]  //Friday
    [InlineData("2022-08-22T00:00:00.000Z", "2022-08-06", 10)]  //Saturday
    [InlineData("2022-08-22T00:00:00.000Z", "2022-08-07", 10)]  //Sunday
    public void ValidDate_10DaysLimit_EveryDayOfTheWeek(string transfer, string todaysDate, int limit)
    {
        var transferDate = DateTime.Parse(transfer);
        var todayDateOnly = DateOnly.Parse(todaysDate);
        var transferDateOnly = DateOnly.FromDateTime(transferDate);

        var result = DateValidator.ValidDateForAction(transferDateOnly, todayDateOnly, limit);

        Assert.True(result);
    }
    [Fact]
    public void GetDateAfter()
    {
        var result = DateValidator.GetDateAfter(10, DateOnly.Parse("2022-08-07"));

        var excpected = new DateOnly(2022, 08, 22);
       

        Assert.Equal(excpected, result);
    }
}
