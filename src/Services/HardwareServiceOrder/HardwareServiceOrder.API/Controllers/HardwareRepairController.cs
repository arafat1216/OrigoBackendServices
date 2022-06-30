using AutoMapper;
using Common.Interfaces;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace HardwareServiceOrder.API.Controllers
{
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

        [Route("{customerId:Guid}/config/sur")]
        [HttpPatch]
        [SwaggerOperation(Tags = new[] { "Configuration" })]
        public async Task<IActionResult> ConfigureSur(Guid customerId, [FromBody] ViewModels.CustomerServiceProvider customerServiceProvider, Guid callerId)
        {
            var settings = await _hardwareServiceOrderService.ConfigureCustomerSettingsAsync(customerId, callerId);

            var serviceId = await _hardwareServiceOrderService.ConfigureCustomerServiceProviderAsync(customerServiceProvider.ProviderId, customerId, customerServiceProvider.ApiUserName, customerServiceProvider.ApiPassword);

            var response = new CustomerSettingsResponseDTO(_mapper.Map<ViewModels.CustomerSettings>(settings))
            {
                ApiUsername = serviceId
            };

            return Ok(response);
        }

        /// <summary>
        /// Configure loan device
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="loanDevice">Loan device details</param>
        /// <param name="callerId">API caller's identifier</param>
        /// <returns></returns>
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

        [Route("{customerId:Guid}/orders")]
        [HttpPost]
        //[ProducesResponseType(typeof(ViewModels.HardwareServiceOrderResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HardwareServiceOrderDTO), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> CreateHardwareServiceOrder(Guid customerId, [FromBody] ViewModels.NewHardwareServiceOrder model)
        {
            var dto = _mapper.Map<NewHardwareServiceOrderDTO>(model);
            var vm = await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(customerId, dto);

            //return Ok(_mapper.Map<ViewModels.HardwareServiceOrderResponse>(vm));
            return Ok(vm);
        }

        [Route("{customerId:Guid}/orders/{orderId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(HardwareServiceOrderDTO), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> GetHardwareServiceOrder(Guid customerId, Guid orderId)
        {
            // TODO: Fix this so it don't create a new object when the result is null!
            var dto = await _hardwareServiceOrderService.GetHardwareServiceOrderAsync(customerId, orderId) ?? new HardwareServiceOrderServices.ServiceModels.HardwareServiceOrderDTO();

            //return Ok(_mapper.Map<ViewModels.HardwareServiceOrderResponse>(dto));
            return Ok(dto);
        }


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
