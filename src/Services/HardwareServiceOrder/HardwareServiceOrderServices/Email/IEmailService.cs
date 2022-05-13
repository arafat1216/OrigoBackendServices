using HardwareServiceOrderServices.Email.Models;

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
    }
}
