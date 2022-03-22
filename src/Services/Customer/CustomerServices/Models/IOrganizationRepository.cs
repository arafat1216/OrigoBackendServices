using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace CustomerServices.Models
{
    public interface IOrganizationRepository
    {
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);

        Task<Organization> AddAsync(Organization customer);
        Task<IList<CustomerUserCount>> GetOrganizationUserCountsAsync();
        Task<IList<Organization>> GetOrganizationsAsync();
        Task<IList<Organization>> GetOrganizationsAsync(Guid? parentId);
        Task<IList<Organization>> GetCustomersAsync();
        Task<IList<Organization>> GetCustomersAsync(Guid? parentId);
        //Task<Organization> GetOrganizationAsync(Guid customerId);

        Task<Organization?> GetOrganizationAsync(Guid organizationId,
                                                 Expression<Func<Organization, bool>>? filter = null,
                                                 bool includeDepartments = true,
                                                 bool includePreferences = false,
                                                 bool includeLocation = false,
                                                 bool includeAddress = false,
                                                 bool customersOnly = true,
                                                 bool excludeDeleted = true);


        /// <summary>
        ///     Finds an entity with the given primary key.
        /// </summary>
        /// <param name="id"> The primary key value. </param>
        /// <returns> If found, the retrieved entity, or <see langword="null"/> if no results was found. </returns>
        Task<Organization?> GetOrganizationAsync(int id);
        Task<Organization?> GetCustomerAsync(Guid customerId);
        Task<Organization> GetOrganizationByOrganizationNumber(string organizationNumber);
        Task<OrganizationPreferences> GetOrganizationPreferencesAsync(Guid organizationId);
        Task<Location> GetOrganizationLocationAsync(Guid? locationId);
        Task<Location> DeleteOrganizationLocationAsync(Location organizationLocation);
        Task<Organization> DeleteOrganizationAsync(Organization organization);

        Task<User> GetUserByUserName(string emailAddress);
        Task<User> GetUserByMobileNumber(string mobileNumber);
        Task<int> GetUsersCount(Guid customerId);
        Task<IList<User>> GetAllUsersAsync(Guid customerId);
        Task<User> GetUserAsync(Guid customerId, Guid userId);
        Task<User> GetUserAsync(Guid userId);
        Task<User> AddUserAsync(User newUser);
        Task<User> DeleteUserAsync(User user);


        Task<Location> AddOrganizationLocationAsync(Location location);
        Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);
        Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);

        Task<IList<Department>> GetDepartmentsAsync(Guid customerId);
        Task<Department> GetDepartmentAsync(Guid customerId, Guid departmentId);
        Task<IList<Department>> DeleteDepartmentsAsync(IList<Department> department);


        /// <summary>
        ///     Registers a new partner.
        /// </summary>
        /// <param name="partner"> The partner to be created. </param>
        /// <returns> The created object. </returns>
        Task<Partner> AddPartnerAsync(Partner partner);

        /// <summary>
        ///     Checks if a <see cref="Organization"/>'s internal ID is registered as a <see cref="Partner"/>.
        /// </summary>
        /// <param name="organizationId"> The organizations internal identifier. </param>
        /// <returns> A <see cref="bool"/> stating if the organization's internal ID was found in the partner list. </returns>
        Task<bool> OrganizationIsPartner(int organizationId);

        /// <summary>
        ///     Retrieves a partner with the provided ID.
        /// </summary>
        /// <param name="partnerId"> The partners external ID. </param>
        /// <returns> If found, the corresponding partner. Otherwise it returns <see langword="null"/>. </returns>
        Task<Partner?> GetPartnerAsync(Guid partnerId);

        /// <summary>
        ///     Retrieves all partners.
        /// </summary>
        /// <returns> A collection containing all partners. </returns>
        Task<IList<Partner>> GetPartnersAsync();
    }
}
