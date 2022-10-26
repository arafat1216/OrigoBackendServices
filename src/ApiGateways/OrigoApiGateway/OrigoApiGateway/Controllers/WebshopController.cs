using Common.Enums;
using Common.Exceptions;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class WebshopController : ControllerBase
    {
        private readonly IWebshopService WebshopService;
        private ILogger<WebshopController> Logger { get; }
        private readonly WebshopConfiguration WebshopConfiguration;
        private IUserServices UserServices { get; }

        public WebshopController(
            IUserServices userServices,
            IWebshopService webshopService, 
            IOptions<WebshopConfiguration> options, 
            ILogger<WebshopController> logger)
        {
            UserServices = userServices;
            WebshopConfiguration = options.Value;
            WebshopService = webshopService;
            Logger = logger;
        }

        private static async Task<JwtSecurityToken> ValidateToken(
            string token,
            string issuer,
            IEnumerable<String> audiences,
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            if (string.IsNullOrEmpty(issuer)) throw new ArgumentNullException(nameof(issuer));

            var discoveryDocument = await configurationManager.GetConfigurationAsync(ct);
            var signingKeys = discoveryDocument.SigningKeys;

            var validationParameters = new TokenValidationParameters
            {
                RequireExpirationTime = true,
                RequireSignedTokens = true,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateAudience = true,
                ValidAudiences = audiences,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2) // Allow for some drift in server time
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, validationParameters, out var rawValidatedToken);

                return (JwtSecurityToken)rawValidatedToken;
            }
            catch (SecurityTokenValidationException)
            {
                return null;
            }
        }

        [Route("users")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> CheckAndProvisionWebShopUser()
        {
            if (!HttpContext.Request.Headers.ContainsKey("Authorization"))
                return Forbid();

            var bearerToken = HttpContext.Request.Headers["Authorization"];

            if (AuthenticationHeaderValue.TryParse(bearerToken, out var headerValue))
            {
                var scheme = headerValue.Scheme; // "Bearer"
                var parameter = headerValue.Parameter; // Token

                var issuer = WebshopConfiguration.Issuer;
                var audiences = WebshopConfiguration.Audiences;
                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{issuer}/.well-known/oauth-authorization-server",
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever());

                var jsonToken = await ValidateToken(parameter, issuer, audiences, configurationManager);
                if (jsonToken == null)
                {
                    Logger.LogInformation("Invalid ID Token recieved");
                    return Forbid();
                }

                var expectedAlg = SecurityAlgorithms.RsaSha256; //Okta uses RS256
                if (jsonToken.Header?.Alg == null || jsonToken.Header?.Alg != expectedAlg)
                {
                    Logger.LogInformation("Token signing algorithm is not RS256");
                    return Forbid();
                }

                var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrWhiteSpace(email))
                {
                    Logger.LogInformation("No email found in the token");
                    return BadRequest();
                }

                HttpContext.User.AddIdentity(new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.Actor, Guid.Empty.SystemUserId().ToString())
                }));

                try
                {
                    await WebshopService.ProvisionImplementUserAsync(email);
                    return Ok(WebshopConfiguration.WebshopRedirectUrl);
                }
                catch (Exception ex)
                {
                    Logger.LogError("{0}", ex.Message);
                    return BadRequest();
                }
            }

            Logger.LogInformation("Unable to parse the bearer token");
            return BadRequest();
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [Route("provision")]
        [Authorize]
        public async Task<ActionResult> ProvisionAuthenticatedUserIntoWebshop()
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                if (!Guid.TryParse(actor, out Guid callerId))
                    throw new BadHttpRequestException("Invalid callerId");

                await WebshopService.ProvisionUserWithEmployeeRoleAsync(callerId);
                
                return Ok(WebshopConfiguration.WebshopRedirectUrl);
            }
            catch (BadHttpRequestException exception)
            {
                Logger.LogError("Error in ProvisionUserIntoWebshop", exception);
                return BadRequest();
            }
            catch (Exception)
            {
                Logger.LogInformation("Unexpected exception in ProvisionUserIntoWebshop.");
                return BadRequest("Something went wrong during user provisioning into the web shop.");
            }
        }
    }
}
