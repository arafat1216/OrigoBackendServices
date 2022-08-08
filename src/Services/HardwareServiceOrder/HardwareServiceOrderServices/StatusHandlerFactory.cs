using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.Services;
using Microsoft.Extensions.Options;

namespace HardwareServiceOrderServices
{
    /// <summary>
    ///     A factory used for creating and providing different service-order status-handler classes based on the service-types.
    /// </summary>
    public class StatusHandlerFactory : IStatusHandlerFactory
    {
        private readonly IOptions<OrigoConfiguration> _options;
        private readonly IEmailService _emailService;
        private readonly IHardwareServiceOrderRepository _hardwareServiceOrderRepository;
        private readonly IAssetService _assetService;

        /// <summary>
        ///     Initializes a new <see cref="StatusHandlerFactory"/> class utilizing injections.
        /// </summary>
        /// <param name="options"> The injected <see cref="IOptions{TOptions}"/> interface. </param>
        /// <param name="emailService"> The injected <see cref="IEmailService"/> interface. </param>
        /// <param name="hardwareServiceOrderRepository"> The injected <see cref="IHardwareServiceOrderRepository"/> interface. </param>
        /// <param name="assetService"> The injected <see cref="IAssetService"/> interface. </param>
        public StatusHandlerFactory(IOptions<OrigoConfiguration> options,
            IEmailService emailService,
            IHardwareServiceOrderRepository hardwareServiceOrderRepository,
            IAssetService assetService)
        {
            _options = options;
            _emailService = emailService;
            _hardwareServiceOrderRepository = hardwareServiceOrderRepository;
            _assetService = assetService;
        }

        /// <inheritdoc cref="IStatusHandlerFactory.GetStatusHandler"/>
        public ServiceOrderStatusHandlerService GetStatusHandler(ServiceTypeEnum serviceType)
        {
            switch (serviceType)
            {
                case ServiceTypeEnum.SUR:
                    var statusHandler = new ServiceOrderStatusHandlerServiceForSUR(_options, _hardwareServiceOrderRepository, _assetService, _emailService);
                    return statusHandler;
                default:
                    throw new NotSupportedException("This service type currently is not supported");
            }
        }
    }
}
