using AssetServices.Email.Configuration;
using AssetServices.Email.Model;
using AssetServices.Exceptions;
using Common.Configuration;
using Common.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Resources;
using System.Threading.Tasks;


namespace AssetServices.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfigurationAsset _emailConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient => _httpClientFactory.CreateClient("emailservices");
        private readonly IFlatDictionaryProvider _flatDictionaryProvider;
        private readonly ResourceManager _resourceManager;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailConfigurationAsset> emailConfiguration,
            IFlatDictionaryProvider flatDictionaryProvider,
            ResourceManager resourceManager, IHttpClientFactory httpClientFactory,
            ILogger<EmailService> logger)
        {
            _emailConfiguration = emailConfiguration.Value;
            _httpClientFactory = httpClientFactory;
            _flatDictionaryProvider = flatDictionaryProvider;
            _resourceManager = resourceManager;
            _logger = logger;
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
            catch (Exception ex)
            {
                _logger?.LogError("{0}", $"Email failed for AssetService. Message: {ex.Message}");
                throw new EmailException($"Email failed for AssetService, with Sub: {subject ?? _emailConfiguration.Subject} to Recipient: {string.Join(',', to)}", Guid.Parse("5441179c-97f0-4e84-8b9c-b5bec10a7448"), ex);
            }
        }
        public async Task UnassignedFromUserEmailAsync(UnassignedFromUserNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(UnassignedFromUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var subject = _resourceManager.GetString(UnassignedFromUserNotification.Subject, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(subject, template, new List<string> { emailData.Recipient }, variables);
        }
        public async Task UnassignedFromManagerEmailAsync(UnassignedFromManagerNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(UnassignedFromManagerNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var subject = _resourceManager.GetString(UnassignedFromManagerNotification.Subject, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(subject, template, emailData.Recipient, variables);
        }
        public async Task ReAssignedToUserEmailAsync(ReAssignedToUserNotification emailData, string languageCode)
        {
            emailData.AssetLink = _emailConfiguration.OrigoBaseUrl + _emailConfiguration.AssetPath;

            var template = _resourceManager.GetString(ReAssignedToUserNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var subject = _resourceManager.GetString(ReAssignedToUserNotification.Subject, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(subject, template, new List<string> { emailData.Recipient }, variables);
        }
        public async Task ReportAssetEmailAsync(ReportAssetNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(ReportAssetNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var subject = _resourceManager.GetString(ReportAssetNotification.Subject, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(subject, template, emailData.Recipients, variables);
        }
        public async Task PendingReturnEmailAsync(PendingReturnNotification emailData, string languageCode)
        {
            emailData.AssetLink = _emailConfiguration.OrigoBaseUrl + _emailConfiguration.AssetPath;

            var template = _resourceManager.GetString(PendingReturnNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var subject = _resourceManager.GetString(PendingReturnNotification.Subject, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(subject, template, emailData.Recipients, variables);
        }
        public async Task AssetBuyoutEmailAsync(AssetBuyoutNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(AssetBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var subject = _resourceManager.GetString(AssetBuyoutNotification.Subject, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            await SendAsync(subject, template, new List<string>() { emailData.Recipient }, variables);
        }
        /// <inheritdoc/>
        public async Task ManagerBuyoutEmailAsync(ManagerOnBehalfBuyoutNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(ManagerOnBehalfBuyoutNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            var subject = _resourceManager.GetString(ManagerOnBehalfBuyoutNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(languageCode));
            await SendAsync(subject, template, new List<string>() { emailData.Recipient }, variables);
        }
        /// <inheritdoc/>
        public async Task ManagerReturnEmailAsync(ManagerOnBehalfReturnNotification emailData, string languageCode)
        {
            var template = _resourceManager.GetString(ManagerOnBehalfReturnNotification.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(emailData);
            var subject = _resourceManager.GetString(ManagerOnBehalfReturnNotification.SubjectTemplatename, CultureInfo.CreateSpecificCulture(languageCode));
            await SendAsync(subject, template, new List<string>() { emailData.Recipient }, variables);
        }

    }
}
