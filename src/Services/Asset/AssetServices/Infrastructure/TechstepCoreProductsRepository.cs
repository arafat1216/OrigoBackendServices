using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AssetServices.Models;

namespace AssetServices.Infrastructure;

class TechstepCoreProductsRepository : ITechstepCoreProductsRepository
{
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient HttpClient => _httpClientFactory.CreateClient("techstep-core-products");

    public TechstepCoreProductsRepository(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IList<TechstepProduct>> GetPartNumbersAsync(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            return new List<TechstepProduct>();
        }
        var techstepProducts = await HttpClient.GetFromJsonAsync<List<TechstepProduct>>($"?searchString={searchText}&categoryId=9371");
        if (techstepProducts == null)
        {
            return new List<TechstepProduct>();
        }

        return techstepProducts;
    }
}