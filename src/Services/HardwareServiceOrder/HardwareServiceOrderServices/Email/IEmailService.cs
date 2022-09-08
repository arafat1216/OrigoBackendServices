using HardwareServiceOrderServices.Email.Models;

namespace HardwareServiceOrderServices.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// Send order confirmation email
        /// </summary>
        /// <param name="order">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format</param>
        /// <returns></returns>
        Task SendOrderConfirmationEmailAsync(OrderConfirmationEmail order, string languageCode);
        
        /// <summary>
        /// Send order confirmation email for Packaging service
        /// </summary>
        /// <param name="order">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format</param>
        /// <returns></returns>
        Task SendOrderConfirmationEmailAsync(RemarketingPackaging order, string languageCode);
        
        /// <summary>
        /// Send order confirmation email for Non Packaging service
        /// </summary>
        /// <param name="order">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format</param>
        /// <returns></returns>
        Task SendOrderConfirmationEmailAsync(RemarketingNoPackaging order, string languageCode);

        /// <summary>
        /// Send asset repair email
        /// </summary>
        /// <param name="olderThan">The DateTime for filtering orders. If specified, all orders that are older than specified datetime will be returned. If not specified, seven days older orders will be returned.</param>
        /// <param name="statusId">Status identifier.</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format.</param>
        /// <returns></returns>
        Task<List<AssetRepairEmail>> SendAssetRepairEmailAsync(DateTime? olderThan, int? statusId, string languageCode = "en");

        /// <summary>
        /// Send loan device notification email
        /// </summary>
        /// <param name="statusIds">List of status IDs for filtering.</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format.</param>
        /// <returns></returns>
        Task<List<LoanDeviceEmail>> SendLoanDeviceEmailAsync(List<int> statusIds, string languageCode = "en");

        /// <summary>
        /// Send order discarding email
        /// </summary>
        /// <param name="statusId">Status identifier</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format.</param>
        /// <returns></returns>
        Task<List<AssetDiscardedEmail>> SendOrderDiscardedEmailAsync(int statusId, string languageCode = "en");

        /// <summary>
        /// Send email for cancelled order
        /// </summary>
        /// <param name="statusId">Status identifier</param>
        /// <param name="languageCode">Code of the language in ISO 639-1 format.</param>
        /// <returns></returns>
        Task<List<OrderCancellationEmail>> SendOrderCancellationEmailAsync(int statusId, string languageCode = "en");

        /// <summary>
        /// Send email
        /// </summary>
        /// <param name="to">The receipient</param>
        /// <param name="bodyTemplateKey">Name of resource file key for retrieving body</param>
        /// <param name="subjectKey">Name of the resource file key for retrieving subject</param>
        /// <param name="parameters">List fields used in the email body.</param>
        /// <param name="languageCode"></param>
        /// <returns>Code of the language in ISO 639-1 format.</returns>
        Task SendEmailAsync(string to, string subjectKey, string bodyTemplateKey, Dictionary<string, string> parameters, string languageCode = "en");
    }
}
