
namespace SubscriptionManagementServices.ServiceModels
{
    public class NewCustomerStandardBusinessSubscriptionProduct
    {
        public int OperatorId { get; set; }
        public string SubscriptionName { get; set; } = string.Empty;
        public string? DataPackage { get; set; }
        public Guid CallerId { get; set; }
        public IList<string> AddOnProducts { get; set; } = new List<string>();
    }
}
