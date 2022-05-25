﻿using AssetServices.Email.Model;
using System.Threading.Tasks;

namespace AssetServices.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// Asset Re-assigned to User Email
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task ReAssignedToUserEmailAsync(ReAssignedToUserNotification emailData, string languageCode);
        /// <summary>
        /// Asset Unassigned From User Email
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task UnassignedFromUserEmailAsync(UnassignedFromUserNotification emailData, string languageCode);
        /// <summary>
        /// Asset Unassigned From Manager Email
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task UnassignedFromManagerEmailAsync(UnassignedFromManagerNotification emailData, string languageCode);

    }
}