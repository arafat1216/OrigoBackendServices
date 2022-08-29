using System;
using System.Collections.Generic;

namespace Customer.API.WriteModels
{
    /// <summary>
    /// Resend invitation request model.
    /// </summary>
    public class ResendInvitation
    {
        /// <summary>
        /// A list of user ids that the invitation mail should be sent to.
        /// </summary>
        public IList<Guid> UserIds { get; set; }
    }
}
