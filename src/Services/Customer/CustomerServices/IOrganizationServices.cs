using CustomerServices.Models;
using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <param name="hierarchical"> When <see langword="true"/>, only top-level organizations are returned, with their children instead attached as
        ///     a sub-item inside their parent-organizations. When <see langword="false"/>, a flat list is generated that included all organizations. </param>
        /// <param name="customersOnly">  </param>
        /// <param name="partnerId"> When provided, the results will only include organizations belonging to this partner. 
        ///     The partner-filter is disabled if the value is <see langword="null"/>. </param>
        /// <returns> A list containing all matching organizations. </returns>
        Task<IList<Organization>> GetOrganizationsAsync(bool hierarchical = false, bool customersOnly = false, Guid? partnerId = null);

        Task<IList<CustomerUserCount>> GetCustomerUsersAsync();
        Task<Organization?> GetOrganizationAsync(Guid customerId, bool includePreferences = false, bool includeLocation = false, bool customersOnly = false);
        Task<Organization> UpdateOrganizationAsync(Organization updateOrganization, bool usingPatch = false);
        Task<Organization> PutOrganizationAsync(Guid organizationId, Guid? parentId, Guid? primaryLocation, Guid callerId, string name, string organizationNumber,
                                                               string street, string postCode, string city, string country,
                                                               string firstName, string lastName, string email, string phoneNumber);
        Task<Organization> PatchOrganizationAsync(Guid organizationId, Guid? parentId, Guid? primaryLocation, Guid callerId, string name, string organizationNumber,
                                                               string street, string postCode, string city, string country,
                                                               string firstName, string lastName, string email, string phoneNumber);
        Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId, bool hardDelete = false);
        Task<OrganizationPreferencesDTO> GetOrganizationPreferencesAsync(Guid organizationId);
        Task<OrganizationPreferences> UpdateOrganizationPreferencesAsync(OrganizationPreferences preferences, bool usingPatch = false);
        Task<Location?> GetLocationAsync(Guid locationId);
        Task<Location> UpdateOrganizationLocationAsync(Location updateLocation, bool usingPatch = false);
        Task<LocationDTO> DeleteOrganizationLocationAsync(Guid locationId, Guid callerId, bool hardDelete = false);
        Task<OrganizationDTO> AddOrganizationAsync(NewOrganizationDTO newOrganization);
        Task<Location> AddOrganizationLocationAsync(Location location);
        Task<bool> ParentOrganizationIsValid(Guid? parentId, Guid organizationId);
        Task<Organization?> GetOrganizationByOrganizationNumberAsync(string organizationNumber);
    }
}