﻿using Common.Enums;
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

        [Route("{organizationId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Gone)]
        public async Task<ActionResult<Organization>> Get(Guid organizationId, bool includeOrganizationPreferences = true, bool includeLocation = true)
        {
            try
            {
                var organization = await _organizationServices.GetOrganizationAsync(organizationId, includeOrganizationPreferences, includeLocation);
                if (organization == null) return NotFound();

                var foundCustomer = new Organization
                {
                    Id = organization.OrganizationId,
                    OrganizationName = organization.OrganizationName,
                    OrganizationNumber = organization.OrganizationNumber,
                    OrganizationAddress = new Address(organization.OrganizationAddress),
                    OrganizationContactPerson = new ContactPerson(organization.OrganizationContactPerson),
                    OrganizationPreferences = (organization.OrganizationPreferences == null) ? null : new OrganizationPreferences(organization.OrganizationPreferences),
                    OrganizationLocation = (organization.OrganizationLocation == null) ? null : new Location(organization.OrganizationLocation)
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
        [ProducesResponseType(typeof(IEnumerable<Organization>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Organization>>> Get(bool hierarchical = false)
        {
            try
            {
                var organizations = await _organizationServices.GetOrganizationsAsync(hierarchical);
                IList<Organization> list = new List<Organization>();

                foreach (CustomerServices.Models.Organization org in organizations)
                {
                    var organizationView = new Organization
                    {
                        Id = org.OrganizationId,
                        OrganizationName = org.OrganizationName,
                        OrganizationNumber = org.OrganizationNumber,
                        OrganizationAddress = new Address(org.OrganizationAddress),
                        OrganizationContactPerson = new ContactPerson(org.OrganizationContactPerson),
                        OrganizationPreferences = (org.OrganizationPreferences == null) ? null : new OrganizationPreferences(org.OrganizationPreferences),
                        OrganizationLocation = (org.OrganizationLocation == null) ? null : new Location(org.OrganizationLocation),
                        ChildOrganizations = new List<Organization>()
                    };
                    if (org.ChildOrganizations != null)
                    {
                        foreach (CustomerServices.Models.Organization childOrg in org.ChildOrganizations)
                        {
                            var childOrgView = new Organization
                            {
                                Id = childOrg.OrganizationId,
                                OrganizationName = childOrg.OrganizationName,
                                OrganizationNumber = childOrg.OrganizationNumber,
                                OrganizationAddress = new Address(childOrg.OrganizationAddress),
                                OrganizationContactPerson = new ContactPerson(childOrg.OrganizationContactPerson),
                                OrganizationPreferences = (childOrg.OrganizationPreferences == null) ? null : new OrganizationPreferences(childOrg.OrganizationPreferences),
                                OrganizationLocation = (childOrg.OrganizationLocation == null) ? null : new Location(childOrg.OrganizationLocation)
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

        [HttpPost]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.Created)]
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
                else if (organization.OrganizationLocation == null)
                {
                    organizationLocation = new CustomerServices.Models.Location(Guid.NewGuid(), organization.CallerId, organization.OrganizationName, organization.InternalNotes,
                                                                                organization.OrganizationAddress.Street, organization.OrganizationAddress.Street,
                                                                                organization.OrganizationAddress.PostCode, organization.OrganizationAddress.City,
                                                                                organization.OrganizationAddress.Country);
                    await _organizationServices.AddOrganizationLocationAsync(organizationLocation);
                }
                else
                {
                    organizationLocation = new CustomerServices.Models.Location(Guid.NewGuid(), organization.CallerId, organization.OrganizationLocation.Name, organization.OrganizationLocation.Description,
                                                                                organization.OrganizationLocation.Address1, organization.OrganizationLocation.Address2,
                                                                                organization.OrganizationLocation.PostalCode, organization.OrganizationLocation.City,
                                                                                organization.OrganizationLocation.Country);

                    // only save the location if it does not already exist
                    await _organizationServices.AddOrganizationLocationAsync(organizationLocation);
                }

                // Create entities from NewOrganization to reduce the number of fields required to make them in OrganizationServices
                var organizationContactPerson = new CustomerServices.Models.ContactPerson(organization.OrganizationContactPerson?.FullName, organization.OrganizationContactPerson?.Email,
                                                                                          organization.OrganizationContactPerson?.PhoneNumber);

                var organizationAddress = new CustomerServices.Models.Address(organization.OrganizationAddress?.Street, organization.OrganizationAddress?.PostCode,
                                                                              organization.OrganizationAddress?.City, organization.OrganizationAddress?.Country);

                var newOrganization = new CustomerServices.Models.Organization(Guid.NewGuid(), organization.CallerId, organization.ParentId,
                                                                               organization.OrganizationName, organization.OrganizationNumber,
                                                                               organizationAddress, organizationContactPerson,
                                                                               null, organizationLocation);

                // organizationPreferences needs the OrganizationId from newOrganization, and is therefore made last
                if (organization.OrganizationPreferences != null)
                {
                    var organizationPreferences = new CustomerServices.Models.OrganizationPreferences(newOrganization.OrganizationId, newOrganization.CreatedBy, organization.OrganizationPreferences?.WebPage,
                                                                                                 organization.OrganizationPreferences?.LogoUrl, organization.OrganizationPreferences?.OrganizationNotes,
                                                                                                 organization.OrganizationPreferences.EnforceTwoFactorAuth, organization.OrganizationPreferences?.PrimaryLanguage,
                                                                                                 organization.OrganizationPreferences.DefaultDepartmentClassification);

                    // save the organization preferences
                    await _organizationServices.AddOrganizationPreferencesAsync(organizationPreferences);
                    newOrganization.OrganizationPreferences = organizationPreferences;
                }
                else // Create an empty "default" variant of organization preferences
                {
                    var organizationPreferences = new CustomerServices.Models.OrganizationPreferences(newOrganization.OrganizationId, newOrganization.CreatedBy, "",
                                                                                                 "", "", false, "EN", 0);

                    // save the organization preferences
                    await _organizationServices.AddOrganizationPreferencesAsync(organizationPreferences);
                    newOrganization.OrganizationPreferences = organizationPreferences;
                }

                // Save new organization
                var updatedOrganization = await _organizationServices.AddOrganizationAsync(newOrganization);

                var updatedOrganizationView = new Organization
                {
                    Id = updatedOrganization.OrganizationId,
                    OrganizationName = updatedOrganization.OrganizationName,
                    OrganizationNumber = updatedOrganization.OrganizationNumber,
                    OrganizationAddress = new Address(updatedOrganization.OrganizationAddress),
                    OrganizationContactPerson = new ContactPerson(updatedOrganization.OrganizationContactPerson),
                    OrganizationPreferences = (updatedOrganization.OrganizationPreferences == null) ? null : new OrganizationPreferences(updatedOrganization.OrganizationPreferences),
                    OrganizationLocation = (updatedOrganization.OrganizationLocation == null) ? null : new Location(updatedOrganization.OrganizationLocation)
                };

                return CreatedAtAction(nameof(CreateOrganization), new { id = updatedOrganizationView.Id }, updatedOrganizationView);
            }
            catch (Exception ex)
            {
                return BadRequest("Unknown error: " + ex.Message);
            }
        }

        [Route("{organizationId:Guid}/organization")]
        [HttpPut]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Organization>> UpdateOrganization([FromBody] UpdateOrganization organization)
        {
            try
            {
                if (organization.OrganizationId == Guid.Empty)
                {
                    return BadRequest("OrganizationId cannot be empty.");
                }

                var organizationOriginal = await _organizationServices.GetOrganizationAsync(organization.OrganizationId, true, true);

                if (organizationOriginal == null)
                {
                    return BadRequest("Organization with the given id was not found.");
                }

                if (organization.ParentId != Guid.Empty)
                {
                    var organizationParent = await _organizationServices.GetOrganizationAsync(organization.ParentId, false, false);
                    if (organizationParent.ParentId != Guid.Empty)
                    {
                        return BadRequest("Parent of the organization cannot itself have a parent.");
                    }
                }

                // Check if location already exists, or if we must make a new one
                CustomerServices.Models.Location newLocation;
                if (organization.PrimaryLocation != Guid.Empty)
                {
                    var location = await _organizationServices.GetLocationAsync(organization.PrimaryLocation);
                    if (location == null)
                    {
                        return BadRequest("No location was found with the given id.");
                    }
                    else
                    {
                        newLocation = location;
                    }
                }
                else
                {
                    if (organization.OrganizationLocation == null)
                    {
                        return BadRequest("An organization must have a location object if PrimaryLocation is empty.");
                    }
                    else
                    {
                        newLocation = new CustomerServices.Models.Location(Guid.NewGuid(), organization.CallerId, organization.OrganizationLocation.Name, organization.OrganizationLocation.Description,
                                                                           organization.OrganizationLocation.Address1, organization.OrganizationLocation.Address2,
                                                                           organization.OrganizationLocation.PostalCode, organization.OrganizationLocation.City,
                                                                           organization.OrganizationLocation.Country);
                    }
                }

                CustomerServices.Models.Address newAddress;
                if (organization.OrganizationAddress == null)
                {
                    newAddress = new CustomerServices.Models.Address("", "", "", "");
                }
                else
                {
                    newAddress = new CustomerServices.Models.Address(organization.OrganizationAddress.Street, organization.OrganizationAddress.PostCode,
                                                                     organization.OrganizationAddress.City, organization.OrganizationAddress.Country);
                }

                CustomerServices.Models.ContactPerson newContactPerson;
                if (organization.OrganizationContactPerson == null)
                {
                    newContactPerson = new CustomerServices.Models.ContactPerson("", "", "");
                }
                else
                {
                    newContactPerson = new CustomerServices.Models.ContactPerson(organization.OrganizationContactPerson.FullName,
                                                                                 organization.OrganizationContactPerson.Email,
                                                                                 organization.OrganizationContactPerson.PhoneNumber);
                }

                CustomerServices.Models.OrganizationPreferences newOrganizationPreferences;
                if (organization.OrganizationPreferences == null)
                {
                    newOrganizationPreferences = new CustomerServices.Models.OrganizationPreferences(organization.OrganizationId, organization.CallerId, "", "", "", false, "", 0);
                }
                else
                {
                    newOrganizationPreferences = new CustomerServices.Models.OrganizationPreferences(organization.OrganizationId, organization.CallerId, organization.OrganizationPreferences.WebPage,
                                                                                                     organization.OrganizationPreferences.LogoUrl, organization.OrganizationPreferences.OrganizationNotes,
                                                                                                     organization.OrganizationPreferences.EnforceTwoFactorAuth, organization.OrganizationPreferences.PrimaryLanguage,
                                                                                                     organization.OrganizationPreferences.DefaultDepartmentClassification);
                }

                await _organizationServices.UpdateOrganizationPreferencesAsync(newOrganizationPreferences);
                await _organizationServices.UpdateOrganizationLocationAsync(newLocation);

                CustomerServices.Models.Organization newOrganization = new CustomerServices.Models.Organization(organization.OrganizationId, organization.CallerId, organization.ParentId,
                                                                                        organization.OrganizationName, organization.OrganizationNumber,
                                                                                        newAddress, newContactPerson,
                                                                                        newOrganizationPreferences, newLocation);

                var updatedOrganization = await _organizationServices.UpdateOrganizationAsync(newOrganization);

                var updatedOrganizationView = new Organization
                {
                    Id = updatedOrganization.OrganizationId,
                    OrganizationName = updatedOrganization.OrganizationName,
                    OrganizationNumber = updatedOrganization.OrganizationNumber,
                    OrganizationAddress = new Address(updatedOrganization.OrganizationAddress),
                    OrganizationContactPerson = new ContactPerson(updatedOrganization.OrganizationContactPerson),
                    OrganizationPreferences = new OrganizationPreferences(newOrganization.OrganizationPreferences),
                    OrganizationLocation = new Location(newOrganization.OrganizationLocation)
                };

                return updatedOrganizationView;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationsController - UpdateOrganization - unknown error: " + ex.Message);
                return BadRequest("OrganizationsController - UpdateOrganization - unknown error: " + ex.Message);
            }
        }

        [Route("{organizationId:Guid}/organization")]
        [HttpPost]
        [ProducesResponseType(typeof(Organization), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Organization>> PatchOrganization([FromBody] UpdateOrganization organization)
        {
            try
            {
                if (organization.OrganizationId == Guid.Empty)
                {
                    return BadRequest("OrganizationId cannot be empty.");
                }

                var organizationOriginal = await _organizationServices.GetOrganizationAsync(organization.OrganizationId, true, true);

                if (organizationOriginal == null)
                {
                    return BadRequest("Organization with the given id was not found.");
                }

                if (organization.ParentId != Guid.Empty)
                {
                    var organizationParent = await _organizationServices.GetOrganizationAsync(organization.ParentId, false, false);
                    if (organizationParent.ParentId != Guid.Empty)
                    {
                        return BadRequest("Parent of the organization cannot itself have a parent.");
                    }
                }

                // Check if location already exists, or if we must make a new one
                CustomerServices.Models.Location newLocation;
                if (organization.PrimaryLocation != Guid.Empty)
                {
                    var location = await _organizationServices.GetLocationAsync(organization.PrimaryLocation);
                    if (location == null)
                    {
                        return BadRequest("No location was found with the given id.");
                    }
                    else
                    {
                        newLocation = location;
                    }
                }
                else
                {
                    if (organization.OrganizationLocation == null)
                    {
                        return BadRequest("An organization must have a location object if PrimaryLocation is empty.");
                    }
                    else
                    {
                        newLocation = new CustomerServices.Models.Location(Guid.NewGuid(), organization.CallerId, organization.OrganizationLocation.Name, organization.OrganizationLocation.Description,
                                                                           organization.OrganizationLocation.Address1, organization.OrganizationLocation.Address2,
                                                                           organization.OrganizationLocation.PostalCode, organization.OrganizationLocation.City,
                                                                           organization.OrganizationLocation.Country);
                    }
                }

                CustomerServices.Models.Address newAddress;
                if (organization.OrganizationAddress == null)
                {
                    newAddress = new CustomerServices.Models.Address("", "", "", "");
                }
                else
                {
                    newAddress = new CustomerServices.Models.Address(organization.OrganizationAddress.Street, organization.OrganizationAddress.PostCode, organization.OrganizationAddress.City, organization.OrganizationAddress.Country);
                }

                CustomerServices.Models.ContactPerson newContactPerson;
                if (organization.OrganizationContactPerson == null)
                {
                    newContactPerson = new CustomerServices.Models.ContactPerson("", "", "");
                }
                else
                {
                    newContactPerson = new CustomerServices.Models.ContactPerson(organization.OrganizationContactPerson.FullName, organization.OrganizationContactPerson.Email, organization.OrganizationContactPerson.PhoneNumber);
                }

                CustomerServices.Models.OrganizationPreferences newOrganizationPreferences;
                if (organization.OrganizationPreferences == null)
                {
                    newOrganizationPreferences = new CustomerServices.Models.OrganizationPreferences(organization.OrganizationId, organization.CallerId, "", "", "", false, "", 0);
                }
                else
                {
                    newOrganizationPreferences = new CustomerServices.Models.OrganizationPreferences(organization.OrganizationId, organization.CallerId, organization.OrganizationPreferences.WebPage,
                                                                                                     organization.OrganizationPreferences.LogoUrl, organization.OrganizationPreferences.OrganizationNotes,
                                                                                                     organization.OrganizationPreferences.EnforceTwoFactorAuth, organization.OrganizationPreferences.PrimaryLanguage,
                                                                                                     organization.OrganizationPreferences.DefaultDepartmentClassification);
                }

                await _organizationServices.UpdateOrganizationPreferencesAsync(newOrganizationPreferences, true);
                await _organizationServices.UpdateOrganizationLocationAsync(newLocation, true);

                CustomerServices.Models.Organization newOrganization = new CustomerServices.Models.Organization(organization.OrganizationId, organization.CallerId, organization.ParentId,
                                                                                        organization.OrganizationName, organization.OrganizationNumber,
                                                                                        newAddress, newContactPerson,
                                                                                        newOrganizationPreferences, newLocation);

                var updatedOrganization = await _organizationServices.UpdateOrganizationAsync(newOrganization, true);
                var updatedOrganizationView = new Organization
                {
                    Id = updatedOrganization.OrganizationId,
                    OrganizationName = updatedOrganization.OrganizationName,
                    OrganizationNumber = updatedOrganization.OrganizationNumber,
                    OrganizationAddress = new Address(updatedOrganization.OrganizationAddress),
                    OrganizationContactPerson = new ContactPerson(updatedOrganization.OrganizationContactPerson),
                    OrganizationPreferences = new OrganizationPreferences(newOrganization.OrganizationPreferences),
                    OrganizationLocation = new Location(newOrganization.OrganizationLocation)
                };

                return updatedOrganizationView;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationsController - UpdateOrganization - unknown error: " + ex.Message);
                return BadRequest("OrganizationsController - UpdateOrganization - unknown error: " + ex.Message);
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
                    Id = removedOrganization.OrganizationId,
                    OrganizationName = removedOrganization.OrganizationName,
                    OrganizationNumber = removedOrganization.OrganizationNumber,
                    OrganizationAddress = new Address(removedOrganization.OrganizationAddress),
                    OrganizationContactPerson = new ContactPerson(removedOrganization.OrganizationContactPerson),
                    OrganizationPreferences = (removedOrganization.OrganizationPreferences == null) ? null : new OrganizationPreferences(removedOrganization.OrganizationPreferences),
                    OrganizationLocation = (removedOrganization.OrganizationLocation == null) ? null : new Location(removedOrganization.OrganizationLocation)
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

        [Route("preferences")]
        [HttpPut]
        [ProducesResponseType(typeof(OrganizationPreferences), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrganizationPreferences>> UpdateOrganizationPreferences([FromBody] UpdateOrganizationPreferences preferences)
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

        [Route("{customerId:Guid}/assetCategory")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AssetCategoryType>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<AssetCategoryType>>> GetAllCustomerAssetCategory(Guid customerId)
        {
            var assetCategoryLifecycleTypes = await _organizationServices.GetAssetCategoryTypes(customerId);
            if (assetCategoryLifecycleTypes == null) return NotFound();
            var customerAssetCategories = new List<AssetCategoryType>();
            customerAssetCategories.AddRange(assetCategoryLifecycleTypes.Select(category => new AssetCategoryType
            {
                CustomerId = category.ExternalCustomerId,
                AssetCategoryId = category.AssetCategoryId,
                LifecycleTypes = category.LifecycleTypes.Select(a => new AssetCategoryLifecycleType()
                {
                    CustomerId = a.CustomerId,
                    AssetCategoryId = a.AssetCategoryId,
                    Name = Enum.GetName(typeof(LifecycleType), a.LifecycleType),
                    LifecycleType = a.LifecycleType
                }).ToList()
            }));
            return Ok(customerAssetCategories);
        }

        [Route("{customerId:Guid}/assetCategory")]
        [HttpPost]
        [ProducesResponseType(typeof(AssetCategoryType), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<AssetCategoryType>> AddCustomerAssetCategory(Guid customerId, UpdateAssetCategoryType addedAssetCategory)
        {
            try
            {
                foreach (int lifecycle in addedAssetCategory.LifecycleTypes)
                {
                    // Check if given int is within valid range of values
                    if (!Enum.IsDefined(typeof(LifecycleType), lifecycle))
                    {
                        Array arr = Enum.GetValues(typeof(LifecycleType));
                        StringBuilder errorMessage = new StringBuilder(string.Format("The given value for lifecycle: {0} is out of bounds.\nValid options for lifecycle are:\n", lifecycle));
                        foreach (LifecycleType e in arr)
                        {
                            errorMessage.Append($"    -{(int)e} ({e})\n");
                        }
                        throw new InvalidLifecycleTypeException(errorMessage.ToString());
                    }
                }
                var assetCategories = await _organizationServices.AddAssetCategoryType(customerId, addedAssetCategory.AssetCategoryId, addedAssetCategory.LifecycleTypes);
                var assetCategoryView = new AssetCategoryType
                {
                    CustomerId = assetCategories.ExternalCustomerId,
                    AssetCategoryId = assetCategories.AssetCategoryId,
                    LifecycleTypes = assetCategories.LifecycleTypes.Select(a => new AssetCategoryLifecycleType()
                    {
                        CustomerId = a.CustomerId,
                        AssetCategoryId = a.AssetCategoryId,
                        Name = Enum.GetName(typeof(LifecycleType), a.LifecycleType),
                        LifecycleType = a.LifecycleType
                    }).ToList()
                };
                return Ok(assetCategoryView);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/assetCategory")]
        [HttpDelete]
        [ProducesResponseType(typeof(AssetCategoryType), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<AssetCategoryType>> RemoveCustomerAssetCategory(Guid customerId, UpdateAssetCategoryType deleteAssetCategory)
        {
            try
            {
                foreach (int lifecycle in deleteAssetCategory.LifecycleTypes)
                {
                    // Check if given int is within valid range of values
                    if (!Enum.IsDefined(typeof(LifecycleType), lifecycle))
                    {
                        Array arr = Enum.GetValues(typeof(LifecycleType));
                        StringBuilder errorMessage = new StringBuilder(string.Format("The given value for lifecycle: {0} is out of bounds.\nValid options for lifecycle are:\n", lifecycle));
                        foreach (LifecycleType e in arr)
                        {
                            errorMessage.Append($"    -{(int)e} ({e})\n");
                        }
                        throw new InvalidLifecycleTypeException(errorMessage.ToString());
                    }
                }
                var assetCategories = await _organizationServices.RemoveAssetCategoryType(customerId, deleteAssetCategory.AssetCategoryId, deleteAssetCategory.LifecycleTypes);
                if (assetCategories == null)
                    return NotFound();
                var assetCategoryView = new AssetCategoryType
                {
                    CustomerId = assetCategories.ExternalCustomerId,
                    AssetCategoryId = assetCategories.AssetCategoryId,
                    LifecycleTypes = assetCategories.LifecycleTypes.Select(a => new AssetCategoryLifecycleType()
                    {
                        CustomerId = a.CustomerId,
                        AssetCategoryId = a.AssetCategoryId,
                        Name = Enum.GetName(typeof(LifecycleType), a.LifecycleType),
                        LifecycleType = a.LifecycleType
                    }).ToList()
                };
                return Ok(assetCategoryView);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("{customerId:Guid}/modules")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<ProductModule>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<ProductModule>>> GetCustomerProductModules(Guid customerId)
        {
            var productGroup = await _organizationServices.GetCustomerProductModulesAsync(customerId);
            if (productGroup == null) return NotFound();
            var customerProductModules = new List<ProductModule>();
            customerProductModules.AddRange(productGroup.Select(module => new ProductModule
            {
                ProductModuleId = module.ProductModuleId,
                Name = module.Name,
                ProductModuleGroup = module.ProductModuleGroup.Select(groups => new ProductModuleGroup
                {
                    Name = groups.Name,
                    ProductModuleGroupId = groups.ProductModuleGroupId
                }).ToList()
            }));

            return Ok(customerProductModules);
        }

        [Route("{customerId:Guid}/modules")]
        [HttpPost]
        [ProducesResponseType(typeof(ProductModule), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModule>> AddCustomerProductModules(Guid customerId, UpdateProductModule productModule)
        {
            var productGroup = await _organizationServices.AddProductModulesAsync(customerId, productModule.ProductModuleId, productModule.ProductModuleGroupIds);
            if (productGroup == null) return NotFound();
            var moduleGroup = new ProductModule
            {
                ProductModuleId = productGroup.ProductModuleId,
                Name = productGroup.Name,
                ProductModuleGroup = productGroup.ProductModuleGroup.Select(groups => new ProductModuleGroup
                {
                    Name = groups.Name,
                    ProductModuleGroupId = groups.ProductModuleGroupId
                }).ToList()
            };

            return Ok(moduleGroup);
        }

        [Route("{customerId:Guid}/modules")]
        [HttpDelete]
        [ProducesResponseType(typeof(ProductModule), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductModule>> RemoveCustomerModules(Guid customerId, UpdateProductModule productModule)
        {
            var productGroup = await _organizationServices.RemoveProductModulesAsync(customerId, productModule.ProductModuleId, productModule.ProductModuleGroupIds);
            if (productGroup == null) return NoContent();
            var moduleGroup = new ProductModule
            {
                ProductModuleId = productGroup.ProductModuleId,
                Name = productGroup.Name,
                ProductModuleGroup = productGroup.ProductModuleGroup.Select(groups => new ProductModuleGroup
                {
                    Name = groups.Name,
                    ProductModuleGroupId = groups.ProductModuleGroupId
                }).ToList()
            };

            return Ok(moduleGroup);
        }
    }
}