using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerServices.ServiceModels;

namespace CustomerServices
{
    public interface IUserServices
    {
        Task<int> GetUsersCountAsync(Guid customerId);
        Task<IList<UserDTO>> GetAllUsersAsync(Guid customerId);
        Task<UserDTO> GetUserWithRoleAsync(Guid customerId, Guid userId);
        Task<UserDTO> GetUserWithRoleAsync(Guid userId);
        Task<UserDTO> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName,
            string email, string mobileNumber, string employeeId, UserPreference userPreference, Guid callerId);
        Task<UserDTO> UpdateUserPutAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, UserPreference userPreference, Guid callerId);
        Task<UserDTO> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, UserPreference userPreference, Guid callerId);
        Task<UserDTO> DeleteUserAsync(Guid userId, Guid callerId, bool softDelete = true);
        Task<UserDTO> SetUserActiveStatus(Guid customerId, Guid userId, bool isActive, Guid callerId);
        Task<UserDTO> AssignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<UserDTO> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
    }
}