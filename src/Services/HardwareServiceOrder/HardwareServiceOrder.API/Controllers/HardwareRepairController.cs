using AutoMapper;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices;
using HardwareServiceOrderServices.ServiceModels;
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
            IMapper mapper)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
            _logger = logger;
            _mapper = mapper;
        }

        [Route("{customerId:Guid}/config/sur")]
        [HttpPatch]
        [SwaggerOperation(Tags = new[] { "Configuration" })]
        public async Task<IActionResult> ConfigureSur(Guid customerId, [FromBody] string serviceId, Guid callerId)
        {
            var settings = await _hardwareServiceOrderService.ConfigureServiceIdAsync(customerId, serviceId, callerId);
            return Ok(_mapper.Map<CustomerSettings>(settings));
        }

        [Route("{customerId:Guid}/config/loan-device")]
        [HttpPatch]
        [SwaggerOperation(Tags = new[] { "Configuration" })]
        public async Task<IActionResult> ConfigureLoanDevice(Guid customerId, [FromBody] LoanDevice loanDevice)
        {
            var settings = await _hardwareServiceOrderService.ConfigureLoanPhoneAsync(customerId, loanDevice?.PhoneNumber, loanDevice?.Email, loanDevice.CallerId);
            return Ok(_mapper.Map<CustomerSettings>(settings));
        }

        [Route("{customerId:Guid}/config")]
        [HttpGet]
        [ProducesResponseType(typeof(CustomerSettings), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Configuration" })]
        public async Task<IActionResult> GetConfiguration(Guid customerId)
        {
            var settings = await _hardwareServiceOrderService.GetSettingsAsync(customerId);
            return Ok(_mapper.Map<CustomerSettings>(settings));
        }

        [Route("{customerId:Guid}/orders")]
        [HttpPost]
        [ProducesResponseType(typeof(ViewModels.HardwareServiceOrder), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> CreateHardwareServiceOrder(Guid customerId, [FromBody] ViewModels.HardwareServiceOrder model)
        {
            var dto = _mapper.Map<HardwareServiceOrderDTO>(model);
            var vm = _mapper.Map<ViewModels.HardwareServiceOrder>(await _hardwareServiceOrderService.CreateHardwareServiceOrderAsync(customerId, dto));
            return Ok(vm);
        }

        [Route("{customerId:Guid}/orders/{orderId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ViewModels.HardwareServiceOrder), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> GetHardwareServiceOrder(Guid customerId, Guid orderId)
        {
            var dto = await _hardwareServiceOrderService.GetHardwareServiceOrderAsync(customerId, orderId);
            return Ok(_mapper.Map<ViewModels.HardwareServiceOrder>(dto));
        }

        [Route("{customerId:Guid}/orders/{orderId:Guid}")]
        [HttpPatch]
        [ProducesResponseType(typeof(ViewModels.HardwareServiceOrder), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> UpdateHardwareServiceOrder(Guid customerId, Guid orderId, ViewModels.HardwareServiceOrder model)
        {
            var dto = _mapper.Map<HardwareServiceOrderDTO>(model);
            var vm = _mapper.Map<ViewModels.HardwareServiceOrder>(await _hardwareServiceOrderService.UpdateHardwareServiceOrderAsync(customerId, orderId, dto));
            return Ok(vm);
        }

        [Route("{customerId:Guid}/orders")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ViewModels.HardwareServiceOrder>), (int)HttpStatusCode.OK)]
        [SwaggerOperation(Tags = new[] { "Orders" })]
        public async Task<IActionResult> GetHardwareServiceOrders(Guid customerId)
        {
            var dto = await _hardwareServiceOrderService.GetHardwareServiceOrdersAsync(customerId);
            return Ok(_mapper.Map<List<ViewModels.HardwareServiceOrder>>(dto));
        }

        /// <summary>
        /// Gets all logs  associated with a hardware service order
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="orderId">Order Identifier</param>
        /// <returns>Existing hardware service order</returns>
        [Route("{customerId:Guid}/orders/{orderId:Guid}/logs")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HardwareServiceOrderLog>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHardwareServiceOrderLogs(Guid customerId, Guid orderId)
        {
            var dto = await _hardwareServiceOrderService.GetHardwareServiceOrderLogsAsync(customerId, orderId);
            return Ok(_mapper.Map<List<HardwareServiceOrderLog>>(dto));
        }
    }
}
