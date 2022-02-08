using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SubscriptionManagement.API.Filters;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices;
using System.Net;

namespace SubscriptionManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class SubscriptionManagementController : ControllerBase
    {
        private readonly ILogger<SubscriptionManagementController> _logger;
        private readonly ISubscriptionManagementService _subscriptionServices;
        private readonly IMapper _mapper;

        public SubscriptionManagementController(ILogger<SubscriptionManagementController> logger, ISubscriptionManagementService subscriptionServices, IMapper mapper)
        {
            _logger = logger;
            _subscriptionServices = subscriptionServices;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("operators")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllOperators()
        {
            var operatorList = await _subscriptionServices.GetAllOperatorsAsync();

            return Ok(operatorList);
        }

        [HttpGet]
        [Route("operators/{operatorName}")]
        public async Task<ActionResult<Operator>> GetOperator(string operatorName)
        {
            var operatorObject = await _subscriptionServices.GetOperator(operatorName);
            var mappedOperator = _mapper.Map<Operator>(operatorObject);

            return Ok(mappedOperator);
        }

        [HttpGet]
        [Route("{customerId:Guid}/operators")]
        [ProducesResponseType(typeof(IEnumerable<Operator>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOperatorForCustomer(Guid customerId)
        {
            var customerOperators = await _subscriptionServices.GetAllOperatorsForCustomerAsync(customerId);

            return Ok(customerOperators.Select(m => new Operator(m)));
        }

        [HttpPost]
        [Route("{customerId:Guid}/operators")]
        public async Task<ActionResult<bool>> AddOperatorsForCustomer(Guid customerId, [FromBody] IList<string> operators)
        {
            var addOperatorForCustomer = await _subscriptionServices.AddOperatorForCustomerAsync(customerId, operators);

            return Ok(addOperatorForCustomer);
        }

        [HttpDelete]
        [Route("{customerId:Guid}/operators/{operatorName}")]
        public async Task<ActionResult<bool>> DeleteOperatorsForCustomer(Guid customerId, string operatorName)
        {
            var deleteOperatorForCustomer = await _subscriptionServices.DeleteOperatorForCustomerAsync(customerId, operatorName);

            return Ok(deleteOperatorForCustomer);
        }

        /// <summary>
        /// Submit subscription order
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="subscriptionOrder">Details of the subscription order</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(SubscriptionOrder), (int)HttpStatusCode.OK)]
        [Route("{customerId:Guid}/subscription")]
        public async Task<ActionResult<bool>> AddSubscriptionToCustomer(Guid customerId, [FromBody] SubscriptionOrder subscriptionOrder)
        {
            var addSubscriptionForCustomer = await _subscriptionServices.AddSubscriptionOrderForCustomerAsync(customerId, subscriptionOrder.SubscriptionProductId, subscriptionOrder.OperatorAccountId, subscriptionOrder.DatapackageId, subscriptionOrder.CallerId);

            return CreatedAtAction(nameof(AddSubscriptionToCustomer), new SubscriptionOrder(addSubscriptionForCustomer));
        }

        /// <summary>
        /// Get list of customer operator accounts
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <returns>list of customer operator accounts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerOperatorAccount>), (int)HttpStatusCode.OK)]
        [Route("{customerId:Guid}/operator-accounts")]
        public async Task<IActionResult> GetAllOperatorAccountsForCustomer(Guid customerId)
        {
            var customerOperatorAccounts = await _subscriptionServices.GetAllOperatorAccountsForCustomerAsync(customerId);

            return Ok(customerOperatorAccounts.Select(m => new CustomerOperatorAccount(m)));
        }

        /// <summary>
        /// Setup customer account
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="customerOperatorAccount">Details of customer operator account</param>
        /// <returns>new customer operator account</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerOperatorAccount), (int)HttpStatusCode.OK)]
        [Route("{customerId:Guid}/operator-accounts")]
        public async Task<IActionResult> AddOperatorAccountForCustomer(Guid customerId, [FromBody] CustomerOperatorAccount customerOperatorAccount)
        {
            var newCustomerOperatorAccount = await _subscriptionServices.AddOperatorAccountForCustomerAsync(customerId, customerOperatorAccount.OrganizationId, customerOperatorAccount.AccountNumber, customerOperatorAccount.AccountName, customerOperatorAccount.OperatorId, customerOperatorAccount.CallerId);

            return Ok(new CustomerOperatorAccount(newCustomerOperatorAccount));
        }

        /// <summary>
        /// Add subscription product
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="subscriptionProduct">Details of the subscription product</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{customerId:Guid}/subscription-products")]
        [ProducesResponseType(typeof(SubscriptionProduct), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<SubscriptionProduct>> AddSubscriptionProductForCustomer(Guid customerId, [FromBody] NewSubscriptionProduct subscriptionProduct)
        {
            var addSubscriptionProduct = await _subscriptionServices.AddSubscriptionProductForCustomerAsync(customerId, subscriptionProduct.OperatorName, subscriptionProduct.ProductName, subscriptionProduct.DataPackages, subscriptionProduct.CallerId);

            var mappedSubscriptionProduct = _mapper.Map<SubscriptionProduct>(addSubscriptionProduct);

            return CreatedAtAction(nameof(AddSubscriptionProductForCustomer), mappedSubscriptionProduct);
        }

        [HttpGet]
        [Route("{customerId:Guid}/subscription-products/{operatorName}")]
        [ProducesResponseType(typeof(IList<SubscriptionProduct>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<SubscriptionProduct>>> GetOperatorSubscriptionProductForCustomer(Guid customerId, string operatorName)
        {
            var subscriptionProducts = await _subscriptionServices.GetOperatorSubscriptionProductForCustomerAsync(customerId, operatorName);

            //return the list
            return Ok();
        }

        [HttpDelete]
        [Route("{customerId:Guid}/subscription-products/{subscriptionProductId}")]
        [ProducesResponseType(typeof(SubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<SubscriptionProduct>> DeleteOperatorSubscriptionProductForCustomer(Guid customerId, int subscriptionProductId)
        {
            var deletedSubscriptionProducts = await _subscriptionServices.DeleteOperatorSubscriptionProductForCustomerAsync(customerId, subscriptionProductId);

            var mappedSubscriptionProduct = _mapper.Map<SubscriptionProduct>(deletedSubscriptionProducts);
            //return the deleted subscription product
            return Ok();
        }

        [HttpPatch]
        [Route("{customerId:Guid}/subscription-products/{subscriptionProductId}")]
        [ProducesResponseType(typeof(IList<SubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<SubscriptionProduct>> UpdateOperatorSubscriptionProductForCustomer(Guid customerId, int subscriptionProductId, [FromBody] SubscriptionProduct subscription)
        {
            var updatedSubscriptionProducts = await _subscriptionServices.UpdateOperatorSubscriptionProductForCustomerAsync(customerId, subscriptionProductId);

            var mappedSubscriptionProduct = _mapper.Map<SubscriptionProduct>(updatedSubscriptionProducts);

            //return the updated subscription product
            return Ok();
        }
    }
}