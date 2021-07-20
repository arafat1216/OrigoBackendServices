using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IModuleServices
    {
        Task<IList<AssetCategoryType>> GetAllAssetCategoriesAsync(Guid? customerId);
        Task<IList<ProductModule>> GetModulesAsync();
    }
}
