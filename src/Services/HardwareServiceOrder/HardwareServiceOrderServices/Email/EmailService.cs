using AutoMapper;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Exceptions;
using HardwareServiceOrderServices.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Json;
using System.Resources;
namespace HardwareServiceOrderServices.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient => _httpClientFactory.CreateClient("emailservices");
        private readonly IFlatDictionaryProvider _flatDictionaryProvider;
        private readonly ResourceManager _resourceManager;
        private readonly IMapper _mapper;
        private readonly OrigoConfiguration _origoConfiguration;
        private readonly HardwareServiceOrderContext _hardwareServiceOrderContext;

        public EmailService(
            IOptions<EmailConfiguration> emailConfiguration,
            IFlatDictionaryProvider flatDictionaryProvider,
            ResourceManager resourceManager,
            IMapper mapper,
            IOptions<OrigoConfiguration> origoConfiguration,
            HardwareServiceOrderContext hardwareServiceOrderContext,
            IHttpClientFactory httpClientFactory)
        {
            _emailConfiguration = emailConfiguration.Value;

            _httpClientFactory = httpClientFactory;

            _flatDictionaryProvider = flatDictionaryProvider;

            _resourceManager = resourceManager;

            _mapper = mapper;

            _origoConfiguration = origoConfiguration.Value;
            _hardwareServiceOrderContext = hardwareServiceOrderContext;
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
            catch (Exception ex)
            {
                throw new EmailException($"Email failed for HardwareServiceOrderServices, with Sub: {subject ?? _emailConfiguration.Subject} to Recipient: {string.Join(',', to)}", Guid.Parse("5441179c-97f0-4e84-8b9c-b5bec10a7448"), ex);
            }
        }

        /// <inheritdoc/>
        public async Task SendOrderConfirmationEmailAsync(OrderConfirmationEmail order, string languageCode)
        {
            var template = _resourceManager.GetString(OrderConfirmationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(languageCode));
            order.Subject = _resourceManager.GetString(OrderConfirmationEmail.SubjectKeyName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(order);
            await SendAsync(order.Subject, template, order.Recipient, variables);
        }

        /// <inheritdoc />
        public async Task SendOrderConfirmationEmailAsync(RemarketingPackaging order, string languageCode)
        {
            var template = _resourceManager.GetString(RemarketingPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(languageCode));
            order.Subject = _resourceManager.GetString(RemarketingPackaging.SubjectKeyName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(order);
            await SendAsync(order.Subject, template, order.Recipient, variables);
        }

        /// <inheritdoc />
        public async Task SendOrderConfirmationEmailAsync(RemarketingNoPackaging order, string languageCode)
        {
            var template = _resourceManager.GetString(RemarketingNoPackaging.TemplateKeyName, CultureInfo.CreateSpecificCulture(languageCode));
            order.Subject = _resourceManager.GetString(RemarketingNoPackaging.SubjectKeyName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(order);
            await SendAsync(order.Subject, template, order.Recipient, variables);
        }

        /// <inheritdoc/>
        public async Task<List<AssetRepairEmail>> SendAssetRepairEmailAsync(DateTime? olderThan, int? statusId, string languageCode = "en")
        {
            var orders = _hardwareServiceOrderContext.HardwareServiceOrders.Include(m => m.Status).AsQueryable();

            if (olderThan != null)
                orders = orders.Where(m => m.DateCreated <= olderThan);

            if (statusId != null)
                orders = orders.Where(m => m.Status.Id == statusId);

            var emails = _mapper.Map<List<AssetRepairEmail>>(orders);

            emails.ForEach(async email =>
            {
                email.OrderLink = string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", email.CustomerId, email.OrderId);
                var template = _resourceManager.GetString(AssetRepairEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(languageCode));
                var subject = _resourceManager.GetString(AssetRepairEmail.SubjectKeyName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
            });

            return emails;
        }

        /// <inheritdoc/>
        public async Task<List<LoanDeviceEmail>> SendLoanDeviceEmailAsync(List<int> statusIds, string languageCode = "en")
        {
            var orders = await _hardwareServiceOrderContext.HardwareServiceOrders
                .Include(m => m.Status)
                .Where(m => statusIds.Contains(m.Status.Id) && !m.IsReturnLoanDeviceEmailSent)
                .ToListAsync();

            var emails = _mapper.Map<List<LoanDeviceEmail>>(orders);

            emails.ForEach(async email =>
            {
                var template = _resourceManager.GetString(LoanDeviceEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(languageCode));
                var subject = _resourceManager.GetString(LoanDeviceEmail.SubjectKeyName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
            });

            orders.ForEach(order =>
            {
                order.IsReturnLoanDeviceEmailSent = true;
                _hardwareServiceOrderContext.Entry(order).State = EntityState.Modified;
            });

            if (orders.Count > 0)
                _hardwareServiceOrderContext.SaveChanges();

            return emails;
        }

        /// <inheritdoc/>
        public async Task<List<AssetDiscardedEmail>> SendOrderDiscardedEmailAsync(int statusId, string languageCode = "en")
        {
            var orders = await _hardwareServiceOrderContext.HardwareServiceOrders
                .Include(m => m.Status)
                .Where(m => m.Status.Id == statusId && !m.IsOrderDiscardedEmailSent)
                .ToListAsync();

            var emails = _mapper.Map<List<AssetDiscardedEmail>>(orders);

            emails.ForEach(async email =>
            {
                var template = _resourceManager.GetString(AssetDiscardedEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(languageCode));
                var subject = _resourceManager.GetString(AssetDiscardedEmail.SubjectKeyName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
            });

            orders.ForEach(order =>
            {
                order.IsOrderDiscardedEmailSent = true;
                _hardwareServiceOrderContext.Entry(order).State = EntityState.Modified;
            });

            if (orders.Count > 0)
                _hardwareServiceOrderContext.SaveChanges();

            return emails;
        }

        /// <inheritdoc/>
        public async Task<List<OrderCancellationEmail>> SendOrderCancellationEmailAsync(int statusId, string languageCode = "en")
        {
            var orders = _hardwareServiceOrderContext.HardwareServiceOrders
                .Include(m => m.Status)
                .Include(m => m.ServiceType)
                .Where(m => m.Status.Id == statusId)
                .AsQueryable();

            var emails = _mapper.Map<List<OrderCancellationEmail>>(orders);

            emails.ForEach(async email =>
            {
                email.OrderLink = string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", email.CustomerId, email.OrderId);
                var template = _resourceManager.GetString(OrderCancellationEmail.TemplateKeyName, CultureInfo.CreateSpecificCulture(languageCode));
                var subject = _resourceManager.GetString(OrderCancellationEmail.SubjectKeyName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
            });

            return emails;
        }

        /// <inheritdoc/>
        public async Task SendEmailAsync(string to, string subjectKey, string bodyTemplateKey, Dictionary<string, string> parameters, string languageCode = "en")
        {
            var body = _resourceManager.GetString(bodyTemplateKey, CultureInfo.CreateSpecificCulture(languageCode));
            var subject = _resourceManager.GetString(subjectKey, CultureInfo.CreateSpecificCulture(languageCode));

            await SendAsync(subject, body, to, parameters);
        }
    }
}
