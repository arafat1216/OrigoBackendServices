using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SubscriptionManagement.API.Filters;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices;
using SubscriptionManagementServices.ServiceModels;
using System.Net;

namespace SubscriptionManagement.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class SubscriptionManagementController : ControllerBase
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<SubscriptionManagementController> _logger;
        private readonly ISubscriptionManagementService _subscriptionServices;
        private readonly ICustomerSettingsService _customerSettingsService;
        private readonly IOperatorService _operatorService;
        private readonly IMapper _mapper;

        public SubscriptionManagementController(
            ILogger<SubscriptionManagementController> logger,
            ISubscriptionManagementService subscriptionServices,
            IMapper mapper,
            ICustomerSettingsService customerSettingsService,
            IOperatorService operatorService)
        {
            _logger = logger;
            _subscriptionServices = subscriptionServices;
            _mapper = mapper;
            _customerSettingsService = customerSettingsService;
            _operatorService = operatorService;
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
            var operatorList = await _operatorService.GetAllOperatorsAsync();
            return Ok(_mapper.Map<IList<Operator>>(operatorList));
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
            var @operator = await _operatorService.GetOperatorAsync(id);
            return Ok(_mapper.Map<Operator>(@operator));
        }

        [HttpGet]
        [Route("{organizationId:Guid}/operators")]
        [ProducesResponseType(typeof(IEnumerable<Operator>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOperatorForCustomer(Guid organizationId)
        {
            var customerOperators = await _customerSettingsService.GetAllOperatorsForCustomerAsync(organizationId);
            return Ok(_mapper.Map<IList<Operator>>(customerOperators));
        }

        [HttpPost]
        [Route("{organizationId:Guid}/operators")]
        public async Task<ActionResult> AddOperatorsForCustomer(Guid organizationId, [FromBody] NewOperatorList operators)
        {
            

            await _customerSettingsService.AddOperatorsForCustomerAsync(organizationId, operators);

            return Ok();
        }

        [HttpDelete]
        [Route("{organizationId:Guid}/operators/{id}")]
        public async Task<ActionResult> DeleteOperatorsForCustomer(Guid organizationId, int id)
        {
            await _customerSettingsService.DeleteOperatorForCustomerAsync(organizationId, id);

            return Ok();
        }

        
        /// <summary>
        /// Submit subscription order
        /// </summary>
        /// <param name="organizationId">Customer identifier</param>
        /// <param name="subscriptionOrder">Details of the subscription order</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TransferToBusinessSubscriptionOrderDTO), (int)HttpStatusCode.Created)]
        [Route("{organizationId:Guid}/transfer-to-business")]
        public async Task<ActionResult<TransferToBusinessSubscriptionOrderDTO>> TransferSubscription(Guid organizationId, [FromBody] TransferToBusinessSubscriptionOrderDTO subscriptionOrder)
        {
            var privateSubscription = await _subscriptionServices.TransferPrivateToBusinessSubscriptionOrderAsync(organizationId, subscriptionOrder);

            return CreatedAtAction(nameof(TransferSubscription), privateSubscription);
        }

        /// <summary>
        /// Create a transfer to private subscription order.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="subscriptionOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(TransferToPrivateSubscriptionOrderDTO), (int)HttpStatusCode.Created)]
        [Route("{organizationId:Guid}/transfer-to-private")]
        public async Task<IActionResult> TransferSubscriptionToPrivate(Guid organizationId, [FromBody] TransferToPrivateSubscriptionOrderDTO subscriptionOrder)
        {
            var dto = await _subscriptionServices.TransferToPrivateSubscriptionOrderAsync(organizationId, subscriptionOrder);
            return CreatedAtAction(nameof(TransferSubscriptionToPrivate), dto);
        }
        /// <summary>
        /// Create a order for changing subscription product for customer.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="subscriptionOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ChangeSubscriptionOrderDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [Route("{organizationId:Guid}/change-subscription")]
        public async Task<IActionResult> ChangeSubscriptionOrder(Guid organizationId, [FromBody] NewChangeSubscriptionOrder subscriptionOrder)
        {
            try
            {
                var addedOrder = await _subscriptionServices.ChangeSubscriptionOrder(organizationId, subscriptionOrder);

                return CreatedAtAction(nameof(ChangeSubscriptionOrder), addedOrder);

            }catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Gets a list of all subscription orders for a customer
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/subscription-orders")]
        [ProducesResponseType(typeof(IList<SubscriptionOrderListItemDTO>), (int)HttpStatusCode.OK)]
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionOrders(Guid organizationId)
        {
            return Ok(await _subscriptionServices.GetSubscriptionOrderLog(organizationId));
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
            var customerOperatorAccounts = await _customerSettingsService.GetAllOperatorAccountsForCustomerAsync(organizationId);

            return Ok(customerOperatorAccounts);
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
        public async Task<IActionResult> AddOperatorAccountForCustomer(Guid organizationId, [FromBody] NewOperatorAccount customerOperatorAccount)
        {
            var newCustomerOperatorAccount = await _customerSettingsService.AddOperatorAccountForCustomerAsync(organizationId, customerOperatorAccount.AccountNumber, customerOperatorAccount.AccountName, customerOperatorAccount.OperatorId, customerOperatorAccount.CallerId, customerOperatorAccount.ConnectedOrganizationNumber);

            return Ok(newCustomerOperatorAccount);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        public async Task<IActionResult> DeleteOperatorAccountsForCustomer(Guid organizationId, [FromQuery] string accountNumber, [FromQuery] int operatorId)
        {
            await _customerSettingsService.DeleteCustomerOperatorAccountAsync(organizationId, accountNumber, operatorId);
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
            var addSubscriptionProduct = await _customerSettingsService.AddOperatorSubscriptionProductForCustomerAsync(organizationId, subscriptionProduct.OperatorId, subscriptionProduct.Name, subscriptionProduct.DataPackages, subscriptionProduct.CallerId);

            return CreatedAtAction(nameof(AddSubscriptionProductForCustomer), addSubscriptionProduct);
        }

        /// <summary>
        /// Add customer reference field
        /// </summary>
        /// <param name="organizationId">Customer identifier</param>
        /// <param name="newCustomerReferenceField">Details of the new reference field</param>
        /// <returns></returns>
        /// <remarks>
        ///  Type can be be 'User' or 'Account'.
        /// </remarks>
        [HttpPost]
        [Route("{organizationId:Guid}/customer-reference-fields")]
        [ProducesResponseType(typeof(CustomerReferenceField), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CustomerReferenceField>> AddCustomerReferenceField(Guid organizationId, [FromBody] NewCustomerReferenceField newCustomerReferenceField)
        {
            var addCustomerReferenceField = await _customerSettingsService.AddCustomerReferenceFieldAsync(organizationId, newCustomerReferenceField.Name, newCustomerReferenceField.Type, newCustomerReferenceField.CallerId);
            var mappedCustomerReferenceField = _mapper.Map<CustomerReferenceField>(addCustomerReferenceField);
            return CreatedAtAction(nameof(AddCustomerReferenceField), mappedCustomerReferenceField);
        }

        /// <summary>
        /// Delete a customer reference field based on id for a customer.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="customerReferenceFieldId">Id of the field requested for delete</param>
        /// <returns></returns>
        /// <remarks>
        ///  Type can be be 'User' or 'Account'.
        /// </remarks>
        [HttpDelete]
        [Route("{organizationId:Guid}/customer-reference-fields/{customerReferenceFieldId:int}")]
        [ProducesResponseType(typeof(CustomerReferenceField), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<CustomerReferenceField>>> DeleteCustomerReferenceFieldsForCustomer(Guid organizationId, int customerReferenceFieldId)
        {
            var customerReferenceFieldDTO = await _customerSettingsService.DeleteCustomerReferenceFieldsAsync(organizationId, customerReferenceFieldId);
            if (customerReferenceFieldDTO == null)
            {
                return NotFound();
            }
            var customerReferenceField = _mapper.Map<CustomerReferenceField>(customerReferenceFieldDTO);
            return Ok(customerReferenceField);
        }

        /// <summary>
        /// Get a list of all reference field used by a customer.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        /// <remarks>
        ///  Type can be be 'User' or 'Account'.
        /// </remarks>
        [HttpGet]
        [Route("{organizationId:Guid}/customer-reference-fields")]
        [ProducesResponseType(typeof(IList<CustomerReferenceField>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<CustomerReferenceField>>> GetCustomerReferenceFieldsForCustomer(Guid organizationId)
        {
            var customerReferenceFieldDTOs = await _customerSettingsService.GetCustomerReferenceFieldsAsync(organizationId);
            var customerReferenceFields = _mapper.Map<List<CustomerReferenceField>>(customerReferenceFieldDTOs);
            return Ok(customerReferenceFields);
        }

        [HttpGet]
        [Route("{organizationId:Guid}/subscription-products")]
        [ProducesResponseType(typeof(IList<CustomerSubscriptionProductDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<CustomerSubscriptionProductDTO>>> GetOperatorSubscriptionProductForCustomer(Guid organizationId)
        {
            var subscriptionProducts = await _customerSettingsService.GetAllCustomerSubscriptionProductsAsync(organizationId);

            return Ok(subscriptionProducts);
        }

        [HttpGet]
        [Route("operators/subscription-products")]
        [ProducesResponseType(typeof(IList<SubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<SubscriptionProduct>>> GetOperatorSubscriptionProductForSettingsAsync()
        {
            var subscriptionProducts = await _customerSettingsService.GetAllOperatorSubscriptionProductAsync();

            return Ok(subscriptionProducts);
        }

        [HttpDelete]
        [Route("{organizationId:Guid}/subscription-products/{subscriptionProductId}")]
        [ProducesResponseType(typeof(SubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<CustomerSubscriptionProduct>> DeleteOperatorSubscriptionProductForCustomer(Guid organizationId, int subscriptionProductId)
        {
            var deletedSubscriptionProducts = await _customerSettingsService.DeleteOperatorSubscriptionProductForCustomerAsync(organizationId, subscriptionProductId);
            if (deletedSubscriptionProducts == null)
            {
                return NotFound();
            }

            return Ok(deletedSubscriptionProducts);
        }

        [HttpPatch]
        [Route("{organizationId:Guid}/subscription-products")]
        [ProducesResponseType(typeof(IList<SubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<SubscriptionProduct>> UpdateOperatorSubscriptionProductForCustomer(Guid organizationId, [FromBody] SubscriptionProduct subscriptionProduct)
        {
            var updatedSubscriptionProducts = await _customerSettingsService.UpdateOperatorSubscriptionProductForCustomerAsync(organizationId, _mapper.Map<CustomerSubscriptionProductDTO>(subscriptionProduct));
            var mappedSubscriptionProduct = _mapper.Map<SubscriptionProduct>(updatedSubscriptionProducts);
            return Ok(mappedSubscriptionProduct);
        }
    }
}