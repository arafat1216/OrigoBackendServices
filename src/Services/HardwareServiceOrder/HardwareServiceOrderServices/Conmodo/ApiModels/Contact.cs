using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    /// <summary>
    ///     Contact information
    /// </summary>
    internal class Contact
    {
        /// <summary>
        ///     If left blank, will always create new contact in Conmodo's system"
        /// </summary>
        [JsonPropertyName("externalContactId")]
        public string? ExternalContactId { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        // TODO: Should this be altered to a DateTime or Date?
        [JsonPropertyName("birthday")]
        public DateTimeOffset? Birthday { get; set; }

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


    }
}
