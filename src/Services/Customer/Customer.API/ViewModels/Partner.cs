using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.ViewModels
{
    public record Partner
    { 
        public Guid ExternalId { get; set; }
        public Organization Organization { get; set; }
    }
}
