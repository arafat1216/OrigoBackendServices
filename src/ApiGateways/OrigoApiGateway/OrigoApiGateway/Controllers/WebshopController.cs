using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Filters;
using OrigoApiGateway.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
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

                var handler = new JwtSecurityTokenHandler();

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
