using Dapr.Client;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
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

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration["Authentication:AuthConfig:Authority"];
                options.Audience = Configuration["Authentication:AuthConfig:Audience"];
                options.RequireHttpsMetadata = !WebHostEnvironment.IsDevelopment();
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IAssetServices>(x => new AssetServices(x.GetRequiredService<ILogger<AssetServices>>(),
                DaprClient.CreateInvokeHttpClient("assetservices"),
                x.GetRequiredService<IOptions<AssetConfiguration>>(), new UserServices(x.GetRequiredService<ILogger<UserServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<UserConfiguration>>())));

            services.AddSingleton<ICustomerServices>(x => new CustomerServices(x.GetRequiredService<ILogger<CustomerServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<CustomerConfiguration>>()));

            services.AddSingleton<IUserServices>(x => new UserServices(x.GetRequiredService<ILogger<UserServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<UserConfiguration>>()));

            services.AddSingleton<IModuleServices>(x => new ModuleServices(x.GetRequiredService<ILogger<ModuleServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<ModuleConfiguration>>(),
                new CustomerServices(x.GetRequiredService<ILogger<CustomerServices>>(),
                DaprClient.CreateInvokeHttpClient("customerservices"),
                x.GetRequiredService<IOptions<CustomerConfiguration>>())));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"v{_apiVersion.MajorVersion}", new OpenApiInfo { Title = "Origo API Gateway", Version = $"v{_apiVersion.MajorVersion}" });
            });

            services.AddApplicationInsightsTelemetry();

            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ClockSkew = TimeSpan.FromMinutes(5),
            //        RequireSignedTokens = true,
            //        RequireExpirationTime = true,
            //        ValidateAudience = true,
            //        ValidAudience = Configuration["Authentication:AuthConfig:Audience"],
            //        ValidateIssuer = true,
            //        ValidIssuer = Configuration["Authentication:AuthConfig:Issuer"],
            //        ValidateIssuerSigningKey = true,
            //        //IssuerSigningKey = openidconfig.SigningKeys.FirstOrDefault(),
            //        ValidateLifetime = true,
            //    };

            //    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            //    {
            //        OnTokenValidated = (context) =>
            //        {
            //            // Add access token to user's claim
            //            if (context.SecurityToken is JwtSecurityToken token && context.Principal.Identity is ClaimsIdentity identity && !identity.HasClaim(c => c.Type == "access_token"))
            //            {
            //                identity.AddClaim(new Claim("access_token", token.RawData));
            //            }
            //            return Task.CompletedTask;
            //        },
            //        OnAuthenticationFailed = (context) =>
            //        {
            //            return Task.CompletedTask;
            //        },
            //        OnForbidden = (context) =>
            //        {
            //            return Task.CompletedTask;
            //        }
            //    };
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseExceptionHandler(err => err.UseCustomErrors(env));

            app.UseHealthChecks("/healthz");

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = swaggerBasePath + "/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = $"{swaggerBasePath}/swagger";
                c.SwaggerEndpoint($"/{swaggerBasePath}/swagger/v{_apiVersion.MajorVersion}/swagger.json", $"Origo API Gateway v{_apiVersion.MajorVersion}");
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
