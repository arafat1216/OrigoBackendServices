using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using Dapr.Client;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Okta.AspNetCore;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Helpers;
using OrigoApiGateway.Services;

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

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = _apiVersion;
                config.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("Gateway is ok"), tags: new[] { "Origo API Gateway" });

            services.AddHealthChecksUI().AddInMemoryStorage();

            services.Configure<AssetConfiguration>(Configuration.GetSection("Asset"));
            services.Configure<CustomerConfiguration>(Configuration.GetSection("Customer"));
            services.Configure<UserConfiguration>(Configuration.GetSection("User"));
            services.Configure<ModuleConfiguration>(Configuration.GetSection("Module"));
            services.Configure<DepartmentConfiguration>(Configuration.GetSection("Department"));
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
            //    options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
            //    options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
            //}).AddOktaWebApi(new OktaWebApiOptions
            //{
            //    OktaDomain = "https://techstepportal.okta-emea.com",// "https://origoidp.mytos.no", // Configuration["Authentication:Okta:OktaDomain"],
            //    //AuthorizationServerId = "aus4hxjo6b4FH7i3s0i7",// Configuration["Authentication:Okta:AuthorizationServerId"],
            //    Audience = "0oa4fetdd1BkZ0PcW0i7"
            //});

            //services.AddAuthorization(options =>
            //{
            //    // One static policy - All users must be authenticated
            //    options.DefaultPolicy = new AuthorizationPolicyBuilder(OktaDefaults.ApiAuthenticationScheme)
            //        .RequireAuthenticatedUser()
            //        .Build();
            //});

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddSingleton<IAssetServices>(x => new AssetServices(x.GetRequiredService<ILogger<AssetServices>>(),
                DaprClient.CreateInvokeHttpClient("assetservices"),
                x.GetRequiredService<IOptions<AssetConfiguration>>(), new UserServices(x.GetRequiredService<ILogger<UserServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<UserConfiguration>>())));

            services.AddSingleton<ICustomerServices>(x => new CustomerServices(x.GetRequiredService<ILogger<CustomerServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<CustomerConfiguration>>(), new AssetServices(x.GetRequiredService<ILogger<AssetServices>>(),
                DaprClient.CreateInvokeHttpClient("assetservices"),
                x.GetRequiredService<IOptions<AssetConfiguration>>(), new UserServices(x.GetRequiredService<ILogger<UserServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<UserConfiguration>>()))));

            services.AddSingleton<IUserPermissionService>(x => new UserPermissionService(DaprClient.CreateInvokeHttpClient("customerservices"), x.GetRequiredService<IOptions<UserConfiguration>>()));

            services.AddSingleton<IUserServices>(x => new UserServices(x.GetRequiredService<ILogger<UserServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<UserConfiguration>>()));

            services.AddSingleton<IModuleServices>(x => new ModuleServices(x.GetRequiredService<ILogger<ModuleServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<ModuleConfiguration>>(),
                new CustomerServices(x.GetRequiredService<ILogger<CustomerServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<CustomerConfiguration>>(), new AssetServices(x.GetRequiredService<ILogger<AssetServices>>(),
                DaprClient.CreateInvokeHttpClient("assetservices"),
                x.GetRequiredService<IOptions<AssetConfiguration>>(), new UserServices(x.GetRequiredService<ILogger<UserServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<UserConfiguration>>())))));

            services.AddSingleton<IDepartmentsServices>(x => new DepartmentsServices(x.GetRequiredService<ILogger<DepartmentsServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"), 
                x.GetRequiredService<IOptions<DepartmentConfiguration>>()));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{_apiVersion.MajorVersion}", new OpenApiInfo { Title = "Origo API Gateway", Version = $"v{_apiVersion.MajorVersion}" });
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
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            //app.UseAuthentication().Use(async (context, next) =>
            //{
            //    var authenticateResult = await context.AuthenticateAsync();
            //    if (authenticateResult.Succeeded && authenticateResult.Principal != null)
            //    {
            //        var userEmail = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            //        var userSub = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            //        if (!string.IsNullOrEmpty(userEmail) && !string.IsNullOrEmpty(userSub))
            //        {
            //            var userPermissionIdentity = await userPermissionService.GetUserPermissionsIdentityAsync(userSub, userEmail, CancellationToken.None);
            //            context.User.AddIdentity(userPermissionIdentity);
            //        }
            //    }

            //    await next();
            //});

            //app.UseAuthorization();

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
                endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI();
            });
        }
    }
}
