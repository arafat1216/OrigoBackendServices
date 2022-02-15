using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models.SubscriptionManagement;
using OrigoApiGateway.Services;
using System;
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

        [Route("{customerId:Guid}/subscription-transfer")]
        [HttpPost]
        public async Task<ActionResult> TransferSubscription(Guid customerId, [FromBody] TransferSubscriptionOrder order)
        {
            await _subscriptionManagementService.TransferSubscriptionOrderForCustomerAsync(customerId, order);
            return NoContent();
        }
    }
}
