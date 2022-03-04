using Common.Cryptography;
using Common.Exceptions;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class OrganizationServices : IOrganizationServices
    {
        private readonly ILogger<OrganizationServices> _logger;
        private readonly IOrganizationRepository _customerRepository;

        public OrganizationServices(ILogger<OrganizationServices> logger, IOrganizationRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Returns all organizations. If hierarchical is true, return the organizations with no parent organization, these organizations will have their child organizations appended to them as a list.
        /// </summary>
        /// <param name="hierarchical"></param>
        /// <returns></returns>
        public async Task<IList<Organization>> GetOrganizationsAsync(bool hierarchical = false, bool customersOnly = false)
        {
            if (!hierarchical)
            {

                var orgs = (customersOnly) ? await _customerRepository.GetCustomersAsync() : await _customerRepository.GetOrganizationsAsync();
                foreach (Organization o in orgs)
                {
                    o.Preferences = await _customerRepository.GetOrganizationPreferencesAsync(o.OrganizationId);
                    o.Location = await _customerRepository.GetOrganizationLocationAsync(o.PrimaryLocation);
                }
                return orgs;
            }
            else
            {
                Guid? p = Guid.Empty;
                var organizations = (customersOnly) ? await _customerRepository.GetCustomersAsync(p) : await _customerRepository.GetOrganizationsAsync(p);
                foreach (Organization o in organizations)
                {
                    o.ChildOrganizations = await _customerRepository.GetOrganizationsAsync(o.OrganizationId);
                    o.Preferences = await _customerRepository.GetOrganizationPreferencesAsync(o.OrganizationId);
                    o.Location = await _customerRepository.GetOrganizationLocationAsync(o.PrimaryLocation);
                }
                return organizations;
            }
        }

        /// <summary>
        /// Returns all Organization entities with the given ParentId. If the ParentId is null, return all Organizations that do not have parent entities.
        /// </summary>
        /// <param name="parentId">Guid value that points to the ExternalId of an Organization</param>
        /// <returns>A list of Organizations</returns>
        public async Task<IList<Organization>> GetOrganizationsByParentId(Guid? parentId)
        {
            return await _customerRepository.GetOrganizationsAsync(parentId);
        }

        public async Task<Organization> GetOrganizationAsync(Guid customerId)
        {
            return await _customerRepository.GetOrganizationAsync(customerId);
        }

        /// <summary>
        /// Get the organization with the given Id. Optional: Return the OrganizationPreferences and OrganizationLocation object of the organization along with the organization itself.
        /// </summary>
        /// <param name="customerId">The id of the organization queried</param>
        /// <param name="includePreferences">Include OrganizationPreferences object of the organization if set to true</param>
        /// <param name="includeLocation">Include OrganizationLocation object of the organization if set to true</param>
        /// <returns>Organization</returns>
        public async Task<Organization> GetOrganizationAsync(Guid customerId, bool includePreferences = false, bool includeLocation = false, bool onlyCustomer = false)
        {
            var organization = (!onlyCustomer) ? await _customerRepository.GetOrganizationAsync(customerId) : await _customerRepository.GetCustomerAsync(customerId);

            if (organization != null)
            {
                if (includePreferences)
                {
                    organization.Preferences = await _customerRepository.GetOrganizationPreferencesAsync(customerId);
                }
                if (includeLocation)
                {
                    organization.Location = await _customerRepository.GetOrganizationLocationAsync(organization.PrimaryLocation);
                }
            }

            return organization;
        }

        public async Task<Organization> GetCustomerAsync(Guid customerId)
        {
            return await _customerRepository.GetCustomerAsync(customerId);
        }

        public async Task<Organization> GetOrganizationByOrganizationNumberAsync(string organizationNumber)
        {
            return await _customerRepository.GetOrganizationByOrganizationNumber(organizationNumber);
        }

        public async Task<IList<CustomerUserCount>> GetCustomerUsersAsync()
        {
            return await _customerRepository.GetOrganizationUserCountsAsync();
        }

        /// <summary>
        /// Add the given Organization to the database.
        /// </summary>
        /// <param name="newOrganization">An Organization entity, to be added to the database</param>
        /// <returns></returns>
        public async Task<Organization> AddOrganizationAsync(Organization newOrganization)
        {
            return await _customerRepository.AddAsync(newOrganization);
        }

        public async Task<Organization> PutOrganizationAsync(Guid organizationId, Guid? parentId, Guid? primaryLocation, Guid callerId, string name, string organizationNumber,
                                                               string street, string postCode, string city, string country,
                                                               string firstName, string lastName, string email, string phoneNumber)
        {
            try
            {
                // Check id
                var organizationOriginal = await GetOrganizationAsync(organizationId, true, true);
                if (organizationOriginal == null)
                    throw new CustomerNotFoundException("Organization with the given id was not found.");

                // Check parent
                if (!await ParentOrganizationIsValid(parentId, organizationId))
                    throw new ParentNotValidException("Invalid organization id on parent.");

                // string fields
                if (name == null || name == string.Empty)
                    throw new RequiredFieldIsEmptyException("The name field is required and cannot be one of: (null || string.Empty). Null is allowed for patch queries.");
                organizationNumber = (organizationNumber == null) ? "" : organizationNumber;

                // Check organizationNumber
                var organizationFromOrgNumber = await GetOrganizationByOrganizationNumberAsync(organizationNumber);
                if (organizationFromOrgNumber != null && organizationFromOrgNumber.OrganizationId != organizationOriginal.OrganizationId)
                    throw new InvalidOrganizationNumberException($"Organization numbers must be unique. An Organization with organization number {organizationNumber} already exists.");

                // PrimaryLocation
                Location newLocation;
                if (primaryLocation == null || primaryLocation == Guid.Empty)
                    newLocation = new Location(Guid.Empty, callerId, "", "", "", "", "", "", "");
                else
                {
                    newLocation = await GetLocationAsync((Guid)primaryLocation);
                    if (newLocation == null)
                        throw new LocationNotFoundException();
                }

                // Address
                Address newAddress;
                street = (street == null) ? "" : street;
                postCode = (postCode == null) ? "" : postCode;
                city = (city == null) ? "" : city;
                country = (country == null) ? "" : country;

                newAddress = new Address(street, postCode, city, country);

                // Contact Person
                ContactPerson newContactPerson = new ContactPerson(firstName, lastName, email, phoneNumber);

                // Do update
                Organization newOrganization = new Organization(organizationId, callerId, parentId, name, organizationNumber, newAddress, newContactPerson, organizationOriginal.Preferences, newLocation, organizationOriginal.IsCustomer);

                organizationOriginal.UpdateOrganization(newOrganization);

                await _customerRepository.SaveEntitiesAsync();

                return organizationOriginal;
            }
            catch (CustomerNotFoundException ex)
            {
                _logger.LogError("OrganizationServices - PutOrganizationAsync: No result on given OrganizationId: " + ex.Message);
                throw;
            }
            catch (ParentNotValidException ex)
            {
                _logger.LogError("OrganizationServices - PutOrganizationAsync: Given parentId (not null || empty) led to organization that A: does not exist, B: has itself a parent." +
                    "\n : " + ex.Message);
                throw;
            }
            catch (RequiredFieldIsEmptyException ex)
            {
                _logger.LogError("OrganizationServices - PutOrganizationAsync: The name field is required and cannot be one of: (null || string.Empty). Null is allowed for patch queries." +
                   "\n : " + ex.Message);
                throw;
            }
            catch (LocationNotFoundException ex)
            {
                _logger.LogError("OrganizationServices - PutOrganizationAsync: No result on Given locationId (not null || empty): " + ex.Message);
                throw;
            }
            catch (InvalidOrganizationNumberException ex)
            {
                _logger.LogError("OrganizationServices - PutOrganizationAsync: Conflict due to organization number being in use. " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - PutOrganizationAsync: failed to update: " + ex.Message);
                throw;
            }
        }

        public async Task<Organization> PatchOrganizationAsync(Guid organizationId, Guid? parentId, Guid? primaryLocation, Guid callerId, string name, string organizationNumber, 
                                                               string street, string postCode, string city, string country,
                                                               string firstName, string lastName, string email, string phoneNumber)
        {
            try
            {
                // Check id
                var organizationOriginal = await GetOrganizationAsync(organizationId, true, true);
                if (organizationOriginal == null)
                    throw new CustomerNotFoundException("Organization with the given id was not found.");

                // Check parent
                if (parentId == null)
                    parentId = organizationOriginal.ParentId;
                else
                {
                    // Check parent
                    if (!await ParentOrganizationIsValid(parentId, organizationId))
                        throw new ParentNotValidException("Invalid organization id on parent.");
                }

                // String fields
                if (name == string.Empty)
                    throw new RequiredFieldIsEmptyException("The name field is required and should never be empty (null is allowed for patch).");
                name = (name == null) ? organizationOriginal.Name : name;
                organizationNumber = (organizationNumber == null) ? organizationOriginal.OrganizationNumber : organizationNumber;

                // Check organizationNumber
                var organizationFromOrgNumber = await GetOrganizationByOrganizationNumberAsync(organizationNumber);
                if (organizationFromOrgNumber != null && organizationFromOrgNumber.OrganizationId != organizationOriginal.OrganizationId)
                    throw new InvalidOrganizationNumberException($"Organization numbers must be unique. An Organization with organization number {organizationNumber} already exists.");

                // PrimaryLocation
                Location newLocation;
                if (primaryLocation == null)
                    newLocation = (organizationOriginal.Location == null) ? new Location(Guid.Empty, callerId, "", "", "", "", "", "", "") : organizationOriginal.Location;
                else if (primaryLocation == Guid.Empty)
                    newLocation = new Location(Guid.Empty, callerId, "", "", "", "", "", "", "");
                else
                {
                    newLocation = await GetLocationAsync((Guid)primaryLocation);
                    if (newLocation == null)
                        throw new LocationNotFoundException();
                }

                // Address
                Address newAddress;
                street = (street == null) ? organizationOriginal.Address.Street : street;
                postCode = (postCode == null) ? organizationOriginal.Address.PostCode : postCode;
                city = (city == null) ? organizationOriginal.Address.City : city;
                country = (country == null) ? organizationOriginal.Address.Country : country;

                newAddress = new Address(street, postCode, city, country);

                // ContactPerson
                ContactPerson newContactPerson;
                firstName = (firstName == null) ? organizationOriginal.ContactPerson.FirstName : firstName;
                lastName = (lastName == null) ? organizationOriginal.ContactPerson.LastName : lastName;
                email = (email == null) ? organizationOriginal.ContactPerson.Email : email;
                phoneNumber = (phoneNumber == null) ? organizationOriginal.ContactPerson.PhoneNumber : phoneNumber;

                newContactPerson = new ContactPerson(firstName, lastName, email, phoneNumber);

                // Do update
                Organization newOrganization = new Organization(organizationId, callerId, parentId, name, organizationNumber, newAddress, newContactPerson, organizationOriginal.Preferences, newLocation, organizationOriginal.IsCustomer);

                organizationOriginal.PatchOrganization(newOrganization);

                await _customerRepository.SaveEntitiesAsync();

                return organizationOriginal;
            }
            catch (CustomerNotFoundException ex)
            {
                _logger.LogError("OrganizationServices - PatchOrganizationAsync: No result on given OrganizationId: " + ex.Message);
                throw;
            }
            catch (ParentNotValidException ex)
            {
                _logger.LogError("OrganizationServices - PatchOrganizationAsync: Given parentId (not null || empty) led to organization that A: does not exist, B: has itself a parent." +
                    "\n : " + ex.Message);
                throw;
            }
            catch (RequiredFieldIsEmptyException ex)
            {
                _logger.LogError("OrganizationServices - PatchOrganizationAsync: The name field is required and cannot be string.Empty. Null is allowed for patch queries." +
                   "\n : " + ex.Message);
                throw;
            }
            catch(LocationNotFoundException ex)
            {
                _logger.LogError("OrganizationServices - PatchOrganizationAsync: No result on Given locationId (not null || empty): " + ex.Message);
                throw;
            }
            catch (InvalidOrganizationNumberException ex)
            {
                _logger.LogError("OrganizationServices - PatchOrganizationAsync: Conflict due to organization number being in use. " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - PatchOrganizationAsync: Failed to update: " + ex.Message);
                throw;
            }
        }

        public async Task<Organization> UpdateOrganizationAsync(Organization updateOrganization, bool usingPatch = false)
        {
            try
            {
                var organization = await _customerRepository.GetOrganizationAsync(updateOrganization.OrganizationId);
                if (usingPatch)
                {
                    organization.PatchOrganization(updateOrganization);
                }
                else
                {
                    organization.UpdateOrganization(updateOrganization);
                }

                await _customerRepository.SaveEntitiesAsync();

                return organization;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationAsync failed to update: " + ex.Message);
                throw;
            }
        }

        public async Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var organization = await _customerRepository.GetOrganizationAsync(organizationId);

                if (organization == null)
                    throw new CustomerNotFoundException();

                if (organization.IsDeleted && !hardDelete)
                    throw new CustomerNotFoundException();

                if (organization.PrimaryLocation != null)
                {
                    await DeleteOrganizationLocationAsync((Guid)organization.PrimaryLocation, callerId, hardDelete);
                }

                await DeleteOrganizationPreferencesAsync(organizationId, callerId, hardDelete);

                // set IsDelete, caller and date of change
                organization.Delete(callerId);
                await _customerRepository.SaveEntitiesAsync();

                // Complete delete, removed from database
                if (hardDelete)
                {
                    return await _customerRepository.DeleteOrganizationAsync(organization);
                }

                return organization;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices -DeleteOrganizationAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferences> GetOrganizationPreferencesAsync(Guid organizationId)
        {
            try
            {
                var preferences = await _customerRepository.GetOrganizationPreferencesAsync(organizationId);
                if (preferences == null)
                    return null;

                if (preferences.IsDeleted)
                    throw new EntityIsDeletedException();
                return preferences;
            }
            catch (EntityIsDeletedException ex)
            {
                _logger.LogError("Entity is deleted. {0}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices -GetOrganizationPreferences failed to be retrieved: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences)
        {
            return await _customerRepository.AddOrganizationPreferencesAsync(organizationPreferences);
        }

        public async Task<OrganizationPreferences> UpdateOrganizationPreferencesAsync(OrganizationPreferences preferences, bool usingPatch = false)
        {
            try
            {
                var currentPreferences = await _customerRepository.GetOrganizationPreferencesAsync(preferences.OrganizationId);
                if (currentPreferences == null)
                    return null;

                if (usingPatch)
                    currentPreferences.PatchPreferences(preferences);
                else
                    currentPreferences.UpdatePreferences(preferences);

                await _customerRepository.SaveEntitiesAsync();
                return currentPreferences;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationPreferencesAsync failed to update: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(Guid organizationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var preferences = await _customerRepository.GetOrganizationPreferencesAsync(organizationId);
                if (preferences == null) // object is already deleted
                    return null;
                preferences.Delete(callerId);
                await _customerRepository.SaveEntitiesAsync();

                if (hardDelete)
                {
                    return await _customerRepository.DeleteOrganizationPreferencesAsync(preferences);
                }

                return preferences;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - DeleteOrganizationPreferencesAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferences> RemoveOrganizationPreferencesAsync(Guid organizationId)
        {
            var organizationPreferences = await _customerRepository.GetOrganizationPreferencesAsync(organizationId);
            return await _customerRepository.DeleteOrganizationPreferencesAsync(organizationPreferences);
        }

        /// <summary>
        /// Get the location object with the given Id
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns>Null or Location</returns>
        public async Task<Location> GetLocationAsync(Guid locationId)
        {
            return await _customerRepository.GetOrganizationLocationAsync(locationId);
        }

        public async Task<Location> AddOrganizationLocationAsync(Location location)
        {
            return await _customerRepository.AddOrganizationLocationAsync(location);
        }

        public async Task<Location> UpdateOrganizationLocationAsync(Location updateLocation, bool usingPatch = false)
        {
            try
            {
                var currentLocation = await _customerRepository.GetOrganizationLocationAsync(updateLocation.LocationId);
                if (currentLocation == null)
                {
                    return null;
                }

                if (usingPatch)
                    currentLocation.PatchLocation(updateLocation);
                else
                    currentLocation.UpdateLocation(updateLocation);

                await _customerRepository.SaveEntitiesAsync();
                return currentLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationLocationAsync failed to update: " + ex.Message);
                throw;
            }
        }

        public async Task<Location> DeleteOrganizationLocationAsync(Guid locationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var location = await _customerRepository.GetOrganizationLocationAsync(locationId);
                if (location == null) // object is already deleted
                    return null;
                location.Delete(callerId);
                await _customerRepository.SaveEntitiesAsync();

                if (hardDelete)
                {
                    return await _customerRepository.DeleteOrganizationLocationAsync(location);
                }

                return location;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - DeleteOrganizationLocationAsync failed to delete: " + ex.Message);
                throw;
            }
        }


        public async Task<string> EncryptDataForCustomer(Guid customerId, string message, byte[] secretKey, byte[] iv)
        {
            try
            {
                var customer = await _customerRepository.GetOrganizationAsync(customerId);

                if (customer == null)
                    return null;

                string salt = customer.OrganizationId.ToString();


                var encryptedMessage = Encryption.EncryptData(message, salt, secretKey, iv);

                return encryptedMessage;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError("EncryptDataForCustomer failed with CryptographicException error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("EncryptDataForCustomer failed with unknown error: " + ex.Message);
                throw;
            }
        }

        public async Task<string> DecryptDataForCustomer(Guid customerId, string encryptedData, byte[] secretKey, byte[] iv)
        {
            try
            {
                var customer = await _customerRepository.GetOrganizationAsync(customerId);
                if (customer == null)
                    return null;

                string salt = customer.OrganizationId.ToString();
                var decryptedMessage = Encryption.DecryptData(encryptedData, salt, secretKey, iv);

                return decryptedMessage;
            }
            catch (CryptographicException ex)
            {
                _logger.LogError("DecryptDataForCustomer failed with CryptographicException error: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError("DecryptDataForCustomer failed with unknown error: " + ex.Message);
                throw;
            }
        }

        public async Task<bool> ParentOrganizationIsValid(Guid? parentId, Guid organizationId)
        {
           if (parentId != null && parentId != Guid.Empty)
            {
                var parentOrganization = await GetOrganizationAsync((Guid)parentId, false, false);
                if (parentOrganization == null)
                    return false; // not found

                if (parentOrganization.ParentId != null && parentOrganization.ParentId != Guid.Empty)
                    return false; // invalid hierarchy depth

                var childList = await GetOrganizationsByParentId(organizationId);
                if (childList.Count > 0)
                    return false;
            }
            return true;
        }
    }
}