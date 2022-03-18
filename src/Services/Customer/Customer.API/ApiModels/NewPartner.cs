using System;

namespace Customer.API.ApiModels
{
    public class NewPartner
    {
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
    }
}
