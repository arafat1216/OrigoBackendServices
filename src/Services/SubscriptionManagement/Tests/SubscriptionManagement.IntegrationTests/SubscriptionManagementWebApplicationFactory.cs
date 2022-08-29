using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Common.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SubscriptionManagement.IntegrationTests.Controllers;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.Types;
using SubscriptionManagementServices.Utilities;

// ReSharper disable StringLiteralTypo
// ReSharper disable once ClassNeverInstantiated.Global

namespace SubscriptionManagement.IntegrationTests;

public class SubscriptionManagementWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class

{
    private readonly DbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");

    public readonly int CUSTOMER_SUBSCRIPTION_PRODUCT_ID = 200;
    public readonly int SUBSCRIPTION_PRODUCT_ID = 300;
    public readonly int OPERATOR_ACCOUNT_ID = 100;
    public readonly Guid ORGANIZATION_ID = Guid.Parse("7adbd9fa-97d1-11ec-8500-00155d64bd3d");
    public int FIRST_OPERATOR_ID;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            ReplaceSubscriptionManagementDbContext<SubscriptionManagementContext>(services);
            ReplaceSubscriptionManagementDbContext<LoggingDbContext>(services);
            services.AddSingleton<IDateTimeProvider, MockDateTimeProvider>();


            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var subscriptionManagementContext = scope.ServiceProvider.GetRequiredService<SubscriptionManagementContext>();
            subscriptionManagementContext.Database.EnsureCreated();

            var emailServiceMock = new Mock<IEmailService>();
            emailServiceMock.Setup(m => m.SendAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<object>(), It.IsAny<Dictionary<string, string>>()));
            services.AddScoped(s => emailServiceMock.Object);

            try
            {
                var firstOperator = subscriptionManagementContext.Operators.FirstOrDefault();
                FIRST_OPERATOR_ID = firstOperator!.Id;
                var subscriptionProduct = new SubscriptionProduct(SUBSCRIPTION_PRODUCT_ID, "TOTAL BEDRIFT", firstOperator!, new List<DataPackage>{new DataPackage("20GB", Guid.Empty), new DataPackage("30GB", Guid.Empty)}, Guid.Empty);
                subscriptionManagementContext.SubscriptionProducts.AddRangeAsync(
                    new List<SubscriptionProduct>
                    {
                        subscriptionProduct
                    });
                var customerSubscriptionProduct = new CustomerSubscriptionProduct(CUSTOMER_SUBSCRIPTION_PRODUCT_ID, subscriptionProduct, Guid.Empty, (IList<DataPackage>?)subscriptionProduct.DataPackages);
                var customerOperatorAccount = new CustomerOperatorAccount(OPERATOR_ACCOUNT_ID, ORGANIZATION_ID, "1111111111111", "435543", "CC1", firstOperator!.Id, Guid.Empty);
                subscriptionManagementContext.CustomerOperatorAccounts.Add(customerOperatorAccount);

                var standardPrivateProduct = new CustomerStandardPrivateSubscriptionProduct("PrivateDataPackage","PrivateSubscription",Guid.Empty);
                subscriptionManagementContext.CustomerStandardPrivateSubscriptionProducts.Add(standardPrivateProduct);

                var customerOperatorSettings = new List<CustomerOperatorSettings>
                {
                    new (firstOperator,
                        new List<CustomerSubscriptionProduct> { customerSubscriptionProduct },
                        new List<CustomerOperatorAccount> { customerOperatorAccount },
                        standardPrivateProduct
                        )
                };
                var customerReferenceFields = new List<CustomerReferenceField>
                {
                    new CustomerReferenceField("URef1", CustomerReferenceTypes.User, Guid.Empty),
                    new CustomerReferenceField("URef2", CustomerReferenceTypes.User, Guid.Empty),
                    new CustomerReferenceField("AccURef1", CustomerReferenceTypes.Account, Guid.Empty)
                };
                subscriptionManagementContext.CustomerSettings.Add(new CustomerSettings(ORGANIZATION_ID,
                    customerOperatorSettings, customerReferenceFields));
                subscriptionManagementContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });
        base.ConfigureWebHost(builder);
    }

    private void ReplaceSubscriptionManagementDbContext<T>(IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        _dbConnection.Open();
        services.AddDbContext<T>(options =>
        {
            options.UseSqlite(_dbConnection);
            options.EnableSensitiveDataLogging();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _dbConnection.Dispose();
    }
   
}