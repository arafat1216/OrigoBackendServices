using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ISeedDatabaseService
    {
        public Task<string> CreateTestData();
    }
}
