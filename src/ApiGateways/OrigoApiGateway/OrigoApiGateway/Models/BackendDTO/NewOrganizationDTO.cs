﻿using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewOrganizationDTO
    {
        public string Name { get; set; }

        public string OrganizationNumber { get; set; }

        public OrigoAddress Address { get; set; }

        public OrigoContactPerson ContactPerson { get; set; }
        public NewLocation Location { get; set; }
        public Guid PrimaryLocation { get; set; }
        public Guid ParentId { get; set; }
        public Guid CallerId { get; set; }
       
        public string ContactEmail { get; set; }
        public string InternalNotes { get; set; }
        
        public NewOrganizationPreferences Preferences { get; set; }
    }
}