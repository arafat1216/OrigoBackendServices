using AssetServices.Email.Model;
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
        /// <summary>
        /// Asset Rported Email
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task ReportAssetEmailAsync(ReportAssetNotification emailData, string languageCode);
        /// <summary>
        /// Asset Pending Return Email
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task PendingReturnEmailAsync(PendingReturnNotification emailData, string languageCode);
        /// <summary>
        /// Asset Buyout Email
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task AssetBuyoutEmailAsync(AssetBuyoutNotification emailData, string languageCode);
        /// <summary>
        /// Asset Buyout Email when manager execute on behalf of the user
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task ManagerBuyoutEmailAsync(ManagerOnBehalfBuyoutNotification emailData, string languageCode);
        /// Notifying User that manager/admin performed Return on behalf of the user.
        /// </summary>
        /// <param name="emailData">Email Data that will be sent</param>
        /// <param name="languageCode">Language of the user</param>
        Task ManagerReturnEmailAsync(ManagerOnBehalfReturnNotification emailData, string languageCode);

    }
}
