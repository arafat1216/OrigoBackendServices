using AutoMapper;
using Dapr.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Okta.AspNetCore;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Helpers;
using OrigoApiGateway.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using Common.Utilities;

namespace OrigoApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
        }

        private readonly ApiVersion _apiVersion = new(1, 0);

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment WebHostEnvironment { get; set; }

        private readonly string swaggerBasePath = "origoapi";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddDapr();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = _apiVersion;
                config.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddHealthChecks();
                   

            services.AddHealthChecksUI().AddInMemoryStorage();
            var blobConnectionString = Configuration.GetSection("Storage:ConnectionString").Value;
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(blobConnectionString);
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient<IStorageService, StorageService>();
            services.Configure<AssetConfiguration>(Configuration.GetSection("Asset"));
            services.Configure<CustomerConfiguration>(Configuration.GetSection("Customer"));
            services.Configure<PartnerConfiguration>(Configuration.GetSection("Partner"));
            services.Configure<UserConfiguration>(Configuration.GetSection("User"));
            services.Configure<UserPermissionsConfigurations>(Configuration.GetSection("UserPermissions"));
            services.Configure<DepartmentConfiguration>(Configuration.GetSection("Department"));
            services.Configure<ProductCatalogConfiguration>(Configuration.GetSection("ProductCatalog"));
            services.Configure<SubscriptionManagementConfiguration>(Configuration.GetSection("SubscriptionManagement"));
            services.Configure<HardwareServiceOrderConfiguration>(Configuration.GetSection("HardwareServiceOrder"));
            services.Configure<WebshopConfiguration>(Configuration.GetSection("Webshop"));
            services.Configure<FeatureFlagConfiguration>(Configuration.GetSection("FeatureFlag"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
            }).AddOktaWebApi(new OktaWebApiOptions
            {
                OktaDomain = Configuration["Authentication:Okta:OktaDomain"],
                Audience = Configuration["Authentication:Okta:Audience"]
            });

            services.AddAuthorization(options =>
            {
                // One static policy - All users must be authenticated
                options.DefaultPolicy = new AuthorizationPolicyBuilder(OktaDefaults.ApiAuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            });

            //Filters
            services.AddScoped<ErrorExceptionFilter>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddSingleton<IWebshopService>(s => new WebshopService(
                s.GetRequiredService<ILogger<WebshopService>>(),
                s.GetRequiredService<IOptions<WebshopConfiguration>>(),
                DaprClient.CreateInvokeHttpClient("customerservices")
                ));

            services.AddSingleton<IAssetServices>(x => new AssetServices(
                x.GetRequiredService<ILogger<AssetServices>>(),
                DaprClient.CreateInvokeHttpClient("assetservices"),
                x.GetRequiredService<IOptions<AssetConfiguration>>(),
                new UserServices(
                    x.GetRequiredService<ILogger<UserServices>>(),
                    DaprClient.CreateInvokeHttpClient("customerservices"),
                    x.GetRequiredService<IOptions<UserConfiguration>>(),
                    x.GetRequiredService<IMapper>()
                ),
                x.GetRequiredService<IMapper>()
            ));

            services.AddSingleton<IHardwareRepairService>(x => new HardwareRepairService(
               x.GetRequiredService<ILogger<HardwareRepairService>>(),
               DaprClient.CreateInvokeHttpClient("hardwareserviceorderservices"),
               x.GetRequiredService<IOptions<HardwareServiceOrderConfiguration>>()));

            services.AddSingleton<ICustomerServices>(x => new CustomerServices(
                x.GetRequiredService<ILogger<CustomerServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<CustomerConfiguration>>(),
                new AssetServices(
                    x.GetRequiredService<ILogger<AssetServices>>(),
                    DaprClient.CreateInvokeHttpClient("assetservices"),
                    x.GetRequiredService<IOptions<AssetConfiguration>>(),
                    new UserServices(
                            x.GetRequiredService<ILogger<UserServices>>(),
                            DaprClient.CreateInvokeHttpClient("customerservices"),
                            x.GetRequiredService<IOptions<UserConfiguration>>(),
                            x.GetRequiredService<IMapper>()
                    ),
                    x.GetRequiredService<IMapper>()
                ),
                x.GetRequiredService<IMapper>()
            ));

            services.AddSingleton<IPartnerServices>(x => new PartnerServices(
                x.GetRequiredService<ILogger<PartnerServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<PartnerConfiguration>>(),
                x.GetRequiredService<IMapper>()
            ));

            services.AddSingleton<IUserPermissionService>(x => new UserPermissionService(
                x.GetRequiredService<ILogger<UserPermissionService>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<UserPermissionsConfigurations>>(),
                x.GetRequiredService<IMapper>()
            ));

            services.AddSingleton<IFeatureFlagServices>(x => new FeatureFlagServices(
                x.GetRequiredService<ILogger<FeatureFlagServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<FeatureFlagConfiguration>>()
            ));

            services.AddSingleton<IUserServices>(x => new UserServices(
                x.GetRequiredService<ILogger<UserServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<UserConfiguration>>(),
                x.GetRequiredService<IMapper>()
            ));

            services.AddSingleton<IDepartmentsServices>(x => new DepartmentsServices(
                x.GetRequiredService<ILogger<DepartmentsServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<DepartmentConfiguration>>(),
                x.GetRequiredService<IMapper>()
            ));

            services.AddSingleton<IProductCatalogServices>(x => new ProductCatalogServices(
                x.GetRequiredService<ILogger<ProductCatalogServices>>(),
                DaprClient.CreateInvokeHttpClient("productcatalogservices"),
                x.GetRequiredService<IOptions<ProductCatalogConfiguration>>()
            ));


            services.AddSingleton<ISubscriptionManagementService>(x => new SubscriptionManagementService(
                x.GetRequiredService<ILogger<SubscriptionManagementService>>(),
                    x.GetRequiredService<IOptions<SubscriptionManagementConfiguration>>(),
                    new UserServices(
                        x.GetRequiredService<ILogger<UserServices>>(),
                        DaprClient.CreateInvokeHttpClient("customerservices"),
                        x.GetRequiredService<IOptions<UserConfiguration>>(),
                        x.GetRequiredService<IMapper>()
                    ),
                    DaprClient.CreateInvokeHttpClient("subscriptionmanagementservices"),
                    x.GetRequiredService<IMapper>()
                ));


            if (WebHostEnvironment.EnvironmentName == "Development")
            {
                services.AddSingleton<ISeedDatabaseService>(x => new SeedDatabaseService(
                    x.GetRequiredService<ILogger<SeedDatabaseService>>(),
                    new AssetServices(
                        x.GetRequiredService<ILogger<AssetServices>>(),
                        DaprClient.CreateInvokeHttpClient("assetservices"),
                        x.GetRequiredService<IOptions<AssetConfiguration>>(),
                        new UserServices(
                            x.GetRequiredService<ILogger<UserServices>>(),
                            DaprClient.CreateInvokeHttpClient("customerservices"),
                            x.GetRequiredService<IOptions<UserConfiguration>>(),
                            x.GetRequiredService<IMapper>()
                        ),
                        x.GetRequiredService<IMapper>()
                    ),
                    new CustomerServices(
                            x.GetRequiredService<ILogger<CustomerServices>>(),
                            DaprClient.CreateInvokeHttpClient("customerservices"),
                            x.GetRequiredService<IOptions<CustomerConfiguration>>(),
                            new AssetServices(
                                x.GetRequiredService<ILogger<AssetServices>>(),
                                DaprClient.CreateInvokeHttpClient("assetservices"),
                                x.GetRequiredService<IOptions<AssetConfiguration>>(),
                                new UserServices(
                                    x.GetRequiredService<ILogger<UserServices>>(),
                                    DaprClient.CreateInvokeHttpClient("customerservices"),
                                    x.GetRequiredService<IOptions<UserConfiguration>>(),
                                    x.GetRequiredService<IMapper>()
                                ),
                                x.GetRequiredService<IMapper>()
                            ),
                            x.GetRequiredService<IMapper>()
                        )
                    )
                );
            }
            else
            {
                services.AddMvc(c =>
                    c.Conventions.Add(new HideTestControllersConvention())
                );
            }

            services.AddSwaggerGen(options =>
            {
                // Include XML documentation used for Swagger enrichment
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.EnableAnnotations();
                options.SwaggerDoc($"v{_apiVersion.MajorVersion}", new OpenApiInfo { Title = "Origo API Gateway", Version = $"v{_apiVersion.MajorVersion}" });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
            });

            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IUserPermissionService userPermissionService)
        {
            app.UseExceptionHandler(err => err.UseCustomErrors(env));

            app.UseHealthChecks("/healthz");

            app.UseRouting();

            app.UseAuthentication().Use(async (context, next) =>
            {
                var authenticateResult = await context.AuthenticateAsync();
                if (authenticateResult.Succeeded && authenticateResult.Principal != null)
                {
                    var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                    var userSub = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(userSub))
                    {
                        var userPermissionIdentity = await userPermissionService.GetUserPermissionsIdentityAsync(userSub, userEmail, CancellationToken.None);
                        context.User.AddIdentity(userPermissionIdentity);
                    }
                }

                await next();
            });

            app.UseAuthorization();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = swaggerBasePath + "/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = $"{swaggerBasePath}/swagger";
                c.SwaggerEndpoint($"/{swaggerBasePath}/swagger/v{_apiVersion.MajorVersion}/swagger.json", $"Origo API Gateway v{_apiVersion.MajorVersion}");
                c.OAuthClientId("0oa6ay5cwySaVtkNR0i7");
                c.OAuthClientSecret("N5zA6qXkGINo0CbGLEoxelIySPXBc3dsdpTvKr1v");
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");
            });
        }
    }
}
