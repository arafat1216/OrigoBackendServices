using System;
using System.Reflection;
using Common.Logging;
using CustomerServices;
using CustomerServices.Infrastructure;
using CustomerServices.Models;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Customer.API
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
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = _apiVersion;
                config.AssumeDefaultVersionWhenUnspecified = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{_apiVersion.MajorVersion}",
                    new OpenApiInfo {Title = "Customer Management", Version = $"v{_apiVersion.MajorVersion}"});
            });
            services.AddApplicationInsightsTelemetry();
            services.AddDbContext<CustomerContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("CustomerConnectionString"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                }));
            services.AddDbContext<LoggingDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("CustomerConnectionString"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
                }));
            services.AddScoped<IFunctionalEventLogService, FunctionalEventLogService>();
            services.AddScoped<IOrganizationServices, CustomerServices.CustomerServices>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IUserPermissionServices, UserPermissionServices>();
            services.AddScoped<IModuleServices, ModuleServices>();
            services.AddScoped<IDepartmentsServices, DepartmentsServices>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddMediatR(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v{_apiVersion.MajorVersion}/swagger.json",
                $"CustomerServices v{_apiVersion.MajorVersion}"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}