﻿
namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Request object
    /// </summary>
    public class NewLocation
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        [MaxLength(2)]
        public string Country { get; set; }
    }
}
