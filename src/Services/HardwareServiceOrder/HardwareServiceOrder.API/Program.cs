using Common.Configuration;
using Common.Utilities;
using Dapr.Client;
using HardwareServiceOrder.API;
using HardwareServiceOrder.API.Extensions;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Resources;

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

// Register the two DI items used by EF to retrieve/assign the API's callerID
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IApiRequesterService, ApiRequesterService>();

builder.Services.AddDbContext<HardwareServiceOrderContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("HardwareServiceOrderConnectionString"), sqlOption =>
    {
        sqlOption.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
        sqlOption.MigrationsAssembly(typeof(HardwareServiceOrderContext).GetTypeInfo().Assembly.GetName().Name);
    }));

//Data protection
builder.Services.AddDataProtection()
        .PersistKeysToDbContext<HardwareServiceOrderContext>()
        .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
        });

builder.Services.AddAutoMapper(
    Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(HardwareServiceOrderServices.Mappings.CustomerSettingsProfile)),
    Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(HardwareServiceOrder.API.Mappings.HardwareServiceOrderProfile)));

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
    // Add custom converters to enable DateOnly support in swagger
    options.UseDateOnlyStringConverters();

    // Add a custom operation filter which sets default values
    options.OperationFilter<SwaggerDefaultValues>();

    // Register global header parameters in the API
    options.OperationFilter<SwaggerGlobalHeaderParameters>();

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

builder.Services.Configure<ServiceProviderConfiguration>(builder.Configuration.GetSection("ServiceProviderConfiguration"));
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<OrigoConfiguration>(builder.Configuration.GetSection("Origo"));
builder.Services.Configure<AssetConfiguration>(builder.Configuration.GetSection("Asset"));

builder.Services.AddScoped<IHardwareServiceOrderService, HardwareServiceOrderService>();
builder.Services.AddScoped<IHardwareServiceOrderRepository, HardwareServiceOrderRepository>();
builder.Services.AddScoped<IProviderFactory, ProviderFactory>();
builder.Services.AddScoped<IStatusHandlerFactory, StatusHandlerFactory>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFlatDictionaryProvider, FlatDictionary>();

builder.Services.AddSingleton(s => new ResourceManager("HardwareServiceOrderServices.Resources.HardwareServiceOrder", Assembly.GetAssembly(typeof(EmailService))));
builder.Services.AddSingleton<IAssetService>(s => new AssetService(s.GetRequiredService<IOptions<AssetConfiguration>>(), DaprClient.CreateInvokeHttpClient("assetservices")));
#endregion Builder


#region App

var app = builder.Build();

// The API version descriptor provider used to enumerate defined API versions.
var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

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