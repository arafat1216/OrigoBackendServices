using AssetServices.Email.Model;
using Azure.Storage.Blobs;
using Common.Configuration;
using Common.Utilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Email
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

        private async Task SendAsync(string subject, string body, IList<string> to, Dictionary<string, string> variable)
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
                                            {"recipient", to }
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
                throw new HttpRequestException($"Email failed for AssetService, with Sub: {subject ?? _emailConfiguration.Subject} to Recipient: {string.Join(',',to)}");
            }
        }
        public async Task UnassignedFromUserEmailAsync(UnassignedFromUserNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(UnassignedFromUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(emailData.Subject, template, new List<string> { emailData.Recipient }, variables);
        }
        public async Task UnassignedFromManagerEmailAsync(UnassignedFromManagerNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(UnassignedFromUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(emailData.Subject, template, emailData.Recipient, variables);
        }
        public async Task ReAssignedToUserEmailAsync(ReAssignedToUserNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(emailData.Subject, template, new List<string> { emailData.Recipient }, variables);
        }
    }
}
