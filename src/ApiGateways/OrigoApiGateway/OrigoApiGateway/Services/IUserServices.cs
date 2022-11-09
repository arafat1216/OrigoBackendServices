#nullable enable
using Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.Asset.Backend;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.Customer.Backend;
using System.Security.Claims;

namespace OrigoApiGateway.Services
{
    public interface IUserServices
    {
        Task<CustomerUserCount> GetUsersCountAsync(Guid customerId, FilterOptionsForUser filterOptions);
        Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId);
        Task<OrigoUser> GetUserAsync(Guid userId);
        Task<OrigoMeUser?> GetUserWithPermissionsAsync(Guid? customerId, Guid mainOrganizationId, Guid userId, List<string> permissions,
            List<string> accessList);
        Task<PagedModel<OrigoUser>> GetAllUsersAsync(Guid customerId, FilterOptionsForUser filterOptions, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 25);
        Task<HashSet<UserNamesDTO>> GetAllUsersNamesAsync(Guid customerId, CancellationToken cancellationToken);
        Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser, Guid callerId, bool includeOnboarding);
        Task<OrigoUser> PutUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);
        Task<OrigoUser> PatchUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);
        Task<OrigoUser> DeleteUserAsync(Guid customerId, Guid userId, bool softDelete, Guid callerId);
        Task<OrigoUser> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive, Guid callerId);
        Task<OrigoUser> AssignUserToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<OrigoUser> UnassignUserFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<UserInfoDTO> GetUserInfo(string userName, Guid userId);
        Task<UserInfoDTO?> GetUserWithPhoneNumber(Guid customerId, string mobileNumber);
        Task<OrigoUser> InitiateOffboarding(Guid customerId, Guid userId, string role, List<Guid> departments, OffboardInitiate offboardDate, IList<LifeCycleSetting> lifeCycleSettings, Guid callerId);
        Task<OrigoUser> CancelOffboarding(Guid customerId, Guid userId, string role, List<Guid> departments, Guid callerId);
        Task<OrigoExceptionMessages> ResendOrigoInvitationMail(Guid customerId, InviteUsers users, FilterOptionsForUser filterOptions);
        Task<OrigoUser> CompleteOnboardingAsync(Guid customerId, Guid userId);
        Task<(bool correctOrganization, Guid userId)> UserEmailLinkedToGivenOrganization(Guid organizationId, string userEmail);
        Task SubscriptionHandledForOffboardingAsync(Guid customerId, string mobileNumber);

#nullable enable
        /// <summary>
        ///     An advanced search that retrieves all users that matches the given criteria.
        /// </summary>
        /// <param name="searchParameters"> A class containing all the search-parameters. </param>
        /// <param name="page"> The current page number. </param>
        /// <param name="limit"> The highest number of items that can be added in a single page. </param>
        /// <param name="cancellationToken"> A injected <see cref="CancellationToken"/>. </param>
        /// <param name="includeUserPreferences">
        ///     When <c><see langword="true"/></c>, information about the users preferences is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeDepartmentInfo">
        ///     When <c><see langword="true"/></c>, the users department information is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeOrganizationDetails">
        ///     When <c><see langword="true"/></c>, the users organization details is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <param name="includeRoleDetails">
        ///     When <c><see langword="true"/></c>, the users role details is loaded/included in the retrieved data. 
        ///     <para>This property will not be included unless it's explicitly requested. </para>
        /// </param>
        /// <returns> The asynchronous task. The task results contains a paged list with the corresponding users. </returns>
        Task<PagedModel<OrigoUser>> UserAdvancedSearch(UserSearchParameters searchParameters,
                                                       int page,
                                                       int limit,
                                                       CancellationToken cancellationToken,
                                                       bool includeUserPreferences = false,
                                                       bool includeDepartmentInfo = false,
                                                       bool includeOrganizationDetails = false,
                                                       bool includeRoleDetails = false);
#nullable restore
    }
}
