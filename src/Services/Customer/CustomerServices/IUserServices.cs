﻿#nullable enable
using Common.Interfaces;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IUserServices
    {
        Task<OrganizationUserCount?> GetUsersCountAsync(Guid customerId, Guid[]? assignedToDepartment, string[]? role);        
        Task<PagedModel<UserDTO>> GetAllUsersAsync(Guid customerId, string[]? role, Guid[]? assignedToDepartment, IList<int>? userStatus, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 100);
        Task<PagedModel<UserDTO>> GetAllScimUsersAsync(string? userName, CancellationToken cancellationToken, int startIndex = 0, int limit = 25);
        /// <summary>
        /// Gets a list of all users for a customer. The list only contains user id and full name and is sorted by userid so we can do a faster lookup.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<UserNamesDTO>> GetAllUsersWithNameOnly(Guid customerId, CancellationToken cancellationToken);
        Task<UserDTO> GetUserWithRoleAsync(Guid customerId, Guid userId);
        Task<UserDTO> GetUserWithRoleAsync(Guid userId);
        Task<UserDTO> GetUserByPhoneNumberAsync(Guid customerId, string phoneNumber);
        Task<UserDTO> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName, string email,
            string mobileNumber, string employeeId, UserPreference userPreference, Guid callerId, string role,
            bool newUserNeedsOnboarding, bool newUserNotToBeAddedToOkta);
        Task<UserDTO> UpdateUserPutAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, string mobileNumber, UserPreference userPreference, Guid callerId);
        Task<UserDTO> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, string mobileNumber, UserPreference userPreference, Guid callerId);
        Task<UserDTO> DeleteUserAsync(Guid customerId, Guid userId, Guid callerId, bool softDelete = true);
        Task<UserDTO> DeleteUserAsync(Guid userId, Guid callerId, bool softDelete = true);
        Task<UserDTO> DeleteUserByPhoneNumberAsync(Guid customerId, string phoneNumber, Guid callerId, bool softDelete = true);
        Task<UserDTO> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive, Guid callerId);
        Task<UserDTO> AssignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<UserDTO> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<UserInfoDTO> GetUserInfoFromUserName(string userName);
        Task<UserInfoDTO> GetUserInfoFromUserId(Guid userId);
        Task<UserInfoDTO> GetUserInfoFromPhoneNumber(Guid organizationId, string mobileNumber);
        Task<UserDTO> InitiateOffboarding(Guid customerId, Guid userId, DateTime lastWorkingDay, bool buyoutAllowed, Guid callerId);
        Task<UserDTO> CancelOffboarding(Guid customerId, Guid userId, Guid callerId);
        Task<UserDTO> OverdueOffboarding(Guid customerId, Guid userId, Guid callerId);
        /// <summary>
        /// Activates a user after onboarding wizzard is completed.
        /// </summary>
        /// <param name="customerId">Users connected organization.</param>
        /// <param name="userId">User to be activated.</param>
        /// <returns>A user dto.</returns>
        Task<UserDTO> CompleteOnboardingAsync(Guid customerId, Guid userId);
        /// <summary>
        /// Completing offboarding and updating user status to 'OffboardingCompleted'
        /// Removing access of this user from Okta
        /// </summary>
        /// <param name="customerId">Users connected organization.</param>
        /// <param name="userId">User to be Completing offboarding.</param>
        /// <param name="callerId">ID of the user making request.</param>
        /// <returns>offboard completed user dto.</returns>
        Task<UserDTO> CompleteOffboarding(Guid customerId, Guid userId, Guid callerId);


        /// <summary>
        /// Resends the Origo invitation mail to a list of users. 
        /// </summary>
        /// <param name="customerId">CustomerId of the user.</param>
        /// <param name="userIds">A list of user ids to send the Origo Invitation to.</param>
        /// <param name="role">The roles of the user making the request. Only for partner admin, admin and managers.</param>
        /// <param name="assignedToDepartment">The assigned departments of a manager. Will only contain guids if the role of the user making the request is a manager, otherwise the list will be null.</param>
        /// <returns>Asynchronously task that returns a list of strings with information about exceptions.</returns>
        /// <exception cref="Exceptions.UserNotFoundException">If user with the user id provided is not found.</exception>
        Task<ExceptionMessagesDTO> ResendOrigoInvitationMail(Guid customerId, IList<Guid> userIds, string[]? role, Guid[]? assignedToDepartment);
        Task SubscriptionHandledForOffboardingAsync(Guid customerId, string mobileNumber, Guid callerId);

#nullable enable
        /// <summary>
        ///     An advanced search that retrieves all <c>UserDTO</c> entities that matches the given criteria.
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
        /// <returns> The asynchronous task. The task results contains the corresponding <see cref="UserDTO"/> results. </returns>
        Task<PagedModel<UserDTO>> UserAdvancedSearchAsync(UserSearchParameters searchParameters,
                                                          int page,
                                                          int limit,
                                                          CancellationToken cancellationToken,
                                                          bool includeUserPreferences = false,
                                                          bool includeDepartmentInfo = false,
                                                          bool includeOrganizationDetails = false,
                                                          bool includeRoleDetails = false);
    }
#nullable restore
}