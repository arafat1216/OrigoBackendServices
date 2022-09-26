using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Models.SubscriptionManagement;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "SystemAdmin,PartnerAdmin,CustomerAdmin,Admin")]
    [Route("origoapi/v{version:apiVersion}/Customers/{organizationId:Guid}/customer-reference-field")]
    public class CustomerReferenceFieldController : ControllerBase
    {
        private readonly ISubscriptionManagementService _subscriptionService;
        private readonly ILogger<CustomerReferenceFieldController> _logger;

        public CustomerReferenceFieldController(ISubscriptionManagementService subscriptionService, ILogger<CustomerReferenceFieldController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all customer reference fields
        /// </summary>
        /// <returns>all operators</returns>
        /// <remarks>
        ///  Type can be be 'User' or 'Account'.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(IList<OrigoCustomerReferenceField>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllCustomerReferenceFields(Guid organizationId)
        {
            // If role is not System admin, check access list
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                {
                    return Forbid();
                }
            }
            var origoCustomerReferenceFields = await _subscriptionService.GetAllCustomerReferenceFieldsAsync(organizationId);
            return Ok(origoCustomerReferenceFields);
        }

        /// <summary>
        /// Creates a new customer reference field
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="newCustomerReferenceField"></param>
        /// <returns></returns>
        /// <remarks>
        ///  Type can be be 'User' or 'Account'.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(OrigoCustomerReferenceField), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> CreateCustomerReferenceField(Guid organizationId, [FromBody] NewCustomerReferenceField newCustomerReferenceField)
        {
            try
            {
                // If role is not System admin, check access list
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                var origoCustomerReferenceField = await _subscriptionService.AddCustomerReferenceFieldAsync(organizationId, newCustomerReferenceField, actor);
                return CreatedAtAction(nameof(CreateCustomerReferenceField), origoCustomerReferenceField);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateCustomerReferenceField gateway. Message = {0}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete customer reference field
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="customerReferenceId">The id of the reference field to be deleted</param>
        /// <returns></returns>
        /// <remarks>
        ///  Type can be be 'User' or 'Account'.
        /// </remarks>
        [HttpDelete]
        [Route("{customerReferenceId:int}")]
        [ProducesResponseType(typeof(OrigoCustomerReferenceField), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<OrigoSubscriptionProduct>> DeleteCustomerReferenceField(Guid organizationId, int customerReferenceId)
        {
            try
            {
                // If role is not System admin, check access list
                var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (role != PredefinedRole.SystemAdmin.ToString())
                {
                    var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList();
                    if (accessList != null && (!accessList.Any() || !accessList.Contains(organizationId.ToString())))
                    {
                        return Forbid();
                    }
                }
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                var origoCustomerReferenceField = await _subscriptionService.DeleteCustomerReferenceFieldAsync(organizationId, customerReferenceId, actor);
                return Ok(origoCustomerReferenceField);
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateCustomerReferenceField gateway", ex.Message);
                return BadRequest();
            }
        }
    }
}
