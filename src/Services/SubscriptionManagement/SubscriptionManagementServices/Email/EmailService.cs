using Azure.Storage.Blobs;
using Common.Utilities;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Email.Configuration;
using SubscriptionManagementServices.Email.Models;
using SubscriptionManagementServices.Exceptions;
using System.Globalization;
using System.Net.Http.Json;
using System.Resources;

namespace SubscriptionManagementServices.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfigurationSubscriptionManagement _emailConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient => _httpClientFactory.CreateClient("emailservices");
        private readonly IFlatDictionaryProvider _flatDictionaryProvider;
        private readonly ResourceManager _resourceManager;

        public EmailService(IOptions<EmailConfigurationSubscriptionManagement> emailConfiguration, 
            IFlatDictionaryProvider flatDictionaryProvider, 
            IHttpClientFactory httpClientFactory, 
            ResourceManager resourceManager)
        {
            _emailConfiguration = emailConfiguration.Value;

            _httpClientFactory = httpClientFactory;

            _flatDictionaryProvider = flatDictionaryProvider;
            _resourceManager = resourceManager;
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

        public async Task ActivateSimMailSendAsync(ActivateSimMail emailData, string languageCode)
        {
            var template = _resourceManager.GetString(ActivateSimMail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync($"[{emailData.OrderType}]-[{emailData.SubscriptionOrderId}]", template, variables);
        }

        public async Task CancelSubscriptionMailSendAsync(CancelSubscriptionMail emailData, string languageCode)
        {
            var template = _resourceManager.GetString(CancelSubscriptionMail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync($"[{emailData.OrderType}]-[{emailData.SubscriptionOrderId}]", template, variables);
        }

        public async Task ChangeSubscriptionMailSendAsync(ChangeSubscriptionMail emailData, string languageCode)
        {
            var template = _resourceManager.GetString(ChangeSubscriptionMail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync($"[{emailData.OrderType}]-[{emailData.SubscriptionOrderId}]", template, variables);
        }

        public async Task NewSubscriptionMailSendAsync(NewSubscriptionMail emailData, string languageCode)
        {
            var template = _resourceManager.GetString(NewSubscriptionMail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync($"[{emailData.OrderType}]-[{emailData.SubscriptionOrderId}]", template, variables);
        }

        public async Task OrderSimMailSendAsync(OrderSimMail emailData, string languageCode)
        {
            var template = _resourceManager.GetString(OrderSimMail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync($"[{emailData.OrderType}]-[{emailData.SubscriptionOrderId}]", template, variables);
        }

        public async Task TransferToBusinessMailSendAsync(TransferToBusinessMail emailData, string languageCode)
        {
            var template = _resourceManager.GetString(TransferToBusinessMail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync($"[{emailData.OrderType}]-[{emailData.SubscriptionOrderId}]", template, variables);
        }

        public async Task TransferToPrivateMailSendAsync(TransferToPrivateMail emailData, string languageCode)
        {
            var template = _resourceManager.GetString(TransferToPrivateMail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync($"[{emailData.OrderType}]-[{emailData.SubscriptionOrderId}]", template, variables);
        }
    }
}
