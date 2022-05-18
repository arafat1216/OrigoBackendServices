using Common.Exceptions;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using CustomerServices.ServiceModels;
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
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationServices _organizationServices;
        private readonly IPartnerServices _partnerServices;
        private readonly ILogger<OrganizationsController> _logger;

        public OrganizationsController(ILogger<OrganizationsController> logger, IOrganizationServices customerServices, IPartnerServices partnerServices)
        {
            _logger = logger;
            _organizationServices = customerServices;
            _partnerServices = partnerServices;
        }

        [Route("{organizationId:Guid}/{customerOnly:Bool}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrganizationDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Gone)]
        public async Task<ActionResult<OrganizationDTO>> Get(Guid organizationId, bool includeOrganizationPreferences = true, bool includeLocation = true, bool customerOnly = false)
        {
            try
            {
                var organization = await _organizationServices.GetOrganizationAsync(organizationId, includeOrganizationPreferences, includeLocation, customerOnly);
                if (organization == null) return NotFound();

                var foundCustomer = new OrganizationDTO
                {
                    OrganizationId = organization.OrganizationId,
                    Name = organization.Name,
                    OrganizationNumber = organization.OrganizationNumber,
                    Address = new AddressDTO(organization.Address),
                    ContactPerson = new ContactPersonDTO(organization.ContactPerson),
                    Preferences = (organization.Preferences == null) ? null : new OrganizationPreferencesDTO(organization.Preferences),
                    Location = (organization.Location == null) ? null : new LocationDTO(organization.Location),
                    PartnerId = organization.Partner?.ExternalId
                };

                return Ok(foundCustomer);
            }
            catch (EntityIsDeletedException ex)
            {
                _logger.LogError("Entity is deleted. {0}", ex.Message);
                return StatusCode(StatusCodes.Status410Gone);
            }
            catch (CustomerNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"OrganizationController - UpdateOrganizationPut: An unexpected error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("{customersOnly:Bool}")]
        [ProducesResponseType(typeof(IEnumerable<OrganizationDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrganizationDTO>>> Get(bool hierarchical = false, bool customersOnly = false, [FromQuery] Guid? partnerId = null)
        {
            try
            {
                var organizations = await _organizationServices.GetOrganizationsAsync(hierarchical, customersOnly, partnerId);
                IList<OrganizationDTO> list = new List<OrganizationDTO>();

                foreach (CustomerServices.Models.Organization org in organizations)
                {
                    var organizationView = new OrganizationDTO
                    {
                        OrganizationId = org.OrganizationId,
                        Name = org.Name,
                        OrganizationNumber = org.OrganizationNumber,
                        Address = new AddressDTO(org.Address),
                        ContactPerson = new ContactPersonDTO(org.ContactPerson),
                        Preferences = (org.Preferences == null) ? null : new OrganizationPreferencesDTO(org.Preferences),
                        Location = (org.Location == null) ? null : new LocationDTO(org.Location),
                        ChildOrganizations = new List<OrganizationDTO>(),
                        PartnerId = org.Partner?.ExternalId
                    };

                    if (org.ChildOrganizations != null)
                    {
                        foreach (CustomerServices.Models.Organization childOrg in org.ChildOrganizations)
                        {
                            var childOrgView = new OrganizationDTO
                            {
                                OrganizationId = childOrg.OrganizationId,
                                Name = childOrg.Name,
                                OrganizationNumber = childOrg.OrganizationNumber,
                                Address = new AddressDTO(childOrg.Address),
                                ContactPerson = new ContactPersonDTO(childOrg.ContactPerson),
                                Preferences = (childOrg.Preferences == null) ? null : new OrganizationPreferencesDTO(childOrg.Preferences),
                                Location = (childOrg.Location == null) ? null : new LocationDTO(childOrg.Location)
                            };
                            organizationView.ChildOrganizations.Add(childOrgView);
                        }
                    }
                    list.Add(organizationView);
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error (OrganizationsController - Get Organizations (multiple): " + ex.Message);
            }
        }

        [Route("userCount")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<CustomerServices.Models.OrganizationUserCount>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Gone)]
        public async Task<ActionResult<IList<CustomerServices.Models.OrganizationUserCount>>> GetOrganizationUserCountAsync()
        {
            try
            {
                var organizationUserCount = await _organizationServices.GetOrganizationUserCountAsync();
                if (organizationUserCount == null)
                    return NotFound();

                return Ok(organizationUserCount);
            }
            catch (Exception ex)
            {
                _logger.LogError($"OrganizationController - GetOrganizationUserCountAsync: An unexpected error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(OrganizationDTO))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, description: "Returned when there are problems with the provided input.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, description: "Returned when one or more of the provided ID's don't exist.")]
        [SwaggerResponse(StatusCodes.Status409Conflict, description: "Returned when the change would result in a conflict. Typically this is occurs when a user is trying to create new instances of an existing unique value.")]
        public async Task<ActionResult<OrganizationDTO>> CreateOrganization([FromBody] NewOrganizationDTO organization)
        {
            try
            {
                var result = await _organizationServices.AddOrganizationAsync(organization);

                return CreatedAtAction(nameof(CreateOrganization), new { id = result.OrganizationId }, result);
            }
            catch (DuplicateException ex)
            {
                return Conflict(ex.Message);
            }
            catch (CustomerNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"OrganizationController - CreateOrganization: An unexpected error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("{organizationId:Guid}/organization")]
        [HttpPut]
        [ProducesResponseType(typeof(OrganizationDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationDTO>> UpdateOrganizationPut([FromBody] UpdateOrganization organization)
        {
            try
            {
                // Get address values
                bool addressIsNull = (organization.Address == null);
                string street = (addressIsNull) ? "" : organization.Address.Street;
                string postCode = (addressIsNull) ? "" : organization.Address.PostCode;
                string city = (addressIsNull) ? "" : organization.Address.City;
                string country = (addressIsNull) ? "" : organization.Address.Country;

                // Get ContactPerson values
                bool contactpersonIsNull = (organization.ContactPerson == null);
                string firstName = (contactpersonIsNull) ? "" : organization.ContactPerson.FirstName;
                string lastName = (contactpersonIsNull) ? "" : organization.ContactPerson.LastName;
                string email = (contactpersonIsNull) ? "" : organization.ContactPerson.Email;
                string phoneNumber = (contactpersonIsNull) ? "" : organization.ContactPerson.PhoneNumber;

                // Update
                var updatedOrganization = await _organizationServices.PutOrganizationAsync(organization.OrganizationId, organization.ParentId, organization.PrimaryLocation, organization.CallerId,
                                                           organization.Name, organization.OrganizationNumber, street, postCode, city, country, firstName, lastName, email, phoneNumber);

                var updatedOrganizationView = new OrganizationDTO
                {
                    OrganizationId = updatedOrganization.OrganizationId,
                    Name = updatedOrganization.Name,
                    OrganizationNumber = updatedOrganization.OrganizationNumber,
                    Address = new AddressDTO(updatedOrganization.Address),
                    ContactPerson = new ContactPersonDTO(updatedOrganization.ContactPerson),
                    Preferences = (updatedOrganization.Preferences == null) ? null : new OrganizationPreferencesDTO(updatedOrganization.Preferences),
                    Location = (updatedOrganization.Location == null) ? null : new LocationDTO(updatedOrganization.Location),
                    PartnerId = updatedOrganization.Partner?.ExternalId
                };

                return updatedOrganizationView;
            }
            catch (CustomerNotFoundException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPut: No result on given OrganizationId: " + ex.Message);
                return NotFound(ex.Message);
            }
            catch (ParentNotValidException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPut: Given parentId (not null || empty) led to organization that A: does not exist, B: has itself a parent." +
                    "\n : " + ex.Message);
                return BadRequest(ex.Message);
            }
            catch (RequiredFieldIsEmptyException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPut: The name field is required and cannot be one of: (null || string.Empty). Null is allowed for patch queries." +
                   "\n : " + ex.Message);
                return BadRequest(ex.Message);
            }
            catch (LocationNotFoundException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPut: No result on Given locationId (not null || empty): " + ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOrganizationNumberException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPut: Conflict due to organization number being in use.");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"OrganizationController - UpdateOrganizationPut: An unexpected error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("{organizationId:Guid}/organization")]
        [HttpPost]
        [ProducesResponseType(typeof(OrganizationDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationDTO>> UpdateOrganizationPatch([FromBody] UpdateOrganization organization)
        {
            try
            {
                // Get address values
                bool addressIsNull = (organization.Address == null);
                string street = (addressIsNull) ? null : organization.Address.Street;
                string postCode = (addressIsNull) ? null : organization.Address.PostCode;
                string city = (addressIsNull) ? null : organization.Address.City;
                string country = (addressIsNull) ? null : organization.Address.Country;

                // Get ContactPerson values
                bool contactpersonIsNull = (organization.ContactPerson == null);
                string firstName = (contactpersonIsNull) ? null : organization.ContactPerson.FirstName;
                string lastName = (contactpersonIsNull) ? null : organization.ContactPerson.LastName;
                string email = (contactpersonIsNull) ? null : organization.ContactPerson.Email;
                string phoneNumber = (contactpersonIsNull) ? null : organization.ContactPerson.PhoneNumber;

                // Update
                var updatedOrganization = await _organizationServices.PatchOrganizationAsync(organization.OrganizationId, organization.ParentId, organization.PrimaryLocation, organization.CallerId,
                                                           organization.Name, organization.OrganizationNumber, street, postCode, city, country, firstName, lastName, email, phoneNumber);

                var updatedOrganizationView = new OrganizationDTO
                {
                    OrganizationId = updatedOrganization.OrganizationId,
                    Name = updatedOrganization.Name,
                    OrganizationNumber = updatedOrganization.OrganizationNumber,
                    Address = new AddressDTO(updatedOrganization.Address),
                    ContactPerson = new ContactPersonDTO(updatedOrganization.ContactPerson),
                    Preferences = (updatedOrganization.Preferences == null) ? null : new OrganizationPreferencesDTO(updatedOrganization.Preferences),
                    Location = (updatedOrganization.Location == null) ? null : new LocationDTO(updatedOrganization.Location),
                    PartnerId = updatedOrganization.Partner?.ExternalId
                };

                return updatedOrganizationView;
            }
            catch (CustomerNotFoundException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPatch: No result on given OrganizationId: " + ex.Message);
                return NotFound(ex.Message);
            }
            catch (ParentNotValidException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPatch: Given parentId (not null || empty) led to organization that A: does not exist, B: has itself a parent." +
                    "\n : " + ex.Message);
                return BadRequest(ex.Message);
            }
            catch (RequiredFieldIsEmptyException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPatch: The name field is required and cannot be string.Empty. Null is allowed for patch queries." +
                   "\n : " + ex.Message);
                return BadRequest(ex.Message);
            }
            catch (LocationNotFoundException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPatch: No result on Given locationId (not null || empty): " + ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOrganizationNumberException ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPatch: Conflict due to organization number being in use.");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationController - UpdateOrganizationPatch: failed to update: " + ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("{organizationId:Guid}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<OrganizationDTO>> DeleteOrganization([FromBody] DeleteOrganization deleteOrganization)
        {
            try
            {
                if (deleteOrganization.OrganizationId == Guid.Empty)
                    return BadRequest("Invalid given for organization");

                var removedOrganization = await _organizationServices.DeleteOrganizationAsync(deleteOrganization.OrganizationId, deleteOrganization.CallerId, deleteOrganization.HardDelete);

                var removedOrganizationView = new OrganizationDTO
                {
                    OrganizationId = removedOrganization.OrganizationId,
                    Name = removedOrganization.Name,
                    OrganizationNumber = removedOrganization.OrganizationNumber,
                    Address = new AddressDTO(removedOrganization.Address),
                    ContactPerson = new ContactPersonDTO(removedOrganization.ContactPerson),
                    Preferences = (removedOrganization.Preferences == null) ? null : new OrganizationPreferencesDTO(removedOrganization.Preferences),
                    Location = (removedOrganization.Location == null) ? null : new LocationDTO(removedOrganization.Location),
                    PartnerId = removedOrganization.Partner?.ExternalId
                };
                return Ok(removedOrganizationView);

            }
            catch (CustomerNotFoundException ex)
            {
                _logger.LogError("The organization with the given id was not found: {0}", ex.Message);
                return NotFound("The organization with the given id was not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"OrganizationController - DeleteOrganizationAsync: An unexpected error occurred: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Route("{organizationId:Guid}/preferences")]
        [HttpGet]
        [ProducesResponseType(typeof(OrganizationPreferencesDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Gone)]
        public async Task<ActionResult<OrganizationPreferencesDTO>> GetOrganizationPreferences(Guid organizationId)
        {
            try
            {
                var preferences = await _organizationServices.GetOrganizationPreferencesAsync(organizationId);

                if (preferences is null)
                    return NotFound();
                else
                    return Ok(preferences);
            }
            catch (EntityIsDeletedException ex)
            {
                _logger.LogError("The organization preferences are deleted", ex);
                return StatusCode((int)HttpStatusCode.Gone);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error - Get Organizationpreferences: " + ex.Message);
            }
        }

        [Route("{organizationId:Guid}/preferences")]
        [HttpPost]
        [ProducesResponseType(typeof(OrganizationDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationPreferencesDTO>> UpdateOrganizationPreferencesPatch([FromBody] UpdateOrganizationPreferences preferences)
        {
            try
            {
                if (preferences.OrganizationId == Guid.Empty)
                    return BadRequest("Organization Id cannot be empty");

                var newPreference = new CustomerServices.Models.OrganizationPreferences(preferences.OrganizationId,
                                                                                        preferences.CallerId,
                                                                                        preferences.WebPage,
                                                                                        preferences.LogoUrl,
                                                                                        preferences.OrganizationNotes,
                                                                                        preferences.EnforceTwoFactorAuth,
                                                                                        preferences.PrimaryLanguage,
                                                                                        preferences.DefaultDepartmentClassification);

                var updatedPreferences = await _organizationServices.UpdateOrganizationPreferencesAsync(newPreference, true);
                if (updatedPreferences == null)
                    return NotFound();

                var updatedPreferencesView = new OrganizationPreferencesDTO(updatedPreferences);

                return Ok(updatedPreferencesView);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error: " + ex.Message);
            }
        }

        [Route("{organizationId:Guid}/preferences")]
        [HttpPut]
        [ProducesResponseType(typeof(OrganizationDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationPreferencesDTO>> UpdateOrganizationPreferencesPut([FromBody] UpdateOrganizationPreferences preferences)
        {
            try
            {
                if (preferences.OrganizationId == Guid.Empty)
                    return BadRequest("Organization Id cannot be empty");

                var newPreference = new CustomerServices.Models.OrganizationPreferences(preferences.OrganizationId,
                                                                                        preferences.CallerId,
                                                                                        preferences.WebPage,
                                                                                        preferences.LogoUrl,
                                                                                        preferences.OrganizationNotes,
                                                                                        preferences.EnforceTwoFactorAuth,
                                                                                        preferences.PrimaryLanguage,
                                                                                        preferences.DefaultDepartmentClassification);
                var updatedPreferences = await _organizationServices.UpdateOrganizationPreferencesAsync(newPreference);
                if (updatedPreferences == null)
                    return NotFound();

                var updatedPreferencesView = new OrganizationPreferencesDTO(updatedPreferences);

                return Ok(updatedPreferencesView);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error: " + ex.Message);
            }
        }

        [Route("location")]
        [HttpPut]
        [ProducesResponseType(typeof(LocationDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<LocationDTO>> UpdateOrganizationLocation([FromBody] UpdateLocation location)
        {
            try
            {
                if (location.LocationId == Guid.Empty)
                    return BadRequest("Location id cannot be empty.");

                var newLocation = new CustomerServices.Models.Location(location.LocationId,
                                                                       location.CallerId,
                                                                       location.Name,
                                                                       location.Description,
                                                                       location.Address1,
                                                                       location.Address2,
                                                                       location.PostalCode,
                                                                       location.City,
                                                                       location.Country);

                var updatedLocation = await _organizationServices.UpdateOrganizationLocationAsync(newLocation);

                if (updatedLocation == null)
                    return NotFound();

                var updatedLocationView = new LocationDTO(updatedLocation);

                return Ok(updatedLocationView);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error: " + ex.Message);
            }
        }

        [Route("{locationId:Guid}/location")]
        [HttpDelete]
        [ProducesResponseType(typeof(LocationDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<LocationDTO>> DeleteLocation(Guid locationId, Guid callerId, bool hardDelete = false)
        {
            if (locationId == Guid.Empty)
                return BadRequest("Location id cannot be empty");

            var location = await _organizationServices.GetLocationAsync(locationId);

            if (location == null)
                return NotFound();

            await _organizationServices.DeleteOrganizationLocationAsync(locationId, callerId, hardDelete);

            var deletedLocationView = new LocationDTO(location);

            return Ok(deletedLocationView);
        }

    }
}