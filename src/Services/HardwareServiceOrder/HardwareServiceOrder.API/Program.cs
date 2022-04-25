using HardwareServiceOrder.API;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

#region Sources & References

/*
 * API-versioning sources:
 * https://github.com/dotnet/aspnet-api-versioning/wiki/API-Documentation#aspnet-core
 * https://dev.to/moesmp/what-every-asp-net-core-web-api-project-needs-part-2-api-versioning-and-swagger-3nfm
 */

#endregion

var apiVersion = new ApiVersion(1, 0);


#region Builder

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddHealthChecks();
builder.Services.AddControllers()
                .AddDapr();

builder.Services.AddDbContext<HardwareServiceOrderContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("HardwareServiceOrderConnectionString"), sqlOption =>
    {
        sqlOption.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
        sqlOption.MigrationsAssembly(typeof(HardwareServiceOrderContext).GetTypeInfo().Assembly.GetName().Name);
    }));

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(HardwareServiceOrderServices.Mappings.CustomerSettingsProfile)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);


#region Services configuration: API Versioning w/Swagger

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configuration for the NuGet package: 'Microsoft.AspNetCore.Mvc.Versioning'
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = apiVersion;
    options.AssumeDefaultVersionWhenUnspecified = true;

    // Adds the "api-supported-versions" and "api-deprecated-versions" to the HTTP header response.
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

builder.Services.AddSwaggerGen(options =>
{
    // Add a custom operation filter which sets default values
    options.OperationFilter<SwaggerDefaultValues>();

    options.EnableAnnotations();

    // Retrieve all assemblies containing XML comments
    var xmlFiles = new[]
{
        // Include the current assembly (this project)
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",

        /* Include additional assemblies from other projects below this.
         * 
         * Example:
         * $"{Assembly.GetAssembly(typeof(Translation))?.GetName().Name}.xml"
         */
    };

    // Loop over and include each of the generated XML documentation files so it is added to Swagger.
    foreach (var xmlFile in xmlFiles)
    {
        var xmlCommentFile = xmlFile;
        var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
        if (File.Exists(xmlCommentsFullPath))
        {
            // Integrate the XML comments
            options.IncludeXmlComments(xmlCommentsFullPath);
        }
    }

});

#endregion Services configuration: API Versioning w/Swagger

builder.Services.AddMvc();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddScoped<IHardwareServiceOrderService, HardwareServiceOrderService>();
builder.Services.AddScoped<IHardwareServiceOrderRepository, HardwareServiceOrderRepository>();

#endregion Builder


#region App

var app = builder.Build();

// The API version descriptor provider used to enumerate defined API versions.
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();

app.UseSwaggerUI(options =>
{
    // Build a swagger endpoint for each discovered API version
    foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
    }
});

app.UseHealthChecks("/healthz");

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/healthz");
    endpoints.MapControllers();
});

app.Run();

#endregion App