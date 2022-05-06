using CustomerServices.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;

namespace Customer.API.IntegrationTests
{
    public static class CustomerWebApplicationFactoryExtensions
    {
        public static HttpClient CreateClientWithDbSetup(this WebApplicationFactory<Startup> factory,
            Action<CustomerContext> configure)
        {
            var client = factory.WithWebHostBuilder(builder => { builder.ConfigureTestDatabase(configure); })
                .CreateClient();

            return client;
        }
    }
}
