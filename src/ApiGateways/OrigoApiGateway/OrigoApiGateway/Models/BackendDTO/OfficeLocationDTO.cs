﻿using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class OfficeLocationDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool IsPrimary { get; set; } = false;
        public Guid CallerId { get; set; } = Guid.Empty;
    }
}
