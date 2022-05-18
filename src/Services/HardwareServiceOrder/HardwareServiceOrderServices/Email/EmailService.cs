using AutoMapper;
using Common.Configuration;
using Common.Utilities;
using HardwareServiceOrderServices.Email.Models;
using HardwareServiceOrderServices.Models;
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
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        private readonly IMapper _mapper;
        private readonly OrigoConfiguration _origoConfiguration;

        public EmailService(
            IOptions<EmailConfiguration> emailConfiguration,
            IFlatDictionaryProvider flatDictionaryProvider,
            ResourceManager resourceManager,
            IMapper mapper,
            IOptions<OrigoConfiguration> origoConfiguration,
            IHardwareServiceOrderRepository hardwareServiceOrderRepository)
        {
            _emailConfiguration = emailConfiguration.Value;

            if (!string.IsNullOrEmpty(_emailConfiguration.BaseUrl))
                _httpClient = new HttpClient() { BaseAddress = new Uri(_emailConfiguration.BaseUrl) };

            _flatDictionaryProvider = flatDictionaryProvider;

            _resourceManager = resourceManager;

            _mapper = mapper;

            _origoConfiguration = origoConfiguration.Value;

            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
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

        public async Task<List<AssetRepairEmail>> SendAssetRepairEmailAsync(DateTime olderThan, List<int> statusIds, string languageCode = "EN")
        {
            var orders = await _hardwareServiceOrderRepository.GetAllOrdersAsync(olderThan, statusIds);
            var emails = _mapper.Map<List<AssetRepairEmail>>(orders);

            emails.ForEach(async email =>
            {
                email.OrderLink = string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", email.CustomerId, email.OrderId);
                var template = _resourceManager.GetString(AssetRepairEmail.TemplateName, CultureInfo.CreateSpecificCulture(languageCode));
                await SendAsync(email.Subject, template, email.Recipient, _flatDictionaryProvider.Execute(email));
            });

            return emails;
        }
    }
}
