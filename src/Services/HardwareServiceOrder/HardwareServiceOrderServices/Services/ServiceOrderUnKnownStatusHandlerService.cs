using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace HardwareServiceOrderServices.Services
{
    /// <summary>
    /// Handles Unknown status
    /// </summary>
    public class ServiceOrderUnKnownStatusHandlerService : ServiceOrderStatusHandlerService
    {
        private readonly OrigoConfiguration _origoConfiguration;
        private readonly IEmailService _emailService;
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;

        public ServiceOrderUnKnownStatusHandlerService(IOptions<OrigoConfiguration> options,
            IEmailService emailService,
            IHardwareServiceOrderRepository hardwareServiceOrderRepository)
            : base(hardwareServiceOrderRepository)
        {
            _origoConfiguration = options.Value;
            _emailService = emailService;
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
        }

        /// <inheritdoc cref="UpdateServiceOrderStatusAsync(Guid, ServiceStatusEnum)"/>
        public override async Task UpdateServiceOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus)
        {
            if (newStatus != ServiceStatusEnum.Unknown)
                throw new ArgumentException("This handler connot handle this status");

            var order = await _hardwareServiceOrderRepository.UpdateOrderStatusAsync(orderId, ServiceStatusEnum.Unknown);

            var parameters = new Dictionary<string, string>
            {
                { "Order", JsonSerializer.Serialize(order) },
                {"OrderLink", string.Format($"{_origoConfiguration.BaseUrl}/{_origoConfiguration.OrderPath}", order.CustomerId, order.ExternalId)}
            };

            await _emailService.SendEmailAsync(_origoConfiguration.DeveloperEmail, $"{ServiceStatusEnum.Unknown}_Subject", $"{ServiceStatusEnum.Unknown}_Body", parameters);
        }
    }
}
