using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SubscriptionManagement.API.Filters;
using SubscriptionManagement.API.ViewModels;
using SubscriptionManagementServices;
using SubscriptionManagementServices.ServiceModels;
using System.Net;
using Swashbuckle.AspNetCore.Annotations;
using Common.Enums;
using System.Text.Json;
using Common.Interfaces;

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
        [SwaggerOperation(Tags = new[] { "Operators" })]
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
        [SwaggerOperation(Tags = new[] { "Operators" })]
        public async Task<IActionResult> GetOperator(int id)
        {
            var @operator = await _operatorService.GetOperatorAsync(id);
            return Ok(_mapper.Map<Operator>(@operator));
        }

        [HttpGet]
        [Route("{organizationId:Guid}/operators")]
        [ProducesResponseType(typeof(IEnumerable<Operator>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Customer Operators" })]
        public async Task<IActionResult> GetOperatorForCustomer(Guid organizationId)
        {
            var customerOperators = await _customerSettingsService.GetAllOperatorsForCustomerAsync(organizationId);
            return Ok(_mapper.Map<IList<Operator>>(customerOperators));
        }

        [HttpPost]
        [Route("{organizationId:Guid}/operators")]
        [SwaggerOperation(Tags = new[] { "Customer Operators" })]
        public async Task<ActionResult> AddOperatorsForCustomer(Guid organizationId, [FromBody] NewOperatorList operators)
        {
            await _customerSettingsService.AddOperatorsForCustomerAsync(organizationId, operators);
            return Ok();
        }

        [HttpDelete]
        [Route("{organizationId:Guid}/operators/{id}")]
        [SwaggerOperation(Tags = new[] { "Customer Operators" })]
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
        [ProducesResponseType(typeof(TransferToBusinessSubscriptionOrderDTOResponse), (int)HttpStatusCode.Created)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        [Route("{organizationId:Guid}/transfer-to-business")]
        public async Task<ActionResult<TransferToBusinessSubscriptionOrderDTOResponse>> TransferSubscription(Guid organizationId, [FromBody] TransferToBusinessSubscriptionOrderDTO subscriptionOrder)
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
        [ProducesResponseType(typeof(TransferToPrivateSubscriptionOrderDTOResponse), (int)HttpStatusCode.Created)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
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
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        [Route("{organizationId:Guid}/change-subscription")]
        public async Task<IActionResult> ChangeSubscriptionOrder(Guid organizationId, [FromBody] NewChangeSubscriptionOrder subscriptionOrder)
        {
            var addedOrder = await _subscriptionServices.ChangeSubscriptionOrder(organizationId, subscriptionOrder);

            return CreatedAtAction(nameof(ChangeSubscriptionOrder), addedOrder);
        }

        /// <summary>
        /// Cancel the subscription
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="subscriptionOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(CancelSubscriptionOrderDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        [Route("{organizationId:Guid}/subscription-cancel")]
        public async Task<IActionResult> CancelSubscriptionOrder(Guid organizationId, [FromBody] NewCancelSubscriptionOrder subscriptionOrder)
        {

            var addedOrder = await _subscriptionServices.CancelSubscriptionOrder(organizationId, subscriptionOrder);

            return CreatedAtAction(nameof(CancelSubscriptionOrder), addedOrder);
        }

        /// <summary>
        /// Order SIM card
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="subscriptionOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(OrderSimSubscriptionOrderDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        [Route("{organizationId:Guid}/order-sim")]
        public async Task<IActionResult> OrderSim(Guid organizationId, [FromBody] NewOrderSimSubscriptionOrder subscriptionOrder)
        {
            try
            {
                var addedOrder = await _subscriptionServices.OrderSim(organizationId, subscriptionOrder);

                return CreatedAtAction(nameof(OrderSim), addedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Activate SIM card
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="simOrder"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ActivateSimOrderDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        [Route("{organizationId:Guid}/activate-sim")]
        public async Task<IActionResult> ActivateSim(Guid organizationId, [FromBody] NewActivateSimOrder simOrder)
        {

            var addedOrder = await _subscriptionServices.ActivateSimAsync(organizationId, simOrder);

            return CreatedAtAction(nameof(ActivateSim), addedOrder);

        }
        /// <summary>
        /// New Subscription Order
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(NewSubscriptionOrderDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        [Route("{organizationId:Guid}/new-subscription")]
        public async Task<IActionResult> NewSubscriptionOrder(Guid organizationId, [FromBody] NewSubscriptionOrderRequestDTO order)
        {

            var addedOrder = await _subscriptionServices.NewSubscriptionOrderAsync(organizationId, order);

            return CreatedAtAction(nameof(NewSubscriptionOrder), addedOrder);

        }

        /// <summary>
        /// Gets a list of all subscription orders for a customer
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [Obsolete]
        [Route("{organizationId:Guid}/subscription-orders")]
        [ProducesResponseType(typeof(IList<SubscriptionOrderListItemDTO>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionOrders(Guid organizationId)
        {
            return Ok(await _subscriptionServices.GetSubscriptionOrderLog(organizationId));
        }

        /// <summary>
        /// Gets a Paginated list of all subscription orders for a customer
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="search"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="filterOptionsAsJsonString"></param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/subscription-orders/pagination")]
        [ProducesResponseType(typeof(PagedModel<SubscriptionOrderListItemDTO>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        [HttpGet]
        public async Task<ActionResult> GetAllSubscriptionOrders([FromRoute] Guid organizationId, CancellationToken cancellationToken, [FromQuery(Name = "q")] string? search,
                    [FromQuery] int page = 1, [FromQuery] int limit = 25,
                    [FromQuery(Name = "filterOptions")] string? filterOptionsAsJsonString = null)
        {
            FilterOptionsForSubscriptionOrder? filterOptions = null;
            if (!string.IsNullOrEmpty(filterOptionsAsJsonString))
            {
                filterOptions = JsonSerializer.Deserialize<FilterOptionsForSubscriptionOrder>(filterOptionsAsJsonString);
            }
            return Ok(await _subscriptionServices.GetAllSubscriptionOrderLog(organizationId, search, filterOptions?.OrderType, page, limit, cancellationToken));
        }

        /// <summary>
        /// Gets count of subscription orders for a customer
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="orderTypes">Selected Order Types to filter</param>
        /// <param name="phoneNumber">user's Phone number specific orders</param>
        /// <param name="checkOrderExist">If true, return the count if any orderType defined in parameter exist. If false, return toe total count</param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/subscription-orders/count")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders Count" })]
        [HttpGet]
        public async Task<ActionResult> GetSubscriptionOrdersCountAsync([FromRoute]Guid organizationId, [FromQuery] IList<SubscriptionOrderTypes>? orderTypes = null, [FromQuery] string? phoneNumber = null, [FromQuery] bool? checkOrderExist = false)
        {
            return Ok(await _subscriptionServices.GetSubscriptionOrdersCount(organizationId, orderTypes, phoneNumber, checkOrderExist!.Value));
        }

        /// <summary>
        ///     Get list of customer operator accounts
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <returns>list of customer operator accounts</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerOperatorAccount>), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        [SwaggerOperation(Tags = new[] { "Customer Operator Accounts" })]
        public async Task<IActionResult> GetAllOperatorAccountsForCustomer(Guid organizationId)
        {
            var customerOperatorAccounts =
                await _customerSettingsService.GetAllOperatorAccountsForCustomerAsync(organizationId);

            return Ok(customerOperatorAccounts);
        }

        /// <summary>
        /// Setup customer account
        /// </summary>
        /// <param name="organizationId">Organization identifier</param>
        /// <param name="customerOperatorAccount">Details of customer operator account</param>
        /// <returns>new customer operator account</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CustomerOperatorAccount), (int)HttpStatusCode.Created)]
        [Route("{organizationId:Guid}/operator-accounts")]
        [SwaggerOperation(Tags = new[] { "Customer Operator Accounts" })]
        public async Task<IActionResult> AddOperatorAccountForCustomer(Guid organizationId, [FromBody] NewOperatorAccount customerOperatorAccount)
        {
            var newCustomerOperatorAccount = await _customerSettingsService.AddOperatorAccountForCustomerAsync(
                organizationId, customerOperatorAccount.AccountNumber, customerOperatorAccount.AccountName,
                customerOperatorAccount.OperatorId, customerOperatorAccount.CallerId,
                customerOperatorAccount.ConnectedOrganizationNumber ?? string.Empty);

            return CreatedAtAction(nameof(AddOperatorAccountForCustomer), newCustomerOperatorAccount);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [Route("{organizationId:Guid}/operator-accounts")]
        [SwaggerOperation(Tags = new[] { "Customer Operator Accounts" })]
        public async Task<IActionResult> DeleteOperatorAccountsForCustomer(Guid organizationId, [FromQuery] string accountNumber, [FromQuery] int operatorId)
        {
            await _customerSettingsService.DeleteCustomerOperatorAccountAsync(organizationId, accountNumber, operatorId);
            return Ok();
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
        [SwaggerOperation(Tags = new[] { "Customer Reference Fields" })]
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
        [SwaggerOperation(Tags = new[] { "Customer Reference Fields" })]
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
        [SwaggerOperation(Tags = new[] { "Customer Reference Fields" })]
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
        [SwaggerOperation(Tags = new[] { "Customer Subscription Products" })]
        public async Task<ActionResult<IEnumerable<CustomerSubscriptionProductDTO>>> GetOperatorSubscriptionProductForCustomer(Guid organizationId, [FromQuery] bool includeOperator = false)
        {
            var subscriptionProducts = await _customerSettingsService.GetAllCustomerSubscriptionProductsAsync(organizationId, includeOperator);

            return Ok(subscriptionProducts);
        }

        [HttpGet]
        [Route("operators/subscription-products")]
        [ProducesResponseType(typeof(IList<SubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Tags = new[] { "Customer Subscription Products" })]
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
        [SwaggerOperation(Tags = new[] { "Customer Subscription Products" })]
        public async Task<ActionResult<CustomerSubscriptionProduct>> DeleteOperatorSubscriptionProductForCustomer(Guid organizationId, int subscriptionProductId)
        {
            var deletedSubscriptionProducts = await _customerSettingsService.DeleteOperatorSubscriptionProductForCustomerAsync(organizationId, subscriptionProductId);
            if (deletedSubscriptionProducts == null)
            {
                return NotFound();
            }

            return Ok(deletedSubscriptionProducts);
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
        [SwaggerOperation(Tags = new[] { "Customer Subscription Products" })]
        public async Task<ActionResult<CustomerSubscriptionProduct>> AddSubscriptionProductForCustomer(Guid organizationId, [FromBody] NewSubscriptionProduct subscriptionProduct)
        {
            var addSubscriptionProduct = await _customerSettingsService.AddOperatorSubscriptionProductForCustomerAsync(organizationId, subscriptionProduct.OperatorId, subscriptionProduct.Name, subscriptionProduct.DataPackages, subscriptionProduct.CallerId);

            return CreatedAtAction(nameof(AddSubscriptionProductForCustomer), addSubscriptionProduct);
        }

        [HttpPatch]
        [Route("{organizationId:Guid}/subscription-products/{subscriptionProductId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [SwaggerOperation(Tags = new[] { "Customer Subscription Products" })]
        public async Task<ActionResult<SubscriptionProduct>> UpdateOperatorSubscriptionProductForCustomer(Guid organizationId, int subscriptionProductId, [FromBody] UpdatedSubscriptionProduct subscriptionProduct)
        {
            var customerSubscriptionProductDTO = _mapper.Map<CustomerSubscriptionProductDTO>(subscriptionProduct);
            customerSubscriptionProductDTO.Id = subscriptionProductId;
            var updatedSubscriptionProduct = await _customerSettingsService.UpdateSubscriptionProductForCustomerAsync(organizationId, customerSubscriptionProductDTO);
            return Ok(updatedSubscriptionProduct);
        }

        /// <summary>
        /// Add standard private subscription product
        /// </summary>
        /// <param name="organizationId">Customer identifier</param>
        /// <param name="standardProduct">Details of the standard private subscription product</param>
        /// <returns></returns>
        [HttpPost]
        [Route("{organizationId:Guid}/standard-private-subscription-products")]
        [ProducesResponseType(typeof(CustomerStandardPrivateSubscriptionProductDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Tags = new[] { "Customer Subscription Products" })]
        public async Task<ActionResult<CustomerStandardPrivateSubscriptionProductDTO>> PostStandardPrivateSubscriptionProducts(Guid organizationId, NewCustomerStandardPrivateSubscriptionProduct standardProduct)
        {
            var subscriptionProducts = await _customerSettingsService.CreateStandardPrivateSubscriptionProductsAsync(organizationId, standardProduct);

            return CreatedAtAction(nameof(PostStandardPrivateSubscriptionProducts), subscriptionProducts);
        }
        /// <summary>
        /// Get all standard private subscription product for customer
        /// </summary>
        /// <param name="organizationId">Customer identifier</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{organizationId:Guid}/standard-private-subscription-products")]
        [ProducesResponseType(typeof(CustomerStandardPrivateSubscriptionProductDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Tags = new[] { "Customer Subscription Products" })]
        public async Task<ActionResult<IEnumerable<CustomerStandardPrivateSubscriptionProductDTO>>> GetStandardPrivateSubscriptionProducts(Guid organizationId)
        {
            
            var subscriptionProducts = await _customerSettingsService.GetStandardPrivateSubscriptionProductsAsync(organizationId);

            return Ok(subscriptionProducts);
        }


        /// <summary>
        /// Delete standard private subscription product
        /// </summary>
        /// <param name="organizationId">Customer identifier</param>
        /// <param name="operatorId">The operator to delete from</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{organizationId:Guid}/standard-private-subscription-products/{operatorId:Int}")]
        [ProducesResponseType(typeof(CustomerStandardPrivateSubscriptionProductDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [SwaggerOperation(Tags = new[] { "Customer Subscription Products" })]
        public async Task<ActionResult<CustomerStandardPrivateSubscriptionProductDTO>> DeleteStandardPrivateSubscriptionProducts(Guid organizationId, int operatorId, [FromBody] Guid callerId)
        {
            
            var deletedSubscriptionProducts = await _customerSettingsService.DeleteStandardPrivateSubscriptionProductsAsync(organizationId, operatorId, callerId);
            if (deletedSubscriptionProducts == null)
            {
                return NotFound();
            }

            return Ok(deletedSubscriptionProducts);
        }

        /// <summary>
        /// Gets a detailed view for a subscription order for a customer
        /// </summary>
        /// <param name="organizationId">Customer identifier</param>
        /// <param name="orderId">Order identifier</param>
        /// <param name="orderType">Order type/param>
        /// <returns></returns>
        [HttpGet]
        [Route("{organizationId:Guid}/subscription-orders-detail-view/{orderId:Guid}/{orderType:int}")]
        [ProducesResponseType(typeof(DetailViewSubscriptionOrderLog), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Tags = new[] { "Subscription Orders" })]
        public async Task<ActionResult<DetailViewSubscriptionOrderLog>> GetDetailViewSubscriptionOrderLog(Guid organizationId,Guid orderId, int orderType)
        {

            var subscriptionProducts = await _subscriptionServices.GetDetailViewSubscriptionOrderLogAsync(organizationId, orderId, orderType);

            return Ok(subscriptionProducts);
        }
       
    }
}