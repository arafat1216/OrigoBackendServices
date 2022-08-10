using CustomerServices.Email.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// Invitation email to the customers user to onboard Origo.
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task InvitationEmailToUserAsync(InvitationMail emailData, string languageCode);
        /// <summary>
        /// Employee Offboard Overdue Email to the customers managers to Notify.
        /// </summary>
        /// <param name="emailData">Information that will be dynamically inserted into the email.</param>
        /// <param name="languageCode">Code of the language to be used for sending email.</param>
        /// <returns></returns>
        Task OffboardingOverdueEmailToManagersAsync(OffboardingOverdueMail emailData, string languageCode);

    }
}
