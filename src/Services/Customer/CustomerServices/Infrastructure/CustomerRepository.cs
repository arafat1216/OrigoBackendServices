using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerServices.Infrastructure
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;
        public CustomerRepository(CustomerContext context)
        {
            _context = context;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Customer> AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<IList<Customer>> GetCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _context.Customers
                .Include(p => p.SelectedProductModuleGroups)
                .Include(p => p.SelectedAssetCategories)
                .ThenInclude(p => p.LifecycleTypes)
                .Include(p => p.SelectedAssetCategoryLifecycles)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        public async Task<IList<User>> GetAllUsersAsync(Guid customerId)
        {
            return await _context.Users.Include(u => u.Customer).Where(u => u.Customer.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<User> GetUserAsync(Guid customerId, Guid userId)
        {
            return await _context.Users.Include(u => u.Customer).Where(u => u.Customer.CustomerId == customerId && u.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<User> AddUserAsync(User newUser)
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }


        public async Task<IList<AssetCategoryLifecycleType>> GetAssetCategoryLifecycleType(Guid customerId, Guid assetCategoryId)
        {
            return await _context.AssetCategoryLifecycleTypes.Where(p => p.AssetCategoryId == assetCategoryId && p.CustomerId == customerId).ToListAsync();
        }

        public async Task<IList<AssetCategoryLifecycleType>> GetAllAssetCategoryLifecycleTypesAsync(Guid customerId)
        {
            var customer = await GetCustomerAsync(customerId);
            return customer.SelectedAssetCategoryLifecycles.ToList();
        }

        public async Task<AssetCategoryLifecycleType> DeleteAssetCategoryLifecycleTypeAsync(AssetCategoryLifecycleType assetCategoryLifecycleType)
        {
            _context.AssetCategoryLifecycleTypes.Remove(assetCategoryLifecycleType);
            await _context.SaveChangesAsync();
            return assetCategoryLifecycleType;
        }

        public async Task<AssetCategoryType> GetAssetCategoryTypeAsync(Guid customerId, Guid assetCategoryId)
        {
            return await _context.AssetCategoryTypes.Include(p => p.LifecycleTypes).FirstOrDefaultAsync(p => p.AssetCategoryId == assetCategoryId && p.ExternalCustomerId == customerId);
        }

        public async Task<IList<AssetCategoryType>> GetAssetCategoryTypesAsync(Guid customerId)
        {
            var customer = await GetCustomerAsync(customerId);
            return customer.SelectedAssetCategories.ToList();
        }

        public async Task<AssetCategoryType> DeleteAssetCategoryTypeAsync(AssetCategoryType assetCategoryType)
        {
            _context.AssetCategoryTypes.Remove(assetCategoryType);
            await _context.SaveChangesAsync();
            return assetCategoryType;
        }

        public async Task<IList<ProductModule>> GetModulesAsync()
        {
            return await _context.ProductModules.Include(p => p.ProductModuleGroup).ToListAsync();
        }

        public async Task<IList<ProductModuleGroup>> GetCustomerProductModulesAsync(Guid customerId)
        {
            var customer = await GetCustomerAsync(customerId);
            return customer.SelectedProductModuleGroups.ToList();
        }

        public async Task<ProductModuleGroup> GetProductModuleGroupAsync(Guid moduleGroupId)
        {
            return await _context.ProductModuleGroups.FirstOrDefaultAsync(p => p.ProductModuleGroupId == moduleGroupId);
        }
    }
}
