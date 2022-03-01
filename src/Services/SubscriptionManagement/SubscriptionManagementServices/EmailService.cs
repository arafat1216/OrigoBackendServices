using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace SubscriptionManagementServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly HttpClient _httpClient;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration)
        {
            _emailConfiguration = emailConfiguration.Value;
            _httpClient = new HttpClient() { BaseAddress = new Uri(_emailConfiguration.BaseUrl) };
        }

        public async Task SendEmailAsync(object data)
        {
            var request = new Dictionary<string, object>
            {
                {"emailHeader", new Dictionary<string, object> {
                        {"partner", _emailConfiguration.Partner },
                        {"language", _emailConfiguration.Language},
                        {"subject", _emailConfiguration.Subject },
                        {"sender", _emailConfiguration.Sender },
                        {"recipient", _emailConfiguration.Recipient }
                    } 
                },
                { "content", new Dictionary<string, object> {
                        {"body", data}
                    }
                }
            };

            await _httpClient.PostAsJsonAsync("/notification", new List<Dictionary<string, object>> { request });
        }
    }
}
