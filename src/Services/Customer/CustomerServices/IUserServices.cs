using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IUserServices
    {
        Task<IList<User>> GetAllUsersAsync(Guid customerId);
        Task<User> GetUserAsync(Guid customerId, Guid userId);
        Task<User> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName,
            string email, string mobileNumber, string employeeId);
        Task<User> UpdateUserPostAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId);
        Task<User> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId);
        Task<User> DeleteUserAsync(Guid userId, bool softDelete = true);
        Task<User> AssignDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task<User> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId);
    }
}