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
        private readonly IWebshopService WebshopService;
        private readonly ILogger<WebshopController> Logger;

        public WebshopController(IWebshopService webshopService, ILogger<WebshopController> logger)
        {
            WebshopService = webshopService;
            Logger = logger;
        }

        /// <summary>
        /// Provision Implement-specific web shop user
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <returns></returns>
        [Route("users")]
        [HttpPost]
        public async Task<IActionResult> CheckAndProvisionImplementWebShopUser([FromBody] string email)
        {
            try
            {
                await WebshopService.CheckAndProvisionImplementWebShopUserAsync(email);
                Logger.LogInformation($"A user with email {email} has been successfully provisioned.");
                return Ok();
            }
            catch (Exception exception)
            {
                Logger.LogInformation($"Webshop Implement user provisioning failed for email {email}: {exception.Message}");
                return BadRequest("Unknown error in WebshopController - Check and Provision Webshop User (single): " + exception.Message);
            }
        }

        /// <summary>
        /// Provision non-Implement web shop user.
        /// </summary>
        /// <param name="userId">Id of User to be provisioned</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [Route("provision")]
        [HttpPost]
        public async Task<IActionResult> CheckAndProvisionWebShopUser([FromBody] Guid userId)
        {
            try
            {
                await WebshopService.CheckAndProvisionWebShopUserAsync(userId);
                Logger.LogInformation($"Webshop user with userId {userId} has been provisioned into the web shop.");
                return Ok();
            }
            catch (Exception exception)
            {
                Logger.LogInformation($"CheckAndProvisionWebShopUser failed for userId {userId}: {exception.Message}");
                return BadRequest($"Unknown error in WebshopController - Check and Provision Web Shop User");
            }

        }
    }
}
