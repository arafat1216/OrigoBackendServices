using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Conmodo.ApiModels
{
    internal class Delivery
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("postCity")]
        public string? PostCity { get; set; }

        [JsonPropertyName("postNumber")]
        public string? PostNumber { get; set; }


        /// <summary>
        ///     System-reserved constructor. Should not be used!
        /// </summary>
        [Obsolete("Reserved constructor for the JSON serializers.", error: true)]
        public Delivery()
        {
        }


        public Delivery(string address1, string? address2, string postalCode, string city)
        {
            if (string.IsNullOrEmpty(address2))
                Address = address1;
            else
                Address = $"{address1}\n{address2}";

            PostNumber = postalCode;
            PostCity = city;
        }
    }
}
