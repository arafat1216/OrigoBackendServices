using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OrigoAssetServices.Models;
using OrigoAssetServices.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrigoAssetServices
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment WebHostEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<AssetsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("AssetsDbConnection")));

            services.AddScoped<IAssetServices, AssetServices>();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
