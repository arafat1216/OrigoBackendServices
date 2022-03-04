using System;

namespace Customer.API.ViewModels
{
    public record Partner
    { 
        public Guid ExternalId { get; set; }
        public Organization Organization { get; set; }
    }
}
