using System;
using System.Net.Http;
using Asset.API;
using AssetServices.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Asset.IntegrationTests;

public static class WebApplicationFactoryExtensions
{
    public static HttpClient CreateClientWithDbSetup(this WebApplicationFactory<Startup> factory,
        Action<AssetsContext> configure)
    {
        var client = factory.WithWebHostBuilder(builder => { builder.ConfigureTestDatabase(configure); })
            .CreateClient();

        return client;
    }
}