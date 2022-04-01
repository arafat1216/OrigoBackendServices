using System;
using System.Reflection;
using AssetServices;
using AssetServices.Infrastructure;
using AssetServices.Mappings;
using AssetServices.Models;
using Common.Logging;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddScoped<IFunctionalEventLogService, FunctionalEventLogService>();
            services.AddScoped<IAssetServices, AssetServices.AssetServices>();
            services.AddScoped<IAssetTestDataService, AssetTestDataService>();
            services.AddScoped<IAssetLifecycleRepository, AssetLifecycleRepository>();
            services.AddMediatR(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

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