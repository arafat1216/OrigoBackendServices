using Microsoft.AspNetCore.Mvc;
using SubscriptionManagementServices;

namespace SubscriptionManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SubscriptionManagementController : ControllerBase
    {
        private readonly ILogger<SubscriptionManagementController> _logger;
        private readonly ISubscriptionManagementService _subscriptionServices;

        public SubscriptionManagementController(ILogger<SubscriptionManagementController> logger, ISubscriptionManagementService subscriptionServices)
        {
            _logger = logger;
            _subscriptionServices = subscriptionServices;
        }

        [HttpGet]
        [Route("operator")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllOperators()
        {
            var operators = await _subscriptionServices.GetOperator();

            return Ok(operators);
        }
        [HttpGet]
        [Route("operator/{customerId:Guid}")]
        public async Task<ActionResult<IEnumerable<string>>> GetOperatorForCustomer(Guid customerId)
        {
            var operatorForCustomer = await _subscriptionServices.GetOperator();
            return Ok();
        }
    }
}