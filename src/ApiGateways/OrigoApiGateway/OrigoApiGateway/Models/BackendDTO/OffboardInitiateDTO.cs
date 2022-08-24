namespace OrigoApiGateway.Models.BackendDTO
{
    public class OffboardInitiateDTO
    {
        public DateTime LastWorkingDay { get; set; }
        public bool BuyoutAllowed { get; set; } = false;
        public Guid CallerId { get; set; }
    }
}
