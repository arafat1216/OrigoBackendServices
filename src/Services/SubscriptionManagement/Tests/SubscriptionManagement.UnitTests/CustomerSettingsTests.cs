using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SubscriptionManagement.UnitTests
{
    public class CustomerSettingsTests : SubscriptionManagementServiceBaseTests
    {
        private SubscriptionManagementContext _subscriptionManagementContext;
        private ICustomerSettingsRepository _customerSettingsRepository;
        private ICustomerSettingsService _customerSettingsService;

        public CustomerSettingsTests() : base(
                new DbContextOptionsBuilder<SubscriptionManagementContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitesubscriptionmanagementcustomersettingsunittests.db")
                    .Options
            )
        {
            _subscriptionManagementContext = new SubscriptionManagementContext(ContextOptions);
            _customerSettingsRepository = new CustomerSettingsRepository(_subscriptionManagementContext);
            _customerSettingsService = new CustomerSettingsService(_customerSettingsRepository);
        }

        //[Fact]
        [Trait("Category", "UnitTest")]
        public async Task GetAllOperatorAccountsForCustomer_Check_Total()
        {
            var customerReferenceFields = await _customerSettingsService.GetCustomerReferenceFieldsAsync(CUSTOMER_ONE_ID);
            Assert.Equal(2, customerReferenceFields.Count);
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task AddCustomerOperator()
        {
            await _customerSettingsService.AddOperatorsForCustomerAsync(CUSTOMER_ONE_ID, new List<int> { 1, 2 });
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(2, _subscriptionManagementContext.CustomerOperatorSettings.Count());
        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async Task DeleteCustomerOperator()
        {
            await _customerSettingsService.AddOperatorsForCustomerAsync(CUSTOMER_ONE_ID, new List<int> { 1, 2 });
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(2, _subscriptionManagementContext.CustomerOperatorSettings.Count());

            await _customerSettingsService.DeleteOperatorForCustomerAsync(CUSTOMER_ONE_ID, 1);
            Assert.Equal(1, _subscriptionManagementContext.CustomerSettings.Count());
            Assert.Equal(1, _subscriptionManagementContext.CustomerOperatorSettings.Count());
        }
    }
}
