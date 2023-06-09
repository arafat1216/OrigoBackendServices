﻿using AutoMapper;
using Common.Exceptions;
using Common.Interfaces;
using Customer.API.Filters;
using Customer.API.ViewModels;
using Customer.API.WriteModels;
using CustomerServices;
using CustomerServices.Exceptions;
using CustomerServices.ServiceModels;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error: Returned when an unexpected error occurs.")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationServices _organizationServices;
        private readonly IPartnerServices _partnerServices;
        private readonly ILogger<OrganizationsController> _logger;
        private readonly IMapper _mapper;


        public OrganizationsController(ILogger<OrganizationsController> logger, IOrganizationServices customerServices, IPartnerServices partnerServices, IMapper mapper)
        {
            _logger = logger;
            _organizationServices = customerServices;
            _partnerServices = partnerServices;
            _mapper = mapper;
        }

        [Route("{organizationId:Guid}/{customerOnly:Bool}")]
        [HttpGet]
        [ProducesResponseType(typeof(OrganizationDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Gone)]
        public async Task<ActionResult<OrganizationDTO>> Get([FromRoute] Guid organizationId, [FromQuery] bool includeDepartments = true, [FromQuery] bool includeOrganizationPreferences = true, [FromQuery] bool includeLocation = true, [FromQuery] bool includePartner = true, [FromRoute] bool customerOnly = false)
        {
            try
            {
                var organization = await _organizationServices.GetOrganizationAsync(organizationId, includeDepartments, includeOrganizationPreferences, includeLocation, includePartner, customerOnly);
                if (organization == null)
                    return NotFound();

                return Ok(_mapper.Map<OrganizationDTO>(organization));
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
        public async Task<ActionResult<IEnumerable<OrganizationDTO>>> Get(bool hierarchical = false, [FromRoute] bool customersOnly = false, [FromQuery] Guid? partnerId = null, [FromQuery] bool includePreferences = true)
        {
            try
            {
                var organizations = await _organizationServices.GetOrganizationsAsync(includePreferences, hierarchical, customersOnly, partnerId);
                return Ok(_mapper.Map<IList<OrganizationDTO>>(organizations));
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error (OrganizationsController - Get Organizations (multiple): " + ex.Message);
            }
        }

        /// <summary>
        /// Get all organizations.
        /// </summary>
        /// <remarks>
        /// Retrieves a paginated set containing all matching organizations.
        /// </remarks>
        /// <param name="cancellationToken"> A dependency-injected <see cref="CancellationToken"/>. </param>
        /// <param name="hierarchical"> When <see langword="true"/>, only top-level organizations are returned, with their children instead attached as
        ///     a sub-item inside their parent-organizations. When <see langword="false"/>, a flat list is generated that included all organizations. </param>
        /// <param name="customersOnly"></param>
        /// <param name="search">
        ///     If a value is provided, a lightweight "contains" search is applied to the following key-properties:
        ///     <br/><br/>
        ///     - Name
        /// </param>
        /// <param name="partnerId"> If provided, the results will be filtered to only include organizations belonging to this partner. </param>
        /// <param name="includePreferences"> When <c><see langword="true"/></c>, the <c>Preferences</c> property is
        ///     loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para></param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The highest number of items that can be added in a single page. </param>
        /// <returns> The asynchronous task. The task results contains the corresponding <see cref="ActionResult{TValue}"/>. </returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedModel<OrganizationDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrganizationDTO>>> GetPaginatedOrganizationListAsync(CancellationToken cancellationToken, bool hierarchical = false, [FromQuery] bool customersOnly = false, [FromQuery(Name = "q")][SwaggerSchema(Nullable = true)] string? search = null, [FromQuery][SwaggerSchema(Nullable = true)] Guid? partnerId = null, [FromQuery] bool includePreferences = false, [FromQuery] int page = 1, [FromQuery] int limit = 25)
        {
            try
            {
                var pagedOrganizations = await _organizationServices.GetPaginatedOrganizationsAsync(cancellationToken, page, limit, includePreferences, hierarchical: hierarchical, customersOnly: customersOnly, search: search, partnerId: partnerId);

                var returnModel = new PagedModel<OrganizationDTO>()
                {
                    CurrentPage = pagedOrganizations.CurrentPage,
                    Items = _mapper.Map<IList<OrganizationDTO>>(pagedOrganizations.Items),
                    PageSize = pagedOrganizations.PageSize,
                    TotalItems = pagedOrganizations.TotalItems,
                    TotalPages = pagedOrganizations.TotalPages
                };

                return Ok(returnModel);
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
        public async Task<ActionResult<IList<CustomerServices.Models.OrganizationUserCount>>> GetOrganizationUserCountAsync([FromQuery(Name = "filterOptions")] string? filterOptionsAsJsonString)
        {
            try
            {
                FilterOptionsForUser? filterOptions = null;
                if (!string.IsNullOrEmpty(filterOptionsAsJsonString))
                {
                    filterOptions = JsonSerializer.Deserialize<FilterOptionsForUser>(filterOptionsAsJsonString);
                }


                var organizationUserCount = await _organizationServices.GetOrganizationUserCountAsync(filterOptions?.PartnerId, filterOptions?.AssignedToDepartments);
                if (organizationUserCount == null || organizationUserCount.Count == 0)
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
                organization.Address = organization.Address == null ? new AddressDTO() : organization.Address;
                organization.Address.Street = string.IsNullOrEmpty(organization.Address.Street) ? "" : organization.Address.Street;
                organization.Address.PostCode = string.IsNullOrEmpty(organization.Address.PostCode) ? "" : organization.Address.PostCode;
                organization.Address.City = string.IsNullOrEmpty(organization.Address.City) ? "" : organization.Address.City;
                organization.Address.Country = string.IsNullOrEmpty(organization.Address.Country) ? "" : organization.Address.Country;

                // Get ContactPerson values
                bool contactpersonIsNull = (organization.ContactPerson == null);
                organization.ContactPerson = organization.ContactPerson == null ? new ContactPersonDTO() : organization.ContactPerson;
                organization.ContactPerson.FirstName = string.IsNullOrEmpty(organization.ContactPerson.FirstName) ? "" : organization.ContactPerson.FirstName;
                organization.ContactPerson.LastName = string.IsNullOrEmpty(organization.ContactPerson.LastName) ? "" : organization.ContactPerson.LastName;
                organization.ContactPerson.Email = string.IsNullOrEmpty(organization.ContactPerson.Email) ? "" : organization.ContactPerson.Email;
                organization.ContactPerson.PhoneNumber = string.IsNullOrEmpty(organization.ContactPerson.PhoneNumber) ? "" : organization.ContactPerson.PhoneNumber;

                var updatedData = _mapper.Map<UpdateOrganizationDTO>(organization);
                // Update
                var updatedOrganization = await _organizationServices.PutOrganizationAsync(updatedData);

                return Ok(_mapper.Map<OrganizationDTO>(updatedOrganization));

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
        public async Task<ActionResult<OrganizationDTO>> UpdateOrganizationPatch([FromRoute] Guid organizationId, [FromBody] UpdateOrganization organization)
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
                var updatedOrganization = await _organizationServices.PatchOrganizationAsync(organizationId, organization.ParentId, organization.PrimaryLocation, organization.CallerId,
                                                           organization.Name, organization.OrganizationNumber, street, postCode, city, country, firstName, lastName, email, phoneNumber, organization.LastDayForReportingSalaryDeduction, organization.PayrollContactEmail, organization.AddUsersToOkta ?? default);

                return Ok(_mapper.Map<OrganizationDTO>(updatedOrganization));
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

        [Route("techstep-core-update")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> UpdateOrganizationTechstepCore([FromBody] TechstepCoreCustomerUpdate updatedTechstepData)
        {
            try
            {
                var mappedOrganization = _mapper.Map<TechstepCoreCustomerUpdateDTO>(updatedTechstepData);
                await _organizationServices.UpdateOrganizationTechstepCoreAsync(mappedOrganization);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateOrganizationTechstepCore: failed to update: " + ex.Message);
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);                         //Maybe not return notting else but OK? Does it have an effekt
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
                    Location = (removedOrganization.PrimaryLocation == null) ? null : new LocationDTO(removedOrganization.PrimaryLocation),
                    PartnerId = removedOrganization.Partner?.ExternalId
                };
                return Ok(_mapper.Map<OrganizationDTO>(removedOrganization));

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
        public async Task<ActionResult<OrganizationPreferencesDTO>> GetOrganizationPreferences([FromRoute] Guid organizationId)
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
        [ProducesResponseType(typeof(OrganizationPreferencesDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationPreferencesDTO>> UpdateOrganizationPreferencesPatch([FromBody] UpdateOrganizationPreferences preferences)
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

        [Route("{organizationId:Guid}/preferences")]
        [HttpPut]
        [ProducesResponseType(typeof(OrganizationPreferencesDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationPreferencesDTO>> UpdateOrganizationPreferencesPut([FromBody] UpdateOrganizationPreferences preferences)
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

        [Route("location")]
        [HttpPut]
        [ProducesResponseType(typeof(LocationDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<LocationDTO>> UpdateOrganizationLocation([FromBody] UpdateLocation location)
        {

            if (location.LocationId == Guid.Empty)
                return BadRequest("Location id cannot be empty.");

            var newLocation = new CustomerServices.Models.Location(location.Name,
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

        [Route("{organizationId:Guid}/location")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<LocationDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IList<LocationDTO>>> GetAllLocationInOrganization(Guid organizationId)
        {
            var locations = await _organizationServices.GetAllLocationInOrganization(organizationId);
            return Ok(locations);
        }

        [Route("{organizationId:Guid}/location")]
        [HttpPost]
        [ProducesResponseType(typeof(LocationDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CreateLocationInOrganization([FromBody] NewLocation location, Guid organizationId)
        {
            if (location == null)
                return NotFound();

            var newLocation = _mapper.Map<NewLocationDTO>(location);
            var addedLocation = await _organizationServices.AddLocationInOrganization(newLocation, organizationId, location.CallerId);

            return CreatedAtAction(nameof(CreateLocationInOrganization), addedLocation);
        }
        [Route("{locationId:Guid}/location")]
        [HttpDelete]
        [ProducesResponseType(typeof(LocationDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<LocationDTO>> DeleteLocationAsync(Guid locationId, [FromBody] DeleteContent deleteData)
        {
            if (locationId == Guid.Empty)
                return BadRequest("Location id cannot be empty");

            var location = await _organizationServices.GetLocationAsync(locationId);

            if (location == null)
                return NotFound();

            await _organizationServices.DeleteOrganizationLocationAsync(locationId, deleteData.CallerId, deleteData.hardDelete);

            var deletedLocationView = new LocationDTO(location);

            return Ok(deletedLocationView);
        }
        /// <summary>
        /// Changes the customers status to StartedOnboardning.
        /// </summary>
        /// <param name="organizationId">Organization identification for the organization that has started the onboarding process.</param>
        /// <returns>The organization object that has started the onbarding process.</returns>
        [Route("{organizationId:Guid}/initiate-onboarding")]
        [HttpPost]
        [ProducesResponseType(typeof(OrganizationDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<OrganizationDTO>> InitiateOnboardingAsync(Guid organizationId)
        {
            var organization = await _organizationServices.InitiateOnboardingAsync(organizationId);
            return Ok(_mapper.Map<OrganizationDTO>(organization));
        }

        /// <summary>
        /// Retrieves the hashed apikey saved on the customer.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/hashed-apikey")]
        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<string>> GetHashedApiKey([FromRoute] Guid organizationId, CancellationToken cancellationToken)
        {
            try
            {
                var hashedApiKey = await _organizationServices.GetHashedApiKeyAsync(organizationId, cancellationToken);

                return string.IsNullOrEmpty(hashedApiKey) ? NotFound() : Ok(hashedApiKey);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error - Get hashed api key: " + ex.Message);
            }
        }

        /// <summary>
        /// Hashes an api key and saves it on the organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="apiKey">The api key used by this organization</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [Route("{organizationId:Guid}/hashed-apikey")]
        [HttpPost]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<string>> SaveApiKey([FromRoute] Guid organizationId, CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _organizationServices.GenerateAndSaveHashedApiKeyAsync(organizationId, cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error - Unable to generate and save hashed api key: " + ex.Message);
            }
        }
    }
}