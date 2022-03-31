using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    //[Authorize]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class PartnersController : ControllerBase
    {
        private ILogger<PartnersController> Logger { get; }
        private ICustomerServices CustomerServices { get; }
        private IPartnerServices PartnerServices { get; }
        private readonly IMapper Mapper;
        public PartnersController(ILogger<PartnersController> logger, ICustomerServices customerServices, IPartnerServices partnerServices,  IMapper mapper)
        {
            Logger = logger;
            CustomerServices = customerServices;
            PartnerServices = partnerServices;
            Mapper = mapper;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Partner), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //[Authorize(Roles = "SystemAdmin")]
        //[PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Partner>> CreatePartner([FromBody] NewOrganization newCustomer)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                var createdCustomer = await CustomerServices.CreateOrganizationAsync(newCustomer, callerId);
                if (createdCustomer == null)
                    return BadRequest();


                // create partner with new org as coupeld org

                var partner = await PartnerServices.CreatePartnerAsync(createdCustomer.OrganizationId, callerId);
                if (partner == null)
                    return BadRequest();

                return CreatedAtAction(nameof(CreatePartner), new { Id = partner.ExternalId }, partner);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("{organizationId:guid}")]
        [ProducesResponseType(typeof(Partner), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [Authorize(Roles = "SystemAdmin")]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        public async Task<ActionResult<Partner>> CreatePartner(Guid organizationId)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                var organization = await CustomerServices.GetCustomerAsync(organizationId);
                if (organization == null)
                    return NotFound();


                var createdPartner = await PartnerServices.CreatePartnerAsync(organizationId, callerId);
                if (createdPartner == null)
                {
                    return BadRequest();
                }

                return CreatedAtAction(nameof(CreatePartner), new { Id = createdPartner.ExternalId }, createdPartner);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("{partnerId:guid}")]
        [ProducesResponseType(typeof(IList<Partner>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin")]
        public async Task<ActionResult<IList<Partner>>> GetPartner(Guid partnerId)
        {
            try
            {
                var partners = await PartnerServices.GetPartnerAsync(partnerId);
                return partners != null ? Ok(partners) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("getall")]
        [ProducesResponseType(typeof(IList<Partner>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<IList<Partner>>> GetPartners()
        {
            try
            {
                var partners = await PartnerServices.GetPartnersAsync();
                return partners != null ? Ok(partners) : NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
