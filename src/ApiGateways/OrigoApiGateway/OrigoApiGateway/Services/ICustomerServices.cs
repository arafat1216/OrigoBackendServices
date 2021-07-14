using OrigoApiGateway.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrigoApiGateway.Controllers;

namespace OrigoApiGateway.Services
{
    public interface ICustomerServices
    {
        Task<IList<OrigoCustomer>> GetCustomersAsync();
        Task<OrigoCustomer> GetCustomerAsync(Guid customerId);
        Task<OrigoCustomer> CreateCustomerAsync(OrigoNewCustomer newCustomer);
        Task<OrigoAssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(NewAssetCategoryLifecycleType newAssetCategoryLifecycleType);
        Task<IList<OrigoAssetCategoryLifecycleType>> GetAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId);
        Task<OrigoProductModuleGroup> AddProductModulesAsync(Guid customerId, Guid moduleGroupId);
        Task<OrigoProductModuleGroup> RemoveProductModulesAsync(Guid customerId, Guid moduleGroupId);
    }
}