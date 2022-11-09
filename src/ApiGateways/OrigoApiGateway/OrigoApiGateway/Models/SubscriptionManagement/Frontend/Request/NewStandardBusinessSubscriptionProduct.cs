namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class NewStandardBusinessSubscriptionProduct
    {
        [Required]
        public int OperatorId { get; set; }
        [Required]
        public string SubscriptionName { get; set; } = string.Empty;
        public string? DataPackage { get; set; }
        public IList<string> AddOnProducts { get; set; } = new List<string>();
    }
}
