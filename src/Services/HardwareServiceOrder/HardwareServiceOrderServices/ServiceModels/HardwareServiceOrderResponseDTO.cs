using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.ServiceModels
{
    public class HardwareServiceOrderResponseDTO
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public ServiceStatusEnum Status { get; set; }
        public ServiceTypeEnum Type { get; set; }
        public Guid Owner { get; set; }
        public ServiceProviderEnum ServiceProvider { get; set; }
        public IEnumerable<ExternalServiceEventDTO> Events { get; set; }
        public Guid AssetLifecycleId { get; set; }
        public DeliveryAddressDTO DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public string ExternalServiceManagementLink { get; set; }
    }
}
