﻿using System;

namespace Customer.API.ViewModels
{
    public record DeleteOrganization
    {
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
        public bool HardDelete { get; set; }
    }
}
