using Common.Extensions;
using Common.Logging;
using Common.Utilities;
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
            .Include(p => p.SelectedProductModules)
            .ThenInclude(p => p.ProductModuleGroup)
            .Include(p => p.SelectedProductModuleGroups)
            .Include(p => p.SelectedAssetCategories)
            .ThenInclude(p => p.LifecycleTypes)
            .Include(p => p.Departments)
            .FirstOrDefaultAsync(c => c.OrganizationId == customerId);
        }

        public async Task<Organization> GetCustomerAsync(Guid customerId)
        {
            return await _customerContext.Organizations
            .Include(p => p.SelectedProductModules)
            .ThenInclude(p => p.ProductModuleGroup)
            .Include(p => p.SelectedProductModuleGroups)
            .Include(p => p.SelectedAssetCategories)
            .ThenInclude(p => p.LifecycleTypes)
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
                .Include(p => p.SelectedProductModules)
                .ThenInclude(p => p.ProductModuleGroup)
                .Include(p => p.SelectedProductModuleGroups)
                .Include(p => p.SelectedAssetCategories)
                .ThenInclude(p => p.LifecycleTypes)
                .Include(p => p.Departments)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.OrganizationId == customerId);
        }

        public async Task<User> GetUserByUserName(string emailAddress)
        {
            return await _customerContext.Users
                .Include(u => u.Customer)
                .Include(u => u.Departments)
                .Include(u => u.UserPreference)
                .Where(u => u.Email.ToLower() == emailAddress.ToLower())
                .FirstOrDefaultAsync();
        }
        public async Task<User> GetUserByMobileNumber(string mobileNumber)
        {
            return await _customerContext.Users.Where(u => u.MobileNumber == mobileNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetUsersCount(Guid customerId)
        {
            return await _customerContext.Users.CountAsync(u => u.Customer.OrganizationId == customerId);
        }

        public async Task<IList<User>> GetAllUsersAsync(Guid customerId)
        {
            return await _customerContext.Users
                .Include(u => u.Customer)
                .Include(u => u.UserPreference)
                .Where(u => u.Customer.OrganizationId == customerId)
                .ToListAsync();
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

        public async Task<IList<AssetCategoryLifecycleType>> DeleteAssetCategoryLifecycleTypeAsync(Organization customer, AssetCategoryType assetCategory, IList<AssetCategoryLifecycleType> assetCategoryLifecycleTypes, Guid callerId)
        {
            try
            {
                foreach (var assetLifecycle in assetCategoryLifecycleTypes)
                {
                    customer.RemoveLifecyle(assetCategory, assetLifecycle,callerId);
                }
                _customerContext.AssetCategoryLifecycleTypes.RemoveRange(assetCategoryLifecycleTypes);
            }
            catch
            {
                // item is already removed or did not exsit
            }
            await SaveEntitiesAsync();
            return assetCategoryLifecycleTypes;
        }

        public async Task<AssetCategoryType> GetAssetCategoryTypeAsync(Guid customerId, int assetCategoryId)
        {
            return await _customerContext.AssetCategoryTypes.Include(p => p.LifecycleTypes).FirstOrDefaultAsync(p => p.Id == assetCategoryId && p.ExternalCustomerId == customerId);
        }

        public async Task<AssetCategoryType> GetAssetCategoryTypeAsync(Guid customerId, Guid assetCategoryId)
        {
            return await _customerContext.AssetCategoryTypes.Include(p => p.LifecycleTypes).FirstOrDefaultAsync(p => p.AssetCategoryId == assetCategoryId && p.ExternalCustomerId == customerId);
        }

        public async Task<IList<AssetCategoryType>> GetAssetCategoryTypesAsync(Guid customerId)
        {
            var customer = await GetCustomerAsync(customerId);
            if (customer == null)
                return null;
            return customer.SelectedAssetCategories.ToList();
        }

        public async Task<AssetCategoryType> DeleteAssetCategoryTypeAsync(AssetCategoryType assetCategoryType)
        {
            try
            {
                _customerContext.AssetCategoryTypes.Remove(assetCategoryType);
            }
            catch
            {
                // item is already removed or did not exsit
            }
            await SaveEntitiesAsync();
            return assetCategoryType;
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

        public async Task<IList<ProductModule>> GetProductModulesAsync()
        {
            return await _customerContext.ProductModules.Include(p => p.ProductModuleGroup).ToListAsync();
        }

        public async Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId)
        {
            var customer = await GetCustomerReadOnlyAsync(customerId);

            // Customer specific service
            if (customer.IsCustomer == false)
                return null;

            var customerModuleGroups = customer.SelectedProductModuleGroups;
            var customerModules = customer.SelectedProductModules.ToList();
            foreach (var module in customerModules)
            {
                var tempModuleGroup = customerModuleGroups.Where(mg => mg.ProductModuleExternalId == module.ProductModuleId);
                module.ProductModuleGroup.Clear();
                foreach (var moduleGroup in tempModuleGroup)
                {
                    module.ProductModuleGroup.Add(moduleGroup);
                }
            }
            return customerModules;
        }

        public async Task<ProductModule> GetProductModuleAsync(Guid moduleId)
        {
            return await _customerContext.ProductModules.Include(m => m.ProductModuleGroup).FirstOrDefaultAsync(p => p.ProductModuleId == moduleId);
        }

        public async Task<ProductModule> AddProductModuleAsync(Guid customerId, Guid moduleId, Guid callerId)
        {
            var customer = await GetCustomerAsync(customerId);
            var module = await GetProductModuleAsync(moduleId);
            if (customer == null || module == null)
            {
                return null;
            }

            if (!customer.SelectedProductModules.Contains(module))
            {
                customer.AddProductModule(module, callerId);
            }
            await SaveEntitiesAsync();
            return await _customerContext.ProductModules.Include(m => m.ProductModuleGroup).FirstOrDefaultAsync(p => p.ProductModuleId == moduleId);
        }

        public async Task<ProductModule> RemoveProductModuleAsync(Guid customerId, Guid moduleId, Guid callerId)
        {
            var customer = await GetCustomerAsync(customerId);
            var module = await GetProductModuleAsync(moduleId);
            if (customer == null || module == null)
            {
                return null;
            }
            try
            {
                customer.RemoveProductModule(module, callerId);
            }
            catch
            {
                // item is already removed or did not exsit
            }
            await SaveEntitiesAsync();
            return await _customerContext.ProductModules.Include(m => m.ProductModuleGroup).FirstOrDefaultAsync(p => p.ProductModuleId == moduleId);
        }

        public async Task<ProductModuleGroup> GetProductModuleGroupAsync(Guid moduleGroupId)
        {
            return await _customerContext.ProductModuleGroups.FirstOrDefaultAsync(p => p.ProductModuleGroupId == moduleGroupId);
        }

        public async Task<ProductModuleGroup> AddProductModuleGroupAsync(Guid customerId, Guid moduleGroupId,Guid callerId)
        {
            var customer = await GetCustomerAsync(customerId);
            var moduleGroup = await GetProductModuleGroupAsync(moduleGroupId);
            if (customer == null || moduleGroup == null)
            {
                return null;
            }
            if (!customer.SelectedProductModuleGroups.Contains(moduleGroup))
            {
                customer.AddProductModuleGroup(moduleGroup,callerId);
            }
            await SaveEntitiesAsync();
            return moduleGroup;
        }

        public async Task<ProductModuleGroup> RemoveProductModuleGroupAsync(Guid customerId, Guid moduleGroupId,Guid callerId)
        {
            var customer = await GetCustomerAsync(customerId);
            var moduleGroup = await GetProductModuleGroupAsync(moduleGroupId);
            if (customer == null || moduleGroup == null)
            {
                return null;
            }
            try
            {
                customer.RemoveProductModuleGroup(moduleGroup,callerId);
            }
            catch
            {
                // item is already removed or did not exsit
            }
            await SaveEntitiesAsync();
            return moduleGroup;
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
                // item is already removed or did not exsit
            }
            await SaveEntitiesAsync();
            return department;
        }
    }
}
