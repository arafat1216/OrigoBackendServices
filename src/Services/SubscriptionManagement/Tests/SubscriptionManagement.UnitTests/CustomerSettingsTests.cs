using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
