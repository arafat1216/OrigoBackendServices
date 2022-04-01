using System;
using System.ComponentModel.DataAnnotations;

namespace OrigoApiGateway.Models
{
    /// <summary>
    ///     A partner read-model.
    /// </summary>
    public class Partner
    {
        /// <summary>
        ///     The partner's ID.
        /// </summary>
        [Required]
        public Guid Id { get; set; }

        //public Guid OrganizationId { get; init; }

        /// <summary>
        ///     The name of the partner's organization.
        /// </summary>
        [Required]
        public string Name { get; init; }

        /// <summary>
        ///     The organization-number as used in the national registry.
        /// </summary>
        [Required]
        public string OrganizationNumber { get; init; }

        public Address Address { get; init; }

        public OrigoContactPerson ContactPerson { get; init; }
    }
}
