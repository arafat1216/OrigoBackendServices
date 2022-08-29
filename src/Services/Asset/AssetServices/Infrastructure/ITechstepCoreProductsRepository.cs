using System.Collections.Generic;
using System.Threading.Tasks;
using AssetServices.Models;

namespace AssetServices.Infrastructure;

public interface ITechstepCoreProductsRepository
{
    Task<IList<TechstepProduct>> GetPartNumbersAsync(string searchText);
}