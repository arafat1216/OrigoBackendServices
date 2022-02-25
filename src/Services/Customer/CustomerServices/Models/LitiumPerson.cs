using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CustomerServices.Models
{
    [Serializable]
    public class LitiumPerson
    {
        [JsonPropertyName("firstname")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastname")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("phonenumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("organizationRoles")]
        public List<LitiumOrganizationRole> OrganizationRoles { get; set; } = new List<LitiumOrganizationRole>();
    }
}