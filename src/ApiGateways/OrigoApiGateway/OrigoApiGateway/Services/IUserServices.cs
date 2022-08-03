using Common.Interfaces;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IUserServices
    {
        Task<int> GetUsersCountAsync(Guid customerId, FilterOptionsForUser filterOptions);
        Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId);
        Task<OrigoUser> GetUserAsync(Guid userId);
        Task<PagedModel<OrigoUser>> GetAllUsersAsync(Guid customerId, FilterOptionsForUser filterOptions, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 1000);
        Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser, Guid callerId);
        Task<OrigoUser> PutUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);
        Task<OrigoUser> PatchUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);
        Task<OrigoUser> DeleteUserAsync(Guid customerId, Guid userId, bool softDelete, Guid callerId);
        Task<OrigoUser> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive, Guid callerId);
        Task<OrigoUser> AssignUserToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<OrigoUser> UnassignUserFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<UserInfoDTO> GetUserInfo(string userName, Guid userId);
        Task<OrigoUser> InitiateOffboarding(Guid customerId, Guid userId, string role, List<Guid> departments, OffboardInitiate offboardDate, Guid callerId);
        Task<OrigoUser> CancelOffboarding(Guid customerId, Guid userId, string role, List<Guid> departments, Guid callerId);
    }
}
