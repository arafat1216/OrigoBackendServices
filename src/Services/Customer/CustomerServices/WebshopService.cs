using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CustomerServices.Models;
using Microsoft.Extensions.Options;

namespace CustomerServices
{
    public class WebshopService : IWebshopService
    {
        private readonly IOktaServices _oktaServices;
        private readonly WebshopConfiguration _webshopConfig;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        private string Token { get; set; } = string.Empty;
        private DateTime TokenExpireTime { get; set; } = DateTime.UtcNow;

        public WebshopService(IOktaServices oktaServices, IOptions<WebshopConfiguration> config)
        {
            _oktaServices = oktaServices;
            _webshopConfig = config.Value;
            _jsonSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        }

        /// <summary>
        /// Enforces the +47 country code on all phone-numbers. If the alternative number 0047 is used, it is replaced.
        /// </summary>
        /// <param name="phoneNumber">The number we want to enforce</param>
        /// <returns>The phone-number with the enforced country code</returns>
        private string EnforcePhoneNumberCountryCode(string phoneNumber, string countryCode = "+47")
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return "";

            phoneNumber = phoneNumber.Trim();

            if (countryCode.Length == 4 && countryCode.StartsWith("004"))
                countryCode = "+" + countryCode.Substring(2, 2);

            if (countryCode.Length != 3 || !countryCode.StartsWith("+4"))
                countryCode = "+47";

            if (phoneNumber.StartsWith("004"))
                phoneNumber = phoneNumber.Substring(4);

            if (!phoneNumber.StartsWith("+"))
                phoneNumber = countryCode + phoneNumber;

            return phoneNumber;
        }

        public async Task<string> GetLitiumTokenAsync()
        {
            if (TokenExpireTime >= DateTime.UtcNow.AddMinutes(3))
            {
                return Token;
            }

            using (HttpClient client = new HttpClient())
            {
                var clientSecret = _webshopConfig.ClientSecret;
                var clientId = _webshopConfig.ClientId;
                var content = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret }
                });
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                try
                {
                    var resMsg = client.PostAsync(_webshopConfig.AccessTokenUri, content).GetAwaiter().GetResult();

                    if (!resMsg.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    string json = await resMsg.Content.ReadAsStringAsync();
                    var deserializedToken = JsonSerializer.Deserialize<JsonElement>(json, _jsonSerializerOptions);

                    Token = deserializedToken.GetProperty("access_token").GetString();
                    TokenExpireTime = DateTime.UtcNow.AddSeconds(deserializedToken.GetProperty("expires_in").GetInt32());
                    return Token;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public async Task<LitiumPerson> GetLitiumPersonByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var token = await GetLitiumTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var resMsg = client.GetAsync($"{_webshopConfig.PersonSearchByEmailUri}{email}").GetAwaiter().GetResult();

                try
                {
                    string json = await resMsg.Content.ReadAsStringAsync();
                    LitiumPerson person = JsonSerializer.Deserialize<LitiumPerson>(json, _jsonSerializerOptions);
                    return person;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public async Task<LitiumOrganization> GetLitiumOrganizationByOrgnumberAsync(string orgNumber)
        {
            var token = await GetLitiumTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
                return null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var resMsg = client.GetAsync($"{_webshopConfig.OrganizationsUri}/{orgNumber}").GetAwaiter().GetResult();

                string json = await resMsg.Content.ReadAsStringAsync();
                List<LitiumOrganization> organizations = JsonSerializer.Deserialize<List<LitiumOrganization>>(json, _jsonSerializerOptions);

                // There can be multiple organizations with the same organizationNumber, for example:
                // Mytos AS
                // Mytos AS (MaaS)
                // ... we are adding code to prioritize maas/flow organizations, if there are multiple returned.
                // If there are still multiple results after all validations, we select the first result as default.

                if (organizations == null || organizations.Count == 0)
                {
                    return null;
                }
                else if (organizations.Count == 1)
                {
                    return organizations.First();
                }

                var maasOrganizations = organizations.Where(o => o.OrganizationName.ToLower()
                    .Contains("maas")
                    || o.OrganizationName.ToLower().Contains("flow")
                    || o.OrganizationName.ToLower().Contains("prioritet")
                );

                if (maasOrganizations.Any())
                {
                    return maasOrganizations.First();
                }

                return organizations.First();
            }
        }


        public async Task<HttpResponseMessage> PostLitiumPerson(LitiumPerson person)
        {
            string token = await GetLitiumTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
                return null;

            // if (string.IsNullOrWhiteSpace(person.PhoneNumber))
            //     throw new ArgumentException("The phone-number is missing for the provided user.");

            person.PhoneNumber = EnforcePhoneNumberCountryCode(person.PhoneNumber);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                foreach (var role in person.OrganizationRoles)
                {
                    role.Organization.LegalRegistrationNumber = role.Organization.LegalRegistrationNumber.Trim().Replace("-", "");
                }

                var personContent = new StringContent(JsonSerializer.Serialize(person), Encoding.UTF8, "text/json");
                var resMsg = await client.PostAsync(_webshopConfig.PostPersonUri, personContent);

                if (!resMsg.IsSuccessStatusCode)
                {
                    throw new ArgumentException($"Webshop error: could not post person {person.Email}");
                }

                return resMsg;
            }
        }

        public async Task CheckAndProvisionWebShopUserAsync(string email)
        {
            try
            {
                var oktaUser = await _oktaServices.GetOktaUserProfileByLoginEmailAsync(email);

                if (oktaUser == null)
                    throw new ArgumentException("General request towards web shop failed.");

                if (string.IsNullOrEmpty(oktaUser.Profile?.OrganizationNumber))
                    throw new ArgumentException("User missing organization number.");

                LitiumOrganization organization = await GetLitiumOrganizationByOrgnumberAsync(oktaUser.Profile.OrganizationNumber);
                if (organization == null)
                    throw new ArgumentException("Okta Organization not found in webshop");

                LitiumPerson person = await GetLitiumPersonByEmail(email);
                if (person == null)
                {
                    person = new LitiumPerson()
                    {
                        FirstName = oktaUser.Profile.FirstName,
                        LastName = oktaUser.Profile.LastName,
                        PhoneNumber = oktaUser.Profile.MobilePhone,
                        Email = oktaUser.Profile.Email,
                        OrganizationRoles = new List<LitiumOrganizationRole>()
                    };

                    person.OrganizationRoles.Add(new LitiumOrganizationRole()
                    {
                        Organization = organization,
                        Role = "Ansatt"
                    });

                    await PostLitiumPerson(person);
                }

                var isRoleCorrectlySetup = person.OrganizationRoles.Any(x => x.Organization.LegalRegistrationNumber == organization.LegalRegistrationNumber && x.Role == "Ansatt");

                if (!isRoleCorrectlySetup)
                {
                    LitiumOrganizationRole newOrganizationRole = new LitiumOrganizationRole()
                    {
                        Organization = organization,
                        Role = "Ansatt"
                    };
                    var idx = person.OrganizationRoles.FindIndex(x => x.Organization.LegalRegistrationNumber == organization.LegalRegistrationNumber);

                    if (idx < 0)
                    {
                        person.OrganizationRoles.Add(newOrganizationRole);
                    }
                    else
                    {
                        person.OrganizationRoles[idx] = newOrganizationRole;
                    }

                    await PostLitiumPerson(person);
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
