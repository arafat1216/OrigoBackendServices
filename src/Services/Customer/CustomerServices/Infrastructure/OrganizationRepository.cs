﻿using Common.Extensions;
using Common.Interfaces;
using Common.Logging;
using Common.Seedwork;
using Common.Utilities;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using CustomerServices.ServiceModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace CustomerServices.Infrastructure
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly CustomerContext _customerContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;

        public OrganizationRepository(CustomerContext customerContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _customerContext = customerContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        public async Task<Organization> AddAsync(Organization customer)
        {
            _customerContext.Organizations.Add(customer);
            await SaveEntitiesAsync();
            return customer;
        }

        public async Task<OrganizationPreferences> AddOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences)
        {
            _customerContext.OrganizationPreferences.Add(organizationPreferences);
            await SaveEntitiesAsync();
            return organizationPreferences;
        }

        public async Task<Location> AddOrganizationLocationAsync(Location location)
        {
            location.SetFieldsToEmptyIfNull();
            _customerContext.Locations.Add(location);
            await SaveEntitiesAsync();
            return location;
        }


        public async Task<IList<OrganizationUserCount>?> GetAllOrganizationsUsersCountAsync(Guid? partnerId, Guid[]? assignedToDepartment) 
        {
            if (partnerId.HasValue)
            {
                return await _customerContext.Users
                    .Include(u => u.Customer)
                    .ThenInclude(o => o.Partner)
                    .Where(u => u.Customer.Partner != null && u.Customer.Partner.ExternalId == partnerId && !u.IsDeleted)
                    .GroupBy(u => u.Customer.OrganizationId).Select(group =>
                    new OrganizationUserCount
                    {
                        OrganizationId = group.Key,
                        Count = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Activated).Count(),
                        NotOnboarded = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Invited ||
                                                        u.UserStatus == Common.Enums.UserStatus.OnboardInitiated ||
                                                        u.UserStatus == Common.Enums.UserStatus
                                                            .AwaitingConfirmation ||
                                                        u.UserStatus == Common.Enums.UserStatus.NotInvited).Count()
                    }).ToListAsync();
            }

            if (assignedToDepartment != null && assignedToDepartment.Any()) {
                return await _customerContext.Users
                                             .Where(u => assignedToDepartment.Contains(u.Customer.OrganizationId) && !u.IsDeleted)
                                             .GroupBy(u => u.Customer.OrganizationId)
                                             .Select(group => new OrganizationUserCount()
                                             {
                                                 OrganizationId = group.Key,
                                                 Count = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Activated).Count(),
                                                 NotOnboarded = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Invited ||
                                                                            u.UserStatus == Common.Enums.UserStatus.OnboardInitiated ||
                                                                            u.UserStatus == Common.Enums.UserStatus.AwaitingConfirmation ||
                                                                            u.UserStatus == Common.Enums.UserStatus.NotInvited).Count()
                                             })
                                             .ToListAsync();
            }

            if (partnerId == null && (assignedToDepartment == null || !assignedToDepartment.Any()))
            {
                return await _customerContext.Users
                    .Where(u => !u.IsDeleted)
                    .GroupBy(u => u.Customer.OrganizationId).Select(group =>
                    new OrganizationUserCount()
                    {
                        OrganizationId = group.Key,
                        Count = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Activated).Count(),
                        NotOnboarded = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Invited ||
                                                        u.UserStatus == Common.Enums.UserStatus.OnboardInitiated ||
                                                        u.UserStatus == Common.Enums.UserStatus
                                                            .AwaitingConfirmation ||
                                                        u.UserStatus == Common.Enums.UserStatus.NotInvited).Count()
                    }).ToListAsync();
            }

            return null;
        }

        public async Task<OrganizationUserCount?> GetOrganizationUsersCountAsync(Guid customerId, Guid[]? assignedToDepartment, string[]? role)
        {

            if (assignedToDepartment != null)
            {
                return await _customerContext.Users
                                            .Include(c => c.Customer)
                                            .Include(d => d.Department)
                                            .Where(user => user.Department != null && !user.IsDeleted && user.Customer.OrganizationId == customerId && assignedToDepartment.Contains(user.Department.ExternalDepartmentId))
                                            .GroupBy(a => a.Customer.OrganizationId)
                                            .Select(group => new OrganizationUserCount()
                                            {
                                                OrganizationId = group.Key,
                                                Count = group.Where(u => u.UserStatus != Common.Enums.UserStatus.Deactivated &&
                                                                         u.UserStatus != Common.Enums.UserStatus.OffboardCompleted &&
                                                                         u.UserStatus != Common.Enums.UserStatus.OffboardOverdue).Count(),
                                                NotOnboarded = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Invited ||
                                                                           u.UserStatus == Common.Enums.UserStatus.OnboardInitiated ||
                                                                           u.UserStatus == Common.Enums.UserStatus.AwaitingConfirmation ||
                                                                           u.UserStatus == Common.Enums.UserStatus.NotInvited).Count() 
                                            }).FirstOrDefaultAsync();
            }

            if (role != null && role.Any())
            {

                if (role.Contains("CustomerAdmin") || role.Contains("PartnerAdmin") ||
                    role.Contains("SystemAdmin") || role.Contains("GroupAdmin") ||
                    role.Contains("Admin") || role.Contains("PartnerReadOnlyAdmin"))
                {
                    return await _customerContext.Users.Where(user => user.Customer.OrganizationId == customerId && !user.IsDeleted)
                                            .GroupBy(a => a.Customer.OrganizationId)
                                            .Select(group => new OrganizationUserCount()
                                            {
                                                OrganizationId = group.Key,
                                                Count = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Activated).Count(),
                                                NotOnboarded = group.Where(u => u.UserStatus == Common.Enums.UserStatus.Invited ||
                                                                           u.UserStatus == Common.Enums.UserStatus.OnboardInitiated ||
                                                                           u.UserStatus == Common.Enums.UserStatus.AwaitingConfirmation ||
                                                                           u.UserStatus == Common.Enums.UserStatus.NotInvited).Count() 
                                            }).FirstOrDefaultAsync();
                }
            }

          return null;
        }

        public async Task<IList<Organization>> GetOrganizationsAsync(Expression<Func<Organization, bool>>? whereFilter = null,
                                                                     bool customersOnly = true,
                                                                     bool excludeDeleted = true,
                                                                     bool includeDepartments = false,
                                                                     bool includeAddress = false,
                                                                     bool includePartner = true,
                                                                     bool asNoTracking = true)
        {
            IQueryable<Organization> query = _customerContext.Set<Organization>();

            // Parameterized where filtering
            if (whereFilter is not null)
                query = query.Where(whereFilter);

            if (customersOnly)
                query = query.Where(e => e.IsCustomer);

            if (excludeDeleted)
                query = query.Where(e => !e.IsDeleted);

            // Parameterized Includes
            if (includeAddress)
                query = query.Include(e => e.Address);

            if (includeDepartments)
                query = query.Include(e => e.Departments);

            if (includePartner)
                query = query.Include(e => e.Partner);

            // Misc
            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<Organization?> GetOrganizationAsync(Guid organizationId,
                                                              Expression<Func<Organization, bool>>? whereFilter = null,
                                                              bool customersOnly = true,
                                                              bool excludeDeleted = true,
                                                              bool includeDepartments = false,
                                                              bool includeAddress = false,
                                                              bool includePartner = true,
                                                              bool includeLocations = false)
        {
            IQueryable<Organization> query = _customerContext.Set<Organization>();

            // Parameterized where filtering
            if (whereFilter is not null)
                query = query.Where(whereFilter);

            if (customersOnly)
                query = query.Where(e => e.IsCustomer);

            if (excludeDeleted)
                query = query.Where(e => !e.IsDeleted);

            // Parameterized Includes
            if (includeAddress)
                query = query.Include(e => e.Address);

            if (includeDepartments)
                query = query.Include(e => e.Departments);

            if (includePartner)
                query = query.Include(e => e.Partner);

            if (includeLocations)
                query = query.Include(x => x.Locations);

            return await query.AsSplitQuery().FirstOrDefaultAsync(e => e.OrganizationId == organizationId);
        }

        public async Task<Organization?> GetOrganizationAsync(int id)
        {
            return await _customerContext.Organizations
                                         .FindAsync(id);
        }

        public async Task<OrganizationPreferences?> GetOrganizationPreferencesAsync(Guid organizationId)
        {
            return await _customerContext.OrganizationPreferences
                .FirstOrDefaultAsync(c => c.OrganizationId == organizationId);
        }

        public async Task<Location?> GetOrganizationLocationAsync(Guid locationId)
        {
            return await _customerContext.Locations
                .FirstOrDefaultAsync(c => c.ExternalId == locationId);
        }
        public async Task<IList<Location>> GetOrganizationAllLocationAsync(Guid organizationId)
        {
            var org = await _customerContext.Organizations.Include(x => x.Locations).FirstOrDefaultAsync(
                x => x.OrganizationId == organizationId);
            if(org == null || !org.Locations.Any()) return null;
            return org.Locations.ToList();
        }

        public async Task<OrganizationPreferences> DeleteOrganizationPreferencesAsync(OrganizationPreferences organizationPreferences)
        {
            try
            {
                _customerContext.OrganizationPreferences.Remove(organizationPreferences);
            }
            catch
            {
                // item is already removed or did not exist
            }

            await SaveEntitiesAsync();
            return organizationPreferences;
        }

        public async Task<Organization> DeleteOrganizationAsync(Organization organization)
        {
            try
            {
                _customerContext.Organizations.Remove(organization);
            }
            catch
            {
                // item is already removed or did not exist
            }

            await SaveEntitiesAsync();
            return organization;
        }

        public async Task<Location> DeleteOrganizationLocationAsync(Location organizationLocation)
        {
            try
            {
                _customerContext.Locations.Remove(organizationLocation);
            }
            catch
            {
                // item is already removed or did not exist
            }

            await SaveEntitiesAsync();
            return organizationLocation;
        }

        public async Task<Organization?> GetOrganizationByOrganizationNumber(string organizationNumber)
        {
            if (organizationNumber == null)
                return null;

            return await _customerContext.Organizations
                .Where(c => c.OrganizationNumber == organizationNumber && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByUserName(string emailAddress)
        {
            if (emailAddress == null)
                return null;

            return await _customerContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Department)
                .Include(u => u.UserPreference)
                .Where(u => u.Email == emailAddress)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByMobileNumber(string mobileNumber, Guid organizationId)
        {
            if (string.IsNullOrEmpty(mobileNumber))
                return null;

            return await _customerContext.Users.Include(u => u.Customer)
                .Where(u => u.MobileNumber == mobileNumber && u.Customer.OrganizationId == organizationId)
                .FirstOrDefaultAsync();
        }

      
        public async Task<PagedModel<UserDTO>> GetAllUsersAsync(Guid customerId, string[]? role, Guid[]? assignedToDepartment, IList<int>? userStatus, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 100)
        {
            var users = _customerContext.Users
                .Include(u => u.Customer)
                    .Include(u => u.UserPreference)
                    .Include(u => u.Department)
                    .Include(u => u.ManagesDepartments)
                    .Where(u => u.Customer.OrganizationId == customerId && !u.IsDeleted)
                    .SelectMany(
                         u => _customerContext.UserPermissions
                        .Where(up => u.Id == up.User.Id).DefaultIfEmpty(),
                        (u, up) => new UserDTO
                        {
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            AssignedToDepartment = u.Department == null ? Guid.Empty : u.Department.ExternalDepartmentId,
                            DepartmentName = u.Department == null ? null : u.Department.Name,
                            Email = u.Email,
                            OrganizationName = u.Customer.Name,
                            EmployeeId = u.EmployeeId,
                            MobileNumber = u.MobileNumber,
                            Role = up.Role.Name,
                            UserPreference = u.UserPreference == null ? null : new UserPreferenceDTO { Language = u.UserPreference.Language },
                            Id = u.UserId,
                            UserStatus = (int)u.UserStatus,
                            UserStatusName = u.UserStatus.ToString(),
                            LastWorkingDay = u.LastWorkingDay,
                            LastDayForReportingSalaryDeduction = u.LastDayForReportingSalaryDeduction,
                            ManagerOf = u.ManagesDepartments.Select(a => new ManagerOfDTO { DepartmentId = a.ExternalDepartmentId, DepartmentName = a.Name }).ToList()
                        });


            if (!string.IsNullOrEmpty(search))
                users = users.Where(u => u.FirstName.ToLower().Contains(search.ToLower()) ||
                u.LastName.ToLower().Contains(search.ToLower()) ||
                u.Email.ToLower().Contains(search.ToLower()));

            if (userStatus != null)
            {
                users = users.Where(al => userStatus.Contains(al.UserStatus));
            }
            if (assignedToDepartment != null)
            {
                users = users.Where(al => assignedToDepartment.Contains(al.AssignedToDepartment));
            }
            if (role != null)
            {
                users = users.Where(a => role.Contains(a.Role));
            }

            return await users.OrderBy(u => u.FirstName).PaginateAsync(page, limit, cancellationToken);
        }

        public async Task<User?> GetUserAsync(Guid customerId, Guid userId)
        {
            return await _customerContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Department)
                .Include(u => u.UserPreference)
                .Where(u => u.Customer.OrganizationId == customerId && u.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserAsync(Guid userId)
        {
            return await _customerContext.Users
                .Include(u => u.Customer).Where(u => u.UserId == userId)
                .Include(u => u.UserPreference)
                .Include(u => u.Department)
                .Include(u => u.ManagesDepartments)
                .FirstOrDefaultAsync();
        }

        public async Task<User> AddUserAsync(User newUser)
        {
            _customerContext.Users.Add(newUser);
            await SaveEntitiesAsync();
            return newUser;
        }
        public async Task<IList<User>> GetUsersForCustomerAsync(Guid organizationId)
        {
            return await _customerContext.Users
                .Include(u => u.Customer).Where(u => u.Customer.OrganizationId == organizationId)
                .Include(u => u.UserPreference)
                .ToListAsync();
        }

        public async Task<User> DeleteUserAsync(User user)
        {
            _customerContext.Users.Remove(user);
            try
            {
                _customerContext.Entry(user.UserPreference).State = EntityState.Deleted;
            }
            catch
            {
                // User don't have a userpreference. This should not happen.
            }
            await SaveEntitiesAsync();
            return user;
        }

        public async Task SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            await ResilientTransaction.New(_customerContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _customerContext.SaveChangesAsync(cancellationToken);
                if (!_customerContext.IsSQLite)
                {
                    foreach (var @event in _customerContext.GetDomainEventsAsync())
                    {
                        await _functionalEventLogService.SaveEventAsync(@event, _customerContext.Database.CurrentTransaction!);
                    }
                }
                await _mediator.DispatchDomainEventsAsync(_customerContext);
            });
        }

        public async Task<IList<Department>> GetDepartmentsAsync(Guid organizationId)
        {
            return await _customerContext.Organizations.Where(a => a.OrganizationId == organizationId).Include(d => d.Departments).ThenInclude(a => a.Managers).SelectMany(a => a.Departments).ToListAsync();
        }

        public async Task<Department?> GetDepartmentAsync(Guid organizationId, Guid departmentId)
        {
            return await _customerContext.Departments.Include(d => d.ParentDepartment).Include(m => m.Managers).FirstOrDefaultAsync(p => p.Customer.OrganizationId == organizationId && p.ExternalDepartmentId == departmentId);
        }

        public async Task<IList<Department>> DeleteDepartmentsAsync(IList<Department> department)
        {
            try
            {
                _customerContext.Departments.RemoveRange(department);
            }
            catch
            {
                // item is already removed or did not exist
            }
            await SaveEntitiesAsync();
            return department;
        }

        public async Task<Partner> AddPartnerAsync(Partner partner)
        {
            _customerContext.Partners
                            .Add(partner);

            await SaveEntitiesAsync();
            return partner;
        }

        public async Task<Partner?> GetPartnerAsync(Guid partnerId)
        {
            return await _customerContext.Partners
                                         .Where(partner => !partner.IsDeleted && partner.ExternalId == partnerId)
                                         .Include(e => e.Organization)
                                         .FirstOrDefaultAsync();
        }

        public async Task<bool> OrganizationIsPartner(int organizationId)
        {
            return await _customerContext.Partners
                                         .AnyAsync(e => e.Organization.Id == organizationId);
        }

        public async Task<IList<Partner>> GetPartnersAsync()
        {
            return await _customerContext.Partners
                                         .Where(partner => !partner.IsDeleted)
                                         .Include(o => o.Organization)
                                         .OrderBy(o => o.Organization.Name)
                                         .ToListAsync();
        }

        public async Task<Organization?> GetOrganizationByTechstepCustomerIdAsync(long techstepCustomerId)
        {
            return await _customerContext.Organizations.FirstOrDefaultAsync(a => a.TechstepCustomerId == techstepCustomerId);
        }

        public async Task<IList<Guid>> GetOrganizationIdsForPartnerAsync(Guid partnerId)
        {
            var organizationList = await _customerContext.Organizations
                            .Where(o => !o.IsDeleted && !(o.Partner == null) && o.Partner.ExternalId == partnerId)
                            .Include(o => o.Partner)
                            .ThenInclude(p => p.Organization)
                            .Select(o => new {OrgId = o.OrganizationId, PartnerOrgId = o.Partner!.Organization.OrganizationId})
                            .OrderByDescending(s => s.OrgId == s.PartnerOrgId) // Sort to get partner organization first
                            .ToListAsync();
            return organizationList.Select(o => o.OrgId).ToList();
        }
    }
}
