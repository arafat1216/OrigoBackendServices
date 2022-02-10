using CustomerServices.Exceptions;
using CustomerServices.ServiceModels;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices
{
    public class OktaServices : IOktaServices
    {
        private readonly OktaConfiguration _oktaOptions;

        public OktaServices(IOptions<OktaConfiguration> options)
        {
            _oktaOptions = options.Value;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        }

        /// <summary>
        /// Give a newly created user Origo access via okta
        /// </summary>
        /// <param name="mytosSubsGuid"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        /// <param name="mobilePhone"></param>
        /// <param name="activate"></param>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public async Task<string> AddOktaUserAsync(Guid? mytosSubsGuid, string firstName, string lastName, string email, string mobilePhone, bool activate, string countryCode = "+47")
        {
            // Group to add user to ( and by extension - assign to OrigoV2 app)
            string[] groupIds = new string[] { _oktaOptions.OktaGroupId };

            if (null == mytosSubsGuid)
                throw new OktaException("New Okta users needs to have a valid SubsId", HttpStatusCode.BadRequest);

            email = email.Trim();
            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (string.IsNullOrWhiteSpace(email))
                throw new OktaException("The new Okta user don't have a email", HttpStatusCode.BadRequest);

            var displayName = $"{firstName} {lastName}";
            string login = email;

            var data = new
            {
                profile = new
                {
                    firstName,
                    lastName,
                    displayName,
                    email,
                    login,
                    mobilePhone = EnforcePhoneNumberCountryCode(mobilePhone, countryCode),
                },
                groupIds
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", ("SSWS " + _oktaOptions.OktaAuth));
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                var resMsg = await client.PostAsync(_oktaOptions.OktaUrl + "users?activate=" + activate.ToString(), content);
                string msg = await resMsg.Content.ReadAsStringAsync();
                if (resMsg.IsSuccessStatusCode)
                {
                    var jMsg = JObject.Parse(msg);
                    return jMsg["id"].ToString();
                }
                else
                {
                    throw new OktaException(resMsg.ReasonPhrase, resMsg.StatusCode);
                }
            }
        }

        public async Task RemoveUserFromGroup(string userOktaId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", ("SSWS " + _oktaOptions.OktaAuth));
                string url = _oktaOptions.OktaUrl + "groups/" + _oktaOptions.OktaGroupId + "/users/" + userOktaId;
                var resMsg = await client.DeleteAsync(url);
                string msg = await resMsg.Content.ReadAsStringAsync();
                if (resMsg.IsSuccessStatusCode)
                {
                    // if user has no other apps, erase the user
                    bool eraseUser = !await UserHasAppLinks(userOktaId);
                    if (eraseUser)
                    {
                        await DeactivateUserInOkta(userOktaId);
                        await DeleteUserInOkta(userOktaId);
                    }
                    return;
                }
                else
                {
                    throw new OktaException(resMsg.ReasonPhrase, resMsg.StatusCode);
                }
            }
        }

        public async Task DeactivateUserInOkta(string userOktaId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", ("SSWS " + _oktaOptions.OktaAuth));
                string url = _oktaOptions.OktaUrl + "users/" + userOktaId + "/lifecycle/deactivate";
                var resMsg = await client.PostAsync(url, null);
                string msg = await resMsg.Content.ReadAsStringAsync();
                if (resMsg.IsSuccessStatusCode)
                {
                    return;
                }
                else
                {
                    throw new OktaException(resMsg.ReasonPhrase, resMsg.StatusCode);
                }
            }
        }

        public async Task DeleteUserInOkta(string userOktaId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", ("SSWS " + _oktaOptions.OktaAuth));
                string url = _oktaOptions.OktaUrl + "users/" + userOktaId;
                var resMsg = await client.DeleteAsync(url);
                string msg = await resMsg.Content.ReadAsStringAsync();
                if (resMsg.IsSuccessStatusCode)
                {
                    return;
                }
                else
                {
                    throw new OktaException(resMsg.ReasonPhrase, resMsg.StatusCode);
                }
            }
        }

        public async Task AddUserToGroup(string userOktaId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", ("SSWS " + _oktaOptions.OktaAuth));
                string url = _oktaOptions.OktaUrl + "groups/" + _oktaOptions.OktaGroupId + "/users/" + userOktaId;
                var resMsg = await client.PutAsync(url, null);
                string msg = await resMsg.Content.ReadAsStringAsync();
                if (resMsg.IsSuccessStatusCode)
                {
                    return;
                }
                else
                {
                    throw new OktaException(resMsg.ReasonPhrase, resMsg.StatusCode);
                }
            }
        }

        public async Task<bool> UserExistsInOktaAsync(string userOktaId)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", ("SSWS " + _oktaOptions.OktaAuth));
            var url = _oktaOptions.OktaUrl + "users/" + userOktaId;
            var resMsg = await client.GetAsync(url);
            var msg = await resMsg.Content.ReadAsStringAsync();
            return resMsg.IsSuccessStatusCode;
        }

        public async Task<OktaUserDTO> GetOktaUserProfileByLoginEmailAsync(string userLoginEmail)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", ("SSWS " + _oktaOptions.OktaAuth));

                var url = _oktaOptions.OktaUrl + "users/" + WebUtility.UrlEncode(userLoginEmail);
                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<OktaUserDTO>(responseContent);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UserHasAppLinks(string userOktaId)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Authorization", ("SSWS " + _oktaOptions.OktaAuth));
                string url = _oktaOptions.OktaUrl + "users/" + userOktaId + "/appLinks";
                var resMsg = await client.GetAsync(url);
                string msg = await resMsg.Content.ReadAsStringAsync();
                if (resMsg.IsSuccessStatusCode)
                {
                    return msg != "[]";  // if empty list, then user has no applink, else user has applinks
                }
                else
                {
                    throw new OktaException(resMsg.ReasonPhrase, resMsg.StatusCode);
                }
            }
        }

        /// <summary>
        /// Enforces the +47 country code on all phone-numbers. If the alternative number 0047 is used, it is replaced.
        /// </summary>
        /// <param name="phoneNumber">The number we want to enforce</param>
        /// <returns>The phone-number with the enforced country code</returns>
        private string EnforcePhoneNumberCountryCode(string phoneNumber, string countryCode)
        {
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
    }
}

