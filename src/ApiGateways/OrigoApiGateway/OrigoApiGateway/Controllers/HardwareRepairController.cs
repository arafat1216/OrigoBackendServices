using Common.Enums;
using Common.Interfaces;
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
    /// <summary>
    /// Endpoints for hardware service orders
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    // Assets should only be available through a given customer
    // TODO: This needs to be renamed, as it's not only for repairs!
    [Route("/origoapi/v{version:apiVersion}/hardware-repair")]
    [Obsolete("This controller has been deprecated. Please use the hardware-service endpoints instead where possible. Existing functionality will gradually be migrated over to these.")]
    public class HardwareRepairController : ControllerBase
    {
        private readonly IHardwareRepairService _hardwareRepairService;
        private readonly IAssetServices _assetServices;

        public HardwareRepairController(IHardwareRepairService hardwareRepairService, IAssetServices assetServices)
        {
            _hardwareRepairService = hardwareRepairService;
            _assetServices = assetServices;
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
            if (!await HasPermissionToGetOrCreateOrderAsync(customerId, model.AssetId))
                return Forbid();

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (!Guid.TryParse(userId, out Guid userIdGuid))
                return BadRequest();

            var newOrder = await _hardwareRepairService.CreateHardwareServiceOrderAsync(customerId, userIdGuid, model);

            return Ok(newOrder);
        }

        private async Task<bool> HasPermissionToGetOrCreateOrderAsync(Guid customerId, Guid assetId)
        {
            var role = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var asset = await _assetServices.GetAssetForCustomerAsync(customerId, assetId, null);

            var accessList = HttpContext.User.Claims.Where(c => c.Type == "AccessList").Select(y => y.Value).ToList() ?? new List<string> { };

            if (asset == null)
                return false;

            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;

            if (!Guid.TryParse(userId, out Guid userIdGuid))
                return false;

            //Check whether an enduser has permission
            if (role == $"{PredefinedRole.EndUser}" && asset.AssetHolderId != userIdGuid)
            {
                return false;
            }

            //Check whether a manager has permission
            if (new string[] { $"{PredefinedRole.DepartmentManager}", $"{PredefinedRole.Manager}" }.Contains(role))
            {
                if (asset.AssetHolderId != userIdGuid)
                {
                    if (accessList == null || !accessList.Any() || !accessList.Contains($"{asset.ManagedByDepartmentId}"))
                    {
                        return false;
                    }
                }
            }

            //For others except SystemAdmin
            if (role != $"{PredefinedRole.SystemAdmin}")
            {
                if (!accessList.Contains($"{customerId}"))
                {
                    return false;
                }
            }

            return true;
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
            var order = await _hardwareRepairService.GetHardwareServiceOrderAsync(customerId, orderId);

            if (order == null)
                return Forbid();

            if (!await HasPermissionToGetOrCreateOrderAsync(customerId, order.AssetLifecycleId))
                return Forbid();

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
        [ProducesResponseType(typeof(PagedModel<HardwareServiceOrder>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetHardwareServiceOrders(Guid customerId, string? userId, [FromQuery] bool activeOnly = false, int page = 1, int limit = 25)
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


        /*
         * Customer API endpoints:
         * 
         * These endpoints should not offer any additional functionality/data outside what's available in the user/customer-portal.
         * If partner/system-admins, or the backoffice requires additional functionality/data/access, this should be added into a
         * separate backoffice version of the endpoint.
         */




        /*
         * Backoffice only API endpoints
         */

        [HttpGet]
        [Route("provider/")]
        public async Task<ActionResult> Tmp()
        {
            return Ok();
        }
    }
}
