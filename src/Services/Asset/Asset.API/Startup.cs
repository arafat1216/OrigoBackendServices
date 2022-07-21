using System;
using System.Reflection;
using System.Resources;
using Asset.API.Filters;
using AssetServices;
using AssetServices.Email;
using AssetServices.Infrastructure;
using AssetServices.Mappings;
using AssetServices.Models;
using Common.Configuration;
using Common.Logging;
using Common.Utilities;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;

namespace Asset.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private readonly ApiVersion _apiVersion = new(1, 0);

        private IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddDapr();
            services.AddHealthChecks();
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = _apiVersion;
                config.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{_apiVersion.MajorVersion}",
                    new OpenApiInfo {Title = "Asset Management", Version = $"v{_apiVersion.MajorVersion}"});
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(AssetLifecycleProfile)));
            services.AddApplicationInsightsTelemetry();

            services.AddDbContext<AssetsContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("AssetConnectionString"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                }));
            services.AddDbContext<LoggingDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("AssetConnectionString"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                });
            });
            services.AddSingleton(s => new ResourceManager("AssetServices.Resources.Asset", Assembly.GetAssembly(typeof(EmailService))));
            services.AddScoped<IFunctionalEventLogService, FunctionalEventLogService>();
            services.AddScoped<IAssetServices, AssetServices.AssetServices>();
            services.AddScoped<IAssetTestDataService, AssetTestDataService>();
            services.AddScoped<IAssetLifecycleRepository, AssetLifecycleRepository>();
            services.Configure<EmailConfiguration>(Configuration.GetSection("Email"));
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IFlatDictionaryProvider, FlatDictionary>();
            services.AddScoped<ErrorExceptionFilter>();
            
            services.AddHttpClient("emailservices", c => { c.BaseAddress = new Uri("http://emailservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler());

            // Feature flag initialization.
            services.Configure<FeatureFlagConfiguration>(Configuration.GetSection("FeatureFlag"));
            services.AddSingleton<IFeatureDefinitionProvider, ApiFeatureFilter>(
                f => new ApiFeatureFilter(
                    DaprClient.CreateInvokeHttpClient("customerservices"),
                    f.GetRequiredService<IOptions<FeatureFlagConfiguration>>())
            ).AddFeatureManagement();

            VATConfiguration.Initialize(Configuration);
            MinBuyoutConfiguration.Initialize(Configuration);

            services.AddMediatR(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseCloudEvents();
            app.UseHealthChecks("/healthz");
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v{_apiVersion.MajorVersion}/swagger.json",
                $"Customer Asset Services v{_apiVersion.MajorVersion}"));

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");
            });
        }
    }
}