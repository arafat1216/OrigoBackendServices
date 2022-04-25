﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HardwareServiceOrder.API
{
    // API-versioning sources:
    // https://github.com/dotnet/aspnet-api-versioning/wiki/API-Documentation#aspnet-core
    // https://dev.to/moesmp/what-every-asp-net-core-web-api-project-needs-part-2-api-versioning-and-swagger-3nfm

    /// <summary>
    ///     Configures the Swagger generation options.
    /// </summary>
    /// <remarks> 
    ///     This allows API versioning to define a Swagger document per API version after the <see cref="IApiVersionDescriptionProvider"/>
    ///     service has been resolved from the service container.
    /// </remarks>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;


        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
        /// </summary>
        /// <param name="provider"> The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents. </param>
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;


        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            // add a swagger document for each discovered API version
            // note: you might choose to skip or document deprecated API versions differently
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
            }
        }

        static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "Hardware Service Order API",
                Version = description.ApiVersion.ToString(),
                Description = "A API for retrieving and ordering repair and aftermarket device-return services.",
                //Contact = new OpenApiContact() { Name = "Bill Mei", Email = "bill.mei@somewhere.com" },
                //License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
            };

            if (description.IsDeprecated)
            {
                info.Description = $"This API version has been deprecated!\n\n{info.Description}";
            }

            return info;
        }
    }
}
