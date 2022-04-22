
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

var apiVersion = new ApiVersion(1, 0);
var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets/appsettings.secrets.json", optional: true);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddHealthChecks();
builder.Services.AddControllers().AddDapr();



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
        Title = "Hardware Repair",
        Version = $"v{apiVersion.MajorVersion}"
    });

    var xmlComments = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlComments));
    config.EnableAnnotations();
});


builder.Services.AddApplicationInsightsTelemetry();


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
                $"Hardware Repair Services v{apiVersion.MajorVersion}"));

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