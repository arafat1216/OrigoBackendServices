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
        Task SaveEntitiesAsync(CancellationToken cancellationToken = default);

        Task<Organization> AddAsync(Organization customer);

        /// <summary>
        /// Gets the total count for users that is active and count for users that is not considered onboarded for a all organization. 
        /// </summary>
        /// <param name="partnerId">Filter on partner</param>
        /// <param name="assignedToDepartment">A list of guids with department ids, for when the user is a manager.</param>
        /// <returns>A list of counter objects.</returns>
        Task<IList<OrganizationUserCount>?> GetAllOrganizationsUsersCountAsync(Guid? partnerId, Guid[]? assignedToDepartment);

        /// <summary>
        /// Gets the total count for users that is active and count for users that is not considered onboarded for a spesific organization. 
        /// </summary>
        /// <param name="customerId">The id of the organization that the users should be counted for.</param>
        /// <param name="assignedToDepartment">A list of guids with department ids, for when the user is a manager.</param>
        /// <param name="role">The role of the caller.</param>
        /// <returns>A counter object.</returns>
        Task<OrganizationUserCount?> GetOrganizationUsersCountAsync(Guid customerId, Guid[]? assignedToDepartment, string[]? role);

        /// <summary>
        ///     Retrieves all organization. Optional parameters may be provided to apply additional filtering, and for enabling eager loading. <br/>
        ///     The use of named parameters is recommended. <para>
        ///     
        ///     Example: <code>
        ///         var organizations = new OrganizationRepository().GetOrganizationsAsync(
        ///             true,
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
        Task<IList<Organization>> GetOrganizationsAsync(bool asNoTracking,
                                                        Expression<Func<Organization, bool>>? whereFilter = null,
                                                        bool customersOnly = true,
                                                        bool excludeDeleted = true,
                                                        bool includeDepartments = false,
                                                        bool includeAddress = false,
                                                        bool includePartner = true);

        /// <summary>
        ///     Retrieves all organization. Optional parameters may be provided to apply additional filtering, and for enabling eager loading. <br/>
        ///     The use of named parameters is recommended. <para>
        ///     
        ///     Example: <code>
        ///         var organizations = new OrganizationRepository().GetOrganizationsAsync(
        ///             true,
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
        /// <param name="cancellationToken"> The cancellation token, passed down from the API controller. </param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The number of items to retrieve for each <paramref name="page"/>. </param>
        /// <returns> A paginated-list containing the matching organizations. </returns>
        Task<PagedModel<Organization>> GetPaginatedOrganizationsAsync(bool asNoTracking,
                                                                      CancellationToken cancellationToken,
                                                                      Expression<Func<Organization, bool>>? whereFilter = null,
                                                                      bool customersOnly = true,
                                                                      bool excludeDeleted = true,
                                                                      bool includeDepartments = false,
                                                                      bool includeAddress = false,
                                                                      bool includePartner = false,
                                                                      int page = 1,
                                                                      int limit = 25);

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
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns> If found, the queried organization. If no matches are found, it returns <see langword="null"/>. </returns>
        Task<Organization?> GetOrganizationAsync(Guid organizationId,
                                                 Expression<Func<Organization, bool>>? whereFilter = null,
                                                 bool customersOnly = true,
                                                 bool excludeDeleted = true,
                                                 bool includeDepartments = false,
                                                 bool includeAddress = false,
                                                 bool includePartner = true,
                                                 bool includeLocations = false,
                                                 bool asNoTracking = false);

        /// <summary>
        ///     Finds an entity with the given primary key.
        /// </summary>
        /// <param name="id"> The primary key value. </param>
        /// <returns> If found, the retrieved entity, or <see langword="null"/> if no results was found. </returns>
        Task<Organization?> GetOrganizationAsync(int id);

        Task<Organization?> GetOrganizationByOrganizationNumber(string organizationNumber);

        /// <summary>
        ///     Retrieves the <see cref="OrganizationPreferences"/> for a given organization.
        /// </summary>
        /// <param name="organizationId"> The organization ID. </param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns> An asynchronous task. The task results contains the given organization's preferences, or <see langword="null"/> if no results were found. </returns>
        Task<OrganizationPreferences?> GetOrganizationPreferencesAsync(Guid organizationId, bool asNoTracking = false);
        
        Task<Location?> GetOrganizationLocationAsync(Guid locationId);
        Task<IList<Location>?> GetOrganizationAllLocationAsync(Guid organizationId);
        Task<Location> DeleteOrganizationLocationAsync(Location organizationLocation);
        Task<Organization> DeleteOrganizationAsync(Organization organization);

        /// <summary>
        ///     Retrieve the user attached to a given email-address. Optional parameters may be provided to apply additional filtering, and for enabling eager loading. <br/>
        ///     The use of named parameters is recommended.
        /// </summary>
        /// <param name="emailAddress"> The users email-address.</param>
        /// <param name="includeCustomer"> When <see langword="true"/>, it eagerly includes <see cref="User.Customer"/>. </param>
        /// <param name="includeDepartment"> When <see langword="true"/>, it eagerly includes <see cref="User.Department"/>. </param>
        /// <param name="includeUserPreference"> When <see langword="true"/>, it eagerly includes <see cref="User.UserPreference"/>. </param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns> An asynchronous task. The task results contains the user attached to the email-address, or <see langword="null"/> if no results were found. </returns>
        Task<User?> GetUserByEmailAddress(string emailAddress,
                                          bool includeCustomer = false,
                                          bool includeDepartment = false,
                                          bool includeUserPreference = false,
                                          bool asNoTracking = false);

        Task<User?> GetUserByMobileNumber(string mobileNumber, Guid organizationId);
        Task<PagedModel<UserDTO>> GetAllUsersAsync(Guid customerId, string[]? role, Guid[]? assignedToDepartment, IList<int>? userStatus, bool asNoTracking, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 25);

        /// <summary>
        ///     Retrieves a user by the organization and user IDs. Optional parameters may be provided to apply additional filtering, and for enabling eager loading. <br/>
        ///     The use of named parameters is recommended.
        /// </summary>
        /// <param name="organizationId"> The organizations ID. </param>
        /// <param name="userId"> The users ID. </param>
        /// <param name="includeDepartment"> When <see langword="true"/>, it eagerly includes <see cref="User.Department"/>. </param>
        /// <param name="includeUserPreference"> When <see langword="true"/>, it eagerly includes <see cref="User.UserPreference"/>. </param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns> An asynchronous task. The task results contains the user attached to the email-address, or <see langword="null"/> if no results were found. </returns>
        Task<User?> GetUserAsync(Guid organizationId, Guid userId, bool includeDepartment = false, bool includeUserPreference = false, bool asNoTracking = false);

        Task<User?> GetUserAsync(Guid userId);

        /// <summary>
        /// Retrieves all users within the given organization. Optional parameters may be provided to apply additional filtering, and for enabling eager loading. <br/>
        ///     The use of named parameters is recommended.
        /// </summary>
        /// <param name="organizationId"> The organization's ID. </param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <param name="includeUserPreference"> When <see langword="true"/>, it eagerly includes <see cref="User.UserPreference"/>. </param>
        /// <returns> An asynchronous task. The task results contains a list of all users within the organization. </returns>
        Task<IList<User>> GetUsersForCustomerAsync(Guid organizationId, bool asNoTracking, bool includeUserPreference = false);

        Task<User> AddUserAsync(User newUser);
        Task<User> DeleteUserAsync(User user);


        Task<Location> AddOrganizationLocationAsync(Location location);
        Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);
        Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);

        Task<IList<Department>> GetDepartmentsAsync(Guid organizationId, bool asNoTracking);

        /// <summary>
        ///     Retrieves a paginated department-list for a given organization.
        /// </summary>
        /// <param name="organizationId"> The organization to retrieve departments for. </param>
        /// <param name="includeManagers"> Should <see cref="Department.Managers"/> be loaded and included in the result? </param>
        /// <param name="asNoTracking"> 
        ///     Should the query be run using '<c>AsNoTracking</c>'? 
        ///     
        ///     <para>
        ///     To improve performance, this should be set to <see langword="true"/> for read-only operations.
        ///     However, if any write operations will occur, then this should always be set to <see langword="false"/>. </para>
        /// </param>
        /// <param name="cancellationToken"> The cancellation token, passed down from the API controller. </param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The number of items to retrieve for each <paramref name="page"/>. </param>
        /// <returns> 
        ///     A task that represents the asynchronous operation. The task result is a paginated dataset containing the retrieved departments.
        /// </returns>
        Task<PagedModel<Department>> GetPaginatedDepartmentsAsync(Guid organizationId, bool includeManagers, bool asNoTracking, CancellationToken cancellationToken, int page = 1, int limit = 25);

        Task<Department?> GetDepartmentAsync(Guid organizationId, Guid departmentId);
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
        /// <param name="includeOrganization"> When <see langword="true"/>, it eagerly includes <see cref="Partner.Organization"/>. </param>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns> If found, the corresponding partner. Otherwise it returns <see langword="null"/>. </returns>
        Task<Partner?> GetPartnerAsync(Guid partnerId, bool includeOrganization, bool asNoTracking = false);

        /// <summary>
        ///     Retrieves all partners.
        /// </summary>
        /// <param name="asNoTracking"> When <see langword="true"/> then query will explicitly be run as no-tracking. If the value is <see langword="false"/> 
        ///     the query will use the default behavior. <para>
        ///     
        ///     No tracking queries are useful when the results are used in a <i>read-only</i> scenario. They're quicker to execute because there's no need to set up the
        ///     change tracking information. If you don't need to update the entities retrieved from the database, then a no-tracking query should be used. See 
        ///     <see href="https://docs.microsoft.com/en-us/ef/core/querying/tracking"/> for more details. </para></param>
        /// <returns> A collection containing all partners. </returns>
        Task<IList<Partner>> GetPartnersAsync(bool asNoTracking);

        Task<Organization?> GetOrganizationByTechstepCustomerIdAsync(long techstepCustomerId);
        Task<IList<Guid>> GetOrganizationIdsForPartnerAsync(Guid partnerId);
        Task<List<UserNamesDTO>> GetAllUsersWithNameOnly(Guid customerId, CancellationToken cancellationToken);
    }
}
