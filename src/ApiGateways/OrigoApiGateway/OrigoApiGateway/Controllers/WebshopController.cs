using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
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
        private readonly IWebshopService _webshopService;
        private ILogger<WebshopController> _logger { get; }
        private readonly WebshopConfiguration _webshopConfiguration;

        public WebshopController(IWebshopService webshopService, IOptions<WebshopConfiguration> options, ILogger<WebshopController> logger)
        {
            _webshopConfiguration = options.Value;
            _webshopService = webshopService;
            _logger = logger;
        }

        private static async Task<JwtSecurityToken> ValidateToken(
            string token,
            string issuer,
            IEnumerable<String> audiences,
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager,
            CancellationToken ct = default(CancellationToken))
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

                var issuer = _webshopConfiguration.Issuer;
                var audiences = _webshopConfiguration.Audiences;
                var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{issuer}/.well-known/oauth-authorization-server",
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever());

                var jsonToken = await ValidateToken(parameter, issuer, audiences, configurationManager);
                if (jsonToken == null)
                {
                    _logger.LogInformation("Invalid ID Token recieved");
                    return Forbid();
                }

                var expectedAlg = SecurityAlgorithms.RsaSha256; //Okta uses RS256
                if (jsonToken.Header?.Alg == null || jsonToken.Header?.Alg != expectedAlg)
                {
                    _logger.LogInformation("Token signing algorithm is not RS256");
                    return Forbid();
                }

                var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogInformation("No email found in the token");
                    return BadRequest();
                }

                try
                {
                    await _webshopService.ProvisionUserAsync(email);
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError("{0}", ex.Message);
                    return BadRequest();
                }
            }

            _logger.LogInformation("Unable to parse the bearer token");
            return BadRequest();
        }
    }
}
