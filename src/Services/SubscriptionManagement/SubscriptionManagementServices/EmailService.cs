using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System;
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

        public async Task SendEmailAsync(string subject, object data)
        {
            if (string.IsNullOrEmpty(_emailConfiguration.BaseUrl)) return;
            
            try
            {
                var request = new Dictionary<string, object>
            {
                {"emailHeader", new Dictionary<string, object> {
                        {"partner", _emailConfiguration.Partner },
                        {"language", _emailConfiguration.Language},
                        {"subject", subject ?? _emailConfiguration.Subject },
                        {"sender", _emailConfiguration.Sender },
                        {"recipient", _emailConfiguration.Recipient }
                    }
                },
                { "content", new Dictionary<string, object> {
                        {"body", JsonSerializer.Serialize(data)}
                    }
                }
            };

                var response = await _httpClient.PostAsJsonAsync("/notification", new List<Dictionary<string, object>> { request });

                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {

            }
        }
    }
}
