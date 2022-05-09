using HardwareServiceOrderServices.Email.Models;

namespace HardwareServiceOrderServices.Email
{
    public interface IEmailService
    {
        Task SendAsync(string subject, string to, string type, object data);
        /// <summary>
        /// Send order confirmation email
        /// </summary>
        /// <param name="order"></param>
        /// <param name="languageCode">Code of the language to be used for sending email</param>
        /// <returns></returns>
        Task SendOrderConfirmationEmailAsync(OrderConfirmation order, string languageCode);
    }
}
