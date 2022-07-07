using AutoMapper;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HardwareServiceOrder.API.Controllers
{
    /// <summary>
    /// Endpoints for hardware service orders
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hardware-repair")]
    public class HardwareRepairController : ControllerBase
    {
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        private readonly ILogger<HardwareRepairController> _logger;
        private readonly IMapper _mapper;

        public HardwareRepairController(
            IHardwareServiceOrderService hardwareServiceOrderService,
            ILogger<HardwareRepairController> logger,
            IMapper mapper,
            IApiRequesterService apiRequesterService)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Configures SUR for a customer
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="customerServiceProvider">Customer service provider details</param>
        /// <param name="callerId">Caller's identifier</param>
        /// <returns>Whole customer settings</returns>
        [Route("{customerId:Guid}/config/sur")]
        [HttpPatch]
        [SwaggerOperation(Tags = new[] { "Configuration" })]
        public async Task<IActionResult> ConfigureSur(Guid customerId, [FromBody] ViewModels.CustomerServiceProvider customerServiceProvider, Guid callerId)
        {
            var settings = await _hardwareServiceOrderService.ConfigureCustomerSettingsAsync(customerId, callerId);

            var serviceId = await _hardwareServiceOrderService.ConfigureCustomerServiceProviderAsync(customerServiceProvider.ProviderId, customerId, customerServiceProvider.ApiUserName, customerServiceProvider.ApiPassword);

            var response = new CustomerSettingsResponseDTO(_mapper.Map<ViewModels.CustomerSettings>(settings))
            {
                ApiUsername = customerServiceProvider.ApiUserName
            };

            return Ok(response);
        }

        /// <summary>
        /// Configure loan device
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="loanDevice">Loan device details</param>
        /// <param name="callerId">API caller's identifier</param>
        /// <returns>Whole customer settings</returns>
        [Route("{customerId:Guid}/config/loan-device")]
        [HttpPatch]
        [SwaggerOperation(Tags = new[] { "Configuration" })]
        public async Task<IActionResult> ConfigureLoanDevice(Guid customerId, [FromBody] LoanDevice loanDevice, Guid callerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var settings = await _hardwareServiceOrderService.ConfigureLoanPhoneAsync(customerId, loanDevice.PhoneNumber, loanDevice.Email, loanDevice.ProvidesLoanDevice, callerId);

            var response = new CustomerSettingsResponseDTO(_mapper.Map<ViewModels.CustomerSettings>(settings))
            {
                ApiUsername = await _hardwareServiceOrderService.GetServicerProvidersUsernameAsync(customerId, (int)ServiceProviderEnum.ConmodoNo)
            };

            return Ok(response);
        }

        /// <summary>
        /// Get customers settings
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <returns>Whole customer settings</returns>
        [Route("{customerId:Guid}/config")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.CustomerSettings), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Configuration" })]
        public async Task<IActionResult> GetConfiguration(Guid customerId)
        {
            var settings = await _hardwareServiceOrderService.GetSettingsAsync(customerId);

            var dto = _mapper.Map<ViewModels.CustomerSettings>(settings) ?? new ViewModels.CustomerSettings { CustomerId = customerId };

            return Ok(new CustomerSettingsResponseDTO(dto)
            {
                ApiUsername = await _hardwareServiceOrderService.GetServicerProvidersUsernameAsync(customerId, (int)ServiceProviderEnum.ConmodoNo)
            });
        }

        /// <summary>
        /// Creates a hardware service order
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="model">Order details</param>
        /// <returns>New hardware service order</returns>
        [Route("{customerId:Guid}/orders")]
        [HttpPost]
        [ProducesResponseType(typeof(HardwareServiceOrderDTO), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> CreateHardwareServiceOrder(Guid customerId, [FromBody] ViewModels.NewHardwareServiceOrder model)
        {
            var dto = _mapper.Map<NewHardwareServiceOrderDTO>(model);
            var vm = await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(customerId, dto);

            //return Ok(_mapper.Map<ViewModels.HardwareServiceOrderResponse>(vm));
            return Ok(vm);
        }

        /// <summary>
        /// Gets a hardware service order
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="orderId">Order Identifier</param>
        /// <returns>Existing hardware service order</returns>
        [Route("{customerId:Guid}/orders/{orderId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(HardwareServiceOrderDTO), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> GetHardwareServiceOrder(Guid customerId, Guid orderId)
        {
            // TODO: Fix this so it don't create a new object when the result is null!
            var dto = await _hardwareServiceOrderService.GetHardwareServiceOrderAsync(customerId, orderId) ?? new HardwareServiceOrderServices.ServiceModels.HardwareServiceOrderDTO();

            return Ok(dto);
        }

        /// <summary>
        /// Gets list of hardware service orders for a customer
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="page">Page number</param>
        /// <param name="userId">me for userId</param>
        /// <param name="limit">Number of items to be returned</param>
        /// <param name="activeOnly">Should return active orders only?</param>
        /// <returns>List of hardware service orders</returns>
        [Route("{customerId:Guid}/orders")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HardwareServiceOrderDTO>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> GetHardwareServiceOrders(Guid customerId, Guid? userId, [FromQuery] bool activeOnly, CancellationToken cancellationToken, int page = 1, int limit = 25)
        {
            var dto = await _hardwareServiceOrderService.GetHardwareServiceOrdersAsync(customerId, userId, activeOnly, cancellationToken, page, limit);

            return Ok(dto);
        }
    }
}
