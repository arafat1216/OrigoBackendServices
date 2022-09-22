using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Email.Models
{
    public class PrivateSubscriptionMail
    {
        public PrivateSubscriptionMail(PrivateSubscription? privateSubscription)
        {
            if (privateSubscription == null) 
            {
                BirthDate = "N/A";
                Address = "N/A";
                Country = "N/A";
                Email = "N/A";
                FirstName = "N/A";
                LastName = "N/A";
                PostalCode = "N/A";
                PostalPlace = "N/A";
            } 
            else 
            {
                BirthDate = privateSubscription.BirthDate.ToShortDateString() ?? "N/A";
                Address = privateSubscription.Address ?? "N/A";
                Country = privateSubscription.Country ?? "N/A";
                Email = privateSubscription.Email ?? "N/A";
                FirstName = privateSubscription.FirstName ?? "N/A";
                LastName = privateSubscription.LastName ?? "N/A";
                PostalCode = privateSubscription.PostalCode ?? "N/A";
                PostalPlace = privateSubscription.PostalPlace ?? "N/A";
            }
        }
        /// <summary>
        /// First name of the person getting the subscription changed/added etc.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last name of the person getting the subscription changed/added etc.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Address of the person getting the subscription changed/added etc.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Postal code of the person getting the subscription changed/added etc.
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Postal place of the person getting the subscription changed/added etc.
        /// </summary>
        public string PostalPlace { get; set; }
        /// <summary>
        /// Country of the person getting the subscription changed/added etc.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Email of the person getting the subscription changed/added etc.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Birth date of the person getting the subscription changed/added etc.
        /// </summary>
        public string BirthDate { get; set; }
    }
}
