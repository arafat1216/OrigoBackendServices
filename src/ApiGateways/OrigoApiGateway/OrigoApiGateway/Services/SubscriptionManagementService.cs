using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {
      
        private readonly ILogger<SubscriptionManagementService> _logger;
        private readonly SubscriptionManagementConfiguration _options;
        private HttpClient HttpClient { get; }
        public SubscriptionManagementService(ILogger<SubscriptionManagementService> logger, IOptions<SubscriptionManagementConfiguration> options, HttpClient httpClient)
        {
            _logger = logger;
            _options = options.Value;
            HttpClient = httpClient;
        }

        public Task<IList<string>> GetAllOperatorList()
        {
            //var operators = await HttpClient.GetFromJsonAsync<IList<OperatorDTO>>($"{_options.ApiPath}");
            IList<string> list = new string[] { "Telenor - NO", "Telia - NO", "Telenor - SE", "Telia - SE" };
            return Task.FromResult(list);
        }
    }
}
