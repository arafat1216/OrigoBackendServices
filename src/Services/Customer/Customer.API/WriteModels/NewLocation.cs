using System;
using System.ComponentModel.DataAnnotations;

namespace Customer.API.WriteModels
{
    /// <summary>
    ///     Location model to Create location
    /// </summary>
    public class NewLocation
    {
        /// <summary>
        ///     The Name of the Location.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        ///     A detailed Description abouht the Location.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        ///     Address line 1 for the location
        /// </summary>
        public string Address1 { get; set; } = string.Empty;

        /// <summary>
        ///     Address line 2 for the location
        /// </summary>
        public string Address2 { get; set; } = string.Empty;

        /// <summary>
        ///     Area Postal Code of the location
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        ///     City name of the location
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        ///     Set to make Primary location for the Organization
        /// </summary>
        public bool IsPrimary { get; set; } = false;

        /// <summary>
        ///     Guid ID of the caller
        /// </summary>
        public Guid CallerId { get; set; } = Guid.Empty;

        /// <summary>
        ///     Internal backing field for <see cref="Country"/>.
        /// </summary>
        private string _country;

        /// <summary>
        ///     The country, using the uppercase <c>ISO 3166-1 (alpha-2)</c> standard.
        /// </summary>
        /// <example> US </example>
        [RegularExpression("^[a-zA-Z]{2}")] // Exactly 2 characters
        [Required]
        public string Country
        {
            get { return _country; }
            set { _country = value?.ToUpper(); }
        }
    }
}
