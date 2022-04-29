﻿namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public class DeliveryAddress
    {
        public string Recipient { get; set; }

        public string Address1 { get; set; }

        public string? Address2 { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public string Country { get; set; }
    }
}