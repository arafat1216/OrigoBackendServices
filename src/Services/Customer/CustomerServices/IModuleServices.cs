using CustomerServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IModuleServices
    {
        Task<IList<ProductModule>> GetModulesAsync();
    }
}
