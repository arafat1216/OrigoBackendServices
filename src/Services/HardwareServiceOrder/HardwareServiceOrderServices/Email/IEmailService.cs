﻿using HardwareServiceOrderServices.Email.Models;

namespace HardwareServiceOrderServices.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// Send order confirmation email
        /// </summary>
        /// <param name="order">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task SendOrderConfirmationEmailAsync(OrderConfirmationEmail order, string languageCode);

        /// <summary>
        /// Send asset repair email
        /// </summary>
        /// <param name="olderThan"></param>
        /// <param name="statusId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        Task<List<AssetRepairEmail>> SendAssetRepairEmailAsync(DateTime? olderThan, int? statusId, string languageCode = "en");

        /// <summary>
        /// Send loan device notification email
        /// </summary>
        /// <param name="statusIds"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        Task<List<LoanDeviceEmail>> SendLoanDeviceEmailAsync(List<int> statusIds, string languageCode = "en");

        /// <summary>
        /// Send order discarding email
        /// </summary>
        /// <param name="statusId"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        Task<List<AssetDiscardedEmail>> SendOrderDiscardedEmailAsync(int statusId, string languageCode = "en");
    }
}
