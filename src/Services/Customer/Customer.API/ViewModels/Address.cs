using System.ComponentModel.DataAnnotations;

namespace Customer.API.ViewModels
{
    public record Address
    {
        public Address(CustomerServices.Models.Address customerCompanyAddress)
        {
            Street = (customerCompanyAddress == null) ? "" : customerCompanyAddress.Street;
            PostCode = (customerCompanyAddress == null) ? "" : customerCompanyAddress.PostCode;
            City = (customerCompanyAddress == null) ? "" : customerCompanyAddress.City;
            Country = (customerCompanyAddress == null) ? "" : customerCompanyAddress.Country;
        }

        public Address(){}

        public string Street { get; set; }

        public string PostCode { get; set; }

        public string City { get; set; }

        /// <summary>
        ///     The country, using the <c>ISO 3166-1 (alpha-2)</c> standard.
        /// </summary>
        [RegularExpression("^[a-z]{2}")] // Exactly 2 lowercase characters
        public string Country { get; set; }
    }
}