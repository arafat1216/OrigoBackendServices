using System.ComponentModel.DataAnnotations;

namespace CustomerServices.ServiceModels
{
    public class LocationDTO
    {
        public LocationDTO(CustomerServices.Models.Location organizationLocation)
        {
            Name = organizationLocation.Name;
            Description = organizationLocation.Description;
            Address1 = organizationLocation.Address1;
            Address2 = organizationLocation.Address2;
            PostalCode = organizationLocation.PostalCode;
            City = organizationLocation.City;
            Country = organizationLocation.Country;
        }

        public LocationDTO() { }

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
