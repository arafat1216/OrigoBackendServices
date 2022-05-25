﻿using AutoMapper;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrderServices.Email.Models;
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
        private readonly HttpClient _httpClient;
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
            HardwareServiceOrderContext hardwareServiceOrderContext)
        {
            _emailConfiguration = emailConfiguration.Value;

            if (!string.IsNullOrEmpty(_emailConfiguration.BaseUrl))
                _httpClient = new HttpClient() { BaseAddress = new Uri(_emailConfiguration.BaseUrl) };

            _flatDictionaryProvider = flatDictionaryProvider;

            _resourceManager = resourceManager;

            _mapper = mapper;

            _origoConfiguration = origoConfiguration.Value;
            _hardwareServiceOrderContext = hardwareServiceOrderContext;
        }

        private async Task SendAsync(string subject, string body, string to, Dictionary<string, string> variable)
        {
            if (string.IsNullOrEmpty(_emailConfiguration.BaseUrl))
                return;

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

        public async Task SendOrderConfirmationEmailAsync(OrderConfirmationEmail data, string languageCode)
        {
            var template = _resourceManager.GetString(OrderConfirmationEmail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
            var variables = _flatDictionaryProvider.Execute(data);
            await SendAsync(data.Subject, template, data.Recipient, variables);
        }

        public async Task<List<AssetRepairEmail>> SendAssetRepairEmailAsync(DateTime? olderThan, int? statusId, string languageCode = "en")
        {

            var orders = _hardwareServiceOrderContext.HardwareServiceOrders.Include(m => m.Status).AsQueryable();

            if (olderThan != null)
                orders = orders.Where(m => m.CreatedDate <= olderThan);

            if (statusId != null)
                orders = orders.Where(m => m.Status.Id == statusId);

            var emails = _mapper.Map<List<AssetRepairEmail>>(orders);

            emails.ForEach(async email =>
            {
                email.OrderLink = string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", email.CustomerId, email.OrderId);
                var template = _resourceManager.GetString(AssetRepairEmail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(email.Subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
            });

            return emails;
        }

        public async Task<List<LoanDeviceEmail>> SendLoanDeviceEmailAsync(List<int> statusIds, string languageCode = "en")
        {
            var orders = await _hardwareServiceOrderContext.HardwareServiceOrders
                .Include(m => m.Status)
                .Where(m => statusIds.Contains(m.Status.Id) && !m.IsReturnLoanDeviceEmailSent)
                .ToListAsync();

            var emails = _mapper.Map<List<LoanDeviceEmail>>(orders);

            emails.ForEach(async email =>
            {
                var template = _resourceManager.GetString(LoanDeviceEmail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(email.Subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
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

        public async Task<List<AssetDiscardedEmail>> SendOrderDiscardedEmailAsync(int statusId, string languageCode = "en")
        {
            var orders = await _hardwareServiceOrderContext.HardwareServiceOrders
                .Include(m => m.Status)
                .Where(m => m.Status.Id == statusId && !m.IsOrderDiscardedEmailSent)
                .ToListAsync();

            var emails = _mapper.Map<List<AssetDiscardedEmail>>(orders);

            emails.ForEach(async email =>
            {
                var template = _resourceManager.GetString(AssetDiscardedEmail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(email.Subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
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
                var template = _resourceManager.GetString(OrderCancellationEmail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(email.Subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
            });

            return emails;
        }
    }
}