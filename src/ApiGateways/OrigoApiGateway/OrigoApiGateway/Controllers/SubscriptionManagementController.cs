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
        [HttpGet]
        //[PermissionAuthorize(PermissionOperator.And, Permission.CanReadCustomer, Permission.CanReadAsset)]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "Telenor - Norge", "Telia - Norge", "Telenor - Sverige", "Telia - Sverige" };
        }

        //All avalible operators by country
        [Route("{countryCode}")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(string countryCode)
        {
            return new string[] { "Telia - Norge", "Telenor - Norge" };
        }

        //All avalible operators by organization - this is for form
        [Route("operator/{organizationId:Guid}")]
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get(Guid organizationId)
        {
            return new string[] { "Telenor" };
        }

        [Route("operator/{organizationId:Guid}")]
        [HttpPost]
        public ActionResult CreateOperatorListForCustomer([FromBody] NewOperatorList operatorList)
        {
            return NoContent();
        }
        [Route("subscription/{organizationId:Guid}")]
        [HttpPost]
        public ActionResult CreateOrderForTransferSubscription(Guid organizationId,[FromBody] OrderTransferSubscription order)
        {
            return NoContent();
        }

        [Route("{organizationId:Guid}")]
        [HttpDelete]
        public ActionResult DeleteFromCustomersOperatorList(int operatorId)
        {
            return Ok();
        }
    }
}
