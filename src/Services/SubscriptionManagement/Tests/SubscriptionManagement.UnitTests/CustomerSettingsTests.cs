using System;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using SubscriptionManagementServices.Exceptions;
using Xunit;
using Moq;
using Common.Logging;
using MediatR;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.UnitTests
{
    public class CustomerSettingsTests : SubscriptionManagementServiceBaseTests
    {
        private readonly SubscriptionManagementContext _subscriptionManagementContext;
        private readonly ICustomerSettingsService _customerSettingsService;
        private readonly IMapper? _mapper;
        private readonly OperatorRepository _operatorRepository;

        public CustomerSettingsTests() : base(
                new DbContextOptionsBuilder<SubscriptionManagementContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitecustomersettingsunittests.db")
                    .Options
            )
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
            var customerSettingsRepository = new CustomerSettingsRepository(_subscriptionManagementContext, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            _operatorRepository = new OperatorRepository(_subscriptionManagementContext);
            _customerSettingsService = new CustomerSettingsService(customerSettingsRepository, _operatorRepository, _mapper);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task Add_AddCustomerReferenceField_CheckTotalIncreased()
        {
            var initialCustomerReferenceFields = await _customerSettingsService.GetCustomerReferenceFieldsAsync(ORGANIZATION_ONE_ID);
            Assert.Equal(0, initialCustomerReferenceFields.Count);

            await _customerSettingsService.AddCustomerReferenceFieldAsync(ORGANIZATION_ONE_ID, "EmployeeID", "user", Guid.NewGuid());
            var updatedCustomerReferenceFields = await _customerSettingsService.GetCustomerReferenceFieldsAsync(ORGANIZATION_ONE_ID);
            Assert.Equal(1, updatedCustomerReferenceFields.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task Add_AddCustomerReferenceField_WithEmptyNameThrowsException()
        {
            await Assert.ThrowsAsync<InvalidCustomerReferenceInputDataException>(() => _customerSettingsService.AddCustomerReferenceFieldAsync(ORGANIZATION_ONE_ID, "", "user", Guid.NewGuid()));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddCustomerOperator()
        {
            NewOperatorList operatorList = new NewOperatorList();
            operatorList.Operators = new List<int> { 1, 2 };
            operatorList.CallerId = CALLER_ONE_ID;

            await _customerSettingsService.AddOperatorsForCustomerAsync(ORGANIZATION_ONE_ID,operatorList);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorSettings.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task UpdateCustomerOperator()
        {
            NewOperatorList operatorList = new NewOperatorList();
            operatorList.Operators = new List<int> { 1, 2 };
            operatorList.CallerId = CALLER_ONE_ID;

            await _customerSettingsService.AddOperatorsForCustomerAsync(ORGANIZATION_ONE_ID, operatorList);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorSettings.Count());

            NewOperatorList operatorList2 = new NewOperatorList();
            operatorList2.Operators = new List<int> { 3};
            operatorList2.CallerId = CALLER_ONE_ID;

            await _customerSettingsService.AddOperatorsForCustomerAsync(ORGANIZATION_ONE_ID, operatorList2);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(4, _subscriptionManagementContext.CustomerOperatorSettings.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task DeleteCustomerOperator()
        {
            NewOperatorList operatorList = new NewOperatorList();
            operatorList.Operators = new List<int> { 1, 2 };
            operatorList.CallerId = CALLER_ONE_ID;

            await _customerSettingsService.AddOperatorsForCustomerAsync(ORGANIZATION_ONE_ID, operatorList);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorSettings.Count());

            await _customerSettingsService.DeleteOperatorForCustomerAsync(ORGANIZATION_ONE_ID, 1);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(2, _subscriptionManagementContext.CustomerSettings.FirstOrDefault()!.CustomerOperatorSettings.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddSubscriptionProductForCustomer_Valid()
        {
            var addedCustomerSubscriptionProduct = await _customerSettingsService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID,
                1, "ProductName", new List<string> { "s1", "s2" }, Guid.NewGuid());
            Assert.Equal("ProductName", addedCustomerSubscriptionProduct.Name);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAllOperatorAccountsForCustomer_Check_Total()
        {
            var accounts = await _customerSettingsService.GetAllOperatorAccountsForCustomerAsync(ORGANIZATION_ONE_ID);
            Assert.Equal(3, accounts.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddCustomerOperatorAccount_Valid()
        {
            await _customerSettingsService.AddOperatorAccountForCustomerAsync(ORGANIZATION_ONE_ID, "AcNum4",
                "AC_Name_4", 10, Guid.NewGuid());
            Assert.Equal(4, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task DeleteCustomerOperatorAccount_Valid()
        {
            await _customerSettingsService.DeleteCustomerOperatorAccountAsync(ORGANIZATION_ONE_ID, "AC_NUM1", 10);
            Assert.Equal(2, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task DeleteCustomerOperatorAccount_Invalid()
        {
            var exception = await Record.ExceptionAsync(() =>
                _customerSettingsService.DeleteCustomerOperatorAccountAsync(ORGANIZATION_ONE_ID, "AC_NUM11", 10));

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"No customer operator account with organization ID({ORGANIZATION_ONE_ID}) and account name AC_NUM11 exists.",
                exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddCustomerOperatorAccount_SameObject()
        {
            var exception = await Record.ExceptionAsync(() =>
                _customerSettingsService.AddOperatorAccountForCustomerAsync(ORGANIZATION_ONE_ID, "AC_NUM1", "AC_NUM1",
                    10, Guid.NewGuid()));

            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal(
                $"A customer operator account with organization ID ({ORGANIZATION_ONE_ID}) and account number AC_NUM1 already exists.",
                exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddCustomerOperatorAccount_Invalid()
        {
            var exception = await Record.ExceptionAsync(() =>
                _customerSettingsService.AddOperatorAccountForCustomerAsync(ORGANIZATION_ONE_ID, "AcNum4",
                    "AC_Name_4", 111, Guid.NewGuid()));

            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("No operator exists with ID 111", exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAllOperatorsForCustomer()
        {
            var customerOperators = await _customerSettingsService.GetAllOperatorsForCustomerAsync(ORGANIZATION_ONE_ID);
            Assert.Equal(1, customerOperators.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddOperatorSubscriptionProductForCustomerAsync_WithGlobalProduct()
        {
            var newDataPackages = new List<string>
        {
            "Data Package"
        };
            var operators = await _operatorRepository.GetAllOperatorsAsync();
            var operatorId = operators.FirstOrDefault(a => a.OperatorName == "Op1");


            var subscriptionProductForCustomer = await _customerSettingsService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID, operatorId!.Id, "SubscriptionName", newDataPackages, CALLER_ONE_ID);
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
            var operators = await _operatorRepository.GetAllOperatorsAsync();
            var operatorId = operators.FirstOrDefault(a => a.OperatorName == "Op1");


            var subscriptionProductForCustomer = await _customerSettingsService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID, operatorId!.Id, "Custom Product", newDataPackages, CALLER_ONE_ID);
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

            var operators = await _operatorRepository.GetAllOperatorsAsync();
            var operatorId = operators.FirstOrDefault(a => a.OperatorName == "Op1");


            var subscriptionProductForCustomer = await _customerSettingsService.AddOperatorSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID, operatorId!.Id, "SubscriptionName", newDataPackages, CALLER_ONE_ID);
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

            var operators = await _operatorRepository.GetAllOperatorsAsync();
            var operatorId = operators.FirstOrDefault(a => a.OperatorName == "Op1");


            var subscriptionProductForCustomer = await _customerSettingsService.AddOperatorSubscriptionProductForCustomerAsync(new Guid("00000000-0000-0000-0000-000000000000"), operatorId!.Id, "SubscriptionName", newDataPackages, CALLER_ONE_ID);
            Assert.NotNull(subscriptionProductForCustomer);
            Assert.Equal(1, subscriptionProductForCustomer.Datapackages.Count);
            Assert.True(subscriptionProductForCustomer.isGlobal);
            Assert.Equal("SubscriptionName", subscriptionProductForCustomer.Name);
            Assert.Equal(operatorId.Id, subscriptionProductForCustomer.OperatorId);
        }
    }
}