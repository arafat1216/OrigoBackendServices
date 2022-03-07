﻿using System;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public record NewOrganization
    {
        public string Name { get; init; }

        public string OrganizationNumber { get; init; }

        public Address Address { get; init; }

        public OrigoContactPerson ContactPerson { get; init; }

        public Guid ParentId { get; init; }
        public Guid PrimaryLocation { get; init; }
        public string ContactEmail { get; init; }
        public string InternalNotes { get; init; }
        public NewLocation Location { get; init; }
        public NewOrganizationPreferences Preferences { get; init; }
    }
}
