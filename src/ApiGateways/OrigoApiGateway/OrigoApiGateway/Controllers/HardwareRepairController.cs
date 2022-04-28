using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrigoApiGateway.Models.HardwareServiceOrder;
using OrigoApiGateway.Services;
using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> CreateHardwareServiceOrder(Guid customerId, [FromBody] HardwareServiceOrder model)
        {
            return Ok();
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
            return Ok();
        }

        /// <summary>
        /// Updates an existing hardware service order
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="model">Updated order</param>
        /// <returns>Updated hardware service order</returns>
        [Route("{customerId:Guid}/orders/{orderId:Guid}")]
        [HttpPatch]
        [ProducesResponseType(typeof(HardwareServiceOrder), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateHardwareServiceOrder(Guid customerId, Guid orderId, HardwareServiceOrder model)
        {
            return Ok();
        }

        /// <summary>
        /// Gets list of hardware service orders for a customer
        /// </summary>
        /// <param name="customerId">Customer Identifier</param>
        /// <returns>List of hardware service orders</returns>
        [Route("{customerId:Guid}/orders")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HardwareServiceOrder>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHardwareServiceOrders(Guid customerId)
        {
            return Ok();
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
            return Ok();
        }
    }
}
