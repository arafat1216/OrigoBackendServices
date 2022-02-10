namespace CustomerServices
{
    /// <summary>
    /// Webshop settings are in Customer.API.Appsettings. This class is set to those values, and injected into the WebshpServices class,
    /// which allows WebshpServices access to those values, despite being in a different project.
    /// </summary>
    public class WebshopConfiguration
    {
        public string ClientSecret { get; set; }

        /// <summary>
        /// Endpoint to submit to client secret and get access token
        /// </summary>
        public string AccessTokenUri { get; set; }

        /// <summary>
        /// Endpoint to organization roles of webshop user 
        /// </summary>
        public string RolesUri { get; set; }

        /// <summary>
        /// Endpoint to get organization of webshop user 
        /// </summary>
        public string OrganizationsUri { get; set; }

        /// <summary>
        /// Endpoint to search webshop user by phonenumber 
        /// </summary>
        public string PersonSearchByPhoneNumberUri { get; set; }

        /// <summary>
        /// Endpoint to search webshop user by email 
        /// </summary>
        public string PersonSearchByEmailUri { get; set; }

        /// <summary>
        /// Endpoint to create new webshop user 
        /// </summary>
        public string PostPersonUri { get; set; }
    }
}