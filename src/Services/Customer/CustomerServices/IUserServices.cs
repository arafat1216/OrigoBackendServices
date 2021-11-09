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
        Task<User> AssignOktaUserIdAsync(Guid customerId, Guid userId, string oktaUserId);
        Task<User> SetUserActiveStatus(Guid customerId, Guid userId, bool isActive);
        Task<User> AssignDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task<User> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId);
    }
}