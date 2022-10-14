namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class NewActivateSimOrder
    {
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        [MaxLength(22)]
        public string SimCardNumber { get; set; }
        public string SimCardType { get; set; }
    }
}
