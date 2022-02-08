using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    [Serializable]
    public class LitiumPerson
    {
        [JsonProperty("firstname")]
        public string FirstName { get; set; } = string.Empty;

        [JsonProperty("lastname")]
        public string LastName { get; set; } = string.Empty;

        [JsonProperty("phonenumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonProperty("email")]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("organizationRoles")]
        public List<LitiumOrganizationRole> OrganizationRoles { get; set; } = new List<LitiumOrganizationRole>();
    }
}
