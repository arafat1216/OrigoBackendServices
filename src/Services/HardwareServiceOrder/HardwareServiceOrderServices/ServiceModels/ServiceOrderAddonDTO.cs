using HardwareServiceOrderServices.Models;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <inheritdoc cref="ServiceOrderAddon"/>
    public class ServiceOrderAddonDTO
    {
        /// <inheritdoc cref="ServiceOrderAddon.Id"/>
        public int Id { get; init; }

        /// <inheritdoc cref="ServiceOrderAddon.ThirdPartyId"/>
        [MaxLength(256)]
        [JsonIgnore]
        public string ThirdPartyId { get; set; }

        /// <inheritdoc cref="ServiceOrderAddon.ServiceProviderId"/>
        public int ServiceProviderId { get; set; }

        /// <inheritdoc cref="ServiceOrderAddon.IsUserSelectable"/>
        public bool IsUserSelectable { get; set; }

        /// <inheritdoc cref="ServiceOrderAddon.IsCustomerTogglable"/>
        public bool IsCustomerTogglable { get; set; }
    }
}
