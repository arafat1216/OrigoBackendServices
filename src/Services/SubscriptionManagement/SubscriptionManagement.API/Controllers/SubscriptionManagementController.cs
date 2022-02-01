using Microsoft.AspNetCore.Mvc;
using SubscriptionManagementServices;
using SubscriptionManagementServices.Models;

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
            var operatorList = await _subscriptionServices.GetAllOperators();

            return Ok(operatorList);
        }
        [HttpGet]
        [Route("operator/{operatorName}")]
        public async Task<ActionResult<IEnumerable<string>>> GetOperator(string operatorName)
        {
            var operatorObject = await _subscriptionServices.GetOperator(operatorName);

            return Ok(operatorObject);
        }
        [HttpGet]
        [Route("{customerId:Guid}/operator")]
        public async Task<ActionResult<IEnumerable<string>>> GetOperatorForCustomer(Guid customerId)
        {
            var customerOperator = await _subscriptionServices.GetAllOperatorsForCustomer(customerId);

            return Ok(customerOperator);
        }
        [HttpPost]
        [Route("{customerId:Guid}/operator")]
        public async Task<ActionResult<bool>> AddOperatorsForCustomer(Guid customerId,[FromBody] IList<string> operators)
        {
            var addOperatorForCustomer = await _subscriptionServices.AddOperatorForCustomerAsync(customerId,operators);

            return Ok(addOperatorForCustomer);
        }
        [HttpDelete]
        [Route("{customerId:Guid}/operator/{operatorName}")]
        public async Task<ActionResult<bool>> DeleteOperatorsForCustomer(Guid customerId, string operatorName)
        {
            var deleteOperatorForCustomer = await _subscriptionServices.DeleteOperatorForCustomerAsync(customerId,operatorName);

            return Ok(deleteOperatorForCustomer);
        }
        [HttpPost]
        [Route("{customerId:Guid}/subscription")]
        public async Task<ActionResult<bool>> AddSubscriptionToCustomer(Guid customerId, [FromBody] SubscriptionOrder subscriptionOrder)
        {
            var addSubscriptionForCustomer = await _subscriptionServices.AddSubscriptionForCustomerAsync(customerId);
            
            return Ok(addSubscriptionForCustomer);
        }
    }
}