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
        private readonly ICustomerSettingsService _customerSettingsService;
        private readonly IMapper _mapper;

        public SubscriptionManagementController(
            ILogger<SubscriptionManagementController> logger,
            ISubscriptionManagementService subscriptionServices,
            IMapper mapper,
            ICustomerSettingsService customerSettingsService)
        {
            _logger = logger;
            _subscriptionServices = subscriptionServices;
            _mapper = mapper;
            _customerSettingsService = customerSettingsService;
        }

        /// <summary>
        /// Get all operators
        /// </summary>
        /// <returns>all operators</returns>
        [HttpGet]
        [Route("operators")]
        [ProducesResponseType(typeof(IList<Operator>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllOperators()
        {
            var operatorList = await _subscriptionServices.GetAllOperatorsAsync();

            return Ok(operatorList.Select(m => new Operator(m)));
        }

        /// <summary>
        /// Get operator by ID
        /// </summary>
        /// <param name="id">Operator identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("operators/{id}")]
        [ProducesResponseType(typeof(Operator), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOperator(int id)
        {
            var operatorObject = await _subscriptionServices.GetOperatorAsync(id);

            return Ok(new Operator(operatorObject));
        }

        [HttpGet]
        [Route("{organizationId:Guid}/operators")]
        [ProducesResponseType(typeof(IEnumerable<Operator>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOperatorForCustomer(Guid organizationId)
        {
            var customerOperators = await _subscriptionServices.GetAllOperatorsForCustomerAsync(organizationId);

            return Ok(customerOperators.Select(m => new Operator(m)));
        }

        [HttpPost]
        [Route("{organizationId:Guid}/operators")]
        public async Task<ActionResult> AddOperatorsForCustomer(Guid organizationId, [FromBody] IList<int> operators)
        {
            await _customerSettingsService.AddOperatorsForCustomerAsync(organizationId, operators);

            return Ok();
        }

        [HttpDelete]
        [Route("{customerId:Guid}/operators/{id}")]
        public async Task<ActionResult> DeleteOperatorsForCustomer(Guid customerId, int id)
        {
            await _customerSettingsService.DeleteOperatorForCustomerAsync(customerId, id);

            return Ok();
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
            var addSubscriptionForCustomer = await _subscriptionServices.AddSubscriptionOrderForCustomerAsync(customerId, subscriptionOrder.SubscriptionProductId, subscriptionOrder.OperatorAccountId, subscriptionOrder.DataPackageId, subscriptionOrder.CallerId, subscriptionOrder.SimCardNumber);

            return CreatedAtAction(nameof(AddSubscriptionToCustomer), new SubscriptionOrder(addSubscriptionForCustomer));
        }

        /// <summary>
        /// Submit subscription order
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="subscriptionOrder">Details of the subscription order</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(SubscriptionOrder), (int)HttpStatusCode.OK)]
        [Route("{customerId:Guid}/subscription-transfer")]
        public async Task<IActionResult> TransferSubscription(Guid customerId, [FromBody] TransferSubscriptionOrder subscriptionOrder)
        {
            await _subscriptionServices.TransferSubscriptionOrderAsync(customerId, subscriptionOrder.SubscriptionProductId, subscriptionOrder.OperatorAccountId, subscriptionOrder.DataPackageId, subscriptionOrder.CallerId, subscriptionOrder.SimCardNumber, subscriptionOrder.OrderExecutionDate, subscriptionOrder.NewOperatorAccountId);

            return Ok();
        }

        /// <summary>
        /// Get list of customer operator accounts
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <returns>list of customer operator accounts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerOperatorAccount>), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        public async Task<IActionResult> GetAllOperatorAccountsForCustomer(Guid organizationId)
        {
            var customerOperatorAccounts = await _subscriptionServices.GetAllOperatorAccountsForCustomerAsync(organizationId);

            return Ok(customerOperatorAccounts.Select(m => new CustomerOperatorAccount(m)));
        }

        /// <summary>
        /// Setup customer account
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="customerOperatorAccount">Details of customer operator account</param>
        /// <returns>new customer operator account</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerOperatorAccount), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        public async Task<IActionResult> AddOperatorAccountForCustomer(Guid organizationId, [FromBody] CustomerOperatorAccount customerOperatorAccount)
        {
            var newCustomerOperatorAccount = await _subscriptionServices.AddOperatorAccountForCustomerAsync(organizationId, customerOperatorAccount.AccountNumber, customerOperatorAccount.AccountName, customerOperatorAccount.OperatorId, customerOperatorAccount.CallerId);

            return Ok(new CustomerOperatorAccount(newCustomerOperatorAccount));
        }

        [HttpDelete]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts/{accountNumber}")]
        public async Task<IActionResult> DeleteOperatorAccountsForCustomer(Guid organizationId, string accountNumber)
        {
            await _subscriptionServices.DeleteCustomerOperatorAccountAsync(organizationId, accountNumber);
            return Ok();
        }

        /// <summary>
        /// Add subscription product
        /// </summary>
        /// <param name="organizationId">Customer identifier</param>
        /// <param name="subscriptionProduct">Details of the subscription product</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{organizationId:Guid}/subscription-products")]
        [ProducesResponseType(typeof(SubscriptionProduct), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CustomerSubscriptionProduct>> AddSubscriptionProductForCustomer(Guid organizationId, [FromBody] NewSubscriptionProduct subscriptionProduct)
        {
            var addSubscriptionProduct = await _subscriptionServices.AddSubscriptionProductForCustomerAsync(organizationId, subscriptionProduct.OperatorName, subscriptionProduct.SubscriptionProductName, subscriptionProduct.DataPackages, subscriptionProduct.CallerId);
            var mappedSubscriptionProduct = _mapper.Map<CustomerSubscriptionProduct>(addSubscriptionProduct);
            return CreatedAtAction(nameof(AddSubscriptionProductForCustomer), mappedSubscriptionProduct);
        }

        [HttpGet]
        [Route("{customerId:Guid}/subscription-products/{operatorName}")]
        [ProducesResponseType(typeof(IList<SubscriptionProduct>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<SubscriptionProduct>>> GetOperatorSubscriptionProductForCustomer(Guid customerId, string operatorName)
        {
            var subscriptionProducts = await _subscriptionServices.GetOperatorSubscriptionProductForCustomerAsync(customerId, operatorName);
            
            var subscriptionProductsList = _mapper.Map<IEnumerable<SubscriptionProduct>>(subscriptionProducts);
            
            return Ok(subscriptionProductsList);
        }
        [HttpGet]
        [Route("{customerId:Guid}/subscription-products/settings/{operatorName}")]
        [ProducesResponseType(typeof(IList<SubscriptionProduct>), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<SubscriptionProduct>>> GetOperatorSubscriptionProductForSettingsAsync(Guid customerId, string operatorName)
        {
            var subscriptionProducts = await _subscriptionServices.GetOperatorSubscriptionProductForSettingsAsync(customerId,operatorName);

            var subscriptionProductsList = _mapper.Map<IEnumerable<SubscriptionProduct>>(subscriptionProducts);

            return Ok(subscriptionProductsList);
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