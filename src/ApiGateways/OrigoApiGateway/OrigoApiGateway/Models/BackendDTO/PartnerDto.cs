using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class PartnerDTO
    {
        /// <summary>
        ///     The partner's external (customer-facing) ID
        /// </summary>
        public Guid ExternalId { get; set; }

        public OrganizationDTO Organization { get; set; }
    }
}
