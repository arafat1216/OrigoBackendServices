﻿using Common.Enums;
using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class LocationDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public RecipientType RecipientType { get; set; }

    }
}
