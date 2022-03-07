namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request
{
    public class CustomerReferenceValuePostRequestDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string CallerId { get; set; }
    }
}