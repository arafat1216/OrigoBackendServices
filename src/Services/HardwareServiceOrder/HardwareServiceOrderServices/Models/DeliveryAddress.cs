namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single shipping address.
    /// </summary>
    // See: https://www.posten.no/sende/adressering
    // See: https://thegood.com/insights/address-line-2/
    // See: https://incorporated.zone/address-line-2/
    public class DeliveryAddress
    {
        /// <summary>
        ///     Added to prevent Entity framework No suitable constructor found exception.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private DeliveryAddress()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DeliveryAddress"/>-class.
        /// </summary>
        /// <param name="recipientType"> A discriminator that tells us what kind of address this is. </param>
        /// <param name="recipient"> The name of the recipient. Typically this will be the name of a person or company. </param>
        /// <param name="address1"> The primary address information (street address information). </param>
        /// <param name="address2"> If needed, an optional secondary address line. Please note that not all providers utilizes this field! </param>
        /// <param name="postalCode"> The addresses zip/postal code. </param>
        /// <param name="city"> The name of the city/town/village. </param>
        /// <param name="country"> The 2-character country-code using the uppercase <c>ISO 3166 alpha-2</c> standard. </param>
        public DeliveryAddress(RecipientTypeEnum recipientType, string recipient, string address1, string? address2, string postalCode, string city, string country)
        {
            RecipientType = recipientType;
            Recipient = recipient;
            Address1 = address1;
            Address2 = address2;
            PostalCode = postalCode;
            City = city;
            Country = country;
        }


        /// <summary>
        ///     A discriminator that tells us what kind of address this is. This is needed to the system can forward/register the correct kind if data.
        /// </summary>
        public RecipientTypeEnum RecipientType { get; set; }

        /// <summary>
        ///     The name of the recipient. Typically this will be the name of a person or company.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        ///     The primary address information (street address information).
        /// </summary>
        public string Address1 { get; set; }

        /// <summary>
        ///     If needed, an optional secondary address line. Please note that not all providers utilizes this field!
        ///
        ///     <para>
        ///     This is typically used for additional address designation that is not part of the primary address, such as:
        ///     
        ///     <list type="number">
        ///         <item>
        ///             <term>C/O</term>
        ///             <description>in care of</description>
        ///         </item>
        ///         <item>
        ///             <term>Attn</term>
        ///             <description>attention</description>
        ///         </item>
        ///         <item>
        ///             <term>Apartment number</term>
        ///         </item>
        ///         <item>
        ///             <term>Suite number</term>
        ///         </item>
        ///         <item>
        ///             <term>Unit number</term>
        ///         </item>
        ///         <item>
        ///             <term>Space number</term>
        ///         </item>
        ///         <item>
        ///             <term>Floor number</term>
        ///         </item>
        ///         <item>
        ///             <term>Room number</term>
        ///         </item>
        ///         <item>
        ///             <term>PO Box number</term>
        ///         </item>
        ///     </list></para>
        /// </summary>
        public string? Address2 { get; set; }

        /// <summary>
        ///     The addresses zip/postal code.
        /// </summary>
        /// <remarks>
        ///     The longest worldwide postal-code is 12 characters.
        /// </remarks>
        [Required]
        [StringLength(maximumLength: 12)]
        public string PostalCode { get; set; }

        /// <summary>
        ///     The name of the city/town/village.
        /// </summary>
        /// <remarks>
        ///     The longest worldwide place/city name is 85 characters.
        /// </remarks>
        [StringLength(maximumLength: 85)]
        public string City { get; set; }

        /// <summary>
        ///     The 2-character country-code using the uppercase <c>ISO 3166 alpha-2</c> standard.
        /// </summary>
        [Required]
        [RegularExpression("^[A-Z]{2}$")]
        [StringLength(maximumLength: 2, MinimumLength = 2)]
        public string Country { get; set; }

    }
}
