using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ICustomerServices
    {
        Task<IList<Organization>> GetCustomersAsync();
        Task<Organization> GetCustomerAsync(Guid customerId);
        Task<Organization> CreateCustomerAsync(NewOrganization newCustomer, Guid callerId);
        Task<Organization> UpdateOrganizationAsync(UpdateOrganizationDTO organizationToChange);
        Task<Organization> PatchOrganizationAsync(UpdateOrganizationDTO organizationToChange);
        Task<Organization> DeleteOrganizationAsync(Guid organizationId, Guid callerId);

        Task<IList<OrigoCustomerAssetCategoryType>> GetAssetCategoryForCustomerAsync(Guid customerId);
        Task<OrigoCustomerAssetCategoryType> AddAssetCategoryForCustomerAsync(Guid customerId, NewCustomerAssetCategoryType customerAssetCategoryType, Guid callerId);
        Task<OrigoCustomerAssetCategoryType> RemoveAssetCategoryForCustomerAsync(Guid customerId, NewCustomerAssetCategoryType customerAssetCategoryType, Guid callerId);

        Task<IList<OrigoProductModule>> GetCustomerProductModulesAsync(Guid customerId);
        Task<OrigoProductModule> AddProductModulesAsync(Guid customerId, NewCustomerProductModule productModule, Guid callerId);
        Task<OrigoProductModule> RemoveProductModulesAsync(Guid customerId, NewCustomerProductModule productModule, Guid callerId);
        Task<string> CreateOrganizationSeedData();
    }
}