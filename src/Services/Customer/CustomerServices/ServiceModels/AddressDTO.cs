using CustomerServices.Models;
using System.ComponentModel.DataAnnotations;

namespace CustomerServices.ServiceModels
{
    public record AddressDTO
    {
        public AddressDTO(Address customerCompanyAddress)
        {
            Street = (customerCompanyAddress == null) ? "" : customerCompanyAddress.Street;
            PostCode = (customerCompanyAddress == null) ? "" : customerCompanyAddress.PostCode;
            City = (customerCompanyAddress == null) ? "" : customerCompanyAddress.City;
            Country = (customerCompanyAddress == null) ? "" : customerCompanyAddress.Country;
        }

        public AddressDTO(){}

        public string Street { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

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