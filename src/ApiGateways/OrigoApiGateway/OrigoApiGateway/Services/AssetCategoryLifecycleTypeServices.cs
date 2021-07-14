using OrigoApiGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class AssetCategoryLifecycleTypeServices : IAssetCategoryLifecycleTypesServices
    {
        public Task<OrigoAssetCategoryLifecycleType> AddAssetCategoryLifecycleTypeForCustomerAsync(Guid customerId, OrigoAssetCategoryLifecycleType newAssetCategoryLifecycleType)
        {
            throw new NotImplementedException();
        }

        public Task<IList<OrigoAssetCategoryLifecycleType>> GetAssetCategoryLifecycleTypesForCustomerAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
