namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public record UpdateOperator 
    { 
        public string OperatorName { get; set; }
        public string Country { get; set; }
    }
}
