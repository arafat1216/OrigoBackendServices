using Customer.API.ViewModels;
using CustomerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
                var organization = await _organizationServices.GetOrganizationAsync(newPartner.OrganizationId, true, true, false);
                if (organization == null) return NotFound();

                // if org already stored as fk in partner table?

                var partner = new CustomerServices.Models.Partner(organization, newPartner.CallerId);

                var partnerCreated = await _partnerServices.CreatePartnerAsync(partner);

                var updatedOrganizationView = new Organization
                {
                    OrganizationId = organization.OrganizationId,
                    Name = organization.Name,
                    OrganizationNumber = organization.OrganizationNumber,
                    Address = new Address(organization.Address),
                    ContactPerson = new ContactPerson(organization.ContactPerson),
                    Preferences = (organization.Preferences == null) ? null : new OrganizationPreferences(organization.Preferences),
                    Location = (organization.Location == null) ? null : new Location(organization.Location)
                };

                var partnerView = new Partner
                {
                    ExternalId = partnerCreated.ExternalId,
                    //OrganizationId = updatedOrganizationView.OrganizationId,
                    Organization = updatedOrganizationView
                };

                return partnerView;
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
            catch(Exception ex)
            {
                return BadRequest("Unknown error (PartnersController - Get Partner (single): " + ex.Message);
            }
        }

        [HttpGet]
        [Route("getall")]
        [ProducesResponseType(typeof(IEnumerable<Partner>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Partner>>> GetPartners()
        {
            try
            {
                var partners = await _partnerServices.GetPartnersAsync();
                IList<Partner> list = new List<Partner>();

                foreach (CustomerServices.Models.Partner partner in partners)
                {
                    var organizationView = new Organization
                    {
                        OrganizationId = partner.Organization.OrganizationId,
                        Name = partner.Organization.Name,
                        OrganizationNumber = partner.Organization.OrganizationNumber,
                        Address = new Address(partner.Organization.Address),
                        ContactPerson = new ContactPerson(partner.Organization.ContactPerson),
                        Preferences = (partner.Organization.Preferences == null) ? null : new OrganizationPreferences(partner.Organization.Preferences),
                        Location = (partner.Organization.Location == null) ? null : new Location(partner.Organization.Location),
                        ChildOrganizations = new List<Organization>()
                    };
                    if (partner.Organization.ChildOrganizations != null)
                    {
                        foreach (CustomerServices.Models.Organization childOrg in partner.Organization.ChildOrganizations)
                        {
                            var childOrgView = new Organization
                            {
                                OrganizationId = childOrg.OrganizationId,
                                Name = childOrg.Name,
                                OrganizationNumber = childOrg.OrganizationNumber,
                                Address = new Address(childOrg.Address),
                                ContactPerson = new ContactPerson(childOrg.ContactPerson),
                                Preferences = (childOrg.Preferences == null) ? null : new OrganizationPreferences(childOrg.Preferences),
                                Location = (childOrg.Location == null) ? null : new Location(childOrg.Location)
                            };
                            organizationView.ChildOrganizations.Add(childOrgView);
                        }
                    }
                    var partnerView = new Partner
                    {
                        ExternalId = partner.ExternalId,
                        Organization = organizationView
                    };
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
