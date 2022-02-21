using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models.SubscriptionManagement;
using OrigoApiGateway.Services;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    public class OperatorsController : ControllerBase
    {
        private ISubscriptionManagementService _subscriptionService { get; }
        public OperatorsController(ISubscriptionManagementService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        /// <summary>
        /// Get all operators
        /// </summary>
        /// <returns>all operators</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoOperator>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllOperators()
        {
            var operatorList = await _subscriptionService.GetAllOperatorsAsync();
            return Ok(operatorList);
        }

        /// <summary>
        /// Get operator by ID
        /// </summary>
        /// <param name="id">Operator identifier</param>
        /// <returns>Operator</returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(OrigoOperator), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOperator(int id)
        {
            var operatorObject = await _subscriptionService.GetOperatorAsync(id);
            return Ok(operatorObject);
        }
        
        /// <summary>
        /// Get all operators subscription products
        /// </summary>
        /// <returns>all subscription products for all the operators</returns>
        [HttpGet]
        [Route("subscription-products")]
        [ProducesResponseType(typeof(IList<OrigoSubscriptionProduct>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllOperatorsSubscriptionProducts()
        {
            var operatorSubscriptionProducts = await _subscriptionService.GetAllOperatorsSubscriptionProductsAsync();
            return Ok(operatorSubscriptionProducts);
        }

    }
}
