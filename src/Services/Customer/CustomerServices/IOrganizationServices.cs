using Common.Interfaces;
using CustomerServices.Models;
using CustomerServices.ServiceModels;

#nullable enable

namespace CustomerServices
{
    public interface IOrganizationServices
    {
        /// <summary>
        ///     Returns all organizations. <para>
        ///     
        ///     If <paramref name="hierarchical"/> is <see langword="true"/>, only top-level organizations are returned
        ///     (<c><see cref="Organization.ParentId"/> == <see langword="null"/></c>), and all children are instead added to the organization's
        ///     <see cref="Organization.ChildOrganizations"/> list. </para> 
        /// </summary>
        /// <param name="includePreferences"> When <c><see langword="true"/></c>, the <see cref="Organization.Preferences"/> property is
        ///     loaded/included in the retrieved data. 
        /// 
        ///     <para>
        ///     This property will not be included unless it's explicitly requested. </para></param>
        /// <param name="hierarchical"> When <see langword="true"/>, only top-level organizations are returned, with their children instead attached as
        ///     a sub-item inside their parent-organizations. When <see langword="false"/>, a flat list is generated that included all organizations. </param>
        /// <param name="customersOnly">  </param>
        /// <param name="partnerId"> When provided, the results will only include organizations belonging to this partner. 
        ///     The partner-filter is disabled if the value is <see langword="null"/>. </param>
        /// <returns> A list containing all matching organizations. </returns>
        Task<IList<Organization>> GetOrganizationsAsync(bool includePreferences, bool hierarchical = false, bool customersOnly = false, Guid? partnerId = null);

        /// <summary>
        ///     Retrieves all organizations using pagination. <para>
        ///     
        ///     If <paramref name="hierarchical"/> is <see langword="true"/>, only top-level organizations are returned
        ///     (<c><see cref="Organization.ParentId"/> == <see langword="null"/></c>), and all children are instead added to the organization's
        ///     <see cref="Organization.ChildOrganizations"/> list. </para> 
        /// </summary>
        /// <param name="cancellationToken"> A dependency-injected <see cref="CancellationToken"/>. </param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The number of items to retrieve for each <paramref name="page"/>. </param>
        /// <param name="includePreferences"> When <c><see langword="true"/></c>, the <see cref="Organization.Preferences"/> property is
        ///     loaded/included in the retrieved data. 
        /// 
        ///     <para>
        ///     This property will not be included unless it's explicitly requested. </para></param>
        /// <param name="hierarchical"> When <see langword="true"/>, only top-level organizations are returned, with their children instead attached as
        ///     a sub-item inside their parent-organizations. When <see langword="false"/>, a flat list is generated that included all organizations. </param>
        /// <param name="customersOnly">  </param>
        /// <param name="search"> </param>
        /// <param name="partnerId"> When provided, the results will only include organizations belonging to this partner. 
        ///     The partner-filter is disabled if the value is <see langword="null"/>. </param>
        /// <returns> A paginated list containing all matching organizations. </returns>
        Task<PagedModel<Organization>> GetPaginatedOrganizationsAsync(CancellationToken cancellationToken, int page, int limit, bool includePreferences, bool hierarchical = false, bool customersOnly = false, string? search = null, Guid? partnerId = null);

        Task<IList<OrganizationUserCount>?> GetOrganizationUserCountAsync(Guid? partnerId, Guid[]? assignedToDepartment);
        Task<Organization?> GetOrganizationAsync(Guid customerId, bool includeDepartments, bool includePreferences, bool includeLocation, bool includePartner, bool customersOnly = false);
        Task<Organization> UpdateOrganizationAsync(Organization updateOrganization, bool usingPatch = false);
        Task<Organization> PutOrganizationAsync(UpdateOrganizationDTO updatedOrganization);
        Task<Organization> PatchOrganizationAsync(Guid organizationId, Guid? parentId, Guid? primaryLocation, Guid callerId, string name, string organizationNumber,
                                                               string street, string postCode, string city, string country,
                                                               string firstName, string lastName, string email, string phoneNumber, int? lastSalaryReportingDay, string payrollEmail = "", bool addUsersToOkta = false);
        Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId, bool hardDelete = false);
        Task<OrganizationPreferencesDTO> GetOrganizationPreferencesAsync(Guid organizationId);
        Task<OrganizationPreferences> UpdateOrganizationPreferencesAsync(OrganizationPreferences preferences, bool usingPatch = false);
        Task<Location?> GetLocationAsync(Guid locationId);
        Task<Location> UpdateOrganizationLocationAsync(Location updateLocation, bool usingPatch = false);
        Task<IList<LocationDTO>> DeleteOrganizationAllLocationAsync(Guid organizationId, Guid callerId, bool hardDelete = false);
        Task<LocationDTO> DeleteOrganizationLocationAsync(Guid locationId, Guid callerId, bool hardDelete = false);
        Task<OrganizationDTO> AddOrganizationAsync(NewOrganizationDTO newOrganization);
        Task<Location> AddOrganizationLocationAsync(Location location);
        Task<bool> ParentOrganizationIsValid(Guid? parentId, Guid organizationId);
        Task<Organization?> GetOrganizationByOrganizationNumberAsync(string organizationNumber);
        Task<LocationDTO> AddLocationInOrganization(NewLocationDTO location, Guid customerId, Guid callerId);
        Task<IList<LocationDTO>> GetAllLocationInOrganization(Guid customerId);
        Task<Organization?> InitiateOnboardingAsync(Guid organizationId);
        Task UpdateOrganizationTechstepCoreAsync(TechstepCoreCustomerUpdateDTO updateTechstepCore);
        Task<IList<Guid>> GetOrganizationIdsForPartnerAsync(Guid partnerId);
        Task<string?> GetHashedApiKeyAsync(Guid organizationId, CancellationToken cancellationToken);
        Task<string> GenerateAndSaveHashedApiKeyAsync(Guid organizationId, CancellationToken cancellationToken);
    }
}