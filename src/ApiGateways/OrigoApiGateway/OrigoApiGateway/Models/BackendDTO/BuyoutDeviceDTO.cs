namespace OrigoApiGateway.Models.BackendDTO
{
    public class BuyoutDeviceDTO
    {
        public Guid AssetLifeCycleId { get; set; }

        [EmailAddress]
        public string PayrollContactEmail { get; set; } = string.Empty;

        public Guid CallerId { get; set; }
    }
}
