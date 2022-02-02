using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices;
using System.Text.Json;
using System.Net;

namespace SubscriptionManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SubscriptionManagementController : ControllerBase
    {
        private readonly ILogger<SubscriptionManagementController> _logger;
        private readonly ISubscriptionManagementService _subscriptionServices;
        private readonly IMapper _mapper;

        public SubscriptionManagementController(ILogger<SubscriptionManagementController> logger, ISubscriptionManagementService subscriptionServices,IMapper mapper)
        {
            _logger = logger;
            _subscriptionServices = subscriptionServices;
            _mapper = mapper;
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
        public async Task<ActionResult<ViewModels.OperatorViewModel>> GetOperator(string operatorName)
        {
            var operatorObject = await _subscriptionServices.GetOperator(operatorName);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };

            return Ok(JsonSerializer.Serialize<object>(new ViewModels.OperatorViewModel(operatorObject), options));
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
        public async Task<ActionResult<bool>> AddOperatorsForCustomer(Guid customerId, [FromBody] IList<string> operators)
        {
            var addOperatorForCustomer = await _subscriptionServices.AddOperatorForCustomerAsync(customerId, operators);

            return Ok(addOperatorForCustomer);
        }

        [HttpDelete]
        [Route("{customerId:Guid}/operator/{operatorName}")]
        public async Task<ActionResult<bool>> DeleteOperatorsForCustomer(Guid customerId, string operatorName)
        {
            var deleteOperatorForCustomer = await _subscriptionServices.DeleteOperatorForCustomerAsync(customerId, operatorName);

            return Ok(deleteOperatorForCustomer);
        }

        [HttpPost]
        [Route("{customerId:Guid}/subscription")]
        public async Task<ActionResult<bool>> AddSubscriptionToCustomer(Guid customerId, [FromBody] SubscriptionOrder subscriptionOrder)
        {
            var addSubscriptionForCustomer = await _subscriptionServices.AddSubscriptionForCustomerAsync(customerId);

            return Ok(addSubscriptionForCustomer);
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
            var newCustomerOperatorAccount = await _subscriptionServices.AddOperatorAccountForCustomerAsync(customerId, customerOperatorAccount.OrganizationId, customerOperatorAccount.AccountNumber, customerOperatorAccount.AccountName, customerOperatorAccount.OperatorId);

            return Ok(new CustomerOperatorAccount(newCustomerOperatorAccount));
        }

        [HttpPost]
        [Route("{customerId:Guid}/subscriptionProduct")]
        [ProducesResponseType(typeof(ViewModels.SubscriptionProductViewModel), (int)HttpStatusCode.Created)]
         [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ViewModels.SubscriptionProductViewModel>> AddSubscriptionProductForCustomer(Guid customerId, [FromBody] NewSubscriptionProduct subscriptionProduct)
        {
            try
            {
                var addSubscriptionProduct = await _subscriptionServices.AddSubscriptionProductForCustomerAsync(customerId, subscriptionProduct.OperatorName, subscriptionProduct.ProductName, subscriptionProduct.DataPackages);
                
                var mappedSubscriptionProduct = _mapper.Map<SubscriptionProductViewModel>(addSubscriptionProduct);

                return CreatedAtAction(nameof(AddSubscriptionProductForCustomer), mappedSubscriptionProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError("AddSubscriptionProductForCustomer backend ", ex);
                return BadRequest("Unable to save create subscription product");
            }
        }
    }
}