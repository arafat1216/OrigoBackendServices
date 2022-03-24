using System;

namespace Customer.API.ViewModels
{
    /// <inheritdoc cref="CustomerServices.Models.Partner"/>
    public record Partner
    { 
        /// <inheritdoc cref="CustomerServices.Models.Partner.ExternalId"/>
        public Guid ExternalId { get; set; }

        /// <inheritdoc cref="CustomerServices.Models.Partner.Organization"/>
        public PartnerOrganization Organization { get; set; }


        public Partner(Guid externalId, PartnerOrganization organization)
        {
            ExternalId = externalId;
            Organization = organization;
        }
    }
}
