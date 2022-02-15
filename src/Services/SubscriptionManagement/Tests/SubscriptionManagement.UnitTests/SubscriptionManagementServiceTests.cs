using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using System;
using System.Collections.Generic;
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
            _subscriptionManagementService = new SubscriptionManagementService(_subscriptionManagementRepository, Options.Create(new TransferSubscriptionDateConfiguration
            {
                MinDaysForNewOperatorWithSIM = 10,
                MinDaysForNewOperator = 4,
                MaxDaysForAll = 30,
                MinDaysForCurrentOperator = 1
            }));
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddSubscriptionProductForCustomer_Valid()
        {
            var added = await _subscriptionManagementService.AddSubscriptionProductForCustomerAsync(ORGANIZATION_ONE_ID, "Op1", "ProductName", new List<string> { "s1", "s2" }, Guid.NewGuid(), false);
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
            await _subscriptionManagementService.AddOperatorAccountForCustomerAsync(ORGANIZATION_ONE_ID, "AcNum4", "AC_Name_4", 1, Guid.NewGuid());
            Assert.Equal(4, _subscriptionManagementContext.CustomerOperatorAccounts.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddCustomerOperatorAccount_InValid()
        {
            var exception = await Record.ExceptionAsync(() =>
                 _subscriptionManagementService.AddOperatorAccountForCustomerAsync(ORGANIZATION_ONE_ID, "AcNum4", "AC_Name_4", 11, Guid.NewGuid())
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
            await _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, string.Empty);
            Assert.Equal(1, _subscriptionManagementContext.SubscriptionOrders.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddSubscriptionOrder_InValid_SubscriptionProduct()
        {
            var exception = await Record.ExceptionAsync(() =>
                 _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(CUSTOMER_ONE_ID, 10, 1, 1, CALLER_ONE_ID, string.Empty)
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
                 _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(CUSTOMER_ONE_ID, 1, 10, 1, CALLER_ONE_ID, string.Empty)
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
                 _subscriptionManagementService.AddSubscriptionOrderForCustomerAsync(CUSTOMER_ONE_ID, 1, 1, 10, CALLER_ONE_ID, string.Empty)
            );

            Assert.Equal(0, _subscriptionManagementContext.SubscriptionOrders.Count());
            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("No DataPackage exists with ID 10", exception.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAllOperatorsForCustomer()
        {
            var customerOperators = await _subscriptionManagementService.GetAllOperatorsForCustomerAsync(CUSTOMER_ONE_ID);
            Assert.Equal(3, customerOperators.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TransferSubscription_Same_Operator_EmptySIM()
        {
            var exception_one_day = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "", DateTime.UtcNow, 1)
            );

            Assert.NotNull(exception_one_day);
            Assert.IsType<ArgumentException>(exception_one_day);
            Assert.Equal("SIM card number is required.", exception_one_day.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TransferSubscription_Same_Operator_InvalidDate()
        {
            var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "[SimCardNumber]", DateTime.UtcNow, 1)
            );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Invalid transfer date. 1 workday ahead or more is allowed.", exception.Message);

            var exception_thirty_day = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "[SimCardNumber]", DateTime.UtcNow.AddDays(30.5), 1)
            );

            Assert.NotNull(exception_thirty_day);
            Assert.IsType<ArgumentException>(exception_thirty_day);
            Assert.Equal("Invalid transfer date. 1 workday ahead or more is allowed.", exception_thirty_day.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TransferSubscription_Diff_Operator_InvalidDate()
        {
            var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "[SimCardNumber]", DateTime.UtcNow, 2)
            );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Invalid transfer date. 4 workdays ahead or more allowed.", exception.Message);

            var exception_thirty_day = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "[SimCardNumber]", DateTime.UtcNow.AddDays(30.5), 2)
            );

            Assert.NotNull(exception_thirty_day);
            Assert.IsType<ArgumentException>(exception_thirty_day);
            Assert.Equal("Invalid transfer date. 4 workdays ahead or more allowed.", exception_thirty_day.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TransferSubscription_Diff_Operator_No_SIM_InvalidDate()
        {
            var exception = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "", DateTime.UtcNow, 2)
            );

            Assert.NotNull(exception);
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exception.Message);

            var exception_thirty_day = await Record.ExceptionAsync(() =>
            _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "", DateTime.UtcNow.AddDays(65), 2)
            );

            Assert.NotNull(exception_thirty_day);
            Assert.IsType<ArgumentException>(exception_thirty_day);
            Assert.Equal("Invalid transfer date. 10 workdays ahead or more is allowed.", exception_thirty_day.Message);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TransferSubscription_Same_Operator_Valid()
        {
            var order = await _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "[SIM]", DateTime.UtcNow.AddDays(1.5), 1);

            Assert.NotNull(order);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TransferSubscription_Diff_Operator_New_SIM()
        {
            var order = await _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "[SIM]", DateTime.UtcNow.AddDays(4.5), 2);

            Assert.NotNull(order);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task TransferSubscription_Diff_Operator_Order_SIM()
        {
            var order = await _subscriptionManagementService.TransferSubscriptionOrderAsync(CUSTOMER_ONE_ID, 1, 1, 1, CALLER_ONE_ID, "", DateTime.UtcNow.AddDays(10.5), 2);

            Assert.NotNull(order);
        }
    }
}
