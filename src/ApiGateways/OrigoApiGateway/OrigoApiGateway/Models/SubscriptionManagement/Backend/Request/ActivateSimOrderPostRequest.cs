namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request
{
    public record ActivateSimOrderPostRequest
    {
        public string MobileNumber { get; set;}
        public int OperatorId { get; set; }
        public string SimNumber { get; set; }
        public string SimType { get; set; }
        
        
  }
}
