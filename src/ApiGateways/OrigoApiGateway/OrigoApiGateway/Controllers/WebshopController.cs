using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
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
        public WebshopController(IWebshopService webshopService, ILogger<WebshopController> logger)
        {
            _webshopService = webshopService;
            _logger = logger;
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
                var issuer = "https://origoidp.mytos.no/oauth2/aus4zl6rw8yihu7k60i7";
                var audiences = (IEnumerable<String>) new String[] {"0oa78f6tkat5lg1qn0i7", "0oa752p55ihmtFo4p0i7"};

                var handler = new JwtSecurityTokenHandler();
                try
                {
                    handler.ValidateToken(parameter, new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = issuer,
                        ValidAudiences = audiences
                    }, out SecurityToken validatedToken);
                }
                catch
                {
                    return Forbid();
                }

                var jsonToken = (JwtSecurityToken)handler.ReadToken(parameter);
                var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogInformation("No email found in the token");
                    return Forbid();
                }

                await _webshopService.ProvisionUserAsync(email);

                return Ok();
            }

            _logger.LogInformation("Unable to parse the bearer token");
            return Forbid();
        }
    }
}
