using Common.Utilities;
using Dapr.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Models;
using Okta.AspNetCore;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Extensions;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Helpers;
using OrigoApiGateway.Services;
using System.Reflection;
using System.Security.Claims;
using Common.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace OrigoApiGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });
            _logger = loggerFactory.CreateLogger<Startup>();
        }

        private readonly ApiVersion _apiVersion = new(1, 0);

        private IConfiguration Configuration { get; }

        private IWebHostEnvironment WebHostEnvironment { get; set; }

        private readonly string swaggerBasePath = "origoapi";
        private readonly ILogger<Startup> _logger;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddDapr();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = _apiVersion;
                config.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddHealthChecks();
            services.AddHealthChecksUI().AddInMemoryStorage();
            services.AddApplicationInsightsTelemetry();

            var blobConnectionString = Configuration.GetSection("Storage:ConnectionString").Value;
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(blobConnectionString);
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddHttpContextAccessor();

            AddServiceConfigurations(services);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultForbidScheme = OktaDefaults.ApiAuthenticationScheme;
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

            services.AddScoped<ErrorExceptionFilter>();
            
            services.AddTransient<IStorageService, StorageService>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            services.AddTransient<AddCallerIdHeaderHandler>();

            AddHttpClientsToFactory(services);

            services.AddSingleton<IWebshopService, WebshopService>();
            services.AddSingleton<IAssetServices, Services.AssetServices>();
            services.AddSingleton<ICustomerServices, CustomerServices>();
            services.AddSingleton<IHardwareServiceOrderService, HardwareServiceOrderService>();
            services.AddSingleton<IPartnerServices, PartnerServices>();
            services.AddSingleton<IUserPermissionService, UserPermissionService>();
            services.AddSingleton<IFeatureFlagServices, FeatureFlagServices>();
            services.AddSingleton<IUserServices, UserServices>();
            services.AddSingleton<IDepartmentsServices, DepartmentsServices>();
            services.AddSingleton<IProductCatalogServices, ProductCatalogServices>();
            services.AddSingleton<ISubscriptionManagementService, SubscriptionManagementService>();

            if (WebHostEnvironment.EnvironmentName == "Development")
            {
                services.AddSingleton<ISeedDatabaseService, SeedDatabaseService>();
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

                // Add custom converters to enable DateOnly support in swagger
                options.UseDateOnlyStringConverters();

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
                    { securityScheme, new string[] { } }
                });
            });
        }

        private void AddHttpClientsToFactory(IServiceCollection services)
        {
            services.AddHttpClient("assetservices", c => { c.BaseAddress = new Uri("http://assetservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler())
                .AddHttpMessageHandler<AddCallerIdHeaderHandler>();
            services.AddHttpClient("customerservices", c => { c.BaseAddress = new Uri("http://customerservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler())
                .AddHttpMessageHandler<AddCallerIdHeaderHandler>();
            var techstepCoreConfiguration = Configuration.GetSection("TechstepCore:Customers");
            var baseUrl = techstepCoreConfiguration.GetValue(typeof(string), "BaseUrl");
            var apiKey = techstepCoreConfiguration.GetValue(typeof(string), "SecretKey").ToString();
            if (baseUrl != null)
            {
                var baseUrlString = baseUrl.ToString();
                if (!string.IsNullOrEmpty(baseUrlString))
                {
                    services.AddHttpClient("techstep-core-customers-no", c => 
                    { 
                        c.BaseAddress = new Uri(baseUrlString); 
                        c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                        c.DefaultRequestHeaders.Add("CountryCode", "NO");
                    });
                    services.AddHttpClient("techstep-core-customers-se", c =>
                    {
                        c.BaseAddress = new Uri(baseUrlString);
                        c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                        c.DefaultRequestHeaders.Add("CountryCode", "SE");
                    });

                }
            }
            services.AddHttpClient("userpermissionservices", c => { c.BaseAddress = new Uri("http://customerservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler())
                .AddHttpMessageHandler<AddCallerIdHeaderHandler>();
            services.AddHttpClient("hardwareserviceorderservices", c => { c.BaseAddress = new Uri("http://hardwareserviceorderservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler())
                .AddHttpMessageHandler<AddCallerIdHeaderHandler>();
            services.AddHttpClient("productcatalogservices", c => { c.BaseAddress = new Uri("http://productcatalogservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler())
                .AddHttpMessageHandler<AddCallerIdHeaderHandler>();
            services.AddHttpClient("subscriptionmanagementservices", c => { c.BaseAddress = new Uri("http://subscriptionmanagementservices"); })
                .AddHttpMessageHandler(() => new InvocationHandler())
                .AddHttpMessageHandler<AddCallerIdHeaderHandler>();
        }

        private void AddServiceConfigurations(IServiceCollection services)
        {
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
            services.Configure<TechstepCoreWebhookConfiguration>(Configuration.GetSection("TechstepCoreWebhook"));
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
                        if (userPermissionIdentity.Claims.IsNullOrEmpty())
                        {
                            await context.ForbidAsync();
                            return;
                        }
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
