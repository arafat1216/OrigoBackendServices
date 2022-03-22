using Customer.API.ViewModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class PartnersController : ControllerBase
    {
        private readonly IOrganizationServices _organizationServices;
        private readonly IPartnerServices _partnerServices;
        private readonly ILogger<PartnersController> _logger;

        public PartnersController(ILogger<PartnersController> logger, IOrganizationServices customerServices, IPartnerServices partnerServices)
        {
            _logger = logger;
            _organizationServices = customerServices;
            _partnerServices = partnerServices;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Partner), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Partner>> CreatePartner([FromBody] NewPartner newPartner)
        {
            try
            {
                var result = await _partnerServices.CreatePartnerAsync(newPartner.OrganizationId, newPartner.CallerId);

                var organization = new PartnerOrganization(result.Organization);
                var partner = new Partner(result.ExternalId, organization);

                return StatusCode(201, partner);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{partnerId:guid}")]
        [ProducesResponseType(typeof(Partner), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Partner>> GetPartner(Guid partnerId)
        {
            try
            {
                var partner = await _partnerServices.GetPartnerAsync(partnerId);
                if (partner == null)
                    return NotFound();
                return Ok(partner);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error (PartnersController - Get Partner (single): " + ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Partner>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Partner>>> GetPartners()
        {
            try
            {
                var partners = await _partnerServices.GetPartnersAsync();
                var list = new List<Partner>();

                foreach (CustomerServices.Models.Partner partner in partners)
                {
                    var partnerOrganization = new PartnerOrganization(partner.Organization);
                    var partnerView = new Partner(partner.ExternalId, partnerOrganization);

                    list.Add(partnerView);
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error (PartnersController - Get Partners (multiple): " + ex.Message);
            }
        }
    }
}
