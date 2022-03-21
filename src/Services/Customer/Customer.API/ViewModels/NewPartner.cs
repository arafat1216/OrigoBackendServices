using System;

namespace Customer.API.ViewModels
{
    public class NewPartner
    {
        public Guid OrganizationId { get; set; }
        public Guid CallerId { get; set; }
    }
}
