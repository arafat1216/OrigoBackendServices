using Customer.API.Filters;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class WebshopController : ControllerBase
    {
        private readonly IWebshopService _webshopService;
        private readonly ILogger<WebshopController> _logger;

        public WebshopController(IWebshopService webshopService, ILogger<WebshopController> logger)
        {
            _webshopService = webshopService;
            _logger = logger;
        }

        /// <summary>
        /// Provision web shop user
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <returns></returns>
        [Route("users")]
        [HttpPost]
        public async Task<IActionResult> CheckAndProvisionWebShopUser([FromBody] string email)
        {
            await _webshopService.CheckAndProvisionWebShopUserAsync(email);
            _logger.LogInformation($"A user with email {email} has been successfully provisioned.");
            return Ok();
        }
    }
}
