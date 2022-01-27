using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models.SubscriptionManagement;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
    [Route("/origoapi/v{version:apiVersion}/[controller]")]
    public class SubscriptionManagementController : ControllerBase
    {
        //All avalible operators
        [Route("operator")]
        [HttpGet]
        //[PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Telenor - NO", "Telia - NO", "Telenor - SE", "Telia - SE" };
        }

        //All avalible operators by country
        [Route("operator/{operatorName}")]
        [HttpGet]
        public ActionResult<string> Get(string operatorName)
        {
            return "Telia - NO";
        }

        //All avalible operators by organization - this is for form
        [Route("operator/{organizationId:Guid}")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(Guid organizationId)
        {
            return new string[] { "Telenor - NO", "Telia - NO" };
        }

        [Route("{organizationId:Guid}/operator/{operatorName}")]
        [HttpPost]
        public ActionResult CreateOperatorListForCustomer(Guid organizationId, [FromBody] NewOperatorList operatorList)
        {
            return NoContent();
        }

        [Route("{organizationId:Guid}/operator/{operatorName}")]
        [HttpDelete]
        public ActionResult DeleteFromCustomersOperatorList(Guid organizationId, string operatorName)
        {
            return Ok();
        }

        [Route("{organizationId:Guid}/subscription")]
        [HttpPost]
        public ActionResult CreateOrderForTransferSubscription(Guid organizationId, [FromBody] OrderTransferSubscription order)
        {
            return NoContent();
        }

     
    }
}
