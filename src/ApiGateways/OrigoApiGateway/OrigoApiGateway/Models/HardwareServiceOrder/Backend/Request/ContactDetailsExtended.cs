#nullable enable

using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
    public class ContactDetailsExtended : ContactDetails
    {
        /// <summary>
        ///     The ID of the <see cref="ContactDetails.UserId">user's</see> organization
        /// </summary>
        /// <example> 00000000-0000-0000-0000-000000000000 </example>
        [Required]
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     The name of the <see cref="ContactDetails.UserId">user's</see> organization.
        /// </summary>
        /// <example> MYTOS AS </example>
        [Required]
        public string OrganizationName { get; set; }

        /// <summary>
        ///     The national organization-number the <see cref="ContactDetails.UserId">user's</see> organization.
        /// </summary>
        /// <example> 917 070 709 </example>
        [Required]
        public string? OrganizationNumber { get; set; }

        /// <summary>
        ///     The ID of the <see cref="ContactDetails.UserId">user's</see> partner.
        /// </summary>
        /// <example> 00000000-0000-0000-0000-000000000000 </example>
        [Required]
        public Guid PartnerId { get; set; }

        /// <summary>
        ///     The name of the <see cref="ContactDetails.UserId">user's</see> partner.
        /// </summary>
        /// <example> TECHSTEP ASA </example>
        [Required]
        public string PartnerName { get; set; }

        /// <summary>
        ///     The national organization-number for the <see cref="ContactDetails.UserId">user's</see> partner.
        /// </summary>
        /// <example> 977 037 093 </example>
        [Required]
        public string PartnerOrganizationNumber { get; set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactDetailsExtended"/> intended for use with JSON (de-)serializers, AutoMapper, 
        ///     unit-testing and other automated tools.
        /// </summary>
        [Obsolete("Reserved for use by JSON (de-)serializers, AutoMapper, unit-testing and other automated tools.")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ContactDetailsExtended() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {

        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactDetailsExtended"/>-class.
        /// </summary>
        /// <param name="userId"> The user's ID. </param>
        /// <param name="firstName"> The user's first-name. </param>
        /// <param name="lastName"> The user's last-name. </param>
        /// <param name="email"> The user's e-mail. </param>
        /// <param name="phoneNumber"> The user's phone-number in E.164 format. </param>
        /// <param name="organizationId"> The ID of the user's organization. </param>
        /// <param name="organizationName"> The name of the user's organization. </param>
        /// <param name="organizationNumber"> The national organization-number the users organization. </param>
        /// <param name="partnerId"> The ID of the user's partner. </param>
        /// <param name="partnerName"> The name of the user's partner. </param>
        /// <param name="partnerOrganizationNumber"> The national organization-number for the user's partner. </param>
        public ContactDetailsExtended(Guid userId, string firstName, string lastName, string email, string? phoneNumber, Guid organizationId, string organizationName, string? organizationNumber, Guid partnerId, string partnerName, string partnerOrganizationNumber) : base(userId, firstName, lastName, email, phoneNumber)
        {
            OrganizationId = organizationId;
            OrganizationName = organizationName;
            OrganizationNumber = organizationNumber;
            PartnerId = partnerId;
            PartnerName = partnerName;
            PartnerOrganizationNumber = partnerOrganizationNumber;
        }
    }
}
