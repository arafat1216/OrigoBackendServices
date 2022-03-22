﻿using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public interface IOrganizationRepository
    {
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);
        Task<Organization> AddAsync(Organization customer);
        Task<IList<CustomerUserCount>> GetOrganizationUserCountsAsync();
        Task<IList<Organization>> GetOrganizationsAsync();
        Task<IList<Organization>> GetOrganizationsAsync(Guid? parentId);
        Task<IList<Organization>> GetCustomersAsync();
        Task<IList<Organization>> GetCustomersAsync(Guid? parentId);
        Task<Organization> GetOrganizationAsync(Guid customerId);
        Task<Organization> GetCustomerAsync(Guid customerId);
        Task<Organization> GetOrganizationByOrganizationNumber(string organizationNumber);
        Task<OrganizationPreferences> GetOrganizationPreferencesAsync(Guid organizationId);
        Task<Location> GetOrganizationLocationAsync(Guid? locationId);
        Task<Location> DeleteOrganizationLocationAsync(Location organizationLocation);
        Task<Organization> DeleteOrganizationAsync(Organization organization);
        Task<User> GetUserByUserName(string emailAddress);
        Task<User> GetUserByMobileNumber(string mobileNumber);
        Task<int> GetUsersCount(Guid customerId);
        Task<PagedModel<User>> GetAllUsersAsync(Guid customerId, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 100);
        Task<User> GetUserAsync(Guid customerId, Guid userId);
        Task<User> GetUserAsync(Guid userId);
        Task<User> AddUserAsync(User newUser);
        Task<User> DeleteUserAsync(User user);


        Task<Location> AddOrganizationLocationAsync(Location location);
        Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);
        Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences);

        Task<IList<Department>> GetDepartmentsAsync(Guid customerId);
        Task<Department> GetDepartmentAsync(Guid customerId, Guid departmentId);
        Task<IList<Department>> DeleteDepartmentsAsync(IList<Department> department);
        Task<Partner> AddPartnerAsync(Partner partner);
        Task<Partner> GetPartnerAsync(Guid partnerId);
        Task<IList<Partner>> GetPartnersAsync();
    }
}
