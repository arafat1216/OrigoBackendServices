using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.ServiceModels
{
    public class HardwareServiceOrderDTO
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public string FaultType { get; set; }
        public string BasicDescription { get; set; }
        public string UserDescription { get; set; }
        public string? ExternalProviderLink { get; set; }
        public ServiceType ServiceType { get; set; }
        public ServiceStatus ServiceStatus { get; set; }
        public ServiceProvider ServiceProvider { get; set; }
        public DeliveryAddressDTO DeliveryAddress { get; set; }

        public Guid UserId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string Email { get; set; }

        public Guid OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public string? OrganizationNumber { get; set; }

        public Guid PartnerId { get; set; }

        public string PartnerName { get; set; }

        public string PartnerOrganizationNumber { get; set; }

        public string ErrorDescription { get; set; }
    }
}
