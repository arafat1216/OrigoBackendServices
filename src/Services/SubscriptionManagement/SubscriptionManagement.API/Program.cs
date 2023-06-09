using Common.Logging;
using Common.Utilities;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using SubscriptionManagement.API.Filters;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Email;
using SubscriptionManagementServices.Email.Configuration;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.Utilities;
using System.Reflection;
using System.Resources;

var apiVersion = new ApiVersion(1, 0);

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//https://andrewlock.net/exploring-dotnet-6-part-1-looking-inside-configurationmanager-in-dotnet-6/

builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddDapr();

builder.Services.AddDbContext<SubscriptionManagementContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SubscriptionManagementConnectionString"), sqlOption =>
    {
        sqlOption.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
        sqlOption.MigrationsAssembly(typeof(SubscriptionManagementContext).GetTypeInfo().Assembly.GetName().Name);
    }));

builder.Services.AddDbContext<LoggingDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SubscriptionManagementConnectionString"), sqlOptions =>
    {
        sqlOptions.MigrationsAssembly(typeof(SubscriptionManagementContext).GetTypeInfo().Assembly.GetName().Name);
        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
    }));


builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(SubscriptionManagementServices.Mappings.CustomerSubscriptionProductProfile)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = apiVersion;
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddMvc();

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc($"v{apiVersion.MajorVersion}", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Subscription Management",
        Version = $"v{apiVersion.MajorVersion}"
    });

    var xmlComments = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlComments));
    config.EnableAnnotations();
});


builder.Services.AddApplicationInsightsTelemetry();
builder.Services.Configure<TransferSubscriptionDateConfiguration>(builder.Configuration.GetSection("TransferSubscriptionOrderConfiguration"));
builder.Services.Configure<EmailConfigurationSubscriptionManagement>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IFunctionalEventLogService, FunctionalEventLogService>();
builder.Services.AddScoped<ISubscriptionManagementService, SubscriptionManagementService>();
builder.Services.AddScoped<ISubscriptionManagementRepository<ISubscriptionOrder>, SubscriptionManagementRepository<ISubscriptionOrder>>();
builder.Services.AddScoped<IOperatorService, OperatorService>();
builder.Services.AddScoped<IOperatorRepository, OperatorRepository>();
builder.Services.AddScoped<ICustomerSettingsService, CustomerSettingsService>();
builder.Services.AddScoped<ICustomerSettingsRepository, CustomerSettingsRepository>();
builder.Services.AddScoped<ErrorExceptionFilter>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFlatDictionaryProvider, FlatDictionary>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddMediatR(typeof(SubscriptionManagementContext));
builder.Services.AddSingleton(s => new ResourceManager("SubscriptionManagementServices.Resources.SubscriptionManagement", Assembly.GetAssembly(typeof(EmailService))));
builder.Services.AddFeatureManagement();

builder.Services.AddHttpClient("emailservices", c => { c.BaseAddress = new Uri("http://emailnotificationservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler());



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v{apiVersion.MajorVersion}/swagger.json",
                $"Subscription Management Services v{apiVersion.MajorVersion}"));

}
app.UseHealthChecks("/healthz");

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/healthz");
    endpoints.MapControllers();
});

app.Run();