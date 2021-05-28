using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrigoApiGateway.Controllers;
using OrigoApiGateway.Models;

namespace OrigoApiGateway.Services
{
    public interface IUserServices
    {
        Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId);
        Task<IEnumerable<OrigoUser>> GetAllUsersAsync(Guid customerId);
        Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser);

    }
}
