using CustomerServices.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace CustomerServices
{
    public interface IPartnerServices
    {
        /// <summary>
        ///     Uses an existing <see cref="Organization"/> to create a new partner.
        /// </summary>
        /// <param name="organizationId"> The ID of the organization that should be set as a partner. </param>
        /// <returns> The created object. </returns>
        Task<Partner> CreatePartnerAsync(Guid organizationId);

        /// <summary>
        ///     Retrieves a partner using it's external ID.
        /// </summary>
        /// <param name="partnerId"> The ID to retrieve. </param>
        /// <returns> If found, the corresponding partner. Otherwise it returns <see langword="null"/>. </returns>
        Task<Partner?> GetPartnerAsync(Guid partnerId, bool includeOrganization);

        /// <summary>
        ///     Retrieves all partners.
        /// </summary>
        /// <returns> A list containing all partners. </returns>
        Task<IList<Partner>> GetPartnersAsync();
    }
}
