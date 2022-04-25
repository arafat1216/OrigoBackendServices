using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Services;
using System;
using System.Net;
using System.Threading.Tasks;

namespace OrigoApiGateway.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    // Assets should only be available through a given customer
    [Route("/origoapi/v{version:apiVersion}/hardware-repair")]
    public class HardwareRepairController : ControllerBase
    {
        private readonly IHardwareRepairService _hardwareRepairService;
        public HardwareRepairController(IHardwareRepairService hardwareRepairService)
        {
            _hardwareRepairService = hardwareRepairService;
        }

        [Route("{customerId:Guid}/config/sur")]
        [HttpPatch]
        [ProducesResponseType(typeof(CustomerSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConfigureSur(Guid customerId, [FromBody] string serviceId)
        {
            var settings = await _hardwareRepairService.ConfigureServiceIdAsync(customerId, serviceId);
            return Ok(settings);
        }

        [Route("{customerId:Guid}/config/loan-device")]
        [HttpPatch]
        [ProducesResponseType(typeof(CustomerSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConfigureLoanDevice(Guid customerId, [FromBody] LoanDevice loanDevice)
        {
            var settings = await _hardwareRepairService.ConfigureLoanPhoneAsync(customerId, loanDevice);
            return Ok(settings);
        }

        [Route("{customerId:Guid}/config")]
        [HttpGet]
        [ProducesResponseType(typeof(CustomerSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetConfiguration(Guid customerId)
        {
            var settings = await _hardwareRepairService.GetSettingsAsync(customerId);
            return Ok(settings);
        }
    }
}
