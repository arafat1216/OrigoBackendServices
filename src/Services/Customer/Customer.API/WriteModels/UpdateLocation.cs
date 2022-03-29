﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Customer.API.WriteModels
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

        /// <summary>
        ///     The country, using the <c>ISO 3166-1 (alpha-2)</c> standard.
        /// </summary>
        [RegularExpression("^[A-Z]{2}")] // Exactly 2 uppercase characters
        public string Country { get; set; }
    }
}
