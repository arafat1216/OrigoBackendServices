using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrigoApiGateway.Authorization;
using OrigoApiGateway.Models;
using OrigoApiGateway.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Route("origoapi/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized)]
    [SwaggerResponse(StatusCodes.Status403Forbidden)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: Returned when an unexpected error occurs.")]
    public class PartnersController : ControllerBase
    {
        private readonly ILogger<PartnersController> _logger;
        private readonly ICustomerServices _customerServices;
        private readonly IPartnerServices _partnerServices;
        private readonly IMapper _mapper;

        public PartnersController(ILogger<PartnersController> logger, ICustomerServices customerServices, IPartnerServices partnerServices, IMapper mapper)
        {
            _logger = logger;
            _customerServices = customerServices;
            _partnerServices = partnerServices;
            _mapper = mapper;
        }

        /// <summary>
        ///     Create a new partner.
        /// </summary>
        /// <remarks>
        ///     This creates a new organization, and converts it to a partner.
        /// </remarks>
        /// <param name="newPartnerOrganization"> The partner's organization details. </param>
        /// <returns> The created partner object. </returns>
        [HttpPost]
        [Authorize(Roles = "SystemAdmin")]
        [PermissionAuthorize(PermissionOperator.And, Permission.CanCreateCustomer, Permission.CanUpdateCustomer)]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(Partner))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Partner>> CreatePartnerAsync([FromBody] NewPartnerOrganization newPartnerOrganization)
        {
            try
            {
                var actor = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
                Guid.TryParse(actor, out Guid callerId);

                var createdCustomer = await _customerServices.CreatePartnerOrganization(newPartnerOrganization, callerId);
                if (createdCustomer == null)
                    return BadRequest();

                var partner = await _partnerServices.CreatePartnerAsync(createdCustomer.OrganizationId, callerId);
                if (partner == null)
                    return BadRequest();

                return StatusCode(StatusCodes.Status201Created, partner);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        ///     Retrieves a partner by it's ID.
        /// </summary>
        /// <remarks>
        ///     Retrieves a partner by it's ID.
        /// </remarks>
        /// <param name="partnerId"> The ID of the partner that should be retrieved. </param>
        /// <returns> If found, the corresponding partner-object. </returns>
        [HttpGet]
        [Route("{partnerId:guid}")]
        [Authorize(Roles = "SystemAdmin,PartnerAdmin")]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Partner))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Partner>> GetPartnerAsync([FromRoute] Guid partnerId)
        {
            try
            {
                var partner = await _partnerServices.GetPartnerAsync(partnerId);

                if (partner is null)
                    return NotFound();
                else
                    return Ok(partner);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        ///     Retrieves all partners.
        /// </summary>
        /// <remarks>
        ///     Retrieves a list containing all partners the user has access to.
        /// </remarks>
        /// <returns> A list containing all partners available to the user. </returns>
        [HttpGet]
        [Authorize(Roles = "SystemAdmin")]
        [PermissionAuthorize(Permission.CanReadCustomer)]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(IList<Partner>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IList<Partner>>> GetPartnersAsync()
        {
            try
            {
                var partners = await _partnerServices.GetPartnersAsync();
                return Ok(partners);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
