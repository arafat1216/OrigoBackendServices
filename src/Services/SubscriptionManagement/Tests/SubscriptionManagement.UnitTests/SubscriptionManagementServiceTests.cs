﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Common.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using SubscriptionManagementServices.Utilities;
using Xunit;

namespace SubscriptionManagement.UnitTests;

public class SubscriptionManagementServiceTests : SubscriptionManagementServiceBaseTests
{
    private readonly ISubscriptionManagementService _subscriptionManagementService;
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
                MinDaysForCurrentOperator = 1
            }), _mapper, new Mock<IEmailService>().Object);
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
                    OperatorAccountId = 1
                }
                ));

        Assert.NotNull(exceptionOneDay);
        Assert.IsType<ArgumentException>(exceptionOneDay);
        Assert.Equal("SIM card number is required.", exceptionOneDay.Message);
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
                    OperatorAccountId = 1
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid transfer date. 1 workday ahead or more is allowed.", exception.Message);

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
                    SIMCardNumber = "[SIMCardNumber]"
                }
                ));

        Assert.NotNull(exceptionThirtyDay);
        Assert.IsType<ArgumentException>(exceptionThirtyDay);
        Assert.Equal("Invalid transfer date. 1 workday ahead or more is allowed.", exceptionThirtyDay.Message);
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
                    SIMCardNumber = "[SIMCardNumber]"
                }
                ));

        Assert.NotNull(exception);
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Invalid transfer date. 4 workdays ahead or more allowed.", exception.Message);

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
                    SIMCardNumber = "[SIMCardNumber]"
                }
                ));

        Assert.NotNull(exceptionThirtyDay);
        Assert.IsType<ArgumentException>(exceptionThirtyDay);
        Assert.Equal("Invalid transfer date. 4 workdays ahead or more allowed.", exceptionThirtyDay.Message);
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
                    SIMCardNumber = ""
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
                    SIMCardNumber = ""
                }
                ));

        Assert.NotNull(exceptionThirtyDay);
        Assert.IsType<ArgumentException>(exceptionThirtyDay);
        Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exceptionThirtyDay.Message);
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
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "[MobileNumber]",
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
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "[MobileNumber]",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
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
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "[MobileNumber]",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package",
                    AddOnProducts = new List<string> { "P1", "P2" },
                    CustomerReferenceFields = new List<NewCustomerReferenceValue> { new() { Name = "X", Type = "Y", Value = "VAL"} }
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
                    SIMCardAction = "[SIMCardAction]",
                    MobileNumber = "[MobileNumber]",
                    SubscriptionProductId = 1,
                    DataPackage = "Data Package"
                });

        Assert.NotNull(order);
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
    [InlineData("Regular", true)]
    [InlineData("Data", true)]
    [InlineData("Twin", true)]
    [InlineData("k", false)]
    [InlineData("regular", true)]
    [InlineData("REGULAR", true)]
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
}
