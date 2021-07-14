using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface IAssetCategoryLifecycleTypesServices
    {
        Task<IList<OrigoAssetCategoryLifecycleType>> GetAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId);
        Task<OrigoAssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, OrigoAssetCategoryLifecycleType newAssetCategoryLifecycleType);
    }
}
