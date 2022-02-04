using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models.SubscriptionManagement;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
    [Route("/origoapi/v{version:apiVersion}/[controller]")]
    public class SubscriptionManagementController : ControllerBase
    {
        private readonly ISubscriptionManagementService _subscriptionManagementService;
        private readonly ILogger<SubscriptionManagementController> _logger;
        private readonly IMapper _mapper;

        public SubscriptionManagementController(ISubscriptionManagementService subscriptionManagementService, ILogger<SubscriptionManagementController> logger, IMapper mapper)
        {
            _subscriptionManagementService = subscriptionManagementService;
            _logger = logger;
            _mapper = mapper;
        }


        //All avalible operators 
        [HttpGet]
        [Route("operator")]
        //[PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var operatorList = await _subscriptionManagementService.GetAllOperators();
            return Ok(operatorList);
        }

        //Operator by name
        [Route("operator/{operatorName}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrigoOperator), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoOperator>> Get(string operatorName)
        {
            try
            {
                var operatorObject = await _subscriptionManagementService.GetOperator(operatorName);
                
                return Ok(operatorObject);
            }
            catch (Exception ex)
            {
                _logger.LogError("Get with operator name ", ex.Message);
                return BadRequest();
            }
            
        }

        //All avalible operators by organization - this is for form
        [Route("{organizationId:Guid}/operator")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get(Guid organizationId)
        {
            var customersOperators = await _subscriptionManagementService.GetAllOperatorsForCustomer(organizationId);
            return Ok(customersOperators);
        }

        [Route("{organizationId:Guid}/operator")]
        [HttpPost]
        public async Task<ActionResult> CreateOperatorListForCustomerAsync(Guid organizationId, [FromBody] NewOperatorList operatorList)
        {
            var addOperatorListForCustomer = await _subscriptionManagementService.AddOperatorForCustomerAsync(organizationId, operatorList.Operators);
            if (!addOperatorListForCustomer)
            {
                
            }
            return NoContent();
        }

        [Route("{organizationId:Guid}/operator/{operatorName}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteFromCustomersOperatorList(Guid organizationId, string operatorName)
        {
            var deleteFromCustomerOperators = await _subscriptionManagementService.DeleteOperatorForCustomerAsync(organizationId, operatorName);
            if (!deleteFromCustomerOperators)
            {

            }
            return NoContent();
        }

        [Route("{organizationId:Guid}/subscription")]
        [HttpPost]
        public async Task<ActionResult> CreateOrderForTransferSubscriptionAsync(Guid organizationId, [FromBody] OrderTransferSubscription order)
        {
            var transeferSubscriptionOrder = await _subscriptionManagementService.AddSubscriptionForCustomerAsync(organizationId, order);
            if (!transeferSubscriptionOrder)
            {

            }
            return NoContent();
        }

        [Route("{organizationId:Guid}/subscriptionProducts")]
        [HttpPost]
        [ProducesResponseType(typeof(OrigoSubscriptionProduct), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> CreateSubscriptionProductForCustomer(Guid organizationId, [FromBody] NewSubscriptionProduct newSubscriptionProduct)
        {
            try 
            {
                var subscriptionProduct = await _subscriptionManagementService.AddSubscriptionProductForCustomerAsync(organizationId, newSubscriptionProduct);
                //if (subscriptionProduct == null)
                //{
                //    return BadRequest();
                //}
                return CreatedAtAction(nameof(CreateSubscriptionProductForCustomer), newSubscriptionProduct);
            }
            catch (Exception ex) 
            {

                _logger.LogError("CreateSubscriptionProductForCustomer gateway", ex.Message);
                return BadRequest();
            }
           
        }

        [Route("{organizationId:Guid}/subscriptionProducts/{operatorName}")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoSubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<OrigoSubscriptionProduct>>> GetSubscriptionProductsForCustomer(Guid organizationId, string operatorName)
        {
            try
            {
                var subscriptionProductList = await _subscriptionManagementService.GetSubscriptionProductForCustomerAsync(organizationId, operatorName);
                //if (subscriptionProductList == null)
                //{
                //    return BadRequest();
                //}
                return Ok(subscriptionProductList);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetSubscriptionProductsForCustomer gateway", ex.Message);
                return BadRequest();
            }
        }

        [Route("{organizationId:Guid}/subscriptionProducts/{subscriptionProductId}")]
        [HttpDelete]
        [ProducesResponseType(typeof(IList<OrigoSubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> DeleteSubscriptionProductsForCustomer(Guid organizationId, int subscriptionProductId)
        {
            try
            {
                var subscriptionProductList = await _subscriptionManagementService.DeleteSubscriptionProductForCustomerAsync(organizationId, subscriptionProductId);
                //if (subscriptionProductList == null)
                //{
                //    return BadRequest();
                //}
                return Ok(subscriptionProductList);
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteSubscriptionProductsForCustomer gateway", ex.Message);
                return BadRequest();
            }
        }
        [HttpPatch]
        [Route("{customerId:Guid}/subscriptionProducts/{subscriptionProductId}")]
        [ProducesResponseType(typeof(OrigoSubscriptionProduct), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> UpdateOperatorSubscriptionProductForCustomer(Guid customerId, int subscriptionProductId, [FromBody] UpdateSubscriptionProduct subscriptionProduct)
        {
            try
            {
                var updatedSubscriptionProducts = await _subscriptionManagementService.UpdateOperatorSubscriptionProductForCustomerAsync(customerId, subscriptionProductId,subscriptionProduct);

               

                //return the updated subscription product
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateOperatorSubscriptionProductForCustomer gateway ", ex);
                return BadRequest("Unable to update subscription product");
            }
        }


    }
}
