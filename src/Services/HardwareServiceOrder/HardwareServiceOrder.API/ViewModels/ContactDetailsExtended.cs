namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    ///     Extends the <see cref="ContactDetailsDTO"/>-class by adding in additional details about the organization and partner.
    /// </summary>
    public class ContactDetailsExtended : ContactDetails
    {
        /// <inheritdoc cref="ContactDetailsExtendedDTO.OrganizationId"/>
        public Guid OrganizationId { get; set; }

        /// <inheritdoc cref="ContactDetailsExtendedDTO.OrganizationName"/>
        public string OrganizationName { get; set; }

        /// <inheritdoc cref="ContactDetailsExtendedDTO.OrganizationNumber"/>
        public string? OrganizationNumber { get; set; }

        /// <inheritdoc cref="ContactDetailsExtendedDTO.PartnerId"/>
        public Guid PartnerId { get; set; }

        /// <inheritdoc cref="ContactDetailsExtendedDTO.PartnerName"/>
        public string PartnerName { get; set; }

        /// <inheritdoc cref="ContactDetailsExtendedDTO.PartnerOrganizationNumber"/>
        public string PartnerOrganizationNumber { get; set; }
    }
}
