namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoActivateSimOrder
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string SimNumber { get; set; }
        public string SimType { get; set; }
    }
}
