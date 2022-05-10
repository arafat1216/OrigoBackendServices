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
        ///     If left blank, will always create new contact in Conmodo's system
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

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("postNumber")]
        public string? PostNumber { get; set; }

        [JsonPropertyName("postCity")]
        public string? PostCity { get; set; }

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
        /// </summary>
        /// <param name="externalContactId"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="cellPhoneNumber"></param>
        /// <param name="deliveryAddress"></param>
        public Contact(string externalContactId, string firstName, string lastName, string email, string cellPhoneNumber, Delivery deliveryAddress)
        {
            ExternalContactId = externalContactId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            CellPhoneNumber = cellPhoneNumber;
            Delivery = deliveryAddress;
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
