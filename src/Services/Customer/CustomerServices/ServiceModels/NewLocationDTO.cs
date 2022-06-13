using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.ServiceModels
{
    public class NewLocationDTO
    {
        public string Name { get; init; }
        public string Description { get; init; }
        public string Address1 { get; init; }
        public string Address2 { get; init; }
        public string PostalCode { get; init; }
        public string City { get; init; }
        public string Country { get; init; }
        public bool IsPrimary { get; init; }
    }
}
