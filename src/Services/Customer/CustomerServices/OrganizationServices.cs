#nullable enable
using AutoMapper;
using Common.Configuration;
using Common.Cryptography;
using Common.Enums;
using Common.Exceptions;
using Common.Interfaces;
using CustomerServices.Email;
using CustomerServices.Exceptions;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CustomerServices
{
    public class OrganizationServices : IOrganizationServices
    {
        private readonly ILogger<OrganizationServices> _logger;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly TechstepPartnerConfiguration _techstepPartnerConfiguration;

        public OrganizationServices(ILogger<OrganizationServices> logger, IOrganizationRepository customerRepository, IMapper mapper, IEmailService emailService,
            IOptions<TechstepPartnerConfiguration> techstepPartnerConfiguration)
        {
            _logger = logger;
            _organizationRepository = customerRepository;
            _mapper = mapper;
            _emailService = emailService;
            _techstepPartnerConfiguration = techstepPartnerConfiguration.Value;
        }

        public async Task<IList<Organization>> GetOrganizationsAsync(bool includePreferences, bool hierarchical = false, bool customersOnly = false, Guid? partnerId = null)
        {
            IList<Organization> organizations;
            Partner? partner = null;

            if (partnerId is not null)
            {
                partner = await _organizationRepository.GetPartnerAsync((Guid)partnerId, true);

                // If partner is null (not found), then we should just return an empty list as we won't get any results from the query anyway...
                if (partner is null)
                    return new List<Organization>();
            }

            // TODO: We need to refactor this when we have time, as this may result in a lot of queries and higher loading times.
            if (!hierarchical)
            {
                if (partner is null)
                    organizations = await _organizationRepository.GetOrganizationsAsync(true, customersOnly: customersOnly);
                else
                    organizations = await _organizationRepository.GetOrganizationsAsync(true, whereFilter: entity => entity.Partner == partner, customersOnly: customersOnly);

                if (includePreferences)
                {
                    foreach (var organization in organizations)
                    {
                        organization.Preferences = await _organizationRepository.GetOrganizationPreferencesAsync(organization.OrganizationId, asNoTracking: true);
                    }
                }
            }
            else
            {
                if (partner is null)
                    organizations = await _organizationRepository.GetOrganizationsAsync(true, whereFilter: entity => entity.ParentId == null, customersOnly: customersOnly);
                else
                    organizations = await _organizationRepository.GetOrganizationsAsync(true, whereFilter: (entity => entity.ParentId == null && entity.Partner == partner), customersOnly: customersOnly);

                foreach (var organization in organizations)
                {
                    organization.ChildOrganizations = await _organizationRepository.GetOrganizationsAsync(true, whereFilter: entity => entity.ParentId == organization.OrganizationId);

                    if (includePreferences)
                    {
                        organization.Preferences = await _organizationRepository.GetOrganizationPreferencesAsync(organization.OrganizationId, asNoTracking: true);
                    }
                }
            }

            return organizations;
        }

        public async Task<PagedModel<Organization>> GetPaginatedOrganizationsAsync(CancellationToken cancellationToken, int page, int limit, bool includePreferences, bool hierarchical = false, bool customersOnly = false, Guid? partnerId = null)
        {
            PagedModel<Organization> pagedOrganizations;
            Partner? partner = null;

            if (partnerId is not null)
            {
                partner = await _organizationRepository.GetPartnerAsync((Guid)partnerId, true);

                // If partner is null (not found), then we should just return an empty list as we won't get any results from the query anyway...
                if (partner is null)
                {
                    return new PagedModel<Organization>
                    {
                        CurrentPage = page,
                        Items = new List<Organization>(),
                        PageSize = limit,
                        TotalPages = 1,
                        TotalItems = 0
                    };
                }
            }

            // TODO: We need to refactor this when we have time, as this may result in a lot of queries and higher loading times.
            if (!hierarchical)
            {
                if (partner is null)
                    pagedOrganizations = await _organizationRepository.GetPaginatedOrganizationsAsync(true, cancellationToken, customersOnly: customersOnly, page: page, limit: limit);
                else
                    pagedOrganizations = await _organizationRepository.GetPaginatedOrganizationsAsync(true, cancellationToken, whereFilter: entity => entity.Partner == partner, customersOnly: customersOnly, page: page, limit: limit);

                if (includePreferences)
                {
                    foreach (var organization in pagedOrganizations.Items)
                    {
                        organization.Preferences = await _organizationRepository.GetOrganizationPreferencesAsync(organization.OrganizationId, asNoTracking: true);
                    }
                }
            }
            else
            {
                if (partner is null)
                    pagedOrganizations = await _organizationRepository.GetPaginatedOrganizationsAsync(true, cancellationToken, whereFilter: entity => entity.ParentId == null, customersOnly: customersOnly, page: page, limit: limit);
                else
                    pagedOrganizations = await _organizationRepository.GetPaginatedOrganizationsAsync(true, cancellationToken, whereFilter: (entity => entity.ParentId == null && entity.Partner == partner), customersOnly: customersOnly, page: page, limit: limit);

                foreach (var organization in pagedOrganizations.Items)
                {
                    organization.ChildOrganizations = await _organizationRepository.GetOrganizationsAsync(true, whereFilter: entity => entity.ParentId == organization.OrganizationId);

                    if (includePreferences)
                    {
                        organization.Preferences = await _organizationRepository.GetOrganizationPreferencesAsync(organization.OrganizationId, asNoTracking: true);
                    }
                }
            }

            return pagedOrganizations;
        }

        /// <summary>
        ///     Returns all organizations with a given <see cref="Organization.ParentId"/>. If <paramref name="parentId"/> is <see langword="null"/>,
        ///     it returns all top/root level organizations.
        /// </summary>
        /// <param name="parentId"> The ID of the parent organization. </param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns> A list containing all matching organizations. </returns>
        public async Task<IList<Organization>> GetOrganizationsByParentId(Guid? parentId, bool asNoTracking)
        {
            return await _organizationRepository.GetOrganizationsAsync(asNoTracking, whereFilter: entity => entity.ParentId == parentId);
        }

        public async Task<Organization?> GetOrganizationAsync(Guid customerId)
        {
            return await _organizationRepository.GetOrganizationAsync(customerId, includeDepartments: true);
        }

        /// <summary>
        /// Get the organization with the given Id. Optional: Return the OrganizationPreferences and OrganizationLocation object of the organization along with the organization itself.
        /// </summary>
        /// <param name="customerId">The id of the organization queried</param>
        /// <param name="includePreferences">Include OrganizationPreferences object of the organization if set to true</param>
        /// <param name="includeLocation">Include OrganizationLocation object of the organization if set to true</param>
        /// <returns>Organization</returns>
        public async Task<Organization?> GetOrganizationAsync(Guid customerId, bool includeDepartments, bool includePreferences, bool includeLocation, bool includePartner, bool customersOnly = false)
        {
            var organization = await _organizationRepository.GetOrganizationAsync(customerId, includeDepartments: includeDepartments, customersOnly: customersOnly, includeLocations: includeLocation, includePartner: includePartner, asNoTracking: true);

            if (organization is not null)
            {
                if (includePreferences)
                {
                    organization.Preferences = await _organizationRepository.GetOrganizationPreferencesAsync(customerId, asNoTracking: true);
                }
                if (includeLocation && organization.PrimaryLocation is null && organization.Address != null)
                {
                    var newLocation = new Location(string.Empty, string.Empty,
                    organization.Address.Street, string.Empty, organization.Address.PostCode, organization.Address.City,
                    organization.Address.Country);
                    newLocation.SetPrimaryLocation(true);
                    organization.AddLocation(newLocation, customerId, Guid.Empty);
                    await _organizationRepository.AddOrganizationLocationAsync(newLocation);
                }
            }

            return organization;
        }

        public async Task<Organization> GetOrganizationByOrganizationNumberAsync(string organizationNumber)
        {
            return await _organizationRepository.GetOrganizationByOrganizationNumber(organizationNumber);
        }

        public async Task<IList<OrganizationUserCount>?> GetOrganizationUserCountAsync(Guid? partnerId, Guid[]? assignedToDepartment)
        {
            return await _organizationRepository.GetAllOrganizationsUsersCountAsync(partnerId, assignedToDepartment);
        }

        public async Task<OrganizationDTO> AddOrganizationAsync(NewOrganizationDTO newOrganization)
        {
            // Make sure default values that should never be assigned are identified as null.
            if (newOrganization.ParentId == Guid.Empty)
                newOrganization.ParentId = null;
            if (newOrganization.PartnerId == Guid.Empty)
                newOrganization.PartnerId = null;
            if (newOrganization.PrimaryLocation == Guid.Empty)
                newOrganization.PrimaryLocation = null;

            // TODO: This check should be added in, but we can't do so until the partner-implementation is completed in frontend. Otherwise it will always trigger.
            // Basic checks
            //if (newOrganization.PartnerId is null && newOrganization.IsCustomer)
            //    throw new ArgumentException("The organization must have a partner before it can be set as a customer.");

            #region Location
            Location location;

            // Error checking
            if (newOrganization.Location is null && newOrganization.PrimaryLocation is null)
                throw new ArgumentException($"No location have been provided. Provide one of the following: {nameof(newOrganization.Location)} or {nameof(newOrganization.PrimaryLocation)}.");
            else if (newOrganization.Location is not null && newOrganization.PrimaryLocation is not null)
                throw new DuplicateException($"Only one of the following should be provided: {nameof(newOrganization.Location)} or {nameof(newOrganization.PrimaryLocation)}.");

            // Value mapping (either use an existing location, or create a new one)
            if (newOrganization.PrimaryLocation is not null)
            {
                location = await _organizationRepository.GetOrganizationLocationAsync((Guid)newOrganization.PrimaryLocation);

                if (location is null)
                    throw new ArgumentException("Location not found");
            }
            else
            {
                location = new Location(newOrganization.Location.Name,
                                            newOrganization.Location.Description, newOrganization.Location.Address1,
                                            newOrganization.Location.Address2, newOrganization.Location.PostalCode,
                                            newOrganization.Location.City, newOrganization.Location.Country);

                location = await _organizationRepository.AddOrganizationLocationAsync(location);
            }
            #endregion Location


            #region Partner
            Partner? partner = null;
            // If it has a partner, make sure it exist!
            long? techstepCustomerId = null;
            if (newOrganization.PartnerId is not null)
            {
                partner = await _organizationRepository.GetPartnerAsync((Guid)newOrganization.PartnerId, false);

                if (partner is null)
                    throw new ArgumentException("Partner not found");

                //Only if the partner is Techstep the customers techstepCustomerId should be saved
                if (partner.ExternalId == _techstepPartnerConfiguration.PartnerId)
                {
                    techstepCustomerId = newOrganization.TechstepCustomerId;
                }
            }
            #endregion Partner

            #region Parent
            // If it has a parent, make sure it exist!
            if (newOrganization.ParentId != null)
            {
                var parentOrganization = await GetOrganizationAsync((Guid)newOrganization.ParentId);

                if (parentOrganization is null)
                    throw new CustomerNotFoundException("The parent organization was not found.");
                if (parentOrganization.ParentId != null)
                    throw new ArgumentException("The parent is not valid. All parent-organizations must be top-level organizations, and cannot have other parent.");
            }
            #endregion Parent


            // Make sure we don't add an already existing organization-number.
            if (await GetOrganizationByOrganizationNumberAsync(newOrganization.OrganizationNumber) != null)
                throw new DuplicateException($"An organization with the provided organization-number already exists.");

            var address = new Address(newOrganization.Address.Street, newOrganization.Address.PostCode, newOrganization.Address.City, newOrganization.Address.Country);
            var contactPerson = new ContactPerson(newOrganization.ContactPerson.FirstName, newOrganization.ContactPerson.LastName, newOrganization.ContactPerson.Email, newOrganization.ContactPerson.PhoneNumber);
            var organization = new Organization(Guid.NewGuid(), newOrganization.ParentId,
                                                newOrganization.Name, newOrganization.OrganizationNumber, address,
                                                contactPerson, null, location,
                                                partner, newOrganization.IsCustomer, lastSalaryReportingDay: null, addUsersToOkta: newOrganization.AddUsersToOkta, accountOwner: newOrganization.AccountOwner, techstepCustomerId: techstepCustomerId);

            organization = await _organizationRepository.AddAsync(organization);


            #region Preferences

            // OrganizationPreferences needs the OrganizationId from newOrganization, and is therefore made last
            if (newOrganization.Preferences is not null)
            {
                if (newOrganization.Preferences.EnforceTwoFactorAuth == null)
                    newOrganization.Preferences.EnforceTwoFactorAuth = false;

                if (newOrganization.Preferences.DefaultDepartmentClassification == null)
                    newOrganization.Preferences.DefaultDepartmentClassification = 0;

                organization.Preferences = new OrganizationPreferences(organization.OrganizationId, organization.CreatedBy, newOrganization.Preferences?.WebPage,
                                                          newOrganization.Preferences?.LogoUrl, newOrganization.Preferences?.OrganizationNotes,
                                                          (bool)newOrganization.Preferences.EnforceTwoFactorAuth, newOrganization.Preferences?.PrimaryLanguage,
                                                          (short)newOrganization.Preferences.DefaultDepartmentClassification);
            }
            else // When no preferences have been added, create an empty "default" variant
            {
                organization.Preferences = new OrganizationPreferences(organization.OrganizationId, organization.CreatedBy, null, null, null, false, "en", 0);
            }

            await _organizationRepository.AddOrganizationPreferencesAsync(organization.Preferences);

            #endregion


            return new OrganizationDTO(organization);
        }


        public async Task<Organization> PutOrganizationAsync(UpdateOrganizationDTO updatedOrganization)
        {
            try
            {
                // Check id
                var organizationOriginal = await GetOrganizationAsync(updatedOrganization.OrganizationId, false, true, true, true);
                if (organizationOriginal is null)
                    throw new CustomerNotFoundException("Organization with the given id was not found.");

                // Check parent
                if (!await ParentOrganizationIsValid(updatedOrganization.ParentId, updatedOrganization.OrganizationId))
                    throw new ParentNotValidException("Invalid organization id on parent.");

                //If Techstep is partner make sure that organization gets the original name and organization
                if (organizationOriginal.Partner != null && (organizationOriginal.Partner.ExternalId == _techstepPartnerConfiguration.PartnerId))
                {
                    updatedOrganization.OrganizationNumber = organizationOriginal.OrganizationNumber;
                    updatedOrganization.Name = organizationOriginal.Name;
                }

                // string fields
                if (updatedOrganization.Name == null || updatedOrganization.Name == string.Empty)
                    throw new RequiredFieldIsEmptyException("The name field is required and cannot be one of: (null || string.Empty). Null is allowed for patch queries.");
                updatedOrganization.OrganizationNumber = (updatedOrganization.OrganizationNumber is null) ? "" : updatedOrganization.OrganizationNumber;

                // Check organizationNumber
                var organizationFromOrgNumber = await GetOrganizationByOrganizationNumberAsync(updatedOrganization.OrganizationNumber);
                if (organizationFromOrgNumber is not null && organizationFromOrgNumber.OrganizationId != organizationOriginal.OrganizationId)
                    throw new InvalidOrganizationNumberException($"Organization numbers must be unique. An Organization with organization number {updatedOrganization.OrganizationNumber} already exists.");

                // PrimaryLocation
                Location newLocation;
                if (updatedOrganization.PrimaryLocation is null || updatedOrganization.PrimaryLocation == Guid.Empty)
                    newLocation = new Location("", "", "", "", "", "", "");
                else
                {
                    if (updatedOrganization.PrimaryLocation is null)
                        throw new LocationNotFoundException();

                    newLocation = await _organizationRepository.GetOrganizationLocationAsync((Guid)updatedOrganization.PrimaryLocation.Value);

                    if (newLocation is null)
                        throw new LocationNotFoundException();
                }

                // Address
                Address newAddress;

                newAddress = new Address(updatedOrganization.Address.Street, updatedOrganization.Address.PostCode, updatedOrganization.Address.City, updatedOrganization.Address.Country);

                // Contact Person
                var newContactPerson = new ContactPerson(updatedOrganization.ContactPerson.FirstName, updatedOrganization.ContactPerson.LastName, updatedOrganization.ContactPerson.Email, updatedOrganization.ContactPerson.Email);

                // Do update
                var newOrganization = new Organization(updatedOrganization.OrganizationId, updatedOrganization.ParentId, updatedOrganization
                    .Name, updatedOrganization.OrganizationNumber, newAddress, newContactPerson, organizationOriginal.Preferences, newLocation, organizationOriginal.Partner, organizationOriginal.IsCustomer, updatedOrganization.LastDayForReportingSalaryDeduction, null, null, updatedOrganization.PayrollContactEmail, updatedOrganization.AddUsersToOkta == null ? false : true);

                organizationOriginal.UpdateOrganization(newOrganization);

                await _organizationRepository.SaveEntitiesAsync();

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

        public async Task<Organization> PatchOrganizationAsync(Guid organizationId, Guid? parentId,
            Guid? primaryLocation, Guid callerId, string name, string organizationNumber, string street,
            string postCode, string city, string country, string firstName, string lastName, string email,
            string phoneNumber, int? lastSalaryReportingDay, string payrollEmail = "", bool addUsersToOkta = false)
        {
            try
            {
                // Check id
                var organizationOriginal = await GetOrganizationAsync(organizationId, false, true, true, true);
                if (organizationOriginal is null)
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

                //Customers with techstep as a partner should not be able to update their name and organization number
                if (organizationOriginal.Partner == null || organizationOriginal.Partner.ExternalId != _techstepPartnerConfiguration.PartnerId)
                {
                    // String fields
                    if (name == string.Empty)
                        throw new RequiredFieldIsEmptyException("The name field is required and should never be empty (null is allowed for patch).");
                    name ??= organizationOriginal.Name;
                    organizationNumber ??= organizationOriginal.OrganizationNumber;

                    // Check organizationNumber
                    var organizationFromOrgNumber = await GetOrganizationByOrganizationNumberAsync(organizationNumber);
                    if (organizationFromOrgNumber != null && organizationFromOrgNumber.OrganizationId != organizationOriginal.OrganizationId)
                        throw new InvalidOrganizationNumberException($"Organization numbers must be unique. An Organization with organization number {organizationNumber} already exists.");
                }
                else
                {
                    organizationNumber = organizationOriginal.OrganizationNumber;
                    name = organizationOriginal.Name;
                }
                // PrimaryLocation
                Location newLocation;
                if (primaryLocation == null)
                    newLocation = (organizationOriginal.PrimaryLocation is null) ? new Location("", "", "", "", "", "", "") : organizationOriginal.PrimaryLocation;
                else if (primaryLocation == Guid.Empty)
                    newLocation = new Location("", "", "", "", "", "", "");
                else
                {
                    if (primaryLocation is null)
                        throw new LocationNotFoundException();

                    newLocation = await _organizationRepository.GetOrganizationLocationAsync((Guid)primaryLocation);

                    if (newLocation is null)
                        throw new LocationNotFoundException();
                }

                // Address
                Address newAddress;
                street ??= organizationOriginal.Address.Street;
                postCode ??= organizationOriginal.Address.PostCode;
                city ??= organizationOriginal.Address.City;
                country ??= organizationOriginal.Address.Country;

                newAddress = new Address(street, postCode, city, country);

                // ContactPerson
                ContactPerson newContactPerson;
                firstName ??= organizationOriginal.ContactPerson.FirstName;
                lastName ??= organizationOriginal.ContactPerson.LastName;
                email ??= organizationOriginal.ContactPerson.Email;
                phoneNumber ??= organizationOriginal.ContactPerson.PhoneNumber;

                newContactPerson = new ContactPerson(firstName, lastName, email, phoneNumber);

                // Do update
                var newOrganization = new Organization(organizationId, parentId, name, organizationNumber, newAddress, newContactPerson, organizationOriginal.Preferences, newLocation, organizationOriginal.Partner, organizationOriginal.IsCustomer, lastSalaryReportingDay, null, null, payrollEmail, addUsersToOkta);

                organizationOriginal.UpdateOrganization(newOrganization);

                await _organizationRepository.SaveEntitiesAsync();

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
            catch (LocationNotFoundException ex)
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
                var organization = await _organizationRepository.GetOrganizationAsync(updateOrganization.OrganizationId, includeDepartments: true);

                if (organization is null)
                    throw new CustomerNotFoundException();

                if (usingPatch)
                {
                    organization.UpdateOrganization(updateOrganization);
                }
                else
                {
                    organization.UpdateOrganization(updateOrganization);
                }

                await _organizationRepository.SaveEntitiesAsync();

                return organization;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationAsync failed to update: " + ex.Message);
                throw;
            }
        }
        public async Task UpdateOrganizationTechstepCoreAsync(TechstepCoreCustomerUpdateDTO updateTechstepCore)
        {
            foreach (var updatedOrganization in updateTechstepCore.Data)
            {
                var organization = await _organizationRepository.GetOrganizationByTechstepCustomerIdAsync(updatedOrganization.TechstepCustomerId);
                if (organization == null) throw new CustomerNotFoundException($"Organization with techstep core id {updatedOrganization.TechstepCustomerId} is not found.");

                //update organization name
                if (!string.IsNullOrWhiteSpace(updatedOrganization.Name) && (updatedOrganization.Name != organization.Name))
                {
                    organization.UpdateOrganizationName(updatedOrganization.Name);
                }

                //update organization organization number
                if (!string.IsNullOrWhiteSpace(updatedOrganization.OrgNumber) && (updatedOrganization.OrgNumber != organization.OrganizationNumber))
                {
                    organization.UpdateOrganizationNumber(updatedOrganization.OrgNumber);
                }

                //update organization country code
                if (!string.IsNullOrWhiteSpace(updatedOrganization.CountryCode))
                {
                    var organizationPreferences = await _organizationRepository.GetOrganizationPreferencesAsync(organization.OrganizationId);

                    if (organizationPreferences == null)
                    {
                        //create default preferences
                        organization.Preferences = new OrganizationPreferences(organization.OrganizationId, Guid.Empty, null, null, null, false, updatedOrganization.CountryCode.ToLower(), 0); //caller id by Techstep core
                    }
                    else if (organizationPreferences.PrimaryLanguage != null && (updatedOrganization.CountryCode.ToLower() != organizationPreferences.PrimaryLanguage))
                    {

                        var newPreferences = new OrganizationPreferences(organization.OrganizationId, Guid.Empty,
                            organizationPreferences.WebPage ?? null, organizationPreferences.LogoUrl ?? null, organizationPreferences.OrganizationNotes ?? null,
                            organizationPreferences.EnforceTwoFactorAuth, updatedOrganization.CountryCode.ToLower(), organizationPreferences.DefaultDepartmentClassification);

                        organizationPreferences.UpdatePreferences(newPreferences);
                    }
                }

                //update organizations account owner
                if (!string.IsNullOrWhiteSpace(updatedOrganization.AccountOwner) && (updatedOrganization.AccountOwner != organization.AccountOwner))
                {
                    organization.UpdateTechstepAccountOwner(updatedOrganization.AccountOwner);
                }
            }
            await _organizationRepository.SaveEntitiesAsync();

        }

        public async Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var organization = await _organizationRepository.GetOrganizationAsync(organizationId, includeDepartments: true);

                if (organization is null)
                    throw new CustomerNotFoundException();

                if (organization.IsDeleted && !hardDelete)
                    throw new CustomerNotFoundException();

                if (organization.Locations != null && organization.Locations.Any())
                {
                    await DeleteOrganizationAllLocationAsync(organizationId, callerId, hardDelete);
                }

                await DeleteOrganizationPreferencesAsync(organizationId, callerId, hardDelete);

                // set IsDelete, caller and date of change
                organization.Delete(callerId);
                await _organizationRepository.SaveEntitiesAsync();

                // Complete delete, removed from database
                if (hardDelete)
                {
                    return await _organizationRepository.DeleteOrganizationAsync(organization);
                }

                return organization;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices -DeleteOrganizationAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        public async Task<OrganizationPreferencesDTO> GetOrganizationPreferencesAsync(Guid organizationId)
        {
            try
            {
                var preferences = await _organizationRepository.GetOrganizationPreferencesAsync(organizationId, asNoTracking: true);

                // TODO: We should likely throw an exception here
                if (preferences is null)
                    return null;
                if (preferences.IsDeleted)
                    throw new EntityIsDeletedException();

                return new OrganizationPreferencesDTO(preferences);
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

        public async Task<OrganizationPreferences> UpdateOrganizationPreferencesAsync(OrganizationPreferences preferences, bool usingPatch = false)
        {
            try
            {
                var currentPreferences = await _organizationRepository.GetOrganizationPreferencesAsync(preferences.OrganizationId);

                // TODO: We should likely throw an exception here
                if (currentPreferences is null)
                    return null;

                if (usingPatch)
                    currentPreferences.PatchPreferences(preferences);
                else
                    currentPreferences.UpdatePreferences(preferences);

                await _organizationRepository.SaveEntitiesAsync();
                return currentPreferences;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationPreferencesAsync failed to update: " + ex.Message);
                throw;
            }
        }

        private async Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(Guid organizationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var preferences = await _organizationRepository.GetOrganizationPreferencesAsync(organizationId);

                // TODO: We should likely throw an exception here
                if (preferences is null) // object is already deleted
                    return null;

                preferences.Delete(callerId);
                await _organizationRepository.SaveEntitiesAsync();

                if (hardDelete)
                {
                    return await _organizationRepository.DeleteOrganizationPreferencesAsync(preferences);
                }

                return preferences;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - DeleteOrganizationPreferencesAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Get the location object with the given Id
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns>Null or Location</returns>
        public async Task<Location> GetLocationAsync(Guid locationId)
        {
            return await _organizationRepository.GetOrganizationLocationAsync(locationId);
        }

        public async Task<Location> AddOrganizationLocationAsync(Location location)
        {
            return await _organizationRepository.AddOrganizationLocationAsync(location);
        }

        public async Task<Location> UpdateOrganizationLocationAsync(Location updateLocation, bool usingPatch = false)
        {
            try
            {
                var currentLocation = await _organizationRepository.GetOrganizationLocationAsync(updateLocation.ExternalId);

                // TODO: We should likely throw an exception here
                if (currentLocation is null)
                    return null;

                if (usingPatch)
                    currentLocation.PatchLocation(updateLocation);
                else
                    currentLocation.UpdateLocation(updateLocation);

                await _organizationRepository.SaveEntitiesAsync();
                return currentLocation;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - UpdateOrganizationLocationAsync failed to update: " + ex.Message);
                throw;
            }
        }
        public async Task<IList<LocationDTO>> DeleteOrganizationAllLocationAsync(Guid organizationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var locations = await _organizationRepository.GetOrganizationAllLocationAsync(organizationId);

                // TODO: We should likely throw an exception here
                if (locations is null) // object is already deleted
                    return null;

                var response = new List<LocationDTO>();
                foreach (var location in locations)
                {
                    location.Delete(callerId);
                    response.Add(new LocationDTO(location));
                }
                await _organizationRepository.SaveEntitiesAsync();

                if (hardDelete)
                {
                    response = new List<LocationDTO>();
                    foreach (var location in locations)
                    {
                        location.Delete(callerId);
                        var result = await _organizationRepository.DeleteOrganizationLocationAsync(location);
                        response.Add(new LocationDTO(result));
                    }
                    return response;
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - DeleteOrganizationAllLocationAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        public async Task<LocationDTO> DeleteOrganizationLocationAsync(Guid locationId, Guid callerId, bool hardDelete = false)
        {
            try
            {
                var location = await _organizationRepository.GetOrganizationLocationAsync(locationId);

                // TODO: We should likely throw an exception here
                if (location is null) // object is already deleted
                    return null;

                location.Delete(callerId);
                await _organizationRepository.SaveEntitiesAsync();

                if (hardDelete)
                {
                    var result = await _organizationRepository.DeleteOrganizationLocationAsync(location);
                    return new LocationDTO(result);
                }

                return new LocationDTO(location);
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - DeleteOrganizationLocationAsync failed to delete: " + ex.Message);
                throw;
            }
        }

        public async Task<LocationDTO> AddLocationInOrganization(NewLocationDTO location, Guid customerId, Guid callerId)
        {
            try
            {
                var organization = await _organizationRepository.GetOrganizationAsync(customerId, includeLocations: true);

                if (organization is null) throw new CustomerNotFoundException();

                var newLocation = new Location(location.Name, location.Description,
                    location.Address1, location.Address2, location.PostalCode, location.City,
                    location.Country);

                if (newLocation.IsNull()) throw new ArgumentException($"Location have all field Empty");

                if (location.IsPrimary)
                {
                    var existingPrimaryLocation = organization.PrimaryLocation;
                    if (existingPrimaryLocation is not null)
                        existingPrimaryLocation.SetPrimaryLocation(false);
                    newLocation.SetPrimaryLocation(true);
                }
                organization.AddLocation(newLocation, customerId, callerId);

                var addedLocation = await _organizationRepository.AddOrganizationLocationAsync(newLocation);

                return new LocationDTO(addedLocation);
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - AddLocationInOrganization failed to Add Location: " + ex.Message);
                throw;
            }
        }
        public async Task<IList<LocationDTO>> GetAllLocationInOrganization(Guid customerId)
        {
            try
            {
                var locations = await _organizationRepository.GetOrganizationAllLocationAsync(customerId);
                return _mapper.Map<IList<LocationDTO>>(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError("OrganizationServices - GetAllLocationInOrganization failed to Get Locations: " + ex.Message);
                throw;
            }
        }

        public async Task<string> EncryptDataForCustomer(Guid customerId, string message, byte[] secretKey, byte[] iv)
        {
            try
            {
                var customer = await _organizationRepository.GetOrganizationAsync(customerId, includeDepartments: true);

                if (customer is null)
                    throw new CustomerNotFoundException();

                var salt = customer.OrganizationId.ToString();
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
                var customer = await _organizationRepository.GetOrganizationAsync(customerId, includeDepartments: true);

                if (customer is null)
                    throw new CustomerNotFoundException();

                var salt = customer.OrganizationId.ToString();
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
                var parentOrganization = await GetOrganizationAsync((Guid)parentId, false, false, false, false);
                if (parentOrganization is null)
                    return false; // not found

                if (parentOrganization.ParentId is not null && parentOrganization.ParentId != Guid.Empty)
                    return false; // invalid hierarchy depth

                var childList = await GetOrganizationsByParentId(organizationId, true);
                if (childList.Count > 0)
                    return false;
            }

            return true;
        }

        public async Task<Organization?> InitiateOnboardingAsync(Guid organizationId)
        {
            var customer = await _organizationRepository.GetOrganizationAsync(organizationId);

            if (customer == null) throw new CustomerNotFoundException();


            var users = await _organizationRepository.GetUsersForCustomerAsync(organizationId, false, includeUserPreference: true);
            if (users.Count == 0 && !users.Any()) throw new ArgumentException($"Customers need to have at least one user imported to initiate the onboarding process.");

            customer.InitiateOnboarding();
            var customerPreferences = await _organizationRepository.GetOrganizationPreferencesAsync(organizationId);

            if (users != null && users.Any()) await SendInvitationMail(users, customerPreferences != null ? customerPreferences.PrimaryLanguage : "en");

            await _organizationRepository.SaveEntitiesAsync();

            return customer;

        }
        public async Task SendInvitationMail(IList<User> users, string defaultLanguage)
        {

            foreach (var user in users)
            {
                try
                {
                    if (user.UserStatus == UserStatus.NotInvited)
                    {

                        await _emailService.InvitationEmailToUserAsync(new Email.Models.InvitationMail
                        {
                            FirstName = user.FirstName,
                            Recipient = new List<string> { user.Email }
                        }, user.UserPreference != null ? user.UserPreference.Language : defaultLanguage);

                        user.ChangeUserStatus(null, UserStatus.Invited);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"SendIvitationMail failed. " + ex.Message);
                }
            }


        }

        public async Task<IList<Guid>> GetOrganizationIdsForPartnerAsync(Guid partnerId)
        {
            return await _organizationRepository.GetOrganizationIdsForPartnerAsync(partnerId);
        }
    }
}