using HardwareServiceOrderServices;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    /// <summary>
    /// Controller for handling new status of the orders
    /// </summary>
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

        /// <summary>
        /// This method retrieves latest statuses from service provider's and updates origo hardware service order statuses
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateStatus()
        {
            await _hardwareServiceOrderService.UpdateOrderStatusAsync();

            return Ok();
        }
    }
}
