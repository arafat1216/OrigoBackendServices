using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using ProductCatalog.API.Infrastructure;
using ProductCatalog.Common.Generic;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

#region Sources & References

/*
 * API-versioning sources:
 * https://github.com/dotnet/aspnet-api-versioning/wiki/API-Documentation#aspnet-core
 * https://dev.to/moesmp/what-every-asp-net-core-web-api-project-needs-part-2-api-versioning-and-swagger-3nfm
 */

#endregion


#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true);

builder.Configuration.AddUserSecrets<Program>(optional: true);

// Configuration for the NuGet package: 'Microsoft.AspNetCore.Mvc.Versioning'
builder.Services.AddApiVersioning(options =>
{
    // Adds the "api-supported-versions" and "api-deprecated-versions" to the HTTP header.
    options.ReportApiVersions = true;
});


// Configuration for the NuGet package: 'Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer'
builder.Services.AddVersionedApiExplorer(options =>
{
    // Add the versioned API explorer, which also adds the 'IApiVersionDescriptionProvider'-service. The specified value formats the version as "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";

    options.SubstituteApiVersionInUrl = true;
});


// Options-generator for Swagger that's required when using API versioning
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();


// Swagger configuration. Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();

    options.EnableAnnotations();


    var xmlFiles = new[]
    {
        // Include the current assembly (this project)
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",

        // Include from another project
        $"{Assembly.GetAssembly(typeof(Translation))?.GetName().Name}.xml"
    };

    // Loop over and include each of the generated XML documentation files so it is added to Swagger.
    foreach (var xmlFile in xmlFiles)
    {
        var xmlCommentFile = xmlFile;
        var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
        if (File.Exists(xmlCommentsFullPath))
        {
            options.IncludeXmlComments(xmlCommentsFullPath);
        }
    }
});


builder.Services.AddControllers();
builder.Services.AddRouting(options =>
{
    // Enforce lowercase URLs / routes
    options.LowercaseUrls = true;
});

#endregion


#region App

var app = builder.Build();
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Required to make swagger auto-detect and populate the versioned endpoints
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

#endregion