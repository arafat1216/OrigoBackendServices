﻿using Common.Interfaces;
using OrigoApiGateway.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IUserServices
    {
        Task<int> GetUsersCountAsync(Guid customerId);
        Task<OrigoUser> GetUserAsync(Guid customerId, Guid userId);
        Task<OrigoUser> GetUserAsync(Guid userId);
        Task<PagedModel<OrigoUser>> GetAllUsersAsync(Guid customerId, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 1000);
        Task<OrigoUser> AddUserForCustomerAsync(Guid customerId, NewUser newUser, Guid callerId);
        Task<OrigoUser> PutUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);
        Task<OrigoUser> PatchUserAsync(Guid customerId, Guid userId, OrigoUpdateUser updateUser, Guid callerId);
        Task<bool> DeleteUserAsync(Guid customerId, Guid userId, bool softDelete, Guid callerId);
        Task<OrigoUser> SetUserActiveStatusAsync(Guid customerId, Guid userId, bool isActive, Guid callerId);
        Task<OrigoUser> AssignUserToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task<OrigoUser> UnassignUserFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId, Guid callerId);
    }
}
