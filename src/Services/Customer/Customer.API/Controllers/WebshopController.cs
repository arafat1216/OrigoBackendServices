using Customer.API.Filters;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
            try
            {
                await _webshopService.CheckAndProvisionWebShopUserAsync(email);
                _logger.LogInformation($"A user with email {email} has been successfully provisioned.");
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogInformation($"Webshop user provisioning failed: " + ex.Message);
                return BadRequest("Unknown error in WebshopController - Check and Provision Webshop User (single): " + ex.Message);
            }
            
        }
    }
}
