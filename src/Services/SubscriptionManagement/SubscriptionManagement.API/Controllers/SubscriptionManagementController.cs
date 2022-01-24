using Microsoft.AspNetCore.Mvc;

namespace SubscriptionManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubscriptionManagementController : ControllerBase
    {
        private readonly ILogger<SubscriptionManagementController> _logger;

        public SubscriptionManagementController(ILogger<SubscriptionManagementController> logger)
        {
            _logger = logger;
        }
    }
}