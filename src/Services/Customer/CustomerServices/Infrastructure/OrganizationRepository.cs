using Common.Extensions;
using Common.Interfaces;
using Common.Logging;
using Common.Utilities;
using CustomerServices.Infrastructure.Context;
using CustomerServices.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            _customerContext.Locations.Add(location);
            await SaveEntitiesAsync();
            return location;
        }

        public async Task<IList<Organization>> GetOrganizationsAsync()
        {
            return await _customerContext.Organizations.Where(o => !o.IsDeleted).ToListAsync();
        }

        public async Task<IList<CustomerUserCount>> GetOrganizationUserCountsAsync()
        {
            return await _customerContext.Users
            .Where(u => u.IsActive)
            .GroupBy(u => u.Customer.OrganizationId)
            .Select(group => new CustomerUserCount()
            {
                OrganizationId = group.Key,
                Count = group.Count()
            })
            .ToListAsync();
        }

        /// <summary>
        /// Get all organizations who has parentId as parent organization.
        /// If parentId is null, it will find all root organizations in the database.
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<IList<Organization>> GetOrganizationsAsync(Guid? parentId)
        {
            if (parentId == Guid.Empty)
                parentId = null;
            return await _customerContext.Organizations.Where(p => p.ParentId == parentId && !p.IsDeleted).ToListAsync();
        }

        /// <summary>
        /// Get all organizations who are also customers.
        /// </summary>
        /// <returns></returns>
        public async Task<IList<Organization>> GetCustomersAsync()
        {
            return await _customerContext.Organizations.Where(o => !o.IsDeleted && o.IsCustomer == true).ToListAsync();
        }

        /// <summary>
        /// Get all organizations who are also customers and who has parentId as parent organization.
        /// If parentId is null, it will find all root customers in the database.
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public async Task<IList<Organization>> GetCustomersAsync(Guid? parentId)
        {
            if (parentId == Guid.Empty)
                parentId = null;
            return await _customerContext.Organizations.Where(p => p.ParentId == parentId && !p.IsDeleted && p.IsCustomer == true).ToListAsync();
        }

        public async Task<Organization> GetOrganizationAsync(Guid customerId)
        {
            return await _customerContext.Organizations
            .Include(p => p.Departments)
            .FirstOrDefaultAsync(c => c.OrganizationId == customerId);
        }

        public async Task<Organization> GetCustomerAsync(Guid customerId)
        {
            return await _customerContext.Organizations
            .Include(p => p.Departments)
            .FirstOrDefaultAsync(c => c.OrganizationId == customerId && c.IsCustomer == true);
        }

        public async Task<OrganizationPreferences> GetOrganizationPreferencesAsync(Guid organizationId)
        {
            return await _customerContext.OrganizationPreferences
                .FirstOrDefaultAsync(c => c.OrganizationId == organizationId);
        }

        public async Task<Location> GetOrganizationLocationAsync(Guid? locationId)
        {
            return await _customerContext.Locations
                .FirstOrDefaultAsync(c => c.LocationId == locationId);
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

        private async Task<Organization> GetCustomerReadOnlyAsync(Guid customerId)
        {
            return await _customerContext.Organizations
                .Include(p => p.Departments)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.OrganizationId == customerId);
        }

        public async Task<Organization> GetOrganizationByOrganizationNumber(string organizationNumber)
        {
            if (organizationNumber == null)
                return null;
            return await _customerContext.Organizations.Where(c => c.OrganizationNumber == organizationNumber && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByUserName(string emailAddress)
        {
            if (emailAddress == null)
                return null;
            return await _customerContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Departments)
                .Include(u => u.UserPreference)
                .Where(u => u.Email == emailAddress)
                .FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByMobileNumber(string mobileNumber)
        {
            if (mobileNumber == null)
                return null;
            return await _customerContext.Users.Where(u => u.MobileNumber == mobileNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetUsersCount(Guid customerId)
        {
            return await _customerContext.Users.Where(u => u.Customer.OrganizationId == customerId && u.IsActive).CountAsync();
        }

        public async Task<PagedModel<User>> GetAllUsersAsync(Guid customerId, CancellationToken cancellationToken, string search = "", int page = 1, int limit = 100)
        {
            var users = _customerContext.Users
                    .Include(u => u.Customer)
                    .Include(u => u.UserPreference)
                    .Where(u => u.Customer.OrganizationId == customerId);

            if (!string.IsNullOrEmpty(search))
                users = users.Where(u => u.FirstName.ToLower().Contains(search.ToLower()) ||
                u.LastName.ToLower().Contains(search.ToLower()) ||
                u.Email.ToLower().Contains(search.ToLower()));

            return await users.OrderBy(u => u.FirstName).PaginateAsync(page, limit, cancellationToken);
        }

        public async Task<User> GetUserAsync(Guid customerId, Guid userId)
        {
            return await _customerContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Departments)
                .Include(u => u.UserPreference)
                .Where(u => u.Customer.OrganizationId == customerId && u.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _customerContext.Users
                .Include(u => u.Customer).Where(u => u.UserId == userId)
                .Include(u => u.UserPreference)
                .Include(u => u.Departments)
                .Include(u => u.ManagesDepartments)
                .FirstOrDefaultAsync();
        }

        public async Task<User> AddUserAsync(User newUser)
        {
            _customerContext.Users.Add(newUser);
            await SaveEntitiesAsync();
            return newUser;
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

        public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            int numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_customerContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                foreach (var @event in _customerContext.GetDomainEventsAsync())
                {
                    await _functionalEventLogService.SaveEventAsync(@event, _customerContext.Database.CurrentTransaction);
                }
                await _customerContext.SaveChangesAsync(cancellationToken);
                numberOfRecordsSaved = await _customerContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_customerContext);
            });
            return numberOfRecordsSaved;
        }

        public async Task<IList<Department>> GetDepartmentsAsync(Guid customerId)
        {
            return await _customerContext.Departments.Where(p => p.Customer.OrganizationId == customerId).ToListAsync();
        }

        public async Task<Department> GetDepartmentAsync(Guid customerId, Guid departmentId)
        {
            return await _customerContext.Departments.Include(d => d.ParentDepartment).FirstOrDefaultAsync(p => p.Customer.OrganizationId == customerId && p.ExternalDepartmentId == departmentId);
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
            _customerContext.Partners.Add(partner);
            await SaveEntitiesAsync();
            return partner;
        }

        public async Task<Partner> GetPartnerAsync(Guid partnerId)
        {
            return await _customerContext.Partners.Where(partner => !partner.IsDeleted && partner.ExternalId == partnerId).Include(partner => partner.Organization).FirstOrDefaultAsync();
        }

        public async Task<IList<Partner>> GetPartnersAsync()
        {
            return await _customerContext.Partners.Where(partner => !partner.IsDeleted)
                .Include(partner => partner.Organization).ToListAsync();

        }
    }
}
