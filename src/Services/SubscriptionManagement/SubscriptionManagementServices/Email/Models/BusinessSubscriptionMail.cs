using SubscriptionManagementServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.Email.Models
{
    public class BusinessSubscriptionMail
    {
        public BusinessSubscriptionMail(BusinessSubscription? businessSubscription)
        {
            if (businessSubscription == null)
            {
                OrganizationNumber = "N/A";
                Address = "N/A";
                Country = "N/A";
                Name = "N/A";
                PostalCode = "N/A";
                PostalPlace = "N/A";
            }
            else
            {
                Name = businessSubscription.Name ?? "N/A";
                OrganizationNumber = businessSubscription.OrganizationNumber ?? "N/A";
                Address = businessSubscription.Address ?? "N/A";
                Country = businessSubscription.Country ?? "N/A";
                PostalCode = businessSubscription.PostalCode ?? "N/A";
                PostalPlace = businessSubscription.PostalPlace ?? "N/A";
            }
        }
        /// <summary>
        /// Namme of the organization.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Organization number.
        /// </summary>
        public string OrganizationNumber { get; set; }
        /// <summary>
        /// Address of the organization.
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Postal code of the organization
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Postal place of the organization
        /// </summary>
        public string PostalPlace { get; set; }
        /// <summary>
        /// Country code of the organization
        /// </summary>
        public string Country { get; set; }
    }
}
