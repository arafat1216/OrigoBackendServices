﻿using CustomerServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests
{
    public class CustomerServicesServicesTests : CustomerServicesBaseTest
    {
        public CustomerServicesServicesTests()
            : base(
                new DbContextOptionsBuilder<CustomerContext>()
                    // ReSharper disable once StringLiteralTypo
                    .UseSqlite("Data Source=sqlitecustomerunittests.db")
                    .Options
            )
        {

        }

        [Fact]
        [Trait("Category", "UnitTest")]
        public async void GetCompanyOne_CheckName()
        {
            // Arrange
            await using var context = new CustomerContext(ContextOptions);
            var customerRepository = new CustomerRepository(context);
            var customerService = new CustomerServices(Mock.Of<ILogger<CustomerServices>>(), customerRepository);

            // Act
            var customer = await customerService.GetCustomerAsync(CUSTOMER_ONE_ID);

            // Assert
            Assert.Equal("COMPANY ONE", customer.CompanyName);
        }
    }
}