#nullable enable
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
        Task<UserDTO> GetUserWithRoleAsync(Guid customerId, Guid userId);
        Task<UserDTO> GetUserWithRoleAsync(Guid userId);
        Task<UserDTO> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName,
            string email, string mobileNumber, string employeeId, UserPreference userPreference, Guid callerId, string role);
        Task<UserDTO> UpdateUserPutAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, string mobileNumber, UserPreference userPreference, Guid callerId);
        Task<UserDTO> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, string mobileNumber, UserPreference userPreference, Guid callerId);
        Task<UserDTO> DeleteUserAsync(Guid customerId, Guid userId, Guid callerId, bool softDelete = true);
        Task<UserDTO> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive, Guid callerId);
        Task<UserDTO> AssignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<UserDTO> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<UserInfo> GetUserInfoFromUserName(string userName);
        Task<UserInfo> GetUserInfoFromUserId(Guid userId);
        Task<UserDTO> InitiateOffboarding(Guid customerId, Guid userId, DateTime lastWorkingDay, bool buyoutAllowed, Guid callerId);
        Task<UserDTO> CancelOffboarding(Guid customerId, Guid userId, Guid callerId);
        Task<UserDTO> OverdueOffboarding(Guid customerId, Guid userId, Guid callerId);

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
    }
}