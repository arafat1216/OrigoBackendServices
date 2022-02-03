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
        [Route("{organizationId:Guid}/subscriptionProduct")]
        [HttpPost]
        public async Task<ActionResult<OrigoSubscriptionProduct>> CreateSubscriptionProducts(Guid organizationId, [FromBody] NewSubscriptionProduct newSubscriptionProduct)
        {
            OrigoSubscriptionProduct NewSubscriptionProduct = await _subscriptionManagementService.AddSubscriptionProductForCustomerAsync(organizationId, newSubscriptionProduct);

            return CreatedAtAction(nameof(CreateSubscriptionProducts), newSubscriptionProduct);
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


    }
}
