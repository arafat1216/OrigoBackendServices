using Azure.Storage.Blobs;
using Common.Configuration;
using Common.Utilities;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace HardwareServiceOrderServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly HttpClient _httpClient;
        private readonly IFlatDictionaryProvider _flatDictionaryProvider;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration, IFlatDictionaryProvider flatDictionaryProvider)
        {
            _emailConfiguration = emailConfiguration.Value;

            if (!string.IsNullOrEmpty(_emailConfiguration.BaseUrl))
                _httpClient = new HttpClient() { BaseAddress = new Uri(_emailConfiguration.BaseUrl) };

            _flatDictionaryProvider = flatDictionaryProvider;
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

        public async Task SendAsync(string subject, string to, string type, object data)
        {
            if (string.IsNullOrEmpty(_emailConfiguration.BaseUrl)) return;

            var variables = _flatDictionaryProvider.Execute(data);
            var templateName = _emailConfiguration.Templates[type];
            if (string.IsNullOrEmpty(templateName)) return;

            try
            {
                var blobContainerClient = new BlobContainerClient(_emailConfiguration.AzureStorageConnectionString, _emailConfiguration.TemplateContainer);
                var blobClient = blobContainerClient.GetBlobClient(templateName);
                var templateContent = (await blobClient.DownloadAsync()).Value.Content;

                using var reader = new StreamReader(templateContent);
                var body = await reader.ReadToEndAsync();

                await SendAsync(subject, body, to, variables);
            }
            catch (Exception)
            {

            }
        }
    }
}
