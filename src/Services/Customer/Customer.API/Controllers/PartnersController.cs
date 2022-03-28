using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
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
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: Returned when an unexpected error occurs.")]
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

        /// <summary>
        ///     Creates a new partner.
        /// </summary>
        /// <remarks>
        ///     Registers an existing organization as a partner. The organization should ideally be newly created as it cannot
        ///     be flagged as a customer or have a partner assigned.
        /// </remarks>
        /// <param name="newPartner"> The details for the new partner. </param>
        /// <returns> The created object. </returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(Partner))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request: Returned if the organization is set as a customer, or is assigned to a parter.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "Conflict: Returned if the organization is already registered as a partner.")]
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
                if (ex is ArgumentException || ex is CustomerNotFoundException)
                    return BadRequest($"{ex.Message}");
                if (ex is DuplicateException)
                    return Conflict($"{ex.Message}");
                else
                    return StatusCode(500, $"Unknown error: {ex.Message}");
            }
        }

        /// <summary>
        ///     Retrieves a partner by it's ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves the partner matching the provided ID..
        /// </remarks>
        /// <param name="partnerId"> The ID of the partner that should be retrieved. </param>
        /// <returns> If found, the corresponding partner. </returns>
        [HttpGet]
        [Route("{partnerId:guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Partner))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "The partner was not found")]
        public async Task<ActionResult<Partner?>> GetPartner(Guid partnerId)
        {
            try
            {
                var partner = await _partnerServices.GetPartnerAsync(partnerId);

                if (partner == null)
                    return NotFound();

                var partnerOrganization = new PartnerOrganization(partner.Organization);
                var partnerView = new Partner(partner.ExternalId, partnerOrganization);

                return Ok(partnerView);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unknown error: {ex.Message}");
            }
        }

        /// <summary>
        ///     Retrieves all partners.
        /// </summary>
        /// <remarks>
        ///     Retrieves a list containing all partners registered in the solution.
        /// </remarks>
        /// <returns> A list containing all partners. </returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Partner))]
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
                return StatusCode(500, $"Unknown error: {ex.Message}");
            }
        }
    }
}
