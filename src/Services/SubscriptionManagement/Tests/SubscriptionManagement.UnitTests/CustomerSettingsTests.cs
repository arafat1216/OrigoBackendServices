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

namespace SubscriptionManagement.UnitTests
{
    public class CustomerSettingsTests : SubscriptionManagementServiceBaseTests
    {
        private readonly SubscriptionManagementContext _subscriptionManagementContext;
        private readonly ICustomerSettingsService _customerSettingsService;
        private readonly IMapper? _mapper;

        public CustomerSettingsTests() : base(
                new DbContextOptionsBuilder<SubscriptionManagementContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitesubscriptionmanagementcustomersettingsunittests.db")
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
            _customerSettingsService = new CustomerSettingsService(customerSettingsRepository, _mapper);
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
            await _customerSettingsService.AddOperatorsForCustomerAsync(ORGANIZATION_ONE_ID, new List<int> { 1, 2 }, CALLER_ONE_ID);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorSettings.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task UpdateCustomerOperator()
        {
            await _customerSettingsService.AddOperatorsForCustomerAsync(ORGANIZATION_ONE_ID, new List<int> { 1, 2 }, CALLER_ONE_ID);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorSettings.Count());

            await _customerSettingsService.AddOperatorsForCustomerAsync(ORGANIZATION_ONE_ID, new List<int> { 3 }, CALLER_ONE_ID);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(4, _subscriptionManagementContext.CustomerOperatorSettings.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task DeleteCustomerOperator()
        {
            await _customerSettingsService.AddOperatorsForCustomerAsync(ORGANIZATION_ONE_ID, new List<int> { 1, 2 }, CALLER_ONE_ID);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorSettings.Count());

            await _customerSettingsService.DeleteOperatorForCustomerAsync(ORGANIZATION_ONE_ID, 1);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(2, _subscriptionManagementContext.CustomerSettings.FirstOrDefault().CustomerOperatorSettings.Count());
        }
    }
}
