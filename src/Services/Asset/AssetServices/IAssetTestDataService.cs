using System.Threading.Tasks;

namespace AssetServices
{
    public interface IAssetTestDataService
    {
        Task<string> CreateAssetTestData();
    }
}
