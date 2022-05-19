using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices.Email;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hardware-repair-notifications")]
    public class HardwareRepairNotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<HardwareRepairController> _logger;

        public HardwareRepairNotificationController(IEmailService emailService, ILogger<HardwareRepairController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Send asset repair notification email
        /// </summary>
        /// <param name="statusIds"></param>
        /// <param name="olderThan"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("asset-repair")]
        public async Task<IActionResult> SendAssetRepairNotification([FromBody] List<ServiceStatusEnum> statusIds, DateTime? olderThan = null, string languageCode = "EN")
        {
            if (olderThan == null)
                olderThan = DateTime.Today.AddDays(-7);

            var response = await _emailService.SendAssetRepairEmailAsync(olderThan.GetValueOrDefault(), statusIds.Cast<int>().ToList(), languageCode);

            return Ok(response);
        }

        /// <summary>
        /// Send return loan device email notification
        /// </summary>
        /// <param name="statusIds"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("loan-device")]
        public async Task<IActionResult> SendLoanDeviceNotification([FromBody] List<ServiceStatusEnum> statusIds, string languageCode = "EN")
        {
            var response = await _emailService.SendLoanDeviceEmailAsync(statusIds.Cast<int>().ToList(), languageCode);

            return Ok(response);
        }

        /// <summary>
        /// Send order discarding email
        /// </summary>
        /// <param name="statusId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("order-discarded")]
        public async Task<IActionResult> SendOrderDiscardedNotification(int? statusId, string languageCode = "EN")
        {
            var response = await _emailService.SendOrderDiscardedEmailAsync(statusId.GetValueOrDefault(), languageCode);
            return Ok(response);
        }
    }
}
