using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Infrastructure;
using System.Reflection;

var apiVersion = new ApiVersion(1, 0);

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//https://andrewlock.net/exploring-dotnet-6-part-1-looking-inside-configurationmanager-in-dotnet-6/


builder.Services.AddControllers().AddDapr();

builder.Services.AddDbContext<SubscriptionManagmentContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("SubscriptionManagmentConnectionString"), sqlOption =>
    {
        sqlOption.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
        sqlOption.MigrationsAssembly(typeof(SubscriptionManagmentContext).GetTypeInfo().Assembly.GetName().Name);
    }));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = apiVersion;
    config.AssumeDefaultVersionWhenUnspecified = true;
});

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc($"v{apiVersion.MajorVersion}", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Subscription Management",
        Version = $"v{apiVersion.MajorVersion}"
    });
});


builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v{apiVersion.MajorVersion}/swagger.json",
                $"Subscription Management Services v{apiVersion.MajorVersion}"));

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
