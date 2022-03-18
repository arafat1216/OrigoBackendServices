using System;

namespace Customer.API.ApiModels
{
    public class UpdateLocation
    {
        public Guid LocationId { get; set; }
        public Guid CallerId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
