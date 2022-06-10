using Common.Interfaces;
using CustomerServices.ServiceModels;
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
        Task<IList<OrganizationUserCount>> GetOrganizationUserCountAsync();

        /// <summary>
        ///     Retrieves all organization. Optional parameters may be provided to apply additional filtering, and for enabling eager loading. <br/>
        ///     The use of named parameters is recommended. <para>
        ///     
        ///     Example: <code>
        ///         var organizations = new OrganizationRepository().GetOrganizationsAsync(
        ///             Guid.Empty, 
        ///             whereFilter: entity => (entity.PrimaryLocation == Guid.Empty || entity.PrimaryLocation == null), 
        ///             customersOnly: true
        ///         )
        ///     </code></para>
        /// </summary>
        /// <param name="whereFilter"> An optional, parameterized lambda-expression that applies a "<c>.Where()</c>" filter-condition to the query and it's
        ///     retrieved dataset. This filter is not applied when the value is <see langword="null"/>. </param>
        /// <param name="customersOnly"> When <see langword="true"/>, a parameterized "<c>.Where()</c>" filter condition is added to the
        ///     query, causing only organizations where "<c><see cref="Organization.IsCustomer"/> == <see langword="true"/></c>" to be retrieved. </param>
        /// <param name="excludeDeleted"> When <see langword="true"/>, a parameterized "<c>.Where()</c>" filter condition is added to the query, 
        ///     causing soft-deleted organizations (<c><see cref="Organization.IsDeleted"/> == <see langword="true"/></c>) to be excluded from the results. </param>
        /// <param name="includeDepartments"> When <see langword="true"/>, it eagerly includes <see cref="Organization.Departments"/>. </param>
        /// <param name="includeAddress"> When <see langword="true"/>, it eagerly includes <see cref="Organization.Address"/>. </param>
        /// <param name="includePartner"> When <see langword="true"/>, it eagerly includes <see cref="Organization.Partner"/>. </param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns> A list of all matching organizations. </returns>
        Task<IList<Organization>> GetOrganizationsAsync(Expression<Func<Organization, bool>>? whereFilter = null,
                                                        bool customersOnly = true,
                                                        bool excludeDeleted = true,
                                                        bool includeDepartments = false,
                                                        bool includeAddress = false,
                                                        bool includePartner = true,
                                                        bool asNoTracking = true);

        /// <summary>
        ///     Retrieves a single organization using it's ID. Optional parameters may be provided to apply additional filtering, and for enabling eager loading. <br/>
        ///     The use of named parameters is recommended. <para>
        ///     
        ///     Example: <code>
        ///         Organization org = new OrganizationRepository().GetOrganizationAsync(
        ///             Guid.Empty, 
        ///             whereFilter: entity => (entity.PrimaryLocation == Guid.Empty || entity.PrimaryLocation == null), 
        ///             customersOnly: true
        ///         )
        ///     </code></para>
        /// </summary>
        /// <param name="organizationId"> The ID of the organization that is retrieved. </param>
        /// <param name="whereFilter"> An optional, parameterized lambda-expression that applies a "<c>.Where()</c>" filter-condition to the query and it's
        ///     retrieved dataset. This filter is not applied when the value is <see langword="null"/>. </param>
        /// <param name="customersOnly"> When <see langword="true"/>, a parameterized "<c>.Where()</c>" filter condition is added to the
        ///     query, causing only organizations where "<c><see cref="Organization.IsCustomer"/> == <see langword="true"/></c>" to be retrieved. </param>
        /// <param name="excludeDeleted"> When <see langword="true"/>, a parameterized "<c>.Where()</c>" filter condition is added to the query, 
        ///     causing soft-deleted organizations (<c><see cref="Organization.IsDeleted"/> == <see langword="true"/></c>) to be excluded from the results. </param>
        /// <param name="includeDepartments"> When <see langword="true"/>, it eagerly includes <see cref="Organization.Departments"/>. </param>
        /// <param name="includeAddress"> When <see langword="true"/>, it eagerly includes <see cref="Organization.Address"/>. </param>
        /// <param name="includePartner"> When <see langword="true"/>, it eagerly includes <see cref="Organization.Partner"/>. </param>
        /// <returns> If found, the queried organization. If no matches are found, it returns <see langword="null"/>. </returns>
        Task<Organization?> GetOrganizationAsync(Guid organizationId,
                                                 Expression<Func<Organization, bool>>? whereFilter = null,
                                                 bool customersOnly = true,
                                                 bool excludeDeleted = true,
                                                 bool includeDepartments = false,
                                                 bool includeAddress = false,
                                                 bool includePartner = true);

        /// <summary>
        ///     Finds an entity with the given primary key.
        /// </summary>
        /// <param name="id"> The primary key value. </param>
        /// <returns> If found, the retrieved entity, or <see langword="null"/> if no results was found. </returns>
        Task<Organization?> GetOrganizationAsync(int id);
        Task<Organization?> GetOrganizationByOrganizationNumber(string organizationNumber);
        Task<OrganizationPreferences?> GetOrganizationPreferencesAsync(Guid organizationId);
        Task<Location?> GetOrganizationLocationAsync(Guid locationId);
        Task<Location> DeleteOrganizationLocationAsync(Location organizationLocation);
        Task<Organization> DeleteOrganizationAsync(Organization organization);

        Task<User?> GetUserByUserName(string emailAddress);
        Task<User?> GetUserByMobileNumber(string mobileNumber);
        Task<int> GetUsersCount(Guid customerId);
        Task<PagedModel<UserDTO>> GetAllUsersAsync(Guid customerId, string[]? role, Guid[]? assignedToDepartment, IList<int>? UserStatus, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 100);
        Task<User?> GetUserAsync(Guid customerId, Guid userId);
        Task<User?> GetUserAsync(Guid userId);
        Task<User> AddUserAsync(User newUser);
        Task<User> DeleteUserAsync(User user);


        Task<Location> AddOrganizationLocationAsync(Location location);
        Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);
        Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);

        Task<IList<Department>> GetDepartmentsAsync(Guid customerId);
        Task<Department?> GetDepartmentAsync(Guid customerId, Guid departmentId);
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
