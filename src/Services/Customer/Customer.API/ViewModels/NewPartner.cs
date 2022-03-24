using System;

namespace Customer.API.ViewModels
{
    /// <summary>
    ///     Registers a new partner.
    /// </summary>
    public class NewPartner
    {
        /// <summary>
        ///     The ID for the existing organization that should be registered as a partner.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     The ID of the user that is creating the partner.
        /// </summary>
        public Guid CallerId { get; set; }
    }
}
