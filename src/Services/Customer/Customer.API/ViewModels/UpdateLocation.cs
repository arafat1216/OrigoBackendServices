using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.ViewModels
{
    public class UpdateLocation
    {
        public Guid LocationId { get; protected set; }
        public Guid CallerId { get; protected set; }

        public string Name { get; protected set; }
        public string? Description { get; protected set; }
        public string? Address1 { get; protected set; }
        public string? Address2 { get; protected set; }
        public string? PostalCode { get; protected set; }
        public string? City { get; protected set; }
        public string? Country { get; protected set; }
    }
}
