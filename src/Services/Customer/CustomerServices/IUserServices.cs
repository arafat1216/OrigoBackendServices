﻿using CustomerServices.Models;
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
        Task<User> GetUserAsync(Guid customerId, Guid userId);
        Task<UserDTO> GetUserWithRoleAsync(Guid customerId, Guid userId);
        Task<User> AddUserForCustomerAsync(Guid customerId, string firstName, string lastName,
            string email, string mobileNumber, string employeeId, UserPreference userPreference);
        Task<User> UpdateUserPutAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, UserPreference userPreference);
        Task<User> UpdateUserPatchAsync(Guid customerId, Guid userId, string firstName, string lastName,
            string email, string employeeId, UserPreference userPreference);
        Task<User> DeleteUserAsync(Guid userId, bool softDelete = true);
        Task<User> SetUserActiveStatus(Guid customerId, Guid userId, bool isActive);
        Task<User> AssignDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task<User> UnassignDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task AssignManagerToDepartment(Guid customerId, Guid userId, Guid departmentId);
        Task UnassignManagerFromDepartment(Guid customerId, Guid userId, Guid departmentId);
    }
}