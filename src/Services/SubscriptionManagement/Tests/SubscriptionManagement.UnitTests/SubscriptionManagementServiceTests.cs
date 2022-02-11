﻿using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SubscriptionManagement.UnitTests
{
    public class SubscriptionManagementServiceTests : SubscriptionManagementServiceBaseTests
    {
        private SubscriptionManagementContext _subscriptionManagementContext;
        private ISubscriptionManagementRepository _subscriptionManagementRepository;
        private ISubscriptionManagementService _subscriptionManagementService;

        public SubscriptionManagementServiceTests() : base(
                new DbContextOptionsBuilder<SubscriptionManagementContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitecustomerunittests.db")
                    .Options
            )
        {
            _subscriptionManagementContext = new SubscriptionManagementContext(ContextOptions);
            _subscriptionManagementRepository = new SubscriptionManagementRepository(_subscriptionManagementContext);
            _subscriptionManagementService = new SubscriptionManagementService(_subscriptionManagementRepository);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAllOperatorAccountsForCustomer_Check_Total()
        {
            var accounts = await _subscriptionManagementService.GetAllOperatorAccountsForCustomerAsync(CUSTOMER_ONE_ID);
            Assert.Equal(3, accounts.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddCustomerOperatorAccount_Valid()
        {
            await _subscriptionManagementService.AddOperatorAccountForCustomerAsync(CUSTOMER_ONE_ID, ORGANIZATION_ONE_ID, "AcNum4", "AC_Name_4", 1, Guid.NewGuid());
            Assert.Equal(4, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddCustomerOperatorAccount_InValid()
        {
            var exception = await Record.ExceptionAsync(() =>
                 _subscriptionManagementService.AddOperatorAccountForCustomerAsync(CUSTOMER_ONE_ID, ORGANIZATION_ONE_ID, "AcNum4", "AC_Name_4", 11, Guid.NewGuid())
            );

            Assert.Equal(3, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("No operator exists with ID 11", exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddSubscriptionOrder_Valid()
        {
            await _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID);
            Assert.Equal(1, _subscriptionManagementContext.SubscriptionOrders.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddSubscriptionOrder_InValid_SubscriptionProduct()
        {
            var exception = await Record.ExceptionAsync(() =>
                 _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(CUSTOMER_ONE_ID, 10, 1, 1, CALLER_ONE_ID)
            );

            Assert.Equal(0, _subscriptionManagementContext.SubscriptionOrders.Count());
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("No subscription product exists with ID 10", exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddSubscriptionOrder_InValid_OperatorAccount()
        {
            var exception = await Record.ExceptionAsync(() =>
                 _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(CUSTOMER_ONE_ID, 1, 10, 1, CALLER_ONE_ID)
            );

            Assert.Equal(0, _subscriptionManagementContext.SubscriptionOrders.Count());
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("No operator account exists with ID 10", exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddSubscriptionOrder_InValid_DataPackage()
        {
            var exception = await Record.ExceptionAsync(() =>
                 _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(CUSTOMER_ONE_ID, 1, 1, 10, CALLER_ONE_ID)
            );

            Assert.Equal(0, _subscriptionManagementContext.SubscriptionOrders.Count());
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("No Datapackage exists with ID 10", exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAllOperatorsForCustomer()
        {
            var customerOperators = await _subscriptionManagementService.GetAllOperatorsForCustomerAsync(CUSTOMER_ONE_ID);
            Assert.Equal(3, customerOperators.Count());
        }
    }
}