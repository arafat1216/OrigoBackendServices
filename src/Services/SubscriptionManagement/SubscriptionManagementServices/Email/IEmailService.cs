using SubscriptionManagementServices.Email.Models;

namespace SubscriptionManagementServices.Email
{
    public interface IEmailService
    {
        Task SendAsync(string orderType, Guid subscriptionOrderId, object data, Dictionary<string, string> others = null);

        /// <summary>
        /// Sends a mail with information of the activating sim. 
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task ActivateSimMailSendAsync(ActivateSimMail emailData, string languageCode);
        /// <summary>
        /// Sends a mail with information for subscription that is cancelled. 
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task CancelSubscriptionMailSendAsync(CancelSubscriptionMail emailData, string languageCode);
        /// <summary>
        /// Sends a mail with information for the changed subscription. 
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task ChangeSubscriptionMailSendAsync(ChangeSubscriptionMail emailData, string languageCode);
        /// <summary>
        /// Sends a mail to user with information of the new subscription. 
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task NewSubscriptionMailSendAsync(NewSubscriptionMail emailData, string languageCode);
        /// <summary>
        /// Sends a mail to user with information for the ordered sim. 
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task OrderSimMailSendAsync(OrderSimMail emailData, string languageCode);
        /// <summary>
        /// Sends a mail to user with information for the subscription being transfered. 
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task TransferToBusinessMailSendAsync(TransferToBusinessMail emailData, string languageCode);
        /// <summary>
        /// Sends a mail to user with information for the subscription being transfered. 
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task TransferToPrivateMailSendAsync(TransferToPrivateMail emailData, string languageCode);
    }
}
