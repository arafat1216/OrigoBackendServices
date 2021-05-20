using AssetServices;
using AssetServices.Infrastructure;
using AssetServices.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Asset.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
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
                c.SwaggerDoc($"v{_apiVersion.MajorVersion}", new OpenApiInfo { Title = "Asset Management", Version = $"v{_apiVersion.MajorVersion}" });
            });
            services.AddDbContext<AssetsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AssetConnectionString")));
            services.AddScoped<IAssetServices, AssetServices.AssetServices>();
            services.AddScoped<IAssetRepository, AssetRepository>();

            //services.AddHealthChecks()
            //    .AddCheck("self", () => HealthCheckResult.Healthy("Gateway is ok"), tags: new[]{"Origo API Gateway"})
            //    .AddSqlServer(
            //        connectionString: Configuration.GetConnectionString("AssetsDbConnection"),
            //        healthQuery: "SELECT 1;",
            //        name: "sql",
            //        failureStatus: HealthStatus.Degraded,
            //        tags: new string[] { "db", "sql", "sqlserver" }
            //    );

            //services.AddHealthChecksUI().AddInMemoryStorage();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/v{_apiVersion.MajorVersion}/swagger.json", $"Customer Asset Services v{_apiVersion.MajorVersion}"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
