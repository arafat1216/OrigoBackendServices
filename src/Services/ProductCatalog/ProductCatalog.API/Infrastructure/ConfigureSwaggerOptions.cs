using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProductCatalog.API.Infrastructure
{
    // API-versioning sources:
    // https://github.com/dotnet/aspnet-api-versioning/wiki/API-Documentation#aspnet-core
    // https://dev.to/moesmp/what-every-asp-net-core-web-api-project-needs-part-2-api-versioning-and-swagger-3nfm

    /// <summary>
    ///     Configures the Swagger generation options.
    /// </summary>
    /// <remarks> This allows API versioning to define a Swagger document per API version after the <see cref="IApiVersionDescriptionProvider"/>
    ///     service has been resolved from the service container. </remarks>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;


        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider"> The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents. </param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;


        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // Adds a swagger document for each discovered API version.
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }


        private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = $"Product Catalog API",
                Version = description.ApiVersion.ToString(),
                Description = @"Provides services for adding products and modules to organizations.     asd asdasd
                                asd asd
                                        asd asd",
            };

            if (description.IsDeprecated)
                info.Description = @$"This API version has been deprecated.\n\n{info.Description}";

            return info;
        }
    }
}
