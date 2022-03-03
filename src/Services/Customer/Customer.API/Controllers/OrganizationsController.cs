using Common.Enums;
using Common.Exceptions;
using Customer.API.ViewModels;
using CustomerServices;
using CustomerServices.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Customer.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [SuppressMessage("ReSharper", "RouteTemplates.RouteParameterConstraintNotResolved")]
    [SuppressMessage("ReSharper", "RouteTemplates.ControllerRouteParameterIsNotPassedToMethods")]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationServices _organizationServices;
        private readonly ILogger<OrganizationsController> _logger;

        public OrganizationsController(ILogger<OrganizationsController> logger, IOrganizationServices customerServices)
        {
            _logger = logger;
            _organizationServices = customerServices;
        }

        [Route("{organizationId:Guid}/{customerOnly:Bool}")]
        [HttpGet]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Gone)]
        public async Task<ActionResult<Organization>> Get(Guid organizationId, bool includeOrganizationPreferences = true, bool includeLocation = true, bool customerOnly = false)
        {
            try
            {
                var organization = await _organizationServices.GetOrganizationAsync(organizationId, includeOrganizationPreferences, includeLocation, customerOnly);
                if (organization == null) return NotFound();

                var foundCustomer = new Organization
                {
                    OrganizationId = organization.OrganizationId,
                    Name = organization.Name,
                    OrganizationNumber = organization.OrganizationNumber,
                    Address = new Address(organization.Address),
                    ContactPerson = new ContactPerson(organization.ContactPerson),
                    Preferences = (organization.Preferences == null) ? null : new OrganizationPreferences(organization.Preferences),
                    Location = (organization.Location == null) ? null : new Location(organization.Location)
                };
                return Ok(foundCustomer);
            }
            catch (EntityIsDeletedException ex)
            {
                _logger.LogError("Entity is deleted. {0}", ex.Message);
                return StatusCode((int)HttpStatusCode.Gone);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error - Get Organization (single): " + ex.Message);
            }
        }

        [HttpGet]
        [Route("{customersOnly:Bool}")]
        [ProducesResponseType(typeof(IEnumerable<Organization>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Organization>>> Get(bool hierarchical = false, bool customersOnly = false)
        {
            try
            {
                var organizations = await _organizationServices.GetOrganizationsAsync(hierarchical, customersOnly);
                IList<Organization> list = new List<Organization>();

                foreach (CustomerServices.Models.Organization org in organizations)
                {
                    var organizationView = new Organization
                    {
                        OrganizationId = org.OrganizationId,
                        Name = org.Name,
                        OrganizationNumber = org.OrganizationNumber,
                        Address = new Address(org.Address),
                        ContactPerson = new ContactPerson(org.ContactPerson),
                        Preferences = (org.Preferences == null) ? null : new OrganizationPreferences(org.Preferences),
                        Location = (org.Location == null) ? null : new Location(org.Location),
                        ChildOrganizations = new List<Organization>()
                    };
                    if (org.ChildOrganizations != null)
                    {
                        foreach (CustomerServices.Models.Organization childOrg in org.ChildOrganizations)
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
        [ProducesResponseType(typeof(IList<CustomerServices.Models.CustomerUserCount>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Gone)]
        public async Task<ActionResult<IList<CustomerServices.Models.CustomerUserCount>>> GetOrganizationUsers()
        {
            try
            {
                var customerUserCounts = await _organizationServices.GetCustomerUsersAsync();
                if (customerUserCounts == null)
                    return NotFound();

                return Ok(customerUserCounts);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error - Get Customer User counts (multiple): " + ex.Message + ex.StackTrace);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<Organization>> CreateOrganization([FromBody] NewOrganization organization)
        {
            try
            {
                // Check if organization.parentId is set to a valid value
                if (organization.ParentId != Guid.Empty)
                {
                    var parentOrg = await _organizationServices.GetOrganizationAsync(organization.ParentId, false, false);
                    if (parentOrg == null)
                        return BadRequest("Organization with given parentId does not exist.");
                    if (!(parentOrg.ParentId == null || parentOrg.ParentId == Guid.Empty))
                    {
                        return BadRequest("Parent organization cannot itself have a parent organization.");
                    }
                }

                // Check for organization number conflict
                if (await _organizationServices.GetOrganizationByOrganizationNumberAsync(organization.OrganizationNumber) != null)
                {
                    return Conflict($"Organization with organization number {organization.OrganizationNumber} already exists");
                }

                // Location
                CustomerServices.Models.Location organizationLocation;
                if (organization.PrimaryLocation != Guid.Empty)
                {
                    var loc = await _organizationServices.GetLocationAsync(organization.PrimaryLocation);

                    if (loc == null)
                    {
                        return BadRequest(string.Format("No Location object with the given Id was found: {0}", organization.PrimaryLocation));
                    }
                    else
                    {
                        organizationLocation = loc;
                    }
                }

                // Allow creation of organization without a given Location object. Create a new location object from data in OrganizationAddress.
                else if (organization.Location == null)
                {
                    organizationLocation = new CustomerServices.Models.Location(Guid.NewGuid(), organization.CallerId, organization.Name, organization.InternalNotes,
                                                                                organization.Address.Street, organization.Address.Street,
                                                                                organization.Address.PostCode, organization.Address.City,
                                                                                organization.Address.Country);

                    organizationLocation.SetFieldsToEmptyIfNull();
                    await _organizationServices.AddOrganizationLocationAsync(organizationLocation);
                }
                else
                {
                    organizationLocation = new CustomerServices.Models.Location(Guid.NewGuid(), organization.CallerId, organization.Location.Name, organization.Location.Description,
                                                                                organization.Location.Address1, organization.Location.Address2,
                                                                                organization.Location.PostalCode, organization.Location.City,
                                                                                organization.Location.Country);

                    organizationLocation.SetFieldsToEmptyIfNull();

                    // only save the location if it does not already exist
                    await _organizationServices.AddOrganizationLocationAsync(organizationLocation);
                }

                // Create entities from NewOrganization to reduce the number of fields required to make them in OrganizationServices
                var organizationContactPerson = new CustomerServices.Models.ContactPerson(organization.ContactPerson?.FirstName, organization.ContactPerson?.LastName, organization.ContactPerson?.Email,
                                                                                          organization.ContactPerson?.PhoneNumber);

                var organizationAddress = new CustomerServices.Models.Address(organization.Address?.Street, organization.Address?.PostCode,
                                                                              organization.Address?.City, organization.Address?.Country);

                Guid? parentId = (organization.ParentId == Guid.Empty) ? null : organization.ParentId;
                var newOrganization = new CustomerServices.Models.Organization(Guid.NewGuid(), organization.CallerId, parentId,
                                                                               organization.Name, organization.OrganizationNumber,
                                                                               organizationAddress, organizationContactPerson,
                                                                               null, organizationLocation, organization.IsCustomer);

                // organizationPreferences needs the OrganizationId from newOrganization, and is therefore made last
                if (organization.Preferences != null)
                {
                    if (organization.Preferences.EnforceTwoFactorAuth == null)
                        organization.Preferences.EnforceTwoFactorAuth = false;
                    if (organization.Preferences.DefaultDepartmentClassification == null)
                        organization.Preferences.DefaultDepartmentClassification = 0;

                    var organizationPreferences = new CustomerServices.Models.OrganizationPreferences(newOrganization.OrganizationId, newOrganization.CreatedBy, organization.Preferences?.WebPage,
                                                                                                 organization.Preferences?.LogoUrl, organization.Preferences?.OrganizationNotes,
                                                                                                 (bool)organization.Preferences.EnforceTwoFactorAuth, organization.Preferences?.PrimaryLanguage,
                                                                                                 (short)organization.Preferences.DefaultDepartmentClassification);

                    // save the organization preferences
                    await _organizationServices.AddOrganizationPreferencesAsync(organizationPreferences);
                    newOrganization.Preferences = organizationPreferences;
                }
                else // Create an empty "default" variant of organization preferences
                {
                    var organizationPreferences = new CustomerServices.Models.OrganizationPreferences(newOrganization.OrganizationId, newOrganization.CreatedBy, "",
                                                                                                 "", "", false, "EN", 0);

                    // save the organization preferences
                    await _organizationServices.AddOrganizationPreferencesAsync(organizationPreferences);
                    newOrganization.Preferences = organizationPreferences;
                }

                // Save new organization
                var updatedOrganization = await _organizationServices.AddOrganizationAsync(newOrganization);

                var updatedOrganizationView = new Organization
                {
                    OrganizationId = updatedOrganization.OrganizationId,
                    Name = updatedOrganization.Name,
                    OrganizationNumber = updatedOrganization.OrganizationNumber,
                    Address = new Address(updatedOrganization.Address),
                    ContactPerson = new ContactPerson(updatedOrganization.ContactPerson),
                    Preferences = (updatedOrganization.Preferences == null) ? null : new OrganizationPreferences(updatedOrganization.Preferences),
                    Location = (updatedOrganization.Location == null) ? null : new Location(updatedOrganization.Location)
                };

                return CreatedAtAction(nameof(CreateOrganization), new { id = updatedOrganizationView.OrganizationId }, updatedOrganizationView);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error: " + ex.Message);
            }
        }

        [Route("{organizationId:Guid}/organization")]
        [HttpPut]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Organization>> UpdateOrganizationPut([FromBody] UpdateOrganization organization)
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

                var updatedOrganizationView = new Organization
                {
                    OrganizationId = updatedOrganization.OrganizationId,
                    Name = updatedOrganization.Name,
                    OrganizationNumber = updatedOrganization.OrganizationNumber,
                    Address = new Address(updatedOrganization.Address),
                    ContactPerson = new ContactPerson(updatedOrganization.ContactPerson),
                    Preferences = (updatedOrganization.Preferences == null) ? null : new OrganizationPreferences(updatedOrganization.Preferences),
                    Location = (updatedOrganization.Location == null) ? null : new Location(updatedOrganization.Location)
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
                _logger.LogError("OrganizationController - UpdateOrganizationPut: failed to update: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [Route("{organizationId:Guid}/organization")]
        [HttpPost]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Organization>> UpdateOrganizationPatch([FromBody] UpdateOrganization organization)
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

                var updatedOrganizationView = new Organization
                {
                    OrganizationId = updatedOrganization.OrganizationId,
                    Name = updatedOrganization.Name,
                    OrganizationNumber = updatedOrganization.OrganizationNumber,
                    Address = new Address(updatedOrganization.Address),
                    ContactPerson = new ContactPerson(updatedOrganization.ContactPerson),
                    Preferences = (updatedOrganization.Preferences == null) ? null : new OrganizationPreferences(updatedOrganization.Preferences),
                    Location = (updatedOrganization.Location == null) ? null : new Location(updatedOrganization.Location)
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
                return BadRequest(ex.Message);
            }
        }

        [Route("{organizationId:Guid}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Organization>> DeleteOrganization([FromBody] DeleteOrganization deleteOrganization)
        {
            try
            {
                if (deleteOrganization.OrganizationId == Guid.Empty)
                    return BadRequest("Invalid given for organization");

                var removedOrganization = await _organizationServices.DeleteOrganizationAsync(deleteOrganization.OrganizationId, deleteOrganization.CallerId, deleteOrganization.HardDelete);

                var removedOrganizationView = new Organization
                {
                    OrganizationId = removedOrganization.OrganizationId,
                    Name = removedOrganization.Name,
                    OrganizationNumber = removedOrganization.OrganizationNumber,
                    Address = new Address(removedOrganization.Address),
                    ContactPerson = new ContactPerson(removedOrganization.ContactPerson),
                    Preferences = (removedOrganization.Preferences == null) ? null : new OrganizationPreferences(removedOrganization.Preferences),
                    Location = (removedOrganization.Location == null) ? null : new Location(removedOrganization.Location)
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
                return BadRequest("Unknown error (OrganizationsController - Delete Organization: " + ex.Message);
            }
        }

        [Route("{organizationId:Guid}/preferences")]
        [HttpGet]
        [ProducesResponseType(typeof(OrganizationPreferences), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Gone)]
        public async Task<ActionResult<OrganizationPreferences>> GetOrganizationPreferences(Guid organizationId)
        {
            try
            {
                var preferences = await _organizationServices.GetOrganizationPreferencesAsync(organizationId);
                if (preferences == null)
                    return NotFound();

                var preferencesView = new OrganizationPreferences(preferences);

                return Ok(preferencesView);
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
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationPreferences>> UpdateOrganizationPreferencesPatch([FromBody] UpdateOrganizationPreferences preferences)
        {
            try
            {
                if (preferences.OrganizationId == Guid.Empty)
                    return BadRequest("Organization Id cannot be empty");

                CustomerServices.Models.OrganizationPreferences newPreference = new CustomerServices.Models.OrganizationPreferences(preferences.OrganizationId,
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

                var updatedPreferencesView = new OrganizationPreferences(updatedPreferences);

                return Ok(updatedPreferencesView);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error: " + ex.Message);
            }
        }

        [Route("{organizationId:Guid}/preferences")]
        [HttpPut]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationPreferences>> UpdateOrganizationPreferencesPut([FromBody] UpdateOrganizationPreferences preferences)
        {
            try
            {
                if (preferences.OrganizationId == Guid.Empty)
                    return BadRequest("Organization Id cannot be empty");

                CustomerServices.Models.OrganizationPreferences newPreference = new CustomerServices.Models.OrganizationPreferences(preferences.OrganizationId,
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

                var updatedPreferencesView = new OrganizationPreferences(updatedPreferences);

                return Ok(updatedPreferencesView);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error: " + ex.Message);
            }
        }

        [Route("location")]
        [HttpPut]
        [ProducesResponseType(typeof(Location), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Location>> UpdateOrganizationLocation([FromBody] UpdateLocation location)
        {
            try
            {
                if (location.LocationId == Guid.Empty)
                    return BadRequest("Location id cannot be empty.");
                CustomerServices.Models.Location newLocation = new CustomerServices.Models.Location(location.LocationId,
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

                var updatedLocationView = new Location(updatedLocation);

                return Ok(updatedLocationView);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error: " + ex.Message);
            }
        }

        [Route("{locationId:Guid}/location")]
        [HttpDelete]
        [ProducesResponseType(typeof(Location), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Location>> DeleteLocation(Guid locationId, Guid callerId, bool hardDelete = false)
        {
            if (locationId == Guid.Empty)
                return BadRequest("Location id cannot be empty");

            var location = await _organizationServices.GetLocationAsync(locationId);

            if (location == null)
                return NotFound();

            await _organizationServices.DeleteOrganizationLocationAsync(locationId, callerId, hardDelete);

            var deletedLocationView = new Location(location);

            return Ok(deletedLocationView);
        }

    }
}