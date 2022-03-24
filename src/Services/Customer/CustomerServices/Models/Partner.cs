using Common.Seedwork;
using CustomerServices.DomainEvents;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable enable

namespace CustomerServices.Models
{
    /// <summary>
    ///     Represents a single partner.
    /// </summary>
    public class Partner : Entity
    {
        /// <summary>
        ///     The partner's own organization. This contains most of the partner's base-information.
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        ///     The partner's external ID.
        /// </summary>
        public Guid ExternalId { get; set; }

        /*
         * EF Navigation
         */

        /// <summary>
        ///     A read-only collection containing all organizations belonging to this partner.
        /// </summary>

        public virtual IReadOnlyCollection<Organization> Customers { get; set; } = new HashSet<Organization>();

        /*
         * Constructors
         */

        /// <summary>
        ///     Constructor reserved for automated Entity Framework operations.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Partner()
        {
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public Partner(Organization organization, Guid callerId)
        {
            ExternalId = Guid.NewGuid();
            Organization = organization;
            CreatedBy = callerId;
            UpdatedBy = callerId;
            AddDomainEvent(new PartnerCreatedDomainEvent(this));
        }
    }
}
