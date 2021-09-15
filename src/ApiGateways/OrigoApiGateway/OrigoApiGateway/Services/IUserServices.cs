using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IUserServices
    {
        Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId);
        Task<IEnumerable<OrigoUser>> GetAllUsersAsync(Guid customerId);
        Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser);
        Task<OrigoUser> UpdateUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser);
        Task<OrigoUser> DeleteUserAsync(Guid customerId, Guid userId);
    }
}
