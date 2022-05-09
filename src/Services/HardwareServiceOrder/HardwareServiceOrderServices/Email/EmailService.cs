using Azure.Storage.Blobs;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrderServices.Email.Models;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Json;
using System.Resources;
namespace HardwareServiceOrderServices.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly HttpClient _httpClient;
        private readonly IFlatDictionaryProvider _flatDictionaryProvider;
        private readonly ResourceManager _resourceManager;

        public EmailService(
            IOptions<EmailConfiguration> emailConfiguration, 
            IFlatDictionaryProvider flatDictionaryProvider, 
            ResourceManager resourceManager)
        {
            _emailConfiguration = emailConfiguration.Value;

            if (!string.IsNullOrEmpty(_emailConfiguration.BaseUrl))
                _httpClient = new HttpClient() { BaseAddress = new Uri(_emailConfiguration.BaseUrl) };

            _flatDictionaryProvider = flatDictionaryProvider;

            _resourceManager = resourceManager;
        }

        private async Task SendAsync(string subject, string body, string to, Dictionary<string, string> variable)
        {
            try
            {
                var request = new Dictionary<string, object>
                                {
                                    {"emailHeader", new Dictionary<string, object> {
                                            {"partner", _emailConfiguration.Partner },
                                            {"language", _emailConfiguration.Language},
                                            {"subject", subject ?? _emailConfiguration.Subject },
                                            {"sender", _emailConfiguration.Sender },
                                            {"recipient", new List<string>{to } }
                                        }
                                    },
                                    { "content", new Dictionary<string, object> {
                                            {"body", body},
                                            {"variables", variable}
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

        public async Task SendOrderConfirmationEmailAsync(OrderConfirmation data, string languageCode)
        {
            var template = _resourceManager.GetString(OrderConfirmation.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(data);
            await SendAsync(data.Subject, template, data.Recipient, variables);
        }
    }
}
