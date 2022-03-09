using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request
{
    public record ActivateSimOrderPostRequest
    {
        public string MobileNumber { get; set;}
        public int OperatorId { get; set; }
        public string SimCardNumber { get; set; }
        public string SimCardType { get; set; }
        public Guid CallerId { get; set; }
        
        
  }
}
