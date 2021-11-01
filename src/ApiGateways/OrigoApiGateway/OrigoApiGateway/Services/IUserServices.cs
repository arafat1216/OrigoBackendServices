using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IUserServices
    {
        Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId);
        Task<IEnumerable<OrigoUser>> GetAllUsersAsync(Guid customerId, IReadOnlyCollection<Guid> filteredDepartments = null);
        Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser);
        Task<OrigoUser> PutUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser);
        Task<OrigoUser> PatchUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser);
        Task<bool> DeleteUserAsync(Guid customerId, Guid userId, bool softDelete);
        Task<OrigoUser> AssignUserToDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task<OrigoUser> UnassignUserFromDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId);
    }
}
