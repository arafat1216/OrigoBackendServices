using Common.Extensions;
using CustomerServices.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Customer.API.IntegrationTests
{
    public static class CustomerWebApplicationFactoryExtensions
    {
        public static HttpClient CreateClientWithDbSetup(this WebApplicationFactory<Startup> factory,
            Action<CustomerContext> configure)
        {
            var client = factory.WithWebHostBuilder(builder => { builder.ConfigureTestDatabase(configure); })
                .CreateClient();
            client.DefaultRequestHeaders.Add("X-Authenticated-UserId", Guid.Empty.SystemUserId().ToString());
            return client;
        }
    }
}
