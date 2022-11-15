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
using SubscriptionManagement.IntegrationTests.Helpers;
using SubscriptionManagementServices.Email;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Utilities;

// ReSharper disable StringLiteralTypo
// ReSharper disable once ClassNeverInstantiated.Global

namespace SubscriptionManagement.IntegrationTests;

public class SubscriptionManagementWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class

{
    private readonly DbConnection _dbConnection = new SqliteConnection("Data Source=:memory:");

    public int CUSTOMER_SUBSCRIPTION_PRODUCT_ID => SubscriptionManagementDataSeeding.CUSTOMER_SUBSCRIPTION_PRODUCT_ID;
    public int SUBSCRIPTION_PRODUCT_ID => SubscriptionManagementDataSeeding.SUBSCRIPTION_PRODUCT_ID;
    public int OPERATOR_ACCOUNT_ID => SubscriptionManagementDataSeeding.OPERATOR_ACCOUNT_ID;
    public Guid ORGANIZATION_ID => SubscriptionManagementDataSeeding.ORGANIZATION_ID;
    public Guid ORGANIZATION_TWO_ID => SubscriptionManagementDataSeeding.ORGANIZATION_TWO_ID;
    public string PHONE_NUMBER => SubscriptionManagementDataSeeding.PHONE_NUMBER;
    public int FIRST_OPERATOR_ID => SubscriptionManagementDataSeeding.FIRST_OPERATOR_ID;

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
                SubscriptionManagementDataSeeding.PopulateData(subscriptionManagementContext);
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