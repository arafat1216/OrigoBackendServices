using System;

namespace Customer.API.ApiModels
{
    public record Partner
    { 
        public Guid ExternalId { get; set; }
        public Organization Organization { get; set; }
    }
}
