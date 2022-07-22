using Common.Logging;
using Common.Utilities;
using Customer.API.Filters;
using CustomerServices;
using CustomerServices.Email;
using CustomerServices.Infrastructure;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Mappings;
using CustomerServices.Models;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using Common.Infrastructure;
using Common.Utilities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Resources;

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

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddHealthChecks();
            services.AddControllers().AddDapr();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = _apiVersion;
                config.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{_apiVersion.MajorVersion}",
                    new OpenApiInfo { Title = "Customer Management", Version = $"v{_apiVersion.MajorVersion}" });

                c.EnableAnnotations();

                // Setup for multiple XML documentation files (for referenced projects)
                var xmlFiles = new[]
                {
                    // Include the current assembly (this project)
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml",

                    // Include from another project below here
                    // Example: $"{Assembly.GetAssembly(typeof(Translation))?.GetName().Name}.xml"
                };

                // Loop over and include each of the generated XML documentation files so it is added to Swagger.
                foreach (var xmlCommentFile in xmlFiles)
                {
                    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                    if (File.Exists(xmlCommentsFullPath))
                    {
                        c.IncludeXmlComments(xmlCommentsFullPath);
                    }
                }
                c.OperationFilter<AuthenticatedUserHeaderFilter>();
            });

            services.AddApplicationInsightsTelemetry();
            // Register the two DI items used by EF to retrieve/assign the API's callerID
            services.AddHttpContextAccessor();
            services.AddTransient<IApiRequesterService, ApiRequesterService>();
            services.AddDbContext<CustomerContext>(
                    options =>
                    {
                        options.UseSqlServer(Configuration.GetConnectionString("CustomerConnectionString"),
                            sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                                sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
                            });
                        //options.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
                    }
            );
            services.AddDbContext<LoggingDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("CustomerConnectionString"), sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                    //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
                    sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
                }));
            services.AddAutoMapper(Assembly.GetExecutingAssembly(), Assembly.GetAssembly(typeof(UserDTOProfile)));
            services.AddSingleton(s => new ResourceManager("CustomerServices.Resources.Customer", Assembly.GetAssembly(typeof(EmailService))));
            services.Configure<OktaConfiguration>(Configuration.GetSection("Okta"));
            services.Configure<WebshopConfiguration>(Configuration.GetSection("Webshop"));
            services.AddScoped<IFunctionalEventLogService, FunctionalEventLogService>();
            services.AddScoped<IOrganizationServices, OrganizationServices>();
            services.AddScoped<IPartnerServices, PartnerServices>();
            services.AddScoped<IUserServices, UserServices>();
            services.AddScoped<IOktaServices, OktaServices>();
            services.AddScoped<IUserPermissionServices, UserPermissionServices>();
            services.AddScoped<IDepartmentsServices, DepartmentsServices>();
            services.AddScoped<IOrganizationTestDataService, OrganizationTestDataService>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IWebshopService, WebshopService>();
            services.AddScoped<IFeatureFlagServices, FeatureFlagServices>();
            services.AddScoped<ErrorExceptionFilter>();
            
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IFlatDictionaryProvider, FlatDictionary>();


            services.AddHttpClient("emailservices", c => { c.BaseAddress = new Uri("http://emailservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler());

            services.AddMediatR(typeof(Startup));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseCloudEvents();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v{_apiVersion.MajorVersion}/swagger.json",
                $"CustomerServices v{_apiVersion.MajorVersion}"));

            app.UseHealthChecks("/healthz");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapControllers();
            });
        }
    }
}