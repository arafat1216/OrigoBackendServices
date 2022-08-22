using System.Collections.Generic;
using System.Threading.Tasks;
using AssetServices.Models;
using Microsoft.Extensions.Options;

namespace AssetServices.Infrastructure;

public interface ITechstepCoreProductsRepository
{
    Task<IList<TechstepProduct>> GetPartNumbersAsync(string searchText);
}

class TechstepCoreProductsRepository : ITechstepCoreProductsRepository
{
    private readonly IOptions<TechstepCoreProductsRepositoryOptions> _options;

    public TechstepCoreProductsRepository(IOptions<TechstepCoreProductsRepositoryOptions> options)
    {
        _options = options;
    }

    public Task<IList<TechstepProduct>> GetPartNumbersAsync(string searchText)
    {
        throw new System.NotImplementedException();
    }
}

public class TechstepCoreProductsRepositoryOptions
{
    public string SecurityKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
}