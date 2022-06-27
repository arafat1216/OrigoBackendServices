using Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response;
using OrigoApiGateway.Services;
using System.Net;
using System.Security.Claims;

#nullable enable

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

        /// <summary>
        /// Configures SUR for a customer
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="serviceId">Service Identifier</param>
        /// <returns>Whole customer settings</returns>
        [Route("{customerId:Guid}/config/sur")]
        [HttpPatch]
        [ProducesResponseType(typeof(CustomerSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConfigureSur(Guid customerId, [FromBody] string serviceId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                {
                    return Forbid();
                }
            }

            var settings = await _hardwareRepairService.ConfigureServiceIdAsync(customerId, serviceId);
            return Ok(settings);
        }

        /// <summary>
        /// Configures loan device for a customer
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="loanDevice">Loan device details</param>
        /// <returns>Whole customer settings</returns>
        [Route("{customerId:Guid}/config/loan-device")]
        [HttpPatch]
        [ProducesResponseType(typeof(CustomerSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ConfigureLoanDevice(Guid customerId, [FromBody] LoanDevice loanDevice)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                {
                    return Forbid();
                }
            }

            var settings = await _hardwareRepairService.ConfigureLoanPhoneAsync(customerId, loanDevice);
            return Ok(settings);
        }

        /// <summary>
        /// Get customers settings
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <returns>Whole customer settings</returns>
        [Route("{customerId:Guid}/config")]
        [HttpGet]
        [ProducesResponseType(typeof(CustomerSettings), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetConfiguration(Guid customerId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                {
                    return Forbid();
                }
            }
            var settings = await _hardwareRepairService.GetSettingsAsync(customerId);
            return Ok(settings);
        }

        /// <summary>
        /// Creates a hardware service order
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="model">Order details</param>
        /// <returns>New hardware service order</returns>
        [Route("{customerId:Guid}/orders")]
        [HttpPost]
        [ProducesResponseType(typeof(HardwareServiceOrder), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateHardwareServiceOrder(Guid customerId, [FromBody] NewHardwareServiceOrder model)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                {
                    return Forbid();
                }
            }

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (!Guid.TryParse(userId, out Guid userIdGuid))
                return BadRequest();

            var newOrder = await _hardwareRepairService.CreateHardwareServiceOrderAsync(customerId, userIdGuid, model);

            return Ok(newOrder);
        }

        /// <summary>
        /// Gets a hardware service order
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="orderId">Order Identifier</param>
        /// <returns>Existing hardware service order</returns>
        [Route("{customerId:Guid}/orders/{orderId:Guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(HardwareServiceOrder), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHardwareServiceOrder(Guid customerId, Guid orderId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == PredefinedRole.EndUser.ToString() || role == PredefinedRole.DepartmentManager.ToString() || role == PredefinedRole.Manager.ToString())
            {
                return Forbid();
            }

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                {
                    return Forbid();
                }
            }
            var order = await _hardwareRepairService.GetHardwareServiceOrderAsync(customerId, orderId);

            return Ok(order);
        }


        /// <summary>
        /// Gets list of hardware service orders for a customer
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="page">Page number</param>
        /// <param name="userId">me for userId</param>
        /// <param name="limit">Number of items to be returned</param>
        /// <returns>List of hardware service orders</returns>
        [Route("{customerId:Guid}/orders")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HardwareServiceOrder>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHardwareServiceOrders(Guid customerId, string userId, [FromQuery] bool activeOnly = false, int page = 1, int limit = 25)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                {
                    return Forbid();
                }
            }
            if (role == PredefinedRole.EndUser.ToString())
            {
                userId = "me";
            }

            Guid? userIdGuid = null;
            if (userId == "me")
                userIdGuid = new Guid(HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value);

            var orders = await _hardwareRepairService.GetHardwareServiceOrdersAsync(customerId, userIdGuid, activeOnly, page, limit);

            return Ok(orders);
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
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (role != PredefinedRole.SystemAdmin.ToString())
            {
                var accessList = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "AccessList")?.Value;
                if (accessList == null || !accessList.Any() || !accessList.Contains(customerId.ToString()))
                {
                    return Forbid();
                }
            }

            return Ok();
        }
    }
}
