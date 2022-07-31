namespace OrigoApiGateway.Models.BackendDTO
{
    public class PendingBuyoutDeviceDTO
    {
        public Guid AssetLifeCycleId { get; set; }
        public DateTime LasWorkingDay { get; set; }
        public EmailPersonAttributeDTO User { get; set; }
        public Guid CallerId { get; set; }
    }
}
