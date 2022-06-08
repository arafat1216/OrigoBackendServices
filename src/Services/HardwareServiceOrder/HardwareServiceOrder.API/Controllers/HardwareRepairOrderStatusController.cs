using HardwareServiceOrderServices;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hardware-repair-order-status")]
    public class HardwareRepairOrderStatusController : ControllerBase
    {
        private readonly IHardwareServiceOrderService _hardwareServiceOrderService;
        public HardwareRepairOrderStatusController(IHardwareServiceOrderService hardwareServiceOrderService)
        {
            _hardwareServiceOrderService = hardwareServiceOrderService;
        }

        public async Task<IActionResult> UpdateStatus()
        {
            return Ok();
        }
    }
}
