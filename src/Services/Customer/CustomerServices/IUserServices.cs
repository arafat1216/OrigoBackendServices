using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomerServices.Models;

namespace CustomerServices
{
    public interface IUserServices
    {
        Task<IList<User>> GetAllUsersAsync(Guid customerId);
        Task<User> GetUserAsync(Guid customerId, Guid userId);
        Task<User> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName,
            string email, string mobileNumber, string employeeId);
    }
}