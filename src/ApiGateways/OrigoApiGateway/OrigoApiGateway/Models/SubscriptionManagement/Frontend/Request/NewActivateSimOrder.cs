namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class NewActivateSimOrder
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string SimNumber { get; set; }
        public string SimType { get; set; }
    }
}
