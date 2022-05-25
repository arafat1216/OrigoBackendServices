using Common.Converters;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Contact information
    /// </summary>
    internal class Contact
    {
        /// <summary>
        ///     If left blank, will always create new contact in Conmodo's system. <para>
        ///     
        ///     For <see cref="CreateOrderRequest.CustomerHandler">customerHandler</see> this is used to identify who handled the service.
        ///     Can be set at fixed value if this is not something the needs to kept track of. </para>
        /// </summary>
        [JsonPropertyName("externalContactId")]
        public string? ExternalContactId { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        // TODO: Should this be DateOnly, DateTime or DateTimeOffset?
        [JsonConverter(typeof(DateOnlyNullableJsonConverter))]
        [JsonPropertyName("birthday")]
        public DateOnly? Birthday { get; set; }

        [JsonPropertyName("companyName")]
        public string? CompanyName { get; set; }

        [JsonPropertyName("organizationNumber")]
        public string? OrganizationNumber { get; set; }

        [JsonPropertyName("cellPhoneNumber")]
        public string? CellPhoneNumber { get; set; }

        [JsonPropertyName("homePhoneNumber")]
        public string? HomePhoneNumber { get; set; }

        [JsonPropertyName("jobPhoneNumber")]
        public string? JobPhoneNumber { get; set; }

        [JsonPropertyName("faxNumber")]
        public string? FaxNumber { get; set; }

        /// <summary>
        ///     All correspondence from Conmodo is sent to this email address
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example> "SE" </example>
        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("postNumber")]
        public string? PostNumber { get; set; }

        [JsonPropertyName("postCity")]
        public string? PostCity { get; set; }

        /// <summary>
        ///     The address the device was sent from (and will be returned to if 'return to customer' is registered as an extra service).
        /// </summary>
        [JsonPropertyName("delivery")]
        public Delivery? Delivery { get; set; }


        /// <summary>
        ///     System-reserved constructor. Should not be used!
        /// </summary>
        [Obsolete("Reserved constructor for the JSON serializers.", error: true)]
        public Contact()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Contact"/> class intended for use in <see cref="CreateOrderRequest.Owner"/>.
        ///     This is used for personal/user-attached devices.
        /// </summary>
        /// <param name="externalContactId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="cellPhoneNumber"></param>
        /// <param name="deliveryAddress"></param>
        /// <param name="country"></param>
        public Contact(string externalContactId, string firstName, string lastName, string email, string? cellPhoneNumber, Delivery deliveryAddress, string country)
        {
            ExternalContactId = externalContactId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            CellPhoneNumber = cellPhoneNumber;
            Delivery = deliveryAddress;
            Country = country;
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="Contact"/> class intended for use in <see cref="CreateOrderRequest.Owner"/>.
        ///     This is used for non-personal/functional devices.
        /// </summary>
        /// <param name="externalContactId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="companyName"></param>
        /// <param name="organizationNumber"></param>
        /// <param name="email"></param>
        /// <param name="cellPhoneNumber"></param>
        /// <param name="deliveryAddress"></param>
        /// <param name="country"></param>
        public Contact(string externalContactId, string firstName, string lastName, string companyName, string organizationNumber, string email, string? cellPhoneNumber, Delivery deliveryAddress, string country)
        {
            ExternalContactId = externalContactId;
            FirstName = firstName;
            LastName = lastName;
            CompanyName = companyName;
            OrganizationNumber = organizationNumber;
            Email = email;
            CellPhoneNumber = cellPhoneNumber;
            Delivery = deliveryAddress;
            Country = country;
        }



        /// <summary>
        ///     Initializes a new instance of the <see cref="Contact"/> class intended for use in <see cref="CreateOrderRequest.CustomerHandler"/>.
        /// </summary>
        /// <param name="externalContactId"></param>
        /// <param name="companyName"></param>
        /// <param name="organizationNumber"></param>
        public Contact(string externalContactId, string companyName, string organizationNumber)
        {
            ExternalContactId = externalContactId;
            CompanyName = companyName;
            OrganizationNumber = organizationNumber;
        }
    }
}
