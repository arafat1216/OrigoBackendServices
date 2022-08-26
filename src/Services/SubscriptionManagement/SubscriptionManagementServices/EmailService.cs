using Azure.Storage.Blobs;
using Common.Configuration;
using Common.Utilities;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Exceptions;
using System.Net.Http.Json;

namespace SubscriptionManagementServices
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient => _httpClientFactory.CreateClient("emailservices");
        private readonly IFlatDictionaryProvider _flatDictionaryProvider;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration, IFlatDictionaryProvider flatDictionaryProvider, IHttpClientFactory httpClientFactory)
        {
            _emailConfiguration = emailConfiguration.Value;

            _httpClientFactory = httpClientFactory;

            _flatDictionaryProvider = flatDictionaryProvider;
        }

        private async Task SendAsync(string subject, string body, Dictionary<string, string> variable)
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
                        {"recipient", _emailConfiguration.Recipient }
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
            catch (Exception ex)
            {
                throw new EmailException($"Email failed for SubscriptionManagementSerives, with Sub: {subject ?? _emailConfiguration.Subject} to Recipient: {string.Join(',', _emailConfiguration.Recipient)}", Guid.Parse("5441179c-97f0-4e84-8b9c-b5bec10a7448"), ex);
            }
        }

        public async Task SendAsync(string orderType, Guid subscriptionOrderId, object data, Dictionary<string, string> others = null)
        {
            var variables = _flatDictionaryProvider.Execute(data);

            variables["OrderType"] = orderType;

            if (others != null)
            {
                foreach (var item in others)
                    variables[item.Key] = item.Value;
            }

            var templateName = string.Empty;

            foreach (var item in _emailConfiguration.Templates)
            {
                if (orderType.Contains(item.Key))
                    templateName = item.Value;
            }

            if (string.IsNullOrEmpty(templateName)) return;

            try
            {
                var blobContainerClient = new BlobContainerClient(_emailConfiguration.AzureStorageConnectionString, _emailConfiguration.TemplateContainer);
                var blobClient = blobContainerClient.GetBlobClient(templateName);
                var templateContent = (await blobClient.DownloadAsync()).Value.Content;

                using var reader = new StreamReader(templateContent);
                var body = await reader.ReadToEndAsync();

                await SendAsync($"[{orderType}]-[{subscriptionOrderId}]", body, variables);

            }
            catch (Exception)
            {
            }
        }
    }
}
