using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.ServiceModels
{
    public class PrivateSubscriptionDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PostalPlace { get; set; }
        [MaxLength(2)]
        public string? Country { get; set; }
        [MaxLength(320)]
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? OperatorName { get; set; }
        public PrivateSubscriptionDTO? RealOwner { get; set; }
    }
}
