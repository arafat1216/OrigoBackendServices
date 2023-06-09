﻿using HardwareServiceOrder.API.Filters;
using HardwareServiceOrder.API.ViewModels;
using HardwareServiceOrderServices.Email;
using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/hardware-repair-notifications")]
    [ServiceFilter(typeof(ErrorExceptionFilter))]
    public class HardwareRepairNotificationController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<HardwareRepairController> _logger;

        public HardwareRepairNotificationController(IEmailService emailService, ILogger<HardwareRepairController> logger, IApiRequesterService apiRequesterService)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Send asset repair notification email
        /// </summary>
        /// <param name="statusId">Status identifier</param>
        /// <param name="olderThan">The DateTime for filtering orders. If specified, all orders that are older than specified datetime will be returned. If not specified, seven days older orders will be returned.</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("asset-repair")]
        public async Task<IActionResult> SendAssetRepairNotification(int? statusId, DateTime? olderThan = null, string languageCode = "en")
        {
            if (olderThan == null)
                olderThan = DateTime.Today.AddDays(-7);
            statusId = statusId ?? (int)ServiceStatusEnum.Registered;
            var response = await _emailService.SendAssetRepairEmailAsync(olderThan.GetValueOrDefault(), statusId, languageCode.ToLower());

            return Ok(response);
        }

        /// <summary>
        /// Send return loan device email notification
        /// </summary>
        /// <param name="statusIds">List of statuses for filtering</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format</param>
        /// <returns></returns>
        [HttpPost]
        [Route("loan-device")]
        public async Task<IActionResult> SendLoanDeviceNotification([FromBody] List<ServiceStatusEnum>? statusIds, string languageCode = "en")
        {
            statusIds = statusIds == null || !statusIds.Any() ? new List<ServiceStatusEnum>
            {
                ServiceStatusEnum.CompletedRepaired,
                ServiceStatusEnum.CompletedRepairedOnWarranty,
                ServiceStatusEnum.CompletedReplaced,
                ServiceStatusEnum.CompletedReplacedOnWarranty,
                ServiceStatusEnum.CompletedNotRepaired
            } : statusIds;

            var response = await _emailService.SendLoanDeviceEmailAsync(statusIds.Cast<int>().ToList(), languageCode.ToLower());

            return Ok(response);
        }

        /// <summary>
        /// Send order discarding email
        /// </summary>
        /// <param name="statusId">Status identifier</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format</param>
        /// <returns></returns>
        [HttpPost]
        [Route("order-discarded")]
        public async Task<IActionResult> SendOrderDiscardedNotification(int? statusId, string languageCode = "en")
        {
            statusId = statusId ?? (int)ServiceStatusEnum.CompletedDiscarded;
            var response = await _emailService.SendOrderDiscardedEmailAsync(statusId.GetValueOrDefault(), languageCode.ToLower());
            return Ok(response);
        }

        /// <summary>
        /// Send order discarding email
        /// </summary>
        /// <param name="statusId">Status identifier</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format</param>
        /// <returns></returns>
        [HttpPost]
        [Route("order-cancelled")]
        public async Task<IActionResult> SendOrderCancellationNotification(int? statusId, string languageCode = "en")
        {
            statusId = statusId ?? (int)ServiceStatusEnum.Canceled;
            var response = await _emailService.SendOrderCancellationEmailAsync(statusId.GetValueOrDefault(), languageCode.ToLower());
            return Ok(response);
        }
    }
}
