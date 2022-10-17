
using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.ServiceModels
{
    public class SimCardAddressRequestDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        [MaxLength(2)]
        public string? Country { get; set; }
    }
}
