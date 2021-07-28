using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Extensions;
using Common.Logging;
using Common.Utilities;
using MediatR;

namespace CustomerServices.Infrastructure
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _customerContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;

        public CustomerRepository(CustomerContext customerContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _customerContext = customerContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        public async Task SaveChanges()
        {
            await _customerContext.SaveChangesAsync();
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            _customerContext.Customers.Add(customer);
            await SaveEntitiesAsync();
            return customer;
        }

        public async Task<IList<Customer>> GetCustomersAsync()
        {
            return await _customerContext.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _context.Customers
                .Include(p => p.SelectedProductModules)
                .ThenInclude(p => p.ProductModuleGroup)
                .Include(p => p.SelectedProductModuleGroups)
                .Include(p => p.SelectedAssetCategories)
                .ThenInclude(p => p.LifecycleTypes)
                .Include(p => p.SelectedAssetCategoryLifecycles)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<IList<User>> GetAllUsersAsync(Guid customerId)
        {
            return await _customerContext.Users.Include(u => u.Customer).Where(u => u.Customer.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<User> GetUserAsync(Guid customerId, Guid userId)
        {
            return await _customerContext.Users.Include(u => u.Customer).Where(u => u.Customer.CustomerId == customerId && u.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<User> AddUserAsync(User newUser)
        {
            _customerContext.Users.Add(newUser);
            await SaveEntitiesAsync();
            return newUser;
        }


        public async Task<IList<AssetCategoryLifecycleType>> GetAssetCategoryLifecycleType(Guid customerId, Guid assetCategoryId)
        {
            return await _customerContext.AssetCategoryLifecycleTypes.Where(p => p.AssetCategoryId == assetCategoryId && p.CustomerId == customerId).ToListAsync();
        }

        public async Task<IList<AssetCategoryLifecycleType>> GetAllAssetCategoryLifecycleTypesAsync(Guid customerId)
        {
            var customer = await GetCustomerAsync(customerId);
            return customer.SelectedAssetCategoryLifecycles.ToList();
        }

        public async Task<AssetCategoryLifecycleType> DeleteAssetCategoryLifecycleTypeAsync(AssetCategoryLifecycleType assetCategoryLifecycleType)
        {
            _customerContext.AssetCategoryLifecycleTypes.Remove(assetCategoryLifecycleType);
            await _customerContext.SaveChangesAsync();
            return assetCategoryLifecycleType;
        }

        public async Task<AssetCategoryType> GetAssetCategoryTypeAsync(Guid customerId, Guid assetCategoryId)
        {
            return await _customerContext.AssetCategoryTypes.Include(p => p.LifecycleTypes).FirstOrDefaultAsync(p => p.AssetCategoryId == assetCategoryId && p.ExternalCustomerId == customerId);
        }

        public async Task<IList<AssetCategoryType>> GetAssetCategoryTypesAsync(Guid customerId)
        {
            var customer = await GetCustomerAsync(customerId);
            return customer.SelectedAssetCategories.ToList();
        }

        public async Task<AssetCategoryType> DeleteAssetCategoryTypeAsync(AssetCategoryType assetCategoryType)
        {
            _customerContext.AssetCategoryTypes.Remove(assetCategoryType);
            await _customerContext.SaveChangesAsync();
            return assetCategoryType;
        }

        public async Task<IList<ProductModuleGroup>> GetProductModuleGroupsAsync()
        {
            return await _context.ProductModuleGroups.ToListAsync();
        }

        public async Task<IList<ProductModuleGroup>> GetCustomerProductModuleGroupsAsync(Guid customerId)
        {
            var customer = await GetCustomerAsync(customerId);
            return customer.SelectedProductModuleGroups.ToList();
        }

        public async Task<ProductModuleGroup> GetProductModuleGroupAsync(Guid moduleGroupId)
        {
            return await _customerContext.ProductModuleGroups.FirstOrDefaultAsync(p => p.ProductModuleGroupId == moduleGroupId);
        }

        private async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            int numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_customerContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _customerContext.SaveChangesAsync(cancellationToken);
                foreach (var @event in _customerContext.GetDomainEventsAsync())
                {
                    await _functionalEventLogService.SaveEventAsync(@event, _customerContext.Database.CurrentTransaction);
                }
                numberOfRecordsSaved = await _customerContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_customerContext);
            });
            return numberOfRecordsSaved;
        }

        public async Task<IList<ProductModule>> GetProductModulesAsync()
        {
            return await _context.ProductModules.Include(p => p.ProductModuleGroup).ToListAsync();
        }

        public async Task<IList<ProductModule>> GetCustomerProductModulesAsync(Guid customerId)
        {
            var customer = await GetCustomerAsync(customerId);
            return customer.SelectedProductModules.ToList();
        }

        public async Task<ProductModule> GetProductModuleAsync(Guid moduleId)
        {
            return await _context.ProductModules.Include(m => m.ProductModuleGroup).FirstOrDefaultAsync(p => p.ProductModuleId == moduleId);
        }
    }
}
