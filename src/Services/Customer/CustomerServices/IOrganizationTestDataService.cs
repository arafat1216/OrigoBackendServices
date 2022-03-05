using System.Threading.Tasks;

namespace CustomerServices
{
    public interface IOrganizationTestDataService
    {
        public Task<string> CreateOrganizationTestData();
    }
}
