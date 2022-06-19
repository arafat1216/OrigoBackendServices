using System.ComponentModel.DataAnnotations;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public class DeliveryAddress
    {
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
        ///     </list><para>
        ///     
        ///     Please note that not all providers utilizes this field.</para>
        /// </summary>
        public string? Address2 { get; set; }

        /// <summary>
        ///     The addresses zip/postal code.
        /// </summary>
        [Required]
        [StringLength(maximumLength: 12)] // The longest worldwide postal-code.
        public string PostalCode { get; set; }

        /// <summary>
        ///     The name of the city/town/village.
        /// </summary>
        [StringLength(maximumLength: 85)] // The longest worldwide place/city name
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
