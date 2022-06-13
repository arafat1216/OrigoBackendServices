using Common.Enums;
using CustomerServices.Models;
using System;
using System.ComponentModel.DataAnnotations;

#nullable enable

namespace CustomerServices.ServiceModels
{
    public class LocationDTO
    {
        public LocationDTO() { }

        public LocationDTO(Location organizationLocation)
        {
            Id = organizationLocation.ExternalId;
            Name = organizationLocation.Name;
            Description = organizationLocation.Description;
            Address1 = organizationLocation.Address1;
            Address2 = organizationLocation.Address2;
            PostalCode = organizationLocation.PostalCode;
            City = organizationLocation.City;
            Country = organizationLocation.Country;
            RecipientType = organizationLocation.RecipientType;
            IsPrimary = organizationLocation.IsPrimary;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public string? Description { get; set; }

        public string Address1 { get; set; }

        public string? Address2 { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }
        public bool IsPrimary { get; set; } = false;
        public RecipientType RecipientType { get; set; }


        /// <summary>
        ///     Internal backing field for <see cref="Country"/>.
        /// </summary>
        private string _country;

        /// <summary>
        ///     The country, using the uppercase <c>ISO 3166-1 (alpha-2)</c> standard.
        /// </summary>
        /// <example> US </example>
        [RegularExpression("^[a-zA-Z]{2}")] // Exactly 2 characters
        public string Country
        {
            get { return _country; }
            set { _country = value.ToUpper(); }
        }
    }
}
