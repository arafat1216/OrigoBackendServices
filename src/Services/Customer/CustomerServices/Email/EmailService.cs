using Common.Configuration;
using Common.Utilities;
using CustomerServices.Email.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Resources;
using System.Threading.Tasks;

namespace CustomerServices.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient => _httpClientFactory.CreateClient("emailservices");
        private readonly IFlatDictionaryProvider _flatDictionaryProvider;
        private readonly ResourceManager _resourceManager;

        public EmailService(
            IOptions<EmailConfiguration> emailConfiguration,
            IFlatDictionaryProvider flatDictionaryProvider,
            ResourceManager resourceManager, IHttpClientFactory httpClientFactory)
        {
            _emailConfiguration = emailConfiguration.Value;
            _httpClientFactory = httpClientFactory;
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
                throw new HttpRequestException($"Email failed for CustomerService, with Sub: {subject ?? _emailConfiguration.Subject} to Recipient: {string.Join(',', to)}");
            }
        }

        public async Task InvitationEmailToUserAsync(InvitationMail emailData, string languageCode)
        {
            var template = _resourceManager.GetString(InvitationMail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(emailData.Subject, template, emailData.Recipient , variables);
        }
    }
}
